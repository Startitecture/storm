// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UserRow.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// <summary>
//   The user row.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.Data.Providers.Tests.FieldsModel
{
    using System;
    using System.Collections.Generic;

    using Startitecture.Orm.Model;
    using Startitecture.Orm.Query;

    /// <summary>
    /// The user row.
    /// </summary>
    public partial class UserRow : ICompositeEntity
    {
        /// <summary>
        /// The user relations.
        /// </summary>
        private static readonly Lazy<IEnumerable<IEntityRelation>> UserRelations =
            new Lazy<IEnumerable<IEntityRelation>>(
                () => new TransactSqlFromClause<UserRow>().InnerJoin<PersonRow>(row => row.UserId, row => row.PersonId).Relations);

        /// <summary>
        /// Gets or sets the PersonId.
        /// </summary>
        [RelatedEntity(typeof(PersonRow))]
        public int PersonId { get; set; }

        /// <summary>
        /// Gets or sets the FirstName.
        /// </summary>
        [RelatedEntity(typeof(PersonRow))]
        public string FirstName { get; set; }

        /// <summary>
        /// Gets or sets the MiddleName.
        /// </summary>
        [RelatedEntity(typeof(PersonRow))]
        public string MiddleName { get; set; }

        /// <summary>
        /// Gets or sets the LastName.
        /// </summary>
        [RelatedEntity(typeof(PersonRow))]
        public string LastName { get; set; }

        /// <summary>
        /// Gets the entity relations.
        /// </summary>
        public IEnumerable<IEntityRelation> EntityRelations => UserRelations.Value;
    }
}
