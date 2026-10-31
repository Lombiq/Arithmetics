# 19: Implement exp2Minus1

**What to build:** Correctly rounded 2^x - 1 under section 5.5, evaluated as one mathematical expression.

**Blocked by:** 18: Deliver certified bounded exp2 for posit16 and posit32.

**Status:** ready-for-agent

- [ ] Cover E2 8/16/32 and Posit32; NaR propagates, zero is exact, and finite range handling follows section 4.1.
- [ ] Preserve cancellation-sensitive tiny inputs and limiting behavior for large negative inputs; do not subtract one from rounded exp2 output.
- [ ] Supply full-domain certified bounds, finite hardware termination, and exact/hard-rounding resolution without approximate fallback.
- [ ] Independently verify all posit8/16 inputs and posit32 zero-neighborhood, range-boundary, hard-rounding, and seeded vectors.
- [ ] Synthesize all precision kernels and compare simulated/device raw outputs with independent vectors.
- [ ] Update authoritative generation, public documentation, and coverage matrix; retain extensions and Posit32 compatibility.
