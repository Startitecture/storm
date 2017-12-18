// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MessageRoutingRequestCollection.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// <summary>
//   The message routing request collection.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.MessageQueuing
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;

    /// <summary>
    /// The message routing request collection.
    /// </summary>
    /// <typeparam name="TMessage">
    /// The type of message contained within the routing request.
    /// </typeparam>
    public class MessageRoutingRequestCollection<TMessage> : ReadOnlyCollection<MessageRoutingRequest<TMessage>>
        where TMessage : IEquatable<TMessage>, IComparable<TMessage>
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageRoutingRequestCollection{TMessage}"/> class.
        /// </summary>
        public MessageRoutingRequestCollection()
            : this(new MessageRoutingRequest<TMessage>[0])
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageRoutingRequestCollection{TMessage}"/> class.
        /// </summary>
        /// <param name="list">
        /// A list of message routing requests to initialize the collection with.
        /// </param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "Microsoft.Design", 
            "CA1006:DoNotNestGenericTypesInMemberSignatures", 
            Justification = "For collections of generics this is the only available approach.")]
        public MessageRoutingRequestCollection(IList<MessageRoutingRequest<TMessage>> list)
            : base(list)
        {
        }

        #endregion
    }
}