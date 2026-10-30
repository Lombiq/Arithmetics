# 54: Implement acosh

**What to build:** Correctly rounded inverse hyperbolic cosine (standard arcCosH) under sections 5.1 and 5.5.

**Blocked by:** 26: Implement log.

**Status:** ready-for-agent

- [ ] Cover E2 8/16/32 and Posit32; document the standard domain/NaR rules, exact zero at one, and nonnegative branch.
- [ ] Preserve accuracy immediately above one and avoid large-input intermediate overflow; public rounded log/root compositions are insufficient.
- [ ] Certify complete-domain errors, finite hardware resource/termination bounds, and exact/hard-rounding resolution.
- [ ] Independently exhaust posit8/16 and test posit32 domain endpoint neighbors, extreme positives, and difficult rounding.
- [ ] Supply synthesized kernels and independent simulated/device bit checks at all precisions.
- [ ] Update generation, domain/API documentation, correctness evidence, and coverage matrix.
