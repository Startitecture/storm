// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FieldPlacementRow.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// <summary>
//   The field placement row.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.Data.Providers.Tests.FieldsModel
{
    using System;
    using System.Collections.Generic;

    using Startitecture.Orm.Model;
    using Startitecture.Orm.Query;
    using Startitecture.Orm.Sql;

    /// <summary>
    /// The field placement row.
    /// </summary>
    public partial class FieldPlacementRow : ICompositeEntity
    {
        /// <summary>
        /// The field placement relations.
        /// </summary>
        private static readonly Lazy<IEnumerable<IEntityRelation>> FieldPlacementRelations =
            new Lazy<IEnumerable<IEntityRelation>>(
                () =>
                    new SqlFromClause<FieldPlacementRow>()
                        .InnerJoin(row => row.LayoutSectionId, row => row.LayoutSection.LayoutSectionId)
                        .InnerJoin(row => row.UnifiedFieldId, row => row.UnifiedField.UnifiedFieldId)
                        .Relations);

        /// <summary>
        /// Gets or sets the layout section.
        /// </summary>
        [Relation]
        public LayoutSectionRow LayoutSection { get; set; }

        /// <summary>
        /// Gets or sets the unified field.
        /// </summary>
        [Relation]
        public UnifiedFieldRow UnifiedField { get; set; }

        /// <summary>
        /// Gets the entity relations.
        /// </summary>
        public IEnumerable<IEntityRelation> EntityRelations
        {
            get
            {
                return FieldPlacementRelations.Value;
            }
        }
    }
}
