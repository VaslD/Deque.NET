using System;
using System.Collections.Generic;
using System.Linq;

using Deque.NUnit.Helpers;

using NUnit.Framework;

namespace Deque.NUnit.Deque
{
public class GenericCollectionTests
{
    [Test]
    public void AddAppendsItemToTheRightEnd()
    {
        ICollection<Int32> deque = new Deque<Int32>(new[] {1, 2, 3});
        deque.Add(4);

        Assert.AreEqual(4,                  deque.Count);
        Assert.AreEqual(new[] {1, 2, 3, 4}, deque as Deque<Int32>);
    }

    [Test]
    public void ContainsReturnsTrueIfDequeHasItem()
    {
        var deque = new Deque<Int32>(new[] {1, 2, 3});

        Assert.True(deque.Contains(2));
    }

    [Test]
    public void ContainsReturnsFalseIfDequeDoesntHaveItem()
    {
        var deque = new Deque<Int32>(new[] {1, 2, 3});

        Assert.False(deque.Contains(4));
    }

    [Test]
    public void ClearResetsDeque()
    {
        var deque = new Deque<Int32>(new[] {1, 2, 3});
        deque.Clear();

        Assert.AreEqual(0,            deque.Count);
        Assert.AreEqual(new Int32[0], deque);
    }

    [Test]
    public void ClearKeepsCapacity()
    {
        var deque = new Deque<Int32>(6);
        deque.Clear();

        Assert.AreEqual(6, deque.Capacity);
    }

#if !DEBUG
    [Test]
    public void ClearPurgesReferences()
    {
        var obj1 = new Object();
        var obj2 = new Object();

        var ref1 = new WeakReference(obj1);
        var ref2 = new WeakReference(obj2);

        var deque = new Deque<Object>(new[] {obj1, obj2});

        deque.Clear();

        //assert that all strong references to the two objects have been cleaned
        GC.Collect();
        Assert.False(ref1.IsAlive);
        Assert.False(ref2.IsAlive);

        /*
         * Make sure the GC doesn't clean the deque and all its references before this.
         * If it did, the above assertions could be "false positives" - the references could have been collected
         * not because Deque<T>.Clear worked, but because the deque itself was collected.
         */
        GC.KeepAlive(deque);
    }
#endif

#if !DEBUG
    [Test]
    public void ClearPurgesReferencesWhenDequeLoopsAround()
    {
        var obj1 = new Object();
        var obj2 = new Object();
        var obj3 = new Object();
        var obj4 = new Object();

        var ref1 = new WeakReference(obj1);
        var ref2 = new WeakReference(obj2);
        var ref3 = new WeakReference(obj3);
        var ref4 = new WeakReference(obj4);

        var deque = new Deque<Object>(new[] {obj1, obj2, obj3});
        deque.PopLeft();
        deque.PushRight(obj4);

        deque.Clear();

        //assert that all strong references to the two objects have been cleaned
        GC.Collect();
        Assert.False(ref1.IsAlive);
        Assert.False(ref2.IsAlive);
        Assert.False(ref3.IsAlive);
        Assert.False(ref4.IsAlive);

        //Make sure the GC doesn't clean the deque and all its references before this.
        GC.KeepAlive(deque);
    }
#endif

    [Test]
    public void CopyToWithNullArrayThrowsException()
    {
        var deque = new Deque<Int32>(new[] {1, 2, 3});
        Assert.Throws<ArgumentNullException>(() => deque.CopyTo(null, 0));
    }

    [Test]
    public void CopyToWithNegativeIndexThrowsException()
    {
        var deque = new Deque<Int32>(new[] {1, 2, 3});
        var array = new Int32[1];

        Assert.Throws<ArgumentOutOfRangeException>(() => deque.CopyTo(array, -1));
        Assert.True(array.AllDefault());
    }

    [Test]
    public void CopyToWithIndexEqualToArrayLengthThrowsException()
    {
        var deque = new Deque<Int32>(new[] {1, 2, 3});
        var array = new Int32[1];

        Assert.Throws<ArgumentOutOfRangeException>(() => deque.CopyTo(array, 1));
        Assert.True(array.AllDefault());
    }

