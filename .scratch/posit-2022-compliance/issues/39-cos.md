# 39: Implement cos

**What to build:** Correctly rounded radian cosine under section 5.5.

**Blocked by:** 18: Deliver certified bounded exp2 for posit16 and posit32.

**Status:** ready-for-agent

- [ ] Cover E2 8/16/32 and Posit32, including NaR, exact cos(0), even symmetry, and section 4.1 rounding.
- [ ] Certify full-range radian reduction without rounded phase shifts or rounded public function composition.
- [ ] Record constants, complete error bounds, finite hardware resource/termination limits, and exact/hard-case resolution.
- [ ] Independently exhaust posit8/16; include posit32 max-scale arguments, quadrant/root neighborhoods, tiny inputs, and hard-rounding cases.
- [ ] Provide synthesized kernels with independent simulated/device numerical evidence for all precisions.
- [ ] Update authoritative generation, function documentation, correctness provenance, and coverage matrix.
