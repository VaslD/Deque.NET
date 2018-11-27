using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

using Deque.NUnit.Common;

using NUnit.Framework;

namespace Deque.NUnit.ConcurrentDeque
{
public class SnapshotAlgorithmTests
{
    private const Int32 ThreadCount = 10;

    private const Int32 PopLeft   = 0;
    private const Int32 PopRight  = 1;
    private const Int32 PushLeft  = 2;
    private const Int32 PushRight = 3;

    [Datapoints]
    public static IEnumerable<Object[]> MutationSteps
        => new[]
           {
               new Object[] {"No-op", new Int32[] { }},
               new Object[] {"PopLeft all nodes", new[] {PopLeft, PopLeft, PopLeft, PopLeft, PopLeft}},
               new Object[] {"PopRight all nodes", new[] {PopRight, PopRight, PopRight, PopRight, PopRight}},
               new Object[]
               {
                   "PopRight all nodes, PushRight once",
                   new[] {PopRight, PopRight, PopRight, PopRight, PopRight, PushRight}
               },
               new Object[] {"Pop all nodes", new[] {PopRight, PopRight, PopLeft, PopLeft, PopLeft}},
               new Object[] {"Pop all but one node", new[] {PopRight, PopRight, PopLeft, PopLeft}},
               new Object[] {"Pop once from both ends", new[] {PopRight, PopLeft}},
               new Object[] {"PopRight once, PushRight once", new[] {PopRight, PushRight}},
               new Object[] {"PopRight twice, PushRight once", new[] {PopRight, PopRight, PushRight}},
               new Object[]
               {
                   "PopRight, PushRight, PopLeft all x-y nodes",
                   new[] {PopRight, PushRight, PopLeft, PopLeft, PopLeft, PopLeft}
               },
               new Object[]
               {
                   "PopRight, PushRight, PopLeft all nodes",
                   new[] {PopRight, PushRight, PopLeft, PopLeft, PopLeft, PopLeft, PopLeft}
               },
               new Object[]
               {
                   "PopLeft, PushLeft, PopRight all x-y nodes",
                   new[] {PopLeft, PushLeft, PopRight, PopRight, PopRight, PopRight}
               },
               new Object[]
               {
                   "PopLeft, PushLeft, PopRight all nodes",
                   new[] {PopLeft, PushLeft, PopRight, PopRight, PopRight, PopRight, PopRight}
               },
               new Object[] {"Pop/Push right, Pop/Push left", new[] {PopRight, PushRight, PopLeft, PushLeft}}
           };

    [Theory]
    public void AlgorithmRecreatesOriginalXySequence(String msg, IEnumerable<Int32> ops)
    {
        Int32[] array = {0, 1, 2, 3, 4};
        var     deque = new ConcurrentDeque<Int32>(array);

        Action<ConcurrentDeque<Int32>> mutationCallback = d =>
                                                          {
                                                              foreach (var op in ops) ExecuteOp(deque, op);
                                                          };

        var snapshot = Execute(deque, mutationCallback);

        Assert.AreEqual(array, snapshot);
    }

    [RepeatTest(30)]
    [Property("Category", "LongRunning")]
    public void AlgorithmRecreatesOriginalXySequenceStressTest()
    {
        Int32[] array = {0, 1, 2, 3, 4};
        var     deque = new ConcurrentDeque<Int32>(array);

        Boolean  cancelled = false;
        Thread[] threads   = null;
        Action<ConcurrentDeque<Int32>> mutationCallback = d =>
                                                          {
                                                              Action executeOps = () =>
                                                                                  {
                                                                                      var rnd =
                                                                                          new Random(Thread
                                                                                                    .CurrentThread
                                                                                                    .ManagedThreadId);

                                                                                      //randomly mutate deque
                                                                                      while (!cancelled)
                                                                                          ExecuteOp(deque, rnd.Next(4));
                                                                                  };

                                                              threads = executeOps.StartInParallel(ThreadCount);

                                                              //yield the processor and let the threads run
                                                              Thread.Yield();
                                                          };

        //Act
        var snapshot = Execute(deque, mutationCallback);

        //stop threads
        cancelled = true;
        Assert.NotNull(threads);
        foreach (var thread in threads) thread.Join();

        //Assert
        Assert.AreEqual(array, snapshot);
    }

    private static List<T> Execute<T>(ConcurrentDeque<T> deque, Action<ConcurrentDeque<T>> mutationCallback)
    {
        //try to grab a reference to a stable anchor (fast route)
        ConcurrentDeque<T>.Anchor anchor = deque._anchor;

        //try to grab a reference to a stable anchor (slow route)
        if (anchor._status != ConcurrentDeque<T>.DequeStatus.Stable)
        {
            var spinner = new SpinWait();
            do
            {
                anchor = deque._anchor;
                spinner.SpinOnce();
            } while (anchor._status != ConcurrentDeque<T>.DequeStatus.Stable);
        }

        var x = anchor._left;
        var y = anchor._right;

        //run callback
        mutationCallback(deque);

        if (x == null) return new List<T>();

        if (x == y) return new List<T> {x._value};

        var xaPath  = new List<ConcurrentDeque<T>.Node>();
        var current = x;
        while (current != null && current != y)
        {
            xaPath.Add(current);
            current = current._right;
        }

        if (current == y)
        {
            xaPath.Add(current);
            return xaPath.Select(node => node._value).ToList();
        }

        current = y;
        var a      = xaPath.Last();
        var ycPath = new Stack<ConcurrentDeque<T>.Node>();
        while (current._left != null && current._left._right != current && current != a)
        {
            ycPath.Push(current);
            current = current._left;
        }

        var common = current;
        ycPath.Push(common);

        var xySequence = xaPath
                        .TakeWhile(node => node != common)
                        .Select(node => node._value)
                        .Concat(
                                ycPath.Select(node => node._value));

        return xySequence.ToList();
    }

    private static void ExecuteOp(ConcurrentDeque<Int32> deque, Int32 op)
    {
        Int32 item;
        switch (op)
        {
        case PopLeft:
            deque.TryPopLeft(out item);
            break;
        case PopRight:
            deque.TryPopRight(out item);
            break;
        case PushLeft:
            deque.PushLeft(10);
            break;
        case PushRight:
            deque.PushRight(10);
            break;
        default: throw new InvalidOperationException();
        }
    }
}
}
