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
- The same template generates `Posit\Posit32.Generated.cs`, a partial `Posit32` containing members shared with generated posit types.
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

## Posit comparison behavior (2022 standard)

Fixed-size E0-E4 types and `Posit32` compare their raw encodings as signed two's-complement integers.
All six comparison operators, typed/object `CompareTo`, and equality agree: equal inputs are neither greater nor
less; negative real values retain numerical order; NaR equals itself and precedes every real value.
This intentionally corrects former strict-comparison behavior (including equal and negative operands).
Generated types now also expose `<=` and `>=`. `Posit32` and `Posit32E2` share one template-generated comparison body.

`NaRBitMask` and `IsNaR()` name the MSB-only Not-a-Real value; `NaNBitMask` and `IsNaN()` remain compatibility aliases.
`IsNegative()` identifies only negative real values, never NaR or zero. Existing `IsPositive()` is a raw sign-bit
polarity helper, not the standard `sign` function: it returns true for zero and false for NaR. NaR has no mathematical
sign. These encoding/comparison checks do not claim correctness of arithmetic, rounding, or IEEE/text conversions.

## Posit precision conversions (2022 standard)

The E2 `Posit8E2`, `Posit16E2`, and `Posit32E2` types support explicit conversions between all precisions; `Posit32`
also provides compatibility adapters for these conversions. Widening appends zero representation bits exactly.
Narrowing follows Posit Standard (2022), sections 4.1 and 6.1: exact values are preserved, finite overflow clamps to
signed maxPos, nonzero underflow clamps to signed minPos, and ties use the literal appended-one extended-representation
boundary with an even least-significant encoding bit. Zero and NaR are preserved. Other exponent-size conversions remain
extensions and are not certified by this E2 compliance claim.

## Posit unary operations (2022 standard)

Fixed-size E0-E4 types and `Posit32` expose `Negate`, `Abs`, posit-valued `Sign`, `NearestInt`, `Ceiling`, `Floor`,
`Next` and `Prior`. `Negate` and the existing unary `-` operator use the same exact two's-complement primitive;
the existing `Abs` and unary-operator behavior is unchanged. `Sign` returns -1, 0 or 1 as a posit, unlike the
raw-polarity helper `IsPositive`. These six numerical operations preserve NaR.

`NearestInt` uses ties to the even **integer**, not the even encoding. `Ceiling`/`Floor` bound the input above/below,
including negative inputs; all three preserve already integral values even beyond machine-integer range.
They operate on the posit's exact fields without IEEE or integer conversion. For nonintegral inputs, both bounding
integers are exactly representable, so integer selection requires no subsequent posit rounding.
`Next`/`Prior` instead traverse raw representations modulo their width, including NaR:
`maxPos -> NaR -> -maxPos` and `-minPos -> zero -> minPos` are successor transitions.

Independent exact-dyadic tests exhaust every E2 8/16-bit input and cover 32-bit integer/regime boundaries and seeded
inputs, including `Posit32`. E0/E1/E3/E4 regression coverage remains enabled. This unary-operation claim does not
extend to the experimental configurable `Posit` type or certify the remaining arithmetic/transcendental operations.

## Posit addition and subtraction (2022 standard)

`Posit8E2`, `Posit16E2`, `Posit32E2` and `Posit32` implement `+` as Posit Standard (2022), sections 4.1 and 5.4
require: the exact signed sum is rounded once. Exact sums, including exact cancellation to zero, are returned
unchanged. Finite overflow clamps to signed maxPos. Otherwise the literal appended-one boundary decides, with ties
going to the even encoding. NaR in either operand returns NaR, checked before any zero shortcut. `a - b` is
`a + (-b)` with exact negation, so it also rounds once.

Every operator rounds at its source boundary: `(a + b) - c` rounds twice and is never reassociated or implicitly
fused. Use the explicit `FusedSum` when a single rounding is wanted. The `int` operand overloads convert the integer
first, then add once.

