# Posit Standard (2022) coverage matrix

The matrix inventories each required operation once; later tickets update only the rows they own. The API column names all required precision surfaces, with `Posit32` tracked as a compatibility adapter alongside `Posit32E2`. Evidence is deliberately split into compatibility preservation and normative conformance: a legacy result or a shared-kernel comparison is not a standard-correctness result.

Evidence values: `pending` means no ticket evidence recorded; `preserved` means a compatibility check passed; `verified` requires independent expected results against the PDF contract. Hardware evidence must distinguish transform/VHDL generation, vendor synthesis, and numerical simulation/device execution.

| PDF section | Operation | Public API inventory (`Posit8E2`, `Posit16E2`, `Posit32E2`, `Posit32`) | Preserved behavior | Independent standard software evidence | Hardware evidence |
|---|---|---|---|---|---|
| 5.2 | negate | Unary `-` on all four surfaces | Pending | Pending | Pending |
| 5.2 | abs | `Abs` on all four surfaces | Pending | Pending | Pending |
| 5.2 | sign | `Sign` on all four surfaces | Pending | Pending | Pending |
| 5.2 | nearestInt | `NearestInt` on all four surfaces | Pending | Pending | Pending |
| 5.2 | ceil | `Ceil` on all four surfaces | Pending | Pending | Pending |
| 5.2 | floor | `Floor` on all four surfaces | Pending | Pending | Pending |
| 5.2 | next | `Next` on all four surfaces | Pending | Pending | Pending |
| 5.2 | prior | `Prior` on all four surfaces | Pending | Pending | Pending |
| 5.3 | compareEqual | `==`, `Equals`, `CompareTo` on all four surfaces | Pending | Pending | Pending |
| 5.3 | compareNotEqual | `!=` on all four surfaces | Pending | Pending | Pending |
| 5.3 | compareGreater | `>` on all four surfaces | Pending | Pending | Pending |
| 5.3 | compareGreaterEqual | `>=` on all four surfaces | Pending | Pending | Pending |
| 5.3 | compareLess | `<` on all four surfaces | Pending | Pending | Pending |
| 5.3 | compareLessEqual | `<=` on all four surfaces | Pending | Pending | Pending |
| 5.4 | addition | `+` on all four surfaces | Pending | Pending | Pending |
| 5.4 | subtraction | `-` on all four surfaces | Pending | Pending | Pending |
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
| 6.1 | precision conversion | All six directed E2 8/16/32 conversions; `Posit32` compatibility conversions | Pending | Pending | Pending |
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
