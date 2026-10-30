# 30: Certify sqrt

**What to build:** Independently certified square root under sections 4 and 5.5, correcting the existing implementation as required.

**Blocked by:** 03: Correct rounding and standard precision conversions.

**Status:** ready-for-agent

- [ ] Cover E2 8/16/32 and Posit32, with sqrt(0)=0, NaR propagation, and invalid negative inputs returning NaR.
- [ ] Round mathematical sqrt directly using sufficient root/remainder information; preserve exact squares and section 4.1 boundaries.
- [ ] Use independent integer/rational boundary comparisons or certified enclosures, not the production root algorithm, to establish expectations.
- [ ] Exhaust posit8/16 and test posit32 square neighbors, minPos/maxPos, difficult rounding, and seeded inputs.
- [ ] Document finite hardware resources/termination; supply synthesized entry points and independent numerical hardware checks.
- [ ] Update generation, compatibility notes, root contracts, and coverage matrix.
