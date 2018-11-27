using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;

using Deque.NUnit.Common;
using Deque.NUnit.Helpers;

using NUnit.Framework;

namespace Deque.NUnit.ConcurrentDeque.Internal
{
public class StressTests
{
    private const Int32 ThreadCount = 20;
    private const Int32 RunningTime = 3000;

    [Test]
    [Property("Category", "LongRunning")]
    public void PushRightIsAtomic()
    {
        //Arrange
        Int64   pushCount = 0;
        Int64   sum       = 0;
        Boolean cancelled = false;

        var deque = new ConcurrentDeque<Int32>();

        //keep adding items to the deque
        Action pushRight = () =>
                           {
                               Random rnd = new Random();
                               while (!cancelled)
                               {
                                   Int32 val = rnd.Next(1, 11);
                                   deque.PushRight(val);
                                   Interlocked.Increment(ref pushCount);
                                   Interlocked.Add(ref sum, val);
                               }
                           };

        //Act
        pushRight.RunInParallel(() => cancelled = true, ThreadCount, RunningTime);

        //Assert
        VerifyState(deque, pushCount, sum);
    }

    [Test]
    [Property("Category", "LongRunning")]
    public void PushLeftIsAtomic()
    {
        //Arrange
        Int64   pushCount = 0;
        Int64   sum       = 0;
        Boolean cancelled = false;

        var deque = new ConcurrentDeque<Int32>();

        //keep adding items to the deque
        Action pushLeft = () =>
                          {
                              Random rnd = new Random();
                              while (!cancelled)
                              {
                                  Int32 val = rnd.Next(1, 11);
                                  deque.PushLeft(val);
                                  Interlocked.Increment(ref pushCount);
                                  Interlocked.Add(ref sum, val);
                              }
                          };

        //Act
        pushLeft.RunInParallel(() => cancelled = true, ThreadCount, RunningTime);

        //Assert
        VerifyState(deque, pushCount, sum);
    }

    [Test]
    [Property("Category", "LongRunning")]
    public void TryPopRightIsAtomic()
    {
        //Arrange
        const Int32  initialCount = 5000000;
        const Double stopAt       = initialCount * 0.9;

        Int32 popCount = 0;
        var   deque    = new ConcurrentDeque<Int32>();

        for (Int32 i = 0; i < initialCount; i++) deque.PushRight(i);

        Action popRight = () =>
                          {
                              while (popCount <= stopAt)
                              {
                                  Int32 i;
                                  Assert.True(deque.TryPopRight(out i));
                                  Interlocked.Increment(ref popCount);
                              }
                          };
        //Act
        popRight.RunInParallel(ThreadCount);

        //Assert
        var   expectedCount = initialCount - popCount;
        Int64 expectedSum   = Enumerable.Range(0, expectedCount).LongSum();

        VerifyState(deque, expectedCount, expectedSum);
    }

    [Test]
    [Property("Category", "LongRunning")]
    public void TryPopLeftIsAtomic()
    {
        //Arrange
        const Int32  initialCount = 5000000;
        const Double stopAt       = initialCount * 0.9;

        Int32 popCount = 0;
        var   deque    = new ConcurrentDeque<Int32>();

        for (Int32 i = 0; i < initialCount; i++) deque.PushLeft(i);

        Action popLeft = () =>
                         {
                             while (popCount <= stopAt)
                             {
                                 Int32 i;
                                 Assert.True(deque.TryPopLeft(out i));
                                 Interlocked.Increment(ref popCount);
                             }
                         };

        //Act
        popLeft.RunInParallel(ThreadCount);

        //Assert
        var   expectedCount = initialCount - popCount;
        Int64 expectedSum   = Enumerable.Range(0, expectedCount).LongSum();

        VerifyState(deque, expectedCount, expectedSum);
    }

    /// <summary>
    /// Verifies that parallel interleaved push right/pop right operations don't leave the deque in a corrupted state
    /// </summary>
    [Test]
    [Property("Category", "LongRunning")]
    public void InterleavedPushPopRightOps()
    {
        //Arrange
        var     deque     = new ConcurrentDeque<Int32>();
        Int64   sum       = 0;
        Int64   nodeCount = 0;
        Boolean cancelled = false;

        Action action = () =>
                        {
                            Random rnd = new Random();

                            while (!cancelled)
                            {
                                //slightly biased towards "push"
                                if (rnd.NextDouble() >= 0.45)
                                {
                                    //push
                                    var val = rnd.Next(1, 51);
                                    deque.PushRight(val);
                                    Interlocked.Increment(ref nodeCount);
                                    Interlocked.Add(ref sum, val);
                                }
                                else
                                {
                                    //pop
                                    Int32 val;
                                    if (deque.TryPopRight(out val))
                                    {
                                        Interlocked.Decrement(ref nodeCount);
                                        Interlocked.Add(ref sum, -val);
                                    }
                                }
                            }
                        };

        //Act
        action.RunInParallel(() => cancelled = true, ThreadCount, RunningTime);

        //Assert
        VerifyState(deque, nodeCount, sum);
    }

