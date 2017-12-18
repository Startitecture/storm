// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ClientProxy.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// <summary>
//   Provides access to a WCF service interface and automatically closes connections when disposed.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.EntityServices
{
    using System;
    using System.Diagnostics;
    using System.ServiceModel;
    using System.ServiceModel.Channels;

    using SAF.ActionTracking;
    using SAF.Core;
    using SAF.Data;
    using SAF.StringResources;

    /// <summary>
    /// Provides access to a WCF service interface and automatically closes connections when disposed.
    /// </summary>
    /// <typeparam name="TInterface">
    /// The type of service interface to create a proxy for.
    /// </typeparam>
    public sealed class ClientProxy<TInterface> : IDisposable
        where TInterface : class
    {
        /// <summary>
        /// The action event proxy.
        /// </summary>
        private readonly IActionEventProxy actionEventProxy;

        #region Fields

        /// <summary>
        /// The channel factory.
        /// </summary>
        private readonly IServiceClient<TInterface> client;

        /////// <summary>
        /////// The open lock.
        /////// </summary>
        ////private readonly object openLock = new object();

        /////// <summary>
        /////// The client stopwatch.
        /////// </summary>
        ////private readonly Stopwatch clientStopwatch = Stopwatch.StartNew();

        /////// <summary>
        /////// The opened.
        /////// </summary>
        ////private bool opened;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ClientProxy{TInterface}"/> class.
        /// </summary>
        public ClientProxy()
            : this(typeof(TInterface).Name)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ClientProxy{TInterface}"/> class.
        /// </summary>
        /// <param name="actionEventProxy">
        /// The action event proxy.
        /// </param>
        public ClientProxy(IActionEventProxy actionEventProxy)
            : this(typeof(TInterface).Name)
        {
            this.actionEventProxy = actionEventProxy;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ClientProxy{TInterface}"/> class.
        /// </summary>
        /// <param name="endpointConfigurationName">
        /// The endpoint configuration name.
        /// </param>
        public ClientProxy(string endpointConfigurationName)
        {
            this.client = new EntityServiceClient<TInterface>(endpointConfigurationName);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ClientProxy{TInterface}"/> class.
        /// </summary>
        /// <param name="binding">
        /// The binding to use to connect to the service.
        /// </param>
        /// <param name="endpointAddress">
        /// The endpoint address of the service.
        /// </param>
        public ClientProxy(Binding binding, string endpointAddress)
        {
            this.client = new EntityServiceClient<TInterface>(binding, new EndpointAddress(endpointAddress));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ClientProxy{TInterface}"/> class for duplex communication.
        /// </summary>
        /// <param name="instanceContext">
        /// The instance context for duplex communication.
        /// </param>
        public ClientProxy(InstanceContext instanceContext)
            : this(instanceContext, typeof(TInterface).Name)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ClientProxy{TInterface}"/> class for duplex communication.
        /// </summary>
        /// <param name="instanceContext">
        /// The instance context.
        /// </param>
        /// <param name="endpointConfigurationName">
        /// The endpoint configuration name.
        /// </param>
        public ClientProxy(InstanceContext instanceContext, string endpointConfigurationName)
        {
            this.client = new DuplexEntityServiceClient<TInterface>(instanceContext, endpointConfigurationName);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ClientProxy{TInterface}"/> class.
        /// </summary>
        /// <param name="instanceContext">
        /// The instance context.
        /// </param>
        /// <param name="binding">
        /// The binding to use to connect to the service.
        /// </param>
        /// <param name="endpointAddress">
        /// The endpoint address of the service.
        /// </param>
        public ClientProxy(InstanceContext instanceContext, Binding binding, string endpointAddress)
        {
            this.client = new DuplexEntityServiceClient<TInterface>(instanceContext, binding, new EndpointAddress(endpointAddress));
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets a proxy to the service.
        /// </summary>
        public IServiceClient<TInterface> Client
        {
            get
            {
                return this.client;
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// Executes the specified action, re-throwing fault exceptions as domain exceptions.
        /// </summary>
        /// <param name="item">
        /// The item the action is executed on.
        /// </param>
        /// <param name="action">
        /// The action to execute.
        /// </param>
        /// <typeparam name="TItem">
        /// The type of item the action is executed on.
        /// </typeparam>
        /// <exception cref="BusinessException">
        /// A <see cref="FaultException"/> is thrown for a <see cref="EntityValidationFault"/>.
        /// </exception>
        /// <exception cref="RepositoryException">
        /// A <see cref="FaultException"/> is thrown for a <see cref="EntityRepositoryFault"/>.
        /// </exception>
        /// <exception cref="ApplicationConfigurationException">
        /// A <see cref="FaultException"/> is thrown for a <see cref="ApplicationConfigurationFault"/>.
        /// </exception>
        /// <exception cref="OperationException">
        /// A <see cref="FaultException"/> is thrown for a <see cref="InternalOperationFault"/>.
        /// </exception>
        public void Execute<TItem>(TItem item, Action<TItem> action)
        {
            if (action == null)
            {
                throw new ArgumentNullException("action");
            }

            ////if (this.opened == false)
            ////{
            ////    lock (this.openLock)
            ////    {
            ////        if (this.actionEventProxy != null)
            ////        {
            ////            this.actionEventProxy.RecordAction(this.clientStopwatch);
            ////        }

            ////        this.client.Open();
            ////        this.opened = true;

            ////        if (this.actionEventProxy != null)
            ////        {
            ////            this.actionEventProxy.RecordAction(this.clientStopwatch);
            ////        }
            ////    }
            ////}

            try
            {
                if (this.actionEventProxy != null)
                {
                    this.actionEventProxy.PerformAction(item, action);
                }
                else
                {
                    action(item);
                }
            }
            catch (TimeoutException ex)
            {
                // Handle the timeout exception.
                ////this.client.Abort();
                throw new OperationException(item, ex.Message, ex);
            }
            catch (FaultException<EntityValidationFault> ex)
            {
                throw new BusinessException(item, ex.Detail.ValidationErrors, ex);
            }
            catch (FaultException<EntityRepositoryFault> ex)
            {
                throw new RepositoryException(item, ex.Message, ex);
            }
            catch (FaultException<ApplicationConfigurationFault> ex)
            {
                throw new ApplicationConfigurationException(ex.Message, ex, ex.Detail.ConfigurationArea);
            }
            catch (FaultException<InternalOperationFault> ex)
            {
                throw new OperationException(item, ex.Message, ex);
            }
            catch (FaultException ex)
            {
                throw new OperationException(item, ex.Message, ex);
            }
            catch (CommunicationException ex)
            {
                // Handle the communication exception.
                ////this.client.Abort();
                throw new OperationException(item, ex.Message, ex);
            }
        }

        /// <summary>
        /// Executes the specified action, re-throwing fault exceptions as domain exceptions.
        /// </summary>
        /// <param name="item">
        /// The item the action is executed on.
        /// </param>
        /// <param name="action">
        /// The action to execute.
        /// </param>
        /// <typeparam name="TItem">
        /// The type of item the action is executed on.
        /// </typeparam>
        /// <typeparam name="TResult">
        /// The type of result produced by the action.
        /// </typeparam>
        /// <exception cref="BusinessException">
        /// A <see cref="FaultException"/> is thrown for a <see cref="EntityValidationFault"/>.
        /// </exception>
        /// <exception cref="RepositoryException">
        /// A <see cref="FaultException"/> is thrown for a <see cref="EntityRepositoryFault"/>.
        /// </exception>
        /// <exception cref="ApplicationConfigurationException">
        /// A <see cref="FaultException"/> is thrown for a <see cref="ApplicationConfigurationFault"/>.
        /// </exception>
        /// <exception cref="OperationException">
        /// A <see cref="FaultException"/> is thrown for a <see cref="InternalOperationFault"/>.
        /// </exception>
        /// <returns>
        /// The result of the action.
        /// </returns>
        public TResult ExecuteFor<TItem, TResult>(TItem item, Func<TItem, TResult> action)
        {
            if (action == null)
            {
                throw new ArgumentNullException("action");
            }

            ////if (this.opened == false)
            ////{
            ////    if (this.actionEventProxy != null)
            ////    {
            ////        this.actionEventProxy.RecordAction(this.clientStopwatch);
            ////    }

            ////    lock (this.openLock)
            ////    {
            ////        this.client.Open();
            ////        this.opened = true;
            ////    }

            ////    if (this.actionEventProxy != null)
            ////    {
            ////        this.actionEventProxy.RecordAction(this.clientStopwatch);
            ////    }
            ////}

            try
            {
                if (this.actionEventProxy != null)
                {
                    return this.actionEventProxy.PerformActionWithResult(item, action);
                }
                else
                {
                    return action(item);
                }
            }
            catch (TimeoutException ex)
            {
                // Handle the timeout exception.
                ////this.client.Abort();
                throw new OperationException(item, ex.Message, ex);
            }
            catch (FaultException<EntityValidationFault> ex)
            {
                throw new BusinessException(item, ex.Detail.ValidationErrors, ex);
            }
            catch (FaultException<EntityRepositoryFault> ex)
            {
                throw new RepositoryException(item, ex.Message, ex);
            }
            catch (FaultException<ApplicationConfigurationFault> ex)
            {
                throw new ApplicationConfigurationException(ex.Message, ex, ex.Detail.ConfigurationArea);
            }
            catch (FaultException<InternalOperationFault> ex)
            {
                throw new OperationException(item, ex.Message, ex);
            }
            catch (FaultException ex)
            {
                throw new OperationException(item, ex.Message, ex);
            }
            catch (CommunicationException ex)
            {
                // Handle the communication exception.
                ////this.client.Abort();
                throw new OperationException(item, ex.Message, ex);
            }
        }

        /// <summary>
        /// Executes the specified action, re-throwing fault exceptions as domain exceptions.
        /// </summary>
        /// <param name="action">
        /// The action to execute.
        /// </param>
        /// <typeparam name="TResult">
        /// The type of result produced by the action.
        /// </typeparam>
        /// <exception cref="BusinessException">
        /// A <see cref="FaultException"/> is thrown for a <see cref="EntityValidationFault"/>.
        /// </exception>
        /// <exception cref="RepositoryException">
        /// A <see cref="FaultException"/> is thrown for a <see cref="EntityRepositoryFault"/>.
        /// </exception>
        /// <exception cref="ApplicationConfigurationException">
        /// A <see cref="FaultException"/> is thrown for a <see cref="ApplicationConfigurationFault"/>.
        /// </exception>
        /// <exception cref="OperationException">
        /// A <see cref="FaultException"/> is thrown for a <see cref="InternalOperationFault"/>.
        /// </exception>
        /// <returns>
        /// The result of the action.
        /// </returns>
        public TResult ExecuteFor<TResult>(Func<TResult> action)
        {
            if (action == null)
            {
                throw new ArgumentNullException("action");
            }

            ////if (this.opened == false)
            ////{
            ////    if (this.actionEventProxy != null)
            ////    {
            ////        this.actionEventProxy.RecordAction(this.clientStopwatch);
            ////    }

            ////    lock (this.openLock)
            ////    {
            ////        this.client.Open();
            ////        this.opened = true;
            ////    }

            ////    if (this.actionEventProxy != null)
            ////    {
            ////        this.actionEventProxy.RecordAction(this.clientStopwatch);
            ////    }
            ////}

            try
            {
                if (this.actionEventProxy != null)
                {
                    this.actionEventProxy.RecordAction(this, action);
                }

                var result = action();
                if (this.actionEventProxy != null)
                {
                    this.actionEventProxy.RecordAction(this, action);
                }

                return result;
            }
            catch (TimeoutException ex)
            {
                // Handle the timeout exception.
                ////this.client.Abort();
                throw new OperationException(this, ex.Message, ex);
            }
            catch (FaultException<EntityValidationFault> ex)
            {
                throw new BusinessException(this, ex.Detail.ValidationErrors, ex);
            }
            catch (FaultException<EntityRepositoryFault> ex)
            {
                throw new RepositoryException(this, ex.Message, ex);
            }
            catch (FaultException<ApplicationConfigurationFault> ex)
            {
                throw new ApplicationConfigurationException(ex.Message, ex, ex.Detail.ConfigurationArea);
            }
            catch (FaultException<InternalOperationFault> ex)
            {
                throw new OperationException(this, ex.Message, ex);
            }
            catch (FaultException ex)
            {
                throw new OperationException(this, ex.Message, ex);
            }
            catch (CommunicationException ex)
            {
                // Handle the communication exception.
                ////this.client.Abort();
                throw new OperationException(this, ex.Message, ex);
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public void Dispose()
        {
            if (this.client == null)
            {
                return;
            }

            if (this.client.State == CommunicationState.Opened)
            {
                try
                {
                    this.client.Close();
                }
                catch (InvalidOperationException ex)
                {
                    Trace.TraceError(ErrorMessages.ClientProxyCouldNotBeClosed, typeof(TInterface).ToRuntimeName(), ex.Message);
                    this.client.Abort();
                }
                catch (CommunicationObjectFaultedException ex)
                {
                    Trace.TraceError(ErrorMessages.ClientProxyCouldNotBeClosed, typeof(TInterface).ToRuntimeName(), ex.Message);
                    this.client.Abort();
                }
                catch (CommunicationException ex)
                {
                    Trace.TraceError(ErrorMessages.ClientProxyCouldNotBeClosed, typeof(TInterface).ToRuntimeName(), ex.Message);
                    this.client.Abort();
                }
                catch (TimeoutException ex)
                {
                    Trace.TraceError(ErrorMessages.ClientProxyCouldNotBeClosed, typeof(TInterface).ToRuntimeName(), ex.Message);
                    this.client.Abort();                    
                }
            }
            else
            {
                // Some error has occurred but the factory is not already closing.
                this.client.Abort();
            }
        }

        #endregion
    }
}