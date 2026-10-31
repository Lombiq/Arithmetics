# 13: Complete 128-bit and native integer conversions

**What to build:** Section 6.4 conversions for Int128/UInt128 and native-sized signed/unsigned integers.

**Blocked by:** 12: Complete 8-64-bit integer conversions.

**Status:** ready-for-agent

- [ ] Cover E2 8/16/32 and Posit32 in both directions, reusing the established integer rounding/sentinel contract.
- [ ] Use the same authoritative 32-bit Posit32/Posit32E2 conversion kernels as ticket 12, with explicit adapters where the existing public signatures differ.
- [ ] Preserve all 128-bit source information; implement hardware transport with explicit words when host wide types are not directly transformable.
- [ ] Define native-sized conversion width explicitly for each supported host/hardware profile, without assuming all native integers are 64 bits.
- [ ] Independently check MSB-only sentinels, signed/unsigned boundaries, wide sticky-bit rounding, and range checking after rounding.
- [ ] Synthesize all wide/native conversion realizations and compare hardware outputs with independent raw-word expectations.
- [ ] Update public adapters, generation, platform documentation, and coverage matrix; retain smaller-width regression coverage.
