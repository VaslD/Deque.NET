using System;
using System.Collections.Concurrent;

using NUnit.Framework;

namespace Deque.NUnit.ConcurrentDeque
{
public class IsEmptyTests
{
    [Test]
    public void IsEmptyReturnsTrueIfDequeIsEmpty()
    {
        var deque = new ConcurrentDeque<Int32>();

        Assert.True(deque.IsEmpty);
    }

    [Test]
    public void IsEmptyReturnsFalseIfDequeHasItems()
    {
        var deque = new ConcurrentDeque<Int32>(new[] {1});

        Assert.False(deque.IsEmpty);
    }
}
}
