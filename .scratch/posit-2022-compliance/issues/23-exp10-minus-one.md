# 23: Implement exp10Minus1

**What to build:** Correctly rounded 10^x - 1 under section 5.5.

**Blocked by:** 22: Implement exp10.

**Status:** ready-for-agent

- [ ] Cover E2 8/16/32 and Posit32 with NaR propagation, exact zero, and section 4.1 range handling.
- [ ] Preserve near-zero cancellation and negative-input limits; reuse internal certified machinery without subtracting from rounded exp10.
- [ ] Document full-domain certified errors, finite resource/iteration bounds, and resolution for every hard rounding case.
- [ ] Exhaust posit8/16 against independent expectations; include posit32 tiny values, range transitions, and seeded/hard-case vectors.
- [ ] Synthesize and independently check raw hardware outputs for all standard precisions.
- [ ] Update authoritative generation, public contracts, provenance, and coverage matrix; preserve extension behavior outside documented fixes.
