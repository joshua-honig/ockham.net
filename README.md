# ockham.net
The simplest solution to basic .Net needs

## Purpose

To solve frequent .Net needs in simple, standards-conformant ways

## Principles

 - Every Ockham module should solve a clear problem that is not solved in the .Net BCL, or in the particular libraries it is meant to augment.
 - Every Ockham module shall support [.Net Standard](https://docs.microsoft.com/en-us/dotnet/standard/net-standard) 1.3+. At the moment we are specifically compiling against .Net 4.5, [.Net Standard 1.3](https://github.com/dotnet/standard/blob/master/docs/versions/netstandard1.3.md), and [.Net Standard 2.0](https://github.com/dotnet/standard/blob/master/docs/versions/netstandard2.0.md). Specific modules may target other specific versions where appropriate (such as .Net 4.6, .Net 4.7), but only in addition to support of netstandard1.3 and netstandard2.0
 - Ockham modules will follow the same naming and general pattern conventions as established in the [dotnet/corefx](https://github.com/dotnet/corefx) project
 - Ockham modules will have 100% code coverage with [Xunit](https://xunit.github.io/) before publishing release versions
 
 
 ## Modules
 
  |Module|Description|
  |------|-----------|
  |**[Ockham.Data](https://github.com/joshua-honig/ockham.net/tree/master/src/Ockham.Data)**|Basic data conversion and type inspection utilities|
  |**[Ockham.Test](https://github.com/joshua-honig/ockham.net/tree/master/src/Ockham.Test)**|A small set of framework-agnostic unit testing utilities|
