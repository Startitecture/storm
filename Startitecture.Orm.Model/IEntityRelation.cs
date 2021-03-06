﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IEntityRelation.cs" company="Startitecture">
//   Copyright (c) Startitecture. All rights reserved.
// </copyright>
// <summary>
//   Provides an interface for classes that describe a relation between two entities.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.Model
{
    using System.Linq.Expressions;

    /// <summary>
    /// Provides an interface for classes that describe a relation between two entities.
    /// </summary>
    public interface IEntityRelation
    {
        /// <summary>
        /// Gets the type of entity relation.
        /// </summary>
        EntityRelationType RelationType { get; }

        /// <summary>
        /// Gets the source expression.
        /// </summary>
        LambdaExpression SourceExpression { get; }

        /// <summary>
        /// Gets the source entity alias.
        /// </summary>
        string SourceEntityAlias { get; }

        /// <summary>
        /// Gets the relation expression.
        /// </summary>
        LambdaExpression RelationExpression { get; }

        /// <summary>
        /// Gets the relation entity alias.
        /// </summary>
        string RelationEntityAlias { get; }
    }
}