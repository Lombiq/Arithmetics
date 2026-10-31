# Posit Standard (2022) coverage matrix

The matrix inventories each required operation once; later tickets update only the rows they own. The API column names all required precision surfaces, with `Posit32` tracked as a compatibility adapter alongside `Posit32E2`. Evidence is deliberately split into compatibility preservation and normative conformance: a legacy result or a shared-kernel comparison is not a standard-correctness result.

Evidence values: `pending` means no ticket evidence recorded; `preserved` means a compatibility check passed; `verified` requires independent expected results against the PDF contract. Hardware evidence must distinguish transform/VHDL generation, vendor synthesis, and numerical simulation/device execution.

| PDF section | Operation | Public API inventory (`Posit8E2`, `Posit16E2`, `Posit32E2`, `Posit32`) | Preserved behavior | Independent standard software evidence | Hardware evidence |
|---|---|---|---|---|---|
| 3.2-3.3, 5.1 | raw encoding / NaR interpretation | Raw constructors, `NaRBitMask`, `IsNaR`, `IsNaN`, `IsNegative` on all four surfaces | Raw storage and NaN aliases preserved; IsPositive remains a raw polarity helper | Verified: independent signed-field dyadic decoder, hand-derived anchors, exhaustive posit8/16 raw inputs and posit32 boundary/seeded inputs (ticket 02) | Comparison entry-point CPU and VHDL evidence below; vendor numerical evidence deferred |
| 5.2 | negate | `Negate`, unary `-` on all four surfaces | Unary operator and shared two's-complement primitive unchanged | Verified: ticket 04 exact signed-dyadic expectations, exhaustive 8/16-bit and 32-bit boundaries/seeded inputs, NaR | Four bounded CPU entry points independently checked; Catapult VHDL approved with unary operations inlined; vendor synthesis/simulation/device deferred to 56 |
| 5.2 | abs | `Abs` on all four surfaces | Existing shared implementation unchanged | Verified: ticket 04 exact absolute values over the same exhaustive/boundary inputs, NaR | Same ticket 04 CPU/VHDL evidence; all previous approvals unchanged; vendor evidence deferred to 56 |
| 5.2 | sign | `Sign` on all four surfaces | Added posit-valued API; raw IsPositive unchanged | Verified: exact posit -1/0/1, NaR; exhaustive 8/16-bit and 32-bit boundaries/seeded inputs | Same ticket 04 CPU/VHDL evidence; vendor evidence deferred to 56 |
| 5.2 | nearestInt | `NearestInt` on all four surfaces | Added shared generated algorithm; E0-E4 regressions retained | Verified: independent BigInteger quotient/remainder and exact re-encoding, even-integer ties, exhaustive 8/16-bit and 32-bit boundaries/seeded inputs | Same ticket 04 CPU/VHDL evidence; no integer/IEEE conversion; vendor evidence deferred to 56 |
| 5.2 | ceil | `Ceiling` on all four surfaces | Added shared generated algorithm; E0-E4 regressions retained | Verified: independent exact ceiling, signed inputs/NaR and integral values beyond machine-integer range; same exhaustive/boundary inputs | Same ticket 04 CPU/VHDL evidence; vendor evidence deferred to 56 |
| 5.2 | floor | `Floor` on all four surfaces | Added shared generated algorithm; E0-E4 regressions retained | Verified: independent exact floor, signed inputs/NaR and integral values beyond machine-integer range; same exhaustive/boundary inputs | Same ticket 04 CPU/VHDL evidence; vendor evidence deferred to 56 |
| 5.2 | next | `Next` on all four surfaces | Added representation traversal API | Verified: modulo-width successor, wraparound through zero/NaR/extremes; same exhaustive/boundary inputs | Same ticket 04 CPU/VHDL evidence; vendor evidence deferred to 56 |
| 5.2 | prior | `Prior` on all four surfaces | Added representation traversal API | Verified: modulo-width predecessor, wraparound through zero/NaR/extremes; same exhaustive/boundary inputs | Same ticket 04 CPU/VHDL evidence; vendor evidence deferred to 56 |
| 5.3 | compareEqual | `==`, typed/object `Equals`, typed/object `CompareTo` on all four surfaces | Raw equality preserved; ordering corrected | Verified: ticket 02 exact-reference pair checks, including NaR self-equality | Four CPU entry points verified; Catapult VHDL approved; vendor synthesis/simulation/device deferred |
| 5.3 | compareNotEqual | `!=` on all four surfaces | Preserved raw inequality | Verified: ticket 02 exact-reference pair checks | Four CPU entry points verified; Catapult VHDL approved; vendor synthesis/simulation/device deferred |
| 5.3 | compareGreater | `>` on all four surfaces | Intentional equal/negative/NaR ordering corrections | Verified: ticket 02 exact-reference pair checks | Four CPU entry points verified; Catapult VHDL approved; six existing comparison-dependent snapshots reviewed; vendor evidence deferred |
| 5.3 | compareGreaterEqual | `>=` on all four surfaces | Added generated surface; Posit32 ordering corrected | Verified: ticket 02 exact-reference pair checks | Four CPU entry points verified; Catapult VHDL approved; vendor synthesis/simulation/device deferred |
| 5.3 | compareLess | `<` on all four surfaces | Intentional equal/cross-sign/NaR ordering corrections | Verified: ticket 02 exact-reference pair checks | Four CPU entry points verified; Catapult VHDL approved; vendor synthesis/simulation/device deferred |
| 5.3 | compareLessEqual | `<=` on all four surfaces | Added generated surface; Posit32 ordering corrected | Verified: ticket 02 exact-reference pair checks | Four CPU entry points verified; Catapult VHDL approved; vendor synthesis/simulation/device deferred |
| 5.4 | addition | `+` on all four surfaces | E2 8/16-bit results unchanged; legacy 32-bit results corrected; `Posit32`/`Posit32E2` share one generated kernel; E0/E1/E3/E4 legacy addition and regressions retained | Verified: ticket 05 exact BigInteger sums with the shared section 4.1 `PositRoundingReference.Round` oracle; all 65,536 posit8 pairs, 32,224 posit16 and 43,616 posit32 hard/seeded pairs (NaR/zero, cancellation, every scale gap, ties, clamping); `Posit32` bit-identical to `Posit32E2`; source-order expressions vs explicit fusion | Four bounded CPU entry points independently checked, including rounding-sensitive expressions; Catapult VHDL approved; six existing Posit32 sample snapshots audited; vendor synthesis/simulation/device deferred to 56 |
| 5.4 | subtraction | `-` on all four surfaces | Remains `a + (-b)` with exact negation; same compatibility as addition | Verified: ticket 05 exact differences over the same vectors; `a - a` returns zero and NaR precedes zero shortcuts | Same ticket 05 CPU/VHDL evidence; vendor synthesis/simulation/device deferred to 56 |
| 5.4 | multiplication | `*` on all four surfaces | Pending | Pending | Pending |
| 5.4 | division | `/` on all four surfaces | Pending | Pending | Pending |
| 5.5 | sqrt | `Sqrt` on all four surfaces | Pending | Pending | Pending |
| 5.5 | rSqrt | `RSqrt` on all four surfaces | Pending | Pending | Pending |
| 5.5 | exp | `Exp` on all four surfaces | Pending | Pending | Pending |
| 5.5 | expMinus1 | `ExpMinus1` on all four surfaces | Pending | Pending | Pending |
| 5.5 | exp2 | `Exp2` on all four surfaces | Pending | Pending | Pending |
| 5.5 | exp2Minus1 | `Exp2Minus1` on all four surfaces | Pending | Pending | Pending |
| 5.5 | exp10 | `Exp10` on all four surfaces | Pending | Pending | Pending |
| 5.5 | exp10Minus1 | `Exp10Minus1` on all four surfaces | Pending | Pending | Pending |
| 5.5 | log | `Log` on all four surfaces | Pending | Pending | Pending |
| 5.5 | logPlus1 | `LogPlus1` on all four surfaces | Pending | Pending | Pending |
| 5.5 | log2 | `Log2` on all four surfaces | Pending | Pending | Pending |
| 5.5 | log2Plus1 | `Log2Plus1` on all four surfaces | Pending | Pending | Pending |
| 5.5 | log10 | `Log10` on all four surfaces | Pending | Pending | Pending |
| 5.5 | log10Plus1 | `Log10Plus1` on all four surfaces | Pending | Pending | Pending |
| 5.5 | sin | `Sin` on all four surfaces | Pending | Pending | Pending |
| 5.5 | sinPi | `SinPi` on all four surfaces | Pending | Pending | Pending |
| 5.5 | cos | `Cos` on all four surfaces | Pending | Pending | Pending |
| 5.5 | cosPi | `CosPi` on all four surfaces | Pending | Pending | Pending |
| 5.5 | tan | `Tan` on all four surfaces | Pending | Pending | Pending |
| 5.5 | tanPi | `TanPi` on all four surfaces | Pending | Pending | Pending |
| 5.5 | arcSin | `ArcSin` on all four surfaces | Pending | Pending | Pending |
| 5.5 | arcSinPi | `ArcSinPi` on all four surfaces | Pending | Pending | Pending |
| 5.5 | arcCos | `ArcCos` on all four surfaces | Pending | Pending | Pending |
| 5.5 | arcCosPi | `ArcCosPi` on all four surfaces | Pending | Pending | Pending |
| 5.5 | arcTan | `ArcTan` on all four surfaces | Pending | Pending | Pending |
| 5.5 | arcTanPi | `ArcTanPi` on all four surfaces | Pending | Pending | Pending |
| 5.5 | sinH | `SinH` on all four surfaces | Pending | Pending | Pending |
| 5.5 | cosH | `CosH` on all four surfaces | Pending | Pending | Pending |
| 5.5 | tanH | `TanH` on all four surfaces | Pending | Pending | Pending |
| 5.5 | arcSinH | `ArcSinH` on all four surfaces | Pending | Pending | Pending |
| 5.5 | arcCosH | `ArcCosH` on all four surfaces | Pending | Pending | Pending |
| 5.5 | arcTanH | `ArcTanH` on all four surfaces | Pending | Pending | Pending |
| 5.6 | hypot | `Hypot` on all four surfaces | Pending | Pending | Pending |
| 5.6 | pow | `Pow` on all four surfaces | Pending | Pending | Pending |
| 5.6 | arcTan2 | `ArcTan2` on all four surfaces | Pending | Pending | Pending |
| 5.6 | arcTan2Pi | `ArcTan2Pi` on all four surfaces | Pending | Pending | Pending |
| 5.7 | fMM | `FMM` on all four surfaces | Pending | Pending | Pending |
| 5.8 | compound | `Compound` on all four surfaces | Pending | Pending | Pending |
| 5.8 | rootN | `RootN` on all four surfaces | Pending | Pending | Pending |
| 5.11 | pToQ | Explicit posit-to-quire conversion on all four surfaces | Pending | Pending | Pending |
| 5.11 | qNegate | `Quire` negation for each matching precision | Pending | Pending | Pending |
| 5.11 | qAbs | `Quire` absolute value for each matching precision | Pending | Pending | Pending |
| 5.11 | qAddP | Matching-precision quire-plus-posit on all four surfaces | Pending | Pending | Pending |
| 5.11 | qSubP | Matching-precision quire-minus-posit on all four surfaces | Pending | Pending | Pending |
| 5.11 | qAddQ | Matching-precision quire addition | Pending | Pending | Pending |
| 5.11 | qSubQ | Matching-precision quire subtraction | Pending | Pending | Pending |
| 5.11 | qMulAdd | Matching-precision quire multiply-add | Pending | Pending | Pending |
| 5.11 | qMulSub | Matching-precision quire multiply-subtract | Pending | Pending | Pending |
| 5.11 | qToP | Matching-precision quire-to-posit conversion on all four surfaces | Pending | Pending | Pending |
| 6.1 | precision conversion | All six directed E2 8/16/32 conversions; `Posit32` compatibility conversions | Widening remains exact; narrowing now follows standard rounding and saturation | Verified: independent exact-rational oracle; exhaustive 8/16-bit sources and all 32-bit target-boundary ties/neighbors; all six directions and `Posit32` adapters | Six bounded raw-memory entry points; CPU outputs and Catapult VHDL approved; vendor synthesis/simulation/device deferred to ticket 56 |
| 6.3 | decimal parse | `Parse`/`TryParse` adapters on all four surfaces | Pending | Pending | Pending |
| 6.3 | decimal format | `ToString` adapters on all four surfaces | Pending | Pending | Pending |
| 6.4 | integer conversion | Signed/unsigned 8/16/32/64-bit conversions on all four surfaces | Pending | Pending | Pending |
| 6.4 | wide/native integer conversion | Int128/UInt128 and native-sized conversions on all four surfaces | Pending | Pending | Pending |
| 6.5 | IEEE binary conversion | Half/float/double conversions on all four surfaces | Pending | Pending | Pending |

