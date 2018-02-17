// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Form.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Forms.Model
{
    using System;
    using System.Collections.Generic;

    using SAF.StringResources;

    using Startitecture.Core;

    /// <summary>
    /// The form.
    /// </summary>
    public class Form : IEquatable<Form>
    {
        /// <summary>
        /// The comparison properties.
        /// </summary>
        private static readonly Func<Form, object>[] ComparisonProperties =
            {
                item => item.Name,
                item => item.IsActive
            };

        /// <summary>
        /// The form versions.
        /// </summary>
        private readonly SortedSet<FormVersion> formVersions = new SortedSet<FormVersion>();

        /// <summary>
        /// Initializes a new instance of the <see cref="Form"/> class.
        /// </summary>
        /// <param name="name">
        /// The name of the form.
        /// </param>
        /// <exception cref="ArgumentException">
        /// <paramref name="name"/> is null or whitespace.
        /// </exception>
        public Form(string name)
            : this(name, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Form"/> class.
        /// </summary>
        /// <param name="name">
        /// The name of the form.
        /// </param>
        /// <param name="formId">
        /// The form ID.
        /// </param>
        /// <exception cref="ArgumentException">
        /// <paramref name="name"/> is null or whitespace.
        /// </exception>
        public Form(string name, int? formId)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException(ErrorMessages.ValueCannotBeNullOrWhiteSpace, nameof(name));
            }

            this.Name = name;
            this.FormId = formId;
        }

        /// <summary>
        /// Gets the form ID.
        /// </summary>
        public int? FormId { get; private set; }

        /// <summary>
        /// Gets the name.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating whether is active.
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// The form versions.
        /// </summary>
        public IEnumerable<FormVersion> FormVersions => this.formVersions;

        /// <summary>
        /// The latest version.
        /// </summary>
        public FormVersion LatestVersion => this.formVersions.Max;

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
        public static bool operator ==(Form valueA, Form valueB)
        {
            return EqualityComparer<Form>.Default.Equals(valueA, valueB);
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
        public static bool operator !=(Form valueA, Form valueB)
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
        public bool Equals(Form other)
        {
            return Evaluate.Equals(this, other, ComparisonProperties);
        }

        #endregion

        /// <summary>
        /// Revises the current form.
        /// </summary>
        /// <returns>
        /// The newly created <see cref="FormVersion"/>.
        /// </returns>
        public FormVersion Revise()
        {
            return this.Revise(this.Name);
        }

        /// <summary>
        /// Revises the current form.
        /// </summary>
        /// <param name="name">
        /// The name of the form.
        /// </param>
        /// <returns>
        /// The newly created <see cref="FormVersion"/>.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// <paramref name="name"/> is null or whitespace.
        /// </exception>
        public FormVersion Revise(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException(ErrorMessages.ValueCannotBeNullOrWhiteSpace, nameof(name));
            }

            this.Name = name;
            var formVersion = new FormVersion();
            formVersion.AddToForm(this, name);
            this.formVersions.Add(formVersion);
            return formVersion;
        }
    }
}