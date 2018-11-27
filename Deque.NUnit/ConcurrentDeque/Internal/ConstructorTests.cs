using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

using Deque.NUnit.Helpers;

using NUnit.Framework;

namespace Deque.NUnit.ConcurrentDeque.Internal
{
public class ConstructorTests
{
    [Test]
    public void PointersAreNullByDefault()
    {
        //Act
        var deque = new ConcurrentDeque<Int32>();

        //Assert
        var anchor = deque._anchor;
        Assert.Null(anchor._left);
        Assert.Null(anchor._right);
    }

    [Test]
    public void IsStableByDefault()
    {
        //Act
        var deque = new ConcurrentDeque<Int32>();

        //Assert
        var anchor = deque._anchor;
        Assert.AreEqual(ConcurrentDeque<Int32>.DequeStatus.Stable, anchor._status);
    }

    [Theory]
    public void WithEnumerableMaintainsPointersIntegrity(Int32[] collection)
    {
        var deque = new ConcurrentDeque<Int32>(collection);

        Assert.AreEqual(collection,           deque.TraverseLeftRight().Select(n => n._value));
        Assert.AreEqual(collection.Reverse(), deque.TraverseRightLeft().Select(n => n._value));
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