    [Test]
    public void CopyToWithIndexGreaterThanArrayLengthThrowsException()
    {
        var deque = new Deque<Int32>(new[] {1, 2, 3});
        var array = new Int32[1];

        Assert.Throws<ArgumentOutOfRangeException>(() => deque.CopyTo(array, 2));
        Assert.True(array.AllDefault());
    }

    [Test]
    public void CopyToThrowsExceptionIfArrayIsntLongEnough()
    {
        var deque  = new Deque<Int32>(new[] {1, 2, 3});
        var array1 = new Int32[2];
        var array2 = new Int32[3];

        Assert.Throws<ArgumentException>(() => deque.CopyTo(array1, 0));
        Assert.Throws<ArgumentException>(() => deque.CopyTo(array2, 1));

        Assert.True(array1.AllDefault());
        Assert.True(array2.AllDefault());
    }

    [Test]
    public void CopyToCopiesDequesContent()
    {
        var deque = new Deque<String>(new[] {"1", "2", "3"});
        var array = new String[5];
        deque.CopyTo(array, 1);

        Assert.Null(array[0]);
        Assert.Null(array[4]);
        Assert.AreEqual(array.Skip(1).Take(3), deque);
    }

    [Test]
    public void CopyToCopiesDequesContentWhenDequeLoopsAround()
    {
        var deque = new Deque<String>(new[] {"1", "2", "3"});
        deque.PopRight();
        deque.PushLeft("0");

        var array = new String[5];
        deque.CopyTo(array, 1);

        Assert.Null(array[0]);
        Assert.Null(array[4]);
        Assert.AreEqual(array.Skip(1).Take(3), deque);
    }

    [Test]
    public void RemoveUnknownItemThrowsException()
    {
        ICollection<Int32> deque = new Deque<Int32>(new[] {2, 3, 4});
        Assert.False(deque.Remove(5));
    }

    [Test]
    public void RemoveLeftmostItemRemovesItem()
    {
        ICollection<Int32> deque = new Deque<Int32>(new[] {2, 3, 4});

        Assert.True(deque.Remove(2));
        Assert.AreEqual(new[] {3, 4}, deque as IEnumerable<Int32>);
    }

    [Test]
    public void RemoveRightmostItemRemovesItem()
    {
        ICollection<Int32> deque = new Deque<Int32>(new[] {2, 3, 4});

        Assert.True(deque.Remove(4));
        Assert.AreEqual(new[] {2, 3}, deque as IEnumerable<Int32>);
    }

    [Test]
    public void RemoveItemTowardsTheLeftEndRemovesItem()
    {
        ICollection<Int32> deque = new Deque<Int32>(new[] {2, 3, 4, 5, 6, 7});

        Assert.True(deque.Remove(4));
        Assert.AreEqual(new[] {2, 3, 5, 6, 7}, deque as IEnumerable<Int32>);
    }

    [Test]
    public void RemoveItemTowardsTheLeftEndRemovesItemWhenDequeLoopsAround()
    {
        var deque = new Deque<Int32>(new[] {2, 3, 4, 5, 6, 7});
        deque.PopRight();
        deque.PushLeft(1);

        Assert.True((deque as ICollection<Int32>).Remove(3));
        Assert.AreEqual(new[] {1, 2, 4, 5, 6}, deque);
    }

    [Test]
    public void RemoveItemTowardsTheRightEndRemovesItem()
    {
        ICollection<Int32> deque = new Deque<Int32>(new[] {2, 3, 4, 5, 6, 7});

        Assert.True(deque.Remove(5));
        Assert.AreEqual(new[] {2, 3, 4, 6, 7}, deque as IEnumerable<Int32>);
    }

    [Test]
    public void RemoveItemTowardsTheRightEndRemovesItemWhenDequeLoopsAround()
    {
        var deque = new Deque<Int32>(new[] {2, 3, 4, 5, 6, 7});
        deque.PopLeft();
        deque.PushRight(8);

        Assert.True((deque as ICollection<Int32>).Remove(6));
        Assert.AreEqual(new[] {3, 4, 5, 7, 8}, deque);
    }
}
}
