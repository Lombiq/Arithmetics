using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Xunit;
using Reference = Lombiq.Arithmetics.Tests.PositMultiplicationReference;

namespace Lombiq.Arithmetics.Tests;

public class PositMultiplicationTests
{
    private const int MaximumReportedMismatches = 20;

    private static readonly Surface[] Posit8Surfaces =
    [
        new("Posit8E2", (left, right) =>
            (new Posit8E2((byte)left, fromBitMask: true) * new Posit8E2((byte)right, fromBitMask: true)).PositBits),
    ];

    private static readonly Surface[] Posit16Surfaces =
    [
        new("Posit16E2", (left, right) =>
            (new Posit16E2((ushort)left, fromBitMask: true) * new Posit16E2((ushort)right, fromBitMask: true)).PositBits),
    ];

    private static readonly Surface[] Posit32Surfaces =
    [
        new("Posit32E2", (left, right) =>
            (new Posit32E2(left, fromBitMask: true) * new Posit32E2(right, fromBitMask: true)).PositBits),
        new("Posit32", (left, right) =>
            (new Posit32(left, fromBitMask: true) * new Posit32(right, fromBitMask: true)).PositBits),
    ];

    [Fact]
    public void Posit8E2ProductsMatchTheExactReferenceForEveryPair() =>
        Verify(8, Reference.CorePairs(8));

    [Fact]
    public void Posit16E2ProductsMatchTheExactReferenceOnHardAndSeededVectors()
    {
        Verify(16, Reference.CorePairs(16));
        Verify(16, Reference.SeededPairs(16, 12_000));
        Verify(16, Reference.SweepPairs(16, AllEncodings(16)));
    }

    [Fact]
    public void Posit32E2AndPosit32ProductsMatchTheExactReferenceOnHardAndSeededVectors()
    {
        Verify(32, Reference.CorePairs(32));
        Verify(32, Reference.SeededPairs(32, 12_000));

        var operands = new List<uint>(Reference.PatternOperands(32));
        operands.AddRange(Reference.SeededPairs(32, 2_000).Select(pair => pair.Left));
        Verify(32, Reference.SweepPairs(32, operands));
    }

    [Theory]
    [InlineData(8)]
    [InlineData(16)]
    [InlineData(32)]
    public void PowersOfTwoMultiplyExactlyAndClampAtTheExtremes(int size)
    {
        var maximum = (4 * size) - 8;
        var powers = Reference.PowersOfTwo(size);

        // The exact product is 2^(left + right): maxPos or minPos once it leaves the posit range, and otherwise the
        // power itself or, where the posit cannot hold that exponent, its section 4.1 rounding.
        var expectedByExponent = new uint[(2 * maximum) + 1];
        for (var exponent = -maximum; exponent <= maximum; exponent++)
        {
            expectedByExponent[exponent + maximum] = Reference.PowerOfTwo(exponent, size);
        }

        var failures = new List<string>();
        foreach (var (leftExponent, left) in powers)
        {
            foreach (var (rightExponent, right) in powers)
            {
                var expected = expectedByExponent[Math.Clamp(leftExponent + rightExponent, -maximum, maximum) + maximum];
                foreach (var surface in SurfacesFor(size))
                {
                    Check(failures, surface, left, right, expected);
                    Check(failures, surface, Reference.Negate(left, size), right, Reference.Negate(expected, size));
                    Check(failures, surface, Reference.Negate(left, size), Reference.Negate(right, size), expected);
                }
            }
        }

        Assert.Empty(failures);
    }

    [Theory]
    [InlineData(8)]
    [InlineData(16)]
    [InlineData(32)]
    public void NaRIsDecidedBeforeZeroAndZeroIsExact(int size)
    {
        var nar = 1U << (size - 1);
        var others = new List<uint>(Reference.SpecialOperands(size));
        others.AddRange(Reference.PatternOperands(size));
        others.AddRange(Reference.PowersOfTwo(size).Select(power => power.Bits));
        var failures = new List<string>();
        foreach (var other in others)
        {
            foreach (var surface in SurfacesFor(size))
            {
                // Section 5.1: any NaR operand gives NaR, even 0 × NaR; otherwise a zero operand gives exactly 0.
                var expected = other == nar ? nar : 0U;
                Check(failures, surface, 0, other, expected);
                Check(failures, surface, other, 0, expected);
                Check(failures, surface, nar, other, nar);
                Check(failures, surface, other, nar, nar);
            }
        }

        Assert.Empty(failures);
    }

