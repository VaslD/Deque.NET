using System;
using System.Collections.Generic;

using NUnit.Framework;

namespace Deque.NUnit.Deque
{
public class IndexerTests
{
    [Test]
    public void NegativeIndexThrowsException()
    {
        var deque = new Deque<Int32>(new[] {1, 2, 3});
        Assert.Throws<ArgumentOutOfRangeException>(() => _ = deque[-1]);
    }

    [Test]
    public void IndexGreaterThanCountThrowsException()
    {
        var deque = new Deque<Int32>(new[] {1, 2, 3});
        Assert.Throws<ArgumentOutOfRangeException>(() => _ = deque[3]);
    }

    [Test]
    public void IndexerReturnsItemAtTheGivenIndex()
    {
        //make deque "wrap around" the ring buffer
        var deque = new Deque<Int32>(new[] {1, 2, 3, 4, 5});
        deque.PopLeft();
        deque.PopLeft();
        deque.PushRight(6);

        Int32[] expectedSequence = {3, 4, 5, 6};

        for (Int32 i = 0; i < deque.Count; i++) Assert.AreEqual(expectedSequence[i], deque[i]);
    }

    [Test]
    public void SetterChangesItemAtTheGivenIndex()
    {
        var deque = new Deque<Int32>(new[] {1, 2, 3});

        deque[1] = 4;

        Assert.AreEqual(new[] {1, 4, 3}, deque);
    }
}
}
