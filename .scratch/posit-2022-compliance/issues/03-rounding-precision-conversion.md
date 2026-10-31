# 03: Correct rounding and standard precision conversions

**What to build:** Public exact widening and correctly rounded narrowing between standard precisions, exercising a reliable section 4.1 rounding foundation through section 6.1 conversions.

**Blocked by:** 02: Certify encoding, NaR, and comparisons.

**Status:** done

- [x] Implement all six directed E2 8/16/32 precision conversions, including Posit32 compatibility. Widen by appending zero bits; preserve zero and NaR.
- [x] Implement the literal extended-representation boundary and even-LSB tie rule, retaining sticky/discarded information. Do not assume adjacent arithmetic midpoints are equivalent.
- [x] Make each corrected 32-bit rounding/conversion algorithm authoritative for both Posit32 and Posit32E2; any signature-specific constructor or conversion adapter must delegate to the same numerical implementation.
- [x] Preserve exact zero; clamp finite overflow to signed maxPos and nonzero underflow to signed minPos.
- [x] Extend the independent oracle with exact rational rounding; hand-check regime-boundary ties and values on either side. Exhaust small-source inputs and test wide-source hard boundaries.
- [ ] Supply hardware conversion entry points, bounded storage, synthesis evidence, and independent simulated/device output comparisons.
- [x] Update parameterized generation, conversion documentation, and coverage matrix; verify Posit32/E2 agreement and extension regressions.

## Comments

- **Pre-implementation contract (Posit Standard (2022), original PDF, sections 3.1-3.3, pp. 6-7; 4.1, p. 8; 5.1, p. 9; and 6.1, p. 12):** For E2 posit8/16/32 and the Posit32 compatibility surface, NaR and zero are preserved; widening is exact by appending zero representation bits. Narrowing follows section 4.1: return exact values unchanged; clamp finite magnitudes above maxPos to signed maxPos and nonzero magnitudes below minPos to signed minPos; otherwise locate the adjacent target posits `u < x < w`, append a `1` to `u`'s target-width encoding to form the next-width posit `v`, and return `u` when `x < v` or when `x == v` and the LSB of `u`'s encoding is zero, else return `w`. This literal representational boundary and tie rule—not an assumed arithmetic midpoint—govern all six directed conversions. Section 5.1 requires exceptional NaR propagation and permits mixed precision through section 6.1 conversions; the Posit32 and Posit32E2 narrowing paths must share one authoritative algorithm while preserving their public adapters.
- Independent expected values will use the section 3.3 signed-field decoder as exact integer/binary-exponent rationals, plus a separate rounding oracle implementing the literal `U1` threshold and LSB rule without production conversion/arithmetic or IEEE casts. Evidence will include hand-derived regime-boundary values and exact ties with values on either side, exhaustive source posit8 encodings in both narrowing directions, and exact rational boundary/seeded cases for the wider-source conversions, including zero, NaR, signed saturation, and nonzero underflow.
- **Completed software evidence:** The independent `BigInteger` dyadic oracle uses the ticket 02 decoder/comparison and a separately decoded target-precision `U1` boundary. The 8- and 16-bit source encodings are exhaustive where they narrow; 32-bit narrowing covers every target `U1` boundary and both neighbors in both signs, plus seeded inputs. Hand-checked ties straddle regime boundaries. All six E2 directions and Posit32 adapters agree with the oracle. The Arithmetics suite passes 312/312; the full VHDL suite passes 47/47, including six bounded raw-memory hardware entry points, CPU output checks, and the Catapult approval. `dotnet build Hastlayer.SDK.sln` and `dotnet build Hastlayer.SDK.sln -t:Rebuild` both succeed with zero warnings and errors. Two forced implementation-and-test generation passes produce identical SHA-256 hashes for all 34 owned generated outputs.
- **Deferred hardware evidence for ticket 56:** Vendor synthesis, vendor simulation, physical-device checks, and hardware-dependent `Hast.DynamicTests` were not run. The hardware entry points, bounded memory layout, CPU outputs, and Hastlayer-to-VHDL transformation are verified; the unchecked hardware acceptance item remains open until the vendor/device evidence is supplied.
