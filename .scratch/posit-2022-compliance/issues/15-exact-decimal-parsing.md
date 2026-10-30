# 15: Implement exact decimal parsing

**What to build:** Decimal-to-posit interchange under section 6.3, including numerical parsing in generated hardware rather than host-side precomputation.

**Blocked by:** 03: Correct rounding and standard precision conversions.

**Status:** ready-for-agent

- [ ] Cover E2 8/16/32 and Posit32; define canonical invariant grammar, NaR token, exponent/sign handling, input bounds, and malformed/capacity error contracts.
- [ ] Round the mathematical decimal value directly using section 4.1, avoiding intermediate double conversion and double rounding.
- [ ] Provide bounded digit/character buffers and explicit lengths for hardware; string/culture wrappers may adapt transport but not replace numerical evaluation.
- [ ] Independently check ties and near-ties, huge/tiny exponents, signed zeros, invalid strings, buffer limits, and equivalent decimal spellings.
- [ ] Synthesize the complete numeric parsing kernel and compare hardware outputs/error indicators with independent decimal-rational expectations.
- [ ] Document supported textual forms, buffer capacities, and wrappers; update generation and coverage matrix.
