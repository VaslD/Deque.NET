using System;
using System.Collections.Generic;

using NUnit.Framework;

namespace Deque.NUnit.Deque
{
public class CapacityTests
{
    [Test]
    public void SettingToLessThanCountThrowsException()
    {
        var deque = new Deque<Int32>();
        deque.PushRight(5);

        Assert.Throws<ArgumentOutOfRangeException>(() => deque.Capacity = 0);
    }

    [Test]
    public void SettingToHigherThanCountIncreasesCapacity()
    {
        var deque = new Deque<Int32>();
        deque.Capacity = 6;

        Assert.AreEqual(6, deque.Capacity);
    }

    [Test]
    public void SettingToCurrentCapacityDoesNothing()
    {
        var deque = new Deque<Int32>(5);
        deque.Capacity = 5;
        Assert.AreEqual(5, deque.Capacity);
    }

    [Test]
    public void SettingCopiesItems()
    {
        var deque = new Deque<Int32>(new[] {1, 2, 3, 4});
        deque.PopLeft();

        deque.Capacity = 7;

        Assert.AreEqual(new[] {2, 3, 4}, deque);
    }

    [Test]
    public void SettingWhenDequeLoopsAroundArrayCopiesItems()
    {
        var deque = new Deque<Int32>(new[] {1, 2, 3, 4});
        deque.PopLeft();
        deque.PopLeft();
        deque.PushRight(5);

        deque.Capacity = 3;

        Assert.AreEqual(new[] {3, 4, 5}, deque);
    }

    [Test]
    public void TrimExcessCompactsCapacity()
    {
        var deque = new Deque<Int32>(new[] {1, 2, 3});
        deque.PushRight(4);

        Assert.True(deque.Capacity > 4);

        deque.TrimExcess();

        Assert.AreEqual(4, deque.Capacity);
    }
}
}
