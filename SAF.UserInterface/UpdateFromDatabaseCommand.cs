// -----------------------------------------------------------------------
// <copyright file="UpdateFromDatabaseCommand.cs" company="Microsoft">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace SAF.UserInterface
{
    using System;
    using System.Windows.Threading;

    using SAF.Data;
    using SAF.Data.Persistence;

    /// <summary>
    /// Executes a data integration with a database as the data source.
    /// </summary>
    /// <typeparam name="TSource">The type of data source providing the update.</typeparam>
    public abstract class UpdateFromDatabaseCommand<TSource> : UpdateCommand<OpenDatabaseDirective, TSource>
        where TSource : IDataSource
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateFromDatabaseCommand&lt;TSource&gt;"/> class.
        /// </summary>
        /// <param name="name">The name of the command.</param>
        /// <param name="updater">The entity updater that will execute the command.</param>
        /// <param name="commandDispatcher">The dispatcher that will relay command events back to the caller.</param>
        protected UpdateFromDatabaseCommand(string name, IEntityUpdater<TSource> updater, Dispatcher commandDispatcher)
            : base(name, updater, commandDispatcher)
        {
        }

        /// <summary>
        /// Generates a data source using the specified directive.
        /// </summary>
        /// <param name="directive">A directive that provides connection information to the command.</param>
        /// <returns>A data source created from the specified connection infomration.</returns>
        protected override sealed TSource GenerateDataSource(OpenDatabaseDirective directive)
        {
            if (directive == null)
            {
                throw new ArgumentNullException("directive");
            }

            return this.GenerateDataSource(directive.ConnectionString);
        }

        /// <summary>
        /// Generates a data source using the specified connection string and connection timeout.
        /// </summary>
        /// <param name="connectionString">The connection string to the database.</param>
        /// <returns>A data source created from the specified connection infomration.</returns>
        protected abstract TSource GenerateDataSource(string connectionString);
    }
}
