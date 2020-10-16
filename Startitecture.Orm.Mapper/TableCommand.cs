﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TableCommand.cs" company="Startitecture">
//   Copyright (c) Startitecture. All rights reserved.
// </copyright>
// <summary>
//   Base class for SQL table commands.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.Mapper
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Common;
    using System.Globalization;
    using System.Threading.Tasks;

    using JetBrains.Annotations;

    using Startitecture.Core;
    using Startitecture.Orm.Common;
    using Startitecture.Orm.Model;
    using Startitecture.Resources;

    /// <summary>
    /// Base class for SQL table commands.
    /// </summary>
    /// <typeparam name="T">
    /// The type of the structure to use in the command.
    /// </typeparam>
    public abstract class TableCommand<T> : ITableCommand
    {
        /// <summary>
        /// Gets the table command provider for the command.
        /// </summary>
        private readonly IDbTableCommandFactory tableCommandFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="TableCommand{T}"/> class.
        /// </summary>
        /// <param name="tableCommandFactory">
        /// The table command provider.
        /// </param>
        /// <param name="databaseContext">
        /// The database context.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="tableCommandFactory"/> is null.
        /// </exception>
        protected TableCommand([NotNull] IDbTableCommandFactory tableCommandFactory, IDatabaseContext databaseContext)
        {
            this.tableCommandFactory = tableCommandFactory ?? throw new ArgumentNullException(nameof(tableCommandFactory));
            this.DatabaseContext = databaseContext;
            this.EntityDefinition = this.DatabaseContext.RepositoryAdapter.DefinitionProvider.Resolve<T>();
        }

        /// <summary>
        /// Gets the table value parameter.
        /// </summary>
        public string ParameterName { get; private set; }

        /// <summary>
        /// Gets the command text to execute.
        /// </summary>
        public abstract string CommandText { get; }

        /// <summary>
        /// Gets the database context for the command.
        /// </summary>
        protected IDatabaseContext DatabaseContext { get; }

        /// <summary>
        /// Gets the entity definition.
        /// </summary>
        protected IEntityDefinition EntityDefinition { get; }

        /// <summary>
        /// Gets the item definition for the items passed to <see cref="Execute{TItem}"/> or <see cref="ExecuteForResults{TItem}"/>.
        /// </summary>
        /// <remarks>
        /// For implementers: This property is not set until the <see cref="Execute{TItem}"/> or <see cref="ExecuteForResults{TItem}"/> methods are
        /// called.
        /// </remarks>
        protected IEntityDefinition ItemDefinition { get; private set; }

        /// <summary>
        /// Gets the item type passed to <see cref="Execute{TItem}"/> or <see cref="ExecuteForResults{TItem}"/>.
        /// </summary>
        /// <remarks>
        /// For implementers: This property is not set until the <see cref="Execute{TItem}"/> or <see cref="ExecuteForResults{TItem}"/> methods are
        /// called.
        /// </remarks>
        protected Type ItemType { get; private set; }

        /// <summary>
        /// Executes the the insertion without retrieving inserted values.
        /// </summary>
        /// <typeparam name="TItem">
        /// The type of items being inserted, updated, or merged.
        /// </typeparam>
        /// <param name="items">
        /// The items that are part of the table operation.
        /// </param>
        public void Execute<TItem>([NotNull] IEnumerable<TItem> items)
        {
            if (items == null)
            {
                throw new ArgumentNullException(nameof(items));
            }

            this.SetCommandProperties<TItem>();

            this.DatabaseContext.OpenSharedConnection();

            using (var command = this.tableCommandFactory.Create(this, items))
            {
                command.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Executes the the insertion without retrieving inserted values.
        /// </summary>
        /// <typeparam name="TItem">
        /// The type of items being inserted, updated, or merged.
        /// </typeparam>
        /// <param name="items">
        /// The items that are part of the table operation.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/> that is executing the command.
        /// </returns>
        public async Task ExecuteAsync<TItem>([NotNull] IEnumerable<TItem> items)
        {
            if (items == null)
            {
                throw new ArgumentNullException(nameof(items));
            }

            this.SetCommandProperties<TItem>();

            await this.DatabaseContext.OpenSharedConnectionAsync().ConfigureAwait(false);

            using (var command = this.tableCommandFactory.Create(this, items))
            {
                if (command is DbCommand asyncCommand)
                {
                    await asyncCommand.ExecuteNonQueryAsync().ConfigureAwait(false);
                }
                else
                {
                    command.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// Executes the table command and updates identity columns.
        /// </summary>
        /// <typeparam name="TItem">
        /// The type of items being inserted, updated, or merged.
        /// </typeparam>
        /// <param name="items">
        /// The items that are part of the table operation.
        /// </param>
        /// <returns>
        /// An <see cref="IEnumerable{TStructure}"/> of items returned by the command.
        /// </returns>
        public IEnumerable<T> ExecuteForResults<TItem>([NotNull] IEnumerable<TItem> items)
        {
            if (items == null)
            {
                throw new ArgumentNullException(nameof(items));
            }

            this.SetCommandProperties<TItem>();
            var returnList = new List<T>();

            this.DatabaseContext.OpenSharedConnection();

            using (var reader = this.ExecuteReader(items))
            {
                var entityDefinition = this.DatabaseContext.RepositoryAdapter.DefinitionProvider.Resolve<T>();

                while (reader.Read())
                {
                    this.FillReturnList(reader, entityDefinition, returnList);
                }
            }

            return returnList;
        }

        /// <summary>
        /// Executes the table command and updates identity columns.
        /// </summary>
        /// <typeparam name="TItem">
        /// The type of items being inserted, updated, or merged.
        /// </typeparam>
        /// <param name="items">
        /// The items that are part of the table operation.
        /// </param>
        /// <returns>
        /// An <see cref="IEnumerable{TStructure}"/> of items returned by the command.
        /// </returns>
        public async Task<IEnumerable<T>> ExecuteForResultsAsync<TItem>([NotNull] IEnumerable<TItem> items)
        {
            if (items == null)
            {
                throw new ArgumentNullException(nameof(items));
            }

            this.SetCommandProperties<TItem>();
            var returnList = new List<T>();

            await this.DatabaseContext.OpenSharedConnectionAsync().ConfigureAwait(false);

            using (var reader = await this.ExecuteReaderAsync(items).ConfigureAwait(false))
            {
                var entityDefinition = this.DatabaseContext.RepositoryAdapter.DefinitionProvider.Resolve<T>();

                if (reader is DbDataReader asyncReader)
                {
                    while (await asyncReader.ReadAsync().ConfigureAwait(false))
                    {
                        this.FillReturnList(reader, entityDefinition, returnList);
                    }
                }
                else
                {
                    while (reader.Read())
                    {
                        this.FillReturnList(reader, entityDefinition, returnList);
                    }
                }
            }

            return returnList;
        }

        /// <summary>
        /// Fills the return list with POCOs hydrated from the reader.
        /// </summary>
        /// <param name="reader">
        /// The reader to read.
        /// </param>
        /// <param name="entityDefinition">
        /// The entity definition for the POCOs.
        /// </param>
        /// <param name="returnList">
        /// The return list to populate.
        /// </param>
        /// <exception cref="OperationException">
        /// The <see cref="FlatPocoFactory"/> did not return a delegate of the expected type.
        /// </exception>
        private void FillReturnList(IDataReader reader, IEntityDefinition entityDefinition, ICollection<T> returnList)
        {
            var pocoDataRequest = new PocoDataRequest(reader, entityDefinition, this.DatabaseContext);
            var mappingDelegate = FlatPocoFactory.CreateDelegate<T>(pocoDataRequest).MappingDelegate;

            if (mappingDelegate is Func<IDataReader, T> pocoDelegate)
            {
                var poco = pocoDelegate.Invoke(reader);
                returnList.Add(poco);
            }
            else
            {
                throw new OperationException(
                    pocoDataRequest,
                    string.Format(CultureInfo.CurrentCulture, ErrorMessages.DelegateCouldNotBeCreatedWithReader, pocoDataRequest));
            }
        }

        /// <summary>
        /// Sets the command properties for the command.
        /// </summary>
        /// <typeparam name="TItem">
        /// The type of item to return with the command.
        /// </typeparam>
        private void SetCommandProperties<TItem>()
        {
            this.ItemDefinition = this.DatabaseContext.RepositoryAdapter.DefinitionProvider.Resolve<TItem>();
            this.ItemType = typeof(TItem);
            this.ParameterName = $"{this.ItemDefinition.EntityName}Rows";
        }

        /// <summary>
        /// Executes a command and returns a data reader.
        /// </summary>
        /// <param name="items">
        /// The items that are part of the table operation.
        /// </param>
        /// <typeparam name="TItem">
        /// The type of items being inserted, updated, or merged.
        /// </typeparam>
        /// <returns>
        /// The <see cref="IDataReader"/> associated with the command.
        /// </returns>
        private IDataReader ExecuteReader<TItem>(IEnumerable<TItem> items)
        {
            using (var command = this.tableCommandFactory.Create(this, items))
            {
                return command.ExecuteReader();
            }
        }

        /// <summary>
        /// Executes a command and returns a data reader.
        /// </summary>
        /// <param name="items">
        /// The items that are part of the table operation.
        /// </param>
        /// <typeparam name="TItem">
        /// The type of items being inserted, updated, or merged.
        /// </typeparam>
        /// <returns>
        /// The <see cref="IDataReader"/> associated with the command.
        /// </returns>
        private async Task<IDataReader> ExecuteReaderAsync<TItem>(IEnumerable<TItem> items)
        {
            using (var command = this.tableCommandFactory.Create(this, items))
            {
                if (command is DbCommand asyncCommand)
                {
                    return await asyncCommand.ExecuteReaderAsync().ConfigureAwait(false);
                }
                else
                {
                    return command.ExecuteReader();
                }
            }
        }
    }
}