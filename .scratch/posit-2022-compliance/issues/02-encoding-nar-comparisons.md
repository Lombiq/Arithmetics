# 02: Certify encoding, NaR, and comparisons

**What to build:** Standard raw-value interpretation and consistent public comparisons under Posit Standard (2022) sections 3, 5.1, and 5.3, with an independent exact reference foundation.

**Blocked by:** 01: Unify standard implementation generation without changing behavior.

**Status:** ready-for-agent

- [ ] Independently decode E2 posit8/16/32 into exact signed dyadic values; validate against hand-derived positive, negative, truncated-field, zero, NaR, minPos, and maxPos encodings.
- [ ] Cover Posit8E2, Posit16E2, Posit32E2, and Posit32. All six comparisons, typed comparison, and equality agree with signed two's-complement representation ordering; NaR equals itself and precedes real values.
- [ ] Implement the corrected 32-bit standard comparison behavior once for Posit32 and Posit32E2, using shared Hastlayer-compatible logic/generation with explicit public adapters; do not treat legacy ordering as the compliance oracle.
- [ ] Expose NaR terminology without removing compatibility aliases; sign classification must not imply NaR has a mathematical sign.
- [ ] Exhaust all posit8 pairs and posit16 unary decoding inputs; add posit32 boundaries and seeded random vectors. Expected values do not call production decoding.
- [ ] Provide hardware comparison entry points, actual synthesis evidence, and simulated/device raw-result checks against independent vectors.
- [ ] Update generation, consumer behavior-change notes, and the requirement/API/software/hardware matrix; preserve non-E2 regression coverage.
