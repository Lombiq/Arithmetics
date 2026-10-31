# 14: Complete IEEE binary conversions

**What to build:** Bidirectional IEEE binary16/32/64 conversions for Half, float, and double under section 6.5.

**Blocked by:** 03: Correct rounding and standard precision conversions.

**Status:** ready-for-agent

- [ ] Cover E2 8/16/32 and Posit32 with raw-bit hardware adapters for every source/target format.
- [ ] Implement corrected 32-bit IEEE conversion algorithms once for Posit32 and Posit32E2; retain format-specific public constructors/operators only as adapters to shared logic.
- [ ] Map every IEEE NaN/infinity to NaR, both IEEE zeros to posit zero, posit zero to positive IEEE zero, and NaR to a quiet NaN.
- [ ] Correctly round finite values including subnormals, representable ties, overflow, and underflow; document nearest-even as the normal environment mode and match any other exposed modes.
- [ ] Exhaust binary16 input encodings and small posit outputs; independently check binary32/64 boundaries and seeded raw encodings without production float-conversion helpers.
- [ ] Synthesize all conversion kernels and compare actual simulated/device raw encodings against independent expectations.
- [ ] Update generation, public conversion documentation, extension regressions, and coverage matrix.

## Comments

- Ticket 02's independent oracle found that raw `0xFFFFFFFF` (-minPos) converts to twice the expected magnitude
  (`-2^-120`) in both `Posit32` and `Posit32E2`; correct this IEEE conversion boundary here, not in ticket 02.
