# 20: Implement exp

**What to build:** Correctly rounded natural exponential under section 5.5.

**Blocked by:** 18: Deliver certified bounded exp2 for posit16 and posit32.

**Status:** ready-for-agent

- [ ] Cover E2 8/16/32 and Posit32, with exp(0) exact, NaR propagation, and section 4.1 finite saturation.
- [ ] Evaluate mathematical e^x directly or with certified internal transformations; rounded posit constants/base changes are not valid substitutes.
- [ ] Justify reduction constants, approximation bounds, internal widths, finite hardware termination, and resolution of every rounding case.
- [ ] Independently check all posit8/16 inputs plus posit32 hard cases, near-zero inputs, saturation transitions, and seeded vectors.
- [ ] Supply hardware entry points, actual synthesis evidence, and simulated/device numerical comparisons for every precision.
- [ ] Update generation, API documentation, certified machinery provenance, and compliance matrix; retain extension regressions.
