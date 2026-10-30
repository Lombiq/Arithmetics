# 55: Implement atanh

**What to build:** Correctly rounded inverse hyperbolic tangent (standard arcTanH) under sections 5.1 and 5.5.

**Blocked by:** 26: Implement log.

**Status:** ready-for-agent

- [ ] Cover E2 8/16/32 and Posit32; document NaR, real-domain, pole/endpoint behavior, exact zero, and odd symmetry.
- [ ] Retain tiny inputs and information near plus/minus one; evaluate the complete expression without rounded logarithm compositions.
- [ ] Document certified errors, bounded hardware termination, and exact/hard-rounding resolution over the entire valid domain.
- [ ] Independently exhaust posit8/16; test posit32 endpoint neighbors, tiny values, and hard-rounding/seeded cases.
- [ ] Synthesize every precision and compare hardware raw outputs with independent expected results.
- [ ] Update generation, public/domain documentation, provenance, and coverage matrix.
