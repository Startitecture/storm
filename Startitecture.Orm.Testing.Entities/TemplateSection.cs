﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TemplateSection.cs" company="Startitecture">
//   Copyright (c) Startitecture. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
// <summary>
//   The template section.
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
    /// The template section.
    /// </summary>
    [Table("TemplateSection")]
    public class TemplateSection : IEquatable<TemplateSection>
    {
        /// <summary>
        /// The comparison properties.
        /// </summary>
        private static readonly Func<TemplateSection, object>[] ComparisonProperties =
            {
                item => item.TemplateSectionId,
                item => item.TemplateVersionId,
                item => item.Header,
                item => item.Order,
                item => item.TemplateVersion
            };

        /// <summary>
        /// Gets or sets the template section id.
        /// </summary>
        [Column]
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int TemplateSectionId { get; set; }

        /// <summary>
        /// Gets or sets the header.
        /// </summary>
        [Column]
        public string Header { get; set; }

        /// <summary>
        /// Gets or sets the order.
        /// </summary>
        [Column]
        public int Order { get; set; }

        /// <summary>
        /// Gets or sets the template version id.
        /// </summary>
        [Column]
        public int TemplateVersionId { get; set; }

        /// <summary>
        /// Gets or sets the template version.
        /// </summary>
        [Relation]
        public TemplateVersion TemplateVersion { get; set; }

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
        public static bool operator ==(TemplateSection valueA, TemplateSection valueB)
        {
            return EqualityComparer<TemplateSection>.Default.Equals(valueA, valueB);
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
        public static bool operator !=(TemplateSection valueA, TemplateSection valueB)
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
            return this.Header;
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
        public bool Equals(TemplateSection other)
        {
            return Evaluate.Equals(this, other, ComparisonProperties);
        }
    }
}