using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace Lombiq.Arithmetics.Tests;

/// <summary>
/// Independent expected products and hard vectors for E2 multiplication, Standard (2022), sections 4.1 and 5.4. The
/// exact product of two finite operands is the dyadic (c1 × c2) × 2^(e1 + e2) of their section 3.3 decodings; the shared
/// <see cref="PositRoundingReference"/> rounds it once. No production arithmetic, IEEE conversion or midpoint is used.
/// </summary>
[SuppressMessage(
    "Security",
    "CA5394:Do not use insecure randomness",
    Justification = "Deterministic numerical test vectors, not security-sensitive randomness.")]
[SuppressMessage(
    "Security",
    "SCS0005:Weak random number generator",
    Justification = "Deterministic numerical test vectors, not security-sensitive randomness.")]
internal static class PositMultiplicationReference
{
    private const int Seed = 20_220_506;

    // Every other bit set (0x5555...); shifted left by one it gives the other alternating pattern.
    private const ulong AlternatingBits = ulong.MaxValue / 3;

    /// <summary>
    /// A pair whose exact product is the section 4.1 tie value V, the (n+1)-bit posit with representation U1, between
    /// the n-bit posits U and U+1 (<paramref name="Lower"/> is U).
    /// </summary>
    [StructLayout(LayoutKind.Auto)]
    public readonly record struct TiePair(uint Left, uint Right, uint Lower);

    public static uint Multiply(uint left, uint right, int size)
    {
        var a = PositEncodingReference.Decode(left, size);
        var b = PositEncodingReference.Decode(right, size);

        // Section 5.1: NaR is decided before zero, so 0 × NaR is NaR.
        var exact = a.IsNaR || b.IsNaR
            ? PositEncodingReference.NaR
            : PositEncodingReference.FromDyadic(a.Coefficient * b.Coefficient, a.Exponent + b.Exponent);
        return PositRoundingReference.Round(exact, size);
    }

    /// <summary>
    /// Returns the n-bit encoding of <paramref name="coefficient"/> × 2^<paramref name="exponent"/>, rounded if not
    /// representable.
    /// </summary>
    public static uint Exact(int coefficient, int exponent, int size) =>
        PositRoundingReference.Round(PositEncodingReference.FromDyadic(coefficient, exponent), size);

    /// <summary>Zero, NaR, the extremes, the neighbors of 1 and of the extremes, in both signs.</summary>
    public static IReadOnlyList<uint> SpecialOperands(int size)
    {
        var nar = 1U << (size - 1);
        var one = 1U << (size - 2);
        var operands = new SortedSet<uint> { 0, nar };
        foreach (var positive in new uint[] { 1, 2, 3, one - 1, one, one + 1, nar - 3, nar - 2, nar - 1 })
        {
            operands.Add(positive);
            operands.Add(PositAdditionReference.NegateBits(positive, size));
        }

        return [.. operands];
    }

    /// <summary>
    /// Every power of two from minPos to maxPos that the posit represents exactly (a posit with fewer than two exponent
    /// bits left after its regime cannot hold every exponent value), as (exponent, encoding).
    /// </summary>
    public static IReadOnlyList<(int Exponent, uint Bits)> PowersOfTwo(int size)
    {
        var maximum = (4 * size) - 8;
        var powers = new List<(int Exponent, uint Bits)>();
        for (var exponent = -maximum; exponent <= maximum; exponent++)
        {
            if (TryRepresent(PositEncodingReference.FromDyadic(1, exponent), size, out var bits)) powers.Add((exponent, bits));
        }

        return powers;
    }

    /// <summary>
    /// Terminated and unterminated regimes of every run length and sign (taken from the bit fields only, not from a
    /// decoder), followed by exponent/fraction tails of all zeros, a lone LSB, all ones, a lone MSB, a cleared MSB and
    /// the two alternating patterns. Both signs.
    /// </summary>
    public static IReadOnlyList<uint> PatternOperands(int size)
    {
        var operands = new SortedSet<uint>();
        foreach (var magnitude in PatternMagnitudes(size))
        {
            operands.Add(magnitude);
            operands.Add(PositAdditionReference.NegateBits(magnitude, size));
        }

        return [.. operands];
    }

