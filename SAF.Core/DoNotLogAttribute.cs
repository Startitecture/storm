// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DoNotLogAttribute.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// <summary>
//   An attribute class to indicate that a specified property should not be logged into a persistent store.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.Core
{
    using System;

    /// <summary>
    /// An attribute class to indicate that a specified property should not be logged into a persistent store.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class DoNotLogAttribute : Attribute
    {
    }
}
