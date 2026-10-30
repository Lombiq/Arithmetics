# 48: Implement arcTan2

**What to build:** Correctly rounded argument of real + i*imaginary under sections 5.1 and 5.6.

**Blocked by:** 46: Implement arcTan.

**Status:** ready-for-agent

- [ ] Cover E2 8/16/32 and Posit32 with the standard real-first argument order, explicitly distinguished from .NET Atan2.
- [ ] Handle all quadrants, NaR, axes, origin-to-NaR, and positive rounded pi on the negative real axis with zero imaginary part.
- [ ] Reuse certified internal angle evaluation without rounded division/public arcTan composition; preserve extreme component ratios.
- [ ] Document full-domain certified bounds and finite hardware termination. Independently exhaust posit8 pairs and test wide axis/branch neighbors, extreme ratios, and hard rounding.
- [ ] Synthesize all precision entry points and compare hardware results against independent quadrant-aware expectations.
- [ ] Update generation, calling-order/domain documentation, correctness evidence, and coverage matrix.
