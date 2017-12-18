namespace SAF.UserInterface
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Windows.Threading;

    using SAF.Data;
    using SAF.Data.Persistence;

    /// <summary>
    /// Updates the Federal Exclusion Compliance database with the specified data folder.
    /// </summary>
    /// <typeparam name="TSource">The type of <see cref="IFolderDataSource"/> contained in the data folder.</typeparam>
    public abstract class UpdateFromFolderCommand<TSource> : UpdateCommand<OpenFolderDirective, TSource>
        where TSource : IFolderDataSource
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateFromFolderCommand&lt;TSource&gt;"/> class.
        /// </summary>
        /// <param name="name">The name of the command.</param>
        /// <param name="updater">The entity updater that will execute the command.</param>
        /// <param name="commandDispatcher">The dispatcher that will relay command events back to the caller.</param>
        protected UpdateFromFolderCommand(string name, IEntityUpdater<TSource> updater, Dispatcher commandDispatcher)
            : base(name, updater, commandDispatcher)
        {
        }

        /// <summary>
        /// Generates a data source given the specified file name.
        /// </summary>
        /// <param name="directive">An open folder directive to retrieve the paths for the data source.</param>
        /// <returns>A data source based on the folder.</returns>
        protected override sealed TSource GenerateDataSource(OpenFolderDirective directive)
        {
            if (directive == null)
            {
                throw new ArgumentNullException("directive");
            }

            string path = directive.PathDialog.GetPaths(directive).FirstOrDefault();

            if (String.IsNullOrEmpty(path))
            {
                return default(TSource);
            }
            else
            {
                return this.GenerateDataSource(new DirectoryInfo(path));
            }
        }

        /// <summary>
        /// Generates a data source given the specified file name.
        /// </summary>
        /// <param name="folder">The folder to use to generate the data source.</param>
        /// <returns>A data source based on the folder.</returns>
        protected abstract TSource GenerateDataSource(DirectoryInfo folder);
    }
}
