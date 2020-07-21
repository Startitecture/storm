// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SqlClientAdapter.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace Startitecture.Orm.SqlClient
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;
    using System.Globalization;
    using System.Linq;
    using System.Linq.Expressions;

    using Common;

    using JetBrains.Annotations;

    using Startitecture.Core;
    using Startitecture.Orm.Model;
    using Startitecture.Orm.Sql;
    using Startitecture.Resources;

    /// <summary>
    /// Implements the <see cref="IRepositoryAdapter"/> interface for SQL Server connections.
    /// </summary>
    public class SqlClientAdapter : IRepositoryAdapter
    {
        /// <summary>
        /// The data context.
        /// </summary>
        private readonly IDatabaseContext dataContext;

        /// <summary>
        /// The query factory.
        /// </summary>
        private readonly IQueryFactory queryFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlClientAdapter"/> class.
        /// </summary>
        /// <param name="dataContext">
        /// The data context.
        /// </param>
        public SqlClientAdapter([NotNull] IDatabaseContext dataContext)
        {
            this.dataContext = dataContext ?? throw new ArgumentNullException(nameof(dataContext));
            this.queryFactory = new TransactSqlQueryFactory(dataContext.DefinitionProvider);
        }

        /// <inheritdoc />
        public bool Contains<TDataItem>([NotNull] EntitySelection<TDataItem> selection)
        {
            if (selection == null)
            {
                throw new ArgumentNullException(nameof(selection));
            }

            // Always remember to supply this method with an array of values!
            var queryContext = new QueryContext(selection, this.dataContext.DefinitionProvider.Resolve<TDataItem>(), StatementOutputType.Contains);
            var sql = this.queryFactory.Create(queryContext);

            return this.dataContext.ExecuteScalar<int>(sql, selection.PropertyValues.ToArray()) > 0;
        }

        /// <inheritdoc />
        public TDataItem FirstOrDefault<TDataItem>([NotNull] EntitySelection<TDataItem> selection)
        {
            if (selection == null)
            {
                throw new ArgumentNullException(nameof(selection));
            }

            try
            {
                var statementOutputType = selection.ParentExpression == null
                                              ? StatementOutputType.Select
                                              : StatementOutputType.CteSelect;

                var queryContext = new QueryContext(selection, this.dataContext.DefinitionProvider.Resolve<TDataItem>(), statementOutputType);
                var statement = this.queryFactory.Create(queryContext);

                ////Trace.TraceInformation("Using unique query: {0} [{1}]", sql.SQL, String.Join(", ", sql.Arguments));
                return this.dataContext.FirstOrDefault<TDataItem>(statement, selection.PropertyValues.ToArray());
            }
            catch (InvalidOperationException ex)
            {
                throw new RepositoryException(selection, ex.Message, ex);
            }
            catch (DataException ex)
            {
                throw new RepositoryException(selection, ex.Message, ex);
            }
            catch (SqlException ex)
            {
                throw new RepositoryException(selection, ex.Message, ex);
            }
        }

        /// <inheritdoc />
        public IEnumerable<TDataItem> SelectItems<TDataItem>(EntitySelection<TDataItem> selection)
        {
            if (selection == null)
            {
                throw new ArgumentNullException(nameof(selection));
            }

            try
            {
                var statementOutputType = selection.ParentExpression == null
                                              ? StatementOutputType.Select
                                              : StatementOutputType.CteSelect;

                var queryContext = new QueryContext(selection, this.dataContext.DefinitionProvider.Resolve<TDataItem>(), statementOutputType);
                var statement = this.queryFactory.Create(queryContext);

                ////Trace.TraceInformation("Using select query: {0} [{1}]", sql.SQL, String.Join(", ", sql.Arguments));
                return this.dataContext.Query<TDataItem>(statement, selection.PropertyValues.ToArray());
            }
            catch (InvalidOperationException ex)
            {
                throw new RepositoryException(selection, ex.Message, ex);
            }
            catch (DataException ex)
            {
                throw new RepositoryException(selection, ex.Message, ex);
            }
            catch (SqlException ex)
            {
                throw new RepositoryException(selection, ex.Message, ex);
            }
        }

        /// <inheritdoc />
        public T ExecuteScalar<T>([NotNull] ISelection selection)
        {
            if (selection == null)
            {
                throw new ArgumentNullException(nameof(selection));
            }

            var queryContext = new QueryContext(selection, this.dataContext.DefinitionProvider.Resolve(selection.EntityType), StatementOutputType.Select);
            var statement = this.queryFactory.Create(queryContext);
            return this.dataContext.ExecuteScalar<T>(statement, selection.PropertyValues.ToArray());
        }

        /// <inheritdoc />
        public TDataItem Insert<TDataItem>(TDataItem dataItem)
            where TDataItem : ITransactionContext
        {
            if (Evaluate.IsNull(dataItem))
            {
                throw new ArgumentNullException(nameof(dataItem));
            }

            object result;

            try
            {
                result = this.dataContext.Insert(dataItem);
            }
            catch (InvalidOperationException ex)
            {
                throw new RepositoryException(dataItem, ex.Message, ex);
            }
            catch (DataException ex)
            {
                throw new RepositoryException(dataItem, ex.Message, ex);
            }
            catch (SqlException ex)
            {
                throw new RepositoryException(dataItem, ex.Message, ex);
            }

            if (result == null)
            {
                string message = string.Format(CultureInfo.CurrentCulture, ErrorMessages.DataItemInsertionFailed, typeof(TDataItem).Name, dataItem);
                throw new RepositoryException(dataItem, message);
            }

            return dataItem;
        }

        /// <inheritdoc />
        public int Update<TDataItem>(
            [NotNull] TDataItem dataItem,
            [NotNull] EntitySelection<TDataItem> selection,
            [NotNull] params Expression<Func<TDataItem, object>>[] setExpressions)
        {
            if (dataItem == null)
            {
                throw new ArgumentNullException(nameof(dataItem));
            }

            if (selection == null)
            {
                throw new ArgumentNullException(nameof(selection));
            }

            if (setExpressions == null)
            {
                throw new ArgumentNullException(nameof(setExpressions));
            }

            var transactSqlUpdate = new UpdateStatement<TDataItem>(this.dataContext.DefinitionProvider, this.queryFactory, new TransactSqlQualifier(), selection);
            var updateOperation = setExpressions.Any() ? transactSqlUpdate.Set(dataItem, setExpressions) : transactSqlUpdate.Set(dataItem);

            try
            {
                // Always use ToArray()!
                return this.dataContext.Execute(updateOperation.ExecutionStatement, updateOperation.ExecutionParameters.ToArray());
            }
            catch (InvalidOperationException ex)
            {
                throw new RepositoryException(dataItem, ex.Message, ex);
            }
            catch (DataException ex)
            {
                throw new RepositoryException(dataItem, ex.Message, ex);
            }
            catch (SqlException ex)
            {
                throw new RepositoryException(dataItem, ex.Message, ex);
            }
        }

        /// <inheritdoc />
        public int DeleteSelection<TDataItem>(EntitySelection<TDataItem> selection)
        {
            if (selection == null)
            {
                throw new ArgumentNullException(nameof(selection));
            }

            try
            {
                var queryContext = new QueryContext(selection, this.dataContext.DefinitionProvider.Resolve<TDataItem>(), StatementOutputType.Delete);
                var statement = this.queryFactory.Create(queryContext);
                return this.dataContext.Execute(statement, selection.PropertyValues.ToArray());
            }
            catch (InvalidOperationException ex)
            {
                throw new RepositoryException(selection, ex.Message, ex);
            }
            catch (DataException ex)
            {
                throw new RepositoryException(selection, ex.Message, ex);
            }
            catch (SqlException ex)
            {
                throw new RepositoryException(selection, ex.Message, ex);
            }
        }
    }
}
