// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LinkedSelection.cs" company="Startitecture">
//   Copyright (c) Startitecture. All rights reserved.
// </copyright>
// <summary>
//   Contains a reference to a linked selection with the link type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.Model
{
    using System;

    using JetBrains.Annotations;

    /// <summary>
    /// Contains a reference to a linked selection with the link type.
    /// </summary>
    public class LinkedSelection
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LinkedSelection"/> class.
        /// </summary>
        /// <param name="selection">
        /// The parent selection.
        /// </param>
        /// <param name="linkType">
        /// The link type.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="selection"/> is null.
        /// </exception>
        public LinkedSelection([NotNull] IEntitySet selection, SelectionLinkType linkType)
        {
            this.Selection = selection ?? throw new ArgumentNullException(nameof(selection));
            this.LinkType = linkType;
        }

        #region Public Properties

        /// <summary>
        /// Gets the link type.
        /// </summary>
        public SelectionLinkType LinkType { get; }

        /// <summary>
        /// Gets the linked selection.
        /// </summary>
        public IEntitySet Selection { get; }

        #endregion

        /// <inheritdoc />
        public override string ToString()
        {
            return $"{this.LinkType} {this.Selection}";
        }
    }
}