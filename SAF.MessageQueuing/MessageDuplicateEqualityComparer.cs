// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MessageDuplicateEqualityComparer.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// <summary>
//   Compares two messages for duplication using the equality comparer provided in the constructor.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.MessageQueuing
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Compares two messages for duplication using the equality comparer provided in the constructor.
    /// </summary>
    /// <typeparam name="TMessage">
    /// The type of message to compare.
    /// </typeparam>
    public class MessageDuplicateEqualityComparer<TMessage> : EqualityComparer<MessageRoutingRequest<TMessage>>
        where TMessage : IEquatable<TMessage>, IComparable<TMessage>
    {
        /// <summary>
        /// The duplicate comparer.
        /// </summary>
        private readonly IEqualityComparer<TMessage> duplicateComparer;

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageDuplicateEqualityComparer{TMessage}"/> class.
        /// </summary>
        /// <param name="duplicateComparer">
        /// The duplicate comparer.
        /// </param>
        public MessageDuplicateEqualityComparer(IEqualityComparer<TMessage> duplicateComparer)
        {
            this.duplicateComparer = duplicateComparer;
        }

        /// <summary>
        /// When overridden in a derived class, determines whether two <see cref="T:SAF.MessageQueuing.MessageRoutingRequest`1"/> items
        /// are duplicate requests.
        /// </summary>
        /// <returns>
        /// true if the specified objects are equal; otherwise, false.
        /// </returns>
        /// <param name="x">The first object to compare.</param><param name="y">The second object to compare.</param>
        public override bool Equals(MessageRoutingRequest<TMessage> x, MessageRoutingRequest<TMessage> y)
        {
            if (x == null && y == null)
            {
                return true;
            }

            if (x == null || y == null)
            {
                return false;
            }

            return this.duplicateComparer.Equals(x.Message, y.Message);
        }

        /// <summary>
        /// When overridden in a derived class, serves as a hash function for the specified object for hashing algorithms and data structures, such as a hash table.
        /// </summary>
        /// <returns>
        /// A hash code for the specified object.
        /// </returns>
        /// <param name="obj">The object for which to get a hash code.</param><exception cref="T:System.ArgumentNullException">The type of <paramref name="obj"/> is a reference type and <paramref name="obj"/> is null.</exception>
        public override int GetHashCode(MessageRoutingRequest<TMessage> obj)
        {
            return obj == null ? 0 : this.duplicateComparer.GetHashCode(obj.Message);
        }
    }
}
