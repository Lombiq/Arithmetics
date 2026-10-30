# 09: Complete quire unary and additive arithmetic

**What to build:** qNegate, qAbs, qAddP, qSubP, qAddQ, and qSubQ with exact standard quire semantics under section 5.11.

**Blocked by:** 08: Certify matching posit/quire conversion.

**Status:** ready-for-agent

- [ ] Cover matching E2 8/16/32 quires and Posit32 adapters; preserve width/scale and exact finite results.
- [ ] Detect signed carry-range overflow and reserved NaR results; return sticky NaR instead of wrapping. Validate matching widths explicitly.
- [ ] Define intentional accumulator mutation clearly; ordinary unary/additive operations must not unexpectedly modify operands.
- [ ] Check independent scaled-integer results at limb carry/borrow boundaries, cancellation, extrema, NaR interactions, and capacity limits.
- [ ] Synthesize all six operation entry points and compare generated hardware results with independent expectations.
- [ ] Update quire contracts, generation where applicable, behavior notes, and software/hardware coverage mapping.
