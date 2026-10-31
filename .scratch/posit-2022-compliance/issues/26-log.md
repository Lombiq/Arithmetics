# 26: Implement log

**What to build:** Correctly rounded natural logarithm under sections 5.1 and 5.5.

**Blocked by:** 18: Deliver certified bounded exp2 for posit16 and posit32.

**Status:** ready-for-agent

- [ ] Cover E2 8/16/32 and Posit32; document invalid-domain/NaR behavior, exact log(1), and section 4.1 output rounding.
- [ ] Certify range reduction and constants across minPos to maxPos, including cancellation near one.
- [ ] Supply finite hardware precision/termination bounds and exact or certified resolution of all rounding ambiguities.
- [ ] Independently verify all posit8/16 inputs and posit32 endpoint, near-one, hard-rounding, and seeded cases.
- [ ] Provide hardware entry points, actual synthesis evidence, and independent simulated/device result comparisons.
- [ ] Update authoritative generation, correctness/domain documentation, extension regressions, and coverage matrix.
