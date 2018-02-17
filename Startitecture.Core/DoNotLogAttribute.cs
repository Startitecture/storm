// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DoNotLogAttribute.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// <summary>
//   An attribute class to indicate that a specified property should not be logged into a persistent store.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Core
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
