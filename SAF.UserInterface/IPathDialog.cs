// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IPathDialog.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// <summary>
//   Provides an interface for dialogs that return paths as strings.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.UserInterface
{
    using System.Collections.Generic;

    /// <summary>
    /// Provides an interface for dialogs that return paths as strings.
    /// </summary>
    /// <typeparam name="TDirective">The type of directive that provides information for the path dialog.</typeparam>
    public interface IPathDialog<in TDirective>
    {
        /// <summary>
        /// Gets or sets the path to initiate the dialog at.
        /// </summary>
        string RootPath { get; set; }

        /// <summary>
        /// Gets the path as a string.
        /// </summary>
        /// <param name="directive">The directive containing information required by the path dialog.</param>
        /// <returns>The path as a string, or null if no string was returned.</returns>
        IEnumerable<string> GetPaths(TDirective directive);
    }
}
