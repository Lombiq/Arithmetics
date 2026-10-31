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
