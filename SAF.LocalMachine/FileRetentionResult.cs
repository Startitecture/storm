namespace SAF.LocalMachine
{
    using System;
    using System.IO;

    using SAF.Data.Persistence;

    /// <summary>
    /// Contains the result of a file retention policy action.
    /// </summary>
    public class FileRetentionResult : ItemRetentionResult<FileInfo>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FileRetentionResult"/> class with the specified 
        /// retention directive, result and error (if any).
        /// </summary>
        /// <param name="directive">The retention directive associated with this result.</param>
        /// <param name="result">The result of the action.</param>
        /// <param name="effectiveDate">The effective date and time of the action.</param>
        /// <param name="error">The error, if any, assoicated with the retention action.</param>
        public FileRetentionResult(
            ItemRetentionDirective<FileInfo> directive, 
            PolicyResult result, 
            DateTimeOffset effectiveDate,
            Exception error)
            : base(directive, result, effectiveDate, error)
        {
        }
    }
}
