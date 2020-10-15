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
        /// The selection comparer.
        /// </summary>
        private readonly IComparer<TEntity> selectionComparer;

        /// <summary>
        /// The unique key expressions.
        /// </summary>
        private readonly List<LambdaExpression> uniqueKeyExpressions = new List<LambdaExpression>();

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

            var exampleEntity = this.GetExampleEntity(candidate);
            var entitySet = this.uniqueKeyExpressions.Any()
                                ? this.GetUniqueKeySet(exampleEntity)
                                : new EntitySet<TEntity>().Where(
                                    set => set.MatchKey(exampleEntity, this.RepositoryProvider.EntityDefinitionProvider));

            return this.RepositoryProvider.Contains(entitySet);
        }

        /// <inheritdoc />
        public async Task<bool> ContainsAsync<TItem>(TItem candidate)
        {
            if (Evaluate.IsNull(candidate))
            {
                throw new ArgumentNullException(nameof(candidate));
            }

            var exampleEntity = this.GetExampleEntity(candidate);
            var entitySet = this.uniqueKeyExpressions.Any()
                                ? this.GetUniqueKeySet(exampleEntity)
                                : new EntitySet<TEntity>().Where(
                                    set => set.MatchKey(exampleEntity, this.RepositoryProvider.EntityDefinitionProvider));

            return await this.RepositoryProvider.ContainsAsync(entitySet).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public bool Contains<TItem>([NotNull] EntitySet<TItem> selection)
        {
            if (selection == null)
            {
                throw new ArgumentNullException(nameof(selection));
            }

            return this.RepositoryProvider.Contains(selection.MapSet<TEntity>());
        }

        /// <inheritdoc />
        public async Task<bool> ContainsAsync<TItem>(EntitySet<TItem> selection)
        {
            if (selection == null)
            {
                throw new ArgumentNullException(nameof(selection));
            }

            return await this.RepositoryProvider.ContainsAsync(selection.MapSet<TEntity>()).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public TModel FirstOrDefault<TItem>(TItem candidate)
        {
            if (Evaluate.IsNull(candidate))
            {
                throw new ArgumentNullException(nameof(candidate));
            }

            var exampleEntity = this.GetExampleEntity(candidate);
            var entitySet = this.GetUniqueItemSelection(exampleEntity);
            entitySet.SetDefaultRelations(this.RepositoryProvider.EntityDefinitionProvider);

            var dataItem = this.RepositoryProvider.FirstOrDefault(entitySet);

            if (Evaluate.IsNull(dataItem))
            {
                return default;
            }

            var entity = this.ConstructEntity(dataItem);
            return entity;
        }

        /// <inheritdoc />
        public async Task<TModel> FirstOrDefaultAsync<TItem>(TItem candidate)
        {
            if (Evaluate.IsNull(candidate))
            {
                throw new ArgumentNullException(nameof(candidate));
            }

            var exampleEntity = this.GetExampleEntity(candidate);
            var entitySet = this.GetUniqueItemSelection(exampleEntity);
            entitySet.SetDefaultRelations(this.RepositoryProvider.EntityDefinitionProvider);

            var dataItem = await this.RepositoryProvider.FirstOrDefaultAsync(entitySet).ConfigureAwait(false);

            if (Evaluate.IsNull(dataItem))
            {
                return default;
            }

            var entity = this.ConstructEntity(dataItem);
            return entity;
        }

        /// <inheritdoc />
        public TModel FirstOrDefault<TItem>([NotNull] EntitySet<TItem> selection)
        {
            if (selection == null)
            {
                throw new ArgumentNullException(nameof(selection));
            }

            var entity = this.RepositoryProvider.FirstOrDefault(selection.MapSet<TEntity>());
            return entity != null ? this.EntityMapper.Map<TModel>(entity) : default;
        }

        /// <inheritdoc />
        public async Task<TModel> FirstOrDefaultAsync<TItem>(EntitySet<TItem> selection)
        {
            if (selection == null)
            {
                throw new ArgumentNullException(nameof(selection));
            }

            var entity = await this.RepositoryProvider.FirstOrDefaultAsync(selection.MapSet<TEntity>()).ConfigureAwait(false);
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
            if (defineSet == null)
            {
                throw new ArgumentNullException(nameof(defineSet));
            }

            var modelSet = new EntitySet<TModel>();
            defineSet.Invoke(modelSet);
            var entity = await this.RepositoryProvider.FirstOrDefaultAsync(modelSet.MapSet<TEntity>()).ConfigureAwait(false);
            return entity != null ? this.EntityMapper.Map<TModel>(entity) : default;
        }

        /// <inheritdoc />
        public dynamic DynamicFirstOrDefault([NotNull] ISelection selection)
        {
            if (selection == null)
            {
                throw new ArgumentNullException(nameof(selection));
            }

            return this.RepositoryProvider.DynamicFirstOrDefault(selection.MapSelection<TEntity>());
        }

        /// <inheritdoc />
        public async Task<dynamic> DynamicFirstOrDefaultAsync(ISelection selection)
        {
            if (selection == null)
            {
                throw new ArgumentNullException(nameof(selection));
            }

            return await this.RepositoryProvider.DynamicFirstOrDefaultAsync(selection.MapSelection<TEntity>()).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public IEnumerable<TModel> SelectAll()
        {
            var exampleSelection = new EntitySelection<TEntity>();
            var entities = this.RepositoryProvider.SelectEntities(exampleSelection);
            return this.SelectResults(entities);
        }

        /// <inheritdoc />
        public async Task<IEnumerable<TModel>> SelectAllAsync()
        {
            var exampleSelection = new EntitySelection<TEntity>();
            var entities = await this.RepositoryProvider.SelectEntitiesAsync(exampleSelection).ConfigureAwait(false);
            return this.SelectResults(entities);
        }

        /// <inheritdoc />
        public IEnumerable<TModel> SelectEntities<TItem>([NotNull] EntitySet<TItem> selection)
        {
            if (selection == null)
            {
                throw new ArgumentNullException(nameof(selection));
            }

            var mappedSelection = selection.MapSet<TEntity>();
            var entities = this.RepositoryProvider.SelectEntities(mappedSelection);
            return this.SelectResults(entities);
        }

        /// <inheritdoc />
        public async Task<IEnumerable<TModel>> SelectEntitiesAsync<TItem>(EntitySet<TItem> selection)
        {
            if (selection == null)
            {
                throw new ArgumentNullException(nameof(selection));
            }

            var mappedSelection = selection.MapSet<TEntity>();
            var entities = await this.RepositoryProvider.SelectEntitiesAsync(mappedSelection).ConfigureAwait(false);
            return this.SelectResults(entities);
        }

        /// <inheritdoc />
        public IEnumerable<dynamic> DynamicSelect<TItem>([NotNull] EntitySelection<TItem> selection)
        {
            if (selection == null)
            {
                throw new ArgumentNullException(nameof(selection));
            }

            return this.RepositoryProvider.DynamicSelect(selection.MapSelection<TEntity>());
        }

        /// <inheritdoc />
        public async Task<IEnumerable<dynamic>> DynamicSelectAsync<TItem>(EntitySelection<TItem> selection)
        {
            if (selection == null)
            {
                throw new ArgumentNullException(nameof(selection));
            }

            return await this.RepositoryProvider.DynamicSelectAsync(selection.MapSelection<TEntity>()).ConfigureAwait(false);
        }

        /// <summary>
        /// Gets a unique item selection for the specified entity.
        /// </summary>
        /// <param name="entity">
        /// The entity to create the selection for.
        /// </param>
        /// <returns>
        /// A <see cref="EntitySelection{T}"/> for the specified entity.
        /// </returns>
        protected virtual EntitySelection<TEntity> GetUniqueItemSelection(TEntity entity)
        {
            var entitySelection = new EntitySelection<TEntity>();

            if (this.uniqueKeyExpressions.Count == 0)
            {
                entitySelection.Where(set => set.MatchKey(entity, this.RepositoryProvider.EntityDefinitionProvider));
            }
            else
            {
                foreach (var keyExpression in this.uniqueKeyExpressions)
                {
                    entitySelection.Where(set => set.AreEqual(keyExpression, keyExpression.Compile().DynamicInvoke(entity)));
                }
            }

            return entitySelection;
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

            TEntity exampleEntity;
            var entityDefinition = this.RepositoryProvider.EntityDefinitionProvider.Resolve<TEntity>();

            var keyDefinitions = this.uniqueKeyExpressions.Count == 0
                                    ? entityDefinition.PrimaryKeyAttributes
                                    : this.uniqueKeyExpressions.Select(entityDefinition.Find);

            var keyType = key.GetType();

            // Single key value
            if (keyType.IsValueType || key is string)
            {
                // If the example is a value type, create a new data item as an example and set the key. Assumption is that the two are compatible.
                exampleEntity = new TEntity();
                var keyDefinition = keyDefinitions.First();

                try
                {
                    keyDefinition.SetValueDelegate.DynamicInvoke(exampleEntity, key);
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

                        foreach (var attribute in keyDefinitions)
                        {
                            try
                            {
                                attribute.SetValueDelegate.DynamicInvoke(
                                    exampleEntity,
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
                        exampleEntity = new TEntity();
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

                                attribute.SetValueDelegate.DynamicInvoke(exampleEntity, value);
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
                            exampleEntity = this.EntityMapper.Map<TEntity>(key);
                        }

                        break;
                }
            }

            return exampleEntity;
        }

        /// <summary>
        /// Gets a unique key set for the <paramref name="exampleEntity"/>.
        /// </summary>
        /// <param name="exampleEntity">
        /// The example entity to get a unique key set for.
        /// </param>
        /// <returns>
        /// An <see cref="EntitySet{TEntity}"/> based on the unique keys specified in the <see cref="ReadOnlyRepository{TModel,TEntity}"/>.
        /// </returns>
        private EntitySet<TEntity> GetUniqueKeySet(TEntity exampleEntity)
        {
            var selection = new EntitySet<TEntity>();

            foreach (var keyExpression in this.uniqueKeyExpressions)
            {
                selection.Where(set => set.AreEqual(keyExpression, keyExpression.Compile().DynamicInvoke(exampleEntity)));
            }

            return selection;
        }

        /// <summary>
        /// Selects results and adds them to the cache.
        /// </summary>
        /// <param name="entities">
        /// The data items to select into the results collection.
        /// </param>
        /// <returns>
        /// A collection of entities based on the specified data items.
        /// </returns>
        private IEnumerable<TModel> SelectResults(IEnumerable<TEntity> entities)
        {
            var results = new List<TModel>();

            // Order the list of data items if desired.
            var items = this.selectionComparer == null ? entities : entities.OrderBy(x => x, this.selectionComparer);

            foreach (var dataItem in items)
            {
                var entity = this.ConstructEntity(dataItem);
                results.Add(entity);
            }

            return results;
        }
    }
}