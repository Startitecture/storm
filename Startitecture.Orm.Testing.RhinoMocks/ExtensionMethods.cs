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
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Runtime.CompilerServices;

    using Common;

    using JetBrains.Annotations;

    using Rhino.Mocks;

    using Startitecture.Core;
    using Startitecture.Orm.Model;
    using Startitecture.Orm.Query;
    using Startitecture.Orm.Schema;
    using Startitecture.Orm.Sql;

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
        /// <param name="entityMapper">
        /// The entity mapper.
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
            [NotNull] IEntityMapper entityMapper,
            [NotNull] object entity)
            where TDataItem : class, ITransactionContext
        {
            if (repositoryProvider == null)
            {
                throw new ArgumentNullException(nameof(repositoryProvider));
            }

            if (entityMapper == null)
            {
                throw new ArgumentNullException(nameof(entityMapper));
            }

            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            var dataItem = entityMapper.Map<TDataItem>(entity);

            repositoryProvider.Stub(provider => provider.Contains(Arg<ItemSelection<TDataItem>>.Is.Anything)).Return(true);
            repositoryProvider.Stub(provider => provider.GetFirstOrDefault(Arg<ItemSelection<TDataItem>>.Is.Anything)).Return(dataItem);
            repositoryProvider.Stub(provider => provider.Save(Arg<TDataItem>.Is.Anything)).Return(dataItem);
            return repositoryProvider;
        }

/*
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
                UpdateListKeys(target, repositoryProvider.EntityDefinitionProvider);
            }

            repositoryProvider.Stub(provider => provider.GetSelection(Arg<ItemSelection<TDataItem>>.Is.Anything)).Return(target);
            return repositoryProvider;
        }
*/

/*
        /// <summary>
        /// Stubs a repository provider for a list of items.
        /// </summary>
        /// <param name="repositoryProvider">
        /// The repository provider.
        /// </param>
        /// <param name="entityMapper">
        /// The entity mapper.
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
            [NotNull] IEntityMapper entityMapper,
            [NotNull] IEnumerable<TEntity> source,
            [NotNull] List<TDataItem> target,
            Expression<Func<TDataItem, object>> primaryKey)
            where TDataItem : ITransactionContext
        {
            if (repositoryProvider == null)
            {
                throw new ArgumentNullException(nameof(repositoryProvider));
            }

            if (entityMapper == null)
            {
                throw new ArgumentNullException(nameof(entityMapper));
            }

            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (target == null)
            {
                throw new ArgumentNullException(nameof(target));
            }

            target.Clear();
            var items = entityMapper.Map<List<TDataItem>>(source);
            target.AddRange(items);

            if (primaryKey != null)
            {
                UpdateListKeys(target, primaryKey.GetPropertyName());
            }
            else
            {
                UpdateListKeys(target, repositoryProvider.EntityDefinitionProvider);
            }

            repositoryProvider.Stub(provider => provider.GetSelection(Arg<ItemSelection<TDataItem>>.Is.Anything)).Return(target);
            return repositoryProvider;
        }
*/

        #endregion

        #region Repository Adapters

