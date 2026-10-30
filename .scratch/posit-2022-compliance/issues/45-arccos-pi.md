# 45: Implement arcCosPi

**What to build:** Correctly rounded arccos(x)/pi under section 5.5.

**Blocked by:** 44: Implement arcCos.

**Status:** ready-for-agent

- [ ] Cover E2 8/16/32 and Posit32 with domain/NaR behavior, range zero to one, and exact endpoints/central identities.
- [ ] Evaluate scaling before final rounding using certified internal arcCos machinery; no division of already-rounded outputs.
- [ ] Document certified errors, finite hardware resource/termination limits, and complete hard-case resolution.
- [ ] Independently exhaust posit8/16; test posit32 endpoint neighborhoods and scaled hard-rounding cases.
- [ ] Provide synthesized kernels and independent simulated/device bit comparisons at all precisions.
- [ ] Update generation, scaled-function documentation, correctness evidence, and coverage matrix.
