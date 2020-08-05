// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TransactSqlQualifier.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.SqlClient
{
    using System;

    using Model;

    using Startitecture.Orm.Common;
    using Startitecture.Resources;

    /// <summary>
    /// Qualifies an <see cref="EntityAttributeDefinition"/> for Transact-SQL.
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
    }
}