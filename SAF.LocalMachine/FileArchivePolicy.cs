namespace SAF.LocalMachine
{
    using System.IO;

    using SAF.Data.Persistence;

    /// <summary>
    /// Contains instructions for archiving files.
    /// </summary>
    public class FileArchivePolicy : ArchivePolicy<DirectoryInfo>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FileArchivePolicy"/> class.
        /// </summary>
        /// <param name="nameFormat">The name format to apply to the archived files.</param>
        /// <param name="archiveContainer">The directory to place the files in.</param>
        public FileArchivePolicy(string nameFormat, DirectoryInfo archiveContainer)
            : base(nameFormat, archiveContainer)
        {
        }
    }
}
