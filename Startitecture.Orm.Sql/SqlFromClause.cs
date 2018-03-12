// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SqlFromClause.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// <summary>
//   Defines the SqlFromClause type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.Sql
{
    using Startitecture.Core;
    using Startitecture.Orm.Query;
    using Startitecture.Orm.Schema;

    /// <summary>
    /// Contains the elements of a  Transact SQL FROM clause.
    /// </summary>
    /// <typeparam name="TItem">
    /// The type of item that is being selected from.
    /// </typeparam>
    public class SqlFromClause<TItem> : EntityRelationSet<TItem>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SqlFromClause{TItem}"/> class.
        /// </summary>
        public SqlFromClause()
            : base(Singleton<PetaPocoDefinitionProvider>.Instance)
        {
        }
    }
}
