// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SelectionLinkType.cs" company="Startitecture">
//   Copyright (c) Startitecture. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
// <summary>
//   An enumeration of the link types between selection statements.
// </summary>

namespace Startitecture.Orm.Model
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