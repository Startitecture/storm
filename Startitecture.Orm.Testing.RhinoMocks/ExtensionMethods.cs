// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExtensionMethods.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// <summary>
//   The extension methods.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.Testing.RhinoMocks
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;

    using Common;

    using JetBrains.Annotations;

    using Rhino.Mocks;

    using Startitecture.Core;
    using Startitecture.Orm.Query;
    using Startitecture.Orm.Schema;
    using Startitecture.Orm.Sql;
    using Startitecture.Resources;

    /// <summary>
    /// The extension methods.
    /// </summary>
    public static class ExtensionMethods
    {
        /// <summary>
        /// The number generator.
        /// </summary>
        private static readonly Random NumberGenerator = new Random();

        #region Repository Providers

        /// <summary>
        /// Creates stubs to save and select an existing item.
        /// </summary>
        /// <param name="repositoryProvider">
        /// The repository provider to stub. 
        /// </param>
        /// <param name="entity">
        /// The existing entity. 
        /// </param>
        /// <typeparam name="TDataItem">
        /// The type of data item. 
        /// </typeparam>
        /// <returns>
        /// The <see cref="Startitecture.Orm.Common.IRepositoryProvider"/> with methods stubbed for an existing item.
        /// </returns>
        public static IRepositoryProvider StubForExistingItem<TDataItem>(
            [NotNull] this IRepositoryProvider repositoryProvider,
            [NotNull] object entity)
            where TDataItem : class, ITransactionContext
        {
            if (repositoryProvider == null)
            {
                throw new ArgumentNullException(nameof(repositoryProvider));
            }

            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            if (repositoryProvider.EntityMapper == null)
            {
                throw new InvalidOperationException("The entity mapper must be set on the repository provider.");
            }

            var dataItem = repositoryProvider.EntityMapper.Map<TDataItem>(entity);

            repositoryProvider.Stub(provider => provider.Contains(Arg<ItemSelection<TDataItem>>.Is.Anything)).Return(true);
            repositoryProvider.Stub(provider => provider.GetFirstOrDefault(Arg<ItemSelection<TDataItem>>.Is.Anything)).Return(dataItem);
            repositoryProvider.Stub(provider => provider.Save(Arg<TDataItem>.Is.Anything)).Return(dataItem);
            return repositoryProvider;
        }

        /// <summary>
        /// Creates stubs to save and select an existing item.
        /// </summary>
        /// <param name="repositoryProvider">
        /// The repository provider to stub. 
        /// </param>
        /// <typeparam name="TDataItem">
        /// The type of data item. 
        /// </typeparam>
        /// <returns>
        /// The <see cref="Startitecture.Orm.Common.IRepositoryProvider"/> with methods stubbed for a new item.
        /// </returns>
        public static IRepositoryProvider StubForNewItem<TDataItem>(
            [NotNull] this IRepositoryProvider repositoryProvider)
            where TDataItem : class, ITransactionContext
        {
            if (repositoryProvider == null)
            {
                throw new ArgumentNullException(nameof(repositoryProvider));
            }

            repositoryProvider.Stub(provider => provider.Contains(Arg<ItemSelection<TDataItem>>.Is.Anything)).Return(false);
            repositoryProvider.Stub(provider => provider.Save(Arg<TDataItem>.Is.Anything)).Return(null)
                .WhenCalled(
                    invocation =>
                    {
                        // Finds the item in the argument list.
                        var item = Enumerable.OfType<TDataItem>(invocation.Arguments).First();

                        // If the item needs to have an auto-incremented ID set, we set it to a random number.
                        var primaryKeyAttribute = ((Object)item).GetType().GetCustomAttributes<PrimaryKeyAttribute>().FirstOrDefault();
                        var autoIncrement = (primaryKeyAttribute?.AutoIncrement).GetValueOrDefault();

                        if (primaryKeyAttribute != null && autoIncrement)
                        {
                            var propertyName = primaryKeyAttribute.ColumnName;
                            var property = typeof(TDataItem).GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance);
                            var propertySetter = property.SetMethod;
                            propertySetter.Invoke(item, new object[] { NumberGenerator.Next(1, Int32.MaxValue) });
                        }

                        invocation.ReturnValue = item;
                    });

            return repositoryProvider;
        }

        /// <summary>
        /// Stubs a repository provider for a list of items.
        /// </summary>
        /// <param name="repositoryProvider">
        /// The repository provider.
        /// </param>
        /// <param name="target">
        /// The target of the items.
        /// </param>
        /// <typeparam name="TDataItem">
        /// The type of data item. 
        /// </typeparam>
        /// <returns>
        /// The <see cref="Startitecture.Orm.Common.IRepositoryProvider"/> with methods stubbed for a list of items.
        /// </returns>
        public static IRepositoryProvider StubForList<TDataItem>(
            [NotNull] this IRepositoryProvider repositoryProvider,
            [NotNull] List<TDataItem> target)
            where TDataItem : ITransactionContext
        {
            return StubForList(repositoryProvider, target, null);
        }

        /// <summary>
        /// Stubs a repository provider for a list of items.
        /// </summary>
        /// <param name="repositoryProvider">
        /// The repository provider.
        /// </param>
        /// <param name="target">
        /// The target of the items.
        /// </param>
        /// <param name="primaryKey">
        /// The primary Key.
        /// </param>
        /// <typeparam name="TDataItem">
        /// The type of data item. 
        /// </typeparam>
        /// <returns>
        /// The <see cref="Startitecture.Orm.Common.IRepositoryProvider"/> with methods stubbed for a list of items.
        /// </returns>
        public static IRepositoryProvider StubForList<TDataItem>(
            [NotNull] this IRepositoryProvider repositoryProvider,
            [NotNull] List<TDataItem> target,
            Expression<Func<TDataItem, object>> primaryKey)
            where TDataItem : ITransactionContext
        {
            if (repositoryProvider == null)
            {
                throw new ArgumentNullException(nameof(repositoryProvider));
            }

            if (target == null)
            {
                throw new ArgumentNullException(nameof(target));
            }

            if (primaryKey != null)
            {
                UpdateListKeys(target, primaryKey.GetPropertyName());
            }
            else
            {
                UpdateListKeys(target);
            }

            repositoryProvider.Stub(provider => provider.GetSelection(Arg<ItemSelection<TDataItem>>.Is.Anything)).Return(target);
            return repositoryProvider;
        }

        /// <summary>
        /// Stubs a repository provider for a list of items.
        /// </summary>
        /// <param name="repositoryProvider">
        /// The repository provider.
        /// </param>
        /// <param name="source">
        /// The source of the items.
        /// </param>
        /// <param name="target">
        /// The target of the items.
        /// </param>
        /// <typeparam name="TEntity">
        /// The type of entity. 
        /// </typeparam>
        /// <typeparam name="TDataItem">
        /// The type of data item. 
        /// </typeparam>
        /// <returns>
        /// The <see cref="Startitecture.Orm.Common.IRepositoryProvider"/> with methods stubbed for a list of items.
        /// </returns>
        public static IRepositoryProvider StubForList<TEntity, TDataItem>(
            [NotNull] this IRepositoryProvider repositoryProvider,
            [NotNull] IEnumerable<TEntity> source,
            [NotNull] List<TDataItem> target)
            where TDataItem : ITransactionContext
        {
            return StubForList(repositoryProvider, source, target, null);
        }

        /// <summary>
        /// Stubs a repository provider for a list of items.
        /// </summary>
        /// <param name="repositoryProvider">
        /// The repository provider.
        /// </param>
        /// <param name="source">
        /// The source of the items.
        /// </param>
        /// <param name="target">
        /// The target of the items.
        /// </param>
        /// <param name="primaryKey">
        /// The primary Key.
        /// </param>
        /// <typeparam name="TEntity">
        /// The type of entity. 
        /// </typeparam>
        /// <typeparam name="TDataItem">
        /// The type of data item. 
        /// </typeparam>
        /// <returns>
        /// The <see cref="Startitecture.Orm.Common.IRepositoryProvider"/> with methods stubbed for a list of items.
        /// </returns>
        public static IRepositoryProvider StubForList<TEntity, TDataItem>(
            [NotNull] this IRepositoryProvider repositoryProvider,
            [NotNull] IEnumerable<TEntity> source,
            [NotNull] List<TDataItem> target,
            Expression<Func<TDataItem, object>> primaryKey)
            where TDataItem : ITransactionContext
        {
            if (repositoryProvider == null)
            {
                throw new ArgumentNullException(nameof(repositoryProvider));
            }

            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (target == null)
            {
                throw new ArgumentNullException(nameof(target));
            }

            if (repositoryProvider.EntityMapper == null)
            {
                throw new InvalidOperationException("The entity mapper must be set on the repository provider.");
            }

            target.Clear();
            var items = repositoryProvider.EntityMapper.Map<List<TDataItem>>(source);
            target.AddRange(items);

            if (primaryKey != null)
            {
                UpdateListKeys(target, primaryKey.GetPropertyName());
            }
            else
            {
                UpdateListKeys(target);
            }

            repositoryProvider.Stub(provider => provider.GetSelection(Arg<ItemSelection<TDataItem>>.Is.Anything)).Return(target);
            return repositoryProvider;
        }

        #endregion

        #region Repository Adapters

        /// <summary>
        /// Creates stubs to save and select an existing item.
        /// </summary>
        /// <param name="repositoryAdapter">
        /// The repository adapter to stub. 
        /// </param>
        /// <param name="item">
        /// The existing item. 
        /// </param>
        /// <typeparam name="TDataItem">
        /// The type of data item. 
        /// </typeparam>
        /// <returns>
        /// The <see cref="Startitecture.Orm.Common.IRepositoryAdapter"/> with methods stubbed for an existing item.
        /// </returns>
        public static IRepositoryAdapter StubForExistingItem<TDataItem>(
            [NotNull] this IRepositoryAdapter repositoryAdapter,
            [NotNull] TDataItem item)
            where TDataItem : class, ITransactionContext
        {
            if (repositoryAdapter == null)
            {
                throw new ArgumentNullException(nameof(repositoryAdapter));
            }

            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            repositoryAdapter.Stub(adapter => adapter.Contains(Arg<ItemSelection<TDataItem>>.Is.Anything)).Return(true);

            // TODO: Clone the item when returning it.
            repositoryAdapter.Stub(adapter => adapter.FirstOrDefault(Arg<ItemSelection<TDataItem>>.Is.Anything)).Return(item);
            repositoryAdapter.Stub(
                adapter =>
                adapter.Update(
                    Arg<TDataItem>.Is.Anything,
                    Arg<ItemSelection<TDataItem>>.Is.Anything,
                    Arg<Expression<Func<TDataItem, object>>>.Is.Anything)).Return(1);

            return repositoryAdapter;
        }

        /// <summary>
        /// Creates stubs to save and select an existing item.
        /// </summary>
        /// <param name="repositoryAdapter">
        /// The repository adapter to stub. 
        /// </param>
        /// <param name="entity">
        /// The existing entity. 
        /// </param>
        /// <param name="mapper">
        /// The entity mapper. 
        /// </param>
        /// <typeparam name="TDataItem">
        /// The type of data item. 
        /// </typeparam>
        /// <returns>
        /// The <see cref="Startitecture.Orm.Common.IRepositoryAdapter"/> with methods stubbed for an existing item.
        /// </returns>
        public static IRepositoryAdapter StubForExistingItem<TDataItem>(
            [NotNull] this IRepositoryAdapter repositoryAdapter,
            [NotNull] object entity,
            [NotNull] IEntityMapper mapper)
            where TDataItem : class, ITransactionContext
        {
            if (repositoryAdapter == null)
            {
                throw new ArgumentNullException(nameof(repositoryAdapter));
            }

            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            if (mapper == null)
            {
                throw new ArgumentNullException(nameof(mapper));
            }

            repositoryAdapter.Stub(adapter => adapter.Contains(Arg<ItemSelection<TDataItem>>.Is.Anything)).Return(true);

            // Ensures that the data item is fresh every time. TODO: We may need this in other areas.
            repositoryAdapter.Stub(adapter => adapter.FirstOrDefault(Arg<ItemSelection<TDataItem>>.Is.Anything)).Return(null).WhenCalled(
                invocation =>
                {
                    invocation.ReturnValue = mapper.Map<TDataItem>(entity);
                });

            repositoryAdapter.Stub(
                adapter =>
                adapter.Update(
                    Arg<TDataItem>.Is.Anything,
                    Arg<ItemSelection<TDataItem>>.Is.Anything,
                    Arg<Expression<Func<TDataItem, object>>[]>.Is.Anything)).Return(1);

            return repositoryAdapter;
        }

        /// <summary>
        /// Creates stubs to save and select an existing item.
        /// </summary>
        /// <param name="repositoryAdapter">
        /// The repository adapter to stub. 
        /// </param>
        /// <param name="entity">
        /// The existing entity. 
        /// </param>
        /// <param name="mapper">
        /// The entity mapper. 
        /// </param>
        /// <param name="selectionPredicate">
        /// The selection predicate.
        /// </param>
        /// <typeparam name="TDataItem">
        /// The type of data item. 
        /// </typeparam>
        /// <returns>
        /// The <see cref="Startitecture.Orm.Common.IRepositoryAdapter"/> with methods stubbed for an existing item.
        /// </returns>
        public static IRepositoryAdapter StubForExistingItem<TDataItem>(
            [NotNull] this IRepositoryAdapter repositoryAdapter,
            [NotNull] object entity,
            [NotNull] IEntityMapper mapper,
            Expression<Predicate<ItemSelection<TDataItem>>> selectionPredicate)
            where TDataItem : class, ITransactionContext
        {
            if (repositoryAdapter == null)
            {
                throw new ArgumentNullException(nameof(repositoryAdapter));
            }

            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            if (mapper == null)
            {
                throw new ArgumentNullException(nameof(mapper));
            }

            repositoryAdapter.Stub(adapter => adapter.Contains(Arg<ItemSelection<TDataItem>>.Matches(selectionPredicate))).Return(true);

            // Ensures that the data item is fresh every time. TODO: We may need this in other areas.
            repositoryAdapter.Stub(adapter => adapter.FirstOrDefault(Arg<ItemSelection<TDataItem>>.Matches(selectionPredicate))).Return(null).WhenCalled(
                invocation =>
                {
                    invocation.ReturnValue = mapper.Map<TDataItem>(entity);
                });

            repositoryAdapter.Stub(
                adapter =>
                adapter.Update(
                    Arg<TDataItem>.Is.Anything,
                    Arg<ItemSelection<TDataItem>>.Matches(selectionPredicate),
                    Arg<Expression<Func<TDataItem, object>>[]>.Is.Anything)).Return(1);

            return repositoryAdapter;
        }

        /// <summary>
        /// Creates stubs to save and select an existing item.
        /// </summary>
        /// <param name="repositoryAdapter">
        /// The repository adapter to stub. 
        /// </param>
        /// <param name="entity">
        /// The existing entity. 
        /// </param>
        /// <param name="mapper">
        /// The entity mapper. 
        /// </param>
        /// <param name="itemProperty">
        /// An item property included in the item selection filters, typically the primary key property.
        /// </param>
        /// <param name="value">
        /// The value to test for. This will typically be the primary key value.
        /// </param>
        /// <typeparam name="TDataItem">
        /// The type of data item. 
        /// </typeparam>
        /// <typeparam name="TValue">
        /// The type of value to test for.
        /// </typeparam>
        /// <returns>
        /// The <see cref="Startitecture.Orm.Common.IRepositoryAdapter"/> with methods stubbed for an existing item.
        /// </returns>
        public static IRepositoryAdapter StubForExistingItem<TDataItem, TValue>(
            [NotNull] this IRepositoryAdapter repositoryAdapter,
            [NotNull] object entity,
            [NotNull] IEntityMapper mapper,
            Expression<Func<TDataItem, TValue>> itemProperty,
            TValue value)
            where TDataItem : class, ITransactionContext
        {
            if (repositoryAdapter == null)
            {
                throw new ArgumentNullException(nameof(repositoryAdapter));
            }

            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            if (mapper == null)
            {
                throw new ArgumentNullException(nameof(mapper));
            }

            Expression<Predicate<ItemSelection<TDataItem>>> predicate = selection => TestForItem(selection, itemProperty, value);

            repositoryAdapter.Stub(adapter => adapter.Contains(Arg<ItemSelection<TDataItem>>.Matches(predicate))).Return(true);

            // Ensures that the data item is fresh every time. TODO: We may need this in other areas.
            repositoryAdapter.Stub(adapter => adapter.FirstOrDefault(Arg<ItemSelection<TDataItem>>.Matches(predicate))).Return(null).WhenCalled(
                invocation =>
                {
                    invocation.ReturnValue = mapper.Map<TDataItem>(entity);
                });

            repositoryAdapter.Stub(
                adapter =>
                adapter.Update(
                    Arg<TDataItem>.Is.Anything,
                    Arg<ItemSelection<TDataItem>>.Matches(predicate),
                    Arg<Expression<Func<TDataItem, object>>[]>.Is.Anything)).Return(1);

            return repositoryAdapter;
        }

        /// <summary>
        /// Creates stubs to save and select an existing item.
        /// </summary>
        /// <param name="repositoryAdapter">
        /// The repository adapter to stub. 
        /// </param>
        /// <typeparam name="TDataItem">
        /// The type of data item. 
        /// </typeparam>
        /// <returns>
        /// The <see cref="Startitecture.Orm.Common.IRepositoryAdapter"/> with methods stubbed for a new item.
        /// </returns>
        public static IRepositoryAdapter StubForNewItem<TDataItem>(
            [NotNull] this IRepositoryAdapter repositoryAdapter)
            where TDataItem : class, ITransactionContext
        {
            if (repositoryAdapter == null)
            {
                throw new ArgumentNullException(nameof(repositoryAdapter));
            }

            repositoryAdapter.Stub(adapter => adapter.Contains(Arg<ItemSelection<TDataItem>>.Is.Anything)).Return(false);
            repositoryAdapter.Stub(adapter => adapter.Insert(Arg<TDataItem>.Is.Anything)).Return(null)
                .WhenCalled(
                    invocation =>
                    {
                        // Finds the item in the argument list.
                        var item = Enumerable.OfType<TDataItem>(invocation.Arguments).First();

                        // If the item needs to have an auto-incremented ID set, we set it to a random number.
                        var primaryKeyAttribute = ((Object)item).GetType().GetCustomAttributes<PrimaryKeyAttribute>().FirstOrDefault();
                        var autoIncrement = (primaryKeyAttribute?.AutoIncrement).GetValueOrDefault();

                        if (primaryKeyAttribute != null && autoIncrement)
                        {
                            var propertyName = primaryKeyAttribute.ColumnName;
                            var property = typeof(TDataItem).GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance);
                            var propertySetter = property.SetMethod;
                            propertySetter.Invoke(item, new object[] { NumberGenerator.Next(1, Int32.MaxValue) });
                        }

                        invocation.ReturnValue = item;
                    });

            return repositoryAdapter;
        }

        /// <summary>
        /// Stubs a repository provider for a list of items.
        /// </summary>
        /// <param name="repositoryAdapter">
        /// The repository adapter.
        /// </param>
        /// <param name="target">
        /// The target of the items.
        /// </param>
        /// <typeparam name="TDataItem">
        /// The type of data item. 
        /// </typeparam>
        /// <returns>
        /// The <see cref="Startitecture.Orm.Common.IRepositoryAdapter"/> with methods stubbed for a list of items.
        /// </returns>
        public static IRepositoryAdapter StubForList<TDataItem>(
            [NotNull] this IRepositoryAdapter repositoryAdapter,
            [NotNull] List<TDataItem> target)
            where TDataItem : ITransactionContext
        {
            return StubForList(repositoryAdapter, target, null);
        }

        /// <summary>
        /// Stubs a repository provider for a list of items.
        /// </summary>
        /// <param name="repositoryAdapter">
        /// The repository adapter.
        /// </param>
        /// <param name="target">
        /// The target of the items.
        /// </param>
        /// <param name="primaryKey">
        /// The primary Key.
        /// </param>
        /// <typeparam name="TDataItem">
        /// The type of data item. 
        /// </typeparam>
        /// <returns>
        /// The <see cref="Startitecture.Orm.Common.IRepositoryAdapter"/> with methods stubbed for a list of items.
        /// </returns>
        public static IRepositoryAdapter StubForList<TDataItem>(
            [NotNull] this IRepositoryAdapter repositoryAdapter,
            [NotNull] List<TDataItem> target,
            Expression<Func<TDataItem, object>> primaryKey)
            where TDataItem : ITransactionContext
        {
            if (repositoryAdapter == null)
            {
                throw new ArgumentNullException(nameof(repositoryAdapter));
            }

            if (target == null)
            {
                throw new ArgumentNullException(nameof(target));
            }

            if (primaryKey != null)
            {
                UpdateListKeys(target, primaryKey.GetPropertyName());
            }
            else
            {
                UpdateListKeys(target);
            }

            repositoryAdapter.Stub(adapter => adapter.SelectItems(Arg<ItemSelection<TDataItem>>.Is.Anything)).Return(target);
            return repositoryAdapter;
        }

        /// <summary>
        /// Stubs a repository provider for a list of items.
        /// </summary>
        /// <param name="repositoryAdapter">
        /// The repository provider.
        /// </param>
        /// <param name="source">
        /// The source of the items.
        /// </param>
        /// <param name="entityMapper">
        /// The entity mapper to use to map the items.
        /// </param>
        /// <typeparam name="TEntity">
        /// The type of entity. 
        /// </typeparam>
        /// <typeparam name="TDataItem">
        /// The type of data item. 
        /// </typeparam>
        /// <returns>
        /// The <see cref="Startitecture.Orm.Common.IRepositoryAdapter"/> with methods stubbed for a list of items.
        /// </returns>
        public static IRepositoryAdapter StubForList<TEntity, TDataItem>(
            [NotNull] this IRepositoryAdapter repositoryAdapter,
            [NotNull] IEnumerable<TEntity> source,
            [NotNull] IEntityMapper entityMapper)
            where TDataItem : ITransactionContext
        {
            return StubForList(repositoryAdapter, source, new List<TDataItem>(), entityMapper);
        }

        /// <summary>
        /// Stubs a repository provider for a list of items.
        /// </summary>
        /// <param name="repositoryAdapter">
        /// The repository provider.
        /// </param>
        /// <param name="source">
        /// The source of the items.
        /// </param>
        /// <param name="target">
        /// The target of the items.
        /// </param>
        /// <param name="entityMapper">
        /// The entity mapper to use to map the items.
        /// </param>
        /// <typeparam name="TEntity">
        /// The type of entity. 
        /// </typeparam>
        /// <typeparam name="TDataItem">
        /// The type of data item. 
        /// </typeparam>
        /// <returns>
        /// The <see cref="Startitecture.Orm.Common.IRepositoryAdapter"/> with methods stubbed for a list of items.
        /// </returns>
        public static IRepositoryAdapter StubForList<TEntity, TDataItem>(
            [NotNull] this IRepositoryAdapter repositoryAdapter,
            [NotNull] IEnumerable<TEntity> source,
            [NotNull] List<TDataItem> target,
            [NotNull] IEntityMapper entityMapper)
            where TDataItem : ITransactionContext
        {
            return StubForList(repositoryAdapter, source, target, null, entityMapper);
        }

        /// <summary>
        /// Stubs a repository provider for a list of items.
        /// </summary>
        /// <param name="repositoryAdapter">
        /// The repository provider.
        /// </param>
        /// <param name="source">
        /// The source of the items.
        /// </param>
        /// <param name="target">
        /// The target of the items.
        /// </param>
        /// <param name="primaryKey">
        /// The primary Key.
        /// </param>
        /// <param name="entityMapper">
        /// The entity mapper to use to map the items.
        /// </param>
        /// <typeparam name="TEntity">
        /// The type of entity. 
        /// </typeparam>
        /// <typeparam name="TDataItem">
        /// The type of data item. 
        /// </typeparam>
        /// <returns>
        /// The <see cref="Startitecture.Orm.Common.IRepositoryAdapter"/> with methods stubbed for a list of items.
        /// </returns>
        public static IRepositoryAdapter StubForList<TEntity, TDataItem>(
            [NotNull] this IRepositoryAdapter repositoryAdapter,
            [NotNull] IEnumerable<TEntity> source,
            [NotNull] List<TDataItem> target,
            Expression<Func<TDataItem, object>> primaryKey,
            [NotNull] IEntityMapper entityMapper)
            where TDataItem : ITransactionContext
        {
            if (repositoryAdapter == null)
            {
                throw new ArgumentNullException(nameof(repositoryAdapter));
            }

            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (target == null)
            {
                throw new ArgumentNullException(nameof(target));
            }

            if (entityMapper == null)
            {
                throw new ArgumentNullException(nameof(entityMapper));
            }

            target.Clear();
            var items = entityMapper.Map<List<TDataItem>>(source);
            target.AddRange(items);

            if (primaryKey != null)
            {
                var propertyName = primaryKey.GetPropertyName();
                UpdateListKeys(target, propertyName);
            }
            else
            {
                UpdateListKeys(target);
            }

            repositoryAdapter.Stub(adapter => adapter.SelectItems(Arg<ItemSelection<TDataItem>>.Is.Anything)).Return(target);
            return repositoryAdapter;
        }

        #endregion

        #region Structured Command Providers

        /// <summary>
        /// Stubs an <see cref="Startitecture.Orm.Sql.IStructuredCommandProvider"/> for the specified set of values. 
        /// The IDataReader mock supports FieldCount, GetOrdinal() and GetValues(). GetValues() must be called once per 
        /// IDataReader.Read() to align with expected output.
        /// </summary>
        /// <param name="provider">
        /// The provider.
        /// </param>
        /// <param name="values">
        /// The values to stub.
        /// </param>
        /// <typeparam name="TDataItem">
        /// The type of data item to return via the command.
        /// </typeparam>
        /// <returns>
        /// The <see cref="Startitecture.Orm.Sql.IStructuredCommandProvider"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="provider"/> or <paramref name="values"/> is null.
        /// </exception>
        public static IStructuredCommandProvider StubForList<TDataItem>(
            [NotNull] this IStructuredCommandProvider provider,
            [NotNull] List<TDataItem> values)
        {
            if (provider == null)
            {
                throw new ArgumentNullException(nameof(provider));
            }

            if (values == null)
            {
                throw new ArgumentNullException(nameof(values));
            }

            // Get the structure type from the data type. If none default to the name of the type. Note that concrete commands will 
            // require the type but we'll let that fail there instead of here.
            var structureType = typeof(TDataItem);
            var structureTypeName = structureType.GetCustomAttributes<TableTypeAttribute>().FirstOrDefault()?.TypeName ?? structureType.Name;

            var dataReader = MockRepository.GenerateMock<IDataReader>();

            // We need to mock out the behavior of the data reader.
            dataReader.Expect(reader => reader.Read()).Return(true).Repeat.Times((values as IList<TDataItem>).Count);
            dataReader.Expect(reader => reader.Read()).Return(false).Repeat.Once();

            var properties = from p in typeof(TDataItem).GetProperties(BindingFlags.Instance | BindingFlags.Public)
                             where p.CanWrite //// Skip stuff like TransactionProvider
                             orderby p.Name
                             select p;

            var primaryKeyAttribute = typeof(TDataItem).GetCustomAttributes<PrimaryKeyAttribute>().FirstOrDefault();

            // Set up ordinals in alphabetical order. This mirrors the current convention for UDTTs and structured commands.
            var propertyOrdinals = properties.Select((info, i) => new PropertyOrdinal { Ordinal = i, Property = info }).ToList();

            dataReader.Stub(reader => reader.FieldCount).Return(propertyOrdinals.Count);

            foreach (var propertyOrdinal in propertyOrdinals)
            {
                dataReader.Stub(reader => reader.GetOrdinal(propertyOrdinal.Property.Name)).Return(propertyOrdinal.Ordinal);
            }

            foreach (var value in values)
            {
                dataReader.Expect(reader => reader.GetValues(Arg<object[]>.Is.Anything)).Return(propertyOrdinals.Count).WhenCalled(
                    invocation =>
                        {
                            var itemArray = Enumerable.OfType<object[]>(invocation.Arguments).First();
                            SetListValues(propertyOrdinals, value, primaryKeyAttribute, itemArray);
                        }).Repeat.Once();
            }

            var dataCommand = MockRepository.GenerateMock<IDbCommand>();

            // Only stub when the structured type names match.
            provider.Stub(
                command =>
                command.CreateCommand(
                    Arg<IStructuredCommand>.Matches(structuredCommand => structuredCommand.StructureTypeName == structureTypeName),
                    Arg<DataTable>.Is.Anything,
                    Arg<IDbTransaction>.Is.Anything)).Return(dataCommand);

            dataCommand.Stub(command => command.ExecuteReader()).Return(dataReader);

            return provider;
        }

        #endregion

        #region Mapping

        /// <summary>
        /// Clones an entity object.
        /// </summary>
        /// <param name="entityMapper">
        /// The entity mapper.
        /// </param>
        /// <param name="entity">
        /// The entity to clone.
        /// </param>
        /// <param name="constructor">
        /// A constructor for the entity.
        /// </param>
        /// <typeparam name="TEntity">
        /// The type of entity to clone.
        /// </typeparam>
        /// <returns>
        /// A clone of the <paramref name="entity"/>.
        /// </returns>
        public static TEntity Clone<TEntity>([NotNull] this IEntityMapper entityMapper, TEntity entity, [NotNull] Func<TEntity> constructor)
        {
            if (entityMapper == null)
            {
                throw new ArgumentNullException(nameof(entityMapper));
            }

            if (constructor == null)
            {
                throw new ArgumentNullException(nameof(constructor));
            }

            var clone = constructor();
            return entityMapper.MapTo(entity, clone);
        }

        #endregion

        /// <summary>
        /// Sets the property value for the specified property.
        /// </summary>
        /// <param name="target">
        /// The target.
        /// </param>
        /// <param name="itemProperty">
        /// The item property.
        /// </param>
        /// <param name="value">
        /// The value to set.
        /// </param>
        /// <typeparam name="T">
        /// The type of the item to set a property on.
        /// </typeparam>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "Allows fluent use of the method.")]
        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters", Justification = "Allows fluent use of the method.")]
        public static void SetPropertyValue<T>(this T target, Expression<Func<T, object>> itemProperty, object value)
        {
            if (target == null)
            {
                throw new ArgumentNullException(nameof(target));
            }

            if (itemProperty == null)
            {
                throw new ArgumentNullException(nameof(itemProperty));
            }

            MemberExpression memberSelectorExpression;

            if (itemProperty.Body.NodeType == ExpressionType.Convert)
            {
                if (!(itemProperty.Body is UnaryExpression unaryExpression))
                {
                    throw new BusinessException(itemProperty, ValidationMessages.SelectorCannotBeEvaluated);
                }

                memberSelectorExpression = unaryExpression.Operand as MemberExpression;
            }
            else
            {
                memberSelectorExpression = itemProperty.Body as MemberExpression;
            }

            if (memberSelectorExpression == null)
            {
                throw new BusinessException(itemProperty, ValidationMessages.SelectorCannotBeEvaluated);
            }

            var property = memberSelectorExpression.Member as PropertyInfo;

            if (property == null)
            {
                throw new BusinessException(itemProperty, ValidationMessages.SelectorCannotBeEvaluated);
            }

            property.SetValue(target, value, null);
        }

        /// <summary>
        /// Sets the array values for the specified list.
        /// </summary>
        /// <param name="propertyOrdinals">
        /// The property ordinals.
        /// </param>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <param name="primaryKeyAttribute">
        /// The primary key attribute.
        /// </param>
        /// <param name="itemArray">
        /// The item array.
        /// </param>
        /// <typeparam name="TDataItem">
        /// The type of data item that is the source of the values.
        /// </typeparam>
        private static void SetListValues<TDataItem>(
            IEnumerable<PropertyOrdinal> propertyOrdinals,
            TDataItem value,
            PrimaryKeyAttribute primaryKeyAttribute,
            IList<object> itemArray)
        {
            // Set the values in the array.
            foreach (var propertyOrdinal in propertyOrdinals)
            {
                var propertyValue = propertyOrdinal.Property.GetMethod.Invoke(value, null);

                if (primaryKeyAttribute?.AutoIncrement == true && propertyOrdinal.Property.Name == primaryKeyAttribute.ColumnName
                    && (propertyValue == null || propertyValue.Equals(0)))
                {
                    propertyValue = NumberGenerator.Next(1, int.MaxValue);
                }

                itemArray[propertyOrdinal.Ordinal] = propertyValue;
            }
        }

        /// <summary>
        /// Stubs a list into the repository adapter.
        /// </summary>
        /// <param name="items">
        /// The items to stub
        /// </param>
        /// <typeparam name="TDataItem">
        /// The type of data item. 
        /// </typeparam>
        private static void UpdateListKeys<TDataItem>(IEnumerable<TDataItem> items)
            where TDataItem : ITransactionContext
        {
            var primaryKeyAttribute = typeof(TDataItem).GetCustomAttributes<PrimaryKeyAttribute>().FirstOrDefault();
            var autoIncrement = (primaryKeyAttribute?.AutoIncrement).GetValueOrDefault();

            // Set autonumber keys for the returned items.
            if (primaryKeyAttribute != null && autoIncrement)
            {
                var propertyName = primaryKeyAttribute.ColumnName;
                UpdateListKeys(items, propertyName);
            }
        }

        /// <summary>
        /// The update list keys.
        /// </summary>
        /// <param name="items">
        /// The items.
        /// </param>
        /// <param name="propertyName">
        /// The property name.
        /// </param>
        /// <typeparam name="TDataItem">
        /// The type of data item. 
        /// </typeparam>
        private static void UpdateListKeys<TDataItem>(IEnumerable<TDataItem> items, string propertyName) where TDataItem : ITransactionContext
        {
            var property = typeof(TDataItem).GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance);

            var propertyGetter = property.GetMethod;
            var propertySetter = property.SetMethod;

            foreach (var item in items)
            {
                var existingValue = propertyGetter.Invoke(item, null);

                if (Convert.ToInt64(existingValue) == 0)
                {
                    propertySetter.Invoke(item, new object[] { NumberGenerator.Next(1, int.MaxValue) });
                }
            }
        }

        /// <summary>
        /// Tests for an item value in the selection.
        /// </summary>
        /// <param name="selection">
        /// The selection to test.
        /// </param>
        /// <param name="itemProperty">
        /// The item property.
        /// </param>
        /// <param name="value">
        /// The expected value.
        /// </param>
        /// <typeparam name="TDataItem">
        /// The type of data item.
        /// </typeparam>
        /// <typeparam name="TValue">
        /// The type of the value.
        /// </typeparam>
        /// <returns>
        /// <c>true</c> if the item property has a matching value in the selection; otherwise, <c>false</c>.
        /// </returns>
        private static bool TestForItem<TDataItem, TValue>(
            ItemSelection<TDataItem> selection,
            Expression<Func<TDataItem, TValue>> itemProperty,
            TValue value)
        {
            var valueFilter = selection.Filters.FirstOrDefault(x => x.ItemAttribute.PropertyName == itemProperty.GetPropertyName());

            return valueFilter != null && EqualityComparer<TValue>.Default.Equals(Enumerable.OfType<TValue>(valueFilter.FilterValues).FirstOrDefault(), value);
        }
    }
}