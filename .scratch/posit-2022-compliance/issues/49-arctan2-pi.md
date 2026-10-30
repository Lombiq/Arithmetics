# 49: Implement arcTan2Pi

**What to build:** Correctly rounded argument(real + i*imaginary)/pi under section 5.6.

**Blocked by:** 48: Implement arcTan2.

**Status:** ready-for-agent

- [ ] Cover E2 8/16/32 and Posit32, preserving real-first arguments, quadrant/axis conventions, NaR propagation, and origin-to-NaR.
- [ ] Preserve exact scaled axis/diagonal identities and scale the internal certified angle before final rounding, not the public rounded angle.
- [ ] Certify combined error bounds, finite hardware resources/termination, and hard cases across branch cuts and extreme ratios.
- [ ] Independently exhaust posit8 pairs; test wide-format axis neighbors, exact identities, and scaled rounding boundaries.
- [ ] Synthesize every precision and compare hardware outputs with independent raw expectations.
- [ ] Update generation, public domain/calling-order documentation, provenance, and coverage matrix.
