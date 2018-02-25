// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LayoutPageSection.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// <summary>
//   The layout page section.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.Testing.Model
{
    using System;
    using System.Collections.Generic;

    using JetBrains.Annotations;

    using Startitecture.Core;

    /// <summary>
    /// The layout page section.
    /// </summary>
    public class LayoutPageSection : IEquatable<LayoutPageSection>, IOrderedElement
    {
        /// <summary>
        /// The comparison properties.
        /// </summary>
        private static readonly Func<LayoutPageSection, object>[] ComparisonProperties =
            {
                item => item.LayoutPage,
                item => item.Order,
                item => item.LayoutSection
            };

        /// <summary>
        /// Initializes a new instance of the <see cref="LayoutPageSection"/> class.
        /// </summary>
        /// <param name="layoutSection">
        /// The layout section.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="layoutSection"/> is null.
        /// </exception>
        public LayoutPageSection([NotNull] LayoutSection layoutSection)
        {
            if (layoutSection == null)
            {
                throw new ArgumentNullException(nameof(layoutSection));
            }

            this.LayoutSection = layoutSection;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LayoutPageSection"/> class.
        /// </summary>
        /// <param name="layoutSection">
        /// The layout section.
        /// </param>
        /// <param name="layoutPage">
        /// The layout page.
        /// </param>
        /// <param name="layoutPageSectionId">
        /// The layout page section ID.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="layoutSection"/> or <paramref name="layoutPage"/> is null.
        /// </exception>
        public LayoutPageSection([NotNull] LayoutSection layoutSection, [NotNull] LayoutPage layoutPage, int? layoutPageSectionId)
            : this(layoutSection)
        {
            if (layoutPage == null)
            {
                throw new ArgumentNullException(nameof(layoutPage));
            }

            this.LayoutPage = layoutPage;
            this.LayoutPageSectionId = layoutPageSectionId;
        }

        /// <summary>
        /// Gets the layout page section ID.
        /// </summary>
        public int? LayoutPageSectionId { get; private set; }

        /// <summary>
        /// Gets the layout page.
        /// </summary>
        public LayoutPage LayoutPage { get; private set; }

        /// <summary>
        /// Gets the layout page ID.
        /// </summary>
        public int? LayoutPageId
        {
            get
            {
                return this.LayoutPage?.LayoutPageId;
            }
        }

        /// <summary>
        /// Gets the layout section.
        /// </summary>
        public LayoutSection LayoutSection { get; private set; }

        /// <summary>
        /// Gets the layout section ID.
        /// </summary>
        public int? LayoutSectionId
        {
            get
            {
                return this.LayoutSection?.LayoutSectionId;
            }
        }

        /// <summary>
        /// Gets the order of the section in the page.
        /// </summary>
        public short Order { get; private set; }

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
        public static bool operator ==(LayoutPageSection valueA, LayoutPageSection valueB)
        {
            return EqualityComparer<LayoutPageSection>.Default.Equals(valueA, valueB);
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
        public static bool operator !=(LayoutPageSection valueA, LayoutPageSection valueB)
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
            return $"{this.LayoutPage}:{this.Order}:{this.LayoutSection}";
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
        public bool Equals(LayoutPageSection other)
        {
            return Evaluate.Equals(this, other, ComparisonProperties);
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
        /// Adds the layout page section to the specified page.
        /// </summary>
        /// <param name="page">
        /// The page to add the page section to.
        /// </param>
        /// <param name="order">
        /// The order of the page.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="page"/> is null.
        /// </exception>
        internal void AddToPage([NotNull] LayoutPage page, short order)
        {
            if (page == null)
            {
                throw new ArgumentNullException(nameof(page));
            }

            this.LayoutPage = page;
            this.Order = order;
        }
    }
}