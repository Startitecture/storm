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
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Threading;
    using System.Threading.Tasks;

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
        public EntityRepository(
            IRepositoryProvider repositoryProvider,
            IEntityMapper entityMapper)
            : base(repositoryProvider, entityMapper)
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
        public async Task<TModel> SaveAsync(TModel model, CancellationToken cancellationToken)
        {
            if (Evaluate.IsNull(model))
            {
                throw new ArgumentNullException(nameof(model));
            }

            var entity = await this.SaveEntityAsync(model, cancellationToken).ConfigureAwait(false);
            return this.UpdateModelIdentity(model, entity);
        }

        /// <inheritdoc />
        public int Update<TItem>(UpdateSet<TItem> updateSet)
        {
            if (updateSet == null)
            {
                throw new ArgumentNullException(nameof(updateSet));
            }

            return this.RepositoryProvider.Update(updateSet as UpdateSet<TEntity> ?? updateSet.MapSet<TEntity>());
        }

        /// <inheritdoc />
        public async Task<int> UpdateAsync<TItem>(UpdateSet<TItem> updateSet, CancellationToken cancellationToken)
        {
            if (updateSet == null)
            {
                throw new ArgumentNullException(nameof(updateSet));
            }

            return await this.RepositoryProvider.UpdateAsync(updateSet as UpdateSet<TEntity> ?? updateSet.MapSet<TEntity>(), cancellationToken)
                       .ConfigureAwait(false);
        }

        /// <inheritdoc />
        public void UpdateSingle<TKey, TItem>(
            TKey key,
            TItem source,
            params Expression<Func<TItem, object>>[] setExpressions)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (setExpressions == null)
            {
                throw new ArgumentNullException(nameof(setExpressions));
            }

            var updateSet = this.GetUniqueUpdateSet(key, source, setExpressions);
            this.RepositoryProvider.Update(updateSet);
        }

        /// <inheritdoc />
        public async Task UpdateSingleAsync<TKey, TItem>(
            TKey key,
            TItem source,
            CancellationToken cancellationToken,
            params Expression<Func<TItem, object>>[] setExpressions)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (setExpressions == null)
            {
                throw new ArgumentNullException(nameof(setExpressions));
            }

            var updateSet = this.GetUniqueUpdateSet(key, source, setExpressions);
            await this.RepositoryProvider.UpdateAsync(updateSet, cancellationToken).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public int Delete<TItem>(TItem example)
        {
            if (example == null)
            {
                throw new ArgumentNullException(nameof(example));
            }

            var entitySet = new EntitySet<TEntity>().Where(set => this.GetUniqueSet(example, set));
            return this.RepositoryProvider.Delete(entitySet);
        }

        /// <inheritdoc />
        public async Task<int> DeleteAsync<TItem>(TItem example, CancellationToken cancellationToken)
        {
            if (example == null)
            {
                throw new ArgumentNullException(nameof(example));
            }

            var entitySet = new EntitySet<TEntity>().Where(set => this.GetUniqueSet(example, set));
            return await this.RepositoryProvider.DeleteAsync(entitySet, cancellationToken).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public int DeleteEntities(Action<EntitySet<TModel>> defineSet)
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
        public async Task<int> DeleteEntitiesAsync(Action<EntitySet<TModel>> defineSet, CancellationToken cancellationToken)
        {
            if (defineSet == null)
            {
                throw new ArgumentNullException(nameof(defineSet));
            }

            var itemSet = new EntitySet<TModel>();
            defineSet.Invoke(itemSet);
            return await this.DeleteSelectionAsync(itemSet, cancellationToken).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public int DeleteSelection(IEntitySet entitySet)
        {
            if (entitySet == null)
            {
                throw new ArgumentNullException(nameof(entitySet));
            }

            return this.RepositoryProvider.Delete(entitySet as EntitySet<TEntity> ?? entitySet.MapSet<TEntity>());
        }

        /// <inheritdoc />
        public async Task<int> DeleteSelectionAsync(IEntitySet entitySet, CancellationToken cancellationToken)
        {
            if (entitySet == null)
            {
                throw new ArgumentNullException(nameof(entitySet));
            }

            return await this.RepositoryProvider.DeleteAsync(entitySet as EntitySet<TEntity> ?? entitySet.MapSet<TEntity>(), cancellationToken)
                       .ConfigureAwait(false);
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
        /// <param name="cancellationToken">
        /// The cancellation token for this task.
        /// </param>
        /// <returns>
        /// The saved data entity.
        /// </returns>
        private async Task<TEntity> SaveEntityAsync(TModel model, CancellationToken cancellationToken)
        {
            var entity = new TEntity();
            this.EntityMapper.MapTo(model, entity);

            if (await this.ContainsAsync(entity, cancellationToken).ConfigureAwait(false))
            {
                var updateSet = new UpdateSet<TEntity>()
                    .Set(entity, this.RepositoryProvider.EntityDefinitionProvider)
                    .Where(set => set.MatchKey(entity, this.RepositoryProvider.EntityDefinitionProvider));

                await this.RepositoryProvider.UpdateAsync(updateSet, cancellationToken).ConfigureAwait(false);
            }
            else
            {
                entity = await this.RepositoryProvider.InsertAsync(entity, cancellationToken).ConfigureAwait(false);
            }

            if (entity == null)
            {
                throw new OperationException(model, $"The underlying provider returned a null entity when inserting '{model}'.");
            }

            return entity;
        }

        /// <summary>
        /// Generates an update set for the specified <paramref name="key"/>, <paramref name="source"/>, and <paramref name="setExpressions"/>.
        /// </summary>
        /// <typeparam name="TKey">
        /// The type of key that identifies the entity to update.
        /// </typeparam>
        /// <typeparam name="TItem">
        /// The type of the item containing the source values to update.
        /// </typeparam>
        /// <param name="key">
        /// An item that uniquely identifies the entity.
        /// </param>
        /// <param name="source">
        /// An item that contains the source values to update in the target entity.
        /// </param>
        /// <param name="setExpressions">
        /// The expressions to limit the update to. If no expressions are specified, all updateable attributes are updated.
        /// </param>
        /// <returns>
        /// An <see cref="UpdateSet{T}"/> for the specified key, source and attributes to set.
        /// </returns>
        private UpdateSet<TEntity> GetUniqueUpdateSet<TKey, TItem>(
            TKey key,
            TItem source,
            IEnumerable<Expression<Func<TItem, object>>> setExpressions)
        {
            return new UpdateSet<TEntity>().Set(
                    this.GetExampleEntity(source).Item,
                    setExpressions.Select(
                            expr =>
                            {
                                var parameter = Expression.Parameter(typeof(TEntity), "value");
                                var expression = Expression.Property(parameter, expr.GetPropertyName());
                                var conversion = Expression.Convert(expression, typeof(object));
                                return Expression.Lambda<Func<TEntity, object>>(conversion, parameter);
                            })
                        .ToArray())
                .Where(set => this.GetUniqueSet(key, set));
        }
    }
}