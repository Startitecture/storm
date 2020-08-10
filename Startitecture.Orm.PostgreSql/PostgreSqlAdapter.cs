// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PostgreSqlAdapter.cs" company="Startitecture">
//   Copyright (c) Startitecture. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.PostgreSql
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;

    using JetBrains.Annotations;

    using Startitecture.Core;
    using Startitecture.Orm.Common;
    using Startitecture.Orm.Model;
    using Startitecture.Resources;

    /// <summary>
    /// The postgre sql compiler.
    /// </summary>
    public class PostgreSqlAdapter : RepositoryAdapter
    {
        /// <summary>
        /// The mappers.
        /// </summary>
        private static readonly Dictionary<Tuple<Type, Type>, IValueMapper> Mappers =
            new Dictionary<Tuple<Type, Type>, IValueMapper> { { new Tuple<Type, Type>(typeof(DateTime), typeof(DateTimeOffset)), new DateTimeMapper() } };

        /// <summary>
        /// Initializes a new instance of the <see cref="PostgreSqlAdapter"/> class.
        /// </summary>
        /// <param name="definitionProvider">
        /// The definition provider.
        /// </param>
        public PostgreSqlAdapter([NotNull] IEntityDefinitionProvider definitionProvider)
            : base(definitionProvider, new PostgreSqlQualifier())
        {
        }

        /// <inheritdoc />
        public override IReadOnlyDictionary<Tuple<Type, Type>, IValueMapper> ValueMappers => Mappers;

        /// <inheritdoc />
        public override string CreateExistsStatement(IEntitySet entitySet)
        {
            if (entitySet == null)
            {
                throw new ArgumentNullException(nameof(entitySet));
            }

            var qualifiedColumns = new List<string> { '1'.ToString() };
            var selectionStatement = this.GetSelectionStatement(entitySet, qualifiedColumns);
            return string.Format(CultureInfo.InvariantCulture, "SELECT EXISTS({0})", selectionStatement);
        }

        /// <inheritdoc />
        protected override string CaptureInsertedIdentity([NotNull] string commandText, [NotNull] IEntityDefinition definition)
        {
            if (definition == null)
            {
                throw new ArgumentNullException(nameof(definition));
            }

            if (string.IsNullOrWhiteSpace(commandText))
            {
                throw new ArgumentException(ErrorMessages.ValueCannotBeNullOrWhiteSpace, nameof(commandText));
            }

            if (definition.RowIdentity is null)
            {
                throw new OperationException(definition, "Capturing inserted IDENTITY or sequence values requires that a row identity column exist.");
            }

            return string.Concat(commandText, Environment.NewLine, $"RETURNING {this.NameQualifier.Escape(definition.RowIdentity?.PhysicalName)}");
        }
    }
}