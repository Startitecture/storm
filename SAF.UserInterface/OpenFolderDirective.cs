namespace SAF.UserInterface
{
    /// <summary>
    /// Provides instructions for updating a data source from a path-based resource.
    /// </summary>
    public class OpenFolderDirective
    {
        /// <summary>
        /// Gets or sets a description of the data source.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the path dialog provider.
        /// </summary>
        public IPathDialog<OpenFolderDirective> PathDialog { get; set; }
    }
}
