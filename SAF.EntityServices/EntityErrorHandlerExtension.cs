// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EntityErrorHandlerExtension.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// <summary>
//   Error handling extension for WCF services.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.EntityServices
{
    using System;
    using System.Collections.ObjectModel;
    using System.Configuration;
    using System.Linq;
    using System.ServiceModel;
    using System.ServiceModel.Channels;
    using System.ServiceModel.Configuration;
    using System.ServiceModel.Description;
    using System.ServiceModel.Dispatcher;

    using SAF.Core;
    using SAF.StringResources;

    /// <summary>
    /// Error handling extension for WCF services.
    /// </summary>
    public class EntityErrorHandlerExtension : BehaviorExtensionElement, IServiceBehavior
    {
        /// <summary>
        /// The metadata contract name.
        /// </summary>
        private const string MetadataContractName = "IMetadataExchange";

        /// <summary>
        /// The metadata contract namespace.
        /// </summary>
        private const string MetadataContractNamespace = "http://schemas.microsoft.com/2006/04/mex";

        #region Public Properties

        /// <summary>
        /// Gets the behavior type.
        /// </summary>
        public override Type BehaviorType
        {
            get
            {
                return this.GetType();
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The add binding parameters.
        /// </summary>
        /// <param name="serviceDescription">
        /// The service description.
        /// </param>
        /// <param name="serviceHostBase">
        /// The service host base.
        /// </param>
        /// <param name="endpoints">
        /// The endpoints.
        /// </param>
        /// <param name="bindingParameters">
        /// The binding parameters.
        /// </param>
        public void AddBindingParameters(
            ServiceDescription serviceDescription, 
            ServiceHostBase serviceHostBase, 
            Collection<ServiceEndpoint> endpoints, 
            BindingParameterCollection bindingParameters)
        {
        }

        /// <summary>
        /// The apply dispatch behavior.
        /// </summary>
        /// <param name="serviceDescription">
        /// The service description.
        /// </param>
        /// <param name="serviceHostBase">
        /// The service host base.
        /// </param>
        public void ApplyDispatchBehavior(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
        {
            if (serviceHostBase == null)
            {
                throw new ArgumentNullException("serviceHostBase");
            }

            IErrorHandler errorHandlerInstance = new EntityErrorHandler();

            foreach (var dispatcher in serviceHostBase.ChannelDispatchers.OfType<ChannelDispatcher>())
            {
                dispatcher.ErrorHandlers.Add(errorHandlerInstance);
            }
        }

        /// <summary>
        /// The validate.
        /// </summary>
        /// <param name="serviceDescription">
        /// The service description.
        /// </param>
        /// <param name="serviceHostBase">
        /// The service host base.
        /// </param>
        /// <exception cref="InvalidOperationException">
        /// A <see cref="FaultContractAttribute"/> is not found on an operation contract.
        /// </exception>
        public void Validate(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
        {
            if (serviceDescription == null)
            {
                throw new ArgumentNullException("serviceDescription");
            }

            Func<ServiceEndpoint, bool> checkForFaults =
                endpoint =>
                !endpoint.Contract.Name.Equals(MetadataContractName) || !endpoint.Contract.Namespace.Equals(MetadataContractNamespace);

            bool notValidated =
                serviceDescription.Endpoints.Where(checkForFaults)
                                  .SelectMany(endpoint => endpoint.Contract.Operations)
                                  .Any(description => description.Faults.Count == 0);

            if (notValidated)
            {
                throw new InvalidOperationException(ErrorMessages.FaultContractNotSpecified);
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// The create behavior.
        /// </summary>
        /// <returns>
        /// The <see cref="object"/>.
        /// </returns>
        protected override object CreateBehavior()
        {
            return this;
        }

        #endregion
    }
}