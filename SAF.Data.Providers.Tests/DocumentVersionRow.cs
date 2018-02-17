// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DocumentVersionRow.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.Data.Providers.Tests
{
    using System;
    using System.Collections.Generic;

    using Startitecture.Orm.Model;
    using Startitecture.Orm.Query;
    using Startitecture.Orm.Sql;

    /// <summary>
    /// The document version row.
    /// </summary>
    public partial class DocumentVersionRow : ICompositeEntity
    {
        /// <summary>
        /// The document version relations.
        /// </summary>
        private static readonly Lazy<IEnumerable<IEntityRelation>> DocumentVersionRelations =
            new Lazy<IEnumerable<IEntityRelation>>(
                () => new SqlFromClause<DocumentVersionRow>()
                    .InnerJoin(row => row.DocumentId, row => row.Document.DocumentId).Relations);

        /// <summary>
        /// Gets or sets the document.
        /// </summary>
        [Relation]
        public DocumentRow Document { get; set; }

        /// <summary>Gets the entity relations.</summary>
        public IEnumerable<IEntityRelation> EntityRelations => DocumentVersionRelations.Value;
    }
}