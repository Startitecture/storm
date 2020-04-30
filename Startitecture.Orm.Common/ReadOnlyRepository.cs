// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ReadOnlyRepository.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
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
    using System.Runtime.Caching;

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
        where TEntity : class, ITransactionContext, new()
    {
        /// <summary>
        /// The cache key format.
        /// </summary>
        private const string CacheKeyFormat = "{0}.{1}";

        /// <summary>
        /// The entity cache.
        /// </summary>
        private readonly ObjectCache entityCache = MemoryCache.Default;

        /// <summary>
        /// The cache item policy.
        /// </summary>
        private readonly CacheItemPolicy cacheItemPolicy = new CacheItemPolicy { SlidingExpiration = TimeSpan.FromMinutes(2) };

        /// <summary>
        /// The selection comparer.
        /// </summary>
        private readonly IComparer<TEntity> selectionComparer;

        /// <summary>
        /// The unique key expressions.
        /// </summary>
        private readonly List<Expression<Func<TEntity, object>>> uniqueKeyExpressions = new List<Expression<Func<TEntity, object>>>();

        /// <summary>
        /// Initializes a new instance of the <see cref="ReadOnlyRepository{TModel,TEntity}"/> class.
        /// </summary>
        /// <param name="repositoryProvider">
        /// The repository provider for this repository.
        /// </param>
        /// <param name="entityMapper">
        /// The entity mapper.
        /// </param>
        /// <param name="uniqueKeyExpressions">
        /// The unique key expressions for the entity, if any.
        /// </param>
        public ReadOnlyRepository(
            IRepositoryProvider repositoryProvider,
            IEntityMapper entityMapper,
            params Expression<Func<TEntity, object>>[] uniqueKeyExpressions)
            : this(repositoryProvider, entityMapper, null, uniqueKeyExpressions)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReadOnlyRepository{TModel,TEntity}"/> class.
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
        /// <param name="uniqueKeyExpressions">
        /// The unique key expressions for the entity, if any.
        /// </param>
        public ReadOnlyRepository(
            [NotNull] IRepositoryProvider repositoryProvider,
            [NotNull] IEntityMapper entityMapper,
            IComparer<TEntity> selectionComparer,
            [NotNull] params Expression<Func<TEntity, object>>[] uniqueKeyExpressions)
        {
            // The entity mapper, and its resolution context, is intended to last only for the lifetime of the repository.
            this.EntityMapper = entityMapper ?? throw new ArgumentNullException(nameof(entityMapper));
            this.RepositoryProvider = repositoryProvider ?? throw new ArgumentNullException(nameof(repositoryProvider));
            this.selectionComparer = selectionComparer;
 
            if (uniqueKeyExpressions == null)
            {
                throw new ArgumentNullException(nameof(uniqueKeyExpressions));
            }

            this.uniqueKeyExpressions.Clear();
            this.uniqueKeyExpressions.AddRange(uniqueKeyExpressions);
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

            var dataItem = this.GetExampleEntity(candidate);
            var uniqueItemSelection = this.GetUniqueItemSelection(dataItem);
            return this.RepositoryProvider.Contains(uniqueItemSelection);
        }

        /// <inheritdoc />
        public bool Contains<TItem>([NotNull] ItemSelection<TItem> selection)
        {
            if (selection == null)
            {
                throw new ArgumentNullException(nameof(selection));
            }

            return this.RepositoryProvider.Contains(selection.MapTo<TEntity>());
        }

        /// <inheritdoc />
        public TModel FirstOrDefault<TItem>(TItem candidate)
        {
            if (Evaluate.IsNull(candidate))
            {
                throw new ArgumentNullException(nameof(candidate));
            }

            if (candidate is ITransactionContext context)
            {
                context.SetTransactionProvider(this.RepositoryProvider);
            }

            TModel entity;

            var selectionItem = this.GetExampleEntity(candidate);
            var uniqueItemSelection = this.GetUniqueItemSelection(selectionItem);

            // Because we want to hydrate the entire entity, we need to add joins if available.
            foreach (var relation in this.RepositoryProvider.EntityDefinitionProvider.Resolve<TEntity>().DefaultRelations)
            {
                uniqueItemSelection.AddRelation(relation);
            }

            var cacheResult = this.QueryCache(uniqueItemSelection);

            if (cacheResult.Hit)
            {
                entity = cacheResult.Item;
            }
            else
            {
                var dataItem = this.RepositoryProvider.GetFirstOrDefault(uniqueItemSelection);

                if (Evaluate.IsNull(dataItem))
                {
                    return default;
                }

                dataItem.SetTransactionProvider(this.RepositoryProvider);

                entity = this.ConstructEntity(dataItem);
                this.UpdateCache(cacheResult.Key, entity);
            }

            return entity;
        }

        /// <inheritdoc />
        public TModel FirstOrDefault<TItem>([NotNull] ItemSelection<TItem> selection)
        {
            if (selection == null)
            {
                throw new ArgumentNullException(nameof(selection));
            }

            var item = this.RepositoryProvider.GetFirstOrDefault(selection.MapTo<TEntity>());
            return item != null ? this.EntityMapper.Map<TModel>(item) : default;
        }

        /// <inheritdoc />
        public IEnumerable<TModel> SelectAll()
        {
            var exampleSelection = new ItemSelection<TEntity>();
            var dataItems = this.RepositoryProvider.GetSelection(exampleSelection);
            return this.SelectResults(dataItems);
        }

        /// <inheritdoc />
        public IEnumerable<TModel> Select<TItem>([NotNull] ItemSelection<TItem> selection)
        {
            if (selection == null)
            {
                throw new ArgumentNullException(nameof(selection));
            }

            var mappedSelection = selection.MapTo<TEntity>();
            var dataItems = this.RepositoryProvider.GetSelection(mappedSelection);
            return this.SelectResults(dataItems);
        }

        /// <summary>
        /// Gets a unique item selection for the specified entity.
        /// </summary>
        /// <param name="entity">
        /// The entity to create the selection for.
        /// </param>
        /// <returns>
        /// A <see cref="ItemSelection{TEntity}"/> for the specified entity.
        /// </returns>
        protected virtual ItemSelection<TEntity> GetUniqueItemSelection(TEntity entity)
        {
            if (this.uniqueKeyExpressions.Count == 0)
            {
                return new UniqueQuery<TEntity>(this.RepositoryProvider.EntityDefinitionProvider, entity);
            }

            var itemSelection = new ItemSelection<TEntity>();

            foreach (var keyExpression in this.uniqueKeyExpressions)
            {
                itemSelection.WhereEqual(keyExpression, keyExpression.Compile().Invoke(entity));
            }

            return itemSelection;
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
        protected virtual TModel ConstructEntity(TEntity entity)
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
        protected TEntity GetExampleEntity<TItem>([NotNull] TItem key)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            TEntity dataItem;
            var entityDefinition = this.RepositoryProvider.EntityDefinitionProvider.Resolve<TEntity>();

            var keyDefinitions = this.uniqueKeyExpressions.Count == 0
                                    ? entityDefinition.PrimaryKeyAttributes
                                    : this.uniqueKeyExpressions.Select(entityDefinition.Find);

            var keyType = key.GetType();

            // Single key value
            if (keyType.IsValueType || key is string)
            {
                // If the example is a value type, create a new data item as an example and set the key. Assumption is that the two are compatible.
                dataItem = new TEntity();
                var keyDefinition = keyDefinitions.First();

                try
                {
                    keyDefinition.SetValueDelegate.DynamicInvoke(dataItem, key);
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
                        dataItem = entity;
                        break;
                    case ExpandoObject expando:
                        
                        // Handle dynamics 
                        dataItem = new TEntity();

                        foreach (var attribute in keyDefinitions)
                        {
                            try
                            {
                                attribute.SetValueDelegate.DynamicInvoke(
                                    dataItem,
                                    expando.FirstOrDefault(pair => pair.Key == attribute.PropertyName).Value);
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
                        dataItem = new TEntity();
                        bool anyNull = false;

                        foreach (var attribute in keyDefinitions)
                        {
                            try
                            {
                                var value = key.GetType().GetProperty(attribute.PropertyName)?.GetMethod.Invoke(key, null);

                                if (value == null)
                                {
                                    anyNull = true;
                                    break;
                                }

                                attribute.SetValueDelegate.DynamicInvoke(dataItem, value);
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

                        // If any key definitions are null, our last resort is to try the mapper.
                        if (anyNull)
                        {
                            dataItem = this.EntityMapper.Map<TEntity>(key);
                        }

                        break;
                }
            }

            dataItem.SetTransactionProvider(this.RepositoryProvider);
            return dataItem;
        }

        /// <summary>
        /// Attempts to get a cached entity based on the specified item.
        /// </summary>
        /// <param name="selection">
        /// The item that represents the unique key for the entity.
        /// </param>
        /// <returns>
        /// <c>true</c> if the entity is found; otherwise, <c>false</c>.
        /// </returns>
        private CacheResult<TModel> QueryCache(ItemSelection<TEntity> selection)
        {
            var key = string.Format(CultureInfo.InvariantCulture, CacheKeyFormat, typeof(TModel).ToRuntimeName(), selection);
            var cacheResult = new CacheResult<TModel>(default, false, key);

            if (this.RepositoryProvider.EnableCaching == false)
            {
                return cacheResult;
            }

            var cachedItem = this.entityCache.Get(key);

            if (cachedItem is TModel item)
            {
                ////Trace.TraceInformation("Got item '{0}' from the cache with key '{1}'.", cachedItem, key);
                cacheResult = new CacheResult<TModel>(item, true, key);
            }

            return cacheResult;
        }

        /// <summary>
        /// Updates the cache with the specified key.
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <param name="entity">
        /// The entity to save in the cache.
        /// </param>
        private void UpdateCache(string key, TModel entity)
        {
            this.entityCache.Set(key, entity, this.cacheItemPolicy);
        }

        /// <summary>
        /// Selects results and adds them to the cache.
        /// </summary>
        /// <param name="dataItems">
        /// The data items to select into the results collection.
        /// </param>
        /// <returns>
        /// A collection of entities based on the specified data items.
        /// </returns>
        private IEnumerable<TModel> SelectResults(IEnumerable<TEntity> dataItems)
        {
            var results = new List<TModel>();

            // Order the list of data items if desired.
            var items = this.selectionComparer == null ? dataItems : dataItems.OrderBy(x => x, this.selectionComparer);

            // TODO: Put results in the container for usage by the construct entity hook.
            foreach (var dataItem in items)
            {
                dataItem.SetTransactionProvider(this.RepositoryProvider);
                var entity = this.ConstructEntity(dataItem);
                results.Add(entity);
            }

            return results;
        }
    }
}