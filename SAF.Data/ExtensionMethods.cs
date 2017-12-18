// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExtensionMethods.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// <summary>
//   Contains extension methods for the SAF data namespace.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace SAF.Data
{
    using System;
    using System.Configuration;
    using System.Data;
    using System.Data.Common;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;

    using AutoMapper;

    using JetBrains.Annotations;

    using SAF.Core;
    using SAF.StringResources;

    /// <summary>
    /// Contains extension methods for the SAF data namespace.
    /// </summary>
    public static class ExtensionMethods
    {
        #region Property Mapping

        /// <summary>
        /// Keeps the target property from being mapped.
        /// </summary>
        /// <param name="mappingExpression">
        /// The mapping expression.
        /// </param>
        /// <param name="targetProperty">
        /// The target property.
        /// </param>
        /// <typeparam name="TDataItem">
        /// The type of data item being mapped from.
        /// </typeparam>
        /// <typeparam name="TEntity">
        /// The type of entity being mapped to.
        /// </typeparam>
        /// <returns>
        /// The current <see cref="IMappingExpression"/>.
        /// </returns>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "Allows fluent usage of the method.")]
        public static IMappingExpression<TDataItem, TEntity> DoNotMap<TDataItem, TEntity>(
            [NotNull] this IMappingExpression<TDataItem, TEntity> mappingExpression,
            [NotNull] Expression<Func<TEntity, object>> targetProperty)
        {
            if (mappingExpression == null)
            {
                throw new ArgumentNullException(nameof(mappingExpression));
            }

            if (targetProperty == null)
            {
                throw new ArgumentNullException(nameof(targetProperty));
            }

            return mappingExpression.ForMember(targetProperty, expr => expr.UseDestinationValue()).Ignore(targetProperty);
        }

        /// <summary>
        /// Ignores the specified properties in the destination type.
        /// </summary>
        /// <param name="mappingExpression">
        /// The current mapping expression.
        /// </param>
        /// <param name="propertiesToIgnore">
        /// The properties to ignore.
        /// </param>
        /// <typeparam name="TSource">
        /// The type that will be mapped from.
        /// </typeparam>
        /// <typeparam name="TDestination">
        /// The type that will be mapped to.
        /// </typeparam>
        /// <returns>
        /// The current mapping expression with the specified properties ignored.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="mappingExpression"/> or <paramref name="propertiesToIgnore"/> is null.
        /// </exception>
        public static IMappingExpression<TSource, TDestination> Ignore<TSource, TDestination>(
            [NotNull] this IMappingExpression<TSource, TDestination> mappingExpression,
            [NotNull] params Expression<Func<TDestination, object>>[] propertiesToIgnore)
        {
            if (mappingExpression == null)
            {
                throw new ArgumentNullException(nameof(mappingExpression));
            }

            if (propertiesToIgnore == null)
            {
                throw new ArgumentNullException(nameof(propertiesToIgnore));
            }

            foreach (var expression in propertiesToIgnore)
            {
                mappingExpression.ForMember(expression, expr => expr.Ignore());
            }

            return mappingExpression;
        }

        /// <summary>
        /// Maps a key for the specified mapping expression.
        /// </summary>
        /// <param name="mapping">
        /// The current mapping expression.
        /// </param>
        /// <param name="keySelection">
        /// The key selection.
        /// </param>
        /// <typeparam name="TKey">
        /// The type of key that will map to the data item.
        /// </typeparam>
        /// <typeparam name="TDataItem">
        /// The type of data item that the key will map to.
        /// </typeparam>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="mapping"/> or <paramref name="keySelection"/> is null.
        /// </exception>
        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters", Justification = "Allows fluent usage of the method.")]
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "Allows fluent usage of the method.")]
        public static void MapKey<TKey, TDataItem>(
            [NotNull] this IMappingExpression<TKey, TDataItem> mapping,
            [NotNull] Expression<Func<TDataItem, TKey>> keySelection) where TDataItem : ITransactionContext
        {
            if (mapping == null)
            {
                throw new ArgumentNullException(nameof(mapping));
            }

            if (keySelection == null)
            {
                throw new ArgumentNullException(nameof(keySelection));
            }

            // Do not change this order - we must first ignore all members, THEN set the one we want to use. Otherwise ignore all will 
            // override the settings...
            mapping.ForMember(keySelection.GetPropertyName(), expr => expr.MapFrom(source => source));
            mapping.ForAllOtherMembers(expr => expr.Ignore());
        }

        /// <summary>
        /// Configures a mapping to construct a data item using the specified key.
        /// </summary>
        /// <typeparam name="TKey">
        /// The type of key to use to construct the item.
        /// </typeparam>
        /// <typeparam name="TDataItem">
        /// The type of data item that contains the key.
        /// </typeparam>
        /// <param name="config">
        /// The configuration to apply the mapping to.
        /// </param>
        /// <param name="keySelection">
        /// An expression that selects the data item's primary key.
        /// </param>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "Required for implicit usage.")]
        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters", Justification = "Required for implicit usage.")]
        public static void ConfigureKey<TKey, TDataItem>(this Profile config, Expression<Func<TDataItem, TKey>> keySelection)
            where TDataItem : ITransactionContext
        {
            if (config == null)
            {
                throw new ArgumentNullException(nameof(config));
            }

            if (keySelection == null)
            {
                throw new ArgumentNullException(nameof(keySelection));
            }

            config.CreateMap<TKey, TDataItem>().MapKey(keySelection);
        }

        /// <summary>
        /// Maps an entity property to the specified data item property.
        /// </summary>
        /// <param name="mappingExpression">
        /// The mapping expression.
        /// </param>
        /// <param name="targetProperty">
        /// The entity property.
        /// </param>
        /// <param name="sourceProperty">
        /// The data item property.
        /// </param>
        /// <typeparam name="TDataItem">
        /// The type of data item that is being mapped from.
        /// </typeparam>
        /// <typeparam name="TSource">
        /// The type of the ID to map.
        /// </typeparam>
        /// <typeparam name="TEntity">
        /// The type of the entity that contains the dependency.
        /// </typeparam>
        /// <typeparam name="TTarget">
        /// The type of enumeration to map.
        /// </typeparam>
        /// <returns>
        /// The current mapping expression with the enumeration mapped from the data item to the entity.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="targetProperty"/> or <paramref name="sourceProperty"/> is null.
        /// </exception>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "Allows fluent usage of the method.")]
        public static IMappingExpression<TDataItem, TEntity> MapEntityProperty<TDataItem, TSource, TEntity, TTarget>(
            [NotNull] this IMappingExpression<TDataItem, TEntity> mappingExpression,
            [NotNull] Expression<Func<TEntity, TTarget>> targetProperty,
            [NotNull] Expression<Func<TDataItem, TSource>> sourceProperty)
        {
            if (mappingExpression == null)
            {
                throw new ArgumentNullException(nameof(mappingExpression));
            }

            if (targetProperty == null)
            {
                throw new ArgumentNullException(nameof(targetProperty));
            }

            if (sourceProperty == null)
            {
                throw new ArgumentNullException(nameof(sourceProperty));
            }

            var targetPropertyName = targetProperty.GetPropertyName();

            if (String.IsNullOrWhiteSpace(targetPropertyName))
            {
                throw new OperationException(targetProperty, ErrorMessages.PropertyNameNotSet);
            }

            mappingExpression.ForMember(targetPropertyName, expr => expr.MapFrom(sourceProperty));

            var constructorParam = GetConstructorParam(targetProperty);

            // If we find that the enumeration is present in the constructor and matches the naming convention, then we will also add a
            // call to MapForCtor(), otherwise we just stick with MapProperty().
            if (constructorParam != null)
            {
                mappingExpression.ForMember(targetPropertyName, expr => expr.MapFrom(sourceProperty))
                    .ForCtorParam(constructorParam.Name, expr => expr.MapFrom(sourceProperty));
            }

            return mappingExpression;
        }

        #endregion

        #region Entity Mapping

        /// <summary>
        /// Maps a dependency of the entity to the row.
        /// </summary>
        /// <param name="mappingExpression">
        /// The mapping expression.
        /// </param>
        /// <param name="dependency">
        /// The dependency of the entity to map.
        /// </param>
        /// <param name="dependencyKey">
        /// The dependency Key.
        /// </param>
        /// <typeparam name="TDataItem">
        /// The type of data item that is being mapped from.
        /// </typeparam>
        /// <typeparam name="TEntity">
        /// The type of the entity that contains the dependency.
        /// </typeparam>
        /// <typeparam name="TDependency">
        /// The type of the dependency to map.
        /// </typeparam>
        /// <typeparam name="TKey">
        /// The type of the dependency's key. 
        /// </typeparam>
        /// <returns>
        /// The current mapping expression.
        /// </returns>
        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters", Justification = "Allows fluent usage of the method.")]
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "Allows fluent usage of the method.")]
        public static IMappingExpression<TDataItem, TEntity> MapDependency<TDataItem, TEntity, TDependency, TKey>(
            [NotNull] this IMappingExpression<TDataItem, TEntity> mappingExpression,
            [NotNull] Expression<Func<TEntity, TDependency>> dependency,
            [NotNull] Expression<Func<TDataItem, TKey>> dependencyKey) where TDataItem : ITransactionContext
        {
            if (mappingExpression == null)
            {
                throw new ArgumentNullException(nameof(mappingExpression));
            }

            if (dependency == null)
            {
                throw new ArgumentNullException(nameof(dependency));
            }

            if (dependencyKey == null)
            {
                throw new ArgumentNullException(nameof(dependencyKey));
            }

            var propertyName = dependency.GetPropertyName();
            return mappingExpression.ForMember(
                propertyName,
                expr =>
                    expr.ResolveUsing(
                        (item, entity, obj, context) => ConditionallyMapDependency<TDataItem, TDependency, TKey>(item, dependencyKey, obj, context)));
        }

