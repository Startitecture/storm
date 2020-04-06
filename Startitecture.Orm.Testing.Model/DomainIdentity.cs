// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DomainIdentity.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.Testing.Model
{
    using System;

    using JetBrains.Annotations;

    using Startitecture.Resources;

    /// <summary>
    /// The domain identity.
    /// </summary>
    public class DomainIdentity
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DomainIdentity"/> class.
        /// </summary>
        /// <param name="uniqueIdentifier">
        /// The unique identifier.
        /// </param>
        public DomainIdentity([NotNull] string uniqueIdentifier)
        {
            if (string.IsNullOrWhiteSpace(uniqueIdentifier))
            {
                throw new ArgumentException(ErrorMessages.ValueCannotBeNullOrWhiteSpace, nameof(uniqueIdentifier));
            }

            this.UniqueIdentifier = uniqueIdentifier;
        }

        /// <summary>
        /// Gets the domain identity id.
        /// </summary>
        public int? DomainIdentityId { get; private set; }

        /// <summary>
        /// Gets the unique identifier.
        /// </summary>
        public string UniqueIdentifier { get; private set; }

        /// <summary>
        /// Gets or sets the first name.
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// Gets or sets the middle name.
        /// </summary>
        public string MiddleName { get; set; }

        /// <summary>
        /// Gets or sets the last name.
        /// </summary>
        public string LastName { get; set; }
    }
}