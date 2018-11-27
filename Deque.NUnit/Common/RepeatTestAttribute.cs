using System;
using System.Collections.Generic;

using NUnit.Framework;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;

namespace Deque.NUnit.Common
{
/// <summary>
/// Marks a test method to be executed a given number of times.
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
public class RepeatTestAttribute : TestAttribute, ITestBuilder
{
    readonly int _count;

    /// <summary>
    /// Marks a test method to be executed a given number of times.
    /// </summary>
    /// <param name="count">The number of times to repeat the test.</param>
    public RepeatTestAttribute(int count) { _count = count; }

    public IEnumerable<TestMethod> BuildFrom(IMethodInfo method, Test suite)
    {
        var results = new List<TestMethod>();

        for (int i = 0; i < _count; i++) results.Add(new TestMethod(method));

        return results;
    }
}
}
