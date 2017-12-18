// ----------------------------------------------------------------------------------------------------------------
// <copyright file="ConcurrentSortedCollection.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// <summary>
//   Provides concurrent, sorted queuing for  items.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.MessageQueuing
{
    using System;
    using System.Collections;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;

    using SAF.Core;
    using SAF.StringResources;

    /// <summary>
    /// Provides concurrent, sorted queuing for <see cref="IPriorityMessage"/> items.
    /// </summary>
    /// <typeparam name="TMessage">
    /// The type of <see cref="IPriorityMessage"/> to queue.
    /// </typeparam>
    public class ConcurrentSortedCollection<TMessage> : IProducerConsumerCollection<TMessage>
        where TMessage : IEquatable<TMessage>, IComparable<TMessage>
    {
        // Used for enforcing thread-safety
        #region Fields

        /// <summary>
        /// The sorted set.
        /// </summary>
        private readonly SortedSet<TMessage> sortedSet;

        /// <summary>
        /// The sync object.
        /// </summary>
        private readonly object syncObject = new object();

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ConcurrentSortedCollection{TMessage}"/> class.
        /// </summary>
        /// <param name="comparer">
        /// The comparer.
        /// </param>
        public ConcurrentSortedCollection(IComparer<TMessage> comparer)
        {
            this.sortedSet = new SortedSet<TMessage>(comparer);
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the number of elements contained in the <see cref="T:System.Collections.ICollection"/>.
        /// </summary>
        /// <returns>
        /// The number of elements contained in the <see cref="T:System.Collections.ICollection"/>.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public int Count { get; private set; }

        /// <summary>
        /// Gets a value indicating whether access to the <see cref="T:System.Collections.ICollection"/> is synchronized (thread safe).
        /// </summary>
        /// <returns>
        /// true if access to the <see cref="T:System.Collections.ICollection"/> is synchronized (thread safe); otherwise, false.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public bool IsSynchronized
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// Gets an object that can be used to synchronize access to the <see cref="T:System.Collections.ICollection"/>.
        /// </summary>
        /// <returns>
        /// An object that can be used to synchronize access to the <see cref="T:System.Collections.ICollection"/>.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public object SyncRoot
        {
            get
            {
                return this.syncObject;
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// Copies the elements of the <see cref="T:System.Collections.ICollection"/> to an <see cref="T:System.Array"/>, starting at a
        /// particular <see cref="T:System.Array"/> index.
        /// </summary>
        /// <param name="array">
        /// The one-dimensional <see cref="T:System.Array"/> that is the destination of the elements copied from 
        /// <see cref="T:System.Collections.ICollection"/>. The <see cref="T:System.Array"/> must have zero-based indexing. 
        /// </param>
        /// <param name="index">
        /// The zero-based index in <paramref name="array"/> at which copying begins. 
        /// </param>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="array"/> is null. 
        /// </exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        /// <paramref name="index"/> is less than zero. 
        /// </exception>
        /// <exception cref="T:System.ArgumentException">
        /// <paramref name="array"/> is multidimensional.-or- The number of elements in the source
        /// <see cref="T:System.Collections.ICollection"/> is greater than the available space from <paramref name="index"/> to the end
        /// of the destination <paramref name="array"/>. 
        /// </exception>
        /// <exception cref="T:System.ArgumentException">
        /// The type of the source <see cref="T:System.Collections.ICollection"/> cannot be cast automatically to the type of the 
        /// destination <paramref name="array"/>. 
        /// </exception>
        /// <filterpriority>2</filterpriority>
        public void CopyTo(Array array, int index)
        {
            lock (this.syncObject)
            {
                ((ICollection)this.sortedSet).CopyTo(array, index);
            }
        }

        /// <summary>
        /// Copies the elements of the <see cref="T:System.Collections.Concurrent.IProducerConsumerCollection`1"/> to an <see cref="T:System.Array"/>, starting at a specified index.
        /// </summary>
        /// <param name="array">
        /// The one-dimensional <see cref="T:System.Array"/> that is the destination of the elements copied from the 
        /// <see cref="T:System.Collections.Concurrent.IProducerConsumerCollection`1"/>. The array must have zero-based indexing.
        /// </param>
        /// <param name="index">
        /// The zero-based index in <paramref name="array"/> at which copying begins.
        /// </param>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="array"/> is a null reference (Nothing in Visual Basic).
        /// </exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        /// <paramref name="index"/> is less than zero.
        /// </exception>
        /// <exception cref="T:System.ArgumentException">
        /// <paramref name="index"/> is equal to or greater than the length of the <paramref name="array"/> -or- The number of elements
        /// in the source <see cref="T:System.Collections.Concurrent.ConcurrentQueue`1"/> is greater than the available space from 
        /// <paramref name="index"/> to the end of the destination <paramref name="array"/>.
        /// </exception>
        public void CopyTo(TMessage[] array, int index)
        {
            lock (this.syncObject)
            {
                this.sortedSet.CopyTo(array, index);
            }
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Collections.Generic.IEnumerator`1"/> that can be used to iterate through the collection.
        /// </returns>
        /// <filterpriority>1</filterpriority>
        public IEnumerator<TMessage> GetEnumerator()
        {
            SortedSet<TMessage> returnSet;

            lock (this.syncObject)
            {
                returnSet = new SortedSet<TMessage>(this.sortedSet);
            }

            return returnSet.GetEnumerator();
        }

        /// <summary>
        /// Copies the elements contained in the <see cref="T:System.Collections.Concurrent.IProducerConsumerCollection`1"/> to a new array.
        /// </summary>
        /// <returns>
        /// A new array containing the elements copied from the <see cref="T:System.Collections.Concurrent.IProducerConsumerCollection`1"/>.
        /// </returns>
        public TMessage[] ToArray()
        {
            TMessage[] valueArray;

            lock (this.syncObject)
            {
                valueArray = this.sortedSet.ToArray();
            }

            return valueArray;
        }

        /// <summary>
        /// Attempts to add an object to the <see cref="T:System.Collections.Concurrent.IProducerConsumerCollection`1"/>.
        /// </summary>
        /// <param name="item">
        /// The object to add to the <see cref="T:System.Collections.Concurrent.IProducerConsumerCollection`1"/>.
        /// </param>
        /// <returns>
        /// true if the object was added successfully; otherwise, false.
        /// </returns>
        /// <exception cref="T:System.ArgumentException">
        /// The <paramref name="item"/> was invalid for this collection.
        /// </exception>
        public bool TryAdd(TMessage item)
        {
            if (Evaluate.IsNull(item))
            {
                throw new ArgumentNullException("item");
            }

            lock (this.syncObject)
            {
                // Don't rely on SortedSet<T> for count.
                if (this.sortedSet.Add(item))
                {
                    this.Count++;
                }
                else
                {
                    Trace.TraceError("Could not add '{0}'. Current items: {1}", item, String.Join(Environment.NewLine, this.sortedSet));
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Attempts to remove and return an object from the <see cref="T:System.Collections.Concurrent.IProducerConsumerCollection`1"/>.
        /// </summary>
        /// <returns>
        /// true if an object was removed and returned successfully; otherwise, false.
        /// </returns>
        /// <param name="item">
        /// When this method returns, if the object was removed and returned successfully, <paramref name="item"/> contains the removed
        /// object. If no object was available to be removed, the value is unspecified.
        /// </param>
        public bool TryTake(out TMessage item)
        {
            item = default(TMessage);

            if (this.sortedSet.Count == 0)
            {
                return false;
            }

            lock (this.syncObject)
            {
                if (this.sortedSet.Count == 0)
                {
                    return false;
                }

                var next = this.sortedSet.Min;

                if (Evaluate.IsNull(next))
                {
                    return false;
                }

// ReSharper disable AssignNullToNotNullAttribute -- Evaluate.IsNull checks for null.
                if (this.sortedSet.Remove(next))
// ReSharper restore AssignNullToNotNullAttribute
                {
                    item = next;
                    this.Count--;
                }
                else
                {
                    throw new OperationException(next, String.Format(ErrorMessages.SortedSetItemNotFoundForRemoval, next));
                }
            }

            return true;
        }

        #endregion

        #region Explicit Interface Methods

        /// <summary>
        /// The get enumerator.
        /// </summary>
        /// <returns>
        /// The <see cref="IEnumerator"/>.
        /// </returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        #endregion
    }
}