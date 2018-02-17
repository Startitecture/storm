// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RelationAttribute.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.Model
{
    using System;

    /// <summary>
    /// The relation attribute.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class RelationAttribute : Attribute
    {
    }
}