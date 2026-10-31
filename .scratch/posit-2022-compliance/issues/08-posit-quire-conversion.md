# 08: Certify matching posit/quire conversion

**What to build:** Exact pToQ and correctly rounded qToP under sections 3.4, 5.11, and 6.2.

**Blocked by:** 03: Correct rounding and standard precision conversions.

**Status:** ready-for-agent

- [ ] Pair E2 8/16/32 with standard 128/256/512-bit quires, including Posit32. Preserve width, minPos-squared scale, zero, and the quire NaR pattern.
- [ ] Make corrected 32-bit posit/quire conversion algorithms authoritative for both Posit32 and Posit32E2; keep any existing constructor/operator differences as explicit delegating adapters.
- [ ] Convert all finite posit inputs exactly and all valid quire values using section 4.1; distinguish quire zero from its least nonzero bit.
- [ ] Reject mismatched precision/width explicitly. Conversions must not mutate caller-owned quire storage.
- [ ] Use independent integer-scaled quire expectations; exhaust posit8/16 round trips and test wide cancellation residuals, signs, ties, NaR, and extremes.
- [ ] Provide bounded hardware conversion entry points, synthesis evidence, and independent hardware numerical checks.
- [ ] Update public contracts, parameterized generation, storage documentation, coverage matrix, and existing quire regressions.
