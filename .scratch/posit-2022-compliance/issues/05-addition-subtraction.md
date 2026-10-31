# 05: Correct addition and subtraction

**What to build:** Correctly rounded ordinary addition and subtraction under sections 4 and 5.4, preserving their source-defined rounding boundaries.

**Blocked by:** 03: Correct rounding and standard precision conversions.

**Status:** done

- [x] Cover E2 8/16/32 and Posit32; evaluate exact signed sums before section 4.1 rounding.
- [x] Replace the divergent legacy 32-bit addition/subtraction paths with one authoritative standard algorithm/generation source used by Posit32 and Posit32E2; do not select either current intermediate-width behavior as the oracle.
- [x] Handle NaR before zero shortcuts, exact cancellation, near cancellation, large scale differences, limb/word shift boundaries, and signed range clamping.
- [x] Compare all 65,536 posit8 pairs per operation against independent rational evaluation; test adversarial and seeded posit16/32 pairs.
- [x] Include expressions distinguishing separate operator rounding from explicit fusion; no reassociation may change results.
- [x] Supply bounded hardware entry points for both operations and rounding-sensitive expressions, independent CPU output checks and Hastlayer/VHDL transformation.
- [ ] Supply actual vendor synthesis evidence and bit-exact simulated/device checks (deferred to ticket 56).
- [x] Update authoritative generation, coverage matrix, migration notes, and non-E2 regression coverage.

## Comments

