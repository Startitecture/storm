// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExtensionMethods.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.Repository
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;

    using AutoMapper;

    using JetBrains.Annotations;

    using Startitecture.Core;
    using Startitecture.Orm.Common;
    using Startitecture.Orm.Query;
    using Startitecture.Resources;

    /// <summary>
    /// The extension methods.
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
            [NotNull] Expression<Func<TDataItem, TKey>> keySelection)
            where TDataItem : ITransactionContext
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
                mappingExpression.ForMember(targetPropertyName, expr => expr.MapFrom(sourceProperty)).ForCtorParam(
                    constructorParam.Name,
                    expr => expr.MapFrom(sourceProperty));
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
            [NotNull] Expression<Func<TDataItem, TKey>> dependencyKey)
            where TDataItem : ITransactionContext
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
                expr => expr.ResolveUsing(
                    (item, entity, obj, context) => ConditionallyMapDependency<TDataItem, TDependency, TKey>(item, dependencyKey, obj, context)));
        }

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
            where TDataItem : ITransactionContext where TRelation : ITransactionContext where TEntity : class
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
            var constructorInfo = typeof(TEntity).GetConstructors(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                .OrderByDescending(x => x.GetParameters().Length).First();

            // Look for the name of the member.
            var propertyName = targetProperty.GetPropertyName();
            var constructorParamName = string.Concat(char.ToLower(propertyName.First()), propertyName.Substring(1));

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
            ResolutionContext context)
            where TDataItem : ITransactionContext
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
        /// Selects items based on the data item type of the current repository.
        /// </summary>
        /// <param name="repository">
        /// The current repository.
        /// </param>
        /// <param name="selectExpressions">
        /// The column selection expressions.
        /// </param>
        /// <typeparam name="TEntity">
        /// The type of entity stored in the repository.
        /// </typeparam>
        /// <typeparam name="TDataItem">
        /// The type of data item that represents the entity.
        /// </typeparam>
        /// <returns>
        /// An <see cref="Startitecture.Orm.Query.ItemSelection{TItem}"/> for the specified <typeparamref name="TDataItem"/>
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="repository"/> or <paramref name="selectExpressions"/> is null.
        /// </exception>
        public static ItemSelection<TDataItem> Select<TEntity, TDataItem>(
            [NotNull] this EntityRepository<TEntity, TDataItem> repository,
            [NotNull] params Expression<Func<TDataItem, object>>[] selectExpressions) 
            where TDataItem : class, ITransactionContext, new()
        {
            if (repository == null)
            {
                throw new ArgumentNullException(nameof(repository));
            }

            if (selectExpressions == null)
            {
                throw new ArgumentNullException(nameof(selectExpressions));
            }

            return Sql.SqlSelect.From<TDataItem>();
        }
    }
}