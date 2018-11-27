using System;
using System.Collections.Generic;

using NUnit.Framework;

namespace Deque.NUnit.Deque
{
public class PeekTests
{
    [Test]
    public void PeekLeftThrowsExceptionWhenEmpty()
    {
        var deque = new Deque<Int32>();
        Assert.Throws<InvalidOperationException>(() => deque.PeekLeft());
    }

    [Test]
    public void PeekLeftReturnsLeftmostItem()
    {
        var deque = new Deque<Int32>(new[] {1, 2, 3});
        var item  = deque.PeekLeft();

        Assert.AreEqual(1, item);
    }

    [Test]
    public void PeekLeftDoesNotRemoveItem()
    {
        var deque = new Deque<Int32>(new[] {1, 2, 3});
        deque.PeekLeft();

        Assert.AreEqual(new[] {1, 2, 3}, deque);
    }

    [Test]
    public void PeekRightThrowsExceptionWhenEmpty()
    {
        var deque = new Deque<Int32>();
        Assert.Throws<InvalidOperationException>(() => deque.PeekRight());
    }

    [Test]
    public void PeekRightReturnsRightmostItem()
    {
        var deque = new Deque<Int32>(new[] {1, 2, 3});
        var item  = deque.PeekRight();

        Assert.AreEqual(3, item);
    }

    [Test]
    public void PeekRightReturnsRightmostItemWhenDequeLoopsAround()
    {
        var deque = new Deque<Int32>(new[] {1, 2, 3});
        deque.PopLeft();
        deque.PushRight(4);

        var item = deque.PeekRight();

        Assert.AreEqual(4, item);
    }

    [Test]
    public void PeekRightDoesNotRemoveItem()
    {
        var deque = new Deque<Int32>(new[] {1, 2, 3});
        deque.PeekRight();

        Assert.AreEqual(new[] {1, 2, 3}, deque);
    }
}
}
