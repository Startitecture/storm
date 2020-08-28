// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EntityAttributeTypes.cs" company="Startitecture">
//   Copyright (c) Startitecture. All rights reserved.
// </copyright>
// <summary>
//   Contains the types of an entity attribute.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.Model
{
    using System;

    /// <summary>
    /// Contains the types of an entity attribute.
    /// </summary>
    [Flags]
    public enum EntityAttributeTypes
    {
        /// <summary>
        /// The attribute has no declared type.
        /// </summary>
        None = 0,

        /// <summary>
        /// The attribute is a direct attribute of the data item.
        /// </summary>
        DirectAttribute = 1,

        /// <summary>
        /// The attribute is part of the data item's primary key.
        /// </summary>
        PrimaryKey = 2,

        /// <summary>
        /// The attribute is a row identity.
        /// </summary>
        IdentityColumn = 4,

        /// <summary>
        /// The attribute is part of the data item's unique key.
        /// </summary>
        UniqueKey = 8,

        /// <summary>
        /// The attribute is a direct attribute from a relation of the entity.
        /// </summary>
        RelatedAttribute = 16,

        /// <summary>
        /// The attribute is a related auto number key.
        /// </summary>
        RelatedAutoNumberKey = RelatedAttribute | PrimaryKey | IdentityColumn,

        /// <summary>
        /// The attribute is a relation of the entity.
        /// </summary>
        Relation = 32,

        /// <summary>
        /// The attribute is a related attribute explicitly declared on the entity.
        /// </summary>
        ExplicitRelatedAttribute = 64,

        /// <summary>
        /// The attribute is computed by the database.
        /// </summary>
        Computed = 128
    }
}