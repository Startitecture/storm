// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UnifiedFieldSource.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.Data.Providers.Tests
{
    /// <summary>
    /// The unified field source.
    /// </summary>
    public class UnifiedFieldSource
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UnifiedFieldSource"/> class.
        /// </summary>
        /// <param name="sourceType">
        /// The source type.
        /// </param>
        /// <param name="sourceId">
        /// The source id.
        /// </param>
        public UnifiedFieldSource(UnifiedFieldSourceType sourceType, int sourceId)
        {
            this.SourceType = sourceType;
            this.SourceId = sourceId;
        }

        /// <summary>
        /// Gets the source id.
        /// </summary>
        public int SourceId { get; private set; }

        /// <summary>
        /// Gets the source type.
        /// </summary>
        public UnifiedFieldSourceType SourceType { get; private set; }
    }
}