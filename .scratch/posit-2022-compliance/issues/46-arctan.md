# 46: Implement arcTan

**What to build:** Correctly rounded principal inverse tangent under sections 5.1 and 5.5.

**Blocked by:** 18: Deliver certified bounded exp2 for posit16 and posit32.

**Status:** ready-for-agent

- [ ] Cover E2 8/16/32 and Posit32 with NaR propagation, exact zero, odd symmetry, and the specified rounded principal range.
- [ ] Evaluate tiny and extreme inputs without losing information through premature reciprocal or constant rounding.
- [ ] Document certified reduction/error bounds, finite hardware termination, and exact/hard-case resolution; make reusable internal evaluation available to the approved scaled/quadrant follow-ups.
- [ ] Independently exhaust posit8/16; include posit32 near-one, tiny, extreme, and difficult-rounding cases.
- [ ] Synthesize all precision kernels and compare independent result bits in simulation/device execution.
- [ ] Update generation, branch/range documentation, provenance, and coverage matrix.
