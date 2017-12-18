namespace SAF.LocalMachine
{
    using System;
    using System.IO;

    using SAF.ProcessEngine;

    /// <summary>
    /// Contains the results of a file archival action.
    /// </summary>
    public class FileArchiveResult : TaskResult<FileArchiveDirective, ArchiveResult>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FileArchiveResult"/> class with the specified directive,
        /// target, result and error (if any).
        /// </summary>
        /// <param name="directive">The directive associated with this result.</param>
        /// <param name="source">The source file that was archived.</param>
        /// <param name="target">The archived target file.</param>
        /// <param name="result">The result of the action.</param>
        /// <param name="error">The exception, if any, associated with this result.</param>
        public FileArchiveResult(
            FileArchiveDirective directive, FileInfo source, FileInfo target, ArchiveResult result, Exception error)
            : base(directive, result, error)
        {
            this.Source = source;
            this.Target = target;
        }

        /// <summary>
        /// Gets the original source file.
        /// </summary>
        public FileInfo Source { get; private set; }

        /// <summary>
        /// Gets the updated file system object.
        /// </summary>
        public FileInfo Target { get; private set; }
    }
}
