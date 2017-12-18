namespace SAF.Core
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Xml.Serialization;

    /// <summary>
    /// Base class for generic serializeable collections.
    /// </summary>
    /// <typeparam name="T">The type that the collection contains.</typeparam>
    /// <remarks>Use as a base class for implementing a generic serializeable collection. The <see cref="SerializableAttribute"/> 
    /// and <see cref="XmlIncludeAttribute"/> must be set for the subclass for XML serialization to work properly.</remarks>
    /// <example>
    /// <code>
    /// [Serializable, XmlInclude(typeof(MyType))]
    /// public class MyTypeCollection&lt;MyType&gt; : SerializableCollection&lt;MyType&gt;
    /// {
    ///     public MyTypeCollection()
    ///     {
    ///     }
    ///     public MyTypeCollection(IEnumerable&lt;MyType&gt; values)
    ///         : base(values)
    ///     {
    ///     }
    /// }
    /// </code>
    /// </example>
    public class SerializableCollection<T> : IUpdateableCollection<T>
    {
        /// <summary>
        /// The items contained in this collection.
        /// </summary>
        private readonly List<T> valueList = new List<T>();

        /// <summary>
        /// Initializes a new instance of the <see cref="SerializableCollection&lt;T&gt;"/> class.
        /// </summary>
        [DebuggerHidden]
        public SerializableCollection()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SerializableCollection&lt;T&gt;"/> class with the specified 
        /// list of values.
        /// </summary>
        /// <param name="values">The values to initialize the collection with.</param>
        [DebuggerHidden]
        public SerializableCollection(IEnumerable<T> values)
        {
            this.valueList = new List<T>(values);
        }

        /// <summary>
        /// Gets a value indicating whether the <see cref="SerializableCollection&lt;T&gt;"/> read-only.
        /// </summary>
        public bool IsReadOnly
        {
            get { return false; }
        }

        /// <summary>
        /// Gets the number of elements contained in the <see cref="SerializableCollection&lt;T&gt;"/>. 
        /// </summary>
        public int Count
        {
            get { return this.valueList.Count; }
        }

        /// <summary>
        /// Gets the list that is the base of this collection.
        /// </summary>
        [DebuggerHidden]
        protected IList<T> ValueList
        {
            get { return this.valueList; }
        } 

        /// <summary>
        /// Retrieves the item at the specified index.
        /// </summary>
        /// <param name="index">The index at which to retrieve the item.</param>
        /// <returns>The item at the specified index.</returns>
        /// <exception cref="IndexOutOfRangeException">The specified index is outside the bounds of the collection.</exception>
        [DebuggerHidden]
        public T this[int index]
        {
            get
            {
                return this.valueList[index];
            }

            set
            {
                this.valueList[index] = value;
            }
        }

        /// <summary>
        /// Gets an enumerator for this collection.
        /// </summary>
        /// <returns>An enumerator for this collection.</returns>
        [DebuggerHidden]
        public IEnumerator<T> GetEnumerator()
        {
            return new GenericEnumerator(this);
        }

        /// <summary>
        /// Gets an enumerator for this collection.
        /// </summary>
        /// <returns>An enumerator for this collection.</returns>
        [DebuggerHidden]
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        /// <summary>
        /// Adds an item to this collection.
        /// </summary>
        /// <param name="item">The mapping to add.</param>
        [DebuggerHidden]
        public void Add(T item)
        {
            this.valueList.Add(item);
        }

        /// <summary>
        /// Adds the elements of the specified collection to the end of this collection.
        /// </summary>
        /// <param name="collection">The collection to add.</param>
        public void AddRange(IEnumerable<T> collection)
        {
            this.valueList.AddRange(collection);
        }

        /// <summary>
        /// Updates the first item matching the specified predicate with the new item, or adds the item to the collection.
        /// </summary>
        /// <param name="item">The item to update or add to the collection.</param>
        /// <param name="predicate">The predicate that selects the item.</param>
        public void Update(T item, Func<T, bool> predicate)
        {
            if (this.valueList.Any(predicate))
            {
                lock (this.valueList)
                {
                    int index = this.valueList.IndexOf(this.valueList.First(predicate));
                    this.valueList[index] = item;
                }
            }
            else
            {
                this.Add(item);
            }
        }

        /// <summary>
        /// Removes the first occurrence of a specific object from this collection.
        /// </summary>
        /// <param name="item">The item to remove.</param>
        /// <returns><c>true</c> if item is successfully removed; otherwise, <c>false</c>. This method also returns
        /// <c>false</c> if item was not found in the collection.</returns>
        public bool Remove(T item)
        {
            return this.valueList.Remove(item);
        }

        /// <summary>
        /// Removes all items matching the specified predicate.
        /// </summary>
        /// <param name="predicate">The predicate that determines which items to remove.</param>
        /// <returns>The number of items removed from the list.</returns>
        public int Remove(Func<T, bool> predicate)
        {
            IEnumerable<T> itemsToRemove = this.valueList.Where(predicate).ToArray();

            foreach (T item in itemsToRemove)
            {
                this.Remove(item);
            }

            return itemsToRemove.Count();
        }

        /// <summary>
        /// Copies the elements of the <see cref="SerializableCollection&lt;T&gt;"/> to an <see cref="Array"/>, 
        /// starting at a particular <see cref="Array"/> index. 
        /// </summary>
        /// <param name="array">
        /// The one-dimensional Array that is the destination of the elements copied from <see cref="SerializableCollection&lt;T&gt;"/>.
        /// The Array must have zero-based indexing. 
        /// </param>
        /// <param name="arrayIndex">The zero-based index in array at which copying begins.</param>
        public void CopyTo(T[] array, int arrayIndex)
        {
            this.valueList.CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// Determines whether the <see cref="SerializableCollection&lt;T&gt;"/> contains a specific item. 
        /// </summary>
        /// <param name="item">The object to locate in the <see cref="SerializableCollection&lt;T&gt;"/>.</param>
        /// <returns>
        /// <c>true</c> if item is found in the <see cref="SerializableCollection&lt;T&gt;"/>; otherwise, <c>false</c>.
        /// </returns>
        public bool Contains(T item)
        {
            return this.valueList.Contains(item);
        }

        /// <summary>
        /// Removes all items from the <see cref="SerializableCollection&lt;T&gt;"/>. 
        /// </summary>
        public void Clear()
        {
            this.valueList.Clear();
        }

        /// <summary>
        /// Determines whether the specified <see cref="Object"/> is equal to the current <see cref="SerializableCollection&lt;T&gt;"/>.
        /// </summary>
        /// <param name="obj">The object to compare.</param>
        /// <returns>
        /// <c>true</c> if the specified <see cref="Object"/> is equal to the current <see cref="SerializableCollection&lt;T&gt;"/>; otherwise, 
        /// <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            if (base.Equals(obj))
            {
                return true;
            }

            return (obj is SerializableCollection<T>) ? this.Equals(obj as SerializableCollection<T>) : false;
        }

        /// <summary>
        /// Determines whether the specified <see cref="SerializableCollection&lt;T&gt;"/> is equal to the current 
        /// <see cref="SerializableCollection&lt;T&gt;"/>.
        /// </summary>
        /// <param name="obj">The <see cref="SerializableCollection&lt;T&gt;"/> to compare.</param>
        /// <returns>
        /// <c>true</c> if the specified <see cref="SerializableCollection&lt;T&gt;"/> is equal to the 
        /// current <see cref="SerializableCollection&lt;T&gt;"/>; otherwise, <c>false</c>.
        /// </returns>
        public bool Equals(SerializableCollection<T> obj)
        {
            if (obj == null)
            {
                return false;
            }

            return Evaluate.CollectionEquals(this.valueList, obj.valueList);
        }

        /// <summary>
        /// Serves as a hash function for a particular type.
        /// </summary>
        /// <returns>A hash code for the current <see cref="SerializableCollection&lt;T&gt;"/>.</returns>
        public override int GetHashCode()
        {
            return Evaluate.GenerateHashCode(this.valueList.ToArray());
        }

        /// <summary>
        /// Enumerates through a <see cref="SerializableCollection&lt;T&gt;"/>.
        /// </summary>
        public class GenericEnumerator : IEnumerator<T>
        {
            /// <summary>
            /// The item at the enumerator's current position.
            /// </summary>
            private T current;

            /// <summary>
            /// The collection of items.
            /// </summary>
            private SerializableCollection<T> collection;

            /// <summary>
            /// The current index of the collection.
            /// </summary>
            private int index;

            /// <summary>
            /// Initializes a new instance of the <see cref="GenericEnumerator"/> class with the specified 
            /// <see cref="SerializableCollection&lt;T&gt;"/>.
            /// </summary>
            /// <param name="collection">The <see cref="SerializableCollection&lt;T&gt;"/> to enumerate.</param>
            [DebuggerHidden]
            public GenericEnumerator(SerializableCollection<T> collection)
            {
                this.collection = collection;
                this.index = -1;
            }

            /// <summary>
            /// Finalizes an instance of the <see cref="GenericEnumerator"/> class.
            /// </summary>
            ~GenericEnumerator()
            {
                this.Dispose(false);
            }

            /// <summary>
            /// Gets the item at the enumerator's position in the <see cref="SerializableCollection&lt;T&gt;"/>.
            /// </summary>
            [DebuggerHidden]
            public T Current
            {
                get { return this.current; }
            }

            /// <summary>
            /// Gets the object at the enumerator's position in the collection.
            /// </summary>
            [DebuggerHidden]
            object IEnumerator.Current
            {
                get { return this.Current; }
            }

            /// <summary>
            /// Moves to the next item in the collection.
            /// </summary>
            /// <returns>True if there are items remaining in the collection, otherwise false.</returns>
            [DebuggerHidden]
            public bool MoveNext()
            {
                this.index++;

                if (this.index >= this.collection.ValueList.Count)
                {
                    return false;
                }
                else
                {
                    this.current = this.collection[this.index];
                    return true;
                }
            }

            /// <summary>
            /// Returns to the start of the collection.
            /// </summary>
            [DebuggerHidden]
            public void Reset()
            {
                this.index = -1;
            }

            /// <summary>
            /// Disposes of any unmanaged resources used by this <see cref="GenericEnumerator"/>.
            /// </summary>
            [DebuggerHidden]
            public void Dispose()
            {
                this.Dispose(true);
                GC.SuppressFinalize(this);
            }

            /// <summary>
            /// Disposes of any unmanaged resources used by this <see cref="GenericEnumerator"/>.
            /// </summary>
            /// <param name="disposing">A value indicating whether the Dispose() method is being called (true) or the 
            /// finalizer (false).</param>
            [DebuggerHidden]
            protected virtual void Dispose(bool disposing)
            {
                if (disposing)
                {
                    this.current = default(T);
                }

                // Release unmanaged resources here.
            }
        }
    }
}
