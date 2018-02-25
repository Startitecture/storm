// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ITrackChanges.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// <summary>
//   The TrackChanges interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.Testing.Model
{
    using System;

    using JetBrains.Annotations;

    using Startitecture.Orm.Testing.Model.PM;

    /// <summary>
    /// The TrackChanges interface.
    /// </summary>
    public interface ITrackChanges
    {
        /// <summary>
        /// Gets the last modified by.
        /// </summary>
        PM.Person LastModifiedBy { get; }

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
        void UpdateChangeTracking([NotNull] PM.Person editor);
    }
}