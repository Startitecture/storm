// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FormVersion.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// <summary>
//   The form version.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.Data.Providers.Tests.FieldsModel
{
    using System;
    using System.Collections.Generic;

    using JetBrains.Annotations;

    using Startitecture.Core;

    /// <summary>
    /// The form version.
    /// </summary>
    public class FormVersion : IEquatable<FormVersion>, IComparable, IComparable<FormVersion>, ITrackChanges
    {
        /// <summary>
        /// The comparison properties.
        /// </summary>
        private static readonly Func<FormVersion, object>[] ComparisonProperties =
            {
                item => item.Title,
                item => item.Revision,
                item => item.Form,
                item => item.Header,
                item => item.Instructions,
                item => item.Footer,
                item => item.CreatedBy,
                item => item.CreatedTime,
                item => item.LastModifiedBy,
                item => item.LastModifiedTime,
                item => item.IsActive
            };

        /// <summary>
        /// The form layouts.
        /// </summary>
        private readonly List<FormLayout> formLayouts = new List<FormLayout>();

        /// <summary>
        /// Initializes a new instance of the <see cref="FormVersion"/> class.
        /// </summary>
        /// <param name="form">
        /// The form ID.
        /// </param>
        public FormVersion(Form form)
        {
            this.Form = form;
            this.Revision = 1;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FormVersion"/> class.
        /// </summary>
        /// <param name="form">
        /// The unified form ID.
        /// </param>
        /// <param name="revision">
        /// The version number.
        /// </param>
        /// <param name="createdBy">
        /// The created by.
        /// </param>
        /// <param name="createdTime">
        /// The created time.
        /// </param>
        /// <param name="formVersionId">
        /// The form version ID.
        /// </param>
        public FormVersion(Form form, int revision, Person createdBy, DateTimeOffset createdTime, int? formVersionId)
            : this(form)
        {
            this.Form = form;
            this.Revision = revision;
            this.CreatedBy = createdBy;
            this.CreatedTime = createdTime;
            this.FormVersionId = formVersionId;
        }

        /// <summary>
        /// Gets the form version ID.
        /// </summary>
        public int? FormVersionId { get; private set; }

        /// <summary>
        /// Gets the form.
        /// </summary>
        public Form Form { get; private set; }

        /// <summary>
        /// Gets the form ID.
        /// </summary>
        public int? FormId
        {
            get
            {
                return this.Form?.FormId;
            }
        }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Gets the version number.
        /// </summary>
        public int Revision { get; private set; }

        /// <summary>
        /// Gets or sets the header.
        /// </summary>
        public string Header { get; set; }

        /// <summary>
        /// Gets or sets the instructions.
        /// </summary>
        public string Instructions { get; set; }

        /// <summary>
        /// Gets or sets the footer.
        /// </summary>
        public string Footer { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the form version is active.
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// Gets the user account that created the version.
        /// </summary>
        public Person CreatedBy { get; private set; }

        /// <summary>
        /// Gets the created by person ID.
        /// </summary>
        public int? CreatedByPersonId
        {
            get
            {
                return this.CreatedBy?.PersonId;
            }
        }

        /// <summary>
        /// Gets the time the version was created.
        /// </summary>
        public DateTimeOffset CreatedTime { get; private set; }

        /// <summary>
        /// Gets the last modified by.
        /// </summary>
        public Person LastModifiedBy { get; private set; }

        /// <summary>
        /// Gets the last modified by person ID.
        /// </summary>
        public int? LastModifiedByPersonId
        {
            get
            {
                return this.LastModifiedBy?.PersonId;
            }
        }

        /// <summary>
        /// Gets the last modified time.
        /// </summary>
        public DateTimeOffset LastModifiedTime { get; private set; }

        /// <summary>
        /// Gets the form layouts for the current form version.
        /// </summary>
        public IEnumerable<FormLayout> FormLayouts => this.formLayouts;

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
        /// <exception cref="T:System.ArgumentException">
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
            return this.Title;
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
        /// Loads the form layouts with the specified service.
        /// </summary>
        /// <param name="formLayoutService">
        /// The form layout service.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="formLayoutService"/> is null.
        /// </exception>
        public void Load([NotNull] IFormLayoutService formLayoutService)
        {
            if (formLayoutService == null)
            {
                throw new ArgumentNullException(nameof(formLayoutService));
            }

            var layouts = formLayoutService.GetLayouts(this);
            this.formLayouts.Clear();
            this.formLayouts.AddRange(layouts);
        }

        /// <summary>
        /// Updates change tracking for the current object.
        /// </summary>
        /// <param name="editor">
        /// The editor of the object.
        /// </param>
        public void UpdateChangeTracking(Person editor)
        {
            this.UpdateChangeTracking(editor, DateTimeOffset.Now);
        }

        /// <summary>
        /// Adds a layout to the form version.
        /// </summary>
        /// <param name="name">
        /// The name of the layout.
        /// </param>
        /// <returns>
        /// The new <see cref="FormLayout"/>.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// <paramref name="name"/> is null or whitespace.
        /// </exception>
        /// <exception cref="BusinessException">
        /// The current form version has not been saved in the database.
        /// </exception>
        public FormLayout AddLayout([NotNull] string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException(FieldsMessages.StringCannotBeNullOrWhiteSpace, nameof(name));
            }

            if (this.FormVersionId.HasValue == false || this.FormVersionId < 1)
            {
                throw new BusinessException(this, FieldsMessages.UnsavedFormVersionCannotAddLayout);
            }

            var formLayout = new FormLayout(this.FormVersionId.GetValueOrDefault()) { Name = name };
            this.formLayouts.Add(formLayout);
            return formLayout;
        }

        /// <summary>
        /// Associates the form version with a form.
        /// </summary>
        /// <param name="form">
        /// The form to associate this version with.
        /// </param>
        /// <param name="editor">
        /// The editor of the form.
        /// </param>
        internal void AddTo([NotNull] Form form, [NotNull] Person editor)
        {
            if (editor == null)
            {
                throw new ArgumentNullException(nameof(editor));
            }

            if (form == null)
            {
                throw new ArgumentNullException(nameof(form));
            }

            this.Revision = form.LatestVersion?.Revision + 1 ?? 1;
            this.UpdateChangeTracking(editor);
            this.Form = form;
        }

        /// <summary>
        /// Associates the form version with a form.
        /// </summary>
        /// <param name="editor">
        /// The editor of the form.
        /// </param>
        /// <param name="modifiedTime">
        /// The date time offset.
        /// </param>
        private void UpdateChangeTracking([NotNull] Person editor, DateTimeOffset modifiedTime)
        {
            if (editor == null)
            {
                throw new ArgumentNullException(nameof(editor));
            }

            if (this.CreatedTime == DateTimeOffset.MinValue)
            {
                this.CreatedBy = editor;
                this.CreatedTime = modifiedTime;
                this.LastModifiedTime = this.CreatedTime;
            }
            else
            {
                this.LastModifiedTime = modifiedTime;
            }

            this.LastModifiedBy = editor;
        }
    }
}