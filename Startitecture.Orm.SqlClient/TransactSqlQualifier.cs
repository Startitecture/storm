// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TransactSqlQualifier.cs" company="Startitecture">
//   Copyright (c) Startitecture. All rights reserved.
// </copyright>
// <summary>
//   A name qualifier for Transact-SQL.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.SqlClient
{
    using System;

    using Startitecture.Orm.Common;
    using Startitecture.Resources;

    /// <summary>
    /// A name qualifier for Transact-SQL.
    /// </summary>
    public class TransactSqlQualifier : NameQualifier
    {
        /// <inheritdoc />
        public override string Escape(string identifier)
        {
            if (string.IsNullOrWhiteSpace(identifier))
            {
                throw new ArgumentException(ErrorMessages.ValueCannotBeNullOrWhiteSpace, nameof(identifier));
            }

            return $"[{identifier}]";
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