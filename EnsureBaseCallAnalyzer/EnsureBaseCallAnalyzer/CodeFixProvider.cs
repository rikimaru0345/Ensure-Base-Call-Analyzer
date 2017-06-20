using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Rename;
using Microsoft.CodeAnalysis.Text;

namespace EnsureBaseCallAnalyzer
{
	
	//[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(EnsureBaseCallAnalyzerCodeFixProvider)), Shared]
	//public class EnsureBaseCallAnalyzerCodeFixProvider : CodeFixProvider
	//{
	//	private const string title = "Call base method";

	//	public sealed override ImmutableArray<string> FixableDiagnosticIds
	//	{
	//		get { return ImmutableArray.Create(EnsureBaseCallAnalyzerAnalyzer.DiagnosticId); }
	//	}

	//	public sealed override FixAllProvider GetFixAllProvider()
	//	{
	//		// See https://github.com/dotnet/roslyn/blob/master/docs/analyzers/FixAllProvider.md for more information on Fix All Providers
	//		return WellKnownFixAllProviders.BatchFixer;
	//	}

	//	public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
	//	{
	//		var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
			
	//		var diagnostic = context.Diagnostics.First();
	//		var diagnosticSpan = diagnostic.Location.SourceSpan;

	//		// Find the type declaration identified by the diagnostic.
	//		var declaration = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<TypeDeclarationSyntax>().First();

	//		// Register a code action that will invoke the fix.
	//		/*
	//		 * context.RegisterCodeFix(
	//			CodeAction.Create(
	//				title: title,
	//				createChangedSolution: c => MakeUppercaseAsync(context.Document, declaration, c),
	//				equivalenceKey: title),
	//			diagnostic);
	//		*/
	//	}
		
	//}
	
}