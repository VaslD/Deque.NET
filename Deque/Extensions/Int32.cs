namespace System
{
internal static class Int32Extensions
{
    /// <summary>
    /// Applies the modulo operator.
    /// If <paramref name="n"/> is greater than zero, the result will be in the range [0, n-1].
    /// If it's less than zero, the result will be in the range [n+1, 0].
    /// </summary>
    /// <param name="a">The dividend.</param>
    /// <param name="n">The divisor. Must be different than zero.</param>
    /// <returns>The result of (<paramref name="a"/> mod <paramref name="n"/>).</returns>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="n"/> is zero.</exception>
    internal static Int32 Mod(this Int32 a, Int32 n)
    {
        if (n == 0) throw new ArgumentOutOfRangeException(nameof(n), "(a mod 0) is undefined.");

        //puts a in the [-n+1, n-1] range (for n > 0) using the remainder operator
        //or [n+1, -n-1] for n < 0.
        var remainder = a % n;

        //if the remainder is less than zero, add n to put it in the [0, n-1] range if n is positive
        //if the remainder is greater than zero, add n to put it in the [n+1, 0] range if n is negative
        if ((n > 0 && remainder < 0) || (n < 0 && remainder > 0)) return remainder + n;
        return remainder;
    }
}
}
