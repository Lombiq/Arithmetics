# 34: Implement compound

**What to build:** Correctly rounded (1 + x)^integer under sections 5.1 and 5.8.

**Blocked by:** 18: Deliver certified bounded exp2 for posit16 and posit32; 12: Complete 8-64-bit integer conversions.

**Status:** ready-for-agent

- [ ] Cover E2 8/16/32 and Posit32 with a signed exponent contract consistent with established integer adapters; define domains at x=-1 and signed exponent extrema.
- [ ] Preserve the exact increment and integer power until final rounding, including tiny increments, negative bases, parity, negative exponents, and NaR.
- [ ] Document certified error/range handling and finite hardware termination for the entire supported exponent type, including its minimum value.
- [ ] Independently check every posit8 input with representative exponents and extrema; add wide cancellation-sensitive, boundary, and hard-rounding vectors.
- [ ] Synthesize every precision and compare hardware raw results against independent expectations.
- [ ] Update generation, API/domain documentation, rounding interpretation, and coverage matrix.
