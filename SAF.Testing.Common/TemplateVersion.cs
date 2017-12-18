// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TemplateVersion.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// <summary>
//   The template version.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.Testing.Common
{
    using System;
    using System.Collections.Generic;

    using SAF.Core;
    using SAF.Data;
    using SAF.Data.Providers;

    /// <summary>
    /// The template version.
    /// </summary>
    [TableName("TemplateVersion")]
    [ExplicitColumns]
    [PrimaryKey("TemplateVersionId", AutoIncrement = true)]
    public class TemplateVersion : IEquatable<TemplateVersion>
    {
        /// <summary>
        /// The comparison properties.
        /// </summary>
        private static readonly Func<TemplateVersion, object>[] ComparisonProperties =
            {
                item => item.TemplateVersionId,
                item => item.Description,
                item => item.Revision,
                item => item.FakeSubSubEntityId,
                item => item.FakeSubSubUniqueName,
                item => item.TemplateId,
                item => item.Template,
                item => item.OtherEntityUniqueName
            };

        /// <summary>
        /// Gets or sets the template version id.
        /// </summary>
        [Column]
        public int TemplateVersionId { get; set; }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        [Column]
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the revision.
        /// </summary>
        [Column]
        public int Revision { get; set; }

        /// <summary>
        /// Gets or sets the template id.
        /// </summary>
        [Column]
        public int TemplateId { get; set; }

        /// <summary>
        /// Gets or sets the fake dependent id.
        /// </summary>
        [RelatedEntity(typeof(FakeSubSubRow))]
        public int FakeSubSubEntityId { get; set; }

        /// <summary>
        /// Gets or sets the fake dependent id.
        /// </summary>
        [RelatedEntity(typeof(FakeSubSubRow), true)]
        public string FakeSubSubUniqueName { get; set; }

        /// <summary>
        /// Gets or sets the some entity unique name.
        /// </summary>
        [RelatedEntity(typeof(FakeMultiReferenceRow), true, "OtherEntity")]
        public string OtherEntityUniqueName { get; set; }

        /// <summary>
        /// Gets or sets the template.
        /// </summary>
        [Relation]
        public Template Template { get; set; }

        #region Equality and Comparison Methods

        /// <summary>
        /// Determines if two values of the same type are equal.
        /// </summary>
        /// <param name="valueA">
        /// The first value to compare.
        /// </param>
        /// <param name="valueB">
        /// The second value to compare.
        /// </param>
        /// <returns>
        /// <c>true</c> if the values are equal; otherwise, <c>false</c>.
        /// </returns>
        public static bool operator ==(TemplateVersion valueA, TemplateVersion valueB)
        {
            return EqualityComparer<TemplateVersion>.Default.Equals(valueA, valueB);
        }

        /// <summary>
        /// Determines if two values of the same type are not equal.
        /// </summary>
        /// <param name="valueA">
        /// The first value to compare.
        /// </param>
        /// <param name="valueB">
        /// The second value to compare.
        /// </param>
        /// <returns>
        /// <c>true</c> if the values are not equal; otherwise, <c>false</c>.
        /// </returns>
        public static bool operator !=(TemplateVersion valueA, TemplateVersion valueB)
        {
            return !(valueA == valueB);
        }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        /// A string that represents the current object.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override string ToString()
        {
            return this.Description;
        }

        /// <summary>
        /// Serves as the default hash function. 
        /// </summary>
        /// <returns>
        /// A hash code for the current object.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override int GetHashCode()
        {
            return Evaluate.GenerateHashCode(this, ComparisonProperties);
        }

        /// <summary>
        /// Determines whether the specified object is equal to the current object.
        /// </summary>
        /// <returns>
        /// true if the specified object  is equal to the current object; otherwise, false.
        /// </returns>
        /// <param name="obj">The object to compare with the current object. </param><filterpriority>2</filterpriority>
        public override bool Equals(object obj)
        {
            return Evaluate.Equals(this, obj);
        }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.
        /// </returns>
        /// <param name="other">An object to compare with this object.</param>
        public bool Equals(TemplateVersion other)
        {
            return Evaluate.Equals(this, other, ComparisonProperties);
        }

        #endregion
    }
}