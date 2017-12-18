// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ItemOperation.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// <summary>
//   Defines a data operation on a specific item.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.Data
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;

    using SAF.Core;

    /// <summary>
    /// Defines a data operation on a specific item.
    /// </summary>
    /// <typeparam name="TItem">
    /// The type of item to perform the operation on.
    /// </typeparam>
    public abstract class ItemOperation<TItem>
    {
        #region Fields

        /// <summary>
        /// The value selector.
        /// </summary>
        private readonly Func<EntityAttributeInstance, object> valueSelector = parameter => parameter.Value;

        /// <summary>
        /// The name selector.
        /// </summary>
        private readonly Func<EntityAttributeDefinition, string> nameSelector = x => x.PropertyName;

        /// <summary>
        /// The operation parameters.
        /// </summary>
        private readonly List<EntityAttributeInstance> operationParameters = new List<EntityAttributeInstance>();

        /// <summary>
        /// The item definition.
        /// </summary>
        private readonly IEntityDefinition itemDefinition;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ItemOperation{TItem}"/> class.
        /// </summary>
        /// <param name="itemDefinition">
        /// The item definition.
        /// </param>
        /// <param name="item">
        /// The item to perform the operation on.
        /// </param>
        protected ItemOperation(IEntityDefinition itemDefinition, TItem item)
        {
            if (itemDefinition == null)
            {
                throw new ArgumentNullException("itemDefinition");
            }

            this.itemDefinition = itemDefinition;
            this.Item = item;
            this.OperationName = itemDefinition.EntityName;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the execution statement for the current operation.
        /// </summary>
        public abstract string ExecutionStatement { get; }

        /// <summary>
        /// Gets the parameter values for the current operation.
        /// </summary>
        public IEnumerable<object> ParameterValues
        {
            get
            {
                return this.OperationParameters.Select(this.valueSelector);
            }
        }

        /// <summary>
        /// Gets the parameters for the current operation.
        /// </summary>
        protected IEnumerable<EntityAttributeInstance> OperationParameters
        {
            get
            {
                return this.operationParameters.Count == 0
                           ? this.itemDefinition.DirectAttributes.Select(this.nameSelector).Select(this.CreateParameter)
                           : this.operationParameters;
            }
        }

        /// <summary>
        /// Gets the operation name.
        /// </summary>
        protected string OperationName { get; private set; }

        /// <summary>
        /// Gets the item that was provided for the operation.
        /// </summary>
        protected TItem Item { get; private set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// Applies the properties of the current item to a specific operation.
        /// </summary>
        /// <param name="operation">
        /// The operation name.
        /// </param>
        /// <returns>
        /// The current <see cref="T:SAF.Data.ItemOperation`1"/>.
        /// </returns>
        public ItemOperation<TItem> Using(string operation)
        {
            this.OperationName = operation;
            return this;
        }

        /// <summary>
        /// Specifies the parameters to use for the operation.
        /// </summary>
        /// <param name="parameters">
        /// The parameters to use.
        /// </param>
        /// <returns>
        /// The current <see cref="T:SAF.Data.ItemOperation`1"/>.
        /// </returns>
        public ItemOperation<TItem> WithParameters(params Expression<Func<TItem, object>>[] parameters)
        {
            this.operationParameters.AddRange(parameters.Select(this.CreateParameter));
            return this;
        }

        /// <summary>
        /// Specifies the parameters to use for the operation.
        /// </summary>
        /// <param name="parameters">
        /// The parameters to use.
        /// </param>
        /// <returns>
        /// The current <see cref="T:SAF.Data.ItemOperation`1"/>.
        /// </returns>
        public ItemOperation<TItem> WithParameters(params string[] parameters)
        {
            this.operationParameters.AddRange(parameters.Select(this.CreateParameter));
            return this;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Creates an operation parameter.
        /// </summary>
        /// <param name="selector">
        /// The selector to create the parameter with.
        /// </param>
        /// <returns>
        /// An <see cref="EntityAttributeInstance"/> based on the specified selector and the current item.
        /// </returns>
        private EntityAttributeInstance CreateParameter(Expression<Func<TItem, object>> selector)
        {
            return new EntityAttributeInstance(
                this.itemDefinition.Find(selector),
                selector.Compile().Invoke(this.Item));
        }

        /// <summary>
        /// Creates an operation parameter.
        /// </summary>
        /// <param name="parameterName">
        /// The parameter name.
        /// </param>
        /// <returns>
        /// An <see cref="EntityAttributeInstance"/> based on the specified selector and the current item.
        /// </returns>
        private EntityAttributeInstance CreateParameter(string parameterName)
        {
            return new EntityAttributeInstance(
                this.itemDefinition.Find(parameterName),
                ExtensionMethods.GetPropertyValue(this.Item, parameterName));
        }

        #endregion
    }
}
