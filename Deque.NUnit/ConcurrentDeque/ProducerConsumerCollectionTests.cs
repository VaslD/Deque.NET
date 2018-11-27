using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;

using NUnit.Framework;

namespace Deque.NUnit.ConcurrentDeque
{
public class ProducerConsumerCollectionTests
{
    [Test]
    public void TryAddAppendsValueToTheRight()
    {
        //Act
        IProducerConsumerCollection<Int32> deque = new ConcurrentDeque<Int32>();
        deque.TryAdd(2);
        deque.TryAdd(3);

        //Assert
        Assert.AreEqual(new[] {2, 3}, deque);
    }

    [Test]
    public void TryTakeTakesValueFromTheLeft()
    {
        //Arrange
        IProducerConsumerCollection<Int32> deque = new ConcurrentDeque<Int32>(new[] {1, 2});

        //Act
        Int32 item;
        deque.TryTake(out item);

        //Assert
        Assert.AreEqual(1,         item);
        Assert.AreEqual(new[] {2}, deque);
    }

    [Test]
    public void ToArrayReturnsSnapshot()
    {
        //Arrange
        Int32[] array = {0, 1, 2};
        var     deque = new ConcurrentDeque<Int32>(array);

        //Act
        var snapshot = deque.ToArray();

        //Assert
        Assert.AreEqual(array, snapshot);
    }

    [Test]
    public void CopyToCopiesItems()
    {
        //Arrange
        Int32[] array = {0, 1, 2};
        var     deque = new ConcurrentDeque<Int32>(array);

        //Act
        var copy = new Int32[3];
        deque.CopyTo(copy, 0);

        //Assert
        Assert.AreEqual(array, copy);
    }

    [Test]
    public void CollectionCopyToCopiesItems()
    {
        //Arrange
        Int32[] array = {0, 1, 2};
        var     deque = new ConcurrentDeque<Int32>(array);

        //Act
        var copy = new Int32[3];
        ((ICollection) deque).CopyTo(copy, 0);

        //Assert
        Assert.AreEqual(array, copy);
    }

    [Test]
    public void SyncRootIsNotSupported()
    {
        IProducerConsumerCollection<Int32> deque = new ConcurrentDeque<Int32>();
        Assert.Throws<NotSupportedException>(() => _ = deque.SyncRoot);
    }

    [Test]
    public void IsSynchronizedReturnsFalse()
    {
        IProducerConsumerCollection<Int32> deque = new ConcurrentDeque<Int32>();
        Assert.False(deque.IsSynchronized);
    }

    [Theory]
    public void CountReturnsTheNumberOfItemsInTheDeque(Int32[] items)
    {
        var deque = new ConcurrentDeque<Int32>(items);

        Assert.AreEqual(items.Length, deque.Count);
    }

    [Test]
    public void EnumeratorDoesNotIncludeConcurrentModifications()
    {
        //Arrange
        var   arr   = new[] {1, 2, 3};
        var   deque = new ConcurrentDeque<Int32>(arr);
        Int32 item;

        //Act
        var iterator = deque.GetEnumerator();
        iterator.MoveNext();

        deque.TryPopLeft(out item);
        deque.TryPopLeft(out item);
        deque.PushLeft(6);

        deque.TryPopRight(out item);
        deque.PushRight(6);

        //Assert
        Assert.AreEqual(1, iterator.Current);

        Assert.True(iterator.MoveNext());
        Assert.AreEqual(2, iterator.Current);

        Assert.True(iterator.MoveNext());
        Assert.AreEqual(3, iterator.Current);

        Assert.False(iterator.MoveNext());
    }

    [Datapoints]
    public static IEnumerable<Object[]> Items
        => new[]
           {
               new Object[] {new Int32[] { }},
               new Object[] {new[] {1}},
               new Object[] {new[] {1, 1}}
           };
}
}
