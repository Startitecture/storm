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
    using System.Linq;
    using System.Linq.Expressions;
    using System.Runtime.CompilerServices;
    using System.Threading;
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
    public abstract class TableCommand<T>
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
        protected TableCommand([NotNull] IDbTableCommandFactory tableCommandFactory, [NotNull] IDatabaseContext databaseContext)
        {
            this.tableCommandFactory = tableCommandFactory ?? throw new ArgumentNullException(nameof(tableCommandFactory));
            this.DatabaseContext = databaseContext ?? throw new ArgumentNullException(nameof(databaseContext));
            this.EntityDefinition = this.DatabaseContext.RepositoryAdapter.DefinitionProvider.Resolve<T>();
            this.NameQualifier = this.DatabaseContext.RepositoryAdapter.NameQualifier;
        }

        /// <summary>
        /// Gets the database context for the command.
        /// </summary>
        protected IDatabaseContext DatabaseContext { get; }

        /// <summary>
        /// Gets the entity definition.
        /// </summary>
        protected IEntityDefinition EntityDefinition { get; }

        /// <summary>
        /// Gets the name qualifier for this command.
        /// </summary>
        protected INameQualifier NameQualifier { get; }

        /// <summary>
        /// Gets the from expressions declared by the caller, if any.
        /// </summary>
        protected List<LambdaExpression> FromColumnExpressions { get; } = new List<LambdaExpression>();

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

            this.DatabaseContext.OpenSharedConnection();
            var parameterName = $"{this.DatabaseContext.RepositoryAdapter.DefinitionProvider.Resolve<TItem>().EntityName}Rows";
            var commandText = this.GetCommandText<TItem>(parameterName);

            using (var command = this.tableCommandFactory.Create(this.DatabaseContext, commandText, parameterName, items))
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
        /// <param name="cancellationToken">
        /// The cancellation token for this task.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/> that is executing the command.
        /// </returns>
        public async Task ExecuteAsync<TItem>([NotNull] IEnumerable<TItem> items, CancellationToken cancellationToken)
        {
            if (items == null)
            {
                throw new ArgumentNullException(nameof(items));
            }

            await this.DatabaseContext.OpenSharedConnectionAsync(cancellationToken).ConfigureAwait(false);
            var parameterName = $"{this.DatabaseContext.RepositoryAdapter.DefinitionProvider.Resolve<TItem>().EntityName}Rows";
            var commandText = this.GetCommandText<TItem>(parameterName);

            using (var command = this.tableCommandFactory.Create(this.DatabaseContext, commandText, parameterName, items))
            {
                if (command is DbCommand asyncCommand)
                {
                    await asyncCommand.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
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

            this.DatabaseContext.OpenSharedConnection();

            using (var reader = this.ExecuteReader(items))
            {
                var entityDefinition = this.DatabaseContext.RepositoryAdapter.DefinitionProvider.Resolve<T>();

                while (reader.Read())
                {
                    yield return this.FillReturnList(reader, entityDefinition);
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
        /// <param name="cancellationToken">
        /// The cancellation token for this task.
        /// </param>
        /// <returns>
        /// An <see cref="IEnumerable{TStructure}"/> of items returned by the command.
        /// </returns>
        public async IAsyncEnumerable<T> ExecuteForResultsAsync<TItem>(
            [NotNull] IEnumerable<TItem> items,
            [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            if (items == null)
            {
                throw new ArgumentNullException(nameof(items));
            }

            await this.DatabaseContext.OpenSharedConnectionAsync(cancellationToken).ConfigureAwait(false);

            using (var reader = await this.ExecuteReaderAsync(items, cancellationToken).ConfigureAwait(false))
            {
                var entityDefinition = this.DatabaseContext.RepositoryAdapter.DefinitionProvider.Resolve<T>();

                if (reader is DbDataReader asyncReader)
                {
                    while (await asyncReader.ReadAsync(cancellationToken).ConfigureAwait(false))
                    {
                        yield return this.FillReturnList(reader, entityDefinition);
                    }
                }
                else
                {
                    while (reader.Read())
                    {
                        yield return this.FillReturnList(reader, entityDefinition);
                    }
                }
            }
        }

        /// <summary>
        /// Compiles the command text, using <paramref name="parameterName"/> as the name of the table parameter.
        /// </summary>
        /// <param name="parameterName">
        /// The name to give the table parameter.
        /// </param>
        /// <returns>
        /// The command text for the command.
        /// </returns>
        /// <typeparam name="TItem">
        /// The type of item to generate the command text for.
        /// </typeparam>
        public abstract string GetCommandText<TItem>(string parameterName);

        /// <summary>
        /// Gets the matched attributes between the source and target definitions.
        /// </summary>
        /// <param name="insertAttributes">
        /// The attributes to be inserted into the target table.
        /// </param>
        /// <returns>
        /// A <see cref="Dictionary{TKey,TValue}"/> of source attributes indexed by target attributes.
        /// </returns>
        /// <typeparam name="TItem">
        /// The type of item to get matched attributes for.
        /// </typeparam>
        /// <remarks>
        /// If there is no declaration of source attributes to insert from, then they are implicitly matched using
        /// <see cref="EntityAttributeDefinition.PhysicalName"/>.
        /// </remarks>
        protected Dictionary<EntityAttributeDefinition, EntityAttributeDefinition> GetMatchedAttributes<TItem>(
            IEnumerable<EntityAttributeDefinition> insertAttributes)
        {
            var itemDefinition = this.DatabaseContext.RepositoryAdapter.DefinitionProvider.Resolve<TItem>();
            return (this.FromColumnExpressions.Any()
                        ? insertAttributes.Select(
                            (ia, i) => new
                                       {
                                           InsertAttribute = ia,
                                           SourceAttribute = itemDefinition.Find(this.FromColumnExpressions.ElementAt(i))
                                       })
                        : from sourceAttribute in itemDefinition.DirectAttributes
                          join insertAttribute in insertAttributes on sourceAttribute.PhysicalName equals insertAttribute.PhysicalName
                          orderby sourceAttribute.Ordinal
                          select new
                                 {
                                     InsertAttribute = insertAttribute,
                                     SourceAttribute = sourceAttribute
                                 }).ToDictionary(arg => arg.InsertAttribute, arg => arg.SourceAttribute);
        }

        /// <summary>
        /// Gets the attributes to select out of the INSERTED table.
        /// </summary>
        /// <param name="insertAttributes">
        /// The attributes that will be inserted into the table.
        /// </param>
        /// <returns>
        /// An <see cref="IEnumerable{T}"/> of attributes that will be inserted into the INSERTED table.
        /// </returns>
        /// <remarks>
        /// This method will automatically add IDENTITY columns. If an IDENTITY column is already present, then it is detected so that there are no
        /// duplicate attributes.
        /// </remarks>
        protected IEnumerable<EntityAttributeDefinition> GetInsertedAttributes(IEnumerable<EntityAttributeDefinition> insertAttributes)
        {
            var insertedAttributes = new List<EntityAttributeDefinition>();

            if (this.EntityDefinition.RowIdentity.HasValue)
            {
                insertedAttributes.Add(this.EntityDefinition.RowIdentity.GetValueOrDefault());
            }

            insertedAttributes.AddRange(insertAttributes.Where(definition => definition.IsIdentityColumn == false));
            return insertedAttributes;
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
        /// <exception cref="OperationException">
        /// The <see cref="FlatPocoFactory"/> did not return a delegate of the expected type.
        /// </exception>
        private T FillReturnList(IDataReader reader, IEntityDefinition entityDefinition)
        {
            var pocoDataRequest = new PocoDataRequest(reader, entityDefinition, this.DatabaseContext);
            var mappingDelegate = FlatPocoFactory.CreateDelegate<T>(pocoDataRequest).MappingDelegate;

            if (mappingDelegate is Func<IDataReader, T> pocoDelegate)
            {
                var poco = pocoDelegate.Invoke(reader);
                return poco;
            }
            else
            {
                throw new OperationException(
                    pocoDataRequest,
                    string.Format(CultureInfo.CurrentCulture, ErrorMessages.DelegateCouldNotBeCreatedWithReader, pocoDataRequest));
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
        private IDataReader ExecuteReader<TItem>(IEnumerable<TItem> items)
        {
            var parameterName = $"{this.DatabaseContext.RepositoryAdapter.DefinitionProvider.Resolve<TItem>().EntityName}Rows";
            var commandText = this.GetCommandText<TItem>(parameterName);

            using (var command = this.tableCommandFactory.Create(this.DatabaseContext, commandText, parameterName, items))
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
        /// <param name="cancellationToken">
        /// The cancellation token for this task.
        /// </param>
        /// <typeparam name="TItem">
        /// The type of items being inserted, updated, or merged.
        /// </typeparam>
        /// <returns>
        /// The <see cref="IDataReader"/> associated with the command.
        /// </returns>
        private async Task<IDataReader> ExecuteReaderAsync<TItem>(IEnumerable<TItem> items, CancellationToken cancellationToken)
        {
            var parameterName = $"{this.DatabaseContext.RepositoryAdapter.DefinitionProvider.Resolve<TItem>().EntityName}Rows";
            var commandText = this.GetCommandText<TItem>(parameterName);

            using (var command = this.tableCommandFactory.Create(this.DatabaseContext, commandText, parameterName, items))
            {
                if (command is DbCommand asyncCommand)
                {
                    return await asyncCommand.ExecuteReaderAsync(cancellationToken).ConfigureAwait(false);
                }
                else
                {
                    return command.ExecuteReader();
                }
            }
        }
    }
}