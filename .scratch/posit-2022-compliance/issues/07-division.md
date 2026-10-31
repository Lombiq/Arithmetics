# 07: Correct division

**What to build:** Correctly rounded division under sections 4, 5.1, and 5.4, retaining enough quotient/remainder information for exact rounding decisions.

**Blocked by:** 03: Correct rounding and standard precision conversions.

**Status:** done

- [x] Cover E2 8/16/32 and Posit32; zero divided by zero and any arithmetic involving NaR return NaR as specified.
- [x] Replace the divergent legacy 32-bit division paths with one authoritative standard algorithm/generation source used by Posit32 and Posit32E2; validate quotient and remainder decisions independently.
- [x] Correctly handle nonzero divided by zero, exact quotients, nonterminating quotients, signed range extremes, and remainder-sensitive ties.
- [x] Exhaust all posit8 pairs against independent rational results; include wide-format adversarial and seeded random checks.
- [x] Document bounded divider precision/termination and prove discarded remainders cannot corrupt rounding.
- [x] Provide hardware entry points, independent CPU raw-result checks and Hastlayer/VHDL transformation.
- [ ] Supply actual vendor synthesis evidence and independent simulated/device raw-result checks (deferred to ticket 56).
- [x] Update generation, behavior-change documentation, coverage matrix, and extension regressions.

## Comments

- **Pre-implementation contract:** Independently obtained the original
  [Posit Standard (2022)](https://posithub.org/docs/posit_standard-2.pdf), reading sections 1.3 and 2 (pp. 4-5:
  NaR is the umbrella for anything not mathematically definable as a unique real; bit fields beyond the explicit
  bits are 0), 3.2-3.3 (pp. 6-7, rendered signed-field equation), 4.1-4.3 (p. 8, rendered rounding algorithm), 5.1
  and 5.4 (p. 9) and 6.1 (p. 12). Division returns posit1 ÷ posit2 rounded once per section 4.1, with inputs and
  output of one precision (section 5.1); mixed precisions are only reached through explicit section 6.1 conversions.
  Exceptional domain, checked in this order: either operand NaR -> NaR; x ÷ 0 for every x, including 0 ÷ 0 -> NaR,
  because the quotient is not arbitrarily close to a unique real number in any neighborhood of the inputs (section
  5.1); 0 ÷ x for real nonzero x -> exactly 0. Every other quotient is a nonzero real whose sign is the XOR of the
  operand signs; it is never rounded to 0 or NaR: |x| > maxPos = 2^(4n-8) gives ±maxPos and 0 < |x| < minPos =
  2^(-4n+8) gives ±minPos (section 3.2, p. 6, and 4.1). Otherwise u < x < w are adjacent n-bit posits and v is the
  (n+1)-bit posit of U1: x < v, or x = v with LSB(U) = 0, gives u, else w. Since posit values are dyadic, a
  nonterminating (non-dyadic) quotient can never equal v; only exact dyadic quotients can tie.
- **Bounded divider:** For E2 n ∈ {8, 16, 32}, each magnitude decodes to scale s = 4k + e and an integer
  significand S ∈ [2^(n-5), 2^(n-4)) with n-5 fraction bits (the most any n-bit E2 posit stores; truncated
  exponent/fraction bits read as 0). One integer division q = ⌊L·2^(n-3) / R⌋, remainder r = L·2^(n-3) - q·R,
  uses a (2n-7)-bit dividend (9, 25, 57 bits) and an (n-4)-bit divisor in a 32-bit word for posit8/16 and a 64-bit
  word for posit32. L/R ∈ (1/2, 2) puts q ∈ [2^(n-4), 2^(n-2)); q ≥ 2^(n-3) exactly when L ≥ R, giving the result
  scale sL - sR or sL - sR - 1. Even when L < R, q holds the hidden bit, the n-5 fraction bits a result can store
  and one more bit: every U1 boundary bit of the unbounded encoding lies in q. The exact quotient is
  (q + r/R) units of q's LSB with 0 ≤ r/R < 1, so the discarded r/R only lies strictly below the boundary bit:
  it can distinguish x = v from x > v (sticky r ≠ 0) but never moves x across v, u or w. Encodings are ordered like
  values (section 5.3), so truncating the unbounded regime/exponent/fraction string to n-1 magnitude bits gives U,
  the next bit compares with v, and remaining bits or r ≠ 0 are sticky. The algorithm is one division and loops
  bounded by n-1 regime bits, so it terminates on all inputs with no iteration, approximation or IEEE conversion.
  Results with scale ≥ 4n-8 clamp to maxPos and scale < -(4n-8) to minPos; in between the regime has at most n-1
  bits, so incrementing U never reaches NaR and the magnitude never truncates to 0.
- **Independent evidence plan:** Exact operand values from the ticket 02 signed-field decoder; the quotient as the
  exact BigInteger fraction (coefficients and powers of two), rounded by the shared section 4.1 oracle extended
  for this ticket with an exact rational overload (submodule commits f384c51 and 060559c), never a finite dyadic
  approximation or production arithmetic. Exhaust all 65,536 posit8 pairs; posit16/32 hand-derived ties,
  remainder-sensitive near-ties, nonterminating quotients, exact quotients, regime/exponent-truncation boundaries,
  maxPos/minPos saturation and signed extremes in all sign combinations, plus seeded random pairs; Posit32 and
  Posit32E2 must agree bit for bit. Neither legacy Posit32 nor legacy generated E2 division is an oracle.
