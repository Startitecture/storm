// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EntityRelationType.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
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
    public enum EntityRelationType
    {
        /// <summary>
        /// The inner join.
        /// </summary>
        InnerJoin = 0, 

        /// <summary>
        /// The left join.
        /// </summary>
        LeftJoin = 1
    }
}