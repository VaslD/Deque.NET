using System;
using System.Collections.Generic;

using NUnit.Framework;

namespace Deque.NUnit.Deque
{
    public class ConstructorTests
    {
        [Test]
        public void WithNegativeCapacityThrowsException()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => _ = new Deque<Int32>(-1));
        }

        [Test]
        public void WithNullIEnumerableThrowsException()
        {
            Assert.Throws<ArgumentNullException>(() => _ = new Deque<Int32>(null));
        }

        [Test]
        public void WithZeroCapacityHasZeroCapacity()
        {
            var deque = new Deque<Int32>(0);
            Assert.AreEqual(0, deque.Capacity);
        }

        [Test]
        public void WithCapacityHasSpecifiedCapacity()
        {
            var deque = new Deque<Int32>(5);
            Assert.AreEqual(5, deque.Capacity);
        }

        [Test]
        public void WithICollectionCopiesCollection()
        {
            var array = new[] {1, 2, 3};
            var deque = new Deque<Int32>(array);

            Assert.AreEqual(array, deque);
            Assert.AreEqual(array.Length, deque.Capacity);
        }

        [Test]
        public void WithIEnumerableCopiesCollection()
        {
            var queue = new Queue<Int32>(new[] {1, 2, 3});
            var deque = new Deque<Int32>(queue);

            Assert.AreEqual(queue, deque);
            Assert.AreEqual(queue.Count, deque.Capacity);
        }

        [Test]
        public void HasZeroCapacityByDefault()
        {
            var deque = new Deque<Int32>();
            Assert.AreEqual(0, deque.Capacity);
        }
    }
}