- **Interpretations:** NaR is checked before the zero divisor/dividend cases (both give NaR when either operand is
  NaR); 0 ÷ 0 and x ÷ 0 are NaR (section 5.1 criterion and the NaR definition); 0 ÷ x is the posit 0. Only the E2
  surfaces are standard formats; non-E2 E0/E1/E3/E4 extension types keep their existing division as regressions.
- **Deferred evidence:** Vendor synthesis, simulation, device execution and hardware-dependent DynamicTests remain
  unchecked for ticket 56. Bounded raw-memory CPU checks and Hastlayer/VHDL transformation are in scope.
- **Implementation:** `GenerateDivisionMember` in `PositTemplate.tt` emits the bounded divider above as
  `DivideBits` for `Posit8E2`, `Posit16E2`, `Posit32E2` and the generated `Posit32` partial. `Posit32.cs` keeps
  its `/` operator at its original position as a one-line adapter plus a partial declaration; its legacy body is
  removed and no other member or partial declaration moved. Non-E2 E0/E1/E3/E4 types keep their former division
  as extensions, still covered by the generated `DivisionIsCorrect` regressions. Corrected former behavior: 0 ÷ NaR
  returned 0 on all four surfaces; `Posit32` returned 0 for 0 ÷ 0 and ignored the division remainder; generated E2
  types did not check for a NaR dividend. The legacy hand-picked Posit32 division vectors still pass. On merging
  ticket 05, its identical operand decoder became the shared `AppendE2OperandDecoding` used by `AddBits` and
  `DivideBits`; both kernels' generated outputs are unchanged.
- **Independent tests:** Hand-maintained `PositDivisionReference` decodes operands with the ticket 02 decoder
  and rounds the exact BigInteger quotient with the rational section 4.1 reference, never production arithmetic.
  `PositDivisionTests` exhausts all 65,536 posit8 pairs. 56,445 posit16 and 171,135 posit32 pairs comprise all
  25,600/123,904 pairs of 160/352 boundary operands (zero, NaR, extremes and neighbors of each 2^(4k), both signs),
  64 dividends over all 226/482 signed power-of-two divisors through every regime/exponent-truncating length,
  8,190/8,192 seeded near-target pairs whose dividend is rounded from an (n+1)-bit target × divisor (U1 boundaries
  and posits; NaR targets skipped), and 8,191 seeded random pairs; both 32-bit APIs agree bit for bit. Exact
  quotients/exact U1 ties exercised: 16,564/3,472 (posit8), 13,729/1,554 (posit16) and 34,295/1,981 (posit32).
  Hand-derived anchors (1/3, remainder-decided truncated tie, even/odd exponent-only ties, 0.5, extremes, section
  5.1 cases) check reference and production independently.
  Removing the remainder sticky or rounding ties away from zero fails every division test.
- **Hardware/approval audit:** `PositDivisionCalculator` has four entry points over three 32-bit memory cells; CPU
  checks reuse the linked reference and pair sets. Its new Catapult approval inlines `DivideBits` into each type's
  `op_Division`: one wide division, one remainder multiplication, run-length loops bounded by n - 1 iterations and
  a multicycle constraint per divider, matching the legacy 64-bit division. The three Posit32Advanced snapshots
  (Catapult; identical Xilinx Nexys4 DDR/Alveo U50) were audited per block, declaration and invocation proxy:
  `RepeatedDivision`, the entry/external proxies and all non-division helpers are identical or renumbered only.
  Old-division-only callees (`ctor(UInt32)`, `ctor(Int32)`, `Abs`, `CalculateScaleFactor(SByte,UInt32,Byte)`,
  `GetMostSignificantOnePosition(UInt64)`) and their proxies disappear; `op_Division` becomes a call to the new
  non-inlined `DivideBits` machine, which takes over the legacy divider's identical multicycle constraint. Proxy
  caller lists only lose the old `op_Division`/`Abs` callers or swap `AssemblePositBitsWithRounding` for `DivideBits`;
  members left with one caller use Hastlayer's direct signal connections instead of invocation handlers.
  `AssemblePositBitsWithRounding` lost its `GetTwosComplement` branch: `Sqrt` is now its only caller and passes a
  constant false sign, so Hastlayer folds the sign conditionals. All differences are intentional division effects.
- **Verification:** After merging issue/HAST-136 with ticket 05 (`d956186`; SDK `fe7dcb18`), every ticket 05 and 07
  approval reproduced unchanged. The full analyzer `Rebuild` of `Hastlayer.SDK.sln` (Debug, `--warnaserror`,
  `TreatWarningsAsErrors`, `RunAnalyzersDuringBuild`) has 0 warnings/errors, and `Lombiq.Arithmetics.Tests` pass
  365/365. The full `Hast.Transformer.Vhdl.Tests` run passes 59/59 (51 base + 4 ticket 05 + 4 ticket 07) with no
  received files. The `NuGetTest\Hastlayer.SDK.NuGet.sln` Release analyzer build is clean and its Debug tests pass
  1/1. Main checks repeated after `dotnet restore Hastlayer.SDK.sln --force` gave the same results. Two forced
  `GenerateArithmeticSources` passes on both projects each ran generation and rewrote all 34 `ArithmeticGeneratedFile`
  outputs byte-identically (SHA-256) with CRLF line endings, leaving git clean.
