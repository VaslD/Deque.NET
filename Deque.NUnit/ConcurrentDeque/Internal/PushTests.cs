using System;
using System.Collections.Concurrent;

using NUnit.Framework;

namespace Deque.NUnit.ConcurrentDeque.Internal
{
public class PushTests
{
    [Test]
    public void PushRightAppendsNodeToEmptyDeque()
    {
        //Arrange
        const Int32 value = 5;
        var         deque = new ConcurrentDeque<Int32>();

        //Act
        deque.PushRight(value);

        //Assert
        var anchor = deque._anchor;
        Assert.NotNull(anchor._right);
        Assert.NotNull(anchor._left);
        Assert.AreSame(anchor._left, anchor._right);
        Assert.AreEqual(value, anchor._right._value);
    }

    [Test]
    public void PushRightAppendsNodeToNonEmptyList()
    {
        //Arrange
        const Int32 value = 5;
        var         deque = new ConcurrentDeque<Int32>();
        deque.PushRight(1);

        //Act
        deque.PushRight(value);

        //Assert
        var anchor        = deque._anchor;
        var rightmostNode = anchor._right;
        Assert.NotNull(rightmostNode);
        Assert.AreEqual(value, rightmostNode._value);
    }

    [Test]
    public void PushRightKeepsReferenceToPreviousRightNode()
    {
        //Arrange
        const Int32 prevValue = 1;
        const Int32 value     = 5;
        var         deque     = new ConcurrentDeque<Int32>();
        deque.PushRight(prevValue);

        //Act
        deque.PushRight(value);

        //Assert
        var anchor  = deque._anchor;
        var newNode = anchor._right;
        Assert.AreEqual(prevValue, newNode._left._value);
    }

    [Test]
    public void PushRightStabilizesDeque()
    {
        //Arrange
        const Int32 prevValue = 1;
        const Int32 value     = 5;
        var         deque     = new ConcurrentDeque<Int32>();
        deque.PushRight(prevValue);

        //Act
        deque.PushRight(value);

        //Assert
        var anchor  = deque._anchor;
        var newNode = anchor._right;
        Assert.AreSame(newNode, newNode._left._right);
        Assert.AreEqual(ConcurrentDeque<Int32>.DequeStatus.Stable, anchor._status);
    }

    [Test]
    public void PushLeftAppendsNodeToEmptyList()
    {
        //Arrange
        const Int32 value = 5;
        var         deque = new ConcurrentDeque<Int32>();

        //Act
        deque.PushLeft(value);

        //Assert
        var anchor = deque._anchor;
        Assert.NotNull(anchor._right);
        Assert.NotNull(anchor._left);
        Assert.AreSame(anchor._left, anchor._right);
        Assert.AreEqual(value, anchor._left._value);
    }

    [Test]
    public void PushLeftAppendsNodeToNonEmptyList()
    {
        //Arrange
        const Int32 value = 5;
        var         deque = new ConcurrentDeque<Int32>();
        deque.PushRight(1);

        //Act
        deque.PushLeft(value);

        //Assert
        var anchor       = deque._anchor;
        var leftmostNode = anchor._left;
        Assert.NotNull(leftmostNode);
        Assert.AreEqual(value, leftmostNode._value);
    }

    [Test]
    public void PushLeftKeepsReferenceToPreviousLeftNode()
    {
        //Arrange
        const Int32 prevValue = 1;
        const Int32 value     = 5;
        var         deque     = new ConcurrentDeque<Int32>();
        deque.PushLeft(prevValue);

        //Act
        deque.PushLeft(value);

        //Assert
        var anchor  = deque._anchor;
        var newNode = anchor._left;
        Assert.AreEqual(prevValue, newNode._right._value);
    }

    [Test]
    public void PushLeftStabilizesDeque()
    {
        //Arrange
        const Int32 prevValue = 1;
        const Int32 value     = 5;
        var         deque     = new ConcurrentDeque<Int32>();
        deque.PushLeft(prevValue);

        //Act
        deque.PushLeft(value);

        //Assert
        var anchor  = deque._anchor;
        var newNode = anchor._left;
        Assert.AreSame(newNode, newNode._right._left);
        Assert.AreEqual(ConcurrentDeque<Int32>.DequeStatus.Stable, anchor._status);
    }
}
}