    [Fact]
    public void HandDerivedPosit8ProductsMatch() =>
        VerifyLiteralVectors(
            8,
            (0x40, 0x40, 0x40), // 1 * 1 = 1.
            (0x48, 0x48, 0x50), // 2 * 2 = 4: the exponent field counts up.
            (0x44, 0x44, 0x49), // 1.5 * 1.5 = 2.25 = 2 * 1.125 is exact: the significand product carries into the exponent.
            (0x42, 0x42, 0x44), // 1.25^2 = 1.5625 is the tie between 1.5 (U = 0x44, even) and 1.625 (v = 1.5625): rounds to 0x44.
            (0x43, 0x43, 0x47), // 1.375^2 = 1.890625 < v = 1.9375 between 1.875 (0x47) and 2: rounds down.
            (0x46, 0x46, 0x4C), // 1.75^2 = 3.0625 < v = 3.125 between 3 (0x4C) and 3.25: rounds down.
            (0x48, 0x5B, 0x62), // 2 * 11 = 22 is the tie between 20 (U = 0x61, odd) and 24 (v = 22): rounds up to 0x62.
            (0x48, 0x5D, 0x62), // 2 * 13 = 26 is the tie between 24 (U = 0x62, even) and 28 (v = 26): rounds down to 0x62.
            (0x76, 0x76, 0x7E), // 2^11 * 2^11 = 2^22 is the tie between 2^20 (U = 0x7E, even) and 2^24 (v = 2^22): down.
            (0x0A, 0x0A, 0x02), // 2^-11 * 2^-11 = 2^-22 is the tie between 2^-24 (U = 0x01, odd) and 2^-20 (v = 2^-22): up.
            (0x7F, 0x7F, 0x7F), // maxPos * maxPos = 2^48 overflows: clamps to maxPos.
            (0x01, 0x01, 0x01), // minPos * minPos = 2^-48 underflows to nonzero: clamps to minPos, never 0.
            (0x7F, 0x01, 0x40), // 2^24 * 2^-24 = 1 exactly.
            (0x7F, 0x48, 0x7F), // maxPos * 2 overflows.
            (0x01, 0x38, 0x01), // minPos * 0.5 underflows.
            (0xC0, 0x40, 0xC0), // -1 * 1 = -1.
            (0xC0, 0xC0, 0x40), // -1 * -1 = 1.
            (0x81, 0x7F, 0x81), // -maxPos * maxPos overflows to -maxPos.
            (0xFF, 0xFF, 0x01), // -minPos * -minPos underflows to +minPos.
            (0xFF, 0x7F, 0xC0), // -minPos * maxPos = -1.
            (0x00, 0x80, 0x80), // 0 * NaR = NaR: NaR is decided before zero.
            (0x80, 0x80, 0x80), // NaR * NaR = NaR.
            (0x80, 0x40, 0x80), // NaR * 1 = NaR.
            (0x00, 0x40, 0x00), // 0 * 1 = 0.
            (0x00, 0x7F, 0x00)); // 0 * maxPos = 0.

    [Fact]
    public void HandDerivedPosit16ProductsMatch() =>
        VerifyLiteralVectors(
            16,
            (0x4000, 0x4000, 0x4000), // 1 * 1 = 1.
            (0x4001, 0x4000, 0x4001), // (1 + 2^-11) * 1 keeps its LSB.
            (0x4800, 0x5800, 0x6000), // 2 * 8 = 16: the exponent field wraps into the regime.
            (0x7000, 0x7000, 0x7C00), // 2^8 * 2^8 = 2^16.
            (0x7C00, 0x4008, 0x7C00), // 2^16 * (1 + 2^-8) is the tie between U = 0x7C00 (even) and 2^16 * (1 + 2^-7): down.
            (0x7C00, 0x4018, 0x7C02), // 2^16 * (1 + 3 * 2^-8) is the tie between U = 0x7C01 (odd) and 0x7C02: up.
            (0x7FFF, 0x7FFF, 0x7FFF), // maxPos * maxPos clamps.
            (0x0001, 0x0001, 0x0001), // minPos * minPos clamps.
            (0x0001, 0x7FFF, 0x4000), // 2^-56 * 2^56 = 1.
            (0x7FFF, 0x4800, 0x7FFF), // maxPos * 2 clamps.
            (0x0001, 0x3800, 0x0001), // minPos * 0.5 clamps to nonzero.
            (0x8001, 0x4800, 0x8001), // -maxPos * 2 clamps.
            (0x0000, 0x8000, 0x8000), // 0 * NaR = NaR.
            (0x8000, 0x8000, 0x8000), // NaR * NaR = NaR.
            (0x0000, 0x7FFF, 0x0000)); // 0 * maxPos = 0.

