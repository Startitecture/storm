namespace SAF.UserInterface
{
    using System.Collections.Generic;

    /// <summary>
    /// Contains instructions for opening a file dialog.
    /// </summary>
    public class OpenFilesDirective
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OpenFilesDirective"/> class.
        /// </summary>
        /// <param name="extensions">The extensions to use to filter the files.</param>
        public OpenFilesDirective(params string[] extensions)
        {
            this.Extensions = new List<string>(extensions);
        }

        /// <summary>
        /// Gets or sets a description of the data source.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets the file extensions to use as filters.
        /// </summary>
        public IEnumerable<string> Extensions { get; private set; }

        /// <summary>
        /// Gets or sets the path dialog provider.
        /// </summary>
        public IPathDialog<OpenFilesDirective> PathDialog { get; set; }
    }
}
