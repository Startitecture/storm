// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EntityServiceClient.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.EntityServices
{
    using System.ServiceModel;
    using System.ServiceModel.Channels;

    /// <summary>
    /// The entity service client.
    /// </summary>
    /// <typeparam name="TInterface">
    /// The type of interface that the client connects to.
    /// </typeparam>
    public class EntityServiceClient<TInterface> : ClientBase<TInterface>, IServiceClient<TInterface>
        where TInterface : class
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EntityServiceClient{TInterface}"/> class.
        /// </summary>
        public EntityServiceClient()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EntityServiceClient{TInterface}"/> class.
        /// </summary>
        /// <param name="endpointConfigurationName">
        /// The endpoint configuration name.
        /// </param>
        public EntityServiceClient(string endpointConfigurationName)
            : base(endpointConfigurationName)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EntityServiceClient{TInterface}"/> class.
        /// </summary>
        /// <param name="endpointConfigurationName">
        /// The endpoint configuration name.
        /// </param>
        /// <param name="remoteAddress">
        /// The remote address.
        /// </param>
        public EntityServiceClient(string endpointConfigurationName, string remoteAddress)
            : base(endpointConfigurationName, remoteAddress)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EntityServiceClient{TInterface}"/> class.
        /// </summary>
        /// <param name="endpointConfigurationName">
        /// The endpoint configuration name.
        /// </param>
        /// <param name="remoteAddress">
        /// The remote address.
        /// </param>
        public EntityServiceClient(string endpointConfigurationName, EndpointAddress remoteAddress)
            : base(endpointConfigurationName, remoteAddress)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EntityServiceClient{TInterface}"/> class.
        /// </summary>
        /// <param name="binding">
        /// The binding.
        /// </param>
        /// <param name="remoteAddress">
        /// The remote address.
        /// </param>
        public EntityServiceClient(Binding binding, EndpointAddress remoteAddress)
            : base(binding, remoteAddress)
        {
        }

        /// <summary>
        /// Gets the service channel.
        /// </summary>
        public TInterface ServiceChannel
        {
            get
            {
                return this.Channel;
            }
        }
    }
}