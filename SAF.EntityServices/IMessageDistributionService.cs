// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IMessageDistributionService.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// <summary>
//   Provides an interface for services that manage message distribution.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.EntityServices
{
    using System.Collections.Generic;
    using System.ServiceModel;

    /// <summary>
    /// Provides an interface for services that manage message distribution.
    /// </summary>
    /// <typeparam name="TContract">
    /// The type of contract for the distributed message.
    /// </typeparam>
    /// <typeparam name="TStatus">
    /// The type of status for the service.
    /// </typeparam>
    [ServiceContract]
    public interface IMessageDistributionService<in TContract, TStatus>
    {
        /// <summary>
        /// Gets the messages that have been aborted in active processing.
        /// </summary>
        /// <returns>
        /// A list of status elements containing the aborted messages.
        /// </returns>
        [OperationContract]
        [FaultContract(typeof(ApplicationConfigurationFault))]
        [FaultContract(typeof(EntityValidationFault))]
        [FaultContract(typeof(EntityRepositoryFault))]
        [FaultContract(typeof(InternalOperationFault))]
        List<TStatus> AbortedMessages(); 

        /// <summary>
        /// Sends a message to the distribution channel.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        [OperationContract]
        [FaultContract(typeof(ApplicationConfigurationFault))]
        [FaultContract(typeof(EntityValidationFault))]
        [FaultContract(typeof(EntityRepositoryFault))]
        [FaultContract(typeof(InternalOperationFault))]
        void Send(TContract message);

        /// <summary>
        /// Determines whether the specified message is active in the current channel.
        /// </summary>
        /// <param name="message">
        /// The message to locate.
        /// </param>
        /// <returns>
        /// <c>true</c> if the current channel is actively processing the message; otherwise, <c>false</c>.
        /// </returns>
        [OperationContract]
        [FaultContract(typeof(ApplicationConfigurationFault))]
        [FaultContract(typeof(EntityValidationFault))]
        [FaultContract(typeof(EntityRepositoryFault))]
        [FaultContract(typeof(InternalOperationFault))]
        bool IsActive(TContract message);

        /// <summary>
        /// Cancels the message in the routing status.
        /// </summary>
        /// <param name="message">
        /// The message to cancel.
        /// </param>
        [OperationContract]
        [FaultContract(typeof(ApplicationConfigurationFault))]
        [FaultContract(typeof(EntityValidationFault))]
        [FaultContract(typeof(EntityRepositoryFault))]
        [FaultContract(typeof(InternalOperationFault))]
        void Cancel(TContract message);

        /// <summary>
        /// Requeues the specified message.
        /// </summary>
        /// <param name="message">
        /// The message to requeue.
        /// </param>
        [OperationContract]
        [FaultContract(typeof(ApplicationConfigurationFault))]
        [FaultContract(typeof(EntityValidationFault))]
        [FaultContract(typeof(EntityRepositoryFault))]
        [FaultContract(typeof(InternalOperationFault))]
        void Requeue(TContract message);

        /// <summary>
        /// Accepts notification that the specified channel is available.
        /// </summary>
        /// <param name="channel">
        /// The channel to notify.
        /// </param>
        /// <returns>
        /// The <see cref="DistributionChannelDto"/> of the channel that has been elected to queue messages.
        /// </returns>
        [OperationContract]
        [FaultContract(typeof(ApplicationConfigurationFault))]
        [FaultContract(typeof(EntityValidationFault))]
        [FaultContract(typeof(EntityRepositoryFault))]
        [FaultContract(typeof(InternalOperationFault))]
        DistributionChannelDto NotifyAvailable(DistributionChannelDto channel);
    }
}
