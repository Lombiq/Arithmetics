# 50: Implement sinh

**What to build:** Correctly rounded hyperbolic sine (standard sinH) under section 5.5.

**Blocked by:** 20: Implement exp.

**Status:** ready-for-agent

- [ ] Cover E2 8/16/32 and Posit32 with NaR propagation, exact zero, odd symmetry, and finite section 4.1 saturation.
- [ ] Use certified internal exponential machinery without subtracting rounded exponentials; avoid cancellation for tiny inputs and spurious intermediate overflow.
- [ ] Record full-domain error bounds, finite hardware termination, and exact/hard-rounding resolution.
- [ ] Independently exhaust posit8/16; test posit32 tiny, moderate, saturation-boundary, and hard-rounding inputs.
- [ ] Provide synthesized entry points and independent simulated/device numerical checks for every precision.
- [ ] Update generation, standard-name mapping, correctness documentation, and coverage matrix.
