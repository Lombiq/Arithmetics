# 10: Complete quire products and existing fused conveniences

**What to build:** Exact qMulAdd/qMulSub and compatible once-rounded sums, dot products, multiply-add, add-multiply, and four-argument product-difference conveniences.

**Blocked by:** 09: Complete quire unary and additive arithmetic.

**Status:** ready-for-agent

- [ ] Cover E2 8/16/32 and Posit32 under sections 4.2 and 5.11; multiply inputs exactly inside the accumulator, rounding only at requested posit output.
- [ ] Make each corrected 32-bit product/fused kernel authoritative for Posit32 and Posit32E2; public helper names and overloads may adapt to the existing surfaces but must not retain duplicate numerical algorithms.
- [ ] Preserve NaR, overflow-to-NaR, precision checking, empty sums, exact cancellation, and explicit input-shape error contracts.
- [ ] Prove the standard dot-product and sum capacity guarantees analytically; exercise constructed range boundaries without billions of iterations.
- [ ] Compare independent exact results for product extrema, minPos-squared, chunked/streamed accumulation, ordering, and examples distinguishing fused from ordinary arithmetic.
- [ ] Provide hardware entry points and static storage configuration, actual synthesis evidence, and independent simulated/device checks for every helper.
- [ ] Update consumer fusion contracts, generation, coverage matrix, and existing Hastlayer sample regressions.
