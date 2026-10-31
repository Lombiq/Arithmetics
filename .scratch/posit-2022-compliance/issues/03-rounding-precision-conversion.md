# 03: Correct rounding and standard precision conversions

**What to build:** Public exact widening and correctly rounded narrowing between standard precisions, exercising a reliable section 4.1 rounding foundation through section 6.1 conversions.

**Blocked by:** 02: Certify encoding, NaR, and comparisons.

**Status:** ready-for-agent

- [ ] Implement all six directed E2 8/16/32 precision conversions, including Posit32 compatibility. Widen by appending zero bits; preserve zero and NaR.
- [ ] Implement the literal extended-representation boundary and even-LSB tie rule, retaining sticky/discarded information. Do not assume adjacent arithmetic midpoints are equivalent.
- [ ] Make each corrected 32-bit rounding/conversion algorithm authoritative for both Posit32 and Posit32E2; any signature-specific constructor or conversion adapter must delegate to the same numerical implementation.
- [ ] Preserve exact zero; clamp finite overflow to signed maxPos and nonzero underflow to signed minPos.
- [ ] Extend the independent oracle with exact rational rounding; hand-check regime-boundary ties and values on either side. Exhaust small-source inputs and test wide-source hard boundaries.
- [ ] Supply hardware conversion entry points, bounded storage, synthesis evidence, and independent simulated/device output comparisons.
- [ ] Update parameterized generation, conversion documentation, and coverage matrix; verify Posit32/E2 agreement and extension regressions.
