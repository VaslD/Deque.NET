using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

using NUnit.Framework;

namespace Deque.NUnit.ConcurrentDeque
{
public class ConstructorTests
{
    [Test]
    public void HasNoItemsByDefault()
    {
        var deque = new ConcurrentDeque<Int32>();
        Assert.IsEmpty(deque);
    }

    [Test]
    public void WithNullIEnumerableThrowsException()
    {
        Assert.Throws<ArgumentNullException>(() => new ConcurrentDeque<Int32>(null));
    }

    [Theory]
    public void WithIEnumerableCopiesCollection(Int32[] collection)
    {
        Int32[] array = {1, 2, 3, 4};
        var     deque = new ConcurrentDeque<Int32>(array);

        Assert.AreEqual(array, deque);
    }

    [Datapoints]
    public static IEnumerable<Object[]> Items
        => new[]
           {
               new Object[] {new Int32[] { }},
               new Object[] {new[] {1}},
               new Object[] {new[] {1, 1}}
           };
}
}
