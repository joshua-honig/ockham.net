# ockham.net
The simplest solution to basic .Net needs

## Purpose

To solve frequent .Net needs in simple, standards-conformant ways

## Principles

 - Every Ockham module should solve a clear problem that is not solved in the .Net BCL, or in the particular libraries it is meant to augment.
 - Every Ockham module shall support [.Net Standard](https://docs.microsoft.com/en-us/dotnet/standard/net-standard) 1.3+. At the moment we are specifically compiling against .Net 4.5, [.Net Standard 1.3](https://github.com/dotnet/standard/blob/master/docs/versions/netstandard1.3.md), and [.Net Standard 2.0](https://github.com/dotnet/standard/blob/master/docs/versions/netstandard2.0.md). Specific modules may target other specific versions where appropriate (such as .Net 4.6, .Net 4.7), but only in addition to support of netstandard1.3 and netstandard2.0
 - Ockham modules will follow the same naming and general pattern conventions as established in the [dotnet/corefx](https://github.com/dotnet/corefx) project. Specifically, see [Coding Guidelines](https://github.com/dotnet/corefx/tree/master/Documentation#coding-guidelines)
 - Ockham modules will be fully unit tested with [Xunit](https://xunit.github.io/) before publishing release versions. All tests shall pass against netcoreapp1.0, netcoreapp2.0, net45, and any other specific framework versions targeted by the applicable module.
 
## Conventions

- All Ockham modules will be built against the [MSBuild](https://github.com/Microsoft/MSBuild) version included in VS 2017 or later, or any compatible build engine, in order to use the greatly simplified project format and [succint multitargeting feature](https://blog.nuget.org/20170316/NuGet-now-fully-integrated-into-MSBuild.html).
 
 
## Modules
 
  |Module|Description|
  |------|-----------|
  |**[Ockham.Data](https://github.com/joshua-honig/ockham.net/tree/master/src/Ockham.Data)**|Basic data conversion and type inspection utilities|
  |**[Ockham.Test](https://github.com/joshua-honig/ockham.net/tree/master/src/Ockham.Test)**|A small set of framework-agnostic unit testing utilities|
