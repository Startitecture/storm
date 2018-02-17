// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EntityAttributeTypes.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// <summary>
//   Contains the possible types of a data item property.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.Model
{
    using System;

    /// <summary>
    /// Contains the possible types of a data item property.
    /// </summary>
    [Flags]
    public enum EntityAttributeTypes
    {
        /// <summary>
        /// The attribute is a direct attribute of the data item.
        /// </summary>
        DirectAttribute = 1,

        /// <summary>
        /// The attribute is part of the data item's primary key.
        /// </summary>
        PrimaryKey = 2,

        /// <summary>
        /// The attribute is a direct primary key.
        /// </summary>
        DirectPrimaryKey = DirectAttribute | PrimaryKey,

        /// <summary>
        /// The attribute is an auto number primary key.
        /// </summary>
        IdentityColumn = 4,

        /// <summary>
        /// The attribute is a direct auto number key.
        /// </summary>
        DirectAutoNumberKey = DirectAttribute | PrimaryKey | IdentityColumn,

        /// <summary>
        /// The attribute is part of the data item's unique key.
        /// </summary>
        UniqueKey = 8,

        /// <summary>
        /// The attribute is a direct unique key.
        /// </summary>
        DirectUniqueKey = DirectAttribute | UniqueKey,

        /// <summary>
        /// The attribute is a direct attribute from a relation of the entity.
        /// </summary>
        RelatedAttribute = 16,

        /// <summary>
        /// The attribute is a related primary key.
        /// </summary>
        RelatedPrimaryKey = RelatedAttribute | PrimaryKey,

        /// <summary>
        /// The attribute is a related auto number key.
        /// </summary>
        RelatedAutoNumberKey = RelatedAttribute | PrimaryKey | IdentityColumn,

        /// <summary>
        /// The attribute is a related unique key.
        /// </summary>
        RelatedUniqueKey = RelatedAttribute | UniqueKey,

        /// <summary>
        /// The attribute is being used for mapping but should not be included in the results.
        /// </summary>
        MappedAttribute = 32,

        /// <summary>
        /// The attribute is a relation of the entity.
        /// </summary>
        Relation = 64,

        /// <summary>
        /// The attribute is a related attribute explicitly declared on the entity.
        /// </summary>
        ExplicitRelatedAttribute = 128,
    }
}