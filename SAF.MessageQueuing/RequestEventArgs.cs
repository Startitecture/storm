// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RequestEventArgs.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// <summary>
//   Contains event data associated with QueueEntry events.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.MessageQueuing
{
    using System;

    /// <summary>
    /// Contains event data associated with <see cref="IPriorityMessage"/> events.
    /// </summary>
    /// <typeparam name="TMessage">
    /// The type of <see cref="IPriorityMessage"/> associated with the event.
    /// </typeparam>
    public class RequestEventArgs<TMessage> : EventArgs
        where TMessage : IEquatable<TMessage>, IComparable<TMessage>
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="T:SAF.MessageQueuing.RequestEventArgs`1"/> class.
        /// </summary>
        /// <param name="entryEvent">
        /// The entry event.
        /// </param>
        public RequestEventArgs(MessageEntry<TMessage> entryEvent)
        {
            if (entryEvent == null)
            {
                throw new ArgumentNullException("entryEvent");
            }

            this.RequestEvent = entryEvent;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the entry event.
        /// </summary>
        public MessageEntry<TMessage> RequestEvent { get; private set; }

        #endregion

        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current request.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> that represents the current request.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override string ToString()
        {
            return Convert.ToString(this.RequestEvent);
        }
    }
}