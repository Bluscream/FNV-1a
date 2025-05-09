﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Fnv1a32.cs" company="Always Elucidated Solution Pioneers, LLC">
//   Copyright (c) Always Elucidated Solution Pioneers, LLC. All rights reserved.
// </copyright>
// <summary>
//   Implements the FNV-1a 32-bit variant hashing algorithm.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

// Ignore Spelling: Fnv ib
namespace Fnv1a;

using System;
using System.Diagnostics.CodeAnalysis;
using System.Security.Cryptography;

/// <inheritdoc cref="HashAlgorithm" />
/// <summary>
/// Implements the FNV-1a 32-bit variant hashing algorithm.
/// </summary>
// ReSharper disable once InconsistentNaming
#pragma warning disable S101 // Types should be named in PascalCase
public sealed class Fnv1a32 : HashAlgorithm
#pragma warning restore S101 // Types should be named in PascalCase
{
    /// <summary>
    /// The default prime.
    /// </summary>
    private const uint FnvDefaultPrime = 0x01000193U;

    /// <summary>
    /// The default non-zero offset basis.
    /// </summary>
    private const uint FnvDefaultOffsetBasis = 0x811C9DC5U;

    /// <summary>
    /// The hash.
    /// </summary>
    private uint _hash;

    /// <inheritdoc cref="HashAlgorithm" />
    /// <summary>
    /// Initializes a new instance of the <see cref="Fnv1a32" /> class.
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException">The offset basis must be non-zero.</exception>
    public Fnv1a32()
        : this(FnvDefaultPrime, FnvDefaultOffsetBasis)
    {
        // Intentionally empty.
    }

    /// <inheritdoc cref="HashAlgorithm" />
    /// <summary>
    /// Initializes a new instance of the <see cref="Fnv1a32" /> class.
    /// </summary>
    /// <param name="prime">The prime.</param>
    /// <param name="offsetBasis">The non-zero offset basis.</param>
    /// <exception cref="ArgumentOutOfRangeException">The offset basis must be non-zero.</exception>
    public Fnv1a32(uint prime, uint offsetBasis)
    {
        if (offsetBasis == 0U)
        {
            throw new ArgumentOutOfRangeException(
                nameof(offsetBasis),
                offsetBasis,
                "The offset basis must be non-zero.");
        }

        HashSizeValue = 32;
        FnvPrime = prime;
        FnvOffsetBasis = offsetBasis;
        Initialize();
    }

    /// <summary>
    /// Gets the prime.
    /// </summary>
    /// <value>
    /// The prime.
    /// </value>
    public uint FnvPrime { get; }

    /// <summary>
    /// Gets the non-zero offset basis.
    /// </summary>
    /// <value>
    /// The non-zero offset basis.
    /// </value>
    public uint FnvOffsetBasis { get; }

    /// <inheritdoc />
    /// <summary>
    /// Initializes an implementation of the <see cref="HashAlgorithm" /> class.
    /// </summary>
    public override void Initialize() => _hash = FnvOffsetBasis;

    /// <inheritdoc />
    /// <summary>
    /// When overridden in a derived class, routes data written to the object into the hash algorithm for computing
    /// the hash.
    /// </summary>
    /// <param name="array">The input to compute the hash code for.</param>
    /// <param name="ibStart">The offset into the byte array from which to begin using data.</param>
    /// <param name="cbSize">The number of bytes in the byte array to use as data.</param>
#pragma warning disable IDE0079 // Remove unnecessary suppression
    [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1305:FieldNamesMustNotUseHungarianNotation", Justification = "Reviewed. Suppression is OK here.")]
#pragma warning restore IDE0079 // Remove unnecessary suppression
    protected override void HashCore(byte[] array, int ibStart, int cbSize)
    {
        for (int i = ibStart; i < cbSize; i++)
        {
            unchecked
            {
                _hash ^= array[i];
                _hash *= FnvPrime;
            }
        }
    }

    /// <summary>
    /// Routes data written to the object into the hash algorithm for computing the hash.
    /// </summary>
    /// <param name="source">The input to compute the hash code for.</param>
    protected override void HashCore(ReadOnlySpan<byte> source)
    {
        foreach (byte b in source)
        {
            unchecked
            {
                _hash ^= b;
                _hash *= FnvPrime;
            }
        }
    }

    /// <inheritdoc />
    /// <summary>
    /// When overridden in a derived class, finalizes the hash computation after the last data is processed by the
    /// cryptographic stream object.
    /// </summary>
    /// <returns>
    /// The computed hash code.
    /// </returns>
    protected override byte[] HashFinal() => BitConverter.GetBytes(_hash);

    /// <summary>
    /// Attempts to finalize the hash computation after the last data is processed by the hash algorithm.
    /// </summary>
    /// <param name="destination">The buffer to receive the hash value.</param>
    /// <param name="bytesWritten">When this method returns, the total number of bytes written into
    /// <paramref name="destination" />. This parameter is treated as uninitialized.</param>
    /// <returns><see langword="true" /> if <paramref name="destination" /> is long enough to receive the hash
    /// value; otherwise, <see langword="false" />.</returns>
    /// <exception cref="ArgumentException">The destination Span is shorter than the source array.</exception>
    /// <exception cref="OverflowException">The array is multidimensional and contains more than <see cref="int.MaxValue" /> elements.</exception>
    protected override bool TryHashFinal(Span<byte> destination, out int bytesWritten)
    {
        byte[] bytes = BitConverter.GetBytes(_hash);

        bytes.CopyTo(destination);
        bytesWritten = bytes.Length;
        return true;
    }
}
