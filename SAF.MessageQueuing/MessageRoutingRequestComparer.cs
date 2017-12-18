// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MessageRoutingRequestComparer.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// <summary>
//   Compares two <see cref="T:SAF.MessageQueuing.MessageRoutingRequest`1" /> values by the priority of the message.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.MessageQueuing
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Compares two <see cref="T:SAF.MessageQueuing.MessageRoutingRequest`1"/> values by the priority of the message.
    /// </summary>
    /// <typeparam name="TMessage">
    /// The type of <see cref="IPriorityMessage"/> to compare.
    /// </typeparam>
    public class MessageRoutingRequestComparer<TMessage> : Comparer<MessageRoutingRequest<TMessage>>
        where TMessage : IEquatable<TMessage>, IComparable<TMessage>
    {
        /// <summary>
        /// The message comparer.
        /// </summary>
        private readonly IComparer<TMessage> messageComparer;

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageRoutingRequestComparer{TMessage}"/> class.
        /// </summary>
        /// <param name="messageComparer">
        /// The message comparer.
        /// </param>
        public MessageRoutingRequestComparer(IComparer<TMessage> messageComparer)
        {
            this.messageComparer = messageComparer;
        }

        /// <summary>
        /// When overridden in a derived class, performs a comparison of two objects of the same type and returns a value indicating whether one object is less than, equal to, or greater than the other.
        /// </summary>
        /// <returns>
        /// A signed integer that indicates the relative values of <paramref name="x"/> and <paramref name="y"/>, as shown in the following table.
        /// Value Meaning 
        /// Less than zero <paramref name="x"/> is less than <paramref name="y"/>.
        /// Zero <paramref name="x"/> equals <paramref name="y"/>.
        /// Greater than zero <paramref name="x"/> is greater than <paramref name="y"/>.
        /// </returns>
        /// <param name="x">
        /// The first object to compare.
        /// </param><param name="y">
        /// The second object to compare.
        /// </param>
        public override int Compare(MessageRoutingRequest<TMessage> x, MessageRoutingRequest<TMessage> y)
        {
            // TODO: Create unit tests.
            if (x == null && y == null)
            {
                return 0;
            }

            if (x == null)
            {
                return -1;
            }

            if (y == null)
            {
                return 1;
            }

            return this.messageComparer.Compare(x.Message, y.Message);
        }
    }
}
