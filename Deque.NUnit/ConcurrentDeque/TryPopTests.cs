using System;
using System.Collections.Concurrent;

using NUnit.Framework;

namespace Deque.NUnit.ConcurrentDeque
{
public class TryPopTests
{
    [Test]
    public void TryPopRightFailsOnEmptyDeque()
    {
        //Arrange
        var deque = new ConcurrentDeque<Int32>();

        //Act & Assert
        Int32 item;
        Assert.False(deque.TryPopRight(out item));
        Assert.AreEqual(item, default(Int32));
    }

    [Test]
    public void TryPopRightReturnsTheLastRemainingItem()
    {
        //Arrange
        var deque = new ConcurrentDeque<Int32>();
        deque.PushRight(1);

        //Act & Assert
        Int32 item;
        Assert.True(deque.TryPopRight(out item));
        Assert.AreEqual(item, 1);
        Assert.AreEqual(0,    deque.Count);
    }

    [Test]
    public void TryPopRightReturnsTheRightmostItem()
    {
        //Arrange
        var deque = new ConcurrentDeque<Int32>(new[] {1, 3, 5});

        //Act & Assert
        Int32 item;
        Assert.True(deque.TryPopRight(out item));
        Assert.AreEqual(item,         5);
        Assert.AreEqual(new[] {1, 3}, deque);
    }

    [Test]
    public void TryPopLeftFailsOnEmptyDeque()
    {
        //Arrange
        var deque = new ConcurrentDeque<Int32>();

        //Act & Assert
        Int32 item;
        Assert.False(deque.TryPopLeft(out item));
        Assert.AreEqual(item, default(Int32));
    }

    [Test]
    public void TryPopLeftReturnsTheLastRemainingItem()
    {
        //Arrange
        var deque = new ConcurrentDeque<Int32>();
        deque.PushRight(1);

        //Act & Assert
        Int32 item;
        Assert.True(deque.TryPopLeft(out item));
        Assert.AreEqual(item, 1);
        Assert.AreEqual(0,    deque.Count);
    }

    [Test]
    public void TryPopLeftReturnsTheLeftmostItem()
    {
        //Arrange
        var deque = new ConcurrentDeque<Int32>(new[] {1, 3, 5});

        //Act & Assert
        Int32 item;
        Assert.True(deque.TryPopLeft(out item));
        Assert.AreEqual(item,         1);
        Assert.AreEqual(new[] {3, 5}, deque);
    }
}
}
