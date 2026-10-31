# 11: Implement fused three-factor multiplication

**What to build:** Section 5.7 fMM, the exact product of three posit inputs rounded once, distinct from the existing four-input product-difference helper.

**Blocked by:** 06: Correct multiplication.

**Status:** ready-for-agent

- [ ] Cover E2 8/16/32 and Posit32; no intermediate posit rounding.
- [ ] Implement the corrected 32-bit fMM kernel once for Posit32 and Posit32E2 (and the E2 8/16-bit generated surfaces), with only explicit public API adapters and no duplicate standard algorithm.
- [ ] Use enough signed range and product bits even when a standard two-product quire cannot represent the exact triple product.
- [ ] Apply NaR precedence, zero handling, permutation symmetry, and final section 4.1 saturation.
- [ ] Exhaust posit8 triples or use a documented symmetry reduction retaining complete coverage; test wide-format cancellation-free hard boundaries and seeded triples with an independent exact oracle.
- [ ] Provide bounded hardware kernels, synthesis evidence, and independent hardware result checks.
- [ ] Update generation, API documentation, fusion mapping, and coverage matrix.
