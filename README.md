# Ensure-Base-Call-Analyzer
  A roslyn code analyzer to check if override methods call their base implementation.


__**How to use**__

1) Compile and install the VSIX
2) Create an attribute in your code somewhere like this: `class EnsureBaseCallAttribute : Attribute { }`
3) Add the attribute to any method you want checked and the analyzer will show an error if you do not call the base implementation of that method (or if it is unreachable). Example:

![example usage](http://i.imgur.com/tDhRosA.png)
