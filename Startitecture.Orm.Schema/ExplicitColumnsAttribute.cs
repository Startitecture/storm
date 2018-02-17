// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExplicitColumnsAttribute.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// <summary>
//   Classes marked with the Explicit attribute require all column properties to be marked with the Column attribute.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.Schema
{
    using System;

    /// <summary>
    /// Declares classes that require all column properties to be marked with the <see cref="ColumnAttribute"/> attribute.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class ExplicitColumnsAttribute : Attribute
    {
    }
}