// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RelationAttribute.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.Schema
{
    using System;

    /// <summary>
    /// Declares that this property is a relation to the current entity.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class RelationAttribute : Attribute
    {
    }
}