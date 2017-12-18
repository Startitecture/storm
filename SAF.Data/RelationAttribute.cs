// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RelationAttribute.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.Data
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