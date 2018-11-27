using System;
using System.Collections.Concurrent;

using NUnit.Framework;

namespace Deque.NUnit.ConcurrentDeque
{
public class ClearTests
{
    [Test]
    public void ClearRemovesAllItems()
    {
        //Arrange
        var deque = new ConcurrentDeque<Int32>(new[] {1, 2, 3, 4});

        //Act
        deque.Clear();

        //Assert
        Assert.True(deque.IsEmpty);
    }
}
}
