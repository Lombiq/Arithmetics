# 53: Implement asinh

**What to build:** Correctly rounded inverse hyperbolic sine (standard arcSinH) under section 5.5.

**Blocked by:** 26: Implement log.

**Status:** ready-for-agent

- [ ] Cover E2 8/16/32 and Posit32 with NaR propagation, exact zero, odd symmetry, and the standard real branch.
- [ ] Use certified internal evaluation without rounded log/root composition; preserve tiny-input cancellation and avoid intermediate overflow at large magnitudes.
- [ ] Record full-domain error bounds, finite hardware termination, and resolution of exact/hard-rounding cases.
- [ ] Independently exhaust posit8/16; test posit32 tiny values, regime changes, extremes, and difficult rounding.
- [ ] Synthesize each precision and compare independent numerical vectors with hardware raw outputs.
- [ ] Update authoritative generation, function/branch documentation, correctness provenance, and coverage matrix.
