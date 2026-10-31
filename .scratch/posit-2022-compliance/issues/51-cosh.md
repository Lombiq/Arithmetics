# 51: Implement cosh

**What to build:** Correctly rounded hyperbolic cosine (standard cosH) under section 5.5.

**Blocked by:** 20: Implement exp.

**Status:** ready-for-agent

- [ ] Cover E2 8/16/32 and Posit32 with NaR propagation, exact value at zero, even symmetry, and section 4.1 saturation.
- [ ] Use certified internal exponential evaluation with one final rounding; avoid overflow caused only by an intermediate expression.
- [ ] Document constants/errors, finite hardware resource/termination bounds, and exact/hard-case resolution.
- [ ] Independently exhaust posit8/16; test posit32 near-zero rounding-to-one transitions, large arguments, and saturation/hard cases.
- [ ] Synthesize every precision and compare generated hardware raw outputs against independent expectations.
- [ ] Update generation, function documentation, correctness provenance, and coverage matrix.
