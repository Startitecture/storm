// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LinkedSelection.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// <summary>
//   Contains a reference to a linked selection with the link type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.Model
{
    /// <summary>
    /// Contains a reference to a linked selection with the link type.
    /// </summary>
    /// <typeparam name="TItem">
    /// The type of item in the selection.
    /// </typeparam>
    public class LinkedSelection<TItem>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LinkedSelection{TItem}"/> class.
        /// </summary>
        /// <param name="selection">
        /// The parent selection.
        /// </param>
        /// <param name="linkType">
        /// The link type.
        /// </param>
        public LinkedSelection(ItemSelection<TItem> selection, SelectionLinkType linkType)
        {
            this.Selection = selection;
            this.LinkType = linkType;
        }

        #region Public Properties

        /// <summary>
        /// Gets the link type.
        /// </summary>
        public SelectionLinkType LinkType { get; private set; }

        /// <summary>
        /// Gets the linked selection.
        /// </summary>
        public ItemSelection<TItem> Selection { get; private set; }

        #endregion

        /// <inheritdoc />
        public override string ToString()
        {
            return $"{this.LinkType} {this.Selection}";
        }
    }
}