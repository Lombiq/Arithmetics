# 38: Implement sin

**What to build:** Correctly rounded radian sine under section 5.5.

**Blocked by:** 18: Deliver certified bounded exp2 for posit16 and posit32.

**Status:** ready-for-agent

- [ ] Cover E2 8/16/32 and Posit32 with NaR propagation, exact zero, and section 4.1 output rounding.
- [ ] Certify argument reduction over the entire input range, including maxPos; do not pass through a rounded public pi-scaled function.
- [ ] Document reduction constants, precision/error bounds, finite hardware termination, and hard cases near roots and extrema.
- [ ] Independently exhaust posit8/16; test posit32 extreme arguments, quadrant boundaries, tiny inputs, seeded inputs, and hard rounding.
- [ ] Synthesize every precision and compare generated hardware raw outputs against independent expected values.
- [ ] Update generation, range-reduction provenance, API documentation, and coverage matrix.
