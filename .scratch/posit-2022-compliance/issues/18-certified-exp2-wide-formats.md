# 18: Deliver certified bounded exp2 for posit16 and posit32

**What to build:** Complete section 5.5 exp2 across standard precisions and establish reusable bounded evaluation/reference machinery for subsequent elementary functions.

**Blocked by:** 17: Deliver certified bounded exp2 for posit8 E2.

**Status:** ready-for-agent

- [ ] Implement Posit16E2, Posit32E2, and Posit32 with identical standard semantics; retain certified posit8 behavior.
- [ ] Justify full-domain range reduction, internal precision, certified error bounds, finite refinement/termination, and exact-boundary or hard-case resolution for each wide format.
- [ ] No host math/native multiprecision calls or approximate fallback may resolve production hard cases. An unresolved finite hardware strategy blocks this ticket and its dependents.
- [ ] Exhaust all posit16 inputs and independently verify posit32 exact powers, saturation boundaries, difficult rounding inputs, and seeded samples.
- [ ] Synthesize wide-format kernels and compare independent numerical vectors in simulation/device execution; report actual evidence and resource costs.
- [ ] Expose reusable internal machinery only where justified, retain public compatibility and parameterized generation, and update correctness documentation and coverage matrix.