- **Pre-implementation contract:** Read the original [Posit Standard (2022)](https://posithub.org/docs/posit_standard-2.pdf),
  sections 2 (p. 5), 3.2-3.3 (pp. 6-7), 4.1-4.3 (p. 8), 5.1, 5.2 footnote 7 and 5.4 (p. 9); checked the rendered
  encoding equation (p. 7) and the section 4.1 algorithm (p. 8). For E2 posit8/16/32 and the `Posit32` surface,
  `addition(a, b)` returns the section 4.1 rounding of the exact real `a + b`; `subtraction(a, b)` that of `a - b`.
  Operands and result share one precision (section 5.1). Any NaR operand returns NaR, checked before any zero
  shortcut. Otherwise the sum is an exact signed dyadic (an integer multiple of minPos, section 3.2), with no
  intermediate rounding, truncation or width-dependent loss. Exact zero (including `a - a`) returns zero; an exact
  sum returns its encoding; finite `|x| > maxPos` returns signed maxPos; nonzero `|x| < minPos` returns signed minPos;
  otherwise return `u` if `u < x < v`, or `x == v` and LSB(`U`) is 0, else `w`, where `v` is the (n+1)-bit posit `U1`.
  Negation is the exact 2's complement (section 5.2, footnote 7), so `a - b` is one rounding of exact `a + (-b)`.
- Sections 4.2-4.3: every ordinary `+`/`-` rounds exactly once at its source-defined boundary; expressions such as
  `(a + b) - a` round after each operator and are never reassociated or implicitly fused. Explicit fusion
  (`FusedSum`/quire) stays separate and is not certified here (tickets 08-10). The `int` operand overloads keep their
  written meaning: convert first (integer conversion is ticket 12), then perform one standard addition.
- **Implementation constraints:** one template emitter is the authoritative algorithm for `Posit8E2`, `Posit16E2`,
  `Posit32E2` and the generated `Posit32` partial; neither legacy intermediate width is an oracle. The kernel uses
  bounded fixed-width integers. Alignment is exact while the scale gap fits; beyond that the smaller addend's discarded
  bits become a sticky bit at a guard precision fine enough that no n- or (n+1)-bit boundary lies between the exact
  and jammed sums. That equivalence is evidenced by the independent tests below, not assumed. Non-E2 extension types
  keep their legacy addition.
- **Independent evidence plan:** expected results use ticket 02's `PositEncodingReference` decoder, exact
  `BigInteger` sums, and the shared section 4.1 rounding oracle `PositRoundingReference.Round` (owned by this ticket,
  reused by ticket 06), which decodes `U1` as an (n+1)-bit posit and uses no production arithmetic, arithmetic
  midpoints or IEEE casts. Vectors: all 65,536 posit8 pairs for both `+` and `-`; posit16/32 NaR/zero combinations,
  exact and near cancellation, adjacent encodings, signed maxPos/minPos clamping, scale gaps across the exact/sticky
  threshold and word boundaries up to the maximum span, exact `U1` ties and neighbors in both signs, and seeded pairs;
  bit-identical `Posit32`/`Posit32E2` results; and expressions where separate rounding differs from `FusedSum`.
- **Shared oracle:** `PositRoundingReference.Round(PositEncodingReference, int)` is the shared section 4.1 rounding
  oracle for arbitrary exact results (isolated submodule commit `8aa193e`; ticket 06 reuses it). `Round` clamps to
  signed maxPos/minPos, then compares the exact value with `U1`, decoded as an (n+1)-bit posit, and resolves ties
  to the even LSB. `PositEncodingReference.FromDyadic` represents any exact signed dyadic without `Decode`, and
  `Decode(ulong, int)` accepts 2-64-bit widths. `181bdc8` only suppresses S3776 on `Round`, because the file is linked
  into the SDK VHDL tests under SDK analyzers. In ticket 07's approved design, the existing dyadic overload delegates
  to the single rational-rounding implementation exposed by `Round(BigInteger, BigInteger, int)`, preserving existing
  callers and semantics. Both overloads must pass the same analyzers.
  `PositRoundingReferenceTests` tests `Round` directly:
  - For 8 and 16 bits, every encoding round-trips exactly.
  - Every adjacent pair `u`/`w` (both signs, outside the minPos clamp) checks values just above `u`, just below and
    just above the `U1` boundary `v`, and just below `w`. The `v` tie is checked for both LSB parities, each counted
    maxPos - 1 times.
  - Finite overflow clamps to signed maxPos and nonzero underflow to signed minPos (the clamps are also checked for
    32 bits).
  - Hand-derived posit8/32 anchors and seeded posit32 boundaries complete the coverage.
- **Interpretations:** The minPos clamp is unreachable for addition/subtraction: posit values are integer multiples
  of minPos (section 3.2, p. 6), so a nonzero exact sum is at least minPos in magnitude. Tests assert this rather
  than emulate it. Near the extreme regimes, truncated exponent bits leave some powers of two unrepresentable
  (section 3.3, p. 7). Scale-difference coverage therefore spans every difference between representable powers of
  two. The `int` overloads convert first and then add once; their conversion is certified by ticket 12. `FusedSum`
  is only used as the distinguishing fused value here; its correctness belongs to tickets 08-10.
- **Deferred evidence:** Vendor synthesis, simulation, device execution and hardware-dependent DynamicTests remain
  unchecked for ticket 56. Bounded raw-memory CPU checks and Hastlayer/VHDL transformation are in scope.
- **Implementation:** `GenerateAdditionMember` in `PositTemplate.tt` emits one `AddBits` kernel:
  - It is used by the E2 `operator +` of `Posit8E2`/`Posit16E2`/`Posit32E2` and by the generated `Posit32` partial.
  - The hand-maintained `Posit32.cs` keeps only the `operator +` adapter, followed by the `AddBits` partial
    declaration. Both lie after the existing `Abs`/`GetTwosComplement` partial anchors (lines 527/551), which
    do not move.
  - Subtraction remains `left + -right`, with exact two's-complement negation.
  - NaR is checked before the zero shortcuts.
  - Magnitudes are decoded into a word twice the posit size (at least 32 bits). While the scale gap is at most
    n - 2, the smaller significand is shifted into alignment and any shifted-out bits are ORed into a sticky bit.
    Beyond that, the whole smaller addend becomes the sticky bit.
  - The sum is laid out as an unbounded encoding and rounded to nearest, ties to even, at the n-bit boundary.
    Because encodings are ordered like values, this is the section 4.1 `U1` comparison. Scales of maxPos or above
    clamp to maxPos.
  - The kernel uses only fixed-width integers, with bounded regime-run and leading-one loops. It is marked
    AggressiveInlining, and with Hastlayer method inlining enabled it is fully inlined (see the audit below).
  - Non-E2 (E0/E1/E3/E4) types keep the legacy addition and its existing regression tests.
- **Correction audit:** A throwaway comparison compiled the pre-ticket generated/hand-written sources (`a6026fe`)
  against the independent vectors:
  - Legacy E2 8/16-bit addition and subtraction already matched on all 65,536 posit8 pairs and all 32,224
    posit16 hard/seeded pairs. Their results are unchanged.
  - Of the 43,616 posit32 hard/seeded pairs, legacy `Posit32E2` was wrong on 6,881 additions and 6,889
    subtractions. In every case an operand lay in the extreme regimes (|scale| >= 100), where exponent/fraction
    bits are truncated. 11,740 of these 13,770 results had scale gaps over 30, and 13,695 were off by one ulp. For
    example, minPos (`0x1`, 2^-120) + `0x2` (2^-116) rounded up to `0x3` (2^-114), although the exact 17 * 2^-120 is
    below the `U1` boundary 2^-115.
  - Legacy hand-written `Posit32` was wrong on 3,332 of those pairs per operation (6,664 results), including
    mid-range one-ulp errors at gaps of 4-30 (1,437) and multi-ulp errors at gaps over 30 (2,432). It was also
    wrong on 18,359 of 2,000,000 mixed uniform/near-cancellation/near-scale random sums, including gross errors
    near -maxPos.
  - The two legacy 32-bit kernels disagreed with each other on exactly those 18,359 random sums.
  - Both are now one bit-identical standard kernel; no legacy behavior served as an oracle.
- **Independent tests:** `PositAdditionReference` sums decoded operands exactly in BigInteger and rounds once with
  the shared oracle. It uses no production arithmetic, arithmetic midpoints or IEEE casts. Hand-derived anchors
  validate it separately.
  - Generated E2 Facts check every posit8 pair, 32,224 posit16 and 43,616 posit32 pairs for both operations.
    Larger sizes cover NaR/zero combinations, regime boundaries, every scale gap to the maximum span, `U1` ties and
    neighbors for both parities and signs, exact/near cancellation, and seeded pairs.
  - `Posit32` is checked against the same reference and bit-for-bit against `Posit32E2`.
  - `HardPairsReachEveryRoundingCase` proves the 16/32-bit vectors reach both signed maxPos clamps, rounding to
    signed minPos, both tie directions, NaR with zero, exact and near cancellation, inexact alignment and every
    representable scale difference.
  - Expressions `(a + b) - c`, `a + (b - c)` and `(a - c) + b` round in source order for 4 distinguishing triples,
    whose separate rounding differs from the explicit fused value (`FusedSum` itself agrees with that value), and
    2,048 seeded triples per size.
- **Hardware/approval audit:** `PositAdditionCalculator` has four entry points: `AddPosit8`, `AddPosit16`,
  `AddPosit32E2` and `AddPosit32`.
  - Each uses eight 32-bit cells: a, b and c in cells 0-2, and a + b, a - b and the three expressions in cells 3-7.
  - A VHDL-test CPU theory checks every output against the linked independent reference over all posit8 pairs and the
    hard/seeded/expression vectors above, including `Posit32` at 32 bits.
  - The new Catapult approval contains four entry-point machines. Per type it has `op_Addition` with `AddBits`
    inlined, `op_Subtraction` calling `op_Addition`/`op_UnaryNegation`, and `op_UnaryNegation`. The `Posit32` and
    `Posit32E2` `op_Addition` machines are identical up to type names and bijective local renaming.
  - Six existing snapshots changed: Posit32Sample and Posit32SampleWithInlining for Catapult, Alveo U50 and
    Nexys4 DDR. They were audited block by block, with Xilinx mirroring Catapult.
  - Without inlining, 265 of the 477 old blocks are identical and 29 differ only by bijective renaming. The
    legacy `op_Addition` helper call tree is removed (IsNaN, IsZero, regime/exponent/fraction helpers,
    CalculateScaleFactor, op_GreaterThan, CompareTo instances and proxies). Five `AddBits` machines and a proxy are
    added. `op_Addition` shrinks from 2,195 to 86 lines. The remaining proxy changes only drop removed callers or add
    `AddBits` as a `GetTwosComplement` caller.
  - With inlining, `AddBits` is fully inlined (no machine or `GetTwosComplement` call remains), `op_Addition`
    shrinks from 4,313 to 2,011 lines, and op_GreaterThan is removed. The other 1,504 identifier differences are
    bijective renumbering.
  - All other approvals are byte-identical. No unexplained difference was approved.
- **Verification:**
  - The explicit Debug solution analyzer rebuild (`dotnet build Hastlayer.SDK.sln --configuration Debug -t:Rebuild
    --warnaserror -p:TreatWarningsAsErrors=True -p:RunAnalyzersDuringBuild=true`) passed with zero warnings/errors.
  - Debug Arithmetics tests: 354/354. Full Debug Hast.Transformer.Vhdl.Tests: 55/55.
  - The NuGetTest Release build with the same warning/analyzer flags passed with zero warnings/errors.
  - Debug `dotnet test NuGetTest\Hastlayer.SDK.NuGet.sln` passed 1/1 against published packages, with
    `PositAdditionCalculator` added to the source-only sample exclusions.
  - Two actual `GenerateArithmeticSources` passes for both projects, forced by refreshing the shared include
    timestamp, rewrote and reproduced all 34 project-owned outputs against the pre-pass SHA-256 manifest. CRLF was
    verified, and no skipped target was counted.
