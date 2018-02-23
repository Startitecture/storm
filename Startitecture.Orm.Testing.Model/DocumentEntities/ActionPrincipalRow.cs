// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ActionPrincipalRow.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.Testing.Model.Models
{
    using Startitecture.Orm.Schema;

    /// <summary>
    /// The person row.
    /// </summary>
    [TableName("[dbo].[ViewPersons]")]
    [ExplicitColumns]
    public class ActionPrincipalRow : EntityRowBase
    {
        /// <summary>
        /// Gets or sets the person id.
        /// </summary>
        [Column]
        public int PersonId { get; set; }

        /// <summary>
        /// Gets or sets the person id.
        /// </summary>
        [Column]
        public int? UserId { get; set; }

        /// <summary>
        /// Gets or sets the account name.
        /// </summary>
        [Column]
        public string AccountName { get; set; }

        /// <summary>
        /// Gets or sets the first name.
        /// </summary>
        [Column]
        public string FirstName { get; set; }

        /// <summary>
        /// Gets or sets the middle name.
        /// </summary>
        [Column]
        public string MiddleName { get; set; }

        /// <summary>
        /// Gets or sets the last name.
        /// </summary>
        [Column]
        public string LastName { get; set; }

        /// <summary>
        /// Gets or sets the email.
        /// </summary>
        [Column]
        public string Email { get; set; }

        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        [Column]
        public string Title { get; set; }
    }
}