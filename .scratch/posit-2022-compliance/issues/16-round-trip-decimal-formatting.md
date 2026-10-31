# 16: Implement round-trip decimal formatting

**What to build:** Canonical posit-to-decimal output and .NET wrappers with hardware numerical formatting under section 6.3.

**Blocked by:** 15: Implement exact decimal parsing.

**Status:** ready-for-agent

- [ ] Cover E2 8/16/32 and Posit32, including zero, signs, extremes, and the agreed NaR token.
- [ ] Share the 32-bit numerical decimal formatter between Posit32 and Posit32E2; retain distinct ToString overloads/providers only as explicit wrappers around the common representation conversion.
- [ ] Produce canonical text that parses to the same raw value; justify digit budgets against Table 2. Shortest possible strings are not required.
- [ ] Format into bounded hardware buffers with explicit output length and explicit capacity failures; host allocation/culture adaptation cannot precompute the digits.
- [ ] Exhaust posit8/16 canonical round trips and independently verify emitted decimal values; test posit32 boundaries and seeded raw values.
- [ ] Synthesize formatting kernels and check hardware-generated characters and statuses, not merely host formatting of hardware numbers.
- [ ] Update formatting contracts, buffer documentation, generation, compatibility tests, and coverage matrix.
