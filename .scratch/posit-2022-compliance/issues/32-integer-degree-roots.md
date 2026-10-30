# 32: Implement rootN

**What to build:** Correctly rounded integer-degree roots under sections 5.1 and 5.8.

**Blocked by:** 18: Deliver certified bounded exp2 for posit16 and posit32.

**Status:** ready-for-agent

- [ ] Cover E2 8/16/32 and Posit32; define a signed integer-degree API and resolve the standard's zero-degree, negative-degree, parity, negative-base, and even-root branch semantics before implementation.
- [ ] Evaluate the complete mathematical expression with one final rounding, including reciprocal roots and exact perfect powers, without rounded public compositions.
- [ ] Justify finite hardware bounds for the entire supported degree range, including integer minima; no overflow when taking the degree's magnitude.
- [ ] Check every posit8 input against representative signed degrees/parities/extrema; add wide hard cases and independent root expectations with justified coverage of the remaining domain.
- [ ] Synthesize all precision kernels and compare independent domain, exceptional, and hard-rounding vectors in hardware.
- [ ] Document adopted interpretations, algorithmic certification, generation, and coverage mapping; unresolved semantics/feasibility block completion.
