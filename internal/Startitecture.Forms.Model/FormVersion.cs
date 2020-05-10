// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FormVersion.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Forms.Model
{
    using System;
    using System.Collections.Generic;

    using Startitecture.Core;
    using Startitecture.Resources;

    /// <summary>
    /// The form version.
    /// </summary>
    public class FormVersion : IEquatable<FormVersion>, IComparable, IComparable<FormVersion>
    {
        /// <summary>
        /// The comparison properties.
        /// </summary>
        private static readonly Func<FormVersion, object>[] ComparisonProperties =
            {
                item => item.Form,
                item => item.Revision,
                item => item.Name,
                item => item.IsActive
            };

        /// <summary>
        /// The sections.
        /// </summary>
        private readonly SortedSet<Section> sections = new SortedSet<Section>();

        /// <summary>
        /// Initializes a new instance of the <see cref="FormVersion"/> class.
        /// </summary>
        public FormVersion()
        {
            this.Name = Guid.NewGuid().ToString();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FormVersion"/> class.
        /// </summary>
        /// <param name="formVersionId">
        /// The form version id.
        /// </param>
        public FormVersion(int? formVersionId)
        {
            this.FormVersionId = formVersionId;
        }

        /// <summary>
        /// Gets the form version id.
        /// </summary>
        public int? FormVersionId { get; private set; }

        /// <summary>
        /// Gets the form.
        /// </summary>
        public Form Form { get; private set; }

        /// <summary>
        /// Gets the name.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Gets the revision.
        /// </summary>
        public int Revision { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating whether is active.
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// The sections.
        /// </summary>
        public IEnumerable<Section> Sections => this.sections;

        #region Equality and Comparison Methods and Operators

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
        public static bool operator ==(FormVersion valueA, FormVersion valueB)
        {
            return EqualityComparer<FormVersion>.Default.Equals(valueA, valueB);
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
        public static bool operator !=(FormVersion valueA, FormVersion valueB)
        {
            return !(valueA == valueB);
        }

        /// <summary>
        /// Determines if the first value is less than the second value.
        /// </summary>
        /// <param name="valueA">
        /// The first value to compare.
        /// </param>
        /// <param name="valueB">
        /// The second value to compare.
        /// </param>
        /// <returns>
        /// <c>true</c> if the first value is less than the second value; otherwise, <c>false</c>.
        /// </returns>
        public static bool operator <(FormVersion valueA, FormVersion valueB)
        {
            return Comparer<FormVersion>.Default.Compare(valueA, valueB) < 0;
        }

        /// <summary>
        /// Determines if the first value is greater than the second value.
        /// </summary>
        /// <param name="valueA">
        /// The first value to compare.
        /// </param>
        /// <param name="valueB">
        /// The second value to compare.
        /// </param>
        /// <returns>
        /// <c>true</c> if the first value is greater than the second value; otherwise, <c>false</c>.
        /// </returns>
        public static bool operator >(FormVersion valueA, FormVersion valueB)
        {
            return Comparer<FormVersion>.Default.Compare(valueA, valueB) > 0;
        }

        /// <summary>
        /// Compares the current instance with another object of the same type and returns an integer that indicates
        /// whether the current instance precedes, follows, or occurs in the same position in the sort order as the other object.
        /// </summary>
        /// <returns>
        /// A value that indicates the relative order of the objects being compared. The return value has these meanings:
        /// Value Meaning Less than zero This instance precedes <paramref name="obj"/> in the sort order. Zero This instance
        /// occurs in the same position in the sort order as <paramref name="obj"/>. Greater than zero This instance follows
        /// <paramref name="obj"/> in the sort order.
        /// </returns>
        /// <param name="obj">
        /// An object to compare with this instance.
        /// </param>
        /// <exception cref="ArgumentException">
        /// <paramref name="obj"/> is not the same type as this instance.
        /// </exception>
        /// <filterpriority>2</filterpriority>
        public int CompareTo(object obj)
        {
            return Evaluate.Compare(this, obj);
        }

        /// <summary>
        /// Compares the current object with another object of the same type.
        /// </summary>
        /// <returns>
        /// A value that indicates the relative order of the objects being compared. The return value has the following
        /// meanings: Value Meaning Less than zero This object is less than the <paramref name="other"/> parameter.Zero This
        /// object is equal to <paramref name="other"/>. Greater than zero This object is greater than <paramref name="other"/>.
        /// </returns>
        /// <param name="other">
        /// An object to compare with this object.
        /// </param>
        public int CompareTo(FormVersion other)
        {
            return Evaluate.Compare(this, other, ComparisonProperties);
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
            return this.Name;
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
        /// <param name="obj">
        /// The object to compare with the current object.
        /// </param>
        /// <filterpriority>2</filterpriority>
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
        /// <param name="other">
        /// An object to compare with this object.
        /// </param>
        public bool Equals(FormVersion other)
        {
            return Evaluate.Equals(this, other, ComparisonProperties);
        }

        #endregion

        /// <summary>
        /// Adds the current form version to a form.
        /// </summary>
        /// <param name="form">
        /// The form to add the form version to.
        /// </param>
        /// <param name="name">
        /// The name of the form.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="form"/> is null.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="name"/> is null or whitespace.
        /// </exception>
        internal void AddToForm(Form form, string name)
        {
            if (form == null)
            {
                throw new ArgumentNullException(nameof(form));
            }

            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException(ErrorMessages.ValueCannotBeNullOrWhiteSpace, nameof(name));
            }

            this.Form = form;
            this.Name = name;
            this.Revision = form.LatestVersion?.Revision ?? 0 + 1;
        }
    }
}