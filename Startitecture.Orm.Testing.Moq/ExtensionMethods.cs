// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExtensionMethods.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.Testing.Moq
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Diagnostics;
    using System.Linq;

    using Castle.DynamicProxy.Internal;

    using JetBrains.Annotations;

    using global::Moq;

    using Startitecture.Core;
    using Startitecture.Orm.Common;
    using Startitecture.Orm.Model;

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

            var reader = items.MockDataReaderForList(orderedAttributes);

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
        /// <param name="attributes">
        /// The attribute definitions to use when creating a reader for the item.
        /// </param>
        /// <typeparam name="T">
        /// The type of item to create a reader for.
        /// </typeparam>
        /// <returns>
        /// The <see cref="Mock{IDataReader}"/> for the specified object.
        /// </returns>
        public static Mock<IDataReader> MockDataReader<T>(
            [NotNull] this T item,
            [NotNull] IEnumerable<EntityAttributeDefinition> attributes)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            if (attributes == null)
            {
                throw new ArgumentNullException(nameof(attributes));
            }

            return MockDataReaderForList(new List<T> { item }, attributes.ToList());
        }

        /// <summary>
        /// Mocks a data reader for a list of items. The items that are identity columns with null values are automatically populated with a random value.
        /// </summary>
        /// <param name="items">
        /// The items to mock in the data reader.
        /// </param>
        /// <param name="attributes">
        /// The attributes to mock.
        /// </param>
        /// <typeparam name="T">
        /// The type of item in the list to mock the data reader for.
        /// </typeparam>
        /// <returns>
        /// A <see cref="Mock{IDataReader}"/> for the supplied <paramref name="items"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="items"/> or <paramref name="attributes"/> is null.
        /// </exception>
        /// <exception cref="OperationException">
        /// The type of an attribute is not supported as an identity column.
        /// </exception>
        public static Mock<IDataReader> MockDataReaderForList<T>(
            [NotNull] this IReadOnlyCollection<T> items,
            [NotNull] IReadOnlyCollection<EntityAttributeDefinition> attributes)
        {
            if (items == null)
            {
                throw new ArgumentNullException(nameof(items));
            }

            if (attributes == null)
            {
                throw new ArgumentNullException(nameof(attributes));
            }

            var attributeIndexDictionary = attributes.Distinct(Singleton<ReferenceNameComparer>.Instance)
                .Select(
                    (definition, i) => new
                                           {
                                               Index = i,
                                               Attribute = definition
                                           })
                .ToDictionary(arg => arg.Attribute.ReferenceName, arg => arg.Index);

            var reader = new Mock<IDataReader>();
            var fieldCount = attributes.Count;
            reader.Setup(dataReader => dataReader.FieldCount).Returns(fieldCount);
            reader.Setup(dataReader => dataReader.GetOrdinal(It.IsAny<string>()))
                .Returns((string s) =>
                    {
                        var attributeIndex = attributeIndexDictionary[s];
                        Trace.WriteLine($"Requested reference name '{s}' located attribute with ordinal {attributeIndex}");
                        return attributeIndex;
                    });

            reader.Setup(dataReader => dataReader.GetName(It.IsAny<int>())).Returns((int i) =>
                {
                    var referenceName = attributes.ElementAt(i).ReferenceName;
                    ////Trace.WriteLine($"Requested name for attribute ordinal {i}: {referenceName}");
                    return referenceName;
                });
            reader.Setup(dataReader => dataReader.GetFieldType(It.IsAny<int>()))
                .Returns((int i) => attributes.ElementAt(i).PropertyInfo.PropertyType);

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
                            var attributeDefinition = attributes.ElementAt(i);
                            var elementAt = items.ElementAt(rowNumber);
                            var item = GetBaseObject(elementAt, attributeDefinition);

                            if (item.GetType() != attributeDefinition.ReferenceNode.Value.EntityType)
                            {
                                // The underlying POCO is null; so too is the attribute
                                return true;
                            }

                            var value = attributeDefinition.GetValueDelegate.DynamicInvoke(item);

                            Trace.WriteLine(
                                $"Row {rowNumber}: Got base object {item} for {attributeDefinition} from {elementAt} with value '{value}'");

                            // Will require this to be annotated on the structure
                            if (attributeDefinition.IsIdentityColumn == false || value != null)
                            {
                                if (value == null)
                                {
                                    Trace.WriteLine($"Row {rowNumber}: Value for ({item}) {attributeDefinition.ReferenceName} was null");
                                }

                                return value == null;
                            }

                            var propertyType = attributeDefinition.PropertyInfo.PropertyType;
                            var type = propertyType.IsNullableType() ? propertyType.GetGenericArguments().First() : propertyType;

                            if (type == typeof(long))
                            {
                                var next = RandomNumber.Next(int.MaxValue);
                                attributeDefinition.SetValueDelegate.DynamicInvoke(item, next);
                                Trace.WriteLine(
                                    $"Row {rowNumber}: Set identity value for {item.GetType().Name} {item}: {attributeDefinition.ReferenceName}={next}");
                            }
                            else if (type == typeof(int))
                            {
                                var next = RandomNumber.Next(int.MaxValue);
                                attributeDefinition.SetValueDelegate.DynamicInvoke(item, next);
                                Trace.WriteLine(
                                    $"Row {rowNumber}: Set identity value for {item.GetType().Name} {item}: {attributeDefinition.ReferenceName}={next}");
                            }
                            else if (type == typeof(short))
                            {
                                var next = RandomNumber.Next(short.MaxValue);
                                attributeDefinition.SetValueDelegate.DynamicInvoke(item, next);
                                Trace.WriteLine(
                                    $"Row {rowNumber}: Set identity value for {item.GetType().Name} {item}: {attributeDefinition.ReferenceName}={next}");
                            }
                            else if (type == typeof(byte))
                            {
                                var next = RandomNumber.Next(byte.MaxValue);
                                attributeDefinition.SetValueDelegate.DynamicInvoke(item, next);
                                Trace.WriteLine(
                                    $"Row {rowNumber}: Set identity value for {item.GetType().Name} {item}: {attributeDefinition.ReferenceName}={next}");
                            }
                            else
                            {
                                throw new OperationException($"Type '{type}' is not supported as an identity column.");
                            }

                            // Not null anymore
                            return false;
                        });

            reader.Setup(dataReader => dataReader.GetValue(It.IsAny<int>()))
                .Returns(
                    (int i) =>
                        {
                            var value = GetValue(items, attributes, i, rowNumber);
                            return value;
                        });

            reader.Setup(dataReader => dataReader.GetString(It.IsAny<int>()))
                .Returns(
                    (int i) =>
                        {
                            var value = GetValue(items, attributes, i, rowNumber);
                            return (string)value;
                        });

            reader.Setup(dataReader => dataReader.GetInt64(It.IsAny<int>()))
                .Returns(
                    (int i) =>
                        {
                            var value = GetValue(items, attributes, i, rowNumber);
                            return (long)value;
                        });

            reader.Setup(dataReader => dataReader.GetInt32(It.IsAny<int>()))
                .Returns(
                    (int i) =>
                        {
                            var value = GetValue(items, attributes, i, rowNumber);
                            return (int)value;
                        });

            reader.Setup(dataReader => dataReader.GetInt16(It.IsAny<int>()))
                .Returns(
                    (int i) =>
                        {
                            var value = GetValue(items, attributes, i, rowNumber);
                            return (short)value;
                        });

            reader.Setup(dataReader => dataReader.GetByte(It.IsAny<int>()))
                .Returns(
                    (int i) =>
                        {
                            var value = GetValue(items, attributes, i, rowNumber);
                            return (byte)value;
                        });

            reader.Setup(dataReader => dataReader.GetBoolean(It.IsAny<int>()))
                .Returns(
                    (int i) =>
                        {
                            var value = GetValue(items, attributes, i, rowNumber);
                            return (bool)value;
                        });

            reader.Setup(dataReader => dataReader.GetChar(It.IsAny<int>()))
                .Returns(
                    (int i) =>
                        {
                            var value = GetValue(items, attributes, i, rowNumber);
                            return (char)value;
                        });

            reader.Setup(dataReader => dataReader.GetDateTime(It.IsAny<int>()))
                .Returns(
                    (int i) =>
                        {
                            var value = GetValue(items, attributes, i, rowNumber);
                            return (DateTime)value;
                        });

            reader.Setup(dataReader => dataReader.GetDecimal(It.IsAny<int>()))
                .Returns(
                    (int i) =>
                        {
                            var value = GetValue(items, attributes, i, rowNumber);
                            return (decimal)value;
                        });

            reader.Setup(dataReader => dataReader.GetDouble(It.IsAny<int>()))
                .Returns(
                    (int i) =>
                        {
                            var value = GetValue(items, attributes, i, rowNumber);
                            return (double)value;
                        });

            reader.Setup(dataReader => dataReader.GetFloat(It.IsAny<int>()))
                .Returns(
                    (int i) =>
                        {
                            var value = GetValue(items, attributes, i, rowNumber);
                            return (float)value;
                        });

            reader.Setup(dataReader => dataReader.GetGuid(It.IsAny<int>()))
                .Returns(
                    (int i) =>
                        {
                            var value = GetValue(items, attributes, i, rowNumber);
                            return (Guid)value;
                        });

            return reader;
        }

        /// <summary>
        /// Gets a value from the list item for the specified attribute index.
        /// </summary>
        /// <param name="items">
        /// The items to enumerate.
        /// </param>
        /// <param name="attributes">
        /// The attributes.
        /// </param>
        /// <param name="attributeIndex">
        /// The attribute index.
        /// </param>
        /// <param name="rowNumber">
        /// The row number.
        /// </param>
        /// <typeparam name="T">
        /// The type of item to locate the value for.
        /// </typeparam>
        /// <returns>
        /// The located value as an <see cref="object"/>, or <c>null</c> if the value is not found.
        /// </returns>
        private static object GetValue<T>(IEnumerable<T> items, IEnumerable<EntityAttributeDefinition> attributes, int attributeIndex, int rowNumber)
        {
            var attribute = attributes.ElementAt(attributeIndex);
            var item = items.ElementAt(rowNumber);
            var baseObject = GetBaseObject(item, attribute);

            if (baseObject.GetType() != attribute.ReferenceNode.Value.EntityType)
            {
                // The value is null because the underlying POCO in the object graph is also null
                Trace.WriteLine($"Row {rowNumber}: No base object for {attribute} from {item}");
                return null;
            }

            var value = attribute.GetValueDelegate.DynamicInvoke(baseObject);
            Trace.WriteLine($"Row {rowNumber}: Got base object {baseObject} for {attribute} from {item}: '{value}");
            return value;
        }

        /// <summary>
        /// Gets the base object for the attribute so that the correct POCO is targeted by the attribute delegate.
        /// </summary>
        /// <param name="item">
        /// The root-level POCO item.
        /// </param>
        /// <param name="attribute">
        /// The attribute to get the base object for.
        /// </param>
        /// <returns>
        /// The attribute's base <see cref="object"/>.
        /// </returns>
        private static object GetBaseObject(object item, EntityAttributeDefinition attribute)
        {
            var baseObject = item;
            var currentNode = attribute.EntityNode.List.First;
            var targetNode = attribute.EntityNode;

            while (currentNode != targetNode && currentNode?.Next != null && currentNode.Next.Value.IsVirtual == false)
            {
                var entityLocation = currentNode.Next.Value;

                // If the base object can't be found, the row has been flattened, so return the original item.
                baseObject = GetObjectProperty(baseObject, entityLocation) ?? item;
                currentNode = currentNode.Next;
            }

            return baseObject;
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

        /// <summary>
        /// The reference name comparer.
        /// TODO: Extend this to EntitySelection
        /// </summary>
        private class ReferenceNameComparer : EqualityComparer<EntityAttributeDefinition>
        {
            /// <summary>
            /// Determines whether two <see cref="EntityAttributeDefinition"/> values are equal.
            /// </summary>
            /// <param name="x">
            /// The first object to compare.
            /// </param>
            /// <param name="y">
            /// The second object to compare.
            /// </param>
            /// <returns>
            /// <see langword="true" /> if the specified objects are equal; otherwise, <see langword="false" />.
            /// </returns>
            public override bool Equals(EntityAttributeDefinition x, EntityAttributeDefinition y)
            {
                return Evaluate.Equals(x, y, definition => definition.ReferenceName);
            }

            /// <summary>
            /// When overridden in a derived class, serves as a hash function for the specified object for hashing algorithms and data structures,
            /// such as a hash table.
            /// </summary>
            /// <param name="obj">
            /// The object for which to get a hash code.
            /// </param>
            /// <returns>
            /// A hash code for the specified object.
            /// </returns>
            public override int GetHashCode(EntityAttributeDefinition obj)
            {
                return Evaluate.GenerateHashCode(
                    obj,
                    new List<Func<EntityAttributeDefinition, object>>
                        {
                            item => item.ReferenceName
                        });
            }
        }
    }
}