    [Fact]
    public void HandDerivedPosit32ProductsMatch() =>
        VerifyLiteralVectors(
            32,
            (0x4000_0000, 0x4000_0000, 0x4000_0000), // 1 * 1 = 1.
            (0x7FFF_8000, 0x7FFF_8000, 0x7FFF_FFFF), // 2^60 * 2^60 = 2^120 = maxPos exactly.
            (0x7FFF_8001, 0x7FFF_8000, 0x7FFF_FFFF), // 2^60 * (1 + 2^-12) * 2^60 overflows past maxPos.
            (0x7FFF_FFFF, 0x7FFF_FFFF, 0x7FFF_FFFF), // maxPos * maxPos clamps.
            (0x0000_0001, 0x0000_0001, 0x0000_0001), // minPos * minPos clamps.
            (0x7FFF_FFFF, 0x0000_0001, 0x4000_0000), // 2^120 * 2^-120 = 1.
            (0x7FFF_FFFF, 0x4800_0000, 0x7FFF_FFFF), // maxPos * 2 clamps.
            (0x0000_0001, 0x3800_0000, 0x0000_0001), // minPos * 0.5 clamps to nonzero.
            (0x0000_0000, 0x8000_0000, 0x8000_0000), // 0 * NaR = NaR.
            (0x8000_0000, 0x8000_0000, 0x8000_0000), // NaR * NaR = NaR.
            (0x0000_0000, 0x7FFF_FFFF, 0x0000_0000)); // 0 * maxPos = 0.

    [Fact]
    public void ExactTiesAndNearbyStickyBitsFollowSection41()
    {
        // With ulp = 2^-27 near 1: 1.5 * (1 + ulp) is 1.5 + 1.5 ulp, exactly halfway between 0x44000001 (odd) and 0x44000002.
        // The other three products lie 2^-54, 9 * 2^-54 and 2^-54 above or below such a midpoint: only the lowest
        // product bits decide them, so a rounding that ignores them rounds half to even and gets the wrong neighbor.
        VerifyLiteralVectors(
            32,
            (0x4000_0001, 0x4400_0000, 0x4400_0002), // Tie above odd U = 0x44000001: rounds up to the even neighbor.
            (0x4000_0001, 0x4400_0001, 0x4400_0003), // Tie + 2^-54 above even U = 0x44000002: sticky bit rounds up.
            (0x4000_0001, 0x43FF_FFFF, 0x4400_0000), // Tie - 2^-54 above even U = 0x44000000: rounds down.
            (0x4000_0003, 0x43FF_FFFD, 0x4400_0001)); // Tie - 9 * 2^-54 above odd U = 0x44000001: sticky rounds down.
    }

    [Theory]
    [InlineData(16, 400)]
    [InlineData(32, 40)]
    public void GeneratedTiesRoundToEvenAndTheirNeighborsAreMonotonic(int size, int randomTargets)
    {
        var surfaces = SurfacesFor(size);
        var failures = new List<string>();
        var parities = new int[2];
        var nar = 1U << (size - 1);
        foreach (var (left, right, lower) in Reference.TiePairs(size, randomTargets))
        {
            // Section 4.1: the exact tie V = Decode(U1) goes to the even representation of U and U + 1.
            var rounded = (lower & 1) == 0 ? lower : lower + 1;
            parities[lower & 1]++;
            CheckAllSigns(failures, size, left, right, rounded);

            // One ulp above or below an operand moves the exact product off the tie, so the result may not cross it.
            // For tie products the perturbation is far below the spacing, so this isolates the sticky bit.
            foreach (var (changedLeft, changedRight) in new[]
            {
                (left - 1, right), (left + 1, right), (left, right - 1), (left, right + 1),
            })
            {
                if (changedLeft >= nar || changedRight >= nar) continue;

                var neighbor = Reference.Multiply(changedLeft, changedRight, size);
                var neighborIsAbove = changedLeft > left || changedRight > right;
                if (neighborIsAbove ? neighbor < rounded : neighbor > rounded)
                {
                    AddFailure(failures, "Reference monotonicity", changedLeft, changedRight, rounded, neighbor);
                }

                foreach (var surface in surfaces) Check(failures, surface, changedLeft, changedRight, neighbor);
            }

            if (failures.Count >= MaximumReportedMismatches) break;
        }

        Assert.Empty(failures);
        Assert.True(
            parities[0] >= 100 && parities[1] >= 100,
            string.Create(
                CultureInfo.InvariantCulture,
                $"Too few exact ties for posit{size}: {parities[0]} below an even U and {parities[1]} below an odd U."));
    }

