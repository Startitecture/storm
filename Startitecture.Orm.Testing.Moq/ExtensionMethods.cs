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
        /// <typeparam name="T">
        /// The type of item in the list to create an <see cref="IStructuredCommandProvider"/> mock for.
        /// </typeparam>
        /// <returns>
        /// The <see cref="Mock"/>.
        /// </returns>
        public static Mock<IStructuredCommandProvider> MockCommandProvider<T>(
            [NotNull] this IReadOnlyCollection<T> items,
            [NotNull] IEntityDefinitionProvider definitionProvider)
        {
            if (items == null)
            {
                throw new ArgumentNullException(nameof(items));
            }

            if (definitionProvider == null)
            {
                throw new ArgumentNullException(nameof(definitionProvider));
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
            commandProvider.Setup(provider => provider.NameQualifier).Returns(new TransactSqlQualifier());
            commandProvider.Setup(provider => provider.EntityDefinitionProvider).Returns(definitionProvider);
            commandProvider
                .Setup(provider => provider.CreateCommand(It.IsAny<IStructuredCommand>(), It.IsAny<DataTable>(), It.IsAny<IDbTransaction>()))
                .Returns(command.Object);
            return commandProvider;
        }
    }
}