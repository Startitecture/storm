// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ITrackChanges.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// <summary>
//   The TrackChanges interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.Data.Providers.Tests.FieldsModel
{
    using System;

    using JetBrains.Annotations;

    /// <summary>
    /// The TrackChanges interface.
    /// </summary>
    public interface ITrackChanges
    {
        /// <summary>
        /// Gets the last modified by.
        /// </summary>
        Person LastModifiedBy { get; }

        /// <summary>
        /// Gets the last modified time.
        /// </summary>
        DateTimeOffset LastModifiedTime { get; }

        /// <summary>
        /// Updates change tracking for the current object.
        /// </summary>
        /// <param name="editor">
        /// The editor of the object.
        /// </param>
        void UpdateChangeTracking([NotNull] Person editor);
    }
}