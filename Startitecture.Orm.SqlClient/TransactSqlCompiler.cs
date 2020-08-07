// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TransactSqlCompiler.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.SqlClient
{
    using System;
    using System.Linq;

    using Startitecture.Orm.Common;
    using Startitecture.Orm.Model;

    /// <summary>
    /// The Transact-SQL statement factory.
    /// </summary>
    public class TransactSqlCompiler : StatementCompiler
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TransactSqlCompiler"/> class.
        /// </summary>
        /// <param name="definitionProvider">
        /// The definition provider for the SQL statements.
        /// </param>
        public TransactSqlCompiler(IEntityDefinitionProvider definitionProvider)
            : base(definitionProvider, new TransactSqlQualifier())
        {
        }

        /// <inheritdoc />
        protected override string CaptureInsertedIdentity(string commandText, IEntityDefinition definition)
        {
            // Capture the primary key type. We'll use this later to change the declared type of the returned ID.
            var primaryKeyAttribute = definition.PrimaryKeyAttributes.FirstOrDefault();
            var primaryKeyType = primaryKeyAttribute == EntityAttributeDefinition.Empty
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