// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DirectoryContainer.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// <summary>
//   Represents an Active Directory container object.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.ActiveDirectory
{
    using System;

    using SAF.Core;

    /// <summary>
    /// Represents an Active Directory container object.
    /// </summary>
    public class DirectoryContainer : DirectoryEntity
    {
        /// <summary>
        /// A set of container classes.
        /// </summary>
        public static readonly string[] ContainerCategories =
            new string[] 
            { 
                ObjectCategory.Container, 
                ObjectCategory.OrganizationalUnit,
                ObjectCategory.Site,
                ObjectCategory.SitesContainer,
                ObjectCategory.ServersContainer,
                ObjectCategory.Subnet,
                ObjectCategory.BuiltInDomain
            };

        /// <summary>
        /// Initializes a new instance of the <see cref="DirectoryContainer"/> class.
        /// </summary>
        public DirectoryContainer()
        {
        }

        /// <summary>
        /// Gets or sets the type of container this <see cref="DirectoryContainer"/> represents.
        /// </summary>
        public ContainerType ContainerType { get; set; }

        #region Base Class Overrides

        /// <summary>
        /// Determines whether the specified <see cref="Object"/> is equal to the current <see cref="DirectoryContainer"/>.
        /// </summary>
        /// <param name="obj">The object to compare.</param>
        /// <returns>
        /// <c>true</c> if the specified <see cref="Object"/> is equal to the current <see cref="DirectoryContainer"/>; otherwise, 
        /// <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            if (base.Equals(obj))
            {
                return true;
            }

            return (obj is DirectoryContainer) ? this.Equals(obj as DirectoryContainer) : false;
        }

        /// <summary>
        /// Determines whether the specified <see cref="DirectoryContainer"/> is equal to the current <see cref="DirectoryContainer"/>.
        /// </summary>
        /// <param name="obj">The <see cref="DirectoryContainer"/> to compare.</param>
        /// <returns>
        /// <c>true</c> if the specified <see cref="DirectoryContainer"/> is equal to the current <see cref="DirectoryContainer"/>; otherwise, 
        /// <c>false</c>.
        /// </returns>
        public bool Equals(DirectoryContainer obj)
        {
            if (obj == null)
            {
                return false;
            }

            if (!base.Equals(obj))
            {
                return false;
            }

            object[] thisProperties =
                new object[] 
                {
                    this.ContainerType
                };

            object[] objProperties =
                new object[]
                {
                    obj.ContainerType
                };

            return Evaluate.CollectionEquals(thisProperties, objProperties);
        }

        /// <summary>
        /// Serves as a hash function for a particular type.
        /// </summary>
        /// <returns>A hash code for the current <see cref="DirectoryContainer"/>.</returns>
        public override int GetHashCode()
        {
            return
                base.GetHashCode() ^
                Evaluate.GenerateHashCode(
                    this.ContainerType);
        }

        #endregion
    }
}
