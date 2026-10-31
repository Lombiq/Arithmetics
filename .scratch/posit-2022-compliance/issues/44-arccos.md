# 44: Implement arcCos

**What to build:** Correctly rounded principal inverse cosine under sections 5.1 and 5.5.

**Blocked by:** 18: Deliver certified bounded exp2 for posit16 and posit32.

**Status:** ready-for-agent

- [ ] Cover E2 8/16/32 and Posit32 with the standard domain/NaR rules, zero at one, and principal range from zero to rounded mathematical pi.
- [ ] Avoid cancellation and lost endpoint information near plus/minus one; rounded public arcSin subtraction is insufficient.
- [ ] Certify constants, full-domain evaluation errors, finite hardware bounds, and exact/hard-rounding resolution.
- [ ] Independently exhaust posit8/16 and test posit32 endpoint neighbors, central values, and difficult rounding.
- [ ] Synthesize each precision and compare simulated/device results with independent raw vectors.
- [ ] Update authoritative generation, domain/range documentation, correctness provenance, and coverage matrix.
