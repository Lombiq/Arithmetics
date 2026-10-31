# 21: Implement expMinus1

**What to build:** Correctly rounded e^x - 1 under section 5.5 with one final rounding.

**Blocked by:** 20: Implement exp.

**Status:** ready-for-agent

- [ ] Cover E2 8/16/32 and Posit32, applying NaR, exact zero, and section 4.1 range semantics.
- [ ] Retain tiny positive/negative results and approach-to-minus-one boundaries; reuse certified internal exp machinery rather than subtracting from rounded public exp.
- [ ] Document certified errors, bounded resource/termination limits, and exact or certified hard-case completion.
- [ ] Exhaust posit8/16 with independent expectations; test posit32 cancellation, hard ties, range transitions, and seeded inputs.
- [ ] Synthesize every precision and compare hardware outputs against independent result bits.
- [ ] Update generated APIs, function documentation, and coverage matrix while preserving compatibility and extension regressions.
