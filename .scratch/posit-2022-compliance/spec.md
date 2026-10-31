# Feature-complete, FPGA-synthesizable Posit Standard (2022) implementation

Status: ready-for-agent

## Problem Statement

Lombiq.Arithmetics provides configurable posit formats, arithmetic, square root, conversions, and quire-backed fused operations. Having these APIs does not establish conformance: numerical edge cases and rounding need independent verification, several required functions are missing, and duplicate implementations can diverge.

Users need the complete behavior defined by the [Posit Standard (2022), dated March 2, 2022](https://posithub.org/docs/posit_standard-2.pdf), in both ordinary .NET execution and Hastlayer-generated FPGA logic. They also need the existing E0-E4 configurations retained for flexibility. A software-only mathematical library does not meet this effort's requirements.

## Solution

Provide a complete standard profile for 8-, 16-, and 32-bit posits with maximum exponent size two, together with the corresponding 128-, 256-, and 512-bit quires. Cover every normative format, rounding, function, execution-order, and conversion requirement in sections 3-6 of the standard.

Retain the public `Posit32` API alongside `Posit32E2`, with identical standard numerical behavior. Retain the generated E0, E1, E3, and E4 types as nonstandard extensions, without reducing the template's flexibility.

Every standard function must have an FPGA-synthesizable implementation, not just a software implementation or host-side substitute. Public .NET conveniences may adapt strings, culture, and transport to hardware-compatible data representations; the standard-defined computation itself must execute in generated hardware.

Demonstrate compliance through independently checked raw results at the public API boundary and equivalent checks against generated hardware. Maintain a requirement-to-API-to-test coverage matrix so completeness is auditable.

## User Stories

1. As a numerical application developer, I want all required operations on standard posit8, posit16, and posit32 values, so that choosing a precision does not silently remove standard functionality.
2. As an existing consumer, I want to keep using `Posit32`, so that compliance work does not require gratuitous application rewrites.
3. As a user of configurable formats, I want E0-E4 generation retained, so that I can still choose a different range/precision tradeoff.
4. As a consumer exchanging raw values, I want standard encoding and exceptional values, so that data has the same meaning in another compliant system.
5. As a numerical developer, I want the literal standard rounding rule applied to exact results, so that bit patterns are reproducible even at regime boundaries.
6. As a numerical developer, I want finite overflow and nonzero underflow handled as specified, so that ordinary finite arithmetic does not spuriously become infinity, NaR, or zero.
7. As an application developer, I want NaR propagation and domain behavior implemented consistently, so that zero shortcuts and special cases cannot hide invalid inputs.
8. As an application developer, I want equality, ordering, and comparison APIs to agree, so that sorting and comparisons are reliable, including NaR.
9. As a numerical developer, I want negate, absolute value, sign, nearest integer, ceiling, and floor, so that basic transformations need no floating-point workaround.
10. As a numerical developer, I want next and prior operations with standard wraparound, so that I can traverse representations and investigate numerical boundaries.
11. As a numerical developer, I want correctly rounded addition, subtraction, multiplication, and division, so that basic calculations match the standard.
12. As a numerical developer, I want square root, reciprocal square root, and integer-degree roots, so that root calculations preserve standard accuracy and domain semantics.
13. As a numerical developer, I want natural, binary, and decimal exponentials and their minus-one forms, so that small results are not lost through intermediate cancellation.
14. As a numerical developer, I want natural, binary, and decimal logarithms and their plus-one forms, so that small arguments are evaluated without prematurely rounding the increment.
15. As a numerical developer, I want direct, inverse, and pi-scaled trigonometric functions, so that angular computations need no approximate host conversion.
16. As a numerical developer, I want direct and inverse hyperbolic functions, so that the required elementary function set is complete.
17. As a numerical developer, I want hypot, power, and both two-argument arctangent forms, so that multi-operation definitions are evaluated with one final rounding.
18. As a numerical developer, I want compound and fused three-factor multiplication, so that their complete mathematical expressions are evaluated before rounding.
19. As an accumulator user, I want exact posit-to-quire conversion and quire addition, subtraction, negation, and absolute value, so that accumulation preserves information.
20. As a linear algebra developer, I want exact multiply-add and multiply-subtract into a quire, so that dot products do not accumulate intermediate posit rounding.
21. As an accumulator user, I want quire overflow to produce NaR and remain exceptional, so that wrapping cannot masquerade as a valid result.
22. As a numerical developer, I want correctly rounded quire-to-posit conversion, so that cancellation, tiny residuals, and accumulated values are handled correctly.
23. As a mixed-precision developer, I want exact widening and correctly rounded narrowing between all standard precisions, so that conversion is predictable.
24. As a .NET developer, I want conversions for the environment's fixed-width integer formats with the standard sentinel rules, so that boundary values have documented meaning.
25. As a .NET developer, I want conversions to and from supported IEEE binary floating formats, so that NaNs, infinities, subnormals, and signed zeros are handled correctly.
26. As an application developer, I want decimal parsing and round-trip formatting, so that text interchange preserves posit values without intermediate floating-point errors.
27. As a developer using fused expressions, I want source-specified evaluation order and fusion preserved, so that changing execution backend does not change results.
28. As a Hastlayer user, I want every standard function to synthesize into FPGA logic, so that the complete numerical API is available in accelerated execution.
29. As a Hastlayer user, I want hardware results to match software bit for bit, so that acceleration does not trade correctness for speed.
30. As a maintainer, I want independent numerical reference results, so that a shared bug in implementation and expected-value generation cannot pass as compliance.
31. As a maintainer, I want reproducible template generation and consistent `Posit32` implementations, so that fixes survive regeneration.
32. As an agent implementing one part of this effort, I want section-linked contracts, acceptance criteria, and genuine blocking dependencies, so that I can complete a bounded task without reinterpreting the entire standard.

## Implementation Decisions

### Ticket execution

These requirements apply to every implementation ticket, including refactors and generation changes, and to isolated
agent sessions. Follow the [scientific maintainability rules](../../AGENTS.md#scientific-maintainability) throughout.

1. Before coding, read the assigned ticket, its completed prerequisites, and the original 2022 PDF linked above.
   Read the sections relevant to the task and their referenced definitions, including format/rounding rules in sections
   3-4, common function rules in section 5.1, and the applicable operation or conversion clauses. An isolated session must
   obtain the PDF itself; this conversation, another agent's summary, and session-local extracts are not authoritative.
   Check formulas against the rendered PDF when extraction loses symbols. If the document cannot be read or a normative
   interpretation remains unresolved, report the blocker and pause the affected implementation rather than guessing.
2. Record a concise contract under the ticket's `## Comments` before implementation: cite the PDF sections/pages read,
   state the mathematical operation or preserved invariants, applicable precisions, domain and exceptional cases, and
   exactness/rounding boundaries. Identify independent expected results and boundary cases that will test those rules.
   A behavior-preserving refactor must not silently add numerical fixes.
3. Implement the bounded ticket scope under that contract and the scientific maintainability rules. Before marking the
   ticket done, link its contract to the affected APIs and numerical/regression evidence in the shared
   coverage matrix. Record resolved interpretations and any evidence gaps in the ticket. Review the diff for unrelated
   changes, duplication, and unnecessary abstractions; unresolved requirements remain blockers, not completed work.

### Authority, profile, and compatibility

- The linked 2022 PDF is the normative authority. Section numbers below refer to that document. Extracted text, other posit libraries, and earlier posit conventions are supporting evidence, not replacements for the PDF's formulas.
- The compliance profile is the E2 family at 8, 16, and 32 bits, including the equivalent `Posit32` surface. E2 already meets the exponent-size format requirement; it does not yet establish behavioral compliance.
- The exponent field has two logical bits; omitted bits are zero when the regime exhausts available storage. Supporting additional exponent sizes is permitted as an extension, not a reason to remove those types.
- Keep E0-E4 generation parameterized. Preserve extension regression behavior except for explicitly documented correctness fixes. Newly required standard functions must cover every standard precision; full new function coverage for non-E2 configurations is not a release requirement.
- The configurable `Posit`/`PositEnvironment` surface remains experimental and outside this compliance claim. Avoid implying that arbitrary widths or configurations have been certified.
- Preserve existing public entry points where practical. Correcting a result that contradicts the standard is an intentional behavior change, including integer sentinel behavior; document these changes rather than retaining contradictory behavior under a compliant API.
- The final standard behavior for each operation must come from one authoritative algorithm or generation source shared by `Posit32` and `Posit32E2`, compatible with Hastlayer; public API adapters may differ where existing signatures require it. Ticket 01 is a behavior-preserving preparation only: share primitives whose equivalence is established, and record existing divergences without treating either legacy implementation as the compliance oracle. The operation-specific correctness tickets converge their operation on the shared standard implementation as they correct it. Ticket 56 verifies no duplicate standard algorithms or temporary behavior-profile switches remain.
- Standard operation names in this spec identify semantics, not mandatory C# spellings. Follow existing .NET naming conventions and map each public member to its standard operation.
- Expose NaR terminology clearly. Existing NaN-named members can remain compatibility aliases; NaR comparison behavior must not be changed to IEEE NaN behavior.

### Encoding, exactness, and rounding: sections 3-4

- Implement every standard bit pattern, including zero and NaR, truncated exponent fields, negative values, minPos, and maxPos.
- For precision n, minPos is 2 raised to (-4n + 8); maxPos is its reciprocal. A standard quire has 16n bits and least-significant weight equal to minPos squared.
- Treat operation results as exact before applying section 4.1. Preserve all discarded information needed for a correct rounding decision.
- Follow section 4.1's extended-representation decision boundary and even-LSB tie rule literally. Do not silently substitute an arithmetic midpoint between adjacent posit values; equivalence must be established wherever an optimization relies on it.
- Return exactly representable results unchanged. Saturate real finite overflow to signed maxPos and nonzero underflow to signed minPos. Exact zero remains zero.
- Preserve source-specified operation order and explicit fusion. Ordinary operators round at their defined boundaries; compound standard functions round only after evaluating their mathematical definitions. No implicit reassociation, hidden fusion, or extra precision changes may alter observable results.
- Define mixed-precision evaluation through explicit standard conversions and existing documented language rules. Posit arguments to each standard function have matching precision; matching quire operations use the corresponding quire format.

### Required numerical surface: section 5

The following inventory is exhaustive for the required function families. Each operation must be available at all three standard precisions and synthesizable.

| Section | Required operations |
| --- | --- |
| 5.2 | negate, abs, sign, nearestInt, ceil, floor, next, prior |
| 5.3 | compareEqual, compareNotEqual, compareGreater, compareGreaterEqual, compareLess, compareLessEqual |
| 5.4 | addition, subtraction, multiplication, division |
| 5.5 roots | sqrt, rSqrt |
| 5.5 exponentials | exp, expMinus1, exp2, exp2Minus1, exp10, exp10Minus1 |
| 5.5 logarithms | log, logPlus1, log2, log2Plus1, log10, log10Plus1 |
| 5.5 trigonometry | sin, sinPi, cos, cosPi, tan, tanPi |
| 5.5 inverse trigonometry | arcSin, arcSinPi, arcCos, arcCosPi, arcTan, arcTanPi |
| 5.5 hyperbolic | sinH, cosH, tanH, arcSinH, arcCosH, arcTanH |
| 5.6 | hypot, pow, arcTan2, arcTan2Pi |
| 5.7 | fMM: the product of three posit arguments, rounded once |
| 5.8 | compound, rootN |
| 5.11 | pToQ, qNegate, qAbs, qAddP, qSubP, qAddQ, qSubQ, qMulAdd, qMulSub, qToP |

- Apply section 5.1's NaR and unique-real-limit rules, including the specified branch choices. NaR input propagates through real-valued functions except the specified next/prior behavior; comparisons return Boolean values.
- Evaluate exceptional inputs before shortcuts that would suppress NaR. In particular, zero multiplied by NaR is NaR, and zero divided by zero is NaR.
- All comparisons use the ordering of the representation interpreted as a signed two's-complement integer. NaR equals itself and precedes every real value. Operators, typed comparison, and equality must agree.
- Sign returns a posit, not an integer-only result, and sign(NaR) is NaR. NearestInt ties choose the nearest even integer-valued posit.
- Next/prior traverse representation successors/predecessors with wraparound, including transitions through NaR.
- Specify a domain and exceptional-result table for every function family before implementation. Cover endpoints, poles, branch boundaries, negative bases, integral exponents, zero degree, negative root degrees, and even-root branch selection according to the standard. If normative wording is ambiguous, record and resolve the interpretation before declaring that function compliant.
- Pow(0, 0) and arcTan2(0, 0) return NaR. ArcTan2's inputs denote the real and imaginary components in that order, not the usual .NET Atan2 parameter order. On the negative real axis with zero imaginary component, return positive pi rounded; arcTan2Pi follows its specified scaled definition.
- Evaluate pi-scaled functions against mathematical pi, not a prematurely rounded posit or IEEE approximation. Preserve exact integer/half-integer identities and handle argument reduction over the entire input range.
- Evaluate rSqrt, hypot, plus-one/minus-one variants, compound, rootN, and fMM as complete mathematical expressions with one final rounding. Composing already-rounded posit functions is insufficient.
- Existing fused sum, dot product, multiply-add, add-multiply, and multiply-multiply-subtract conveniences remain supported and consistent with once-rounded semantics. The existing four-argument product-difference operation is not a substitute for fMM.
- Three-factor products can exceed a standard quire's range or exactness capacity even when the final posit result is valid. Provide sufficient intermediate representation for fMM rather than assuming a two-product quire is always suitable.
- Standard function names must always provide correct rounding. Any separately offered approximate variants require distinct names and available source under section 5.9; they do not count toward completion.

### Quires and fused accumulation: sections 3.4, 4.2, 5.11

- Use the exact standard fixed-point encoding and NaR pattern for each supported quire width.
- Provide all ten listed operations, with exact finite accumulation and a single rounding only at qToP.
- Detect carry-range overflow and produce quire NaR instead of modular wraparound. NaR remains exceptional in subsequent arithmetic.
- Enforce precision/width compatibility explicitly. Do not silently reinterpret mismatched storage or return a plausible numeric fallback.
- Preserve stored width and scale across operations. Internal raw storage helpers must not bypass standard arithmetic semantics at public numeric entry points.
- Prevent unintended changes to caller-owned operands during conversion or intermediate sign handling. Any intentional accumulator mutation must be explicit in its API contract.
- Correctly handle empty accumulation, exact cancellation, NaR conversion, the smallest nonzero quire magnitude, and positive/negative residuals.
- Preserve the standard's guaranteed exact dot-product capacity for fewer than 2^31 terms and its sum-capacity guarantees. Verify the range argument analytically and with boundary constructions rather than requiring billions of iterations.

### Conversions and text: section 6

- Support every directed conversion between the standard precisions. Widening appends zero bits exactly; narrowing uses section 4.1, preserving zero and NaR appropriately.
- Provide exact posit-to-matching-quire conversion and correctly rounded matching-quire-to-posit conversion. Cross-width quire conversion is not required.
- Cover .NET's fixed-width signed and unsigned 8-, 16-, 32-, 64-, and 128-bit integer types, plus native-sized integers at their actual platform width. Do not route wider values through narrower integer intermediates.
- Apply section 6.4 in both directions. An integer with only its MSB set converts to NaR, including unsigned input. A posit that is NaR or out of integer range after rounding converts to that integer representation's MSB-only sentinel. Cover rounding before range checking and document the integer rounding interpretation used consistently with the standard.
- Provide conversions for IEEE binary16 (`Half`), binary32 (`float`), and binary64 (`double`). Hardware adapters may accept/return their raw encodings.
- Float-to-posit converts every NaN and infinity to NaR, both signed zeros to posit zero, and finite values using section 4.1. Posit-to-float converts zero to positive zero and NaR to a quiet NaN, with finite conversion obeying the environment's supported IEEE rounding mode.
- Document round-to-nearest, ties-to-even as the normal .NET/hardware conversion mode. If an API exposes other IEEE rounding modes, its hardware and software contracts must agree for each exposed mode.
- Support decimal input and output with a documented canonical invariant representation, round-trip NaR token, grammar, error behavior, and optional culture-aware .NET wrappers. Parse mathematical decimal values without intermediate IEEE rounding.
- Every finite posit must have a decimal representation that parses back to the same raw bits. Use section 6.3/Table 2 as the digit-budget reference; shortest-string formatting is not independently required.
- Decimal conversion is included in hardware coverage. Use explicit character-code or digit buffers and explicit lengths for hardware interfaces; host wrappers may handle string allocation and culture adaptation, not precompute the numerical result.
- Hardware buffer capacities and supported textual forms must be documented. Capacity or malformed-input failures must be explicit and distinct from valid numeric results; no silent truncation. Hardware support must cover canonical round-trip text for every supported posit value.

### Synthesizable numerical implementation

- Every operation in the inventory and the standard conversion surface needs a bounded-resource hardware realization. There is no software-only exception for transcendental functions or difficult rounding cases.
- Production kernels must use constructs and storage that Hastlayer can transform. Arbitrary-precision libraries, native math libraries, and host runtime calls may support independent tests or offline coefficient generation, but cannot be required at FPGA execution time.
- Select algorithms, internal widths, range-reduction methods, coefficients/tables, and termination bounds with correctness arguments covering the complete supported input domain. Record those arguments and provenance beside the implementation's supporting documentation.
- A fixed amount of extra precision or an empirical error estimate alone does not establish correct rounding. Use certified bounds and enough refinement or a certified hard-case strategy to resolve every input, including exact boundary cases.
- Adaptive refinement, if used, must have a justified finite hardware bound and an exact or certified final resolution strategy. Reaching a bound cannot return an approximate value under the standard API.
- Establish feasibility of this bounded correctly rounded strategy early. An unresolved hardware feasibility result blocks dependent implementation; it does not authorize reducing precision coverage or silently weakening accuracy.
- Reuse the same numerical kernels in .NET and hardware where possible. Any backend-specific realization must pass the same numerical contracts and reference vectors.
- Supply callable hardware entry points and necessary fixed-storage configuration for all functions and precisions. Changes to Hastlayer needed to make these kernels transformable are part of the effort when unavoidable.
- No clock-rate, latency, throughput, or resource-utilization target has been agreed. Report practical synthesis cost and limitations; optimization must not weaken correctness or make standard operations unavailable.

### Generation, traceability, and documentation

- **Template-only editing is mandatory for generated code in every ticket.** The T4 `.tt` templates and shared `.ttinclude` inputs are the source of truth for the generated fixed-size posit implementations, helper overloads, and fixed-size tests. Make changes exclusively in those sources and let the generator write the checked-in `.cs` outputs. Never edit generated files directly, even temporarily or with formatting/analyzer tools; the next build will overwrite those edits.
- Identify source ownership from the project's generation inputs and expected outputs before editing. Hand-maintained components such as Posit32 without the E suffix and Quire remain editable C#; not every `.cs` file is generated.
- Follow the repository's documented generated-code workflow, including the C# 5 constraint on executable template blocks, which does not constrain the language version of generated C#. Keep implementation and test templates, shared inputs, and expected output lists consistent while retaining E0-E4 flexibility.
- Extend the existing MSBuild-integrated T4 generation rather than assuming generation still requires Visual Studio. Include source-template changes and all resulting generated files in the same change. Regenerate affected implementation and test outputs, verify generation actually ran rather than being skipped as up-to-date, and verify a second actual generation pass produces no further changes.
- Keep one coverage matrix enumerating every normative requirement, public API mapping, supported precision, software evidence, and hardware evidence. Empty mappings are unfinished work.
- Track compatibility evidence separately from standard-compliance evidence. A preserved pre-refactor result is not proof of normative correctness, and correcting tickets must identify the single shared Posit32/Posit32E2 algorithm or generation source for each operation they change.
- Explain the standard profile, extension status, NaR, sentinel conversions, fusion boundaries, text interchange, and hardware calling conventions in consumer documentation.
- Record intentional numerical behavior changes and migration guidance for existing consumers.

## Testing Decisions

### Public behavior as the primary seam

- Use the existing and newly added public Posit/Quire APIs as the primary numerical seam. Construct raw values, invoke public operations, and compare exact output representations; test helpers and internal layout routines only where they provide additional diagnostic value.
- Test observable contracts rather than implementation structure. Expected results must not be generated by calling the production encoding, rounding, conversion, or mathematical functions under test.
- Use raw posit/quire/integer/IEEE encodings and Boolean results as applicable. Floating-point tolerances cannot certify a correctly rounded posit result.
- Existing fixed-size posit tests, quire tests, legacy exhaustive tests, and Hastlayer Posit sample verification provide prior art. Retain useful coverage, but do not treat legacy E0/E1 vectors or approved HDL snapshots as proof of E2 conformance.

### Independent numerical reference

- Build a test-only oracle with independently implemented standard decoding and section 4.1 rounding.
- Use exact integer/dyadic/rational evaluation for arithmetic, conversions, and accumulation. Use certified high-precision enclosures or another independently justified method for elementary functions.
- Validate the oracle against hand-derived encodings, exceptional cases, boundary constructions, and independently sourced references with confirmed 2022 semantics.
- Pin and record reference tooling and vector-generation parameters. Offline generated vectors must carry enough provenance to reproduce them.
- Mathematical identities and cross-backend agreement supplement independent expected results; neither replaces them.

### Required coverage

- Posit8 E2: exhaustive unary inputs and all 65,536 pairs for each two-posit operation, including comparisons and exceptional inputs.
- Posit8 fMM: exhaustively check all three-input combinations in a bounded/sharded conformance run, or reduce them with a documented symmetry argument that retains complete coverage.
- Posit16 E2: exhaustive unary inputs, plus adversarial boundary and deterministic randomized multi-argument coverage.
- Posit32 E2 and Posit32: boundary, hard-rounding, cancellation, range-reduction, domain, and deterministic randomized coverage; verify both public surfaces agree.
- Posit-plus-integer functions: cover every posit8 input against representative signed degrees/exponents, integer extrema, parity changes, zero, and negative values. Support the remaining domain with the algorithm's correctness argument and generated hard cases.
- Exercise exact section 4.1 boundaries and values on both sides, including extreme regimes where ordinary midpoint intuition can mislead.
- Test complete NaR/zero interaction tables; comparison reflexivity and ordering; next/prior wraparound; minPos/maxPos; every function's domain endpoints and branch cuts.
- Test all standard precision-conversion directions, integer sentinel/range boundaries, IEEE subnormals and exceptional encodings, decimal ties, and canonical text round trips.
- Quire tests cover limb carries/borrows, signed extremes, NaR propagation, overflow, exact cancellation, minPos-squared products, operand preservation, once-rounded output, and width mismatch handling.
- Run extension regression tests independently of standard conformance, applying shared fixes without relabeling non-E2 formats as compliant.

### Hardware evidence

- Transform representative entry points for every standard operation and precision through Hastlayer, including conversion and decimal kernels. Existing VHDL approval tests guard generation regressions, not numerical accuracy by themselves.
- Compile/synthesize generated HDL with the supported FPGA toolchain for the claimed hardware profile. VHDL generation alone does not establish synthesizability.
- Simulate or execute generated hardware against independent expected result vectors. Cover every operation/precision, with full-domain small-format coverage where practical and mandatory exceptional, boundary, and hard-rounding cases.
- Demonstrate preservation of ordinary rounding boundaries and explicit fusion across hardware execution. Include streaming/chunked accumulation and cancellation.
- Distinguish generated, simulated, synthesized, and device-executed evidence in the coverage matrix. Unavailable tools or devices are explicit evidence gaps, not passing results.

### Completion gate

The effort is complete only when every required operation and conversion exists for every claimed precision, correctness is supported by independent tests and algorithmic justification, hardware kernels terminate with bounded resources, and synthesis plus numerical hardware evidence covers the complete required surface. Builds and directly related regression suites must pass; no unresolved software-only functions or approximate hard-case fallbacks may remain under the compliant APIs. All generated implementation and test changes must be reproducible from their authoritative templates and shared inputs; a generated-only edit or a skipped generation run cannot satisfy completion.

Every ticket must also satisfy the [ticket execution requirements](#ticket-execution), with its source-linked contract and
evidence recorded, and the repository's scientific maintainability rules.

## Out of Scope

- New 64-bit standard posit types or complete arbitrary-width compliance.
- Full new elementary-function coverage or 2022 compliance claims for non-E2 extension configurations.
- Unum changes unrelated to shared correctness or generation changes required by this effort.
- Error, Bessel, gamma, digamma, beta, zeta, and other special functions excluded from current requirements by section 5.10.
- .NET generic-math integration, SIMD APIs, shortest-decimal optimization, new application features, and unrelated cleanup.
- A separately marketed approximate math library.
- Specific performance/resource targets, new FPGA board support, or compliance with a future revision of the standard.

## Further Notes

- User-confirmed requirements: preserve E0-E4 template flexibility; achieve complete 2022 behavior for the standard family; require every standard function to be FPGA-synthesizable.
- The test seam was confirmed with the stronger hardware requirement: public API behavior plus independent numerical expectations, extension regressions, and hardware coverage for the entire standard surface rather than only the existing subset.
- Existing build-integrated generation is already present; the work is to extend and verify it, not to introduce an obsolete IDE-only generation workflow.
- Correctly rounded, bounded-resource elementary functions are the largest feasibility and implementation risk. Begin with a narrow end-to-end certified function and its synthesized hardware evidence before expanding function families.
- Backend technology, exact C# member spellings for missing functions, and internal representation choices are implementation decisions constrained by this specification, not preselected dependencies.
- Normative ambiguities must be resolved from the PDF and authoritative clarification, with the adopted interpretation recorded. Do not guess missing mathematical symbols from PDF text extraction.
- Subsequent ticket breakdown should use independently verifiable vertical slices, explicit blocking edges, standard section references, precise domain/rounding criteria, and software plus hardware evidence. No tickets are created by this specification.
