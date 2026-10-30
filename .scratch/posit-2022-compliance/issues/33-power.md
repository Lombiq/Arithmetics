# 33: Implement pow

**What to build:** Correctly rounded two-posit power under sections 5.1 and 5.6.

**Blocked by:** 18: Deliver certified bounded exp2 for posit16 and posit32.

**Status:** ready-for-agent

- [ ] Cover E2 8/16/32 and Posit32 with a complete domain/branch table, including negative bases with exact integral exponents, zero bases, NaR, and pow(0,0)=NaR.
- [ ] Evaluate the complete expression with section 4.1 rounding, avoiding rounded public log/exp compositions and premature underflow/overflow.
- [ ] Certify exponent classification, internal reduction, error bounds, finite hardware termination, and hard-case resolution across the full pair domain.
- [ ] Exhaust all posit8 pairs independently; test wide integer/noninteger exponent boundaries, near-one bases, extreme exponents, and seeded hard cases.
- [ ] Supply synthesized entry points and independent hardware numerical evidence for every precision.
- [ ] Update generation, domain/migration documentation, correctness justification, and coverage matrix.
