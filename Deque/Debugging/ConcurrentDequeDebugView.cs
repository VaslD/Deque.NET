using System.Diagnostics;

namespace System.Collections.Concurrent
{
/// <summary>
/// A debugger view of the <see cref="ConcurrentDeque{T}"/> that makes it simple to browse the
/// collection's contents at a point in time.
/// </summary>
/// <typeparam name="T">The type of elements stored within.</typeparam>
internal sealed class ConcurrentDequeDebugView<T>
{
    private readonly ConcurrentDeque<T> _deque;

    /// <summary>
    /// Returns a snapshot of the underlying collection's elements.
    /// </summary>
    [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
    public T[] Items => _deque.ToArray();

    /// <summary>
    /// Constructs a new debugger view object for the provided collection object.
    /// </summary>
    /// <param name="deque">A collection to browse in the debugger.</param>
    public ConcurrentDequeDebugView(ConcurrentDeque<T> deque)
        => _deque = deque ?? throw new ArgumentNullException(nameof(deque));
}
}