    /// <summary>
    /// Verifies that parallel interleaved push right/pop left operations don't leave the deque in a corrupted state
    /// </summary>
    [Test]
    [Property("Category", "LongRunning")]
    public void InterleavedPushPopLeftOps()
    {
        //Arrange
        var     deque     = new ConcurrentDeque<Int32>();
        Int64   sum       = 0;
        Int64   nodeCount = 0;
        Boolean cancelled = false;

        Action action = () =>
                        {
                            Random rnd = new Random();

                            while (!cancelled)
                            {
                                //slightly biased towards "push"
                                if (rnd.NextDouble() >= 0.45)
                                {
                                    //push
                                    var val = rnd.Next(1, 51);
                                    deque.PushLeft(val);
                                    Interlocked.Increment(ref nodeCount);
                                    Interlocked.Add(ref sum, val);
                                }
                                else
                                {
                                    //pop
                                    Int32 val;
                                    if (deque.TryPopLeft(out val))
                                    {
                                        Interlocked.Decrement(ref nodeCount);
                                        Interlocked.Add(ref sum, -val);
                                    }
                                }
                            }
                        };

        //Act
        action.RunInParallel(() => cancelled = true, ThreadCount, RunningTime);

        //Assert
        VerifyState(deque, nodeCount, sum);
    }

    /// <summary>
    /// Verifies that parallel interleaved push right/pop left/right operations don't leave the deque in a corrupted state.
    /// The test spins up 20 threads, each executing the same action.
    /// The threads start by pushing random integers on both ends of the deque until the total sum of all nodes reaches 5000.
    /// Then, they start popping items until the deque is empty, at which point they start pushing again, and so on, for 3 seconds.
    /// 
    /// At the end, we assert that we can still traverse the deque (from left to right, and right to left) and that the nodes contain the excepted values.
    /// </summary>
    [Test]
    [Property("Category", "LongRunning")]
    public void InterleavedOps()
    {
        //Arrange
        var     deque      = new ConcurrentDeque<Int32>();
        Int64   sum        = 0;
        Int64   nodeCount  = 0;
        Boolean cancelled  = false;
        Boolean shouldPush = true;

        Action action = () =>
                        {
                            Random rnd = new Random();

                            while (!cancelled)
                            {
                                if (shouldPush)
                                {
                                    //push to either end
                                    var val = rnd.Next(1, 51);
                                    if (rnd.NextDouble() > 0.50)
                                        deque.PushLeft(val);
                                    else
                                        deque.PushRight(val);
                                    Interlocked.Increment(ref nodeCount);
                                    Interlocked.Add(ref sum, val);

                                    //start popping
                                    if (nodeCount >= 10000) shouldPush = false;
                                }
                                else
                                {
                                    //pop from either end
                                    Int32 val;
                                    if (rnd.NextDouble() > 0.50)
                                    {
                                        if (deque.TryPopLeft(out val))
                                        {
                                            Interlocked.Decrement(ref nodeCount);
                                            Interlocked.Add(ref sum, -val);
                                        }
                                    }
                                    else
                                    {
                                        if (deque.TryPopRight(out val))
                                        {
                                            Interlocked.Decrement(ref nodeCount);
                                            Interlocked.Add(ref sum, -val);
                                        }
                                    }

                                    //start pushing
                                    if (nodeCount == 0) shouldPush = true;
                                }
                            }
                        };

        //Act
        action.RunInParallel(() => cancelled = true, ThreadCount, RunningTime);

        //Assert
        VerifyState(deque, nodeCount, sum);
    }

    private void VerifyState(ConcurrentDeque<Int32> deque, Int64 expectedCount, Int64 expectedSum)
    {
        //Assert
        Assert.AreEqual(expectedCount, deque.Count);
        Assert.AreEqual(expectedSum,   deque.LongSum());

        //test internal state
        //traverse the deque in both directions
        Assert.AreEqual(expectedCount, deque.TraverseLeftRight().LongCount());
        Assert.AreEqual(expectedCount, deque.TraverseRightLeft().LongCount());
    }
}
}
