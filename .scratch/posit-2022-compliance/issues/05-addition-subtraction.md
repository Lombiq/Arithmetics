# 05: Correct addition and subtraction

**What to build:** Correctly rounded ordinary addition and subtraction under sections 4 and 5.4, preserving their source-defined rounding boundaries.

**Blocked by:** 03: Correct rounding and standard precision conversions.

**Status:** ready-for-agent

- [ ] Cover E2 8/16/32 and Posit32; evaluate exact signed sums before section 4.1 rounding.
- [ ] Replace the divergent legacy 32-bit addition/subtraction paths with one authoritative standard algorithm/generation source used by Posit32 and Posit32E2; do not select either current intermediate-width behavior as the oracle.
- [ ] Handle NaR before zero shortcuts, exact cancellation, near cancellation, large scale differences, limb/word shift boundaries, and signed range clamping.
- [ ] Compare all 65,536 posit8 pairs per operation against independent rational evaluation; test adversarial and seeded posit16/32 pairs.
- [ ] Include expressions distinguishing separate operator rounding from explicit fusion; no reassociation may change results.
- [ ] Supply hardware entry points, synthesis evidence, and bit-exact simulated/device checks for both operations and rounding-sensitive expressions.
- [ ] Update authoritative generation, coverage matrix, migration notes, and non-E2 regression coverage.

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
