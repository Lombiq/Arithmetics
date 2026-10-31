# 06: Correct multiplication

**What to build:** Correctly rounded ordinary multiplication under sections 4 and 5.4.

**Blocked by:** 03: Correct rounding and standard precision conversions.

**Status:** ready-for-agent

- [ ] Cover E2 8/16/32 and Posit32, preserving complete product information before one section 4.1 rounding.
- [ ] Replace the divergent 32-bit product paths with one authoritative standard algorithm/generation source used by Posit32 and Posit32E2; validate against independent exact products, not either legacy result.
- [ ] Propagate NaR before zero shortcuts, including zero times NaR; test signed products, exact powers, nonzero underflow, finite overflow, and exponent/regime transitions.
- [ ] Exhaust all posit8 pairs using independent exact products; include posit16/32 tie, sticky-bit, extreme-scale, and seeded random vectors.
- [ ] Supply bounded product kernels and hardware entry points with actual synthesis and independent simulated/device checks.
- [ ] Update parameterized generation, operation documentation, compatibility notes, and software/hardware coverage matrix; retain extension regression coverage.