`Posit32` and `Posit32E2` now share one template-generated, fixed-width, Hastlayer-compatible addition kernel.
E2 8/16-bit results are unchanged. The former 32-bit results are intentionally corrected: they differed from each
other and from the standard by one or more ulps, chiefly near the extreme regimes and when operand scales differ
widely. Addition for the E0/E1/E3/E4 extension types and the experimental configurable `Posit` type keeps its legacy
behavior and is not certified.

## Posit multiplication (2022 standard)

`Posit8E2`, `Posit16E2`, `Posit32E2` and `Posit32` implement `*` as Posit Standard (2022), sections 4.1 and 5.4
require: the complete exact product is rounded once. NaR in either operand returns NaR, checked before any zero
shortcut, so `0 * NaR` is NaR; otherwise a zero operand returns zero. Exact products, including exact powers of two,
are returned unchanged. Finite overflow clamps to signed maxPos and nonzero underflow to signed minPos, so a finite
nonzero product never becomes NaR or zero. Otherwise the literal appended-one boundary decides, with ties going to the
even encoding.

Every operator rounds at its source boundary: `(a * b) * c` rounds twice and is never reassociated or implicitly
fused. Explicit fused operations and quire accumulation, such as `MultiplyIntoQuire` and `FusedDotProduct`, stay
separate and are not certified here. The `int` operand overloads convert the integer first, then multiply once.

All four types multiply through one template-generated, fixed-width, Hastlayer-compatible algorithm (`Posit32`
delegates its existing `*` operator to it): a bounded `MultiplyBits` per width, built from the same operand decoding
and unbounded-encoding rounding emitters as division. The exact product of the decoded significands fits the word, so
no product bit is lost before the single rounding. E2 8/16-bit finite products are unchanged; only
NaR propagation, including `0 * NaR`, is corrected for them. The former 32-bit results are intentionally corrected:
NaR did not propagate, and finite products differed from each other and from the standard by one or more ulps. The
legacy `Posit32E2` product was wrong near the extreme regimes, the hand-written `Posit32` product at ordinary
magnitudes too, where it could also underflow to zero. Multiplication for the E0/E1/E3/E4 extension types and the
experimental configurable `Posit` type keeps its legacy behavior and is not certified.

## Posit division (2022 standard)

`Posit8E2`, `Posit16E2`, `Posit32E2` and `Posit32` divide through one template-generated algorithm (`Posit32` delegates
its existing `/` operator to it). Following sections 4.1, 5.1 and 5.4, the exact quotient is rounded once: a NaR operand
or a zero divisor (including 0 / 0) gives NaR, 0 / x gives 0, and every other quotient keeps the sign of its operands,
clamping to signed maxPos/minPos instead of reaching NaR or 0. Ties use the appended-one boundary with an even encoding.

One integer division of the decoded significands yields every bit up to and including the rounding boundary bit; its
remainder lies below that bit, so it only decides whether a quotient on the boundary is exact. The division word is
twice the posit width (at least 32 bits) and loops are bounded by the posit width. This replaces the two divergent
former 32-bit algorithms. Formerly, 0 / NaR returned 0 on all four surfaces, `Posit32` also returned 0 for 0 / 0 and
ignored the division remainder, and the generated E2 types did not check for a NaR dividend. Non-E2 extension types
keep their former division. Independent exact-rational tests exhaust all posit8 pairs and cover 16/32-bit ties,
remainder-decided rounding, saturation and seeded pairs.

## Contributing and support

Bug reports, feature requests, comments, questions, code contributions, and love letters are warmly welcome, please do so via GitHub issues and pull requests. Please adhere to our [open-source guidelines](https://lombiq.com/open-source-guidelines) while doing so.

This project is developed by [Lombiq Technologies](https://lombiq.com/). Commercial-grade support is available through Lombiq.
