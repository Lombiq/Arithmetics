# 25: Implement log2Plus1

**What to build:** Correctly rounded log2(1 + x) under section 5.5 without rounding the increment first.

**Blocked by:** 24: Implement log2.

**Status:** ready-for-agent

- [ ] Cover E2 8/16/32 and Posit32; apply the documented domain table around minus one, NaR, and exact zero.
- [ ] Retain tiny inputs that would disappear in rounded 1+x and preserve exact special cases.
- [ ] Reuse certified internal log machinery while maintaining one final rounding; document finite hardware/error bounds and hard-case resolution.
- [ ] Independently exhaust posit8/16 inputs; test posit32 near-minus-one, near-zero, extreme-positive, and hard-rounding vectors.
- [ ] Synthesize kernels and compare hardware result bits to independent expectations at every precision.
- [ ] Update generation, API/domain documentation, coverage matrix, and compatibility checks.
