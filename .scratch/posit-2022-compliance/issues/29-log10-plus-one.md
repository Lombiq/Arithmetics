# 29: Implement log10Plus1

**What to build:** Correctly rounded log10(1 + x) under section 5.5.

**Blocked by:** 28: Implement log10.

**Status:** ready-for-agent

- [ ] Cover E2 8/16/32 and Posit32 with documented domain/NaR semantics and exact zero.
- [ ] Preserve the exact input increment and small cancellation-sensitive outputs; do not call public log10 on rounded 1+x.
- [ ] Reuse certified internal evaluation with documented full-domain bounds, finite hardware termination, and complete hard-case resolution.
- [ ] Independently exhaust posit8/16; cover posit32 near-zero, near-minus-one, extreme, seeded, and difficult-rounding inputs.
- [ ] Synthesize every precision and compare hardware outputs to independent expected raw bits.
- [ ] Update authoritative generation, consumer documentation, correctness evidence, and coverage matrix.
