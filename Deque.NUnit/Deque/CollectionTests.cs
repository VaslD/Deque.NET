using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Deque.NUnit.Helpers;

using NUnit.Framework;

namespace Deque.NUnit.Deque
{
public class CollectionTests
{
    [Test]
    public void CopyToWithMultiDimensionalArrayThrowsException()
    {
        ICollection deque = new Deque<Int32>(new[] {1, 2, 3});
        var         array = new Int32[2, 2];

        var ex = Assert.Throws<ArgumentException>(() => deque.CopyTo(array, 0));
        Assert.That(ex.Message, Contains.Substring("dimension"));

        Assert.True(array.AllDefault<Int32>());
    }

    [Test]
    public void CopyToWithArrayOfWrongTypeThrowsException()
    {
        ICollection deque = new Deque<Int32>(new[] {1, 2, 3});
        var         array = new Int32[3][];

        var ex = Assert.Throws<ArgumentException>(() => deque.CopyTo(array, 0));
        Assert.That(ex.Message, Contains.Substring("array type"));

        Assert.True(array.AllDefault());
    }

    [Test]
    public void CopyToWithNullArrayThrowsException()
    {
        ICollection deque = new Deque<Int32>(new[] {1, 2, 3});
        Assert.Throws<ArgumentNullException>(() => deque.CopyTo(null, 0));
    }

    [Test]
    public void CopyToWithNegativeIndexThrowsException()
    {
        ICollection deque = new Deque<Int32>(new[] {1, 2, 3});
        var         array = new Int32[1];

        Assert.Throws<ArgumentOutOfRangeException>(() => deque.CopyTo(array, -1));
        Assert.True(array.AllDefault());
    }

    [Test]
    public void CopyToWithIndexEqualToArrayLengthThrowsException()
    {
        ICollection deque = new Deque<Int32>(new[] {1, 2, 3});
        var         array = new Int32[1];

        Assert.Throws<ArgumentOutOfRangeException>(() => deque.CopyTo(array, 1));
        Assert.True(array.AllDefault());
    }

    [Test]
    public void CopyToWithIndexGreaterThanArrayLengthThrowsException()
    {
        ICollection deque = new Deque<Int32>(new[] {1, 2, 3});
        var         array = new Int32[1];

        Assert.Throws<ArgumentOutOfRangeException>(() => deque.CopyTo(array, 2));
        Assert.True(array.AllDefault());
    }

    [Test]
    public void CopyToThrowsExceptionIfArrayIsNotLongEnough()
    {
        ICollection deque  = new Deque<Int32>(new[] {1, 2, 3});
        var         array1 = new Int32[2];
        var         array2 = new Int32[3];

        Assert.Throws<ArgumentException>(() => deque.CopyTo(array1, 0));
        Assert.Throws<ArgumentException>(() => deque.CopyTo(array2, 1));

        Assert.True(array1.AllDefault());
        Assert.True(array2.AllDefault());
    }

    [Test]
    public void CopyToCopiesDequesContent()
    {
        ICollection deque = new Deque<String>(new[] {"1", "2", "3"});
        var         array = new String[5];
        deque.CopyTo(array, 1);

        Assert.Null(array[0]);
        Assert.Null(array[4]);
        Assert.AreEqual(array.Skip(1).Take(3), deque.Cast<String>());
    }

    [Test]
    public void CopyToCopiesDequesContentWhenDequeLoopsAround()
    {
        var deque = new Deque<String>(new[] {"1", "2", "3"});
        deque.PopRight();
        deque.PushLeft("0");

        var array = new String[5];
        ((ICollection) deque).CopyTo(array, 1);

        Assert.Null(array[0]);
        Assert.Null(array[4]);
        Assert.AreEqual(array.Skip(1).Take(3), deque);
    }

    [Test]
    public void IsSynchronizedReturnsFalse()
    {
        ICollection deque = new Deque<Int32>();
        Assert.False(deque.IsSynchronized);
    }

    [Test]
    public void SyncRootReturnsObject()
    {
        ICollection deque = new Deque<Int32>();
        Assert.NotNull(deque.SyncRoot);
    }
}
}
