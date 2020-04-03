// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EntityRepository.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.Common
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Reflection;

    using JetBrains.Annotations;

    using Startitecture.Core;
    using Startitecture.Orm.Query;
    using Startitecture.Resources;

    /// <summary>
    /// A base class for entity repositories.
    /// </summary>
    /// <typeparam name="TEntity">
    /// The type of entity stored in the repository.
    /// </typeparam>
    /// <typeparam name="TDataItem">
    /// The type of item that represents the entity in the repository.
    /// </typeparam>
    /// <remarks>
    /// Uses the default memory cache if no cache is specified, with a save policy that does not expire items and a select policy with
    /// a sliding expiration of 30 seconds.
    /// </remarks>
    public class EntityRepository<TEntity, TDataItem> : ReadOnlyRepository<TEntity, TDataItem>, IEntityRepository<TEntity>
        where TDataItem : class, ITransactionContext, new()
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EntityRepository{TEntity,TDataItem}"/> class.
        /// </summary>
        /// <param name="repositoryProvider">
        /// The repository provider for this repository.
        /// </param>
        /// <param name="entityMapper">
        /// The entity mapper.
        /// </param>
        public EntityRepository(IRepositoryProvider repositoryProvider, IEntityMapper entityMapper)
            : this(repositoryProvider, entityMapper, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EntityRepository{TEntity,TDataItem}"/> class.
        /// </summary>
        /// <param name="repositoryProvider">
        /// The repository provider for this repository.
        /// </param>
        /// <param name="entityMapper">
        /// The entity mapper.
        /// </param>
        /// <param name="selectionComparer">
        /// The selection comparer for ordering data items from the repository after being selected from the database.
        /// </param>
        public EntityRepository(
            IRepositoryProvider repositoryProvider,
            IEntityMapper entityMapper,
            IComparer<TDataItem> selectionComparer)
            : base(repositoryProvider, entityMapper, selectionComparer)
        {
        }

        /// <inheritdoc />
        public TEntity Save(TEntity item)
        {
            if (Evaluate.IsNull(item))
            {
                throw new ArgumentNullException(nameof(item));
            }

            return this.SaveEntity(item);
        }

        /// <inheritdoc />
        public TEntity Save<TItem>(TItem item)
        {
            if (Evaluate.IsNull(item))
            {
                throw new ArgumentNullException(nameof(item));
            }

            var entity = this.EntityMapper.Map<TEntity>(item);
            var savedEntity = this.SaveEntity(entity);
            return savedEntity;
        }

        /// <inheritdoc />
        public int Delete<TItem>([NotNull] TItem example)
        {
            if (example == null)
            {
                throw new ArgumentNullException(nameof(example));
            }

            var dataItem = this.GetExampleItem(example);
            var uniqueItemSelection = this.GetUniqueItemSelection(dataItem);
            return this.RepositoryProvider.DeleteItems(uniqueItemSelection);
        }

        /// <inheritdoc />
        public int Delete<TItem>([NotNull] ItemSelection<TItem> selection)
        {
            if (selection == null)
            {
                throw new ArgumentNullException(nameof(selection));
            }

            var itemSelection = selection.MapTo<TDataItem>();
            return this.RepositoryProvider.DeleteItems(itemSelection);
        }

        #region Methods

        /// <summary>
        /// Saves an item to the database.
        /// </summary>
        /// <param name="entity">
        /// The item to save.
        /// </param>
        /// <returns>
        /// The saved data item.
        /// </returns>
        private TDataItem SaveDataItem(TEntity entity)
        {
            var dataItem = new TDataItem();
            dataItem.SetTransactionProvider(this.RepositoryProvider);
            this.EntityMapper.MapTo(entity, dataItem);

            dataItem = this.RepositoryProvider.Save(dataItem);

            // In an update operation, dataItem will be a different object reference blended with the original.
            dataItem.SetTransactionProvider(this.RepositoryProvider);
            return dataItem;
        }

        /// <summary>
        /// Saves the entity.
        /// </summary>
        /// <param name="item">
        /// The item to save.
        /// </param>
        /// <returns>
        /// The saved entity.
        /// </returns>
        private TEntity SaveEntity(TEntity item)
        {
            var dataItem = this.SaveDataItem(item);

            var entityDefinition = this.RepositoryProvider.EntityDefinitionProvider.Resolve<TDataItem>();
            var key = entityDefinition.PrimaryKeyAttributes.First().GetValueDelegate.DynamicInvoke(dataItem);

            // Assume identical key name
            if (entityDefinition.AutoNumberPrimaryKey.HasValue)
            {
                var keyProperty = typeof(TEntity).GetProperty(entityDefinition.AutoNumberPrimaryKey.Value.PropertyName);

                if (keyProperty == null)
                {
                    var message = string.Format(
                        CultureInfo.CurrentCulture,
                        ErrorMessages.MatchingKeyPropertyNotFound,
                        item,
                        typeof(TDataItem),
                        entityDefinition.AutoNumberPrimaryKey.Value.PropertyName);

                    throw new OperationException(item, message);
                }

                try
                {
                    keyProperty.SetValue(item, key);
                }
                catch (ArgumentException ex)
                {
                    var message = string.Format(
                        CultureInfo.CurrentCulture,
                        ErrorMessages.UnableToSetPropertyToValue,
                        typeof(TEntity),
                        keyProperty,
                        key,
                        ex.Message);

                    throw new OperationException(item, message, ex);
                }
                catch (TargetException ex)
                {
                    var message = string.Format(
                        CultureInfo.CurrentCulture,
                        ErrorMessages.UnableToSetPropertyToValue,
                        typeof(TEntity),
                        keyProperty,
                        key,
                        ex.Message);

                    throw new OperationException(item, message, ex);
                }
                catch (MethodAccessException ex)
                {
                    var message = string.Format(
                        CultureInfo.CurrentCulture,
                        ErrorMessages.UnableToSetPropertyToValue,
                        typeof(TEntity),
                        keyProperty,
                        key,
                        ex.Message);

                    throw new OperationException(item, message, ex);
                }
                catch (TargetInvocationException ex)
                {
                    var message = string.Format(
                        CultureInfo.CurrentCulture,
                        ErrorMessages.UnableToSetPropertyToValue,
                        typeof(TEntity),
                        keyProperty,
                        key,
                        ex.Message);

                    throw new OperationException(item, message, ex);
                }
            }

            // Save the dependent elements of the entity.
            return item;
        }

        #endregion
    }
}