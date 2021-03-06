﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LayoutPageRow.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// <summary>
//   The layout page row.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.Testing.Model.FieldEntities
{
    using System;
    using System.Collections.Generic;

    using Startitecture.Orm.Model;
    using Startitecture.Orm.Query;
    using Startitecture.Orm.Sql;

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
                    new SqlFromClause<LayoutPageRow>().InnerJoin(row => row.FormLayoutId, row => row.FormLayout.FormLayoutId)
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
