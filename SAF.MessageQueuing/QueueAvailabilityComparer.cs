// --------------------------------------------------------------------------------------------------------------------
// <copyright file="QueueAvailabilityComparer.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// <summary>
//   Compares two  instances for availability.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.MessageQueuing
{
    using System.Collections.Generic;

    using SAF.Core;

    /// <summary>
    /// Compares two <see cref="IPriorityQueueState"/> instances for availability.
    /// </summary>
    /// <typeparam name="TQueue">
    /// The type of <see cref="IPriorityQueueState"/> to compare.
    /// </typeparam>
    public class QueueAvailabilityComparer<TQueue> : Comparer<TQueue>, IComparer<IPriorityQueueState>
        where TQueue : IObservableQueueState 
    {
        /// <summary>
        /// The availability comparer.
        /// </summary>
        private static readonly QueueAvailabilityComparer<TQueue> AvailabilityComparer = new QueueAvailabilityComparer<TQueue>();

        /// <summary>
        /// Gets the availability comparer.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "Microsoft.Design", 
            "CA1000:DoNotDeclareStaticMembersOnGenericTypes",
            Justification = "Using the Comparer<T> pattern.")]
        public static QueueAvailabilityComparer<TQueue> Availability
        {
            get
            {
                return AvailabilityComparer;
            }
        }

        /// <summary>
        /// When overridden in a derived class, performs a comparison of two objects of the same type and returns a value indicating 
        /// whether one object is less than, equal to, or greater than the other.
        /// </summary>
        /// <returns>
        /// A signed integer that indicates the relative values of <paramref name="x"/> and <paramref name="y"/>, as shown in the 
        /// following table.
        /// Value Meaning Less than zero <paramref name="x"/> is less than <paramref name="y"/>.
        /// Zero <paramref name="x"/> equals <paramref name="y"/>.
        /// Greater than zero <paramref name="x"/> is greater than <paramref name="y"/>.
        /// </returns>
        /// <param name="x">The first object to compare.</param>
        /// <param name="y">The second object to compare.</param>
        public override int Compare(TQueue x, TQueue y)
        {
            if (Evaluate.IsNull(x) && Evaluate.IsNull(y))
            {
                return 0;
            }

            if (Evaluate.IsNull(x))
            {
                return -1;
            }

            if (Evaluate.IsNull(y))
            {
                return 1;
            }

            // ReSharper disable PossibleNullReferenceException - Evaluate.IsNull handles this.
            return this.Compare(x.QueueState, y.QueueState);
            // ReSharper restore PossibleNullReferenceException
        }

        /// <summary>
        /// When overridden in a derived class, performs a comparison of two objects of the same type and returns a value indicating 
        /// whether one object is less than, equal to, or greater than the other.
        /// </summary>
        /// <returns>
        /// A signed integer that indicates the relative values of <paramref name="x"/> and <paramref name="y"/>, as shown in the 
        /// following table.
        /// Value Meaning Less than zero <paramref name="x"/> is less than <paramref name="y"/>.
        /// Zero <paramref name="x"/> equals <paramref name="y"/>.
        /// Greater than zero <paramref name="x"/> is greater than <paramref name="y"/>.
        /// </returns>
        /// <param name="x">The first object to compare.</param>
        /// <param name="y">The second object to compare.</param>
        public int Compare(IPriorityQueueState x, IPriorityQueueState y)
        {
            return Evaluate.Compare(
                x,
                y,
                queue => queue.QueueAborted,
                queue => queue.QueueLength,
                queue => queue.FailureRate,
                queue => queue.AverageResponseLatency,
                queue => queue.GetHashCode());
        }
    }
}
