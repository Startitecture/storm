// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PostgreSqlCompiler.cs" company="Startitecture">
//   Copyright (c) Startitecture. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.PostgreSql
{
    using System;

    using JetBrains.Annotations;

    using Startitecture.Core;
    using Startitecture.Orm.Common;
    using Startitecture.Orm.Model;
    using Startitecture.Resources;

    /// <summary>
    /// The postgre sql compiler.
    /// </summary>
    public class PostgreSqlCompiler : StatementCompiler
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PostgreSqlCompiler"/> class.
        /// </summary>
        /// <param name="definitionProvider">
        /// The definition provider.
        /// </param>
        public PostgreSqlCompiler([NotNull] IEntityDefinitionProvider definitionProvider)
            : base(definitionProvider, new PostgreSqlQualifier())
        {
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

            if (definition.RowIdentity is null)
            {
                throw new OperationException(definition, "Capturing inserted IDENTITY or sequence values requires that a row identity column exist.");
            }

            return string.Concat(commandText, Environment.NewLine, $"RETURNING {this.NameQualifier.Escape(definition.RowIdentity?.PhysicalName)}");
        }
    }
}