# 56: Close the compliance and hardware evidence matrix

**What to build:** An auditable feature-complete release candidate satisfying the full Posit Standard (2022) specification, including FPGA execution of every required operation and conversion.

**Blocked by:** All tickets 01-55, with their titles identifying the corresponding numbered local tickets.

**Status:** ready-for-agent

- [ ] Audit every normative requirement in sections 3-6 against public APIs, all E2 8/16/32 precisions, Posit32 equivalence, software evidence, and hardware evidence. No missing function, domain interpretation, or precision may be waived.
- [ ] Run aggregate exhaustive small-format, exhaustive unary posit16, wide hard-case, deterministic differential, and conversion/quire conformance checks with reproducible independent expectations.
- [ ] Verify ordinary evaluation order, explicit fusion, mixed-precision contracts, streaming/chunked accumulation, and lack of unintended operand mutation across software and hardware.
- [ ] Confirm every required operation/conversion already has bounded-resource implementation, actual synthesis evidence, and independent simulated/device numerical checks; approved HDL snapshots alone are insufficient.
- [ ] Distinguish generation, simulation, synthesis, and device evidence. Missing tools or unavailable hardware remain explicit blockers where required evidence is absent.
- [ ] Verify regeneration consistency, E0-E4 extension regressions, Posit32 compatibility, and directly related Hastlayer integration/build suites.
- [ ] Finish consumer documentation for the compliance profile, NaR, sentinel conversions, intentional behavior changes, decimal buffers, hardware calling conventions, and platform constraints.
- [ ] Check all algorithmic correctness/termination arguments and reference-vector provenance. No approximate fallback or software-only standard function may remain.
- [ ] Record measured synthesis/resource limitations without inventing performance targets; retain all agreed exclusions and leave the parent specification unchanged.
