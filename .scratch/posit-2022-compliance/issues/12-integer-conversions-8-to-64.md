# 12: Complete 8-64-bit integer conversions

**What to build:** Bidirectional conversions between standard posits and signed/unsigned 8-, 16-, 32-, and 64-bit integers under section 6.4.

**Blocked by:** 03: Correct rounding and standard precision conversions.

**Status:** ready-for-agent

- [ ] Cover E2 8/16/32 and Posit32 without narrowing wide inputs through smaller intermediates.
- [ ] Record and justify the integer rounding interpretation against the standard before implementation; flag unresolved ambiguity instead of guessing.
- [ ] Every integer MSB-only input maps to NaR, including unsigned input; NaR or out-of-range posit output maps to the target MSB-only sentinel after rounding/range checking.
- [ ] Check exact independent expectations for all 8-bit integer inputs, posit8/16 conversion inputs, signed/unsigned extrema, ties, and range transitions.
- [ ] Supply bounded raw-integer hardware entry points, synthesis evidence, and independent numerical hardware checks for every width/direction.
- [ ] Document intentional differences from historical truncation/saturation and update generation, compatibility tests, and coverage mapping.
