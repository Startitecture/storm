// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RoutingStatusCollection.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// <summary>
//   Contains a collection of routing status elements.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.MessageQueuing
{
    using System;
    using System.Collections.Generic;

    using SAF.Core;

    /// <summary>
    /// Contains a collection of routing status elements.
    /// </summary>
    /// <typeparam name="TMessage">
    /// The type of message contained in the collection.
    /// </typeparam>
    public class RoutingStatusCollection<TMessage> : SerializableCollection<IRoutingStatus<TMessage>>
        where TMessage : IEquatable<TMessage>, IComparable<TMessage>
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="RoutingStatusCollection{TMessage}"/> class.
        /// </summary>
        public RoutingStatusCollection()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RoutingStatusCollection{TMessage}"/> class.
        /// </summary>
        /// <param name="values">
        /// The values.
        /// </param>
        public RoutingStatusCollection(IEnumerable<IRoutingStatus<TMessage>> values)
            : base(values)
        {
        }

        #endregion
    }
}