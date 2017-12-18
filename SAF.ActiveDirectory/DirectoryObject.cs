// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DirectoryObject.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// <summary>
//   Base class for all directory object wrappers.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.ActiveDirectory
{
    using System;
    using System.Xml.Serialization;

    /// <summary>
    /// Base class for all directory object wrappers.
    /// </summary>
    public abstract class DirectoryObject
    {
        /// <summary>
        /// Gets or sets the globally unique identifier of the directory object.
        /// </summary>
        public Guid DirectoryGuid { get; set; }

        /// <summary>
        /// Gets or sets the distinguished name of the directory object.
        /// </summary>
        public string DistinguishedName { get; set; }

        /// <summary>
        /// Gets or sets the last modified timestamp of the directory object.
        /// </summary>
        public DateTimeOffset? LastModified { get; set; }

        /// <summary>
        /// Gets or sets the created timestamp of the directory object.
        /// </summary>
        public DateTimeOffset? Created { get; set; }

        /// <summary>
        /// Gets the domain of the directory object.
        /// </summary>
        [XmlIgnore]
        public string Domain
        {
            get
            {
                string domain =
                    String.IsNullOrEmpty(this.DistinguishedName) ?
                    null :
                    DomainNameContext.ConvertFrom(
                        NamingContext.GetDefaultNamingContextFromDistinguishedName(
                            this.DistinguishedName));

                return domain;
            }
        }
    }
}
