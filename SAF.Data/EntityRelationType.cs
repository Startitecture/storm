// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EntityRelationType.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// <summary>
//   An enumeration of the relation types between two entities.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.Data
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