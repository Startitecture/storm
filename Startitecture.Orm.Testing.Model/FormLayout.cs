// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FormLayout.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// <summary>
//   The form layout.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.Testing.Model
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using JetBrains.Annotations;

    using Startitecture.Core;

    /// <summary>
    /// The form layout.
    /// </summary>
    public class FormLayout : IEquatable<FormLayout>, IComparable<FormLayout>, IComparable
    {
        /// <summary>
        /// The comparison properties.
        /// </summary>
        private static readonly Func<FormLayout, object>[] ComparisonProperties =
            {
                item => item.FormVersionId,
                item => item.Name,
                item => item.Instructions
            };

        /// <summary>
        /// The layout pages.
        /// </summary>
        private readonly SortedSet<LayoutPage> layoutPages = new SortedSet<LayoutPage>(Singleton<OrderedElementComparer>.Instance);

        /// <summary>
        /// Initializes a new instance of the <see cref="FormLayout"/> class.
        /// </summary>
        /// <param name="formVersionId">
        /// The form version ID.
        /// </param>
        public FormLayout(int formVersionId)
            : this(formVersionId, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FormLayout"/> class.
        /// </summary>
        /// <param name="formVersionId">
        /// The form version ID.
        /// </param>
        /// <param name="formLayoutId">
        /// The form layout ID.
        /// </param>
        public FormLayout(int formVersionId, int? formLayoutId)
        {
            this.FormVersionId = formVersionId;
            this.FormLayoutId = formLayoutId;
        }

        /// <summary>
        /// Gets the form layout ID.
        /// </summary>
        public int? FormLayoutId { get; private set; }

        /// <summary>
        /// Gets the form version ID.
        /// </summary>
        public int FormVersionId { get; private set; }

        /// <summary>
        /// Gets or sets the layout name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the form layout instructions.
        /// </summary>
        public string Instructions { get; set; }

        /// <summary>
        /// Gets or sets the form layout footer.
        /// </summary>
        public string Footer { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the form layout is active.
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// Gets the layout pages on the layout.
        /// </summary>
        public IEnumerable<LayoutPage> LayoutPages => this.layoutPages;

        /// <summary>
        /// Gets the layout sections on the layout.
        /// </summary>
        public IEnumerable<LayoutSection> LayoutSections
        {
            get
            {
                return
                    this.LayoutPages.SelectMany(x => x.LayoutPageSections)
                        .OrderBy(x => x.LayoutPage)
                        .ThenBy(x => x.Order)
                        .Select(x => x.LayoutSection);
            }
        }

        /// <summary>
        /// Gets the field placements on the layout.
        /// </summary>
        public IEnumerable<FieldPlacement> FieldPlacements
        {
            get
            {
                return this.LayoutSections.SelectMany(x => x.FieldPlacements);
            }
        }

        /// <summary>
        /// Gets the unified fields on the layout.
        /// </summary>
        public IEnumerable<UnifiedField> UnifiedFields
        {
            get
            {
                return this.FieldPlacements.Select(x => x.UnifiedField);
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
        public static bool operator ==(FormLayout valueA, FormLayout valueB)
        {
            return EqualityComparer<FormLayout>.Default.Equals(valueA, valueB);
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
        public static bool operator !=(FormLayout valueA, FormLayout valueB)
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
        public static bool operator <(FormLayout valueA, FormLayout valueB)
        {
            return Comparer<FormLayout>.Default.Compare(valueA, valueB) < 0;
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
        public static bool operator >(FormLayout valueA, FormLayout valueB)
        {
            return Comparer<FormLayout>.Default.Compare(valueA, valueB) > 0;
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
        public int CompareTo(FormLayout other)
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
        public bool Equals(FormLayout other)
        {
            return Evaluate.Equals(this, other, ComparisonProperties);
        }

        #endregion

        /// <summary>
        /// Adds a page to the form layout.
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <exception cref="ArgumentException">
        /// <paramref name="name"/> is null or white space.
        /// </exception>
        /// <returns>
        /// The newly added <see cref="LayoutPage"/>.
        /// </returns>
        public LayoutPage AddPage([NotNull] string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException(FieldsMessages.StringCannotBeNullOrWhiteSpace, nameof(name));
            }

            var order = (short)(this.layoutPages.Max?.Order + 1 ?? 1);
            var page = new LayoutPage { Name = name };
            page.AddToLayout(this, order);

            if (this.layoutPages.Add(page) == false)
            {
                throw new BusinessException(page, FieldsMessages.SortedItemAlreadyAddedToCollection);
            }

            return page;
        }

        /// <summary>
        /// Removes a page from the layout.
        /// </summary>
        /// <param name="layoutPage">
        /// The layout page to remove.
        /// </param>
        /// <returns>
        /// <c>true</c> if the page is found and removed; otherwise, <c>false</c>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="layoutPage"/> is null.
        /// </exception>
        public bool RemovePage([NotNull] LayoutPage layoutPage)
        {
            if (layoutPage == null)
            {
                throw new ArgumentNullException(nameof(layoutPage));
            }

            var removed = this.layoutPages.Remove(layoutPage);

            if (removed)
            {
                var searcher = new OrderedElementSearcher<LayoutPage>(this.layoutPages);

                foreach (var page in this.layoutPages)
                {
                    page.SetOrder(searcher);
                }
            }

            return removed;
        }

        /// <summary>
        /// Loads the pages into the layout.
        /// </summary>
        /// <param name="pageService">
        /// The page service to use to load the pages.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="pageService"/> is null.
        /// </exception>
        /// <exception cref="BusinessException">
        /// One of the pages is a duplicate.
        /// </exception>
        public void Load([NotNull] ILayoutPageService pageService)
        {
            if (pageService == null)
            {
                throw new ArgumentNullException(nameof(pageService));
            }

            var pages = pageService.GetPages(this);
            this.layoutPages.Clear();

            foreach (var page in pages)
            {
                page.AddToLayout(this, (short)(page.Order > 0 ? page.Order : this.layoutPages.Count + 1));

                if (this.layoutPages.Add(page) == false)
                {
                    throw new BusinessException(page, FieldsMessages.SortedItemAlreadyAddedToCollection);
                }
            }
        }

        /// <summary>
        /// Loads page sections for the current layout.
        /// </summary>
        /// <param name="pageSectionService">
        /// The page section service.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="pageSectionService"/> is null.
        /// </exception>
        public void Load([NotNull] ILayoutPageSectionService pageSectionService)
        {
            if (pageSectionService == null)
            {
                throw new ArgumentNullException(nameof(pageSectionService));
            }

            var pageSections = pageSectionService.GetSections(this);
            this.layoutPages.Clear();

            foreach (var pageGroup in pageSections.GroupBy(section => section.LayoutPage))
            {
                var page = pageGroup.Key;
                page.AddToLayout(this, (short)(page.Order > 0 ? page.Order : this.layoutPages.Count + 1));

                if (this.layoutPages.Add(page) == false)
                {
                    throw new BusinessException(page, FieldsMessages.SortedItemAlreadyAddedToCollection);
                }

                page.Load(pageSectionService);
            }
        }
    }
}