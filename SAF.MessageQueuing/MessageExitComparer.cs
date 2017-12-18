// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MessageExitComparer.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// <summary>
//   The message exit priority comparer.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.MessageQueuing
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// The message exit priority comparer.
    /// </summary>
    /// <typeparam name="TMessage">
    /// The type of message to compare.
    /// </typeparam>
    public class MessageExitComparer<TMessage> : Comparer<MessageExit<TMessage>>
        where TMessage : IEquatable<TMessage>, IComparable<TMessage>
    {
        /// <summary>
        /// The priority comparer.
        /// </summary>
        private readonly IComparer<MessageRoutingRequest<TMessage>> messageComparer;

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageExitComparer{TMessage}"/> class.
        /// </summary>
        /// <param name="messageComparer">
        /// The priority comparer.
        /// </param>
        public MessageExitComparer(IComparer<TMessage> messageComparer)
        {
            this.messageComparer = new MessageRoutingRequestComparer<TMessage>(messageComparer);
        }

        /// <summary>
        /// When overridden in a derived class, performs a comparison of two objects of the same type and returns a value indicating whether one object is less than, equal to, or greater than the other.
        /// </summary>
        /// <returns>
        /// A signed integer that indicates the relative values of <paramref name="x"/> and <paramref name="y"/>, as shown in the following table.Value Meaning Less than zero <paramref name="x"/> is less than <paramref name="y"/>.Zero <paramref name="x"/> equals <paramref name="y"/>.Greater than zero <paramref name="x"/> is greater than <paramref name="y"/>.
        /// </returns>
        /// <param name="x">The first object to compare.</param>
        /// <param name="y">The second object to compare.</param>
        public override int Compare(MessageExit<TMessage> x, MessageExit<TMessage> y)
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

            return this.messageComparer.Compare(x.RoutingRequest, y.RoutingRequest);
        }
    }
}
