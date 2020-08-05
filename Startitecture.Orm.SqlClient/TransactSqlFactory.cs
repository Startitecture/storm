// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TransactSqlFactory.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.SqlClient
{
    using JetBrains.Annotations;

    using Startitecture.Orm.Common;
    using Startitecture.Orm.Model;

    /// <summary>
    /// The Transact-SQL statement factory.
    /// </summary>
    public class TransactSqlFactory : StatementFactory
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TransactSqlFactory"/> class.
        /// </summary>
        /// <param name="definitionProvider">
        /// The definition provider.
        /// </param>
        public TransactSqlFactory([NotNull] IEntityDefinitionProvider definitionProvider)
            : base(definitionProvider, new TransactSqlQualifier())
        {
        }
    }
}