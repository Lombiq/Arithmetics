# Lombiq Arithmetics

[![Lombiq.Arithmetics NuGet](https://img.shields.io/nuget/v/Lombiq.Arithmetics?label=Lombiq.Arithmetics)](https://www.nuget.org/packages/Lombiq.Arithmetics/)

## About

[Next-generation arithmetic](https://posithub.org/) implementations, improved floating point number types for .NET, written in C#. Includes the following number types:

- Posit: a drop-in replacement for standard `float`s and `double`s that provides more accurate results with fewer bits. Can be used in any .NET software that needs better accuracy than `double`s. For more info see the "[Beating Floating Point at its Own Game: Posit Arithmetic](http://www.johngustafson.net/pdfs/BeatingFloatingPoint.pdf)" white paper.
- Unum: similar to posit but with a more complex implementation, a prior idea; just a basic proof of concept. For more info see the [compilation of unum resources](](http://www.johngustafson.net/unums.html) on Dr. John Gustafson's website.

This project was developed as part of [Hastlayer](https://hastlayer.com/), the .NET HLS tool that converts .NET programs into equivalent logic hardware implementations. Both the posit and unum implementation can be automatically converted into FPGA-implemented logic hardware.

We've written a detailed whitepaper about our posit implementation and its results. You can download the whitepaper for free by visiting the link found on our [Next Generation Arithmetic with Hastlayer page](https://hastlayer.com/arithmetics).

This project is developed by [Lombiq Technologies Ltd](https://lombiq.com/). Commercial-grade support is available through Lombiq.

## About unum

Unum is a new number format invented by Dr. John L. Gustafson that can be used to store any number with exact precision (or known error). It can be used to achieve better range and accuracy than IEEE floating point formats while eliminating the algebraic errors that the IEEE floats are prone to.

For more about its advantages see: [http://ubiquity.acm.org/article.cfm?id=2913029](http://ubiquity.acm.org/article.cfm?id=2913029).

## Editing generated posit code

The T4 templates are the source of truth for these generated files:

- `Posit\PositTemplate.tt` generates the `Posit8E*`, `Posit16E*`, and `Posit32E*` types.
- `Posit\PositHelper.tt` generates the integer helper overloads in `Posit\PositHelper.cs`.
- `Lombiq.Arithmetics.Tests\PositTests\FixedSizePositTests.tt` generates the corresponding `Posit*E*Tests.cs` files.

Editing generated code is **template-only**: change the owning `.tt` or shared `.ttinclude` and let the generator produce
the `.cs` outputs. Never edit generated files directly, even temporarily or with a formatter/code-fix tool. This includes
numerical fixes, API changes, test changes, formatting, and analyzer fixes: direct edits will be overwritten by the next build.
Check the projects' `ArithmeticTemplate` and `ArithmeticGeneratedFile` items for source ownership and the complete output
set, including the primary outputs of multi-file templates. `Posit32.cs` (without an E suffix), `Posit.cs`, and `Quire.cs`
are currently hand-maintained; this rule does not prohibit changes to hand-maintained C#.

Normal builds in Visual Studio, Rider, and `dotnet build` regenerate these files when a template, the shared
`CodeGeneration/TemplateFileManager.ttinclude`, or generation configuration changes, or an expected output is missing.
Unchanged builds skip template execution. Each project regenerates its own group of templates together.
The `T4.BuildTools` package is restored automatically; no extension, global tool, or machine-specific configuration is
required to build. Visual Studio also exposes regeneration through the template's **Run Custom Tool** action.
Keep the executable C# blocks in templates compatible with C# 5, as Visual Studio's MSBuild uses the .NET Framework
template compiler. This does not restrict the C# language version of the generated arithmetic code.

To explicitly run the incremental generation target, use
`dotnet msbuild Lombiq.Arithmetics.csproj -t:GenerateArithmeticSources` after restoring the project. Build the test project
to regenerate its templates as well. Generated files remain separate and checked into source control; review and commit
them together with template changes. The comment-only output of each multi-file template lists its generated files.
The templates and the generated files use Windows newlines on every platform (see `.gitattributes`), so a build doesn't
leave pending changes in Git.
If you change the supported posit environments, update both template loops and the expected output list in
`CodeGeneration/CodeGeneration.targets`.

For every generated-code change, regenerate all affected implementation and test outputs before reviewing the diff and
running the relevant tests. Confirm generation actually executed: an up-to-date build that skipped the templates is not
a regeneration check. A second actual generation pass must reproduce the same files without further changes. Include the
template/shared-input changes and their regenerated outputs together in the same change; no generated-only fix may remain.

## Contributing and support

Bug reports, feature requests, comments, questions, code contributions, and love letters are warmly welcome, please do so via GitHub issues and pull requests. Please adhere to our [open-source guidelines](https://lombiq.com/open-source-guidelines) while doing so.

This project is developed by [Lombiq Technologies](https://lombiq.com/). Commercial-grade support is available through Lombiq.