    public static IEnumerable<(uint Left, uint Right)> AllPairs(IEnumerable<uint> operands)
    {
        var list = new List<uint>(operands);
        foreach (var left in list)
        {
            foreach (var right in list) yield return (left, right);
        }
    }

    /// <summary>Every 8-bit pair or, for larger sizes, every pair of the special, power-of-two and pattern operands.</summary>
    public static IEnumerable<(uint Left, uint Right)> CorePairs(int size)
    {
        if (size == 8)
        {
            for (uint left = 0; left <= byte.MaxValue; left++)
            {
                for (uint right = 0; right <= byte.MaxValue; right++) yield return (left, right);
            }

            yield break;
        }

        var operands = new SortedSet<uint>(SpecialOperands(size));
        foreach (var (_, bits) in PowersOfTwo(size)) operands.Add(bits);
        if (size <= 16) operands.UnionWith(PatternOperands(size));
        foreach (var pair in AllPairs(operands)) yield return pair;
    }

    /// <summary>
    /// Pairs drawn from the pattern operands and from scale-uniform random encodings (regime run length uniform, so
    /// every regime and the clamps are reached) and uniform random bit patterns.
    /// </summary>
    public static IEnumerable<(uint Left, uint Right)> SeededPairs(int size, int countPerKind)
    {
        var random = new Random(Seed + size);
        var mask = PositAdditionReference.Mask(size);
        var patterns = PatternOperands(size);
        for (var i = 0; i < countPerKind; i++)
        {
            yield return (patterns[random.Next(patterns.Count)], patterns[random.Next(patterns.Count)]);
            yield return (ScaleUniformOperand(random, size), ScaleUniformOperand(random, size));
            yield return ((uint)random.NextInt64((long)mask + 1), (uint)random.NextInt64((long)mask + 1));
        }
    }

    /// <summary>
    /// Multipliers that shift the scale across exponent and regime boundaries (so low fraction bits round, with exact
    /// ties or sticky bits) or that add a few fraction bits, both signs of one.
    /// </summary>
    public static IReadOnlyList<uint> Multipliers(int size) =>
    [
        PositAdditionReference.PowerOfTwo(-4, size),
        PositAdditionReference.PowerOfTwo(-3, size),
        PositAdditionReference.PowerOfTwo(-1, size),
        PositAdditionReference.PowerOfTwo(1, size),
        PositAdditionReference.PowerOfTwo(3, size),
        PositAdditionReference.PowerOfTwo(4, size),
        Exact(3, -1, size),
        Exact(5, -2, size),
        Exact(7, -2, size),
        PositAdditionReference.NegateBits(PositAdditionReference.PowerOfTwo(0, size), size),
    ];

    /// <summary>
    /// Pairs of every operand of <paramref name="operands"/> with each multiplier, so each regime, exponent and
    /// fraction pattern of the operand is rounded after a controlled shift.
    /// </summary>
    public static IEnumerable<(uint Left, uint Right)> SweepPairs(int size, IEnumerable<uint> operands)
    {
        var multipliers = Multipliers(size);
        foreach (var operand in operands)
        {
            foreach (var multiplier in multipliers) yield return (operand, multiplier);
        }
    }

    /// <summary>
    /// Exact section 4.1 ties built from the section 3.3 decoding of V = Decode(U1, n + 1): V's odd coefficient is split
    /// into a small odd divisor and its cofactor, and each factor is placed at every scale at which it is exactly
    /// representable. At most a few evenly spread pairs per divisor are returned for every U.
    /// </summary>
    public static IEnumerable<TiePair> TiePairs(int size, int randomTargets)
    {
        foreach (var lower in TieTargets(size, randomTargets))
        {
            var tie = PositEncodingReference.Decode(((ulong)lower << 1) | 1, size + 1);
            for (var divisor = 1; divisor <= 255; divisor += 2)
            {
                foreach (var pair in TiePairsForDivisor(size, tie, lower, divisor)) yield return pair;
            }
        }
    }

