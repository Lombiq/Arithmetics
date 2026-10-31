# 37: Implement cosPi

**What to build:** Correctly rounded cos(pi*x) under section 5.5.

**Blocked by:** 18: Deliver certified bounded exp2 for posit16 and posit32.

**Status:** ready-for-agent

- [ ] Cover E2 8/16/32 and Posit32 with NaR propagation, exact integer/half-integer values, even symmetry, and full-range periodic reduction.
- [ ] Use mathematical pi and exact input reduction rather than multiplication by a rounded constant or rounded sine-phase shifting.
- [ ] Certify error bounds, internal precision, bounded hardware termination, and all exact/hard-rounding cases.
- [ ] Independently exhaust posit8/16; include posit32 large representable integers, root neighborhoods, tiny arguments, and difficult-rounding vectors.
- [ ] Provide synthesized kernels and independent numerical hardware checks at every precision.
- [ ] Update generation, function documentation, certified provenance, and coverage matrix; preserve existing extensions.
