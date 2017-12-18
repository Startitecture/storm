namespace SAF.UserInterface
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows.Threading;

    using SAF.Data;
    using SAF.Data.Persistence;

    /// <summary>
    /// Updates the Federal Exclusion Compliance database with the specified data file.
    /// </summary>
    /// <typeparam name="TSource">The type of <see cref="IFileDataSource"/> contained in the data file.</typeparam>
    public abstract class UpdateFromFileCommand<TSource> : UpdateCommand<OpenFilesDirective, TSource>
        where TSource : IFileDataSource
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateFromFileCommand&lt;TSource&gt;"/> class.
        /// </summary>
        /// <param name="name">The name of the command.</param>
        /// <param name="updater">The entity updater that will execute the command.</param>
        /// <param name="commandDispatcher">The dispatcher that will relay command events back to the caller.</param>
        protected UpdateFromFileCommand(string name, IEntityUpdater<TSource> updater, Dispatcher commandDispatcher)
            : base(name, updater, commandDispatcher)
        {
        }

        /// <summary>
        /// Generates a data source given the specified file name.
        /// </summary>
        /// <param name="directive">An open folder directive to retrieve the paths for the data source.</param>
        /// <returns>A data source based on the folder.</returns>
        protected override sealed TSource GenerateDataSource(OpenFilesDirective directive)
        {
            if (directive == null)
            {
                throw new ArgumentNullException("directive");
            }

            IEnumerable<string> paths = directive.PathDialog.GetPaths(directive);

            if (paths.Count() == 0)
            {
                return default(TSource);
            }
            else
            {
                return this.GenerateDataSource(paths);
            }
        }

        /// <summary>
        /// Generates a data source given the specified file name.
        /// </summary>
        /// <param name="filePaths">The file paths to use to generate the data source.</param>
        /// <returns>
        /// A data source based on the files, or the default value for the type if a path is not returned.
        /// </returns>
        protected abstract TSource GenerateDataSource(IEnumerable<string> filePaths);
    }
}
