// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Generate.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.Mapper.Tests
{
    using System;
    using System.Collections.Concurrent;
    using System.Data;
    using System.Diagnostics;
    using System.Globalization;
    using System.Reflection;

    using Moq;

    using Startitecture.Orm.Model;
    using Startitecture.Orm.Testing.Entities;

    /// <summary>
    /// The generate.
    /// </summary>
    internal class Generate
    {
        /// <summary>
        /// A default integer.
        /// </summary>
        private const int MyInt = 4930;

        /// <summary>
        /// A default nullable integer.
        /// </summary>
        private const int MyNullableInt = 9492;

        /// <summary>
        /// The my short.
        /// </summary>
        private const short MyShort = 838;

        /// <summary>
        /// The my string.
        /// </summary>
        private const string MyString = "MyString";

        /// <summary>
        /// The standard offset.
        /// </summary>
        private static readonly DateTimeOffset MyDateTimeOffset = new DateTimeOffset(2016, 4, 3, 21, 2, 9, TimeSpan.FromHours(4));

        /// <summary>
        /// The my nullable date time offset.
        /// </summary>
        private static readonly DateTimeOffset MyNullableDateTimeOffset = new DateTimeOffset(2016, 3, 13, 7, 23, 29, TimeSpan.FromHours(5));

        /// <summary>
        /// Creates a fake complex row.
        /// </summary>
        /// <returns>
        /// A new <see cref="ComplexFlatRow" /> instance.
        /// </returns>
        public static ComplexFlatRow CreateFakeComplexRow()
        {
            return new ComplexFlatRow
                       {
                           CreatedByDescription = MyString, 
                           CreatedByFakeMultiReferenceEntityId = MyInt, 
                           CreatedByUniqueName = MyString, 
                           UniqueName = MyString, 
                           FakeSubEntityId = MyInt, 
                           FakeSubSubEntityId = MyInt, 
                           ModifiedByUniqueName = MyString, 
                           ModifiedTime = MyDateTimeOffset, 
                           Description = MyString, 
                           FakeSubSubEntityUniqueName = MyString, 
                           FakeEnumerationId = MyInt, 
                           CreationTime = MyDateTimeOffset, 
                           FakeSubEntityUniqueName = MyString, 
                           FakeDependentEntityDependentTimeValue = MyNullableDateTimeOffset, 
                           FakeSubEntityUniqueOtherId = MyShort, 
                           FakeOtherEnumerationId = MyInt, 
                           FakeSubSubEntityDescription = MyString, 
                           FakeDependentEntityId = MyNullableInt, 
                           FakeSubEntityDescription = MyString, 
                           FakeDependentEntityDependentIntegerValue = MyNullableInt, 
                           ModifiedByDescription = MyString, 
                           ModifiedByFakeMultiReferenceEntityId = MyInt, 
                           FakeComplexEntityId = MyInt
                       };
        }

        /// <summary>
        /// Creates a fake complex row.
        /// </summary>
        /// <param name="withDependentEntity">
        /// A value indicating whether to create the row with a dependent entity.
        /// </param>
        /// <returns>
        /// A new <see cref="ComplexFlatRow"/> instance.
        /// </returns>
        public static ComplexRaisedRow CreateFakeRaisedComplexRow(bool withDependentEntity)
        {
            var createdBy = new MultiReferenceRow { Description = MyString, FakeMultiReferenceEntityId = MyInt, UniqueName = MyString };
            var fakeSubSubEntity = new SubSubRow { FakeSubSubEntityId = MyInt, UniqueName = MyString, Description = MyString };
            var fakeSubEntity = new SubRow
                                    {
                                        FakeSubEntityId = MyInt,
                                        FakeSubSubEntityId = MyInt,
                                        SubSubEntity = fakeSubSubEntity,
                                        UniqueName = MyString,
                                        UniqueOtherId = MyShort,
                                        Description = MyString
                                    };

            var modifiedBy = new MultiReferenceRow { FakeMultiReferenceEntityId = MyInt, Description = MyString, UniqueName = MyString };

            var fakeDependentEntity = new DependentRow
                                          {
                                              FakeDependentEntityId = MyInt,
                                              DependentIntegerValue = MyInt,
                                              DependentTimeValue = MyDateTimeOffset
                                          };

            return new ComplexRaisedRow
                       {
                           CreatedBy = createdBy,
                           UniqueName = MyString,
                           FakeSubEntityId = MyInt,
                           SubEntity = fakeSubEntity,
                           ModifiedBy = modifiedBy,
                           DependentEntity = withDependentEntity ? fakeDependentEntity : null,
                           ModifiedTime = MyDateTimeOffset,
                           Description = MyString,
                           FakeEnumerationId = MyInt,
                           CreationTime = MyDateTimeOffset,
                           FakeOtherEnumerationId = MyInt,
                           ComplexEntityId = MyInt
                       };
        }

        /// <summary>
        /// Creates a POCO data request.
        /// </summary>
        /// <param name="item">
        /// The item to use as the source of the property values.
        /// </param>
        /// <param name="definitionProvider">
        /// The definition provider.
        /// </param>
        /// <typeparam name="T">
        /// The type of POCO to create the request for.
        /// </typeparam>
        /// <returns>
        /// A <see cref="Startitecture.Orm.Mapper.PocoDataRequest"/> for the specified type.
        /// </returns>
        public static PocoDataRequest CreatePocoDataRequest<T>(T item, IEntityDefinitionProvider definitionProvider)
        {
            var entityDefinition = definitionProvider.Resolve<T>();
            var attributeDefinitions = new ConcurrentDictionary<string, EntityAttributeDefinition>();

            foreach (var attributeDefinition in entityDefinition.ReturnableAttributes)
            {
                // Don't add duplicates in case of multiple paths.
                if (attributeDefinitions.ContainsKey(attributeDefinition.ReferenceName))
                {
                    continue;
                }

                attributeDefinitions.AddOrUpdate(attributeDefinition.ReferenceName, attributeDefinition, (s, column) => attributeDefinition);
            }

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
                        Trace.TraceError(
                            $"{entityDefinition.EntityContainer}.{baseObject.GetType().Name} '{baseObject}' for {attribute.Value}:{ex.Message}");

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
                    dataReader.Setup(reader => reader.GetString(It.Is<int>(i => i == localOrdinal))).Returns(Convert.ToString(value, CultureInfo.CurrentCulture));
                }
                else if (propertyType == typeof(DateTimeOffset))
                {
                    dataReader.Setup(reader => reader.GetValue(It.Is<int>(i => i == localOrdinal)))
                        .Returns((DateTimeOffset?)value ?? default);
                }
                else if (propertyType == typeof(decimal))
                {
                    dataReader.Setup(reader => reader.GetValue(It.Is<int>(i => i == localOrdinal))).Returns(value);
                    dataReader.Setup(reader => reader.GetDecimal(It.Is<int>(i => i == localOrdinal))).Returns((decimal?)value ?? default);
                }   

                ordinal++;
            }

            var pocoDataRequest = new PocoDataRequest(dataReader.Object, entityDefinition)
                                      {
                                          ////FieldCount = attributeDefinitions.Count,
                                          FirstColumn = 0
                                      };

            return pocoDataRequest;
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