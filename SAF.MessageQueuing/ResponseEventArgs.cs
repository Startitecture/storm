// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ResponseEventArgs.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// <summary>
//   Contains event data associated with QueueExit events.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.MessageQueuing
{
    using System;

    /// <summary>
    /// Contains event data associated with <see cref="T:SAF.MessageQueuing.IResponseEvent`1"/> events.
    /// </summary>
    /// <typeparam name="TMessage">
    /// The type of <see cref="IPriorityMessage"/> that has exited the queue.
    /// </typeparam>
    public class ResponseEventArgs<TMessage> : EventArgs
        where TMessage : IEquatable<TMessage>, IComparable<TMessage>
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="T:SAF.MessageQueuing.ResponseEventArgs`1"/> class.
        /// </summary>
        /// <param name="exitEvent">
        /// The exit event.
        /// </param>
        public ResponseEventArgs(MessageExit<TMessage> exitEvent)
        {
            if (exitEvent == null)
            {
                throw new ArgumentNullException("exitEvent");
            }

            this.ResponseEvent = exitEvent;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the exit event.
        /// </summary>
        public MessageExit<TMessage> ResponseEvent { get; private set; }

        #endregion

        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current response event.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> that represents the current response event.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override string ToString()
        {
            return Convert.ToString(this.ResponseEvent);
        }
    }
}