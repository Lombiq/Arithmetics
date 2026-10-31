# 02: Certify encoding, NaR, and comparisons

**What to build:** Standard raw-value interpretation and consistent public comparisons under Posit Standard (2022) sections 3, 5.1, and 5.3, with an independent exact reference foundation.

**Blocked by:** 01: Unify standard implementation generation without changing behavior.

**Status:** done

- [x] Independently decode E2 posit8/16/32 into exact signed dyadic values; validate against hand-derived positive, negative, truncated-field, zero, NaR, minPos, and maxPos encodings.
- [x] Cover Posit8E2, Posit16E2, Posit32E2, and Posit32. All six comparisons, typed comparison, and equality agree with signed two's-complement representation ordering; NaR equals itself and precedes real values.
- [x] Implement the corrected 32-bit standard comparison behavior once for Posit32 and Posit32E2, using shared Hastlayer-compatible logic/generation with explicit public adapters; do not treat legacy ordering as the compliance oracle.
- [x] Expose NaR terminology without removing compatibility aliases; sign classification must not imply NaR has a mathematical sign.
- [x] Exhaust all posit8 pairs and posit16 unary decoding inputs; add posit32 boundaries and seeded random vectors. Expected values do not call production decoding.
- [x] Provide hardware comparison entry points, Hastlayer/VHDL transformation, and CPU raw-result checks against independent vectors.
- [ ] Provide vendor synthesis evidence and simulated/device raw-result checks (deferred to ticket 56).
- [x] Update generation, consumer behavior-change notes, and the requirement/API/software/hardware matrix; preserve non-E2 regression coverage.

## Comments

- **Pre-implementation contract:** Read the original [Posit Standard (2022)](https://posithub.org/docs/posit_standard-2.pdf),
  sections 2 (p. 5), 3.1-3.4 (pp. 6-7), 4.1-4.3 (p. 8), 5.1 and 5.3 (p. 9); checked the rendered encoding equation
  on p. 7. For E2 posit8/16/32, including Posit32, zero is all-zero and NaR is MSB-only. Decode the original fields,
  without production helpers or magnitude conversion: significand is `(1 - 3s) + f`, scale is
  `(1 - 2s) * (4r + e + s)`, and omitted exponent/fraction bits are zero. Exact test values are signed dyadics
  represented by an integer coefficient and binary exponent. Hand-derived anchors cover both signs, truncated
  exponent/regime fields, minPos/maxPos, zero and NaR. No rounding is performed by decoding or comparison.
- Section 5.3 requires all six comparisons to match signed two's-complement integer ordering; section 5.1 makes
  NaR equal to itself and gives it no mathematical sign. Typed/object comparison and equality must agree.
  Keep existing NaN aliases and raw `IsPositive` polarity (used internally, including zero); explicitly document
  its meaning and expose NaR and real-negative classification without assigning a mathematical sign to NaR.
  Standard `Sign` is owned by ticket 04, not this ticket.
- Independent evidence: exhaustive posit8 pairs, exhaustive posit16 decoding/ordered traversal, and posit32
  boundaries plus deterministic seeded raw vectors. Existing E0-E4 regressions remain enabled. Hardware entry
  points return raw Boolean result flags for both 32-bit adapters and the 8/16-bit standard types.
- Vendor synthesis, simulation and device execution remain unchecked deferred evidence for ticket 56.
- **Implementation and behavior changes:** `GenerateComparisonMember` in `PositTemplate.tt` is the single signed
  comparison body for every fixed-size type and the generated Posit32 partial. Public relational operators are
  adapters to typed comparison; equality remains raw equality. Posit32's partial declaration stays at the original
  CompareTo member position. Corrected equal-operand strict comparisons, negative ordering, cross-sign ordering,
  and NaR ordering; added generated `<=`/`>=`, `NaRBitMask`, `IsNaR` and real-only `IsNegative`. Existing raw
  constructors, IsPositive polarity and NaN aliases are preserved. See the consumer notes in `Readme.md`.
- **Independent software evidence:** `PositEncodingReference` directly evaluates the standard's signed-field
  equation using BigInteger dyadics (no production helpers, floating conversion, or magnitude decoder).
  `ReferenceMatchesHandDerivedEncodings` supplies explicit values for all three precisions. `VerifySurface`
  decodes all 256/65,536 8/16-bit encodings; checks every posit8 pair, self/zero/NaR/adjacent pairs for every
  posit16 encoding, and 8,192 seeded pairs for each larger surface. Posit32 uses all regime boundaries and their
  neighbors (both signs), plus 4,096 seeded values. Comparison expectations come from exact dyadic comparison and
  are separately cross-checked against signed integer ordering. The reference is available to ticket 03; it does
  not implement rounding or use the arithmetic midpoint assumption.
- **Hardware and approval review:** `PositComparisonCalculator` exposes four raw-memory entry points for the four
  standard surfaces, returning six Boolean flags. CPU entry-point tests exhaust posit8 pairs and cover larger
  boundary/seeded vectors independently using signed integers. The new Catapult approval transforms all four
  entry points and shows explicit signed 8/16/32-bit reinterpretation, with CompareTo recursively inlined.
  Six existing Posit32 sample approvals intentionally change because addition calls the corrected `>` operator.
  Reviewed all six by matching generated declarations/state-machine blocks: after bijective local-variable
  alpha-renaming and omitting comments, every common non-comparison block is identical. Semantic differences
  are confined to `op_GreaterThan`, removal of its old IsPositive calls/proxy edges, and (when inlining is
  disabled) new signed CompareTo machines/proxy edges. Inlined configurations have no CompareTo runtime boundary.
  Temporary identifier renumbering explains the larger textual diffs; no other existing approvals changed.
- **Verification:** `dotnet build Hastlayer.SDK.sln -t:Rebuild --verbosity quiet` passed with 0 warnings/errors;
  `dotnet test src\Libraries\External\Lombiq.Arithmetics\Lombiq.Arithmetics.Tests\Lombiq.Arithmetics.Tests.csproj --verbosity quiet`
  passed 307/307 (including all previous 285 tests and E0-E4 regression additions);
  `dotnet test test\Hast.Transformer.Vhdl.Tests\Hast.Transformer.Vhdl.Tests.csproj --verbosity quiet` passed 44/44
  (the previous 40 plus three CPU precision cases and one new transformation approval).
  Two actual implementation/test regeneration passes ran `dotnet msbuild <project> -t:GenerateArithmeticSources -verbosity:minimal`
  for `Lombiq.Arithmetics.csproj` and `Lombiq.Arithmetics.Tests\Lombiq.Arithmetics.Tests.csproj`, forcing execution by
  advancing the shared include timestamp before each pass. All 34 outputs enumerated by the projects'
  `ArithmeticGeneratedFile` items had identical SHA-256 hashes; ownership/output lists did not change.
- **Evidence limits:** Raw interpretation, exact reference decoding and comparison are certified here; existing
  numeric component helpers, IEEE/text conversion and arithmetic results are not. The oracle exposed an IEEE
  boundary defect recorded in ticket 14; no conversion change is included. Vendor synthesis, simulation, device
  execution and hardware-dependent DynamicTests were not run and are not implied by CPU/VHDL evidence.
