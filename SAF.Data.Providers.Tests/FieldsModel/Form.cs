// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Form.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// <summary>
//   The unified form.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.Data.Providers.Tests.FieldsModel
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using JetBrains.Annotations;

    using SAF.Core;

    /// <summary>
    /// The unified form.
    /// </summary>
    public class Form : IEquatable<Form>
    {
        /// <summary>
        /// The comparison properties.
        /// </summary>
        private static readonly Func<Form, object>[] ComparisonProperties = { item => item.Name };

        /// <summary>
        /// The form versions.
        /// </summary>
        private readonly SortedSet<FormVersion> formVersions = new SortedSet<FormVersion>(FormVersionComparer.Version);

        /// <summary>
        /// Initializes a new instance of the <see cref="Form"/> class.
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        public Form([NotNull] string name)
            : this(name, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Form"/> class.
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <param name="formId">
        /// The unified form id.
        /// </param>
        public Form([NotNull] string name, int? formId)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentNullException(nameof(name));
            }

            this.Name = name;
            this.FormId = formId;
        }

        /// <summary>
        /// Gets the unified form id.
        /// </summary>
        public int? FormId { get; private set; }

        /// <summary>
        /// Gets the name.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Gets the form versions.
        /// </summary>
        public IEnumerable<FormVersion> FormVersions
        {
            get
            {
                return this.formVersions;
            }
        }

        /// <summary>
        /// Gets the latest version of the form, whether active or inactive.
        /// </summary>
        public FormVersion LatestVersion
        {
            get
            {
                return this.formVersions.Max;
            }
        }

        /// <summary>
        /// Gets the most recent active version of the form.
        /// </summary>
        public FormVersion CurrentVersion
        {
            get
            {
                return this.formVersions.LastOrDefault(x => x.IsActive);
            }
        }

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
        /// Loads the form versions for the current form.
        /// </summary>
        /// <param name="versionService">
        /// The form version service.
        /// </param>
        public void Load([NotNull] IFormVersionService versionService)
        {
            if (versionService == null)
            {
                throw new ArgumentNullException(nameof(versionService));
            }

            var versions = versionService.GetVersions(this);
            this.formVersions.Clear();

            foreach (var version in versions)
            {
                this.formVersions.Add(version);
            }
        }

        /// <summary>
        /// Renames the form with the new name.
        /// </summary>
        /// <param name="newName">
        /// The new name for the form.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="newName"/> is null or whitespace.
        /// </exception>
        public void Rename([NotNull] string newName)
        {
            if (string.IsNullOrWhiteSpace(newName))
            {
                throw new ArgumentNullException(nameof(newName));
            }

            this.Name = newName;
        }

        /// <summary>
        /// Revises the form with a new form version.
        /// </summary>
        /// <param name="editor">
        /// The editor of the form.
        /// </param>
        /// <returns>
        /// The new <see cref="FormVersion"/>.
        /// </returns>
        public FormVersion Revise(Person editor)
        {
            var newVersion = new FormVersion(this)
            {
                Footer = this.CurrentVersion?.Footer,
                Header = this.CurrentVersion?.Header,
                Instructions = this.CurrentVersion?.Instructions,
                Title = this.CurrentVersion?.Title ?? this.Name
            };

            newVersion.AddTo(this, editor);
            this.formVersions.Add(newVersion);
            return newVersion;
        }

        /// <summary>
        /// The form version comparer.
        /// </summary>
        private class FormVersionComparer : Comparer<FormVersion>
        {
            /// <summary>
            /// Gets a comparer that considers only the Unified Form ID and the version number.
            /// </summary>
            public static FormVersionComparer Version { get; } = new FormVersionComparer();

            /// <summary>
            /// When overridden in a derived class, performs a comparison of two objects of the same type and returns a value
            /// indicating whether one object is less than, equal to, or greater than the other.
            /// </summary>
            /// <returns>
            /// A signed integer that indicates the relative values of <paramref name="x"/> and <paramref name="y"/>, as
            /// shown in the following table.Value Meaning Less than zero <paramref name="x"/> is less than <paramref name="y"/>.Zero
            /// <paramref name="x"/> equals <paramref name="y"/>.Greater than zero <paramref name="x"/> is greater than
            /// <paramref name="y"/>.
            /// </returns>
            /// <param name="x">
            /// The first object to compare.
            /// </param>
            /// <param name="y">
            /// The second object to compare.
            /// </param>
            public override int Compare(FormVersion x, FormVersion y)
            {
                return Evaluate.Compare(x, y, version => version.Form, version => version.Revision);
            }
        }
    }
}