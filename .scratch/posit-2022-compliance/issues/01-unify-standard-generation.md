# 01: Unify standard implementation generation without changing behavior

**What to build:** One authoritative source for equivalent Posit32 and Posit32E2 behavior, retaining existing public signatures and parameterized E0-E4 generation. This is the compatibility prefactor for the feature-complete, FPGA-synthesizable Posit Standard (2022) specification.

**Blocked by:** None (can start immediately).

**Status:** ready-for-agent

- [ ] Preserve the current public Posit32 API and all generated 8/16/32-bit E0-E4 types; avoid introducing numerical corrections in this refactor.
- [ ] Identify template ownership for implementations, helper overloads, and tests. Editing generated code is template-only: change the authoritative `.tt`/`.ttinclude` inputs and let the generator write the outputs. Never edit generated `.cs` files directly; distinguish currently hand-maintained Posit32 and Quire code.
- [ ] Make equivalent 32-bit algorithms originate from shared implementation or generation logic compatible with Hastlayer; document any intentionally different API adapters.
- [ ] Follow the documented template compiler constraints and output tracking. Include template changes with every affected regenerated implementation/test output; two actual MSBuild/T4 generation passes must produce identical files, not merely skip up-to-date templates.
- [ ] Compare pre/post-refactor public results on representative raw inputs and run existing posit, quire, extension, and Hastlayer transformation regressions.
- [ ] Confirm the shared realization still transforms and synthesizes for existing hardware entry points; distinguish unavailable evidence from passing results.
- [ ] Establish the compliance matrix's format/API inventory and record preserved behavior separately from verified standard compliance.
