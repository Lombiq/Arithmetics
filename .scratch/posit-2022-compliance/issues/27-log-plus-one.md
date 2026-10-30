# 27: Implement logPlus1

**What to build:** Correctly rounded natural log(1 + x) under section 5.5.

**Blocked by:** 26: Implement log.

**Status:** ready-for-agent

- [ ] Cover E2 8/16/32 and Posit32 with documented domains, NaR propagation, and exact zero.
- [ ] Evaluate the exact increment and logarithm as one expression, retaining tiny inputs and valid inputs immediately above minus one.
- [ ] Reuse certified internal log evaluation, not rounded public calls; justify bounded hardware resources, termination, and hard-case rounding resolution.
- [ ] Exhaust posit8/16 using independent expectations; test posit32 cancellation, domain boundary, extreme, and seeded hard cases.
- [ ] Synthesize all precision kernels and compare generated hardware result bits with independent vectors.
- [ ] Update generation, public behavior documentation, correctness evidence, and coverage matrix.
