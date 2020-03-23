// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SqlUpdate.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.Sql
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Linq.Expressions;

    using JetBrains.Annotations;

    using Model;

    using Startitecture.Core;
    using Startitecture.Orm.Query;

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
        private readonly IEntityDefinition itemDefinition;

        /// <summary>
        /// The selection.
        /// </summary>
        private readonly ItemSelection<TItem> selection;

        /// <summary>
        /// The query factory.
        /// </summary>
        private readonly TransactSqlQueryFactory queryFactory;

        /// <summary>
        /// The transact SQL join.
        /// </summary>
        private readonly TransactSqlJoin transactSqlJoin;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlUpdate{TItem}"/> class.
        /// </summary>
        /// <param name="definitionProvider">
        /// The definition provider.
        /// </param>
        /// <param name="selection">
        /// The selection of items to update.
        /// </param>
        public SqlUpdate([NotNull] IEntityDefinitionProvider definitionProvider, [NotNull] ItemSelection<TItem> selection)
        {
            if (definitionProvider == null)
            {
                throw new ArgumentNullException(nameof(definitionProvider));
            }

            if (selection == null)
            {
                throw new ArgumentNullException(nameof(selection));
            }

            this.selection = selection;
            this.queryFactory = new TransactSqlQueryFactory(definitionProvider);
            this.itemDefinition = definitionProvider.Resolve<TItem>();
            this.transactSqlJoin = new TransactSqlJoin(definitionProvider);
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the execution statement.
        /// </summary>
        public string ExecutionStatement => this.CreateUpdateStatement();

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
        /// The current <see cref="SqlUpdate{TItem}"/>.
        /// </returns>
        public SqlUpdate<TItem> Set(TItem item)
        {
            return this.Set(item, this.itemDefinition.UpdateableAttributes.ToArray());
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
        /// The current <see cref="SqlUpdate{TItem}"/>.
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
        /// The current <see cref="SqlUpdate{TItem}"/>.
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
        /// <returns>
        /// The update statement as a <see cref="string"/>.
        /// </returns>
        private string CreateUpdateStatement()
        {
            var setItems = new List<string>();
            int index = 0;

            foreach (var attributeInstance in this.attributesToSet)
            {
                var attributeDefinition = attributeInstance.AttributeDefinition;
                var qualifiedName = Singleton<TransactSqlQualifier>.Instance.Qualify(attributeDefinition);
                setItems.Add(
                    attributeInstance.Value == null
                        ? string.Format(CultureInfo.CurrentCulture, NullParameterFormat, qualifiedName)
                        : string.Format(CultureInfo.CurrentCulture, ParameterFormat, qualifiedName, index));

                if (attributeInstance.Value != null)
                {
                    index++;
                }
            }

            var entityName = this.itemDefinition.QualifiedName;
            string joinClause = this.selection.Relations.Any()
                                    ? string.Concat(
                                        Environment.NewLine,
                                        FromClause,
                                        entityName,
                                        Environment.NewLine,
                                        this.transactSqlJoin.Create(this.selection)) ////.Relations.CreateJoinClause())
                                    : string.Empty;

            var setClause = string.Join(string.Concat(',', Environment.NewLine), setItems);
            var predicateClause = this.queryFactory.Create(new QueryContext<TItem>(this.selection, StatementOutputType.Update, index));

            return string.Concat(
                string.Format(CultureInfo.InvariantCulture, SqlUpdateClause, entityName, setClause),
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