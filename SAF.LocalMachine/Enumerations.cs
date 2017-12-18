namespace SAF.LocalMachine
{
    /// <summary>
    /// Specifies the state of an archive action.
    /// </summary>
    public enum ArchiveResult
    {
        /// <summary>
        /// The item was archived successfully.
        /// </summary>
        Success = 0,

        /// <summary>
        /// The item failed to be archived.
        /// </summary>
        Failed = 1,

        /// <summary>
        /// The archive container does not exist.
        /// </summary>
        TargetContainerDoesNotExist = 2,

        /// <summary>
        /// The archive container could not be accessed.
        /// </summary>
        AccessDenied = 3
    }
}
