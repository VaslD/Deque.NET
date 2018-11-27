using System;
using System.Collections.Generic;

using NUnit.Framework;

namespace Deque.NUnit.Deque
{
public class PushTests
{
    [Test]
    public void PushRightAppendsItemToTheRightEnd()
    {
        var deque = new Deque<Int32>();
        deque.PushRight(1);
        deque.PushRight(2);
        deque.PushRight(3);

        Assert.AreEqual(new[] {1, 2, 3}, deque);
    }

    [Test]
    public void PushRightToEmptyDequeIncreasesCapacity()
    {
        var deque = new Deque<Int32>();
        deque.PushRight(5);

        Assert.AreEqual(4, deque.Capacity);
    }

    [Test]
    public void PushRightDoublesCapacity()
    {
        var deque = new Deque<Int32>();

        for (Int32 i = 0; i < 5; i++) deque.PushRight(5);

        Assert.AreEqual(8, deque.Capacity);
    }

    [Test]
    public void PushRightIncreasesCount()
    {
        var deque = new Deque<Int32>();

        for (Int32 i = 0; i < 5; i++) deque.PushRight(5);

        Assert.AreEqual(5, deque.Count);
    }

    [Test]
    public void PushRightLoopsAroundBuffer()
    {
        var deque = new Deque<Int32>(new[] {1, 2, 3});
        deque.PopLeft();
        deque.PushRight(4);

        Assert.AreEqual(3,               deque.Capacity);
        Assert.AreEqual(new[] {2, 3, 4}, deque);
    }

    [Test]
    public void PushLeftAppendsItemToTheLeftEnd()
    {
        var deque = new Deque<Int32>();
        deque.PushLeft(1);
        deque.PushLeft(2);
        deque.PushLeft(3);

        Assert.AreEqual(new[] {3, 2, 1}, deque);
    }

    [Test]
    public void PushLeftToEmptyDequeIncreasesCapacity()
    {
        var deque = new Deque<Int32>();
        deque.PushLeft(5);

        Assert.AreEqual(4, deque.Capacity);
    }

    [Test]
    public void PushLeftDoublesCapacity()
    {
        var deque = new Deque<Int32>();

        for (Int32 i = 0; i < 5; i++) deque.PushLeft(5);

        Assert.AreEqual(8, deque.Capacity);
    }

    [Test]
    public void PushLeftIncreasesCount()
    {
        var deque = new Deque<Int32>();

        for (Int32 i = 0; i < 5; i++) deque.PushLeft(5);

        Assert.AreEqual(5, deque.Count);
    }

    [Test]
    public void PushLeftLoopsAroundBuffer()
    {
        var deque = new Deque<Int32>(new[] {1, 2, 3});
        deque.PopRight();
        deque.PushLeft(0);

        Assert.AreEqual(3,               deque.Capacity);
        Assert.AreEqual(new[] {0, 1, 2}, deque);
    }
}
}
