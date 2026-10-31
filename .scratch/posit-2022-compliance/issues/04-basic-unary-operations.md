# 04: Complete basic unary operations

**What to build:** Negate, abs, sign, nearestInt, ceil, floor, next, and prior as public standard operations under section 5.2.

**Blocked by:** 03: Correct rounding and standard precision conversions.

**Status:** done

- [x] Cover E2 8/16/32 and Posit32; sign returns a posit. Apply NaR propagation except the specified next/prior representation traversal.
- [x] Implement each corrected 32-bit unary standard operation once for Posit32 and Posit32E2 through shared Hastlayer-compatible logic/generation, retaining only explicit public API adapters.
- [x] NearestInt ties choose the nearest even integer-valued posit; ceil/floor work for negative values and already integral large values.
- [x] Verify next/prior wraparound, transitions through NaR, zero, minPos, and both signed extremes.
- [x] Exhaust all posit8 and posit16 inputs against independent exact expectations; add posit32 integer-boundary and seeded random tests.
- [x] Supply bounded hardware entry points for all eight operations, independent CPU output checks and Hastlayer/VHDL transformation.
- [ ] Supply actual vendor synthesis evidence and independent simulated/device numerical checks (deferred to ticket 56).
- [x] Update generation, operation/API mapping, behavior documentation, and extension regressions.

## Comments

- **Pre-implementation contract:** Independently obtained the original
  [Posit Standard (2022)](https://posithub.org/docs/posit_standard-2.pdf), reading sections 3.1-3.4 (pp. 6-7),
  4.1-4.3 (p. 8), 5.1 and 5.2 including footnotes 7-9 (p. 9); checked the rendered signed-field equation,
  rounding algorithm and unary definitions. For E2 posit8/16/32 and Posit32, negate and abs are exact;
  sign returns a same-precision posit -1, 0 or 1. All six numerical operations propagate NaR.
  NearestInt selects the nearest integer-valued posit, with ties to the even integer (not the even encoding).
  Ceiling/floor select the least/greatest integer-valued posit bounding the input, including negative inputs.
  Already integral values, including magnitudes beyond machine-integer range, are unchanged: no integer or IEEE
  conversion is part of these algorithms. Nonintegral posits have exactly representable bounding integers, so
  section 4.1 introduces no second rounding. Next/prior add/subtract one from the raw representation modulo
  2^n, including maxPos -> NaR -> -maxPos and -minPos -> zero -> minPos.
- **Independent evidence plan:** Decode signed dyadics with the ticket 02 reference; use BigInteger quotient
  and remainder to select integers and independent decoded-value search to recover exact encodings.
  Exhaust all 8/16-bit inputs; use hand-derived ties, integer/regime boundaries, signed extremes and seeded
  32-bit inputs. Keep unary reference support separate from tickets 05/06's arbitrary exact-result rounding.
  Existing Abs and metadata declaration anchors are preserved; generated unary logic is shared by both
  32-bit APIs, with explicit adapters only.
- **Deferred evidence:** Vendor synthesis, simulation, device execution and hardware-dependent DynamicTests
  remain unchecked for ticket 56. Bounded raw-memory CPU checks and Hastlayer/VHDL transformation are in scope.
- **Implementation:** `GenerateUnaryMembers` in `PositTemplate.tt` emits `Negate`, posit-valued `Sign`,
  `NearestInt`, `Ceiling` (the .NET spelling for standard ceil), `Floor`, `Next` and `Prior` for every fixed-size
  E0-E4 type and the Posit32 generated partial. `Abs` remains the existing shared generator; `Negate` and existing
  unary-minus adapters use its shared two's-complement primitive. No old Posit32 member or partial declaration
  moved, and no existing numerical behavior was changed. Integer selection clears fractional representation
  bits and conditionally increments one integer unit; the integer parity at scale zero comes from the implicit
  leading one. Once the scale reaches the stored fraction length, the original integral encoding is returned.
- **Independent tests:** Hand-maintained `PositUnaryReference` uses the independent signed-field decoder,
  BigInteger division/remainder and exact decoded-value binary search, never production arithmetic/conversions.
  No arbitrary-result rounding oracle or shared decoder signature was added. The decoder only gained its
  `IComparable<PositEncodingReference>` declaration and lost a redundant cast to satisfy SDK analyzers when
  linked into the VHDL test project. All 256/65,536 E2 8/16-bit inputs are checked; both 32-bit APIs cover powers
  of two and nearby encodings across the range, integer/half-integer boundaries through 4096.5, the last
  nonintegral region, 8,192 seeded inputs, zero/NaR and extremes. Hand-derived +/-0.5, +/-1.5, +/-2.5 and 3.5
  anchors separately validate the oracle. Generated E0-E4 unary regressions and all prior tests remain enabled.
- **Hardware/approval audit:** `PositUnaryCalculator` has four entry points using exactly nine 32-bit memory
  cells, with all eight outputs checked on CPU against the linked independent reference and the same input
  coverage. The new Catapult approval contains four entry-point machines and four existing raw-constructor
  machines; all unary methods and bit/field helpers are inlined. Reviewed NaR/zero early returns, sub-unit
  thresholds, integral bypass, fractional mask/parity/increment and width-truncated successor/predecessor
  arithmetic. All previous approved VHDL files are byte-for-byte unchanged: there are no existing declaration,
  state-machine or local-renaming differences to excuse. `Posit32.cs` is unchanged.
- **Verification:** Explicit Debug solution analyzer rebuild
  (`dotnet build Hastlayer.SDK.sln --configuration Debug -t:Rebuild --warnaserror
  -p:TreatWarningsAsErrors=True -p:RunAnalyzersDuringBuild=true`) passed with zero warnings/errors.
  Debug Arithmetics tests: 334/334; full Debug Hast.Transformer.Vhdl.Tests: 51/51.
  NuGetTest Release build with the same warning/analyzer flags passed with zero warnings/errors; Debug
  `dotnet test NuGetTest\Hastlayer.SDK.NuGet.sln` passed 1/1 against published packages, retaining source-only
  sample exclusion. Two actual `GenerateArithmeticSources` passes for both implementation and test projects
  (forced by refreshing the shared include timestamp) reproduced every one of the 34 project-owned outputs
  against the pre-pass SHA-256 manifest, with CRLF verified. No skipped target was counted.
