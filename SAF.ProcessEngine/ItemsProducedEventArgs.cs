// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ItemsProducedEventArgs.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// <summary>
//   Contains event data related to an items producing event.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.ProcessEngine
{
    using System;

    /// <summary>
    /// Contains event data related to an items producing event.
    /// </summary>
    public sealed class ItemsProducedEventArgs : EventArgs
    {
        /// <summary>
        /// Represents an event with no data.
        /// </summary>
        public static readonly new ItemsProducedEventArgs Empty = new ItemsProducedEventArgs();

        /// <summary>
        /// Prevents a default instance of the <see cref="ItemsProducedEventArgs"/> class from being created.
        /// </summary>
        private ItemsProducedEventArgs()
        {
        }
    }
}
