// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InstanceSection.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// <summary>
//   The instance section.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.Testing.Common
{
    using System;
    using System.Collections.Generic;

    using Startitecture.Core;
    using Startitecture.Orm.Model;
    using Startitecture.Orm.Schema;

    /// <summary>
    /// The instance section.
    /// </summary>
    [TableName("InstanceSection")]
    [ExplicitColumns]
    [PrimaryKey("InstanceSectionId", AutoIncrement = true)]
    public class InstanceSection : IEquatable<InstanceSection>
    {
        /// <summary>
        /// The comparison properties.
        /// </summary>
        private static readonly Func<InstanceSection, object>[] ComparisonProperties =
            {
                item => item.InstanceSectionId,
                item => item.EndDate,
                item => item.InstanceExtension,
                item => item.Instance,
                item => item.InstanceId,
                item => item.FakeDependentId,
                item => item.FakeDependentEntityDependentTimeValue,
                item => item.OwnerId,
                item => item.StartDate,
                item => item.TemplateSection,
                item => item.TemplateSectionId,
                item => item.SomeEntityUniqueName
            };

        /// <summary>
        /// Gets or sets the instance section id.
        /// </summary>
        [Column]
        public int InstanceSectionId { get; set; }

        /// <summary>
        /// Gets or sets the start date.
        /// </summary>
        [Column]
        public DateTimeOffset StartDate { get; set; }

        /// <summary>
        /// Gets or sets the end date.
        /// </summary>
        [Column]
        public DateTimeOffset EndDate { get; set; }

        /// <summary>
        /// Gets or sets the owner id.
        /// </summary>
        [Column]
        public int OwnerId { get; set; }

        /// <summary>
        /// Gets or sets the instance id.
        /// </summary>
        [Column]
        public int InstanceId { get; set; }

        /// <summary>
        /// Gets or sets the instance.
        /// </summary>
        [Relation]
        public Instance Instance { get; set; }

        /// <summary>
        /// Gets or sets the template section id.
        /// </summary>
        [Column]
        public int TemplateSectionId { get; set; }

        /// <summary>
        /// Gets or sets the template version.
        /// </summary>
        [Relation]
        public TemplateSection TemplateSection { get; set; }

        /// <summary>
        /// Gets or sets the fake dependent id.
        /// </summary>
        [RelatedEntity(typeof(FakeDependentRow))]
        public int? FakeDependentId { get; set; }

        /// <summary>
        /// Gets or sets the fake dependent id.
        /// </summary>
        [RelatedEntity(typeof(FakeDependentRow), true)]
        public DateTimeOffset? FakeDependentEntityDependentTimeValue { get; set; }

        /// <summary>
        /// Gets or sets the some entity unique name.
        /// </summary>
        [RelatedEntity(typeof(FakeMultiReferenceRow), true, "SomeEntity")]
        public string SomeEntityUniqueName { get; set; }

        /// <summary>
        /// Gets or sets the instance extension.
        /// </summary>
        [Relation]
        public InstanceExtension InstanceExtension { get; set; }

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
        public static bool operator ==(InstanceSection valueA, InstanceSection valueB)
        {
            return EqualityComparer<InstanceSection>.Default.Equals(valueA, valueB);
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
        public static bool operator !=(InstanceSection valueA, InstanceSection valueB)
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
            return $"{this.Instance}: {this.StartDate}";
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
        public bool Equals(InstanceSection other)
        {
            return Evaluate.Equals(this, other, ComparisonProperties);
        }

        #endregion
    }
}