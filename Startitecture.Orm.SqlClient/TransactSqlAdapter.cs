﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TransactSqlAdapter.cs" company="Startitecture">
//   Copyright (c) Startitecture. All rights reserved.
// </copyright>
// <summary>
//   Provides an <see cref="IRepositoryAdapter"/> implementation for SQL Server.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.SqlClient
{
    using System;
    using System.Collections.Generic;
    using System.Data.SqlTypes;
    using System.Linq;

    using JetBrains.Annotations;

    using Startitecture.Orm.Common;
    using Startitecture.Orm.Model;
    using Startitecture.Resources;

    /// <summary>
    /// Provides an <see cref="IRepositoryAdapter"/> implementation for SQL Server.
    /// </summary>
    public class TransactSqlAdapter : RepositoryAdapter
    {
        /// <summary>
        /// The mappers.
        /// </summary>
        private static readonly Dictionary<Tuple<Type, Type>, IValueMapper> Mappers = new Dictionary<Tuple<Type, Type>, IValueMapper>
                                                                                      {
                                                                                          {
                                                                                              new Tuple<Type, Type>(
                                                                                                  typeof(decimal),
                                                                                                  typeof(SqlMoney)),
                                                                                              new MoneyMapper()
                                                                                          },
                                                                                          {
                                                                                              new Tuple<Type, Type>(
                                                                                                  typeof(decimal),
                                                                                                  typeof(SqlMoney?)),
                                                                                              new MoneyMapper()
                                                                                          }
                                                                                      };

        /// <summary>
        /// Initializes a new instance of the <see cref="TransactSqlAdapter"/> class.
        /// </summary>
        /// <param name="definitionProvider">
        /// The definition provider for the SQL statements.
        /// </param>
        public TransactSqlAdapter(IEntityDefinitionProvider definitionProvider)
            : base(definitionProvider, new TransactSqlQualifier())
        {
        }

        /// <inheritdoc />
        public override IReadOnlyDictionary<Tuple<Type, Type>, IValueMapper> ValueMappers => Mappers;

        /// <inheritdoc />
        public override string CreateSelectionStatement(IEntitySet entitySet)
        {
            if (entitySet == null)
            {
                throw new ArgumentNullException(nameof(entitySet));
            }

            var statement = base.CreateSelectionStatement(entitySet);

            if (entitySet.ParentExpression != null && entitySet.OrderByExpressions.Any())
            {
                return statement + " OPTION (RECOMPILE)";
            }

            return statement;
        }

        /// <inheritdoc />
        protected override string CaptureInsertedIdentity([NotNull] string commandText, [NotNull] IEntityDefinition definition)
        {
            if (string.IsNullOrWhiteSpace(commandText))
            {
                throw new ArgumentException(ErrorMessages.ValueCannotBeNullOrWhiteSpace, nameof(commandText));
            }

            if (definition == null)
            {
                throw new ArgumentNullException(nameof(definition));
            }

            // Capture the primary key type. We'll use this later to change the declared type of the returned ID.
            var primaryKeyAttribute = definition.PrimaryKeyAttributes.FirstOrDefault();
            var primaryKeyType = primaryKeyAttribute == default
                                     ? null
                                     : definition.PrimaryKeyAttributes.First().PropertyInfo.PropertyType;

            string type;

            if (primaryKeyType == typeof(int))
            {
                type = "int";
            }
            else if (primaryKeyType == typeof(long))
            {
                type = "bigint";
            }
            else if (primaryKeyType == typeof(short))
            {
                type = "smallint";
            }
            else if (primaryKeyType == typeof(byte))
            {
                type = "tinyint";
            }
            else
            {
                throw new NotSupportedException($"Identity columns must be of an integer type. Type '{primaryKeyType}' is not supported.");
            }

            // Declare and return the ID without using the OUTPUT statement which triggers will mess with.
            commandText = string.Concat(
                $"DECLARE @NewId {type}",
                Environment.NewLine,
                commandText,
                Environment.NewLine,
                "SET @NewId = SCOPE_IDENTITY()",
                Environment.NewLine,
                "SELECT @NewId");

            return commandText;
        }
    }
}