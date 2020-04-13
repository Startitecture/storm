﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StructuredCommand.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.Sql
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;
    using System.Globalization;
    using System.Linq;
    using System.Reflection;

    using Core;

    using JetBrains.Annotations;

    using Startitecture.Orm.Common;
    using Startitecture.Orm.Mapper;
    using Startitecture.Orm.Schema;
    using Startitecture.Resources;

    /// <summary>
    /// The structured SQL command.
    /// </summary>
    /// <typeparam name="TStructure">
    /// The type of the structure to use in the command.
    /// </typeparam>
    public abstract class StructuredCommand<TStructure> : IStructuredCommand
    {
        /// <summary>
        /// The database transaction.
        /// </summary>
        private readonly IDbTransaction databaseTransaction;

        /// <summary>
        /// Initializes a new instance of the <see cref="StructuredCommand{TStructure}"/> class.
        /// </summary>
        /// <param name="structuredCommandProvider">
        /// The structured command provider.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="structuredCommandProvider"/> is null.
        /// </exception>
        protected StructuredCommand([NotNull] IStructuredCommandProvider structuredCommandProvider)
            : this(structuredCommandProvider, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StructuredCommand{TStructure}"/> class.
        /// </summary>
        /// <param name="structuredCommandProvider">
        /// The structured command provider.
        /// </param>
        /// <param name="databaseTransaction">
        /// The database transaction for the command.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="structuredCommandProvider"/> is null.
        /// </exception>
        protected StructuredCommand([NotNull] IStructuredCommandProvider structuredCommandProvider, IDbTransaction databaseTransaction)
        {
            this.StructuredCommandProvider = structuredCommandProvider ?? throw new ArgumentNullException(nameof(structuredCommandProvider));
            this.databaseTransaction = databaseTransaction;

            var structureType = typeof(TStructure);
            var tableTypeAttribute = structureType.GetCustomAttributes<TableTypeAttribute>().FirstOrDefault();

            if (tableTypeAttribute == null)
            {
                var requiredAttributeName = typeof(TableTypeAttribute).Name;
                throw new OperationException(
                    structureType,
                    string.Format(CultureInfo.CurrentCulture, ErrorMessages.AttributeRequiredForType, structureType, requiredAttributeName));
            }

            this.StructureTypeName = tableTypeAttribute.TypeName;

            var structureDefinition = structuredCommandProvider.EntityDefinitionProvider.Resolve<TStructure>();
            this.Parameter = $"{structureDefinition.EntityName}Table";
        }

        /// <summary>
        /// Gets the table value parameter.
        /// </summary>
        public string Parameter { get; }

        /// <summary>
        /// Gets the structure type name.
        /// </summary>
        public string StructureTypeName { get; }

        /// <summary>
        /// Gets the command text.
        /// </summary>
        public abstract string CommandText { get; }

        /// <summary>
        /// Gets the items for the structured command.
        /// </summary>
        protected List<TStructure> Items { get; } = new List<TStructure>();

        /// <summary>
        /// Gets the structured command provider.
        /// </summary>
        protected IStructuredCommandProvider StructuredCommandProvider { get; }

        /// <summary>
        /// Executes the the insertion without retrieving inserted values.
        /// </summary>
        public void Execute()
        {
            var dataTableLoader = new DataTableLoader<TStructure>(this.StructuredCommandProvider.EntityDefinitionProvider);

            using (var dataTable = dataTableLoader.Load(this.Items))
            {
                this.Execute(dataTable);
            }
        }

        /// <summary>
        /// Executes the structured command and updates identity columns.
        /// </summary>
        /// <returns>
        /// An <see cref="IEnumerable{TStructure}"/> of items returned by the command.
        /// </returns>
        public IEnumerable<TStructure> ExecuteForResults()
        {
            var dataTableLoader = new DataTableLoader<TStructure>(this.StructuredCommandProvider.EntityDefinitionProvider);
            var returnList = new List<TStructure>();

            using (var dataTable = dataTableLoader.Load(this.Items))
            using (var reader = this.ExecuteReader(dataTable))
            {
                var entityDefinition = this.StructuredCommandProvider.EntityDefinitionProvider.Resolve<TStructure>();

                while (reader.Read())
                {
                    var pocoDataRequest = new PocoDataRequest(reader, entityDefinition);
                    var mappingDelegate = FlatPocoFactory.ReturnableFactory.CreateDelegate<TStructure>(pocoDataRequest).MappingDelegate;
                    var pocoDelegate = mappingDelegate as Func<IDataReader, TStructure>;

                    if (pocoDelegate == null)
                    {
                        throw new OperationException(
                            pocoDataRequest,
                            string.Format(CultureInfo.CurrentCulture, ErrorMessages.DelegateCouldNotBeCreatedWithReader, pocoDataRequest));
                    }

                    var poco = pocoDelegate.Invoke(reader);
                    returnList.Add(poco);
                }
            }

            return returnList;
        }

        /// <summary>
        /// Executes the current command with the specified table.
        /// </summary>
        /// <param name="dataTable">
        /// The data table containing the data to send to the operation.
        /// </param>
        /// <exception cref="Startitecture.Core.OperationException">
        /// The underlying <see cref="IDatabaseContextProvider"/> does not have a connection of the type
        /// <see cref="SqlConnection"/>.
        /// </exception>
        private void Execute([NotNull] DataTable dataTable)
        {
            if (dataTable == null)
            {
                throw new ArgumentNullException(nameof(dataTable));
            }

            using (var sqlCommand = this.StructuredCommandProvider.CreateCommand(this, dataTable, this.databaseTransaction))
            {
                sqlCommand.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Executes a command and returns a data reader.
        /// </summary>
        /// <param name="dataTable">
        /// The data table.
        /// </param>
        /// <returns>
        /// The <see cref="IDataReader"/> associated with the command.
        /// </returns>
        private IDataReader ExecuteReader([NotNull] DataTable dataTable)
        {
            if (dataTable == null)
            {
                throw new ArgumentNullException(nameof(dataTable));
            }

            using (var command = this.StructuredCommandProvider.CreateCommand(this, dataTable, this.databaseTransaction))
            {
                return command.ExecuteReader();
            }
        }
    }
}