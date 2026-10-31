# 31: Implement rSqrt

**What to build:** Correctly rounded reciprocal square root under section 5.5.

**Blocked by:** 30: Certify sqrt.

**Status:** ready-for-agent

- [ ] Cover E2 8/16/32 and Posit32; record domain/NaR behavior, including zero and negative inputs.
- [ ] Reuse the authoritative 32-bit Posit32/Posit32E2 kernel/generation source established by the root tickets for the corrected reciprocal-root implementation; keep only required API adapters.
- [ ] Evaluate 1/sqrt(x) with one final section 4.1 rounding; reciprocating a rounded public sqrt result is insufficient.
- [ ] Use independent exact boundary reasoning or certified enclosures and document finite hardware widths/termination and hard-case resolution.
- [ ] Exhaust posit8/16; test posit32 reciprocal-square identities, extremes, root-boundary neighbors, and seeded hard cases.
- [ ] Synthesize each precision and compare simulated/device outputs against independent raw expectations.
- [ ] Update generation, public/domain documentation, correctness argument, and coverage matrix.
