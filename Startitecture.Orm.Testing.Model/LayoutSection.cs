// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LayoutSection.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// <summary>
//   The layout section.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.Testing.Model
{
    using System;
    using System.Collections.Generic;

    using JetBrains.Annotations;

    using Startitecture.Core;

    /// <summary>
    /// The layout section.
    /// </summary>
    public class LayoutSection : IEquatable<LayoutSection>
    {
        /// <summary>
        /// The comparison properties.
        /// </summary>
        private static readonly Func<LayoutSection, object>[] ComparisonProperties =
            {
                item => item.Name,
                item => item.Instructions,
                item => item.ShowHeader,
                item => item.CssStyle
            };

        /// <summary>
        /// The field placements.
        /// </summary>
        private readonly SortedSet<FieldPlacement> fieldPlacements = new SortedSet<FieldPlacement>();

        /// <summary>
        /// Initializes a new instance of the <see cref="LayoutSection"/> class.
        /// </summary>
        public LayoutSection()
            : this(Guid.NewGuid())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LayoutSection" /> class.
        /// </summary>
        /// <param name="instanceIdentifier">
        /// The instance GUID for the layout section.
        /// </param>
        public LayoutSection(Guid instanceIdentifier)
        {
            this.InstanceIdentifier = instanceIdentifier;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LayoutSection"/> class.
        /// </summary>
        /// <param name="instanceIdentifier">
        /// The instance GUID for the layout section.
        /// </param>
        /// <param name="layoutSectionId">
        /// The layout section ID.
        /// </param>
        public LayoutSection(Guid instanceIdentifier, int? layoutSectionId)
        {
            this.LayoutSectionId = layoutSectionId;
            this.InstanceIdentifier = instanceIdentifier;
        }

        /// <summary>
        /// Gets the layout section ID.
        /// </summary>
        public int? LayoutSectionId { get; private set; }

        /// <summary>
        /// Gets the instance GUID.
        /// </summary>
        public Guid InstanceIdentifier { get; private set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the instructions.
        /// </summary>
        public string Instructions { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to show the section header.
        /// </summary>
        public bool ShowHeader { get; set; }

        /// <summary>
        /// Gets or sets the CSS Style for the section.
        /// </summary>
        public string CssStyle { get; set; }

        /// <summary>
        /// Gets the field placements in this section.
        /// </summary>
        public IEnumerable<FieldPlacement> FieldPlacements
        {
            get
            {
                return this.fieldPlacements;
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
        public static bool operator ==(LayoutSection valueA, LayoutSection valueB)
        {
            return EqualityComparer<LayoutSection>.Default.Equals(valueA, valueB);
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
        public static bool operator !=(LayoutSection valueA, LayoutSection valueB)
        {
            return !(valueA == valueB);
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
        public bool Equals(LayoutSection other)
        {
            return Evaluate.Equals(this, other, ComparisonProperties);
        }

        #endregion

        /// <summary>
        /// Loads the field placements into the current section.
        /// </summary>
        /// <param name="placementService">
        /// The placement Service.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="placementService"/> is null.
        /// </exception>
        public void Load([NotNull] IFieldPlacementService placementService)
        {
            if (placementService == null)
            {
                throw new ArgumentNullException(nameof(placementService));
            }

            var placements = placementService.GetPlacements(this);
            this.fieldPlacements.Clear();

            foreach (var fieldPlacement in placements)
            {
                fieldPlacement.AddToLayoutSection(this, (short)(fieldPlacement.Order > 0 ? fieldPlacement.Order : this.fieldPlacements.Count + 1));

                if (this.fieldPlacements.Add(fieldPlacement) == false)
                {
                    throw new BusinessException(fieldPlacement, FieldsMessages.ItemAlreadyAdded);
                }
            }
        }

        /// <summary>
        /// Adds a field placement to the current section.
        /// </summary>
        /// <param name="field">
        /// The field to place.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="field"/> is null.
        /// </exception>
        /// <returns>
        /// The newly added <see cref="FieldPlacement"/>.
        /// </returns>
        public FieldPlacement AddPlacement([NotNull] UnifiedField field)
        {
            if (field == null)
            {
                throw new ArgumentNullException(nameof(field));
            }

            // Add the placement at the end. If there are no field placements, default to 1
            var order = (short)(this.fieldPlacements.Max?.Order + 1 ?? 1);
            var fieldPlacement = new FieldPlacement(field);
            fieldPlacement.AddToLayoutSection(this, order);

            if (this.fieldPlacements.Add(fieldPlacement) == false)
            {
                throw new BusinessException(fieldPlacement, FieldsMessages.ItemAlreadyAdded);
            }

            return fieldPlacement;
        }

        /// <summary>
        /// Removes a field placement from the current section.
        /// </summary>
        /// <param name="fieldPlacement">
        /// The field placement to remove.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="fieldPlacement"/> is null.
        /// </exception>
        /// <returns>
        /// <c>true</c> if <paramref name="fieldPlacement"/> was removed; otherwise, <c>false</c>.
        /// </returns>
        public bool RemovePlacement([NotNull] FieldPlacement fieldPlacement)
        {
            if (fieldPlacement == null)
            {
                throw new ArgumentNullException(nameof(fieldPlacement));
            }

            var removed = this.fieldPlacements.Remove(fieldPlacement);

            if (removed)
            {
                var sorter = new OrderedElementSearcher<FieldPlacement>(this.fieldPlacements);

                foreach (var placement in this.fieldPlacements)
                {
                    placement.SetOrder(sorter);
                }
            }

            return removed;
        }
    }
}