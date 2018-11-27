namespace System.Collections.Concurrent
{
/// <summary>
/// Represents a thread-safe lock-free concurrent double-ended queue, also known as deque (pronounced "deck").
/// Items can be appended to/removed from both ends of the deque.
/// </summary>
/// <typeparam name="T">Specifies the type of the elements in the deque.</typeparam>
public interface IConcurrentDeque<T> : IProducerConsumerCollection<T>
{
    /// <summary>
    /// Gets a value that indicates whether the <see cref="IConcurrentDeque{T}"/> is empty.
    /// </summary>
    Boolean IsEmpty { get; }

    /// <summary>
    /// Adds an item to the right end of the <see cref="IConcurrentDeque{T}"/>.
    /// </summary>
    /// <param name="item">The item to be added to the <see cref="IConcurrentDeque{T}"/>.</param>
    void PushRight(T item);

    /// <summary>
    /// Adds an item to the left end of the <see cref="IConcurrentDeque{T}"/>.
    /// </summary>
    /// <param name="item">The item to be added to the <see cref="IConcurrentDeque{T}"/>.</param>
    void PushLeft(T item);

    /// <summary>
    /// Attempts to remove and return an item from the right end of the <see cref="IConcurrentDeque{T}"/>.
    /// </summary>
    /// <param name="item">When this method returns, if the operation was successful, <paramref name="item"/> contains the 
    /// object removed. If no object was available to be removed, the value is unspecified.</param>
    /// <returns>true if an element was removed and returned succesfully; otherwise, false.</returns>
    Boolean TryPopRight(out T item);

    /// <summary>
    /// Attempts to remove and return an item from the left end of the <see cref="IConcurrentDeque{T}"/>.
    /// </summary>
    /// <param name="item">When this method returns, if the operation was successful, <paramref name="item"/> contains the 
    /// object removed. If no object was available to be removed, the value is unspecified.</param>
    /// <returns>true if an element was removed and returned succesfully; otherwise, false.</returns>
    Boolean TryPopLeft(out T item);

    /// <summary>
    /// Attempts to return the rightmost item of the <see cref="IConcurrentDeque{T}"/> 
    /// without removing it.
    /// </summary>
    /// <param name="item">When this method returns, <paramref name="item"/> contains the rightmost item
    /// of the <see cref="ConcurrentDeque{T}"/> or an unspecified value if the operation failed.</param>
    /// <returns>true if an item was returned successfully; otherwise, false.</returns>
    Boolean TryPeekRight(out T item);

    /// <summary>
    /// Attempts to return the leftmost item of the <see cref="IConcurrentDeque{T}"/> 
    /// without removing it.
    /// </summary>
    /// <param name="item">When this method returns, <paramref name="item"/> contains the leftmost item
    /// of the <see cref="ConcurrentDeque{T}"/> or an unspecified value if the operation failed.</param>
    /// <returns>true if an item was returned successfully; otherwise, false.</returns>
    Boolean TryPeekLeft(out T item);

    /// <summary>
    /// Removes all items from the <see cref="IConcurrentDeque{T}"/>.
    /// </summary>
    void Clear();
}
}
