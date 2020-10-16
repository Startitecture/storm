// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IgnoreAttribute.cs" company="Startitecture">
//   Copyright (c) Startitecture. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
// <summary>
//   Declares properties that shouldn't be included in selection statements.
// </summary>

namespace Startitecture.Orm.Schema
{
    using System;

    /// <summary>
    /// Declares properties that shouldn't be included in selection statements.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class IgnoreAttribute : Attribute
    {
    }
}