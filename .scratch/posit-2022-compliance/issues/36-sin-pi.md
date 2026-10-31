# 36: Implement sinPi

**What to build:** Correctly rounded sin(pi*x) under section 5.5.

**Blocked by:** 18: Deliver certified bounded exp2 for posit16 and posit32.

**Status:** ready-for-agent

- [ ] Cover E2 8/16/32 and Posit32; propagate NaR and preserve exact integer/half-integer identities, signs, and periodicity.
- [ ] Reduce the exact dyadic input before evaluating mathematical pi scaling; a rounded posit/IEEE pi product is insufficient.
- [ ] Document certified constants/errors, full-range reduction, bounded hardware termination, and exact/hard-case rounding resolution.
- [ ] Independently exhaust posit8/16; test posit32 large integers, half-integer neighbors, tiny inputs, and hard-rounding vectors.
- [ ] Synthesize all precision kernels and compare hardware outputs with independent raw expectations.
- [ ] Update generation, mathematical contracts, provenance, extension regressions, and coverage matrix.