/*
        /// <summary>
        /// Conditionally creates the related entity.
        /// </summary>
        /// <param name="item">
        /// The data item.
        /// </param>
        /// <param name="relatedEntityKey">
        /// The related entity key.
        /// </param>
        /// <param name="typeResolver">
        /// The type resolver.
        /// </param>
        /// <typeparam name="TRelation">
        /// The type of relation to create.
        /// </typeparam>
        /// <typeparam name="TKey">
        /// The type of the relation key.
        /// </typeparam>
        /// <typeparam name="TDataItem">
        /// The type of data item to map from.
        /// </typeparam>
        /// <returns>
        /// The resolved relation as an <see cref="object"/>.
        /// </returns>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "Allows fluent usage of the method.")]
        public static object ConditionallyCreateRelatedEntity<TRelation, TKey, TDataItem>(
            [NotNull] this TDataItem item,
            [NotNull] Expression<Func<TDataItem, TKey>> relatedEntityKey,
            [NotNull] Func<TDataItem, TRelation> typeResolver)
            where TRelation : class where TDataItem : ITransactionContext
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            if (relatedEntityKey == null)
            {
                throw new ArgumentNullException(nameof(relatedEntityKey));
            }

            if (typeResolver == null)
            {
                throw new ArgumentNullException(nameof(typeResolver));
            }

            return item.TransactionProvider?.DependencyContainer?.GetDependency<TRelation>(relatedEntityKey.Compile().Invoke(item))
                   ?? typeResolver(item);
        }
*/

        /// <summary>
        /// Resolves or maps an entity from the existing item and key.
        /// </summary>
        /// <param name="item">
        /// The item to map from.
        /// </param>
        /// <param name="relationKey">
        /// The relation key to resolve in the dependency container.
        /// </param>
        /// <typeparam name="TDataItem">
        /// The type of data item to map from.
        /// </typeparam>
        /// <typeparam name="TKey">
        /// The type of the relation key.
        /// </typeparam>
        /// <typeparam name="TEntity">
        /// The type of the relation.
        /// </typeparam>
        /// <returns>
        /// The mapped or resolved <see cref="object"/>.
        /// </returns>
        [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Justification = "Allows fluent usage of the method.")]
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "Allows fluent usage of the method.")]
        public static object ResolveOrMapRelatedEntity<TDataItem, TKey, TEntity>(
            [NotNull] this TDataItem item,
            [NotNull] Expression<Func<TDataItem, TKey>> relationKey)
            where TDataItem : ITransactionContext
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            if (relationKey == null)
            {
                throw new ArgumentNullException(nameof(relationKey));
            }

            if (item.TransactionProvider == null)
            {
                return item;
            }

            var key = relationKey.Compile().Invoke(item);
            var existingEntity = item.TransactionProvider.DependencyContainer.GetDependency<TEntity>(key);

            if (existingEntity != null)
            {
                return existingEntity;
            }

            var mappedEntity = item.TransactionProvider.EntityMapper.Map<TEntity>(item);

            item.TransactionProvider.DependencyContainer.SetDependency(key, mappedEntity);

            return mappedEntity;
        }

        /// <summary>
        /// Resolves or maps an entity from the existing item and key.
        /// </summary>
        /// <param name="item">
        /// The item containing the relation to map from.
        /// </param>
        /// <param name="relationExpression">
        /// The relation to map from.
        /// </param>
        /// <param name="relationKey">
        /// The relation key to resolve in the dependency container.
        /// </param>
        /// <typeparam name="TDataItem">
        /// The type of data item to map from.
        /// </typeparam>
        /// <typeparam name="TRelation">
        /// The type of the relation.
        /// </typeparam>
        /// <typeparam name="TEntity">
        /// The type of the entity to retrieve.
        /// </typeparam>
        /// <returns>
        /// The mapped or resolved <see cref="object"/>.
        /// </returns>
        [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Justification = "Allows fluent usage of the method.")]
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "Allows fluent usage of the method.")]
        public static object ResolveOrMapRelatedEntity<TDataItem, TRelation, TEntity>(
            this TDataItem item,
            [NotNull] Expression<Func<TDataItem, TRelation>> relationExpression,
            Expression<Func<TRelation, object>> relationKey)
            where TDataItem : ITransactionContext
            where TRelation : ITransactionContext
            where TEntity : class
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            if (relationExpression == null)
            {
                throw new ArgumentNullException(nameof(relationExpression));
            }

            if (relationKey == null)
            {
                throw new ArgumentNullException(nameof(relationKey));
            }

            var relation = relationExpression.Compile().Invoke(item);

            if (relation == null)
            {
                return default(TEntity);
            }

            if (item.TransactionProvider == null)
            {
                return relation;
            }

            // Because this probably wasn't done when the row was instantiated.
            if (relation.TransactionProvider == null)
            {
                relation.SetTransactionProvider(item.TransactionProvider);
            }

            var key = relationKey.Compile().Invoke(relation);
            var existingEntity = item.TransactionProvider.DependencyContainer.GetDependency<TEntity>(key);

            if (existingEntity != null)
            {
                return existingEntity;
            }

            var mappedEntity = item.TransactionProvider.EntityMapper.Map<TEntity>(relation);

            item.TransactionProvider.DependencyContainer.SetDependency(key, mappedEntity);

            return mappedEntity;
        }

        /// <summary>
        /// Adds the dependency to the dependency container with the specified key.
        /// </summary>
        /// <typeparam name="TDependency">
        /// The type of dependency.
        /// </typeparam>
        /// <typeparam name="TKey">
        /// The type of the dependency key.
        /// </typeparam>
        /// <typeparam name="TDataItem">
        /// The type of data item to map from.
        /// </typeparam>
        /// <param name="item">
        /// The item containing the dependency key.
        /// </param>
        /// <param name="dependencyKey">
        /// The dependency key selector.
        /// </param>
        /// <param name="dependency">
        /// The dependency to set.
        /// </param>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "Allows fluent usage of the method.")]
        public static void SetDependency<TDependency, TKey, TDataItem>(
            [NotNull] this TDataItem item,
            [NotNull] Expression<Func<TDataItem, TKey>> dependencyKey,
            [NotNull] TDependency dependency)
            where TDataItem : ITransactionContext
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            if (dependencyKey == null)
            {
                throw new ArgumentNullException(nameof(dependencyKey));
            }

            if (dependency == null)
            {
                throw new ArgumentNullException(nameof(dependency));
            }

            if (item.TransactionProvider?.DependencyContainer == null)
            {
                return;
            }

            var key = dependencyKey.Compile().Invoke(item);

            if (item.TransactionProvider.DependencyContainer.ContainsDependency<TDependency>(key))
            {
                return;
            }

            if (Evaluate.IsDefaultValue(key) == false)
            {
                item.TransactionProvider.DependencyContainer.SetDependency(key, dependency);
            }
        }

        #endregion

        /// <summary>
        /// Gets a connection for the specified connection name.
        /// </summary>
        /// <param name="connectionStrings">
        /// The connection string collection containing the connection.
        /// </param>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <returns>
        /// A <see cref="DbConnection"/> for the specified connection string element.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="connectionStrings"/> or <paramref name="name"/> is null.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// None of the connection strings contain an element with the specified <paramref name="name"/>, or the connection string did
        /// not specify a provider.
        /// </exception>
        /// <exception cref="OperationException">
        /// The provider specified in the connection string was associated with a factory that created a null connection.
        /// </exception>
        public static DbConnection GetConnection(this ConnectionStringSettingsCollection connectionStrings, string name)
        {
            if (connectionStrings == null)
            {
                throw new ArgumentNullException(nameof(connectionStrings));
            }

            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            var connectionString = connectionStrings[name];

            if (connectionString == null)
            {
                throw new ArgumentException(ValidationMessages.ConnectionStringNameNotFound, nameof(name));
            }

            string providerName = connectionString.ProviderName;

            if (providerName == null)
            {
                throw new ArgumentException(ValidationMessages.ConnectionStringDidNotSpecifyProvider, nameof(name));
            }

            var providerExists = DbProviderFactories.GetFactoryClasses().Rows.Cast<DataRow>().Any(r => r[2].Equals(providerName));

            if (providerExists == false)
            {
                throw new ArgumentException(ValidationMessages.ConnectionStringProviderDoesNotExist, nameof(name));
            }

            var factory = DbProviderFactories.GetFactory(providerName);
            DbConnection connection = null;

            try
            {
                connection = factory.CreateConnection();

                if (connection == null)
                {
                    throw new OperationException(factory, ValidationMessages.DataConnectionFactoryReturnedNull);
                }

                connection.ConnectionString = connectionString.ConnectionString;
                return connection;
            }
            catch
            {
                connection?.Dispose();
                throw;
            }
        }

        #region Obsolete

        /*
                /// <summary>
                /// Gets a dependency from the repository and throws an exception if the dependency is not found.
                /// </summary>
                /// <param name="repository">
                /// The repository.
                /// </param>
                /// <param name="candidate">
                /// An item that contains the dependency's unique ID or key.
                /// </param>
                /// <typeparam name="T">
                /// The type of dependency to retrieve.
                /// </typeparam>
                /// <typeparam name="TItem">
                /// The type of item that represents the dependency.
                /// </typeparam>
                /// <returns>
                /// The dependency of type <typeparamref name="T"/>.
                /// </returns>
                /// <exception cref="ArgumentNullException">
                /// <paramref name="candidate"/> is null.
                /// </exception>
                /// <exception cref="OperationException">
                /// The dependency of type <typeparamref name="T"/> referenced by <paramref name="candidate"/> was not found in the repository.
                /// </exception>
                [Obsolete("Check locally that the dependency is set instead.")]
                public static T GetDependency<T, TItem>(this IEntityRepository<T> repository, TItem candidate) where T : IValidatingEntity
                {
                    if (repository == null)
                    {
                        throw new ArgumentNullException(nameof(repository));
                    }

                    if (Evaluate.IsNull(candidate))
                    {
                        throw new ArgumentNullException(nameof(candidate));
                    }

                    var dependency = repository.FirstOrDefault(candidate);

                    if (Evaluate.IsNull(dependency))
                    {
                        throw new OperationException(candidate, string.Format(ErrorMessages.RepositoryDependencyDoesNotExist, typeof(T).Name, candidate));
                    }

                    return dependency;
                }
        */

        /*
                /// <summary>
                /// Maps a dependency of the entity to the row.
                /// </summary>
                /// <param name="mappingExpression">
                /// The mapping expression.
                /// </param>
                /// <param name="dependency">
                /// The dependency.
                /// </param>
                /// <typeparam name="TDataItem">
                /// The data item type.
                /// </typeparam>
                /// <typeparam name="TEntity">
                /// The entity type.
                /// </typeparam>
                /// <returns>
                /// The current mapping expression.
                /// </returns>
                [Obsolete("Use MapDependency overload to specify the dependency key.")]
                public static IMappingExpression<TDataItem, TEntity> MapDependency<TDataItem, TEntity>(
                    [NotNull] this IMappingExpression<TDataItem, TEntity> mappingExpression,
                    [NotNull] Expression<Func<TEntity, object>> dependency)
                {
                    if (mappingExpression == null)
                    {
                        throw new ArgumentNullException(nameof(mappingExpression));
                    }

                    if (dependency == null)
                    {
                        throw new ArgumentNullException(nameof(dependency));
                    }

                    return mappingExpression.ForMember(dependency.GetPropertyName(), expr => expr.MapFrom(source => source));
                }
        */

        /*
        /// <summary>
        /// Maps one or more members of the same member type to <typeparamref name="TEntity"/> properties based on a TypePropertyName
        /// naming convention.
        /// </summary>
        /// <param name="expression">
        /// The current mapping expression.
        /// </param>
        /// <param name="entityProperties">
        /// The entity properties to map. All properties must be of the same type.
        /// </param>
        /// <typeparam name="TDataItem">
        /// The type of data item being mapped from.
        /// </typeparam>
        /// <typeparam name="TEntity">
        /// The type of entity being mapped to.
        /// </typeparam>
        /// <typeparam name="TMember">
        /// The type of the member or members to map.
        /// </typeparam>
        /// <returns>
        /// The current mapping expression with the mapped properties.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="expression"/> or <paramref name="entityProperties"/> is null.
        /// </exception>
        [Obsolete("Use MapEntityProperty() instead.")]
        public static IMappingExpression<TDataItem, TEntity> MapByConvention<TDataItem, TEntity, TMember>(
            [NotNull] this IMappingExpression<TDataItem, TEntity> expression,
            [NotNull] params Expression<Func<TEntity, TMember>>[] entityProperties)
        {
            if (expression == null)
            {
                throw new ArgumentNullException(nameof(expression));
            }

            if (entityProperties == null)
            {
                throw new ArgumentNullException(nameof(entityProperties));
            }

            return MapByConvention(expression, typeof(TEntity).Name, entityProperties);
        }
*/

        /*
                /// <summary>
                /// Maps one or more members of the same member type to <typeparamref name="TEntity"/> properties based on a PrefixPropertyName
                /// naming convention.
                /// </summary>
                /// <param name="expression">
                /// The current mapping expression.
                /// </param>
                /// <param name="prefix">
                /// The prefix used for the data row properties.
                /// </param>
                /// <param name="entityProperties">
                /// The entity properties to map. All properties must be of the same type.
                /// </param>
                /// <typeparam name="TDataItem">
                /// The type of data item being mapped from.
                /// </typeparam>
                /// <typeparam name="TEntity">
                /// The type of entity being mapped to.
                /// </typeparam>
                /// <typeparam name="TMember">
                /// The type of the member or members to map.
                /// </typeparam>
                /// <returns>
                /// The current mapping expression with the mapped properties.
                /// </returns>
                /// <exception cref="ArgumentNullException">
                /// <paramref name="expression"/> or <paramref name="entityProperties"/> is null.
                /// </exception>
                [Obsolete("Use MapEntityProperty() instead.")]
                public static IMappingExpression<TDataItem, TEntity> MapByConvention<TDataItem, TEntity, TMember>(
                    [NotNull] this IMappingExpression<TDataItem, TEntity> expression,
                    [NotNull] string prefix,
                    [NotNull] params Expression<Func<TEntity, TMember>>[] entityProperties)
                {
                    if (expression == null)
                    {
                        throw new ArgumentNullException(nameof(expression));
                    }

                    if (string.IsNullOrWhiteSpace(prefix))
                    {
                        throw new ArgumentNullException(nameof(prefix));
                    }

                    if (entityProperties == null)
                    {
                        throw new ArgumentNullException(nameof(entityProperties));
                    }

                    throw new NotSupportedException();

                    ////foreach (var entityProperty in entityProperties)
                    ////{
                    ////    var property = entityProperty;

                    ////    var sourcePropertyName = String.Concat(prefix, property.GetPropertyName());
                    ////    var sourceProperty = typeof(TDataItem).GetProperty(sourcePropertyName);

                    ////    if (sourceProperty == null)
                    ////    {
                    ////        throw new BusinessException(entityProperty, ValidationMessages.PropertyMappingNotFoundByConvention);
                    ////    }

                    ////    var sourceIsNullableButTargetIsNot = sourceProperty.PropertyType.IsNullableType()
                    ////                                         && entityProperty.Body.Type.IsNullableType() == false;

                    ////    if (sourceIsNullableButTargetIsNot)
                    ////    {
                    ////        var message = String.Format(
                    ////            ValidationMessages.NullablePropertyCannotBeMappedByConvention,
                    ////            typeof(TEntity).Name,
                    ////            entityProperty.GetPropertyName());

                    ////        throw new BusinessException(entityProperty, message);

                    ////        ////expression.ForMember(
                    ////        ////    entityProperty.GetPropertyName(),
                    ////        ////    expr => expr.ResolveUsing(item => GetValueOrDefault<TDataItem, TMember>(sourceProperty, item)));
                    ////    }

                    ////    expression.ForMember(entityProperty.GetPropertyName(), expr => expr.MapFrom<TMember>(sourcePropertyName));
                    ////}

                    ////return expression;
                }
        */

        /*
                /// <summary>
                /// Maps one or more members of the same member type to <typeparamref name="TDependency"/> properties based on a PrefixPropertyName
                /// naming convention.
                /// </summary>
                /// <param name="expression">
                /// The current mapping expression.
                /// </param>
                /// <param name="profile">
                /// The profile.
                /// </param>
                /// <param name="dependencyProperty">
                /// The dependency Property.
                /// </param>
                /// <param name="entityProperties">
                /// The entity properties to map. All properties must be of the same type.
                /// </param>
                /// <typeparam name="TEntity">
                /// The type of entity containing the dependency to map by convention.
                /// </typeparam>
                /// <typeparam name="TDataItem">
                /// The type of data item being mapped from.
                /// </typeparam>
                /// <typeparam name="TDependency">
                /// The type of entity being mapped to.
                /// </typeparam>
                /// <typeparam name="TMember">
                /// The type of the member or members to map.
                /// </typeparam>
                /// <returns>
                /// The current mapping expression with the mapped properties.
                /// </returns>
                /// <exception cref="ArgumentNullException">
                /// <paramref name="expression"/> or <paramref name="entityProperties"/> is null.
                /// </exception>
                [Obsolete("Use MapEntityProperty() instead.")]
                public static IMappingExpression<TDataItem, TDependency> MapByConvention<TEntity, TDataItem, TDependency, TMember>(
                    [NotNull] this IMappingExpression<TDataItem, TDependency> expression,
                    [NotNull] EntityMappingProfile<TEntity, TDataItem> profile,
                    [NotNull] Expression<Func<TEntity, TDependency>> dependencyProperty,
                    [NotNull] params Expression<Func<TDependency, TMember>>[] entityProperties)
                    where TDataItem : ITransactionContext
                {
                    if (expression == null)
                    {
                        throw new ArgumentNullException(nameof(expression));
                    }

                    if (profile == null)
                    {
                        throw new ArgumentNullException(nameof(profile));
                    }

                    if (dependencyProperty == null)
                    {
                        throw new ArgumentNullException(nameof(dependencyProperty));
                    }

                    if (entityProperties == null)
                    {
                        throw new ArgumentNullException(nameof(entityProperties));
                    }

                    return expression.MapByConvention(dependencyProperty.GetPropertyName(), entityProperties);
                }
        */

        /*
                /// <summary>
                /// Maps a member that is also a required constructor parameter when the source property name is not the same as the 
                /// constructor parameter name.
                /// </summary>
                /// <param name="mappingExpression">
                /// The mapping expression.
                /// </param>
                /// <param name="entityProperty">
                /// The entity property to map.
                /// </param>
                /// <param name="sourceProperty">
                /// The source of the value for the constructor.
                /// </param>
                /// <typeparam name="TDataItem">
                /// The type of data item.
                /// </typeparam>
                /// <typeparam name="TEntity">
                /// The type of entity.
                /// </typeparam>
                /// <typeparam name="TDest">
                /// The type of the destination member being mapped.
                /// </typeparam>
                /// <typeparam name="TSource">
                /// The type of the source member being mapped.
                /// </typeparam>
                /// <returns>
                /// The current mapping expression.
                /// </returns>
                [Obsolete("Use MapEntityProperty() instead.")]
                public static IMappingExpression<TDataItem, TEntity> MapForCtor<TDataItem, TEntity, TDest, TSource>(
                    [NotNull] this IMappingExpression<TDataItem, TEntity> mappingExpression,
                    [NotNull] Expression<Func<TEntity, TDest>> entityProperty,
                    [NotNull] Expression<Func<TDataItem, TSource>> sourceProperty)
                {
                    if (mappingExpression == null)
                    {
                        throw new ArgumentNullException(nameof(mappingExpression));
                    }

                    if (entityProperty == null)
                    {
                        throw new ArgumentNullException(nameof(entityProperty));
                    }

                    if (sourceProperty == null)
                    {
                        throw new ArgumentNullException(nameof(sourceProperty));
                    }

                    var targetPropertyName = entityProperty.GetPropertyName();

                    if (string.IsNullOrWhiteSpace(targetPropertyName))
                    {
                        throw new OperationException(entityProperty, ErrorMessages.PropertyNameNotSet);
                    }

                    string ctorParamName = char.ToLowerInvariant(targetPropertyName[0]) + targetPropertyName.Substring(1);

                    return mappingExpression.ForMember(targetPropertyName, expr => expr.MapFrom(sourceProperty))
                        .ForCtorParam(ctorParamName, expr => expr.MapFrom(sourceProperty));
                }
        */

        #endregion

        /// <summary>
        /// Gets an entity reference from the specified member expression.
        /// </summary>
        /// <param name="memberExpression">
        /// The member expression.
        /// </param>
        /// <returns>
        /// An <see cref="EntityReference"/> for the specified expression.
        /// </returns>
        /// <exception cref="OperationException">
        /// <paramref name="memberExpression"/> does not have a declaring type.
        /// </exception>
        public static EntityReference GetEntityReference([NotNull] this LambdaExpression memberExpression)
        {
            if (memberExpression == null)
            {
                throw new ArgumentNullException(nameof(memberExpression));
            }

            var attributeMember = memberExpression.GetMember();

            if (attributeMember == null)
            {
                throw new OperationException(memberExpression, ValidationMessages.SelectorCannotBeEvaluated);
            }

            // Check whether there's a RelatedEntityAttribute that will override the natural type.
            // Do not use .Member.DeclaringType in place of .Expression.Type because this could be the base type of an inherited type.
            var relatedEntityAttribute = attributeMember.Member.GetCustomAttribute<RelatedEntityAttribute>();
            var declaringType = relatedEntityAttribute?.EntityType ?? attributeMember.Expression.Type;

            if (declaringType == null)
            {
                throw new OperationException(memberExpression, ValidationMessages.PropertyMustHaveDeclaringType);
            }

            // This will be null for RelatedEntityAttributes, so we check from the attribute directly (below).
            var entityMember = attributeMember.Expression as MemberExpression;

            // The definition provider will handle identical alias/entity names.
            return new EntityReference { EntityType = declaringType, EntityAlias = relatedEntityAttribute?.EntityAlias ?? entityMember?.Member.Name };
        }

        /// <summary>
        /// Gets the constructor parameter for the specified property.
        /// </summary>
        /// <param name="targetProperty">
        /// The target property.
        /// </param>
        /// <typeparam name="TEntity">
        /// The type of entity that contains the property.
        /// </typeparam>
        /// <typeparam name="TDest">
        /// The type of property.
        /// </typeparam>
        /// <returns>
        /// The <see cref="ParameterInfo"/> for the specified property.
        /// </returns>
        internal static ParameterInfo GetConstructorParam<TEntity, TDest>([NotNull] this Expression<Func<TEntity, TDest>> targetProperty)
        {
            if (targetProperty == null)
            {
                throw new ArgumentNullException(nameof(targetProperty));
            }

            // Get the constructor with the largest number of parameters.
            var constructorInfo =
                typeof(TEntity).GetConstructors(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                    .OrderByDescending(x => x.GetParameters().Length)
                    .First();

            // Look for the name of the member.
            var propertyName = targetProperty.GetPropertyName();
            var constructorParamName = String.Concat(Char.ToLower(propertyName.First()), propertyName.Substring(1));

            return constructorInfo.GetParameters().FirstOrDefault(info => info.Name == constructorParamName);
        }

        /// <summary>
        /// Conditionally maps a dependency.
        /// </summary>
        /// <param name="item">
        /// The source item.
        /// </param>
        /// <param name="dependencyKey">
        /// The dependency key.
        /// </param>
        /// <param name="obj">
        /// The object.
        /// </param>
        /// <param name="context">
        /// The resolution context. 
        /// </param>
        /// <typeparam name="TDataItem">
        /// The type of data item to map form.
        /// </typeparam>
        /// <typeparam name="TDependency">
        /// The type of dependency to map to.
        /// </typeparam>
        /// <typeparam name="TKey">
        /// The type of dependency key.
        /// </typeparam>
        /// <returns>
        /// A <typeparamref name="TDependency"/> using the default mapping operation or null if the key is not set.
        /// </returns>
        private static TDependency ConditionallyMapDependency<TDataItem, TDependency, TKey>(
            TDataItem item,
            Expression<Func<TDataItem, TKey>> dependencyKey,
            object obj,
            ResolutionContext context) where TDataItem : ITransactionContext
        {
            // We already have this for some reason.
            if (obj is TDependency)
            {
                return (TDependency)obj;
            }

            var key = dependencyKey.Compile().Invoke(item);

            // This prevents an endless recursion for optional dependencies that aren't set.
            if (Evaluate.IsSet(key) == false)
            {
                return default(TDependency);
            }

            var result = default(TDependency);

            if (item.TransactionProvider != null)
            {
                result = item.TransactionProvider.DependencyContainer.GetDependency<TDependency>(key);
            }

            if (Evaluate.IsNull(result))
            {
                result = context.Mapper.Map<TDependency>(item);

                // Set the dependency as the mapped item.
                item.TransactionProvider?.DependencyContainer.SetDependency(key, result);
            }

            return result;
        }

        /// <summary>
        /// Gets the property name from a selector.
        /// </summary>
        /// <param name="selector">
        /// An expression that selects a property.
        /// </param>
        /// <returns>
        /// The property name as a <see cref="string"/>.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// The expression cannot be evaluated for a member name, or the member is not a property.
        /// </exception>
        public static PropertyInfo GetProperty(this LambdaExpression selector)
        {
            if (selector == null)
            {
                throw new ArgumentNullException(nameof(selector));
            }

            if (!(selector.Body is MemberExpression body))
            {
                if (!(selector.Body is UnaryExpression unaryBody))
                {
                    throw new ArgumentException(ValidationMessages.SelectorCannotBeEvaluated, nameof(selector));
                }

                body = unaryBody.Operand as MemberExpression;

                if (body == null)
                {
                    throw new ArgumentException(ValidationMessages.SelectorCannotBeEvaluated, nameof(selector));
                }
            }

            var propertyInfo = body.Member as PropertyInfo;

            if (propertyInfo == null)
            {
                throw new ArgumentException(ValidationMessages.SelectorCannotBeEvaluated, nameof(selector));
            }

            return propertyInfo;
        }

        /// <summary>
        /// Determines whether an exception contains an inner exception of an exact type.
        /// </summary>
        /// <param name="exception">
        /// The exception to evaluate.
        /// </param>
        /// <typeparam name="TException">
        /// The type of inner exception to evaluate for.
        /// </typeparam>
        /// <returns>
        /// <c>true</c> if the inner exception is the exact type of the <typeparamref name="TException"/> type; otherwise, <c>false</c>.
        /// </returns>
        [SuppressMessage(
            "Microsoft.Design", 
            "CA1004:GenericMethodsShouldProvideTypeParameter", 
            Justification = "This is now common practice for methods with a single generic parameter.")]
        public static bool ContainsInnerException<TException>(this Exception exception)
            where TException : Exception
        {
            return exception?.InnerException != null && exception.InnerException.GetType() == typeof(TException);
        }

        /// <summary>
        /// Retrieves the innermost exception message that is an instance of the specified types to include.
        /// </summary>
        /// <param name="exception">
        /// The current exception.
        /// </param>
        /// <param name="typesToExclude">
        /// The exception types to exclude when searching for the inner exception.
        /// </param>
        /// <returns>
        /// The error message as a <see cref="string"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="exception"/> or <paramref name="typesToExclude"/> are null.
        /// </exception>
        public static string GetInnerExceptionMessage(this Exception exception, params Type[] typesToExclude)
        {
            if (exception == null)
            {
                throw new ArgumentNullException(nameof(exception));
            }

            if (typesToExclude == null)
            {
                throw new ArgumentNullException(nameof(typesToExclude));
            }

            string errorMessage = null;

            var currentException = exception;

            while (currentException != null)
            {
                if (!typesToExclude.Any(x => x.IsInstanceOfType(currentException)))
                {
                    errorMessage = currentException.Message;
                }

                currentException = currentException.InnerException;
            }

            return errorMessage ?? exception.Message;
        }

        /// <summary>
        /// The friendly generic type format string.
        /// </summary>
        private const string FriendlyGenericTypeFormat = "{0}<{1}>";

        /// <summary>
        /// The type name selector.
        /// </summary>
        private static readonly Func<Type, string> TypeNameSelector = x => x.Name;

        /// <summary>
        /// Converts a type to its runtime name, including generic arguments.
        /// </summary>
        /// <param name="type">
        /// The type to convert.
        /// </param>
        /// <returns>
        /// The runtime name, including generic arguments, as a <see cref="string"/>.
        /// </returns>
        public static string ToRuntimeName(this Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            if (type.GetGenericArguments().Any())
            {
                return String.Format(FriendlyGenericTypeFormat, type.Name, String.Join(", ", type.GetGenericArguments().Select(TypeNameSelector)));
            }

            return type.Name;
        }

        /// <summary>
        /// Gets the member of the lambda expression.
        /// </summary>
        /// <param name="expression">
        /// The expression to evaluate.
        /// </param>
        /// <returns>
        /// The <see cref="MemberExpression"/> in the body of the expression.
        /// </returns>
        /// <exception cref="OperationException">
        /// The expression cannot be evaluated as a property.
        /// </exception>
        public static MemberExpression GetMember(this LambdaExpression expression)
        {
            if (expression == null)
            {
                throw new ArgumentNullException(nameof(expression));
            }

            if (expression.Body is MemberExpression body)
            {
                return body;
            }

            if (!(expression.Body is UnaryExpression unaryBody))
            {
                throw new OperationException(expression, ValidationMessages.SelectorCannotBeEvaluated);
            }

            body = unaryBody.Operand as MemberExpression;

            if (body == null)
            {
                throw new OperationException(expression, ValidationMessages.SelectorCannotBeEvaluated);
            }

            return body;
        }

        /// <summary>
        /// Gets the property name from a selector.
        /// </summary>
        /// <param name="selector">
        /// An expression that selects a property.
        /// </param>
        /// <returns>
        /// The property name as a <see cref="string"/>.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// The expression cannot be evaluated for a member name.
        /// </exception>
        public static string GetPropertyName(this LambdaExpression selector)
        {
            if (selector == null)
            {
                throw new ArgumentNullException(nameof(selector));
            }

            return selector.GetMember().Member.Name;
        }

        /// <summary>
        /// The get property value.
        /// </summary>
        /// <param name="entity">
        /// The entity.
        /// </param>
        /// <param name="propertyName">
        /// The property name.
        /// </param>
        /// <typeparam name="T">
        /// The type of entity to get the property value for.
        /// </typeparam>
        /// <returns>
        /// The <see cref="object"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="propertyName"/> or <paramref name="entity"/> is null.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="propertyName"/> specifies a property that does not exist in <typeparamref name="T"/>.
        /// </exception>
        public static object GetPropertyValue<T>(this T entity, string propertyName)
        {
            if (String.IsNullOrWhiteSpace(propertyName))
            {
                throw new ArgumentNullException(nameof(propertyName));
            }

            if (Evaluate.IsNull(entity))
            {
                throw new ArgumentNullException(nameof(entity));
            }

            var info = typeof(T).GetProperty(propertyName);

            if (info == null)
            {
                throw new ArgumentException(
                    String.Format(ValidationMessages.TypeDoesNotContainProperty, typeof(T).Name, propertyName),
                    nameof(propertyName));
            }

            return Core.ExtensionMethods.GetPropertyValue(info, entity);
        }
    }
}
