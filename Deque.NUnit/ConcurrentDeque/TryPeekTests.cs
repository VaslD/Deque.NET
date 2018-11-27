using System;
using System.Collections.Concurrent;

using NUnit.Framework;

namespace Deque.NUnit.ConcurrentDeque
{
public class TryPeekTests
{
    [Test]
    public void TryPeekRightInspectsTheRightmostItem()
    {
        var deque = new ConcurrentDeque<Int32>(new[] {1, 2, 3});

        Int32 item;
        Assert.True(deque.TryPeekRight(out item));
        Assert.AreEqual(3, item);
        Assert.Contains(3, deque);
    }

    [Test]
    public void TryPeekRightFailsIfDequeIsEmpty()
    {
        var deque = new ConcurrentDeque<Int32>();

        Int32 item;
        Assert.False(deque.TryPeekRight(out item));
    }

    [Test]
    public void TryPeekLeftInspectsTheLeftmostItem()
    {
        var deque = new ConcurrentDeque<Int32>(new[] {1, 2, 3});

        Int32 item;
        Assert.True(deque.TryPeekLeft(out item));
        Assert.AreEqual(1, item);
        Assert.Contains(1, deque);
    }

    [Test]
    public void TryPeekLeftFailsIfDequeIsEmpty()
    {
        var deque = new ConcurrentDeque<Int32>();

        Int32 item;
        Assert.False(deque.TryPeekLeft(out item));
    }
}
}
