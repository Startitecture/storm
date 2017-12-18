// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DirectoryEntity.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// <summary>
//   Contains common properties for entities that are directory objects, such as users, groups and computers.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.ActiveDirectory
{
    using System;

    using SAF.Core;

    /// <summary>
    /// Contains common properties for entities that are directory objects, such as users, groups and computers.
    /// </summary>
    public class DirectoryEntity : DirectoryObject
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DirectoryEntity"/> class.
        /// </summary>
        public DirectoryEntity()
        {
        }

        /// <summary>
        /// Gets or sets the name of the directory object.
        /// </summary>
        public string Name { get; set; }

        #region Base Class Overrides

        /// <summary>
        /// Returns a <see cref="String"/> representation of the current <see cref="DirectoryEntity"/>.
        /// </summary>
        /// <returns>A <see cref="String"/> representing the current <see cref="DirectoryEntity"/>.</returns>
        public override string ToString()
        {
            return this.DistinguishedName;
        }

        /// <summary>
        /// Determines whether the specified <see cref="Object"/> is equal to the current <see cref="DirectoryEntity"/>.
        /// </summary>
        /// <param name="obj">The object to compare.</param>
        /// <returns>
        /// <c>true</c> if the specified <see cref="Object"/> is equal to the current <see cref="DirectoryEntity"/>; otherwise, 
        /// <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            if (base.Equals(obj))
            {
                return true;
            }

            return (obj is DirectoryEntity) ? this.Equals(obj as DirectoryEntity) : false;
        }

        /// <summary>
        /// Determines whether the specified <see cref="DirectoryEntity"/> is equal to the current <see cref="DirectoryEntity"/>.
        /// </summary>
        /// <param name="obj">The <see cref="DirectoryEntity"/> to compare.</param>
        /// <returns>
        /// <c>true</c> if the specified <see cref="DirectoryEntity"/> is equal to the current <see cref="DirectoryEntity"/>; otherwise, 
        /// <c>false</c>.
        /// </returns>
        public bool Equals(DirectoryEntity obj)
        {
            if (obj == null)
            {
                return false;
            }

            object[] thisProperties =
                new object[] 
                {
                    this.Created,
                    this.DirectoryGuid,
                    this.DistinguishedName,
                    this.Domain,
                    this.LastModified,
                    this.Name
                };

            object[] objProperties =
                new object[]
                {
                    obj.Created,
                    obj.DirectoryGuid,
                    obj.DistinguishedName,
                    obj.Domain,
                    obj.LastModified,
                    obj.Name
                };

            return Evaluate.CollectionEquals(thisProperties, objProperties);
        }

        /// <summary>
        /// Serves as a hash function for a particular type.
        /// </summary>
        /// <returns>A hash code for the current <see cref="DirectoryEntity"/>.</returns>
        public override int GetHashCode()
        {
            return
                Evaluate.GenerateHashCode(
                    this.Created,
                    this.DirectoryGuid,
                    this.DistinguishedName,
                    this.Domain,
                    this.LastModified,
                    this.Name);
        }

        #endregion
    }
}
