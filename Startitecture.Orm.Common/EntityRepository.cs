// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EntityRepository.cs" company="Startitecture">
//   Copyright (c) Startitecture. All rights reserved.
// </copyright>
// <summary>
//   A base class for entity repositories.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.Common
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Reflection;
    using System.Threading.Tasks;

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
    /// Uses the default memory cache if no cache is specified, with a save policy that does not expire entities and a select policy with
    /// a sliding expiration of 30 seconds.
    /// </remarks>
    public class EntityRepository<TModel, TEntity> : ReadOnlyRepository<TModel, TEntity>, IEntityRepository<TModel>
        where TEntity : class, new()
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
        /// The set comparer for ordering data entities from the repository after being selected from the database.
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

            var entity = this.SaveEntity(model);
            return this.UpdateModelIdentity(model, entity);
        }

        /// <inheritdoc />
        public async Task<TModel> SaveAsync(TModel model)
        {
            if (Evaluate.IsNull(model))
            {
                throw new ArgumentNullException(nameof(model));
            }

            var entity = await this.SaveEntityAsync(model).ConfigureAwait(false);
            return this.UpdateModelIdentity(model, entity);
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
            return this.RepositoryProvider.Delete(uniqueItemSelection);
        }

        /// <inheritdoc />
        public async Task<int> DeleteAsync<TItem>(TItem example)
        {
            if (example == null)
            {
                throw new ArgumentNullException(nameof(example));
            }

            var entity = this.GetExampleEntity(example);
            var uniqueItemSelection = this.GetUniqueItemSelection(entity);
            return await this.RepositoryProvider.DeleteAsync(uniqueItemSelection).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public int DeleteEntities([NotNull] Action<EntitySet<TModel>> defineSet)
        {
            if (defineSet == null)
            {
                throw new ArgumentNullException(nameof(defineSet));
            }

            var itemSet = new EntitySet<TModel>();
            defineSet.Invoke(itemSet);
            return this.DeleteSelection(itemSet);
        }

        /// <inheritdoc />
        public async Task<int> DeleteEntitiesAsync(Action<EntitySet<TModel>> defineSet)
        {
            if (defineSet == null)
            {
                throw new ArgumentNullException(nameof(defineSet));
            }

            var itemSet = new EntitySet<TModel>();
            defineSet.Invoke(itemSet);
            return await this.DeleteSelectionAsync(itemSet).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public int DeleteSelection([NotNull] IEntitySet entitySet)
        {
            if (entitySet == null)
            {
                throw new ArgumentNullException(nameof(entitySet));
            }

            var targetSet = entitySet.MapSet<TEntity>();
            return this.RepositoryProvider.Delete(targetSet);
        }

        /// <inheritdoc />
        public async Task<int> DeleteSelectionAsync(IEntitySet entitySet)
        {
            if (entitySet == null)
            {
                throw new ArgumentNullException(nameof(entitySet));
            }

            var targetSet = entitySet.MapSet<TEntity>();
            return await this.RepositoryProvider.DeleteAsync(targetSet).ConfigureAwait(false);
        }

        /// <summary>
        /// Gets the model's identity (auto number, sequence, etc.) property, if it has one.
        /// </summary>
        /// <returns>
        /// The <see cref="PropertyInfo"/> of the model's identity property.
        /// </returns>
        /// <remarks>
        /// This method is only called when the underlying <typeparamref name="TEntity"/> has an identified identity attribute, such as an identity
        /// column or sequence column. The default action is to match property names between <typeparamref name="TEntity"/> and
        /// <typeparamref name="TModel"/>; if this will not work, then implementers should override this method.
        /// </remarks>
        protected virtual PropertyInfo GetModelIdentityProperty()
        {
            var entityDefinition = this.RepositoryProvider.EntityDefinitionProvider.Resolve<TEntity>();
            var propertyName = entityDefinition.RowIdentity?.PropertyName;
            return string.IsNullOrWhiteSpace(propertyName) ? null : typeof(TModel).GetProperty(propertyName);
        }

        /// <summary>
        /// Saves a domain model in the repository.
        /// </summary>
        /// <param name="model">
        /// The model to update the identity for.
        /// </param>
        /// <param name="entity">
        /// The saved entity.
        /// </param>
        /// <returns>
        /// The saved domain model.
        /// </returns>
        private TModel UpdateModelIdentity(TModel model, TEntity entity)
        {
            var entityDefinition = this.RepositoryProvider.EntityDefinitionProvider.Resolve<TEntity>();

            // Assume identical key name
            if (entityDefinition.RowIdentity.HasValue == false)
            {
                return model;
            }

            var key = entityDefinition.RowIdentity.Value.GetValueDelegate.DynamicInvoke(entity);
            var keyProperty = this.GetModelIdentityProperty();

            if (keyProperty == null)
            {
                // TODO: Reformat this message to be more specific to not finding a property at all.
                var message = string.Format(
                    CultureInfo.CurrentCulture,
                    ErrorMessages.MatchingKeyPropertyNotFound,
                    model,
                    typeof(TEntity),
                    entityDefinition.RowIdentity.Value.PropertyName);

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
            var entity = new TEntity();
            this.EntityMapper.MapTo(model, entity);

            if (this.Contains(entity))
            {
                var updateSet = new UpdateSet<TEntity>()
                    .Set(entity, this.RepositoryProvider.EntityDefinitionProvider)
                    .Where(set => set.MatchKey(entity, this.RepositoryProvider.EntityDefinitionProvider));

                this.RepositoryProvider.Update(updateSet);
            }
            else
            {
                entity = this.RepositoryProvider.Insert(entity);
            }

            if (entity == null)
            {
                throw new OperationException(model, $"The underlying provider returned a null entity when inserting '{model}'.");
            }

            return entity;
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
        private async Task<TEntity> SaveEntityAsync(TModel model)
        {
            var entity = new TEntity();
            this.EntityMapper.MapTo(model, entity);

            if (await this.ContainsAsync(entity).ConfigureAwait(false))
            {
                var updateSet = new UpdateSet<TEntity>()
                    .Set(entity, this.RepositoryProvider.EntityDefinitionProvider)
                    .Where(set => set.MatchKey(entity, this.RepositoryProvider.EntityDefinitionProvider));

                await this.RepositoryProvider.UpdateAsync(updateSet).ConfigureAwait(false);
            }
            else
            {
                entity = await this.RepositoryProvider.InsertAsync(entity).ConfigureAwait(false);
            }

            if (entity == null)
            {
                throw new OperationException(model, $"The underlying provider returned a null entity when inserting '{model}'.");
            }

            return entity;
        }
    }
}