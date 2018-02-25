// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LayoutPage.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// <summary>
//   The layout page.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.Testing.Model
{
    using System;
    using System.Collections.Generic;

    using JetBrains.Annotations;

    using Startitecture.Core;

    /// <summary>
    /// The layout page.
    /// </summary>
    public class LayoutPage : IEquatable<LayoutPage>, IComparable<LayoutPage>, IOrderedElement
    {
        /// <summary>
        /// The comparison properties.
        /// </summary>
        private static readonly Func<LayoutPage, object>[] ComparisonProperties =
            {
                item => item.FormLayout,
                item => item.Order,
                item => item.Name,
                item => item.Instructions,
                item => item.ShowHeader
            };

        /// <summary>
        /// The layout tabs.
        /// </summary>
        private readonly SortedSet<LayoutPageSection> layoutPageSections =
            new SortedSet<LayoutPageSection>(Singleton<OrderedElementComparer>.Instance);

        /// <summary>
        /// Initializes a new instance of the <see cref="LayoutPage" /> class.
        /// </summary>
        public LayoutPage()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LayoutPage"/> class.
        /// </summary>
        /// <param name="formLayout">
        /// The form layout.
        /// </param>
        /// <param name="layoutPageId">
        /// The layout page ID.
        /// </param>
        public LayoutPage([NotNull] FormLayout formLayout, int? layoutPageId)
        {
            if (formLayout == null)
            {
                throw new ArgumentNullException(nameof(formLayout));
            }

            this.FormLayout = formLayout;
            this.LayoutPageId = layoutPageId;
        }

        /// <summary>
        /// Gets the layout page ID.
        /// </summary>
        public int? LayoutPageId { get; private set; }

        /// <summary>
        /// Gets the form layout.
        /// </summary>
        public FormLayout FormLayout { get; private set; }

        /// <summary>
        /// Gets the form layout ID.
        /// </summary>
        public int? FormLayoutId
        {
            get
            {
                return this.FormLayout?.FormLayoutId;
            }
        }

        /// <summary>
        /// Gets the layout tabs in the current layout page.
        /// </summary>
        public IEnumerable<LayoutPageSection> LayoutPageSections
        {
            get
            {
                return this.layoutPageSections;
            }
        }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the instructions.
        /// </summary>
        public string Instructions { get; set; }

        /// <summary>
        /// Gets the order of the page in the layout.
        /// </summary>
        public short Order { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating whether to show the name as a header.
        /// </summary>
        public bool ShowHeader { get; set; }

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
        public static bool operator ==(LayoutPage valueA, LayoutPage valueB)
        {
            return EqualityComparer<LayoutPage>.Default.Equals(valueA, valueB);
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
        public static bool operator !=(LayoutPage valueA, LayoutPage valueB)
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
        public static bool operator <(LayoutPage valueA, LayoutPage valueB)
        {
            return Comparer<LayoutPage>.Default.Compare(valueA, valueB) < 0;
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
        public static bool operator >(LayoutPage valueA, LayoutPage valueB)
        {
            return Comparer<LayoutPage>.Default.Compare(valueA, valueB) > 0;
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
        public int CompareTo(LayoutPage other)
        {
            return Evaluate.Compare(this, other, ComparisonProperties);
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
        public bool Equals(LayoutPage other)
        {
            return Evaluate.Equals(this, other, ComparisonProperties);
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
            return $"{this.GetType().Name} {this.Name}";
        }

        #endregion

        /// <summary>
        /// Sets order of the current element.
        /// </summary>
        /// <param name="searcher">
        /// The ordered element searcher.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="searcher"/> is null.
        /// </exception>
        /// <exception cref="BusinessException">
        /// The current element is not in the searcher's list.
        /// </exception>
        public void SetOrder(IOrderedElementSearcher searcher)
        {
            if (searcher == null)
            {
                throw new ArgumentNullException(nameof(searcher));
            }

            var order = searcher.GetOrder(this);

            if (order < 0)
            {
                throw new BusinessException(this, FieldsMessages.OrderedElementNotInList);
            }

            this.Order = order;
        }

        /// <summary>
        /// Adds a layout section to the current page.
        /// </summary>
        /// <param name="section">
        /// The layout section to add.
        /// </param>
        /// <returns>
        /// The newly created <see cref="LayoutPageSection"/>.
        /// </returns>
        public LayoutPageSection AddSection([NotNull] LayoutSection section)
        {
            if (section == null)
            {
                throw new ArgumentNullException(nameof(section));
            }

            var order = (short)(this.layoutPageSections.Max?.Order + 1 ?? 1);
            var pageSection = new LayoutPageSection(section);
            pageSection.AddToPage(this, order);

            if (this.layoutPageSections.Add(pageSection) == false)
            {
                throw new BusinessException(section, FieldsMessages.SortedItemAlreadyAddedToCollection);
            }

            return pageSection;
        }

        /// <summary>
        /// Removes a layout tab from the current page.
        /// </summary>
        /// <param name="section">
        /// The layout tab to remove.
        /// </param>
        /// <returns>
        /// <c>true</c> if <paramref name="section"/> was removed; otherwise, <c>false</c>.
        /// </returns>
        public bool RemoveSection([NotNull] LayoutPageSection section)
        {
            if (section == null)
            {
                throw new ArgumentNullException(nameof(section));
            }

            var removed = this.layoutPageSections.Remove(section);

            if (removed)
            {
                var sorter = new OrderedElementSearcher<LayoutPageSection>(this.layoutPageSections);

                foreach (var pageSection in this.layoutPageSections)
                {
                    pageSection.SetOrder(sorter);
                }
            }

            return removed;
        }

        /// <summary>
        /// Adds layout page sections to the current layout page.
        /// </summary>
        /// <param name="pageSectionService">
        /// The page section service.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="pageSectionService"/> is null.
        /// </exception>
        /// <exception cref="BusinessException">
        /// One of the page sections has already been added to the underlying sorted set.
        /// </exception>
        public void Load([NotNull] ILayoutPageSectionService pageSectionService)
        {
            if (pageSectionService == null)
            {
                throw new ArgumentNullException(nameof(pageSectionService));
            }

            var sections = pageSectionService.GetSections(this);

            this.layoutPageSections.Clear();

            foreach (var section in sections)
            {
                section.AddToPage(this, (short)(section.Order > 0 ? section.Order : this.layoutPageSections.Count + 1));

                if (this.layoutPageSections.Add(section) == false)
                {
                    throw new BusinessException(section, FieldsMessages.SortedItemAlreadyAddedToCollection);
                }
            }
        }

        /// <summary>
        /// Adds the current page to a form layout.
        /// </summary>
        /// <param name="layout">
        /// The layout to add the current page to.
        /// </param>
        /// <param name="order">
        /// The order of the page.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="layout"/> is null.
        /// </exception>
        internal void AddToLayout([NotNull] FormLayout layout, short order)
        {
            if (layout == null)
            {
                throw new ArgumentNullException(nameof(layout));
            }

            this.FormLayout = layout;
            this.Order = order;
        }
    }
}