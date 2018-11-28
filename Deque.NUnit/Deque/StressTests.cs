using System;
using System.Collections.Generic;
using System.Linq;

using NUnit.Framework;

namespace Deque.NUnit.Deque
{
    public class StressTests
    {
        private List<Int32>  _shadow;
        private Deque<Int32> _deque;
        private Int32        _previousCapacity;

        private enum Op
        {
            PopRight,
            PopLeft,
            PushRight,
            PushLeft,
            Remove,
            Add,
            SetIndex
        }

        [Test]
        public void StressTest()
        {
            _shadow = new List<Int32>(new[] {1, 2, 3, 4});
            _deque  = new Deque<Int32>(new[] {1, 2, 3, 4});

            var         generator       = new RandomOpGenerator();
            var         rnd             = new Random();
            const Int32 operationsCount = 100000;

            for (var i = 0; i < operationsCount; i++)
            {
                //mutate deque
                if (i % 300 == 0)
                {
                    //clear every 300 mutations
                    _deque.Clear();
                    _shadow.Clear();
                }
                else if (i % 200 == 0)
                {
                    //trim excess every 300 mutations
                    _deque.TrimExcess();
                    _shadow.TrimExcess();
                    Assert.AreEqual(_deque.Count, _deque.Capacity);
                }
                else
                {
                    //draw a random operation
                    var op  = generator.Pick();
                    var val = rnd.Next(1000);

                    switch (op)
                    {
                    case Op.PushRight:
                    {
                        val = rnd.Next(1000);
                        _deque.PushRight(val);
                        _shadow.Add(val);
                        break;
                    }
                    case Op.PushLeft:
                    {
                        val = rnd.Next(1000);
                        _deque.PushLeft(val);
                        _shadow.Insert(0, val);
                        break;
                    }
                    case Op.Add:
                    {
                        break;

                        val = rnd.Next(1000);
                        (_deque as ICollection<Int32>).Add(val);
                        _shadow.Add(val);
                        break;
                    }
                    case Op.SetIndex:
                    {
                        break;

                        /*
                        if (_deque.Count == 0) goto case Op.Add;
    
                        var randomIndex = rnd.Next(_shadow.Count);
                        val                  = rnd.Next(1000);
                        _deque[randomIndex]  = val;
                        _shadow[randomIndex] = val;
                        break;
                        */
                    }
                    case Op.PopLeft:
                    {
                        if (_deque.Count == 0) goto case Op.PushLeft;
                        Assert.DoesNotThrow(() => val = _deque.PopLeft());
                        Assert.AreEqual(_shadow.First(), val);
                        _shadow.RemoveAt(0);
                        break;
                    }
                    case Op.PopRight:
                    {
                        if (_deque.Count == 0) goto case Op.PushRight;

                        Assert.DoesNotThrow(() => val = _deque.PopRight());
                        Assert.AreEqual(_shadow.Last(), val);
                        _shadow.RemoveAt(_shadow.Count - 1);
                        break;
                    }
                    case Op.Remove:
                    {
                        break;

                        if (_deque.Count == 0) goto case Op.Add;

                        //draw a random item
                        var index = rnd.Next(_shadow.Count);
                        var item  = _shadow[index];
                        Assert.True((_deque as ICollection<Int32>).Remove(item));
                        _shadow.Remove(item);
                        break;
                    }
                    }
                }

                VerifyEmpty();
                VerifyCount();
                VerifySequence();
                VerifyEnds();
                VerifyCapacity();
                VerifyIndexer();
            }
        }

        private void VerifyIndexer()
        {
            for (var i = 0; i < _shadow.Count; i++) Assert.AreEqual(_shadow[i], _deque[i]);
        }

        private void VerifyCapacity()
        {
            //assert that the capacity is never raised to more than double the number of elements in the deque
            if (_deque.Capacity != _previousCapacity)
            {
                Assert.True(_deque.Capacity <= 4 || _deque.Capacity <= 2 * _deque.Count);
                _previousCapacity = _deque.Capacity;
            }
        }

        private void VerifyEmpty()
        {
            if (_shadow.Count == 0) Assert.True(_deque.Count == 0);
        }

        private void VerifyCount() { Assert.AreEqual(_shadow.Count, _deque.Count); }

        private void VerifySequence() { Assert.AreEqual(_shadow, _deque); }

        private void VerifyEnds()
        {
            if (_shadow.Count != 0)
            {
                Assert.AreEqual(_shadow.First(), _deque.PeekLeft());
                Assert.AreEqual(_shadow.Last(),  _deque.PeekRight());
            }
            else
            {
                Assert.Throws<InvalidOperationException>(() => _deque.PeekLeft());
                Assert.Throws<InvalidOperationException>(() => _deque.PeekRight());
                Assert.Throws<InvalidOperationException>(() => _deque.PopLeft());
                Assert.Throws<InvalidOperationException>(() => _deque.PopRight());
            }
        }

        private class RandomOpGenerator
        {
            private readonly List<Op> _distribution;
            private readonly Random   _rnd;

            public RandomOpGenerator()
            {
                //dictionary of <operation, weight> pairs
                //slightly biased towards "push" operations
                var choices = new SortedDictionary<Op, Int32>
                              {
                                  {Op.PushRight, 2},
                                  {Op.PushLeft, 2},
                                  {Op.Add, 1},
                                  {Op.SetIndex, 1},

                                  {Op.PopRight, 1},
                                  {Op.PopLeft, 1},
                                  {Op.Remove, 1}
                              };

                _distribution = choices
                               .SelectMany(kv =>
                                               Enumerable.Repeat(kv.Key, kv.Value))
                               .ToList();

                _rnd = new Random();
            }

            public Op Pick()
            {
                var index = _rnd.Next(_distribution.Count);
                return _distribution[index];
            }
        }
    }
}
