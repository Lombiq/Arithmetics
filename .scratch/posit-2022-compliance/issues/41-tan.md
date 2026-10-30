# 41: Implement tan

**What to build:** Correctly rounded radian tangent under sections 5.1 and 5.5.

**Blocked by:** 18: Deliver certified bounded exp2 for posit16 and posit32.

**Status:** ready-for-agent

- [ ] Cover E2 8/16/32 and Posit32 with NaR propagation, exact zero, and correct finite range handling.
- [ ] Certify radian reduction over the entire input range, preserving behavior near mathematical poles without rounding arguments or composing rounded sine/cosine.
- [ ] Document full error bounds, internal precision, bounded hardware termination, and complete hard-case resolution.
- [ ] Independently exhaust posit8/16; include posit32 largest arguments, pole-neighbor cases, tiny values, and seeded hard-rounding tests.
- [ ] Provide hardware entry points, actual synthesis evidence, and independent simulated/device comparisons at every precision.
- [ ] Update generation, mathematical/domain documentation, provenance, and coverage matrix.
