# 42: Implement arcSin

**What to build:** Correctly rounded principal inverse sine under sections 5.1 and 5.5.

**Blocked by:** 18: Deliver certified bounded exp2 for posit16 and posit32.

**Status:** ready-for-agent

- [ ] Cover E2 8/16/32 and Posit32; document real-domain endpoints, NaR for invalid inputs, odd symmetry, and the specified principal branch/range.
- [ ] Preserve exact zero and correctly rounded endpoint values using mathematical constants, not rounded public compositions.
- [ ] Certify endpoint-sensitive evaluation, finite hardware bounds, and resolution of every exact or hard-rounding case.
- [ ] Independently exhaust posit8/16; test posit32 inputs adjacent to plus/minus one, tiny inputs, and difficult rounding.
- [ ] Synthesize each precision kernel and compare hardware outputs against independent result bits.
- [ ] Update generation, API/domain contracts, error-bound provenance, and coverage matrix.
