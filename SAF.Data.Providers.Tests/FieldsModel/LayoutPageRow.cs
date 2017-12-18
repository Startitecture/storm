// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LayoutPageRow.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// <summary>
//   The layout page row.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.Data.Providers.Tests.FieldsModel
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// The layout page row.
    /// </summary>
    public partial class LayoutPageRow : ICompositeEntity
    {
        /// <summary>
        /// The layout page relations.
        /// </summary>
        private static readonly Lazy<IEnumerable<IEntityRelation>> LayoutPageRelations =
            new Lazy<IEnumerable<IEntityRelation>>(
                () =>
                    new TransactSqlFromClause<LayoutPageRow>().InnerJoin(row => row.FormLayoutId, row => row.FormLayout.FormLayoutId)
                        ////.InnerJoin(row => row.FormLayout.FormVersionId, row => row.FormLayout.FormVersion.FormVersionId)
                        ////.InnerJoin(row => row.FormLayout.FormVersion.FormId, row => row.FormLayout.FormVersion.Form.FormId)
                        .Relations);

        /// <summary>
        /// Gets or sets the form layout.
        /// </summary>
        [Relation]
        public FormLayoutRow FormLayout { get; set; }

        /// <summary>
        /// Gets the entity relations.
        /// </summary>
        public IEnumerable<IEntityRelation> EntityRelations => LayoutPageRelations.Value;
    }
}
