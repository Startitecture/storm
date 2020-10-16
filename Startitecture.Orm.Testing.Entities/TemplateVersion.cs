// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TemplateVersion.cs" company="Startitecture">
//   Copyright (c) Startitecture. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
// <summary>
//   The template version.
// </summary>

namespace Startitecture.Orm.Testing.Entities
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    using Startitecture.Core;
    using Startitecture.Orm.Model;
    using Startitecture.Orm.Schema;

    /// <summary>
    /// The template version.
    /// </summary>
    [Table("TemplateVersion")]
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
        [Column(Order = 1)]
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int TemplateVersionId { get; set; }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        [Column(Order = 2)]
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the revision.
        /// </summary>
        [Column(Order = 3)]
        public int Revision { get; set; }

        /// <summary>
        /// Gets or sets the template id.
        /// </summary>
        [Column(Order = 4)]
        public int TemplateId { get; set; }

        /// <summary>
        /// Gets or sets the fake dependent id.
        /// </summary>
        [RelatedEntity(typeof(SubSubRow))]
        public int FakeSubSubEntityId { get; set; }

        /// <summary>
        /// Gets or sets the fake dependent id.
        /// </summary>
        [RelatedEntity(typeof(SubSubRow))]
        [Column(nameof(SubSubRow.UniqueName))]
        public string FakeSubSubUniqueName { get; set; }

        /// <summary>
        /// Gets or sets the some entity unique name.
        /// </summary>
        [RelatedEntity(typeof(MultiReferenceRow), "OtherEntity")]
        [Column(nameof(MultiReferenceRow.UniqueName))]
        public string OtherEntityUniqueName { get; set; }

        /// <summary>
        /// Gets or sets the template.
        /// </summary>
        [Relation]
        public Template Template { get; set; }

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
        /// <param name="obj">
        /// The object to compare with the current object.
        /// </param>
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
    }
}