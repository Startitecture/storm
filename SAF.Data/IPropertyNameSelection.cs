// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IPropertyNameSelection.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// <summary>
//   Provides an interface to classes that contain a collection of property names.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.Data
{
    using System.Collections.ObjectModel;

    /// <summary>
    /// Provides an interface to classes that contain a collection of property names.
    /// </summary>
    public interface IPropertyNameSelection
    {
        /// <summary>
        /// Gets the property names stored in the collection.
        /// </summary>
        Collection<string> PropertiesToInclude { get; }
    }
}