// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IServiceClient.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace SAF.EntityServices
{
    using System.ServiceModel;
    using System.ServiceModel.Description;

    /// <summary>
    /// The ServiceClient interface.
    /// </summary>
    /// <typeparam name="TInterface">
    /// The type of service represented by the service channel.
    /// </typeparam>
    public interface IServiceClient<TInterface>
        where TInterface : class
    {
        /// <summary>
        /// Gets the service channel.
        /// </summary>
        TInterface ServiceChannel { get; }

        /// <summary>
        /// Gets the underlying <see cref="T:System.ServiceModel.ChannelFactory`1"/> object.
        /// </summary>
        ChannelFactory<TInterface> ChannelFactory { get; }

        /// <summary>
        /// Gets the client credentials used to call an operation.
        /// </summary>
        ClientCredentials ClientCredentials { get; }

        /// <summary>
        /// Gets the target endpoint for the service to which the client can connect.
        /// </summary>
        ServiceEndpoint Endpoint { get; }

        /// <summary>
        /// Gets the underlying <see cref="System.ServiceModel.IClientChannel"/> implementation.
        /// </summary>
        IClientChannel InnerChannel { get; }

        /// <summary>
        /// Gets the current state of the <see cref="T:System.ServiceModel.ClientBase`1"/> object.
        /// </summary>
        CommunicationState State { get; }

        /// <summary>
        /// Causes the <see cref="T:System.ServiceModel.ClientBase`1"/> object to transition from the created state into the closed state.
        /// </summary>
        void Abort();

        /// <summary>
        /// Causes the <see cref="T:System.ServiceModel.ClientBase`1"/> object to transition from the created state into the closed state.
        /// </summary>
        void Close();

        /// <summary>
        /// Causes the <see cref="T:System.ServiceModel.ClientBase`1"/> object to transition from the created state into the opened state.
        /// </summary>
        void Open();
    }
}