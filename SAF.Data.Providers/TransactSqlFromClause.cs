// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TransactSqlFromClause.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// <summary>
//   Defines the TransactSqlFromClause type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.Data.Providers
{
    using SAF.Core;

    /// <summary>
    /// Contains the elements of a  Transact SQL FROM clause.
    /// </summary>
    /// <typeparam name="TItem">
    /// The type of item that is being selected from.
    /// </typeparam>
    public class TransactSqlFromClause<TItem> : EntityRelationSet<TItem>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TransactSqlFromClause{TItem}"/> class.
        /// </summary>
        public TransactSqlFromClause()
            : base(Singleton<DataItemDefinitionProvider>.Instance)
        {
        }
    }
}
