using System;
using System.Collections.Concurrent;

using NUnit.Framework;

namespace Deque.NUnit.ConcurrentDeque
{
public class PushTests
{
    [Test]
    public void PushRightAppendsNodeToEmptyDeque()
    {
        var deque = new ConcurrentDeque<Int32>();
        deque.PushRight(1);

        Assert.AreEqual(new[] {1}, deque);
    }

    [Test]
    public void PushRightAppendsNodeToNonEmptyList()
    {
        var deque = new ConcurrentDeque<Int32>(new[] {1, 2, 3});
        deque.PushRight(4);

        Assert.AreEqual(new[] {1, 2, 3, 4}, deque);
    }

    [Test]
    public void PushLeftAppendsNodeToEmptyDeque()
    {
        var deque = new ConcurrentDeque<Int32>();
        deque.PushLeft(1);

        Assert.AreEqual(new[] {1}, deque);
    }

    [Test]
    public void PushLeftAppendsNodeToNonEmptyList()
    {
        var deque = new ConcurrentDeque<Int32>(new[] {1, 2, 3});
        deque.PushLeft(0);

        Assert.AreEqual(new[] {0, 1, 2, 3}, deque);
    }
}
}
