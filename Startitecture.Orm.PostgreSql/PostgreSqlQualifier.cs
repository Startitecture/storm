// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PostgreSqlQualifier.cs" company="Startitecture">
//   Copyright (c) Startitecture. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.PostgreSql
{
    using System;

    using Startitecture.Orm.Common;
    using Startitecture.Resources;

    /// <summary>
    /// A name qualifier for PostgreSql.
    /// </summary>
    public class PostgreSqlQualifier : NameQualifier
    {
        /// <inheritdoc />
        public override string Escape(string identifier)
        {
            if (string.IsNullOrWhiteSpace(identifier))
            {
                throw new ArgumentException($"'{nameof(identifier)}' cannot be null or whitespace.", nameof(identifier));
            }

            return string.Concat('"', identifier, '"');
        }

        /// <inheritdoc />
        public override string AddParameterPrefix(string parameterName)
        {
            if (string.IsNullOrWhiteSpace(parameterName))
            {
                throw new ArgumentException(ErrorMessages.ValueCannotBeNullOrWhiteSpace, nameof(parameterName));
            }

            return string.Concat('@', parameterName);
        }
    }
}