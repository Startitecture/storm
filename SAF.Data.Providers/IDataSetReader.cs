namespace SAF.Data.Providers
{
    using System.Collections.Generic;
    using System.Data;

    /// <summary>
    /// Provides an interface to a data set reader.
    /// </summary>
    /// <typeparam name="TDataSet">The type of data set to read.</typeparam>
    /// <typeparam name="TItem">The type of item contained in the dataset.</typeparam>
    public interface IDataSetReader<TDataSet, TItem>
        where TDataSet : DataSet
    {
        /// <summary>
        /// Reads items from the data set.
        /// </summary>
        /// <param name="dataSet">The data set to read items from.</param>
        /// <returns>An enumerable of items read from the data set.</returns>
        IEnumerable<TItem> Read(TDataSet dataSet);
    }
}
