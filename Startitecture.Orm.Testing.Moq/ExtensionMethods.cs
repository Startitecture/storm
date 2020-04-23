// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExtensionMethods.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.Testing.Moq
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Data;
    using System.Diagnostics;
    using System.Globalization;
    using System.Linq;
    using System.Reflection;

    using Castle.DynamicProxy.Internal;

    using JetBrains.Annotations;

    using global::Moq;

    using Startitecture.Core;
    using Startitecture.Orm.Model;
    using Startitecture.Orm.Sql;

    /// <summary>
    /// The extension methods.
    /// </summary>
    public static class ExtensionMethods
    {
        /// <summary>
        /// The random number.
        /// </summary>
        private static readonly Random RandomNumber = new Random();

        /// <summary>
        /// The mock command provider.
        /// </summary>
        /// <param name="items">
        /// The merge items.
        /// </param>
        /// <param name="definitionProvider">
        /// The definition provider.
        /// </param>
        /// <param name="nameQualifier">
        /// The name qualifier.
        /// </param>
        /// <typeparam name="T">
        /// The type of item in the list to create an <see cref="IStructuredCommandProvider"/> mock for.
        /// </typeparam>
        /// <returns>
        /// The <see cref="Mock"/>.
        /// </returns>
        public static Mock<IStructuredCommandProvider> MockCommandProvider<T>(
            [NotNull] this IReadOnlyCollection<T> items,
            [NotNull] IEntityDefinitionProvider definitionProvider,
            [NotNull] INameQualifier nameQualifier)
        {
            if (items == null)
            {
                throw new ArgumentNullException(nameof(items));
            }

            if (definitionProvider == null)
            {
                throw new ArgumentNullException(nameof(definitionProvider));
            }

            if (nameQualifier == null)
            {
                throw new ArgumentNullException(nameof(nameQualifier));
            }

            var structureDefinition = definitionProvider.Resolve<T>();
            var orderedAttributes = structureDefinition.ReturnableAttributes.OrderBy(definition => definition.Ordinal).ToList();

            var reader = new Mock<IDataReader>();
            var fieldCount = orderedAttributes.Count();
            reader.Setup(dataReader => dataReader.FieldCount).Returns(fieldCount);
            reader.Setup(dataReader => dataReader.GetName(It.IsAny<int>())).Returns((int i) => orderedAttributes.ElementAt(i).PropertyName);
            reader.Setup(dataReader => dataReader.GetFieldType(It.IsAny<int>()))
                .Returns((int i) => orderedAttributes.ElementAt(i).PropertyInfo.PropertyType);

            int rowNumber = -1;

            var readActionReturn = reader.SetupSequence(dataReader => dataReader.Read());

            for (int i = 0; i < items.Count; i++)
            {
                readActionReturn.Returns(
                    () =>
                        {
                            rowNumber++;
                            return true;
                        });
            }

            readActionReturn.Returns(false);

            reader.Setup(dataReader => dataReader.IsDBNull(It.IsAny<int>()))
                .Returns(
                    (int i) =>
                        {
                            var item = items.ElementAt(rowNumber);
                            var attributeDefinition = orderedAttributes.ElementAt(i);
                            var value = attributeDefinition.GetValueDelegate.DynamicInvoke(item);

                            // Will require this to be annotated on the structure
                            if (attributeDefinition.IsIdentityColumn == false || value != null)
                            {
                                return value == null;
                            }

                            var propertyType = attributeDefinition.PropertyInfo.PropertyType;
                            var type = propertyType.IsNullableType() ? propertyType.GetGenericArguments().First() : propertyType;

                            if (type == typeof(long))
                            {
                                var next = RandomNumber.Next(int.MaxValue);
                                attributeDefinition.SetValueDelegate.DynamicInvoke(item, next);
                                Trace.WriteLine($"Set identity value for {item?.GetType().Name} {item}: {attributeDefinition.PropertyName}={next}");
                            }
                            else if (type == typeof(int))
                            {
                                var next = RandomNumber.Next(int.MaxValue);
                                attributeDefinition.SetValueDelegate.DynamicInvoke(item, next);
                                Trace.WriteLine($"Set identity value for {item?.GetType().Name} {item}: {attributeDefinition.PropertyName}={next}");
                            }
                            else if (type == typeof(short))
                            {
                                var next = RandomNumber.Next(short.MaxValue);
                                attributeDefinition.SetValueDelegate.DynamicInvoke(item, next);
                                Trace.WriteLine($"Set identity value for {item?.GetType().Name} {item}: {attributeDefinition.PropertyName}={next}");
                            }
                            else if (type == typeof(byte))
                            {
                                var next = RandomNumber.Next(byte.MaxValue);
                                attributeDefinition.SetValueDelegate.DynamicInvoke(item, next);
                                Trace.WriteLine($"Set identity value for {item?.GetType().Name} {item}: {attributeDefinition.PropertyName}={next}");
                            }
                            else
                            {
                                throw new OperationException($"Type '{type}' is not supported as an identity column.");
                            }

                            // Not null anymore
                            return false;
                        });

            reader.Setup(dataReader => dataReader.GetValue(It.IsAny<int>()))
                .Returns((int i) => orderedAttributes.ElementAt(i).GetValueDelegate.DynamicInvoke(items.ElementAt(rowNumber)));

            reader.Setup(dataReader => dataReader.GetString(It.IsAny<int>()))
                .Returns((int i) => (string)orderedAttributes.ElementAt(i).GetValueDelegate.DynamicInvoke(items.ElementAt(rowNumber)));

            reader.Setup(dataReader => dataReader.GetInt64(It.IsAny<int>()))
                .Returns((int i) => (long)orderedAttributes.ElementAt(i).GetValueDelegate.DynamicInvoke(items.ElementAt(rowNumber)));

            reader.Setup(dataReader => dataReader.GetInt32(It.IsAny<int>()))
                .Returns((int i) => (int)orderedAttributes.ElementAt(i).GetValueDelegate.DynamicInvoke(items.ElementAt(rowNumber)));

            reader.Setup(dataReader => dataReader.GetInt16(It.IsAny<int>()))
                .Returns((int i) => (short)orderedAttributes.ElementAt(i).GetValueDelegate.DynamicInvoke(items.ElementAt(rowNumber)));

            reader.Setup(dataReader => dataReader.GetByte(It.IsAny<int>()))
                .Returns((int i) => (byte)orderedAttributes.ElementAt(i).GetValueDelegate.DynamicInvoke(items.ElementAt(rowNumber)));

            reader.Setup(dataReader => dataReader.GetBoolean(It.IsAny<int>()))
                .Returns((int i) => (bool)orderedAttributes.ElementAt(i).GetValueDelegate.DynamicInvoke(items.ElementAt(rowNumber)));

            reader.Setup(dataReader => dataReader.GetChar(It.IsAny<int>()))
                .Returns((int i) => (char)orderedAttributes.ElementAt(i).GetValueDelegate.DynamicInvoke(items.ElementAt(rowNumber)));

            reader.Setup(dataReader => dataReader.GetDateTime(It.IsAny<int>()))
                .Returns((int i) => (DateTime)orderedAttributes.ElementAt(i).GetValueDelegate.DynamicInvoke(items.ElementAt(rowNumber)));

            reader.Setup(dataReader => dataReader.GetDecimal(It.IsAny<int>()))
                .Returns((int i) => (decimal)orderedAttributes.ElementAt(i).GetValueDelegate.DynamicInvoke(items.ElementAt(rowNumber)));

            reader.Setup(dataReader => dataReader.GetDouble(It.IsAny<int>()))
                .Returns((int i) => (double)orderedAttributes.ElementAt(i).GetValueDelegate.DynamicInvoke(items.ElementAt(rowNumber)));

            reader.Setup(dataReader => dataReader.GetFloat(It.IsAny<int>()))
                .Returns((int i) => (float)orderedAttributes.ElementAt(i).GetValueDelegate.DynamicInvoke(items.ElementAt(rowNumber)));

            reader.Setup(dataReader => dataReader.GetGuid(It.IsAny<int>()))
                .Returns((int i) => (Guid)orderedAttributes.ElementAt(i).GetValueDelegate.DynamicInvoke(items.ElementAt(rowNumber)));

            var command = new Mock<IDbCommand>();
            command.Setup(dbCommand => dbCommand.ExecuteReader()).Returns(reader.Object);

            var commandProvider = new Mock<IStructuredCommandProvider>();
            commandProvider.Setup(provider => provider.NameQualifier).Returns(nameQualifier);
            commandProvider.Setup(provider => provider.EntityDefinitionProvider).Returns(definitionProvider);
            commandProvider
                .Setup(provider => provider.CreateCommand(It.IsAny<IStructuredCommand>(), It.IsAny<DataTable>(), It.IsAny<IDbTransaction>()))
                .Returns(command.Object);
            return commandProvider;
        }

        /// <summary>
        /// Mocks a data reader for the specified <paramref name="item"/>.
        /// </summary>
        /// <param name="item">
        /// The item to create the reader for.
        /// </param>
        /// <param name="attributeDefinitions">
        /// The attribute definitions to use when creating a reader for the item.
        /// </param>
        /// <param name="entityDefinition">
        /// The entity definition for the item.
        /// </param>
        /// <typeparam name="T">
        /// The type of item to create a reader for.
        /// </typeparam>
        /// <returns>
        /// The <see cref="Mock{IDataReader}"/> for the specified object.
        /// </returns>
        public static Mock<IDataReader> MockDataReader<T>(this T item, IDictionary<string, EntityAttributeDefinition> attributeDefinitions, IEntityDefinition entityDefinition)
        {
            var dataReader = new Mock<IDataReader>();
            dataReader.Setup(reader => reader.FieldCount).Returns(attributeDefinitions.Count);
            int ordinal = 0;

            foreach (var attribute in attributeDefinitions)
            {
                object baseObject = item;
                var currentNode = attribute.Value.EntityNode.List.First;
                var targetNode = attribute.Value.EntityNode;

                while (currentNode != targetNode && currentNode?.Next != null && currentNode.Next.Value.IsVirtual == false)
                {
                    var entityLocation = currentNode.Next.Value;

                    // If the base object can't be found, the row has been flattened, so return the original item.
                    baseObject = GetObjectProperty(baseObject, entityLocation) ?? item;
                    currentNode = currentNode.Next;
                }

                object value;

                if (baseObject.GetType() != attribute.Value.GetValueMethod.ReflectedType)
                {
                    // The source object is actually null.
                    value = null;
                }
                else
                {
                    try
                    {
                        value = attribute.Value.GetValueDelegate.DynamicInvoke(baseObject);
                    }
                    catch (TargetException ex)
                    {
                        Trace.TraceError($"{entityDefinition.EntityContainer}.{baseObject.GetType().Name} '{baseObject}' for {attribute.Value}:{ex.Message}");

                        throw;
                    }
                }

                // NOTE: All this stubbin' makes the tests slower. Baseline after each change.
                // Need a local variable inside the closure for this to work.
                var localOrdinal = ordinal;
                dataReader.Setup(reader => reader.GetName(It.Is<int>(i => i == localOrdinal))).Returns(attribute.Key);
                dataReader.Setup(reader => reader.GetOrdinal(It.Is<string>(s => s == attribute.Key))).Returns(localOrdinal);

                var propertyType = attribute.Value.PropertyInfo.PropertyType;
                dataReader.Setup(reader => reader.GetFieldType(It.Is<int>(i => i == localOrdinal))).Returns(propertyType);
                dataReader.Setup(reader => reader.IsDBNull(It.Is<int>(i => i == localOrdinal))).Returns(value == null);

                if (propertyType.IsGenericType && propertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
                {
                    var typeOfNullable = Nullable.GetUnderlyingType(propertyType);

                    if (typeOfNullable == typeof(DateTimeOffset))
                    {
                        dataReader.Setup(reader => reader.GetValue(It.Is<int>(i => i == localOrdinal))).Returns(value);
                    }
                    else if (typeOfNullable == typeof(int))
                    {
                        dataReader.Setup(reader => reader.GetValue(It.Is<int>(i => i == localOrdinal))).Returns(value);
                        dataReader.Setup(reader => reader.GetInt32(It.Is<int>(i => i == localOrdinal))).Returns((int?)value ?? default);
                    }
                }
                else if (propertyType == typeof(int))
                {
                    dataReader.Setup(reader => reader.GetValue(It.Is<int>(i => i == localOrdinal))).Returns(value);
                    dataReader.Setup(reader => reader.GetInt32(It.Is<int>(i => i == localOrdinal))).Returns((int?)value ?? default);
                }
                else if (propertyType == typeof(short))
                {
                    dataReader.Setup(reader => reader.GetValue(It.Is<int>(i => i == localOrdinal))).Returns(value);
                    dataReader.Setup(reader => reader.GetInt16(It.Is<int>(i => i == localOrdinal))).Returns((short?)value ?? default);
                }
                else if (propertyType == typeof(bool))
                {
                    dataReader.Setup(reader => reader.GetValue(It.Is<int>(i => i == localOrdinal))).Returns(value);
                    dataReader.Setup(reader => reader.GetBoolean(It.Is<int>(i => i == localOrdinal))).Returns((bool?)value ?? default);
                }
                else if (propertyType == typeof(string))
                {
                    dataReader.Setup(reader => reader.GetValue(It.Is<int>(i => i == localOrdinal))).Returns(value);
                    dataReader.Setup(reader => reader.GetString(It.Is<int>(i => i == localOrdinal)))
                        .Returns(Convert.ToString(value, CultureInfo.CurrentCulture));
                }
                else if (propertyType == typeof(DateTimeOffset))
                {
                    dataReader.Setup(reader => reader.GetValue(It.Is<int>(i => i == localOrdinal))).Returns((DateTimeOffset?)value ?? default);
                }
                else if (propertyType == typeof(decimal))
                {
                    dataReader.Setup(reader => reader.GetValue(It.Is<int>(i => i == localOrdinal))).Returns(value);
                    dataReader.Setup(reader => reader.GetDecimal(It.Is<int>(i => i == localOrdinal))).Returns((decimal?)value ?? default);
                }

                ordinal++;
            }

            return dataReader;
        }

        /// <summary>
        /// Gets an object property referenced by the entity location.
        /// </summary>
        /// <param name="baseObject">
        /// The base object.
        /// </param>
        /// <param name="entityLocation">
        /// The entity location.
        /// </param>
        /// <returns>
        /// The related entity as an <see cref="object"/>.
        /// </returns>
        private static object GetObjectProperty(object baseObject, EntityLocation entityLocation)
        {
            return baseObject.GetType().GetProperty(entityLocation.Alias ?? entityLocation.Name)?.GetMethod.Invoke(baseObject, null);
        }
    }
}