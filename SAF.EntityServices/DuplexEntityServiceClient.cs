// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DuplexEntityServiceClient.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.EntityServices
{
    using System.ServiceModel;
    using System.ServiceModel.Channels;
    using System.ServiceModel.Description;

    using JetBrains.Annotations;

    /// <summary>
    /// The duplex entity service client.
    /// </summary>
    /// <typeparam name="TInterface">
    /// The type of interface that the client connects to.
    /// </typeparam>
    public class DuplexEntityServiceClient<TInterface> : DuplexClientBase<TInterface>, IServiceClient<TInterface>
        where TInterface : class
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DuplexEntityServiceClient{TInterface}"/> class.
        /// </summary>
        /// <param name="callbackInstance">
        /// The callback instance.
        /// </param>
        public DuplexEntityServiceClient(object callbackInstance)
            : base(callbackInstance)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DuplexEntityServiceClient{TInterface}"/> class.
        /// </summary>
        /// <param name="callbackInstance">
        /// The callback instance.
        /// </param>
        /// <param name="endpointConfigurationName">
        /// The endpoint configuration name.
        /// </param>
        public DuplexEntityServiceClient(object callbackInstance, [NotNull] string endpointConfigurationName)
            : base(callbackInstance, endpointConfigurationName)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DuplexEntityServiceClient{TInterface}"/> class.
        /// </summary>
        /// <param name="callbackInstance">
        /// The callback instance.
        /// </param>
        /// <param name="endpointConfigurationName">
        /// The endpoint configuration name.
        /// </param>
        /// <param name="remoteAddress">
        /// The remote address.
        /// </param>
        public DuplexEntityServiceClient(
            object callbackInstance, 
            [NotNull] string endpointConfigurationName, 
            [NotNull] string remoteAddress)
            : base(callbackInstance, endpointConfigurationName, remoteAddress)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DuplexEntityServiceClient{TInterface}"/> class.
        /// </summary>
        /// <param name="callbackInstance">
        /// The callback instance.
        /// </param>
        /// <param name="endpointConfigurationName">
        /// The endpoint configuration name.
        /// </param>
        /// <param name="remoteAddress">
        /// The remote address.
        /// </param>
        public DuplexEntityServiceClient(
            object callbackInstance, 
            [NotNull] string endpointConfigurationName, 
            [NotNull] EndpointAddress remoteAddress)
            : base(callbackInstance, endpointConfigurationName, remoteAddress)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DuplexEntityServiceClient{TInterface}"/> class.
        /// </summary>
        /// <param name="callbackInstance">
        /// The callback instance.
        /// </param>
        /// <param name="binding">
        /// The binding.
        /// </param>
        /// <param name="remoteAddress">
        /// The remote address.
        /// </param>
        public DuplexEntityServiceClient(object callbackInstance, [NotNull] Binding binding, [NotNull] EndpointAddress remoteAddress)
            : base(callbackInstance, binding, remoteAddress)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DuplexEntityServiceClient{TInterface}"/> class.
        /// </summary>
        /// <param name="callbackInstance">
        /// The callback instance.
        /// </param>
        /// <param name="endpoint">
        /// The endpoint.
        /// </param>
        public DuplexEntityServiceClient(object callbackInstance, [NotNull] ServiceEndpoint endpoint)
            : base(callbackInstance, endpoint)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DuplexEntityServiceClient{TInterface}"/> class.
        /// </summary>
        /// <param name="callbackInstance">
        /// The callback instance.
        /// </param>
        public DuplexEntityServiceClient([NotNull] InstanceContext callbackInstance)
            : base(callbackInstance)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DuplexEntityServiceClient{TInterface}"/> class.
        /// </summary>
        /// <param name="callbackInstance">
        /// The callback instance.
        /// </param>
        /// <param name="endpointConfigurationName">
        /// The endpoint configuration name.
        /// </param>
        public DuplexEntityServiceClient([NotNull] InstanceContext callbackInstance, [NotNull] string endpointConfigurationName)
            : base(callbackInstance, endpointConfigurationName)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DuplexEntityServiceClient{TInterface}"/> class.
        /// </summary>
        /// <param name="callbackInstance">
        /// The callback instance.
        /// </param>
        /// <param name="endpointConfigurationName">
        /// The endpoint configuration name.
        /// </param>
        /// <param name="remoteAddress">
        /// The remote address.
        /// </param>
        public DuplexEntityServiceClient(
            [NotNull] InstanceContext callbackInstance, 
            [NotNull] string endpointConfigurationName, 
            [NotNull] string remoteAddress)
            : base(callbackInstance, endpointConfigurationName, remoteAddress)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DuplexEntityServiceClient{TInterface}"/> class.
        /// </summary>
        /// <param name="callbackInstance">
        /// The callback instance.
        /// </param>
        /// <param name="endpointConfigurationName">
        /// The endpoint configuration name.
        /// </param>
        /// <param name="remoteAddress">
        /// The remote address.
        /// </param>
        public DuplexEntityServiceClient(
            [NotNull] InstanceContext callbackInstance, 
            [NotNull] string endpointConfigurationName, 
            [NotNull] EndpointAddress remoteAddress)
            : base(callbackInstance, endpointConfigurationName, remoteAddress)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DuplexEntityServiceClient{TInterface}"/> class.
        /// </summary>
        /// <param name="callbackInstance">
        /// The callback instance.
        /// </param>
        /// <param name="binding">
        /// The binding.
        /// </param>
        /// <param name="remoteAddress">
        /// The remote address.
        /// </param>
        public DuplexEntityServiceClient(
            [NotNull] InstanceContext callbackInstance, 
            [NotNull] Binding binding, 
            [NotNull] EndpointAddress remoteAddress)
            : base(callbackInstance, binding, remoteAddress)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DuplexEntityServiceClient{TInterface}"/> class.
        /// </summary>
        /// <param name="callbackInstance">
        /// The callback instance.
        /// </param>
        /// <param name="endpoint">
        /// The endpoint.
        /// </param>
        public DuplexEntityServiceClient([NotNull] InstanceContext callbackInstance, [NotNull] ServiceEndpoint endpoint)
            : base(callbackInstance, endpoint)
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