# 40: Implement tanPi

**What to build:** Correctly rounded tan(pi*x) under sections 5.1 and 5.5.

**Blocked by:** 18: Deliver certified bounded exp2 for posit16 and posit32.

**Status:** ready-for-agent

- [ ] Cover E2 8/16/32 and Posit32; record pole/domain behavior, NaR propagation, exact integer zeros, and sign/periodicity.
- [ ] Reduce exact inputs and evaluate mathematical pi scaling without rounded constants; division of rounded sinPi/cosPi does not satisfy one-round semantics.
- [ ] Certify full-domain error bounds, finite hardware termination, and exact/hard cases near half-integer poles and output saturation.
- [ ] Independently exhaust posit8/16; test posit32 pole neighbors, huge integers, tiny values, and hard rounding.
- [ ] Synthesize every precision and compare simulated/device result bits against independent expectations.
- [ ] Update generation, domain documentation, correctness provenance, and coverage matrix.
