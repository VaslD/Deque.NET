using System;
using System.Collections.Generic;

using NUnit.Framework;

namespace Deque.NUnit.Deque
{
    public class PopTests
    {
        [Test]
        public void PopLeftThrowsExceptionWhenEmpty()
        {
            var deque = new Deque<Int32>();
            Assert.Throws<InvalidOperationException>(() => deque.PopLeft());
        }

        [Test]
        public void PopLeftReturnsLeftmostItem()
        {
            var deque = new Deque<Int32>(new[] {1, 2, 3});
            var item = deque.PopLeft();

            Assert.AreEqual(1, item);
        }

        [Test]
        public void PopLeftDecreasesCount()
        {
            var deque = new Deque<Int32>(new[] { 1, 2, 3 });
            deque.PopLeft();

            Assert.AreEqual(2, deque.Count);
        }

#if !DEBUG 
        [Test]
        public void PopLeftClearsReference()
        {
            var obj1 = new Object();
            var ref1 = new WeakReference(obj1);

            var deque = new Deque<Object>(new[] {obj1, new Object(), new Object()});
            deque.PopLeft();

            //assert that all strong references to the object have been cleaned
            GC.Collect();
            Assert.False(ref1.IsAlive);

            //Make sure the GC doesn't clean the deque and all its references before this.
            GC.KeepAlive(deque);
        }
#endif

        [Test]
        public void PopRightThrowsExceptionWhenEmpty()
        {
            var deque = new Deque<Int32>();
            Assert.Throws<InvalidOperationException>(() => deque.PopRight());
        }

        [Test]
        public void PopRightReturnsRightmostItem()
        {
            var deque = new Deque<Int32>(new[] { 1, 2, 3 });
            var item = deque.PopRight();

            Assert.AreEqual(3, item);
        }

        [Test]
        public void PopRightDecreasesCount()
        {
            var deque = new Deque<Int32>(new[] { 1, 2, 3 });
            deque.PopRight();

            Assert.AreEqual(2, deque.Count);
        }

#if !DEBUG 
        [Test]
        public void PopRightClearsReference()
        {
            var obj1 = new Object();
            var ref1 = new WeakReference(obj1);

            var deque = new Deque<Object>(new[] {new Object(), new Object(), obj1});
            deque.PopRight();

            //assert that all strong references to the object have been cleaned
            GC.Collect();
            Assert.False(ref1.IsAlive);

            //Make sure the GC doesn't clean the deque and all its references before this.
            GC.KeepAlive(deque);
        }
#endif
    }
}
