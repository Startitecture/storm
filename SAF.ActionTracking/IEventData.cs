// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IEventData.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// <summary>
//   Provides an interface for classes that contain event data.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.ActionTracking
{
    using System;

    /// <summary>
    /// Provides an interface for classes that contain event data.
    /// </summary>
    public interface IEventData
    {
        /// <summary>
        /// Gets the sequential action event ID for the event.
        /// </summary>
        long ActionEventId { get; }

        /// <summary>
        /// Gets or sets the global identifier of the event.
        /// </summary>
        Guid GlobalIdentifier { get; set; }
    }
}