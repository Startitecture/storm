// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EntityRelationType.cs" company="Startitecture">
//   Copyright (c) Startitecture. All rights reserved.
// </copyright>
// <summary>
//   An enumeration of the relation types between two entities.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.Model
{
    /// <summary>
    /// An enumeration of the relation types between two entities.
    /// </summary>
    /// <remarks>
    /// A RIGHT JOIN option is not offered because in set-based logic, A RIGHT JOIN B is equivalent to B LEFT JOIN A.
    /// </remarks>
    public enum EntityRelationType
    {
        /// <summary>
        /// The product of the JOIN should include only rows in which both entities have matching results.
        /// </summary>
        InnerJoin = 0, 

        /// <summary>
        /// The product of the JOIN must always include the left (source) entity; the relation entity columns will return NULL if there is no match.
        /// </summary>
        LeftJoin = 1
    }
}