## Ticket 01 evidence

| Scope | API/source | Preserved behavior | Independent standard correctness | Hardware |
|---|---|---|---|---|
| Raw uint32 two's-complement and absolute-magnitude primitives | Shared method emitter in `PositTemplate.tt`; generated partial `Posit32` and generated `Posit32E2` APIs | Verified by `Posit32AndPosit32E2ShareEquivalentBitOperations` against raw bit formulas for zero, NaR, signed extremes, and representative encodings | Not applicable: this refactor makes no Posit numerical-correctness claim | Hastlayer transformation/VHDL regression passed (40/40; approved snapshots unchanged); vendor synthesis/simulation/device evidence deferred |

`Posit32` and `Posit32E2` currently expose different adapters and have divergent numeric kernels. Ticket 01 does not certify their existing standard-operation results. See ticket 01 comments for the mapped correction tickets; ticket 56 audits that all those corrections converge on one shared implementation/generation source.

## Ticket 02 evidence

`PositEncodingReference` evaluates section 3.3's original signed fields into exact integer/binary-exponent pairs,
independent of production decoding or IEEE conversion. Hand-derived anchors validate positive/negative values,
zero/NaR, truncated exponent and regime fields, minPos and maxPos. E2 tests decode all 256 posit8 and 65,536 posit16
encodings, exhaust the 65,536 posit8 comparison pairs, traverse self/zero/NaR/adjacent posit16 pairs, and check
posit32 regime boundaries plus 4,096 seeded inputs and 8,192 seeded pairs per larger surface. Expected comparisons
use exact dyadics and independently agree with signed integer ordering. Every public comparison/equality surface
is checked; IEEE conversion, production numerical component helpers and rounding are not certified by these tests.

The existing 285 Arithmetics regressions remain enabled; all 307 tests pass, including new E0-E4 comparison checks.
`Posit32` and `Posit32E2` now share one template-emitted CompareTo algorithm, with declarations anchored for metadata
stability. Corrected ordering and added NaR/relational APIs are documented in `Readme.md`; no extension profile was removed.
Two forced MSBuild/T4 passes reproduce all 34 project-owned outputs identically.

`PositComparisonCalculator` supplies raw-memory comparison entry points for all four surfaces. Its CPU checks
exhaust posit8 pairs and compare larger boundary/seeded results against signed-integer flags. The new Catapult VHDL
approval includes signed casts at each precision and inlined typed comparison. The six existing Posit32 sample
snapshot updates were reviewed as intentional comparison corrections (remaining semantic blocks unchanged under
local-variable alpha-renaming), not blanket reapproval. Full VHDL suite: 44/44. Whole-solution rebuild: zero warnings/errors.
Vendor synthesis, simulation, device execution and hardware-dependent DynamicTests remain deferred evidence gaps.
