using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace EnsureBaseCallAnalyzer
{
	[DiagnosticAnalyzer(LanguageNames.CSharp)]
	public class EnsureBaseCallAnalyzerAnalyzer : DiagnosticAnalyzer
	{
		public const string DiagnosticId = "EnsureBaseCallAnalyzer";

		const string Title = "Method does not call its base implementation";
		const string Category = "Usage";
		const string Description = "This method should call its base implementation.";
		const string MessageFormat = "Method '{0}' does not invoke its base implementation";

		static DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat,
			Category, DiagnosticSeverity.Error, isEnabledByDefault: true, description: Description);

		public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get { return ImmutableArray.Create(Rule); } }

		
		public override void Initialize(AnalysisContext context)
		{
			// Full analysis would be really wasteful
			// context.RegisterSemanticModelAction( ... );

			context.RegisterSymbolAction(AnalyzeMethod, SymbolKind.Method);
		}

		private static void AnalyzeMethod(SymbolAnalysisContext context)
		{
			// 1. Get method
			// 2. only when its an override
			// 3. only when base is not abstract (virtual, or later: default-implementation)
			// 4. check if the method calls its base -> report diagnostic

			if (!(context.Symbol is IMethodSymbol method))
				return; // should never happen because SymbolAnalysis is a semantic analysis

			if (!method.IsOverride)
				return; // We are not applicable here
			
			var baseMethod = method.OverriddenMethod;
			if (baseMethod == null)
				return; // happens when: "override void AMethodThatDoesNotEvenExist()"

			if (baseMethod.IsAbstract)
				return; // We cannot call abstract bases; // todo: fix when default implementations are released


			// Before we enter the phase of expensive checks, we check if we should actually do
			if (!HasEnablingAttribute(method))
				return;// No need to check here
			
			var syntaxCalls = method.DeclaringSyntaxReferences
				.Select(r => r.GetSyntax())
				.SelectMany(node => node.DescendantNodes().OfType<InvocationExpressionSyntax>());
			
			// It seems we cannot be sure that the semantic analysis has propagated through all documents when full-solution-analysis is turned off
			// So we have to call GetSemanticModel on the specific syntax tree
			// This happens when we're dealing with 'partial'
			var semCalls = syntaxCalls.Select(callExp =>
			{
				var semanticAnalysis = context.Compilation.GetSemanticModel(callExp.SyntaxTree);
				var callSymbolInfo = semanticAnalysis.GetSymbolInfo(callExp);
				return (semanticAnalysis, callExp, callSymbolInfo);
			});
			
			foreach(var (sem, callExp, callSymbolInfo) in semCalls)
			{
				if (callSymbolInfo.Symbol == null)
					continue; // Syntax error

				var target = callSymbolInfo.Symbol as IMethodSymbol;
				if (target == null)
					continue; // wtf, breakpoint

				if (!target.Equals(baseMethod))
					continue; // Not the right method

				// We can be sure that we're calling the right method here,
				// the only thing left to check is if the call is actually reachable
				var statement = callExp.Ancestors().OfType<StatementSyntax>().FirstOrDefault();
				if (statement == null)
					continue; // wtf? #2 

				var controlFlow = sem.AnalyzeControlFlow(statement);
				if (controlFlow.Succeeded)
					if (!controlFlow.StartPointIsReachable)
						continue; // The call is there, but it will never get executed

				return; // This method calls the base, return! :)
			}
			
			// We found no call to the base, better report it!
			var diagnostic = Diagnostic.Create(Rule, context.Symbol.Locations[0], context.Symbol.Name);
			context.ReportDiagnostic(diagnostic);
		}

		static bool HasEnablingAttribute(IMethodSymbol method)
		{
			var m = method;
			while(m != null)
			{
				if (m.GetAttributes().Any(x => x.AttributeClass.Name == "EnsureBaseCallAttribute"))
					return true;

				m = m.OverriddenMethod;
			}

			return false;
		}
	}
}
