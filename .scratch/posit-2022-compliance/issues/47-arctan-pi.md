# 47: Implement arcTanPi

**What to build:** Correctly rounded arctan(x)/pi under section 5.5.

**Blocked by:** 46: Implement arcTan.

**Status:** ready-for-agent

- [ ] Cover E2 8/16/32 and Posit32 with NaR propagation, exact zero, odd symmetry, and the specified half-unit range bound.
- [ ] Scale certified internal inverse-tangent evaluation before final rounding; rounded public arcTan/pi division is insufficient.
- [ ] Certify combined constants/errors, finite hardware bounds, and exact/hard-case resolution, including approach-to-one-half transitions.
- [ ] Independently exhaust posit8/16; test posit32 exact identities, tiny/extreme inputs, and scaled rounding boundaries.
- [ ] Supply synthesized entry points with independent hardware numerical checks for all precisions.
- [ ] Update generation, pi-scaled contracts, provenance, and coverage matrix.
