// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SqlUpdate.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.Sql
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;

    using Model;
    using Model;

    using Startitecture.Core;
    using Startitecture.Orm.Query;
    using Startitecture.Orm.Schema;

    /// <summary>
    /// Creates an update statement for a specific item in the repository.
    /// </summary>
    /// <typeparam name="TItem">
    /// The type of item that is being updated.
    /// </typeparam>
    public class SqlUpdate<TItem>
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
        private readonly IEntityDefinition itemDefinition = Singleton<PetaPocoDefinitionProvider>.Instance.Resolve<TItem>();

        /// <summary>
        /// The selection.
        /// </summary>
        private readonly ItemSelection<TItem> selection;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlUpdate{TItem}"/> class.
        /// </summary>
        /// <param name="selection">
        /// The selection of items to update.
        /// </param>
        public SqlUpdate(ItemSelection<TItem> selection)
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
                return this.CreateUpdateStatement(this);
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

        /// <summary>
        /// Gets or sets a value indicating whether null properties require a set value.
        /// </summary>
        public bool NullPropertiesRequireSetValue { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// Sets the values to update as part of the operation.
        /// </summary>
        /// <param name="item">
        /// The item containing the target values.
        /// </param>
        /// <returns>
        /// The current <see cref="T:Startitecture.Orm.Sql.SqlUpdate`1"/>.
        /// </returns>
        public SqlUpdate<TItem> Set(TItem item)
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
        /// The current <see cref="T:Startitecture.Orm.Sql.SqlUpdate`1"/>.
        /// </returns>
        public SqlUpdate<TItem> Set(TItem item, params EntityAttributeDefinition[] attributes)
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
        /// The current <see cref="T:Startitecture.Orm.Sql.SqlUpdate`1"/>.
        /// </returns>
        public SqlUpdate<TItem> Set(TItem item, params Expression<Func<TItem, object>>[] properties)
        {
            this.attributesToSet.Clear();
            this.attributesToSet.AddRange(properties.Select(x => this.CreateAttributeInstance(item, x)));
            return this;
        }

        #endregion

        #region Methods

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
        /// The <see cref="Startitecture.Orm.Model.EntityAttributeInstance"/>.
        /// </returns>
        private static EntityAttributeInstance CreateAttributeInstance(TItem item, EntityAttributeDefinition attribute)
        {
            return new EntityAttributeInstance(attribute, item.GetPropertyValue(attribute.PropertyName));
        }

        /// <summary>
        /// Creates the update statement for the current operation.
        /// </summary>
        /// <param name="operation">
        /// The operation to perform.
        /// </param>
        /// <returns>
        /// The update statement as a <see cref="string"/>.
        /// </returns>
        private string CreateUpdateStatement(SqlUpdate<TItem> operation)
        {
            var setItems = new List<string>();
            int index = 0;

            foreach (var attributeInstance in operation.attributesToSet)
            {
                setItems.Add(attributeInstance.Value == null
                                 ? string.Format((string)NullParameterFormat, (object)attributeInstance.AttributeDefinition.GetQualifiedName())
                                 : string.Format(ParameterFormat, attributeInstance.AttributeDefinition.GetQualifiedName(), index));

                if (attributeInstance.Value != null)
                {
                    index++;
                }
            }

            var itemSelection = operation.selection;

            var entityName = operation.itemDefinition.QualifiedName;
            string joinClause = itemSelection.Relations.Any()
                                    ? string.Concat(
                                        Environment.NewLine,
                                        FromClause,
                                        entityName,
                                        Environment.NewLine,
                                        itemSelection.Relations.CreateJoinClause())
                                    : string.Empty;

            var setClause = string.Join(string.Concat(',', Environment.NewLine), setItems);
            var predicateClause = itemSelection.Filters.CreateFilter(index, this.NullPropertiesRequireSetValue);
            return string.Concat(
                string.Format(SqlUpdateClause, entityName, setClause),
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
        /// <param name="property">
        /// The property.
        /// </param>
        /// <returns>
        /// The <see cref="Startitecture.Orm.Model.EntityAttributeInstance"/>.
        /// </returns>
        private EntityAttributeInstance CreateAttributeInstance(TItem item, Expression<Func<TItem, object>> property)
        {
            return new EntityAttributeInstance(this.itemDefinition.Find(property), property.Compile().Invoke(item));
        }

        #endregion
    }
}