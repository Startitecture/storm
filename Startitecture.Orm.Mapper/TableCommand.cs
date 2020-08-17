// --------------------------------------------------------------------------------------------------------------------
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
    using System.Globalization;

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
        /// The database transaction.
        /// </summary>
        private readonly IDbTransaction databaseTransaction;

        /// <summary>
        /// Initializes a new instance of the <see cref="TableCommand{T}"/> class.
        /// </summary>
        /// <param name="tableCommandProvider">
        /// The table command provider.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="tableCommandProvider"/> is null.
        /// </exception>
        protected TableCommand([NotNull] ITableCommandProvider tableCommandProvider)
            : this(tableCommandProvider, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TableCommand{T}"/> class.
        /// </summary>
        /// <param name="tableCommandProvider">
        /// The table command provider.
        /// </param>
        /// <param name="databaseTransaction">
        /// The database transaction for the command.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="tableCommandProvider"/> is null.
        /// </exception>
        protected TableCommand([NotNull] ITableCommandProvider tableCommandProvider, IDbTransaction databaseTransaction)
        {
            this.TableCommandProvider = tableCommandProvider ?? throw new ArgumentNullException(nameof(tableCommandProvider));
            this.databaseTransaction = databaseTransaction;
            this.EntityDefinition = this.TableCommandProvider.DatabaseContext.RepositoryAdapter.DefinitionProvider.Resolve<T>();
        }

        /// <summary>
        /// Gets the table value parameter.
        /// </summary>
        public string ParameterName { get; private set; }

        /// <summary>
        /// Gets the command text.
        /// </summary>
        public abstract string CommandText { get; }

        /// <summary>
        /// Gets the table command provider.
        /// </summary>
        protected ITableCommandProvider TableCommandProvider { get; }

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

            this.ItemDefinition = this.TableCommandProvider.DatabaseContext.RepositoryAdapter.DefinitionProvider.Resolve<TItem>();
            this.ItemType = typeof(TItem);
            this.ParameterName = $"{this.ItemDefinition.EntityName}Rows";

            using (var sqlCommand = this.TableCommandProvider.CreateCommand(this, items, this.databaseTransaction))
            {
                sqlCommand.ExecuteNonQuery();
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

            this.ItemDefinition = this.TableCommandProvider.DatabaseContext.RepositoryAdapter.DefinitionProvider.Resolve<TItem>();
            this.ItemType = typeof(TItem);
            this.ParameterName = $"{this.ItemDefinition.EntityName}Rows";
            var returnList = new List<T>();

            using (var reader = this.ExecuteReader(items))
            {
                var entityDefinition = this.TableCommandProvider.DatabaseContext.RepositoryAdapter.DefinitionProvider.Resolve<T>();

                while (reader.Read())
                {
                    var pocoDataRequest = new PocoDataRequest(reader, entityDefinition, this.TableCommandProvider.DatabaseContext);
                    var mappingDelegate = FlatPocoFactory.ReturnableFactory.CreateDelegate<T>(pocoDataRequest).MappingDelegate;

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
            }

            return returnList;
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
            using (var command = this.TableCommandProvider.CreateCommand(this, items, this.databaseTransaction))
            {
                return command.ExecuteReader();
            }
        }
    }
}