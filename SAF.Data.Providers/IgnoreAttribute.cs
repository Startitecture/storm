// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IgnoreAttribute.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// <summary>
//   Declares properties that shouldn't be included in selection statements.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.Data.Providers
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