    private static IEnumerable<TiePair> TiePairsForDivisor(int size, PositEncodingReference tie, uint lower, int divisor)
    {
        if (!(tie.Coefficient % divisor).IsZero) yield break;

        var maximum = (4 * size) - 8;
        var cofactor = tie.Coefficient / divisor;
        var found = new List<TiePair>();
        for (var shift = -maximum; shift <= maximum; shift++)
        {
            if (TryRepresent(PositEncodingReference.FromDyadic(divisor, shift), size, out var left) &&
                TryRepresent(PositEncodingReference.FromDyadic(cofactor, tie.Exponent - shift), size, out var right))
            {
                found.Add(new TiePair(left, right, lower));
            }
        }

        var step = Math.Max(1, found.Count / (divisor == 1 ? 8 : 3));
        for (var i = 0; i < found.Count; i += step) yield return found[i];
    }

    private static bool TryRepresent(PositEncodingReference value, int size, out uint bits)
    {
        bits = PositRoundingReference.Round(value, size);
        return PositEncodingReference.Decode(bits, size).CompareTo(value) == 0;
    }

    private static IEnumerable<uint> TieTargets(int size, int randomTargets)
    {
        var maxPos = (1U << (size - 1)) - 1;
        var one = 1U << (size - 2);
        foreach (var lower in new uint[] { 1, 2, 3, 4, maxPos - 1, maxPos - 2, maxPos - 3, one - 2, one - 1, one, one + 1, one + 2 })
        {
            yield return lower;
        }

        var random = new Random(Seed + size);
        for (var i = 0; i < randomTargets; i++)
        {
            var lower = ScaleUniformMagnitude(random, size);
            if (lower < maxPos) yield return lower;
        }
    }

    private static IEnumerable<uint> PatternMagnitudes(int size)
    {
        foreach (var (regime, tailLength) in Regimes(size))
        {
            foreach (var tail in Tails(tailLength))
            {
                var bits = (uint)((regime << tailLength) | tail);
                if (bits != 0) yield return bits;
            }
        }
    }

    // Every regime field of the size - 1 bit posit body, taken from the bit layout only: a run of ones or of zeros of
    // every length, ended by the opposite bit unless the run fills the body, with the number of bits that follow it.
    private static IEnumerable<(ulong Regime, int TailLength)> Regimes(int size)
    {
        var bodyLength = size - 1;
        for (var run = 1; run <= bodyLength; run++)
        {
            var terminator = run < bodyLength ? 1 : 0;
            var tailLength = bodyLength - run - terminator;
            yield return (RegimeField(run, regimeBitIsOne: true, terminator), tailLength);
            yield return (RegimeField(run, regimeBitIsOne: false, terminator), tailLength);
        }
    }

    private static ulong RegimeField(int run, bool regimeBitIsOne, int terminator) =>
        regimeBitIsOne ? ((1UL << run) - 1) << terminator : (ulong)terminator;

    private static IEnumerable<ulong> Tails(int length)
    {
        if (length == 0)
        {
            yield return 0;
            yield break;
        }

        var all = (1UL << length) - 1;
        yield return 0;
        yield return 1;
        yield return all;
        yield return 1UL << (length - 1);
        yield return all >> 1;
        yield return (AlternatingBits << 1) & all;
        yield return AlternatingBits & all;
    }

    private static uint ScaleUniformMagnitude(Random random, int size)
    {
        var bodyLength = size - 1;
        while (true)
        {
            var run = random.Next(1, bodyLength + 1);
            var terminator = run < bodyLength ? 1 : 0;
            var tailLength = bodyLength - run - terminator;
            var regime = RegimeField(run, random.Next(2) == 0, terminator);
            var tail = (ulong)random.NextInt64() & ((1UL << tailLength) - 1);
            var bits = (uint)((regime << tailLength) | tail);
            if (bits != 0) return bits;
        }
    }

    private static uint ScaleUniformOperand(Random random, int size)
    {
        var magnitude = ScaleUniformMagnitude(random, size);
        return random.Next(2) == 0 ? magnitude : PositAdditionReference.NegateBits(magnitude, size);
    }
}