/*
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
        /// <param name="definitionProvider">
        /// The definition provider.
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
            Expression<Func<TDataItem, object>> primaryKey,
            IEntityDefinitionProvider definitionProvider)
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
                UpdateListKeys(target, definitionProvider);
            }

            repositoryAdapter.Stub(adapter => adapter.SelectItems(Arg<ItemSelection<TDataItem>>.Is.Anything)).Return(target);
            return repositoryAdapter;
        }
*/

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
        /// <param name="definitionProvider">
        /// The definition provider.
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
            [NotNull] IEntityMapper entityMapper,
            IEntityDefinitionProvider definitionProvider)
            where TDataItem : ITransactionContext
        {
            return StubForList(repositoryAdapter, source, target, null, entityMapper, definitionProvider);
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
        /// <param name="definitionProvider">
        /// The definition provider.
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
            [NotNull] IEntityMapper entityMapper,
            [NotNull] IEntityDefinitionProvider definitionProvider)
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

            if (definitionProvider == null)
            {
                throw new ArgumentNullException(nameof(definitionProvider));
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
                UpdateListKeys(target, definitionProvider);
            }

            repositoryAdapter.Stub(adapter => adapter.SelectItems(Arg<ItemSelection<TDataItem>>.Is.Anything)).Return(target);
            return repositoryAdapter;
        }

        #endregion

        #region Structured Command Providers

        /// <summary>
        /// Stubs an <see cref="IStructuredCommandProvider"/> for the specified set of values. 
        /// The IDataReader mock supports FieldCount, GetOrdinal() and GetValues(). GetValues() must be called once per 
        /// IDataReader.Read() to align with expected output.
        /// </summary>
        /// <param name="commandProvider">
        /// The provider.
        /// </param>
        /// <param name="definitionProvider">
        /// The definition Provider.
        /// </param>
        /// <param name="values">
        /// The values to stub.
        /// </param>
        /// <typeparam name="TDataItem">
        /// The type of data item to return via the command.
        /// </typeparam>
        /// <returns>
        /// The <see cref="IStructuredCommandProvider"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="commandProvider"/> or <paramref name="values"/> is null.
        /// </exception>
        public static IStructuredCommandProvider StubForList<TDataItem>(
            [NotNull] this IStructuredCommandProvider commandProvider,
            [NotNull] IEntityDefinitionProvider definitionProvider,
            [NotNull] List<TDataItem> values)
        {
            if (commandProvider == null)
            {
                throw new ArgumentNullException(nameof(commandProvider));
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

            var primaryKeyAttribute = definitionProvider.Resolve<TDataItem>().PrimaryKeyAttributes.FirstOrDefault(); //// typeof(TDataItem).GetCustomAttributes<PrimaryKeyAttribute>().FirstOrDefault();

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
                            var itemArray = invocation.Arguments.OfType<object[]>().First();
                            SetListValues(propertyOrdinals, value, primaryKeyAttribute, itemArray);
                        }).Repeat.Once();
            }

            var dataCommand = MockRepository.GenerateMock<IDbCommand>();

            // Only stub when the structured type names match.
            commandProvider.Stub(
                command =>
                command.CreateCommand(
                    Arg<IStructuredCommand>.Matches(structuredCommand => structuredCommand.StructureTypeName == structureTypeName),
                    Arg<DataTable>.Is.Anything,
                    Arg<IDbTransaction>.Is.Anything)).Return(dataCommand);

            dataCommand.Stub(command => command.ExecuteReader()).Return(dataReader);

            return commandProvider;
        }

        #endregion

        #region Mapping

        #endregion

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
            EntityAttributeDefinition primaryKeyAttribute,
            IList<object> itemArray)
        {
            // Set the values in the array.
            foreach (var propertyOrdinal in propertyOrdinals)
            {
                var propertyValue = propertyOrdinal.Property.GetMethod.Invoke(value, null);

                if (primaryKeyAttribute.IsIdentityColumn && propertyOrdinal.Property.Name == primaryKeyAttribute.PropertyName
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
        /// <param name="definitionProvider">
        /// The definition provider.
        /// </param>
        /// <typeparam name="TDataItem">
        /// The type of data item. 
        /// </typeparam>
        private static void UpdateListKeys<TDataItem>(IEnumerable<TDataItem> items, IEntityDefinitionProvider definitionProvider)
            where TDataItem : ITransactionContext
        {
            var autoIncrement = definitionProvider.Resolve<TDataItem>().AutoNumberPrimaryKey;

            if (!autoIncrement.HasValue)
            {
                return;
            }

            // Set auto number keys for the returned items.
            var propertyName = autoIncrement.Value.PhysicalName;
            UpdateListKeys(items, propertyName);
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
            var valueFilter = selection.Filters.FirstOrDefault(x => x.AttributeLocation == new AttributeLocation(itemProperty));

            return valueFilter != null && EqualityComparer<TValue>.Default.Equals(valueFilter.FilterValues.OfType<TValue>().FirstOrDefault(), value);
        }
    }
}