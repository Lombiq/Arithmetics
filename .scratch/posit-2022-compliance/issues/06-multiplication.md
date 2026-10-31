# 06: Correct multiplication

**What to build:** Correctly rounded ordinary multiplication under sections 4 and 5.4.

**Blocked by:** 03: Correct rounding and standard precision conversions.

**Status:** done

- [x] Cover E2 8/16/32 and Posit32, preserving complete product information before one section 4.1 rounding.
- [x] Replace the divergent 32-bit product paths with one authoritative standard algorithm/generation source used by Posit32 and Posit32E2; validate against independent exact products, not either legacy result.
- [x] Propagate NaR before zero shortcuts, including zero times NaR; test signed products, exact powers, nonzero underflow, finite overflow, and exponent/regime transitions.
- [x] Exhaust all posit8 pairs using independent exact products; include posit16/32 tie, sticky-bit, extreme-scale, and seeded random vectors.
- [x] Supply bounded product kernels and hardware entry points, independent CPU output checks and Hastlayer/VHDL transformation.
- [ ] Supply actual vendor synthesis evidence and bit-exact simulated/device checks (deferred to ticket 56).
- [x] Update parameterized generation, operation documentation, compatibility notes, and software/hardware coverage matrix; retain extension regression coverage.

## Comments

