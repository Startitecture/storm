// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IMessageRouterFactory.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.MessageQueuing
{
    using System;

    /// <summary>
    /// Provides an interface for classes that create message routers.
    /// </summary>
    /// <typeparam name="TMessage">
    /// The type of message that will be routed.
    /// </typeparam>
    public interface IMessageRouterFactory<TMessage>
        where TMessage : IEquatable<TMessage>, IComparable<TMessage>, IComparable
    {
        /// <summary>
        /// Creates a new <see cref="T:SAF.MessageQueueing.MessageRouter`1"/> instance.
        /// </summary>
        /// <returns>
        /// A new <see cref="T:SAF.MessageQueueing.MessageRouter`1"/> instance.
        /// </returns>
        IMessageRouter<TMessage> Create();
    }
}