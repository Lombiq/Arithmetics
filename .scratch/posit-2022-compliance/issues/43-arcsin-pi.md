# 43: Implement arcSinPi

**What to build:** Correctly rounded arcsin(x)/pi under section 5.5.

**Blocked by:** 42: Implement arcSin.

**Status:** ready-for-agent

- [ ] Cover E2 8/16/32 and Posit32 with the specified principal branch, domain/NaR rules, exact zero, and exact endpoint values plus/minus one-half.
- [ ] Reuse certified internal inverse-sine evaluation, not division of rounded arcSin by rounded pi.
- [ ] Document combined error bounds, finite hardware termination, and complete hard-case resolution for the scaled result.
- [ ] Independently exhaust posit8/16; test posit32 endpoint neighbors, tiny inputs, and scaled-result rounding boundaries.
- [ ] Supply synthesized hardware entry points and independent raw-result checks for every precision.
- [ ] Update generation, pi-scaled API documentation, provenance, and coverage matrix.