- **Pre-implementation contract:** Read the original [Posit Standard (2022)](https://posithub.org/docs/posit_standard-2.pdf),
  sections 3.1-3.3 (pp. 6-7), 4.1 (p. 8), 5.1 and 5.4 (p. 9); checked the rendered pages 6-9 where text extraction loses
  symbols. For E2 posit8/16/32 and the `Posit32` surface, `multiplication(posit1, posit2)` (5.4) returns
  `posit1 × posit2, rounded` (4.1: "Operation results are regarded as exact prior to rounding"). Both inputs and the
  result have the same E2 precision `n` (8, 16 or 32; 5.1 requires equal precisions, and mixed inputs use the 6.1
  conversions first). Each finite posit is the exact signed dyadic value of its 3.3 fields (an integer multiple of
  minPos = 2^(-4n+8), at most maxPos = 2^(4n-8), 3.2), so the product is an exact finite dyadic value whose complete
  significand product is kept until the single rounding. Any NaR input gives NaR (5.1) and that test precedes every
  zero shortcut, so `0 × NaR` and `NaR × 0` are NaR; otherwise a zero operand gives the exactly expressible `0`
  unchanged. The product sign is the XOR of the operand signs. Rounding is the literal 4.1 algorithm: exactly
  expressible values are returned unchanged; `|x| > maxPos` returns `sign(x) × maxPos`; `0 < |x| < minPos` returns
  `sign(x) × minPos` (finite nonzero operands can never produce NaR or `0`); otherwise take the adjacent `n`-bit posits
  `u < x < w`, let `U` be the `n`-bit representation of `u` and `v` the `(n+1)`-bit posit with representation `U1`, and
  return `u` when `x < v` or when `x = v` and the LSB of `U` is 0, else `w`. No arithmetic midpoint, truncation or
  round-half-up is assumed, and neither legacy truncating Posit32/Posit32E2 product is an oracle.
- **Interpretations and boundaries:** Only E2 8/16/32 and the Posit32 compatibility surface follow this contract;
  non-E2 (E0/E1/E3/E4) products keep their legacy algorithm and regression coverage, with any justified correction
  documented. Section 4.1 for a negative `x` is the negation of 4.1 for `|x|` (two's complement negation maps adjacent
  posits to adjacent posits and `U1` boundaries to `U1` boundaries, and flips the LSB parity of `U` consistently with
  swapping `u` and `w`), so the product may round the magnitude and negate the encoding; the test oracle still applies
  4.1 directly to the signed exact product. The `Posit * int` overloads keep their existing integer constructor
  conversion and then use the standard product; integer-conversion semantics belong to the integer-conversion tickets.
  `MultiplyIntoQuire`, fused products (4.2, tickets 10 and 11), division and addition are outside this ticket.
- **Independent evidence plan:** The exact product is formed from the ticket 02 section 3.3 signed-field `BigInteger`
  decoder as `(c1·c2, e1+e2)` and rounded by the shared literal 4.1 oracle owned by ticket 05
  (`PositRoundingReference.Round(PositEncodingReference, size)`), never by production arithmetic, IEEE casts or midpoint
  assumptions. Evidence: all 65,536 posit8 pairs; hand-checked ties, sticky bits, regime/exponent transitions, exact
  powers, maxPos/minPos edges, finite overflow and nonzero underflow in both signs; hard and seeded posit16/32 vectors;
  NaR/zero/sign combinations; Posit32 versus Posit32E2 agreement; retained non-E2 regressions.
- **Shared oracle:** No multiplication-specific rounding oracle exists. `PositMultiplicationReference` decodes both
  operands with `PositEncodingReference.Decode`, builds the exact product with `PositEncodingReference.FromDyadic`
  (coefficient `c1·c2`, exponent `e1+e2`; NaR first) and rounds it once with ticket 05's `PositRoundingReference.Round`
  (isolated commits `8aa193e` and `181bdc8`, integrated in `d956186`). The reference has no production dependency, is
  linked into the SDK VHDL tests, and is clean under the SDK analyzers that apply to linked files.
- **Interpretations:** Unlike addition, both clamps are reachable: a product of two minPos values is far below minPos
  and maxPos × maxPos is far above maxPos, so nonzero underflow returns signed minPos (never 0) and finite overflow
  signed maxPos (never NaR). Near the extreme regimes truncated exponent bits leave powers of two at scales 4n-15,
  4n-13, 4n-11, 4n-10 and 4n-9 (and their negatives) unrepresentable (section 3.3, p. 7). The power-of-two test
  therefore expects the section 4.1 rounding of 2^(a+b), and the tie generator places each factor only where it is
  exactly representable. Operand significands have at most 28 bits (hidden bit plus 27 fraction bits for n = 32), so the
  64-bit product of two significands is exact. The `int` overloads convert first and then multiply once; their
  conversion is certified by ticket 12. The kernel rounds the magnitude and negates it (4.1 symmetry), while the
  independent oracle applies 4.1 to the signed exact value directly.
- **Deferred evidence:** Vendor synthesis, simulation, device execution and hardware-dependent DynamicTests remain
  unchecked for ticket 56. Bounded raw-memory CPU checks and Hastlayer/VHDL transformation are in scope. Fused and quire
  products (`MultiplyIntoQuire`, fused helpers, tickets 10 and 11), division (ticket 07) and non-E2 multiplication are
  outside this ticket.
- **Implementation:** `GenerateE2MultiplicationMembers` in `PositTemplate.tt` emits one
  `MultiplyE2Bits(uint leftBits, uint rightBits, int size)` kernel in the generated `Posit32` partial. It is the only
  product algorithm for `Posit8E2`, `Posit16E2`, `Posit32E2` and `Posit32`, each reaching it through an explicit
  adapter.
  - The generated E2 `operator *` of `Posit8E2`/`Posit16E2`/`Posit32E2` calls it with their size. The hand-maintained
    `Posit32.cs` keeps only its `operator *` adapter; the legacy body and its `IsZero`, `Abs`, fraction, scale-factor
    and rounding helper calls are gone. The partial anchors at lines 527/551/699 (`Abs`, `GetTwosComplement`,
    `AddBits`) do not move; `CompareTo` shifts from 918 to 886 as the legacy body shrinks, with the member order
    unchanged. Template order is `AddBits`, then `MultiplyE2Bits`.
  - NaR is checked before zero, so `0 × NaR` is NaR; a zero operand gives 0. The sign is the XOR of the operand signs
    and the magnitude is rounded once, then negated.
  - Each operand magnitude is decoded with the bounded `PositHelper.LengthOfRunOfBits` scan into a scale and a 32-bit
    significand (hidden bit at bit 31); missing exponent and fraction bits read as zero. The 64-bit product of the two
    significands is exact, and its top bit adds a carry to the scale.
  - A scale of at least 4n - 8 clamps to maxPos and a scale below -(4n - 8) to minPos. In between, the regime and the
    extra `U1` bit always fit in the encoding, so rounding stays within minPos..maxPos and reaches neither 0 nor NaR.
  - The long-form encoding (regime, exponent, fraction) is built with one extra bit below the LSB (the `U1` bit) and
    every lower product bit ORed into a sticky bit. It rounds up when the extra bit is set and either the sticky bit
    is set or the LSB is odd. Because encodings are ordered like values, this is the literal section 4.1 comparison
    with `U1`, not an arithmetic midpoint.
  - The kernel uses only fixed-width integers and is marked AggressiveInlining; with Hastlayer method inlining it is
    fully inlined (see the audit below).
  - Non-E2 types and the `* int` overloads (convert, then one standard product) are unchanged and keep their existing
    regression tests; no non-E2 correction was needed.
- **Correction audit:** A throwaway harness compiled the pre-ticket sources (`d956186`) and ran them against the
  independent exact products over the de-duplicated union of every vector set: 65,536 posit8, 1,502,982 posit16 and
  216,095 posit32 ordered pairs. The current tree has no mismatch, and `Posit32` equals `Posit32E2` everywhere.
  - Legacy `Posit8E2` and `Posit16E2` were wrong only when an operand was NaR (511 posit8 and 769 posit16 pairs,
    including `0 × NaR`); every finite product already matched, so their finite results are unchanged.
  - Legacy `Posit32E2` was wrong on 8,423 pairs: 487 with a NaR operand (including `0 × NaR`) and 7,936 finite products
    (386 off by one ulp, 540 by two to four, 7,010 by more). Every finite error involved |scale| >= 100, where truncated
    exponent bits break the legacy rounding.
  - Legacy hand-written `Posit32` was wrong on 46,387 pairs: 487 NaR-operand results and 45,900 finite ones (3,202 off
    by one ulp, 1,774 by two to four, 40,924 by more; 8,408 at mid-range scales and 37,492 in the extreme regimes).
    1,694 were nonzero products that underflowed to 0 instead of minPos; none had a wrong sign or became NaR. For
    example minPos × minPos (`0x1 * 0x1`) gave `0x4` instead of `0x1`, and `0x1 * 0x20000000` gave `0x0`.
  - The two legacy 32-bit kernels disagreed with each other on 45,379 of those pairs.
  - Both are now one bit-identical standard kernel; no legacy behavior served as an oracle.
- **Independent tests:** `PositMultiplicationReference` (test-only, no production dependency) decodes the operands,
  multiplies them exactly in `BigInteger` and rounds once with the shared oracle. `PositMultiplicationTests` has 21
  tests:
  - Product Facts check every pair in both operand orders: all 65,536 posit8 pairs; for posit16, 145,924 hard pairs
    (every special, power-of-two and bit-pattern operand), 36,000 seeded pairs and ten shift/fraction multipliers
    against all 65,536 encodings; for 32 bits, 59,536 hard pairs (special and power-of-two operands), 36,000 seeded
    pairs and the same multipliers against 6,786 pattern and seeded operands. `Posit32` and `Posit32E2` are both
    checked against the reference, so they agree bit for bit.
  - `PowersOfTwoMultiplyExactlyAndClampAtTheExtremes` multiplies every representable pair of powers of two in each
    sign combination across the overflow and underflow clamps (8, 16 and 32 bits).
  - `NaRIsDecidedBeforeZeroAndZeroIsExact` covers NaR and zero against special, pattern and power-of-two operands.
  - Hand-derived posit8/16/32 anchors validate the reference itself: exact products, carries into the exponent and
    regime, exact ties for both LSB parities, maxPos/minPos clamps in both signs and `0 × NaR`. Every anchor is also
    checked for either operand order and sign.
  - `ExactTiesAndNearbyStickyBitsFollowSection41` pins a posit32 tie and three neighbors that only the lowest product
    bits decide.
  - `GeneratedTiesRoundToEvenAndTheirNeighborsAreMonotonic` builds exact products of the `U1` tie value from its
    (n+1)-bit decoding: 5,608 posit16 pairs (400 random targets, 2,941 below an even U and 2,667 below an odd U) and
    698 posit32 pairs (40 targets, 351 and 347). It checks round-to-even in every sign combination and that one-ulp
    neighbors never cross the tie.
  - `IntegerOverloadsUseTheStandardProduct` checks `* -3` to `* 3` for the four types against the reference.
  - A throwaway mutation run (temporary template edits, reverted) killed all 12 mutants: truncation, round-half-up,
    ignored sticky bit, zero tested before NaR, early maxPos clamp, early minPos clamp, nonzero underflow flushed to 0,
    ignored product carry, one's-complement negation, wrong tie-parity bit, truncated operand fraction (32 bits) and
    truncated `scale / 4`. Two further variants are provably equivalent: dropping the product's lowest bit from the
    sticky bit (always 0 for n <= 32, because the operand significands are multiples of 2^4), and clamping to maxPos
    only above 4n - 8 (`>=` is kept to avoid a shift by 64).
- **Hardware/approval audit:** `PositMultiplicationCalculator` has four entry points: `MultiplyPosit8`,
  `MultiplyPosit16`, `MultiplyPosit32E2` and `MultiplyPosit32`.
  - Each reads the two raw operands from cells 0 and 1 and writes the raw product to cell 2 (three 32-bit cells).
  - A VHDL-test CPU theory (8, 16 and 32 bits; both 32-bit entry points) checks every output against the linked
    independent reference, and that the operand cells stay untouched, over all posit8 pairs and the hard, seeded
    (4,000 per kind), pattern-sweep and tie vectors of the larger sizes.
  - The new Catapult approval (9,522 lines) has twelve instance records: the four entry points, four `op_Multiply` and
    four `.ctor(raw, Boolean)` machines. `MultiplyE2Bits` is inlined into each `op_Multiply` (four inlines, eight
    `LengthOfRunOfBits` scans). The `Posit32` and `Posit32E2` `op_Multiply` machines (99 declaration and 1,834
    state-machine lines each) and their constructors are identical up to type names and bijective local renaming,
    confirmed by a token-aligned comparison (no structural difference, no mapping defect).
  - Six existing snapshots changed: Posit32Sample and Posit32SampleWithInlining for Catapult, Alveo U50 and Nexys4
    DDR. They were audited block by block; Xilinx mirrors Catapult and the two Xilinx devices are byte-identical.
    - Without inlining, `op_Multiply` shrinks from 109 + 669 to 27 + 90 declaration/state-machine lines, with a
      separate `MultiplyE2Bits` machine (77 + 1,404) and one `PositHelper::LengthOfRunOfBits` machine added. The
      legacy-only callees `IsZero`, `GetMostSignificantOnePosition(UInt64)`, `CalculateScaleFactor`, `Abs`,
      `FractionSizeWithoutSignCheck` and `FractionWithHiddenBitWithoutSignCheck` disappear, so instances drop from 148
      to 116 and members from 34 to 30. 17 members are identical and six keep their bodies but lose instances
      (`GetMostSignificantOnePosition(UInt32)`, `LengthOfRunOfBits`, `GetRegimeKValue`, `GetExponentValue`,
      `GetTwosComplement`, `.ctor(UInt32,Boolean)`).
    - With inlining, `MultiplyE2Bits` is inlined into `op_Multiply` (182 + 4,987 to 99 + 1,834 lines). Instances drop
      from 61 to 51 over the same 15 members: 10 identical, two with unchanged bodies and three changed.
    - The remaining body changes (plain: `AssemblePositBitsWithRounding`, `SetZero`, `.ctor(Int32)`, `.ctor(UInt32)`;
      inlining: the last two) are transformer constant-propagation side effects, not numeric changes. A parameter
      becomes a constant only if every call site passes the same constant, and removing the legacy product's call sites
      lets `.ctor(Int32)` fold to `new Posit32(1u)`, `.ctor(UInt32)` to `PositBits = 1u`, `SetZero` to `bits = 1u` and
      `AssemblePositBitsWithRounding` to `signBit = false`, `fractionBits = 1u`. Two temporary controls (reverted)
      confirmed this; in the stronger one a copy of the legacy product was kept and called: 29 of the 34 old members
      were identical, the constructors differed only in instance count (10 to 11), and the copy matched the old
      `op_Multiply` machine (109 + 669 lines) apart from its name.
  - All other approvals are byte-identical. No unexplained difference was approved.
- **Verification:**
  - The explicit Debug solution analyzer rebuild (`dotnet build Hastlayer.SDK.sln --configuration Debug -t:Rebuild
    --warnaserror -p:TreatWarningsAsErrors=True -p:RunAnalyzersDuringBuild=true`) passed with zero warnings/errors.
  - Debug Arithmetics tests: 375/375. Full Debug Hast.Transformer.Vhdl.Tests: 59/59.
  - The NuGetTest Release build with the same warning/analyzer flags (also with `-t:Rebuild`) passed with zero
    warnings/errors.
  - Debug `dotnet test NuGetTest\Hastlayer.SDK.NuGet.sln` passed 1/1 against published packages, with
    `PositMultiplicationCalculator` added to the source-only sample exclusions.
  - Two actual `GenerateArithmeticSources` passes for both projects (the first forced by touching the three templates,
    the second after deleting all 34 project-owned outputs: 18 implementation and 16 test-project files) reproduced
    every output against the pre-pass SHA-256 manifest. CRLF was verified, and no skipped target was counted.
