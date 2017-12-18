// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EntityOperationContext.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.EntityServices
{
    using System.Security.Principal;

    using SAF.ActionTracking;
    using SAF.Core;
    using SAF.Security;

    /// <summary>
    /// The entity operation context.
    /// </summary>
    public class EntityOperationContext : IActionContext
    {
        /// <summary>
        /// The current context.
        /// </summary>
        private static readonly EntityOperationContext CurrentContext = new EntityOperationContext();

        /// <summary>
        /// The operation context.
        /// </summary>
        private readonly ApplicationOperationContext operationContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="EntityOperationContext"/> class.
        /// </summary>
        /// <param name="accountInfoProvider">
        /// The account info provider.
        /// </param>
        public EntityOperationContext(IAccountInfoProvider accountInfoProvider)
        {
            this.operationContext = new ApplicationOperationContext(
                accountInfoProvider,
                typeof(EntityProxy<>),
                typeof(ExtensionMethods),
                typeof(EntityErrorHandler),
                typeof(EntityOperationContext));
        }

        /// <summary>
        /// Prevents a default instance of the <see cref="EntityOperationContext"/> class from being created. 
        /// </summary>
        private EntityOperationContext()
            : this(new WindowsAccountInfoProvider())
        {
        }

        /// <summary>
        /// Gets the current entity context.
        /// </summary>
        public static EntityOperationContext Current
        {
            get
            {
                return CurrentContext;
            }
        }

        /// <summary>
        /// Gets the current action.
        /// </summary>
        public string CurrentAction
        {
            get
            {
                return this.operationContext.CurrentAction;
            }
        }

        /// <summary>
        /// Gets the current action source.
        /// </summary>
        public string CurrentActionSource
        {
            get
            {
                return this.operationContext.CurrentActionSource;
            }
        }

        /// <summary>
        /// Gets the endpoint at which the action is executing.
        /// </summary>
        public string Endpoint
        {
            get
            {
                return this.operationContext.Endpoint;
            }
        }

        /// <summary>
        /// Gets the host on which the action is executing.
        /// </summary>
        public string Host
        {
            get
            {
                return this.operationContext.Host;
            }
        }

        /// <summary>
        /// Gets the identity for the current context.
        /// </summary>
        public IIdentity CurrentIdentity
        {
            get
            {
                return this.operationContext.CurrentIdentity;
            }
        }

        /// <summary>
        /// Gets the current account.
        /// </summary>
        public IAccountInfo CurrentAccount
        {
            get
            {
                return this.operationContext.CurrentAccount;
            }
        }
    }
}
