// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SelectionLinkType.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// <summary>
//   An enumeration of the link types between selection statements.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.Data
{
    /// <summary>
    /// An enumeration of the link types between selection statements.
    /// </summary>
    public enum SelectionLinkType
    {
        /// <summary>
        /// The statements produce a union.
        /// </summary>
        Union = 0, 

        /// <summary>
        /// The statements produce an intersection.
        /// </summary>
        Intersection = 1, 

        /// <summary>
        /// The statements produce an exception.
        /// </summary>
        Exception = 2
    }
}