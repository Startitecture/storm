// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StructuredCommand.cs" company="Startitecture">
//   Copyright (c) Startitecture. All rights reserved.
// </copyright>
// <summary>
//   Base class for structured SQL commands.
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
    using Startitecture.Resources;

    /// <summary>
    /// Base class for structured SQL commands.
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


            var structureDefinition = structuredCommandProvider.DatabaseContext.RepositoryAdapter.DefinitionProvider.Resolve<TStructure>();
            this.Parameter = $"{structureDefinition.EntityName}Rows";
        }

        /// <summary>
        /// Gets the table value parameter.
        /// </summary>
        public string Parameter { get; }

        /// <summary>
        /// Gets the structure type name.
        /// </summary>
        public abstract string StructureTypeName { get; }

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
            using (var sqlCommand = this.StructuredCommandProvider.CreateCommand(this, this.Items, this.databaseTransaction))
            {
                sqlCommand.ExecuteNonQuery();
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
            var returnList = new List<TStructure>();

            using (var reader = this.ExecuteReader())
            {
                var entityDefinition = this.StructuredCommandProvider.DatabaseContext.RepositoryAdapter.DefinitionProvider.Resolve<TStructure>();

                while (reader.Read())
                {
                    var pocoDataRequest = new PocoDataRequest(reader, entityDefinition, this.StructuredCommandProvider.DatabaseContext);
                    var mappingDelegate = FlatPocoFactory.ReturnableFactory.CreateDelegate<TStructure>(pocoDataRequest).MappingDelegate;

                    if (mappingDelegate is Func<IDataReader, TStructure> pocoDelegate)
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
        /// <returns>
        /// The <see cref="IDataReader"/> associated with the command.
        /// </returns>
        private IDataReader ExecuteReader()
        {
            using (var command = this.StructuredCommandProvider.CreateCommand(this, this.Items, this.databaseTransaction))
            {
                return command.ExecuteReader();
            }
        }
    }
}