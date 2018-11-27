using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;

#if DEBUG
[assembly: InternalsVisibleTo("Deque.NUnit")]
#endif

namespace System.Collections.Generic
{
/// <summary>
/// Represents a double-ended queue, also known as deque (pronounced "deck").
/// Items can be appended to/removed from both ends of the deque.
/// </summary>
/// <typeparam name="T">Specifies the type of the elements in the deque.</typeparam>
[DebuggerDisplay("Count = {" + nameof(Count) + "}")]
[DebuggerTypeProxy(typeof(DequeDebugView<>))]
public class Deque<T> : IDeque<T>
{
    private const Int32 DefaultCapacity = 4;

    private static readonly T[] EmptyBuffer = new T[0];

    private Int32 _version;

    private Object _syncRoot;

    /// <summary>
    /// Ring buffer that holds the items.
    /// </summary>
    private T[] _buffer;

    /// <summary>
    /// The offset used to calculate the position of the leftmost item in the buffer.
    /// </summary>
    private Int32 _leftIndex;

    /// <summary>
    /// Initializes a new instance of the <see cref="Deque{T}"/> class.
    /// </summary>
    public Deque() { _buffer = EmptyBuffer; }

    /// <summary>
    /// Initializes a new instance of the <see cref="Deque{T}"/> class with the specified capacity.
    /// </summary>
    /// <param name="capacity">The deque's initial capacity.</param>
    /// <exception cref="ArgumentOutOfRangeException">Capacity cannot be less than 0.</exception>
    public Deque(Int32 capacity)
    {
        if (capacity < 0) throw new ArgumentOutOfRangeException(nameof(capacity), "capacity was less than zero.");

        _buffer = capacity == 0 ? EmptyBuffer : new T[capacity];
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Deque{T}"/> class that contains elements copied from the specified collection.
    /// </summary>
    /// <param name="collection">The collection whose elements are copied to the deque.</param>
    public Deque(IEnumerable<T> collection)
    {
        if (collection == null) throw new ArgumentNullException(nameof(collection));

        InitializeFromCollection(collection);
    }

    private void InitializeFromCollection(IEnumerable<T> enumerable)
    {
        var collection = enumerable as ICollection<T> ?? enumerable.ToArray();

        var count = collection.Count;

        //initialize buffer
        if (count == 0)
            _buffer = EmptyBuffer;
        else
        {
            _buffer = new T[count];
            collection.CopyTo(_buffer, 0);
            Count = count;
        }
    }

    /// <summary>
    /// Adds an item to the right end of the <see cref="Deque{T}"/>.
    /// </summary>
    /// <param name="item">The item to be added to the <see cref="Deque{T}"/>.</param>
    public void PushRight(T item)
    {
        EnsureCapacity(Count + 1);

        //inc count
        Count++;

        //insert item
        Right = item;

        _version++;
    }

    /// <summary>
    /// Adds an item to the left end of the <see cref="Deque{T}"/>.
    /// </summary>
    /// <param name="item">The item to be added to the <see cref="Deque{T}"/>.</param>
    public void PushLeft(T item)
    {
        EnsureCapacity(Count + 1);

        //decrement left index and increment count
        LeftIndex--;
        Count++;

        //insert item
        Left = item;

        _version++;
    }

    /// <summary>
    /// Attempts to remove and return an item from the right end of the <see cref="Deque{T}"/>.
    /// </summary>
    /// <returns>The rightmost item.</returns>
    /// <exception cref="InvalidOperationException">The deque is empty.</exception>
    public T PopRight()
    {
        if (IsEmpty) throw new InvalidOperationException("The deque is empty");

        //retrieve rightmost item and clean buffer slot
        var right = Right;
        Right = default(T);

        //dec count
        Count--;
        _version++;
        return right;
    }

    /// <summary>
    /// Attempts to remove and return an item from the left end of the <see cref="Deque{T}"/>.
    /// </summary>
    /// <returns>The leftmost item.</returns>
    /// <exception cref="InvalidOperationException">The deque is empty.</exception>
    public T PopLeft()
    {
        if (IsEmpty) throw new InvalidOperationException("The deque is empty");

        //retrieve leftmost item and clean buffer slot
        var left = Left;
        Left = default(T);

        //increment left index and decrement count
        LeftIndex++;
        Count--;
        _version++;

        return left;
    }

    /// <summary>
    /// Attempts to return the rightmost item of the <see cref="Deque{T}"/> 
    /// without removing it.
    /// </summary>
    /// <returns>The rightmost item.</returns>
    /// <exception cref="InvalidOperationException">The deque is empty.</exception>
    public T PeekRight()
    {
        if (IsEmpty) throw new InvalidOperationException("The deque is empty");

        //retrieve rightmost item
        return Right;
    }

    /// <summary>
    /// Attempts to return the leftmost item of the <see cref="Deque{T}"/> 
    /// without removing it.
    /// </summary>
    /// <returns>The leftmost item.</returns>
    /// <exception cref="InvalidOperationException">The deque is empty.</exception>
    public T PeekLeft()
    {
        if (IsEmpty) throw new InvalidOperationException("The deque is empty");

        //retrieve leftmost item
        return Left;
    }

    /// <summary>
    /// Removes all items from the <see cref="Deque{T}"/>.
    /// </summary>
    public void Clear()
    {
        //clear the ring buffer to allow the GC to reclaim the references
        if (LoopsAround)
        {
            //clear both halves
            Array.Clear(_buffer, LeftIndex, Capacity  - LeftIndex);
            Array.Clear(_buffer, 0,         LeftIndex + (Count - Capacity));
        }
        else //clear the whole array
            Array.Clear(_buffer, LeftIndex, Count);

        Count     = 0;
        LeftIndex = 0;
        _version++;
    }

    /// <summary>
    /// Returns an enumerator that iterates through the deque.
    /// </summary>
    /// <returns>
    /// A <see cref="IEnumerator{T}"/> that can be used to iterate through the deque.
    /// </returns>
    public Enumerator GetEnumerator() { return new Enumerator(this); }

    /// <summary>
    /// Returns an enumerator that iterates through the deque.
    /// </summary>
    /// <returns>
    /// A <see cref="IEnumerator{T}"/> that can be used to iterate through the deque.
    /// </returns>
    IEnumerator<T> IEnumerable<T>.GetEnumerator() { return GetEnumerator(); }

    /// <summary>
    /// Returns an enumerator that iterates through a deque.
    /// </summary>
    /// <returns>
    /// An <see cref="IEnumerator"/> object that can be used to iterate through the deque.
    /// </returns>
    IEnumerator IEnumerable.GetEnumerator() { return GetEnumerator(); }

    /// <summary>
    /// Adds an item to the <see cref="ICollection{T}"/>.
    /// </summary>
    /// <param name="item">The object to add to the <see cref="ICollection{T}"/>.</param>
    /// <remarks>For <see cref="Deque{T}"/>, this operation will add the item to the right end of the deque.</remarks>
    void ICollection<T>.Add(T item) { PushRight(item); }

    /// <summary>
    /// Removes the first occurrence of a specific object from the <see cref="ICollection{T}"/>.
    /// </summary>
    /// <returns>
    /// true if <paramref name="item"/> was successfully removed from the <see cref="ICollection{T}"/>; otherwise, false.
    /// This method also returns false if <paramref name="item"/> is not found in the original <see cref="ICollection{T}"/>.
    /// </returns>
    /// <param name="item">The object to remove from the <see cref="ICollection{T}"/>.</param>
    Boolean ICollection<T>.Remove(T item)
    {
        //find the index of the item to be removed in the deque
        var comp         = EqualityComparer<T>.Default;
        var virtualIndex = -1;
        var counter      = 0;
        foreach (var dequeItem in this)
        {
            if (comp.Equals(item, dequeItem))
            {
                virtualIndex = counter;
                break;
            }

            counter++;
        }

        //return false if the item wasn't found
        if (virtualIndex == -1) return false;

        //if the removal should be performed on one of the ends, use the corresponding Pop operation instead
        if (virtualIndex == 0)
            PopLeft();
        else if (virtualIndex == Count - 1)
            PopRight();
        else
        {
            if (virtualIndex < Count / 2) //If the item is located towards the left end of the deque
            {
                //move the items to the left of 'item' one index to the right
                for (var i = virtualIndex - 1; i >= 0; i--) this[i + 1] = this[i];

                //clean leftmost item
                Left = default(T);

                //increase left
                LeftIndex++;
                Count--;
            }
            else //If the item is located towards the right end of the deque
            {
                //move the items to the right of 'item' one index to the left
                for (var i = virtualIndex + 1; i < Count; i++) this[i - 1] = this[i];

                //clean rightmost item
                Right = default(T);

                //decrease count
                Count--;
            }

            _version++;
        }

        return true;
    }

    /// <summary>
    /// Determines whether the <see cref="Deque{T}"/> contains a specific value.
    /// </summary>
    /// <returns>
    /// true if <paramref name="item"/> is found in the <see cref="Deque{T}"/>; otherwise, false.
    /// </returns>
    /// <param name="item">The object to locate in the <see cref="Deque{T}"/>.</param>
    public Boolean Contains(T item) { return this.Contains(item, EqualityComparer<T>.Default); }

    /// <summary>
    /// Copies the elements of the <see cref="Deque{T}"/> to an <see cref="Array"/>, starting at a particular <see cref="Array"/> index.
    /// </summary>
    /// <param name="array">The one-dimensional <see cref="Array"/> that is the destination of the elements copied from <see cref="Deque{T}"/>. The <see cref="Array"/> must have zero-based indexing.</param>
    /// <param name="arrayIndex">The zero-based index in <paramref name="array"/> at which copying begins.</param>
    /// <exception cref="ArgumentNullException"><paramref name="array"/> is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="arrayIndex"/> is less than 0 or greater than the <paramref name="array"/>'s upper bound.</exception>
    /// <exception cref="ArgumentException">The number of elements in the source <see cref="Deque{T}"/> is greater than the available space from <paramref name="arrayIndex"/> to the end of the destination <paramref name="array"/>.</exception>
    public void CopyTo(T[] array, Int32 arrayIndex) { ((ICollection) this).CopyTo(array, arrayIndex); }

    void ICollection.CopyTo(Array array, Int32 arrayIndex)
    {
        if (array == null) throw new ArgumentNullException(nameof(array));

        if (array.Rank != 1)
            throw new ArgumentException("Only single dimensional arrays are supported for the requested action.");

        if (arrayIndex < 0)
            throw new ArgumentOutOfRangeException(nameof(arrayIndex), "Index was less than the array's lower bound.");

        if (arrayIndex >= array.Length)
            throw new ArgumentOutOfRangeException(nameof(arrayIndex),
                                                  "Index was greater than the array's upper bound.");

        if (Count == 0) return;

        if (array.Length - arrayIndex < Count) throw new ArgumentException("Destination array was not long enough");

        try
        {
            //if the elements are stored sequentially (i.e., left+count doesn't "wrap around" the array's boundary),
            //copy the array as a whole
            if (!LoopsAround)
            {
                Array.Copy(_buffer, LeftIndex, array, arrayIndex, Count);
            }
            else
            {
                //copy both halves to a new array
                Array.Copy(_buffer, LeftIndex, array, arrayIndex, Capacity - LeftIndex);
                Array.Copy(_buffer, 0, array, arrayIndex + Capacity - LeftIndex,
                           LeftIndex                                + (Count - Capacity));
            }
        }
        catch (ArrayTypeMismatchException)
        {
            throw
                new ArgumentException("Target array type is not compatible with the type of items in the collection.");
        }
    }

    /// <summary>
    /// Sets the capacity to the actual number of elements in the <see cref="Deque{T}"/>.
    /// </summary>
    /// <remarks>
    /// This method can be used to minimize a deque's memory overhead once it is known that no
    /// new elements will be added to the deque. To completely clear a deque and
    /// release all memory referenced by the deque, execute <see cref="Clear"/> followed by <see cref="TrimExcess"/>.
    /// </remarks>
    public void TrimExcess() { Capacity = Count; }

    /// <summary> 
    /// Ensures that the capacity of this list is at least the given minimum
    /// value. If the currect capacity of the list is less than min, the
    /// capacity is increased to twice the current capacity or to min,
    /// whichever is larger.
    /// </summary>
    /// <param name="min">The minimum capacity required.</param>
    private void EnsureCapacity(Int32 min)
    {
        if (Capacity < min)
        {
            var newCapacity = Capacity == 0 ? DefaultCapacity : Capacity * 2;
            newCapacity = Math.Max(newCapacity, min);
            Capacity    = newCapacity;
        }
    }

    /// <summary>
    /// Uses modular arithmetic to calculate the correct ring buffer index for a given (possibly out-of-bounds) index.
    /// If <paramref name="position"/> is over the array's upper boundary, the returned index "wraps/loops around" the upper boundary.
    /// </summary>
    /// <param name="position">The possibly out-of-bounds index.</param>
    /// <returns>The ring buffer index.</returns>
    private Int32 CalcIndex(Int32 position)
    {
        //put 'position' in the range [0, Capacity-1] using modular arithmetic
        if (Capacity != 0) return position.Mod(Capacity);

        //if capacity is 0, _leftIndex must always be 0
        Debug.Assert(_leftIndex == 0);

        return 0;
    }

    /// <summary>
    /// Calculates the ring buffer index corresponding to a given "virtual index".
    /// A virtual index is the index of an item as seen from an enumerator's perspective, i.e., as if the items were laid out sequentially starting at index 0.
    /// As such, a virtual index is in the range [0, Count - 1].
    /// </summary>
    /// <param name="index">The virtual index.</param>
    /// <returns>A ring buffer index</returns>
    private Int32 VirtualIndexToBufferIndex(Int32 index)
    {
        if (index < 0 || index >= Count)
            throw new ArgumentOutOfRangeException(nameof(index),
                                                  "Index was out of range. Must be non-negative and less than the size of the collection.");

        //Apply LeftIndex offset and modular arithmetic
        return CalcIndex(LeftIndex + index);
    }

    /// <summary>
    /// Gets the number of elements contained in the <see cref="Deque{T}"/>.
    /// </summary>
    public Int32 Count { get; private set; }

    /// <summary>
    /// Gets a value indicating whether access to the <see cref="ICollection"/> is synchronized (thread safe).
    /// </summary>
    /// <returns>
    /// true if access to the <see cref="ICollection"/> is synchronized (thread safe); otherwise, false.
    /// For <see cref="Deque{T}"/>, this property always returns false.
    /// </returns>
    Boolean ICollection.IsSynchronized => false;

    /// <summary>
    /// Gets an object that can be used to synchronize access to the <see cref="ICollection"/>.
    /// </summary>
    /// <returns>
    /// An object that can be used to synchronize access to the <see cref="ICollection"/>.
    /// </returns>
    Object ICollection.SyncRoot
    {
        get
        {
            if (_syncRoot == null)
            {
                Interlocked.CompareExchange(ref _syncRoot, new Object(), null);
            }

            return _syncRoot;
        }
    }

    /// <summary>
    /// Gets a value that indicates whether the <see cref="Deque{T}"/> is empty.
    /// </summary>
    public Boolean IsEmpty => Count == 0;

    Boolean ICollection<T>.IsReadOnly => false;

    /// <summary>
    /// Gets or sets the total number of elements the internal data structure can hold without resizing.
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException"><see cref="Capacity"/> cannot be set to a value less than <see cref="Count"/>.</exception>
    public Int32 Capacity
    {
        get { return _buffer.Length; }
        set
        {
            if (value < Count)
                throw new ArgumentOutOfRangeException(nameof(value), "capacity was less than the current size.");

            if (value == Capacity) return;

            var newBuffer = new T[value];

            CopyTo(newBuffer, 0);

            LeftIndex = 0;
            _buffer   = newBuffer;
        }
    }

    /// <summary>
    /// Determines whether the deque "loops around" the array's boundary, i.e., whether the rightmost's index is lower than the leftmost's.
    /// </summary>
    /// <returns>true if the deque loops around the array's boundary; false otherwise.</returns>
    private Boolean LoopsAround => Count > (Capacity - LeftIndex);

    private Int32 LeftIndex { get { return _leftIndex; } set { _leftIndex = CalcIndex(value); } }

    private T Left { get { return this[0]; } set { this[0] = value; } }

    private T Right { get { return this[Count - 1]; } set { this[Count - 1] = value; } }

    /// <summary>
    /// Gets or sets the item at the specified index.
    /// </summary>
    /// <param name="index">The zero-based index of the element to get or set.</param>
    /// <returns>The element at the specified index.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Index was out of range. Must be non-negative and less than <see cref="ICollection{T}.Count"/>.</exception>
    public T this[Int32 index]
    {
        get { return _buffer[VirtualIndexToBufferIndex(index)]; }
        set { _buffer[VirtualIndexToBufferIndex(index)] = value; }
    }

    /// <summary>
    /// Supports a simple iteration over a generic <see cref="Deque{T}"/>.
    /// </summary>
    public struct Enumerator : IEnumerator<T>
    {
        private readonly Deque<T> _deque;
        private readonly Int32    _version;
        private          T        _current;

        //the index of the current item in the deque
        private Int32 _virtualIndex;

        internal Enumerator(Deque<T> deque)
        {
            _deque        = deque;
            _version      = _deque._version;
            _current      = default(T);
            _virtualIndex = -1;
        }

        /// <summary>
        /// Advances the enumerator to the next element of the deque.
        /// </summary>
        /// <returns>
        /// true if the enumerator was successfully advanced to the next element; false if the enumerator has passed the end of the deque.
        /// </returns>
        /// <exception cref="InvalidOperationException">
        /// The collection was modified after the enumerator was created.
        /// </exception>
        public Boolean MoveNext()
        {
            Validate();

            if (_virtualIndex == _deque.Count - 1) return false;

            _virtualIndex++;
            _current = _deque[_virtualIndex];
            return true;
        }

        /// <summary>
        /// Sets the enumerator to its initial position, which is before the first element in the collection.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// The collection was modified after the enumerator was created.
        /// </exception>
        void IEnumerator.Reset()
        {
            Validate();

            _virtualIndex = -1;
            _current      = default(T);
        }

        /// <summary>
        /// Gets the element in the <see cref="Deque{T}"/> at the current position of the enumerator.
        /// </summary>
        public T Current => _current;

        Object IEnumerator.Current => Current;

        /// <summary>
        /// Releases the enumerator's resources.
        /// </summary>
        public void Dispose() { }

        /// <summary>
        /// Verify that the deque hasn't been modified.
        /// </summary>
        private void Validate()
        {
            if (_version != _deque._version)
                throw new InvalidOperationException("Collection was modified; enumeration operation may not execute.");
        }
    }
}
}
