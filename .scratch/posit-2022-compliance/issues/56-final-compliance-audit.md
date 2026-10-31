# 56: Close the compliance and hardware evidence matrix

**What to build:** An auditable feature-complete release candidate satisfying the full Posit Standard (2022) specification, including FPGA execution of every required operation and conversion.

**Blocked by:** All tickets 01-55, with their titles identifying the corresponding numbered local tickets.

**Status:** ready-for-agent

- [ ] Audit every normative requirement in sections 3-6 against public APIs, all E2 8/16/32 precisions, Posit32 equivalence, software evidence, and hardware evidence. No missing function, domain interpretation, or precision may be waived.
- [ ] Run aggregate exhaustive small-format, exhaustive unary posit16, wide hard-case, deterministic differential, and conversion/quire conformance checks with reproducible independent expectations.
- [ ] Verify ordinary evaluation order, explicit fusion, mixed-precision contracts, streaming/chunked accumulation, and lack of unintended operand mutation across software and hardware.
- [ ] Confirm every required operation/conversion already has bounded-resource implementation, actual synthesis evidence, and independent simulated/device numerical checks; approved HDL snapshots alone are insufficient.
- [ ] Distinguish generation, simulation, synthesis, and device evidence. Missing tools or unavailable hardware remain explicit blockers where required evidence is absent.
- [ ] Close every deferred FPGA-tool evidence gap recorded under the `## Comments` of tickets 01-55 with synthesis, simulation, or device evidence.
- [ ] Verify template-only editing: every generated implementation, helper, and test change must originate in authoritative `.tt`/`.ttinclude` inputs, with outputs written only by the generator. Direct edits to generated files are prohibited. Actually regenerate all outputs and confirm a second generation pass introduces no further changes; generated-only patches and incremental no-op builds do not satisfy this gate.
- [ ] Verify every corrected standard operation has one authoritative Posit32/Posit32E2 numerical implementation or generation source, with only explicit public API adapters; no duplicate legacy standard algorithms or temporary behavior-profile switches remain. Check compatibility evidence separately from independent standard-correctness evidence.
- [ ] Verify E0-E4 extension regressions, Posit32 compatibility, and directly related Hastlayer integration/build suites against the regenerated sources.
- [ ] Finish consumer documentation for the compliance profile, NaR, sentinel conversions, intentional behavior changes, decimal buffers, hardware calling conventions, and platform constraints.
- [ ] Check all algorithmic correctness/termination arguments and reference-vector provenance. No approximate fallback or software-only standard function may remain.
- [ ] Record measured synthesis/resource limitations without inventing performance targets; retain all agreed exclusions and leave the parent specification unchanged.
