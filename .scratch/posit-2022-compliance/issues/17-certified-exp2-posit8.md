# 17: Deliver certified bounded exp2 for posit8 E2

**What to build:** A narrow end-to-end certified elementary-function realization: section 5.5 exp2 for Posit8E2 in software and actual synthesized hardware.

**Blocked by:** 03: Correct rounding and standard precision conversions.

**Status:** ready-for-agent

- [ ] Establish independent elementary-function expectations using validated high-precision enclosures or another justified independent method; pin tooling and reproducible vector provenance.
- [ ] Correctly evaluate 2^x for all 256 inputs, including NaR, exact powers, finite overflow, and nonzero underflow under section 4.1.
- [ ] Document the algorithm/table strategy, error bounds, internal widths, finite termination, exact-boundary handling, and certified hard-case resolution. An empirical guard-bit count is insufficient.
- [ ] Exhaustively compare public outputs with independent expectations; reference tooling cannot be called by production FPGA kernels.
- [ ] Generate, synthesize, and numerically check the hardware realization; record latency/resources without inventing performance targets.
- [ ] Preserve E0-E4 generation and update operation/API/evidence mapping. Explicitly identify what remains to be established for wide formats; a small table is not proof of wide-format feasibility.
