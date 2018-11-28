using System;
using System.Extensions;
using System.Collections.Generic;

using NUnit.Framework;

namespace Deque.NUnit.Extensions
{
public class IntExtensionsTests
{
    [Theory]
    public void ModReturnsCorrectModulo(Int32 dividend, Int32 divisor, Int32 expectedMod)
    {
        Assert.AreEqual(expectedMod, dividend.Mod(divisor));
    }

    [Test]
    public void ModThrowsExceptionIfDivisorIsZero() { Assert.Throws<ArgumentOutOfRangeException>(() => 1.Mod(0)); }

    [Datapoints]
    public static IEnumerable<Object[]> GetTestData
    {
        get
        {
            yield return new Object[] {1, 1, 0};
            yield return new Object[] {0, 1, 0};
            yield return new Object[] {-5, 5, 0};
            yield return new Object[] {-5, -5, 0};
            yield return new Object[] {5, -5, 0};
            yield return new Object[] {5, 5, 0};
            yield return new Object[] {2, 10, 2};
            yield return new Object[] {12, 10, 2};
            yield return new Object[] {22, 10, 2};
            yield return new Object[] {-2, 10, 8};
            yield return new Object[] {-12, 10, 8};
            yield return new Object[] {-22, 10, 8};
            yield return new Object[] {2, -10, -8};
            yield return new Object[] {12, -10, -8};
            yield return new Object[] {22, -10, -8};
            yield return new Object[] {-2, -10, -2};
            yield return new Object[] {-12, -10, -2};
            yield return new Object[] {-22, -10, -2};
        }
    }
}
}
