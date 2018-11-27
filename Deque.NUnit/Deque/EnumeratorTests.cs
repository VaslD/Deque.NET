using System;
using System.Collections.Generic;

using NUnit.Framework;

namespace Deque.NUnit.Deque
{
public class EnumeratorTests
{
    [Theory]
    public void MutationsInvalidateEnumerator(Action<Deque<Int32>> mutate)
    {
        var                deque      = new Deque<Int32>(new[] {1, 2, 3});
        IEnumerator<Int32> enumerator = deque.GetEnumerator();

        mutate(deque);

        Assert.Throws<InvalidOperationException>(() => enumerator.MoveNext());
        Assert.Throws<InvalidOperationException>(() => enumerator.Reset());
    }

    [Theory]
    public void ReturnsCorrectContent(Int32[] collection)
    {
        var deque = new Deque<Int32>(collection);
        Assert.AreEqual(collection, deque);
    }

    [Test]
    public void ResetRestartsEnumerator()
    {
        var deque = new Deque<Int32>(new[] {1, 2, 3});

        IEnumerator<Int32> enumerator = deque.GetEnumerator();
        enumerator.MoveNext();
        enumerator.MoveNext();

        //reset enumerator and move to the first element
        enumerator.Reset();
        enumerator.MoveNext();

        Assert.AreEqual(1, enumerator.Current);
    }

    [Test]
    public void ChangingRingBufferLayoutDoesNotInvalidateEnumerator()
    {
        //make deque "wrap around" the ring buffer
        var deque = new Deque<Int32>(new[] {1, 2, 3, 4, 5});
        deque.PopLeft();
        deque.PopLeft();
        deque.PushRight(6);

        //get enumerator and move to the first element
        IEnumerator<Int32> enumerator = deque.GetEnumerator();
        enumerator.MoveNext();

        //set expectations
        IEnumerable<Int32> shadowSequence   = new[] {3, 4, 5, 6};
        IEnumerator<Int32> shadowEnumerator = shadowSequence.GetEnumerator();
        shadowEnumerator.MoveNext();

        //change ring buffer layout
        deque.TrimExcess();

        //Assert
        while (shadowEnumerator.MoveNext())
        {
            Assert.True(enumerator.MoveNext());
            Assert.AreEqual(shadowEnumerator.Current, enumerator.Current);
        }

        Assert.False(enumerator.MoveNext());
    }

    [Datapoints]
    public static IEnumerable<Action<Deque<Int32>>[]> Mutations
        => new[]
           {
               new[] {(Action<Deque<Int32>>) (deque => deque.PushRight(4))},
               new[] {(Action<Deque<Int32>>) (deque => deque.PushLeft(4))},
               new[] {(Action<Deque<Int32>>) (deque => deque.PopRight())},
               new[] {(Action<Deque<Int32>>) (deque => deque.PopLeft())},
               new[] {(Action<Deque<Int32>>) (deque => deque.Clear())},
               new[] {(Action<Deque<Int32>>) (deque => ((ICollection<Int32>) deque).Add(4))},
               new[] {(Action<Deque<Int32>>) (deque => ((ICollection<Int32>) deque).Remove(2))}
           };

    [Datapoints]
    public static IEnumerable<Int32[]> Items
        => new[]
           {
               new Int32[] { },
               new[] {1},
               new[] {1, 2, 3},
               new[] {1, 2, 3, 4}
           };
}
}