    [Theory]
    [InlineData(-3)]
    [InlineData(-1)]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    public void IntegerOverloadsUseTheStandardProduct(int integer)
    {
        foreach (var bits in new uint[] { 0x40, 0x44, 0x5B, 0xC4, 0x76, 0x0A, 0x7F, 0x81, 0x00, 0x80 })
        {
            Assert.Equal(
                Reference.Multiply(bits, Reference.Exact(integer, 0, 8), 8),
                (new Posit8E2((byte)bits, fromBitMask: true) * integer).PositBits);
            Assert.Equal(
                Reference.Multiply(bits << 8, Reference.Exact(integer, 0, 16), 16),
                (new Posit16E2((ushort)(bits << 8), fromBitMask: true) * integer).PositBits);
            Assert.Equal(
                Reference.Multiply(bits << 24, Reference.Exact(integer, 0, 32), 32),
                (new Posit32E2(bits << 24, fromBitMask: true) * integer).PositBits);
            Assert.Equal(
                Reference.Multiply(bits << 24, Reference.Exact(integer, 0, 32), 32),
                (new Posit32(bits << 24, fromBitMask: true) * integer).PositBits);
        }
    }

    private static IReadOnlyList<Surface> SurfacesFor(int size) => size switch
    {
        8 => Posit8Surfaces,
        16 => Posit16Surfaces,
        _ => Posit32Surfaces,
    };

    private static IEnumerable<uint> AllEncodings(int size)
    {
        for (uint bits = 0; bits <= Reference.Mask(size); bits++) yield return bits;
    }

    private static void Verify(int size, IEnumerable<(uint Left, uint Right)> pairs)
    {
        var surfaces = SurfacesFor(size);
        var failures = new List<string>();
        foreach (var (left, right) in pairs)
        {
            var expected = Reference.Multiply(left, right, size);
            foreach (var surface in surfaces)
            {
                Check(failures, surface, left, right, expected);
                Check(failures, surface, right, left, expected);
            }

            if (failures.Count >= MaximumReportedMismatches) break;
        }

        Assert.Empty(failures);
    }

    // Each vector holds for the operands in either order and for either sign of either operand (negating a posit
    // negates the real value, and NaR and zero are their own negations). The independent reference must agree too.
    private static void VerifyLiteralVectors(int size, params (uint Left, uint Right, uint Expected)[] vectors)
    {
        var failures = new List<string>();
        foreach (var (left, right, expected) in vectors) CheckAllSigns(failures, size, left, right, expected);

        Assert.Empty(failures);
    }

    private static void CheckAllSigns(List<string> failures, int size, uint left, uint right, uint expected)
    {
        foreach (var negateLeft in new[] { false, true })
        {
            foreach (var negateRight in new[] { false, true })
            {
                var signedLeft = negateLeft ? Reference.Negate(left, size) : left;
                var signedRight = negateRight ? Reference.Negate(right, size) : right;
                var signedExpected = negateLeft == negateRight ? expected : Reference.Negate(expected, size);

                var reference = Reference.Multiply(signedLeft, signedRight, size);
                if (reference != signedExpected) AddFailure(failures, "Reference", signedLeft, signedRight, signedExpected, reference);

                foreach (var surface in SurfacesFor(size))
                {
                    Check(failures, surface, signedLeft, signedRight, signedExpected);
                    Check(failures, surface, signedRight, signedLeft, signedExpected);
                }
            }
        }
    }

    private static void Check(List<string> failures, Surface surface, uint left, uint right, uint expected)
    {
        var actual = surface.Multiply(left, right);
        if (actual != expected) AddFailure(failures, surface.Name, left, right, expected, actual);
    }

    private static void AddFailure(List<string> failures, string source, uint left, uint right, uint expected, uint actual)
    {
        if (failures.Count >= MaximumReportedMismatches) return;

        failures.Add(string.Create(
            CultureInfo.InvariantCulture,
            $"{source}: 0x{left:X8} * 0x{right:X8} expected 0x{expected:X8}, got 0x{actual:X8}."));
    }

    private sealed record Surface(string Name, Func<uint, uint, uint> Multiply);
}
