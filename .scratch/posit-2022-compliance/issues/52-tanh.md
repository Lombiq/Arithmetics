# 52: Implement tanh

**What to build:** Correctly rounded hyperbolic tangent (standard tanH) under section 5.5.

**Blocked by:** 20: Implement exp.

**Status:** ready-for-agent

- [ ] Cover E2 8/16/32 and Posit32 with NaR propagation, exact zero, odd symmetry, and correct rounding near plus/minus one.
- [ ] Reuse certified internal machinery, avoiding division of rounded sinh/cosh or overflowing intermediate exponentials.
- [ ] Document full-domain certified errors, finite hardware bounds, and complete hard-rounding resolution.
- [ ] Independently exhaust posit8/16; test posit32 tiny inputs, approach-to-one transition points, extremes, and seeded hard cases.
- [ ] Supply synthesized kernels and independent simulated/device raw-result checks at every precision.
- [ ] Update generation, public contracts, provenance, and coverage matrix.
