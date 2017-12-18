// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UpdateOperation.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.Data.Providers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;

    using SAF.Core;

    /// <summary>
    /// Creates an update statement for a specific item in the repository.
    /// </summary>
    /// <typeparam name="TItem">
    /// The type of item that is being updated.
    /// </typeparam>
    public class UpdateOperation<TItem>
    {
        #region Constants

        /// <summary>
        /// The SQL UPDATE clause.
        /// </summary>
        private const string SqlUpdateClause = @"UPDATE {0}
SET
{1}";

        /// <summary>
        /// The SQL parameter format.
        /// </summary>
        private const string ParameterFormat = "{0} = @{1}";

        /// <summary>
        /// The null value.
        /// </summary>
        private const string NullParameterFormat = "{0} = NULL";

        /// <summary>
        /// The from clause.
        /// </summary>
        private const string FromClause = "FROM ";

        /// <summary>
        /// The where clause.
        /// </summary>
        private const string WhereClause = "WHERE";

        #endregion

        #region Fields

        /// <summary>
        /// The attributes to set.
        /// </summary>
        private readonly List<EntityAttributeInstance> attributesToSet = new List<EntityAttributeInstance>();

        /// <summary>
        /// The item definition.
        /// </summary>
        private readonly IEntityDefinition itemDefinition = DataItemDefinition<TItem>.Compiled;

        /// <summary>
        /// The selection.
        /// </summary>
        private readonly ItemSelection<TItem> selection;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateOperation{TItem}"/> class.
        /// </summary>
        /// <param name="selection">
        /// The selection of items to update.
        /// </param>
        public UpdateOperation(ItemSelection<TItem> selection)
        {
            this.selection = selection;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the execution statement.
        /// </summary>
        public string ExecutionStatement
        {
            get
            {
                return CreateUpdateStatement(this);
            }
        }

        /// <summary>
        /// Gets the execution parameters for the update query.
        /// </summary>
        public IEnumerable<object> ExecutionParameters
        {
            get
            {
                var parameters =
                    new List<object>(
                        this.attributesToSet.Where(EntityAttributeInstance.AttributeNotNullPredicate)
                            .Select(EntityAttributeInstance.SelectAttributeValue));

                parameters.AddRange(this.selection.PropertyValues);
                return parameters;
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// Sets the values to update as part of the operation.
        /// </summary>
        /// <param name="item">
        /// The item containing the target values.
        /// </param>
        /// <returns>
        /// The current <see cref="T:SAF.Data.Providers.UpdateOperation`1"/>.
        /// </returns>
        public UpdateOperation<TItem> Set(TItem item)
        {
            return this.Set(item, this.selection.ItemDefinition.UpdateableAttributes.ToArray());
        }

        /// <summary>
        /// Sets the values to update as part of the operation.
        /// </summary>
        /// <param name="item">
        /// The item containing the target values.
        /// </param>
        /// <param name="attributes">
        /// The attributes to update.
        /// </param>
        /// <returns>
        /// The current <see cref="T:SAF.Data.Providers.UpdateOperation`1"/>.
        /// </returns>
        public UpdateOperation<TItem> Set(TItem item, params EntityAttributeDefinition[] attributes)
        {
            this.attributesToSet.Clear();
            this.attributesToSet.AddRange(attributes.Select(x => CreateAttributeInstance(item, x)));
            return this;
        }

        /// <summary>
        /// Sets the values to update as part of the operation.
        /// </summary>
        /// <param name="item">
        /// The item containing the target values.
        /// </param>
        /// <param name="properties">
        /// The properties to update.
        /// </param>
        /// <returns>
        /// The current <see cref="T:SAF.Data.Providers.UpdateOperation`1"/>.
        /// </returns>
        public UpdateOperation<TItem> Set(TItem item, params Expression<Func<TItem, object>>[] properties)
        {
            this.attributesToSet.Clear();
            this.attributesToSet.AddRange(properties.Select(x => CreateAttributeInstance(item, x)));
            return this;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Creates the update statement for the current operation.
        /// </summary>
        /// <param name="operation">
        /// The operation to perform.
        /// </param>
        /// <returns>
        /// The update statement as a <see cref="string"/>.
        /// </returns>
        private static string CreateUpdateStatement(UpdateOperation<TItem> operation)
        {
            var setItems = new List<string>();
            int index = 0;

            foreach (var attributeInstance in operation.attributesToSet)
            {
                setItems.Add(attributeInstance.Value == null
                                 ? String.Format(NullParameterFormat, attributeInstance.AttributeDefinition.QualifiedName)
                                 : String.Format(ParameterFormat, attributeInstance.AttributeDefinition.QualifiedName, index));

                if (attributeInstance.Value != null)
                {
                    index++;
                }
            }

            var itemSelection = operation.selection;

            string joinClause = itemSelection.Relations.Any()
                                    ? String.Concat(
                                        Environment.NewLine,
                                        FromClause,
                                        operation.itemDefinition.EntityName,
                                        Environment.NewLine,
                                        itemSelection.Relations.CreateJoinClause())
                                    : String.Empty;

            var setClause = String.Join(String.Concat(',', Environment.NewLine), setItems);
            var predicateClause = itemSelection.Filters.CreateFilter(index);
            return String.Concat(
                String.Format(SqlUpdateClause, operation.itemDefinition.EntityName, setClause),
                joinClause,
                Environment.NewLine,
                WhereClause,
                Environment.NewLine,
                predicateClause);
        }

        /// <summary>
        /// The create attribute instance.
        /// </summary>
        /// <param name="item">
        /// The item.
        /// </param>
        /// <param name="attribute">
        /// The attribute.
        /// </param>
        /// <returns>
        /// The <see cref="EntityAttributeInstance"/>.
        /// </returns>
        private static EntityAttributeInstance CreateAttributeInstance(TItem item, EntityAttributeDefinition attribute)
        {
            return new EntityAttributeInstance(attribute, Normalize.GetPropertyValue(attribute.Name, item));
        }

        /// <summary>
        /// The create attribute instance.
        /// </summary>
        /// <param name="item">
        /// The item.
        /// </param>
        /// <param name="property">
        /// The property.
        /// </param>
        /// <returns>
        /// The <see cref="EntityAttributeInstance"/>.
        /// </returns>
        private EntityAttributeInstance CreateAttributeInstance(TItem item, Expression<Func<TItem, object>> property)
        {
            return new EntityAttributeInstance(this.itemDefinition.Find(property.GetPropertyName()), property.Compile().Invoke(item));
        }

        #endregion
    }
}