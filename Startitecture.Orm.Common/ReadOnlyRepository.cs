// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ReadOnlyRepository.cs" company="Startitecture">
//   Copyright (c) Startitecture. All rights reserved.
// </copyright>
// <summary>
//   The view repository.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.Common
{
    using System;
    using System.Collections.Generic;
    using System.Dynamic;
    using System.Globalization;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using System.Threading;
    using System.Threading.Tasks;

    using JetBrains.Annotations;

    using Startitecture.Core;
    using Startitecture.Orm.Model;
    using Startitecture.Resources;

    /// <summary>
    /// The view repository.
    /// </summary>
    /// <typeparam name="TModel">
    /// The type of domain model managed by the repository.
    /// </typeparam>
    /// <typeparam name="TEntity">
    /// The type of entity stored in the repository.
    /// </typeparam>
    public class ReadOnlyRepository<TModel, TEntity> : IReadOnlyRepository<TModel>
        where TEntity : class, new()
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ReadOnlyRepository{TModel,TEntity}"/> class.
        /// </summary>
        /// <param name="repositoryProvider">
        /// The repository provider for this repository.
        /// </param>
        /// <param name="entityMapper">
        /// The entity mapper.
        /// </param>
        public ReadOnlyRepository(
            [NotNull] IRepositoryProvider repositoryProvider,
            [NotNull] IEntityMapper entityMapper)
        {
            // The entity mapper, and its resolution context, is intended to last only for the lifetime of the repository.
            this.EntityMapper = entityMapper ?? throw new ArgumentNullException(nameof(entityMapper));
            this.RepositoryProvider = repositoryProvider ?? throw new ArgumentNullException(nameof(repositoryProvider));
        }

        /// <summary>
        /// Gets the repository provider.
        /// </summary>
        protected IRepositoryProvider RepositoryProvider { get; }

        /// <summary>
        /// Gets the entity mapper.
        /// </summary>
        protected IEntityMapper EntityMapper { get; }

        /// <inheritdoc />
        public bool Contains<TItem>(TItem candidate)
        {
            if (Evaluate.IsNull(candidate))
            {
                throw new ArgumentNullException(nameof(candidate));
            }

            var entitySet = new EntitySet<TEntity>().Where(set => this.GetUniqueSet(candidate, set));
            return this.RepositoryProvider.Contains(entitySet);
        }

        /// <inheritdoc />
        public async Task<bool> ContainsAsync<TItem>(TItem candidate)
        {
            return await this.ContainsAsync(candidate, CancellationToken.None).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<bool> ContainsAsync<TItem>(TItem candidate, CancellationToken cancellationToken)
        {
            if (Evaluate.IsNull(candidate))
            {
                throw new ArgumentNullException(nameof(candidate));
            }

            var entitySet = new EntitySet<TEntity>().Where(set => this.GetUniqueSet(candidate, set));
            return await this.RepositoryProvider.ContainsAsync(entitySet, cancellationToken).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public bool Contains<TItem>([NotNull] EntitySet<TItem> selection)
        {
            if (selection == null)
            {
                throw new ArgumentNullException(nameof(selection));
            }

            return this.RepositoryProvider.Contains(selection as EntitySet<TEntity> ?? selection.MapSet<TEntity>());
        }

        /// <inheritdoc />
        public async Task<bool> ContainsAsync<TItem>(EntitySet<TItem> selection)
        {
            return await this.ContainsAsync(selection, CancellationToken.None).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<bool> ContainsAsync<TItem>(EntitySet<TItem> selection, CancellationToken cancellationToken)
        {
            if (selection == null)
            {
                throw new ArgumentNullException(nameof(selection));
            }

            return await this.RepositoryProvider.ContainsAsync(selection as EntitySet<TEntity> ?? selection.MapSet<TEntity>(), cancellationToken)
                       .ConfigureAwait(false);
        }

        /// <inheritdoc />
        public TModel FirstOrDefault<TItem>(TItem candidate)
        {
            if (Evaluate.IsNull(candidate))
            {
                throw new ArgumentNullException(nameof(candidate));
            }

            var entitySet = new EntitySet<TEntity>().Where(set => this.GetUniqueSet(candidate, set));
            entitySet.SetDefaultRelations(this.RepositoryProvider.EntityDefinitionProvider);

            var dataItem = this.RepositoryProvider.FirstOrDefault(entitySet);

            if (Evaluate.IsNull(dataItem))
            {
                return default;
            }

            var entity = this.ConstructModel(dataItem);
            return entity;
        }

        /// <inheritdoc />
        public async Task<TModel> FirstOrDefaultAsync<TItem>(TItem candidate)
        {
            return await this.FirstOrDefaultAsync(candidate, CancellationToken.None).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<TModel> FirstOrDefaultAsync<TItem>(TItem candidate, CancellationToken cancellationToken)
        {
            if (Evaluate.IsNull(candidate))
            {
                throw new ArgumentNullException(nameof(candidate));
            }

            var entitySet = new EntitySet<TEntity>().Where(set => this.GetUniqueSet(candidate, set));
            entitySet.SetDefaultRelations(this.RepositoryProvider.EntityDefinitionProvider);

            var dataItem = await this.RepositoryProvider.FirstOrDefaultAsync(entitySet, cancellationToken).ConfigureAwait(false);

            if (Evaluate.IsNull(dataItem))
            {
                return default;
            }

            var entity = this.ConstructModel(dataItem);
            return entity;
        }

        /// <inheritdoc />
        public TModel FirstOrDefault<TItem>([NotNull] EntitySet<TItem> selection)
        {
            if (selection == null)
            {
                throw new ArgumentNullException(nameof(selection));
            }

            var entity = this.RepositoryProvider.FirstOrDefault(selection as EntitySet<TEntity> ?? selection.MapSet<TEntity>());
            return entity != null ? this.EntityMapper.Map<TModel>(entity) : default;
        }

        /// <inheritdoc />
        public async Task<TModel> FirstOrDefaultAsync<TItem>(EntitySet<TItem> selection)
        {
            return await this.FirstOrDefaultAsync(selection, CancellationToken.None).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<TModel> FirstOrDefaultAsync<TItem>(EntitySet<TItem> selection, CancellationToken cancellationToken)
        {
            if (selection == null)
            {
                throw new ArgumentNullException(nameof(selection));
            }

            var entity = await this.RepositoryProvider
                             .FirstOrDefaultAsync(selection as EntitySet<TEntity> ?? selection.MapSet<TEntity>(), cancellationToken)
                             .ConfigureAwait(false);

            return entity != null ? this.EntityMapper.Map<TModel>(entity) : default;
        }

        /// <inheritdoc />
        public TModel FirstOrDefault([NotNull] Action<EntitySet<TModel>> defineSet)
        {
            if (defineSet == null)
            {
                throw new ArgumentNullException(nameof(defineSet));
            }

            var modelSet = new EntitySet<TModel>();
            defineSet.Invoke(modelSet);
            var entity = this.RepositoryProvider.FirstOrDefault(modelSet.MapSet<TEntity>());
            return entity != null ? this.EntityMapper.Map<TModel>(entity) : default;
        }

        /// <inheritdoc />
        public async Task<TModel> FirstOrDefaultAsync(Action<EntitySet<TModel>> defineSet)
        {
            return await this.FirstOrDefaultAsync(defineSet, CancellationToken.None).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<TModel> FirstOrDefaultAsync(Action<EntitySet<TModel>> defineSet, CancellationToken cancellationToken)
        {
            if (defineSet == null)
            {
                throw new ArgumentNullException(nameof(defineSet));
            }

            var modelSet = new EntitySet<TModel>();
            defineSet.Invoke(modelSet);
            var entity = await this.RepositoryProvider.FirstOrDefaultAsync(modelSet.MapSet<TEntity>(), cancellationToken).ConfigureAwait(false);
            return entity != null ? this.EntityMapper.Map<TModel>(entity) : default;
        }

        /// <inheritdoc />
        public dynamic DynamicFirstOrDefault([NotNull] ISelection selection)
        {
            if (selection == null)
            {
                throw new ArgumentNullException(nameof(selection));
            }

            return this.RepositoryProvider.DynamicFirstOrDefault(selection as EntitySelection<TEntity> ?? selection.MapSelection<TEntity>());
        }

        /// <inheritdoc />
        public async Task<dynamic> DynamicFirstOrDefaultAsync(ISelection selection)
        {
            return await this.DynamicFirstOrDefaultAsync(selection, CancellationToken.None).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<dynamic> DynamicFirstOrDefaultAsync(ISelection selection, CancellationToken cancellationToken)
        {
            if (selection == null)
            {
                throw new ArgumentNullException(nameof(selection));
            }

            return await this.RepositoryProvider.DynamicFirstOrDefaultAsync(
                           selection as EntitySelection<TEntity> ?? selection.MapSelection<TEntity>(),
                           cancellationToken)
                       .ConfigureAwait(false);
        }

        /// <inheritdoc />
        public T GetScalar<T>([NotNull] ISelection selection)
        {
            if (selection == null)
            {
                throw new ArgumentNullException(nameof(selection));
            }

            return this.RepositoryProvider.GetScalar<T>(selection);
        }

        /// <inheritdoc />
        public async Task<T> GetScalarAsync<T>(ISelection selection)
        {
            return await this.GetScalarAsync<T>(selection, CancellationToken.None).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<T> GetScalarAsync<T>([NotNull] ISelection selection, CancellationToken cancellationToken)
        {
            if (selection == null)
            {
                throw new ArgumentNullException(nameof(selection));
            }

            return await this.RepositoryProvider.GetScalarAsync<T>(selection, cancellationToken).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public IEnumerable<TModel> SelectAll()
        {
            var exampleSelection = new EntitySelection<TEntity>();

            foreach (var item in this.RepositoryProvider.SelectEntities(exampleSelection))
            {
                yield return this.ConstructModel(item);
            }
        }

        /// <inheritdoc />
        public async IAsyncEnumerable<TModel> SelectAllAsync()
        {
            await foreach (var item in this.SelectAllAsync(CancellationToken.None).ConfigureAwait(false))
            {
                yield return item;
            }
        }

        /// <inheritdoc />
        public async IAsyncEnumerable<TModel> SelectAllAsync([EnumeratorCancellation] CancellationToken cancellationToken)
        {
            var exampleSelection = new EntitySelection<TEntity>();

            await foreach (var item in this.RepositoryProvider.SelectEntitiesAsync(exampleSelection, cancellationToken).ConfigureAwait(false))
            {
                yield return this.ConstructModel(item);
            }
        }

        /// <inheritdoc />
        public IEnumerable<TModel> SelectEntities<TItem>([NotNull] EntitySet<TItem> selection)
        {
            if (selection == null)
            {
                throw new ArgumentNullException(nameof(selection));
            }

            foreach (var item in this.RepositoryProvider.SelectEntities(selection as EntitySet<TEntity> ?? selection.MapSet<TEntity>()))
            {
                yield return this.ConstructModel(item);
            }
        }

        /// <inheritdoc />
        public async IAsyncEnumerable<TModel> SelectEntitiesAsync<TItem>(EntitySet<TItem> selection)
        {
            await foreach (var item in this.SelectEntitiesAsync(selection, CancellationToken.None).ConfigureAwait(false))
            {
                yield return item;
            }
        }

        /// <inheritdoc />
        public async IAsyncEnumerable<TModel> SelectEntitiesAsync<TItem>(
            EntitySet<TItem> selection,
            [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            if (selection == null)
            {
                throw new ArgumentNullException(nameof(selection));
            }

            await foreach (var item in this.RepositoryProvider
                               .SelectEntitiesAsync(selection as EntitySet<TEntity> ?? selection.MapSet<TEntity>(), cancellationToken)
                               .ConfigureAwait(false))
            {
                yield return this.ConstructModel(item);
            }
        }

        /// <inheritdoc />
        public IEnumerable<dynamic> DynamicSelect<TItem>([NotNull] EntitySelection<TItem> selection)
        {
            if (selection == null)
            {
                throw new ArgumentNullException(nameof(selection));
            }

            return this.RepositoryProvider.DynamicSelect(selection as EntitySelection<TEntity> ?? selection.MapSelection<TEntity>());
        }

        /// <inheritdoc />
        public async IAsyncEnumerable<dynamic> DynamicSelectAsync<TItem>(EntitySelection<TItem> selection)
        {
            await foreach (var item in this.DynamicSelectAsync(selection, CancellationToken.None).ConfigureAwait(false))
            {
                yield return item;
            }
        }

        /// <inheritdoc />
        public async IAsyncEnumerable<dynamic> DynamicSelectAsync<TItem>(
            EntitySelection<TItem> selection,
            [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            if (selection == null)
            {
                throw new ArgumentNullException(nameof(selection));
            }

            await foreach (var item in this.RepositoryProvider.DynamicSelectAsync(
                                   selection as EntitySelection<TEntity> ?? selection.MapSelection<TEntity>(),
                                   cancellationToken)
                               .ConfigureAwait(false))
            {
                yield return item;
            }
        }

        /// <summary>
        /// Constructs the entity for the specified data item.
        /// </summary>
        /// <param name="entity">
        /// The data item to construct an entity for.
        /// </param>
        /// <returns>
        /// A new instance of the entity.
        /// </returns>
        protected virtual TModel ConstructModel(TEntity entity)
        {
            return this.EntityMapper.Map<TModel>(entity);
        }

        /// <summary>
        /// Gets an example item from the provided <typeparamref name="TItem"/> key. If the key is a value type or string, an item is created and its
        /// first unique key property value is set to the key, or first primary key property if no unique keys are specified. If the key is the same
        /// type as the entity, then the entity is returned directly. If the key is a dynamic, then all matching unique attributes of the specified
        /// unique key are set from the dynamic, or all matching primary key values if no unique keys are specified.
        /// </summary>
        /// <param name="key">
        /// The key for the example item.
        /// </param>
        /// <typeparam name="TItem">
        /// The type of item that represents the key.
        /// </typeparam>
        /// <returns>
        /// An example of the data row as a <typeparamref name="TEntity"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="key"/> is null.
        /// </exception>
        /// <exception cref="OperationException">
        /// The key cannot be applied to the <typeparamref name="TEntity"/>. The inner exception contains details as to why.
        /// </exception>
        protected MapResult<TEntity> GetExampleEntity<TItem>([NotNull] TItem key)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            TEntity exampleEntity;
            var entityDefinition = this.RepositoryProvider.EntityDefinitionProvider.Resolve<TEntity>();
            var keyDefinitions = entityDefinition.PrimaryKeyAttributes;
            var keyType = key.GetType();

            var values = new Dictionary<string, object>();

            // Single key value
            if (keyType.IsValueType || key is string)
            {
                // If the example is a value type, create a new data item as an example and set the key. Assumption is that the two are compatible.
                exampleEntity = new TEntity();
                var keyDefinition = keyDefinitions.First();

                try
                {
                    keyDefinition.SetValueDelegate.DynamicInvoke(exampleEntity, key);
                    values.Add(keyDefinition.PropertyName, key);
                }
                catch (ArgumentException ex)
                {
                    var message = string.Format(
                        CultureInfo.CurrentCulture,
                        ErrorMessages.UnableToSetPropertyToValue,
                        typeof(TEntity),
                        keyDefinition.PropertyName,
                        key,
                        ex.Message);

                    throw new OperationException(key, message, ex);
                }
                catch (MemberAccessException ex)
                {
                    var message = string.Format(
                        CultureInfo.CurrentCulture,
                        ErrorMessages.UnableToSetPropertyToValue,
                        typeof(TEntity),
                        keyDefinition.PropertyName,
                        key,
                        ex.Message);

                    throw new OperationException(key, message, ex);
                }
                catch (TargetInvocationException ex)
                {
                    var message = string.Format(
                        CultureInfo.CurrentCulture,
                        ErrorMessages.UnableToSetPropertyToValue,
                        typeof(TEntity),
                        keyDefinition.PropertyName,
                        key,
                        ex.Message);

                    throw new OperationException(key, message, ex);
                }
            }
            else
            {
                switch (key)
                {
                    case TEntity entity:
                        exampleEntity = entity;
                        break;
                    case ExpandoObject expando:

                        // Handle dynamics
                        exampleEntity = new TEntity();

                        foreach (var keyValue in expando)
                        {
                            var attribute = entityDefinition.Find(entityDefinition.EntityName, keyValue.Key);

                            if (attribute == default)
                            {
                                continue;
                            }

                            try
                            {
                                var value = expando.FirstOrDefault(pair => pair.Key == attribute.PropertyName).Value;
                                attribute.SetValueDelegate.DynamicInvoke(exampleEntity, value);
                                values.Add(attribute.PropertyName, value);
                            }
                            catch (ArgumentException ex)
                            {
                                var message = string.Format(
                                    CultureInfo.CurrentCulture,
                                    ErrorMessages.UnableToSetPropertyToValue,
                                    typeof(TEntity),
                                    attribute.PropertyName,
                                    key,
                                    ex.Message);

                                throw new OperationException(key, message, ex);
                            }
                            catch (MemberAccessException ex)
                            {
                                var message = string.Format(
                                    CultureInfo.CurrentCulture,
                                    ErrorMessages.UnableToSetPropertyToValue,
                                    typeof(TEntity),
                                    attribute.PropertyName,
                                    key,
                                    ex.Message);

                                throw new OperationException(key, message, ex);
                            }
                            catch (TargetInvocationException ex)
                            {
                                var message = string.Format(
                                    CultureInfo.CurrentCulture,
                                    ErrorMessages.UnableToSetPropertyToValue,
                                    typeof(TEntity),
                                    attribute.PropertyName,
                                    key,
                                    ex.Message);

                                throw new OperationException(key, message, ex);
                            }
                        }

                        break;

                    default:

                        // Handle anonymous types, require all key definitions to be set.
                        exampleEntity = new TEntity();

                        foreach (var attribute in entityDefinition.DirectAttributes)
                        {
                            try
                            {
                                var value = key.GetType().GetProperty(attribute.PropertyName)?.GetMethod?.Invoke(key, null);

                                if (value == null)
                                {
                                    continue;
                                }

                                attribute.SetValueDelegate.DynamicInvoke(exampleEntity, value);
                                values.Add(attribute.PropertyName, value);
                            }
                            catch (ArgumentException ex)
                            {
                                var message = string.Format(
                                    CultureInfo.CurrentCulture,
                                    ErrorMessages.UnableToSetPropertyToValue,
                                    typeof(TEntity),
                                    attribute.PropertyName,
                                    key,
                                    ex.Message);

                                throw new OperationException(key, message, ex);
                            }
                            catch (MemberAccessException ex)
                            {
                                var message = string.Format(
                                    CultureInfo.CurrentCulture,
                                    ErrorMessages.UnableToSetPropertyToValue,
                                    typeof(TEntity),
                                    attribute.PropertyName,
                                    key,
                                    ex.Message);

                                throw new OperationException(key, message, ex);
                            }
                            catch (TargetInvocationException ex)
                            {
                                var message = string.Format(
                                    CultureInfo.CurrentCulture,
                                    ErrorMessages.UnableToSetPropertyToValue,
                                    typeof(TEntity),
                                    attribute.PropertyName,
                                    key,
                                    ex.Message);

                                throw new OperationException(key, message, ex);
                            }
                        }

                        break;
                }
            }

            return new MapResult<TEntity>(exampleEntity, values);
        }

        /// <summary>
        /// Gets a unique set query for the specified candidate item.
        /// </summary>
        /// <typeparam name="TItem">
        /// The type of the candidate item.
        /// </typeparam>
        /// <param name="candidate">
        /// The candidate item.
        /// </param>
        /// <param name="valueFilterSet">
        /// The filters that define the unique set.
        /// </param>
        protected void GetUniqueSet<TItem>([NotNull] TItem candidate, [NotNull] ValueFilterSet<TEntity> valueFilterSet)
        {
            if (candidate == null)
            {
                throw new ArgumentNullException(nameof(candidate));
            }

            if (valueFilterSet == null)
            {
                throw new ArgumentNullException(nameof(valueFilterSet));
            }

            var mapResult = this.GetExampleEntity(candidate);

            // Detect keys
            var definitionProvider = this.RepositoryProvider.EntityDefinitionProvider;

            // Assume that value or string types equal a surrogate primary key.
            if (typeof(TItem).IsValueType || candidate is string)
            {
                valueFilterSet.MatchKey(mapResult.Item, definitionProvider);
            }
            else if (typeof(TItem) == typeof(TEntity))
            {
                var entityDefinition = definitionProvider.Resolve<TEntity>();

                foreach (var attribute in entityDefinition.DirectAttributes)
                {
                    var value = attribute.GetValueDelegate.DynamicInvoke(mapResult.Item);

                    if (value == null || 0.Equals(value) || Guid.Empty.Equals(value))
                    {
                        continue;
                    }

                    var parameter = Expression.Parameter(typeof(TEntity), "value");
                    var property = Expression.Property(parameter, attribute.PropertyName);
                    var expression = Expression.Lambda(property, parameter);
                    valueFilterSet.Add(new ValueFilter(new AttributeLocation(expression), FilterType.Equality, value));
                }
            }
            else if (mapResult.Values.Any() == false)
            {
                throw new BusinessException(candidate, $"No properties were set on the candidate object {candidate}.");
            }
            else
            {
                foreach (var valueSet in mapResult.Values)
                {
                    var parameter = Expression.Parameter(typeof(TEntity), "value");
                    var property = Expression.Property(parameter, valueSet.Key);
                    var expression = Expression.Lambda(property, parameter);
                    valueFilterSet.Add(new ValueFilter(new AttributeLocation(expression), FilterType.Equality, valueSet.Value));
                }
            }
        }
    }
}