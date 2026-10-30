# 07: Correct division

**What to build:** Correctly rounded division under sections 4, 5.1, and 5.4, retaining enough quotient/remainder information for exact rounding decisions.

**Blocked by:** 03: Correct rounding and standard precision conversions.

**Status:** ready-for-agent

- [ ] Cover E2 8/16/32 and Posit32; zero divided by zero and any arithmetic involving NaR return NaR as specified.
- [ ] Correctly handle nonzero divided by zero, exact quotients, nonterminating quotients, signed range extremes, and remainder-sensitive ties.
- [ ] Exhaust all posit8 pairs against independent rational results; include wide-format adversarial and seeded random checks.
- [ ] Document bounded divider precision/termination and prove discarded remainders cannot corrupt rounding.
- [ ] Provide hardware entry points, synthesis evidence, and independent simulated/device raw-result checks.
- [ ] Update generation, behavior-change documentation, coverage matrix, and extension regressions.
