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
    using Startitecture.Orm.Model;
    using Startitecture.Resources;

    /// <summary>
    /// A base class for entity repositories.
    /// </summary>
    /// <typeparam name="TModel">
    /// The type of domain model managed by the repository.
    /// </typeparam>
    /// <typeparam name="TEntity">
    /// The type of entity stored in the repository.
    /// </typeparam>
    /// <remarks>
    /// Uses the default memory cache if no cache is specified, with a save policy that does not expire items and a select policy with
    /// a sliding expiration of 30 seconds.
    /// </remarks>
    public class EntityRepository<TModel, TEntity> : ReadOnlyRepository<TModel, TEntity>, IEntityRepository<TModel>
        where TEntity : class, ITransactionContext, new()
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EntityRepository{TModel,TEntity}"/> class.
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
        /// Initializes a new instance of the <see cref="EntityRepository{TModel,TEntity}"/> class.
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
            IComparer<TEntity> selectionComparer)
            : base(repositoryProvider, entityMapper, selectionComparer)
        {
        }

        /// <inheritdoc />
        public TModel Save(TModel model)
        {
            if (Evaluate.IsNull(model))
            {
                throw new ArgumentNullException(nameof(model));
            }

            return this.SaveModel(model);
        }

        /// <inheritdoc />
        public int Delete<TItem>([NotNull] TItem example)
        {
            if (example == null)
            {
                throw new ArgumentNullException(nameof(example));
            }

            var entity = this.GetExampleEntity(example);
            var uniqueItemSelection = this.GetUniqueItemSelection(entity);
            return this.RepositoryProvider.DeleteItems(uniqueItemSelection);
        }

        /// <inheritdoc />
        public int Delete<TItem>([NotNull] ItemSelection<TItem> selection)
        {
            if (selection == null)
            {
                throw new ArgumentNullException(nameof(selection));
            }

            var itemSelection = selection.MapTo<TEntity>();
            return this.RepositoryProvider.DeleteItems(itemSelection);
        }

        #region Methods

        /// <summary>
        /// Saves a domain model in the repository.
        /// </summary>
        /// <param name="model">
        /// The model to save.
        /// </param>
        /// <returns>
        /// The saved domain model.
        /// </returns>
        private TModel SaveModel(TModel model)
        {
            var entity = this.SaveEntity(model);

            var entityDefinition = this.RepositoryProvider.EntityDefinitionProvider.Resolve<TEntity>();
            var key = entityDefinition.PrimaryKeyAttributes.First().GetValueDelegate.DynamicInvoke(entity);

            // Assume identical key name
            if (entityDefinition.AutoNumberPrimaryKey.HasValue)
            {
                var keyProperty = typeof(TModel).GetProperty(entityDefinition.AutoNumberPrimaryKey.Value.PropertyName);

                if (keyProperty == null)
                {
                    var message = string.Format(
                        CultureInfo.CurrentCulture,
                        ErrorMessages.MatchingKeyPropertyNotFound,
                        model,
                        typeof(TEntity),
                        entityDefinition.AutoNumberPrimaryKey.Value.PropertyName);

                    throw new OperationException(model, message);
                }

                try
                {
                    keyProperty.SetValue(model, key);
                }
                catch (ArgumentException ex)
                {
                    var message = string.Format(
                        CultureInfo.CurrentCulture,
                        ErrorMessages.UnableToSetPropertyToValue,
                        typeof(TModel),
                        keyProperty,
                        key,
                        ex.Message);

                    throw new OperationException(model, message, ex);
                }
                catch (TargetException ex)
                {
                    var message = string.Format(
                        CultureInfo.CurrentCulture,
                        ErrorMessages.UnableToSetPropertyToValue,
                        typeof(TModel),
                        keyProperty,
                        key,
                        ex.Message);

                    throw new OperationException(model, message, ex);
                }
                catch (MethodAccessException ex)
                {
                    var message = string.Format(
                        CultureInfo.CurrentCulture,
                        ErrorMessages.UnableToSetPropertyToValue,
                        typeof(TModel),
                        keyProperty,
                        key,
                        ex.Message);

                    throw new OperationException(model, message, ex);
                }
                catch (TargetInvocationException ex)
                {
                    var message = string.Format(
                        CultureInfo.CurrentCulture,
                        ErrorMessages.UnableToSetPropertyToValue,
                        typeof(TModel),
                        keyProperty,
                        key,
                        ex.Message);

                    throw new OperationException(model, message, ex);
                }
            }

            // Save the dependent elements of the entity.
            return model;
        }

        /// <summary>
        /// Saves a domain model to the repository as an entity.
        /// </summary>
        /// <param name="model">
        /// The model to save.
        /// </param>
        /// <returns>
        /// The saved data entity.
        /// </returns>
        private TEntity SaveEntity(TModel model)
        {
            var dataItem = new TEntity();
            dataItem.SetTransactionProvider(this.RepositoryProvider);
            this.EntityMapper.MapTo(model, dataItem);

            if (this.Contains(dataItem))
            {
                this.RepositoryProvider.Update(dataItem, this.GetUniqueItemSelection(dataItem));
            }
            else
            {
                dataItem = this.RepositoryProvider.InsertItem(dataItem);
            }

            if (dataItem == null)
            {
                throw new OperationException(model, $"The underlying provider returned a null entity when inserting '{model}'.");
            }

            if (object.ReferenceEquals(this.RepositoryProvider, dataItem.TransactionProvider) == false)
            {
                dataItem.SetTransactionProvider(this.RepositoryProvider);
            }

            return dataItem;
        }

        #endregion
    }
}