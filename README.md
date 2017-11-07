# ockham.net
The simplest solution to basic .Net needs

## Purpose

To solve frequent .Net needs in simple, standards-conformant ways

## Principles

 - Every Ockham module should solve a clear problem that is not solved in the .Net BCL, or in the particular libraries it is meant to augment.
 - **Ockham is not a framework**. Dependencies between Ockham modules shall be miminized.
 - Every Ockham module shall support [.NET Framework](https://docs.microsoft.com/en-us/dotnet/framework/) 4.6+ and [.NET Standard](https://docs.microsoft.com/en-us/dotnet/standard/net-standard) 2.0+. A module may support earlier framework or .NET Standard versions, but is not required to do so. At the moment we are specifically compiling against .Net 4.6 and [.NET Standard 2.0](https://github.com/dotnet/standard/blob/master/docs/versions/netstandard2.0.md) (net46 and netstandard2.0 target frameworks).  
 - Ockham modules will follow the same naming and general pattern conventions as established in the [dotnet/corefx](https://github.com/dotnet/corefx) project. Specifically, see [Coding Guidelines](https://github.com/dotnet/corefx/tree/master/Documentation#coding-guidelines)
 - Ockham modules will be fully unit tested with [Xunit](https://xunit.github.io/) before publishing release versions. All tests shall pass against net46, netcoreapp2.0, and any other specific framework versions targeted by the applicable module.
 - Cloning, building, and modifying Ockham modules will be really, really easy
 
## Conventions

- All Ockham modules will be built against the [MSBuild](https://github.com/Microsoft/MSBuild) version included in VS 2017 or later, or any compatible build engine, in order to use the greatly simplified project format and [succint multitargeting feature](https://blog.nuget.org/20170316/NuGet-now-fully-integrated-into-MSBuild.html#develop-against-multiple-tfms).
- Ockham modules each get their own repository, so that cloning and building is really, really easy (see Principles above)
 
## Modules
 
  |Module|GitHub Repository|Description|
  |------|-----------|---|
  |**[Ockham.NuGet](https://github.com/joshua-honig/ockham.net.nuget)**|**[ockham.net.nuget](https://github.com/joshua-honig/ockham.net.nuget)**|An easy-to-use wrapper around the official [NuGet.Client](https://github.com/NuGet/NuGet.Client) packages|
  |**[Ockham.Data](https://github.com/joshua-honig/ockham.net.test)**|**[ockham.net.data](https://github.com/joshua-honig/ockham.net.data)**|Basic data conversion and type inspection utilities|
  |**[Ockham.Test](https://github.com/joshua-honig/ockham.net.test)**|**[ockham.net.test](https://github.com/joshua-honig/ockham.net.test)**|A small set of framework-agnostic unit testing utilities|
