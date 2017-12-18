namespace SAF.Data.Persistence
{
    using System;

    using SAF.ProcessEngine;

    /// <summary>
    /// Provides an interface to a data extractor.
    /// </summary>
    public interface IDataExtractor : IProcessEngine
    {
        /// <summary>
        /// Occurs before the data source starts loading into the data proxy.
        /// </summary>
        event EventHandler<DataExtractionStartingEventArgs> DataExtractionStarting;
    }

    /// <summary>
    /// Provides an interface to a data extractor that extracts data from a specific type of data source.
    /// </summary>
    /// <typeparam name="TSource">The type of source that contains the data.</typeparam>
    public interface IDataExtractor<in TSource> : IDataExtractor
    {
        /// <summary>
        /// Starts the extraction of data from the data source.
        /// </summary>
        /// <param name="dataSource">The data source containing the items to extract.</param>
        void BeginExtraction(TSource dataSource);
    }
}
