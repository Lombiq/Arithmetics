# 04: Complete basic unary operations

**What to build:** Negate, abs, sign, nearestInt, ceil, floor, next, and prior as public standard operations under section 5.2.

**Blocked by:** 03: Correct rounding and standard precision conversions.

**Status:** ready-for-agent

- [ ] Cover E2 8/16/32 and Posit32; sign returns a posit. Apply NaR propagation except the specified next/prior representation traversal.
- [ ] NearestInt ties choose the nearest even integer-valued posit; ceil/floor work for negative values and already integral large values.
- [ ] Verify next/prior wraparound, transitions through NaR, zero, minPos, and both signed extremes.
- [ ] Exhaust all posit8 and posit16 inputs against independent exact expectations; add posit32 integer-boundary and seeded random tests.
- [ ] Supply bounded hardware entry points for all eight operations, actual synthesis evidence, and independent hardware numerical checks.
- [ ] Update generation, operation/API mapping, behavior documentation, and extension regressions.
