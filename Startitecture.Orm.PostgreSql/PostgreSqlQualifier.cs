// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PostgreSqlQualifier.cs" company="Startitecture">
//   Copyright (c) Startitecture. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.PostgreSql
{
    using Startitecture.Orm.Common;

    /// <summary>
    /// A name qualifier for PostgreSql.
    /// </summary>
    public class PostgreSqlQualifier : NameQualifier
    {
        /// <inheritdoc />
        public override string Escape(string identifier)
        {
            return string.Concat('"', identifier, '"');
        }
    }
}