namespace SAF.LocalMachine
{
    using System;
    using System.Diagnostics;
    using System.IO;

    using SAF.Data.Persistence;

    /// <summary>
    /// Contains instructions for applying a retention policy to a specific file.
    /// </summary>
    public class FileRetentionDirective : ItemRetentionDirective<FileInfo>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FileRetentionDirective"/> class with the specified retention
        /// policy and target file.
        /// </summary>
        /// <param name="policy">The policy to apply to the file.</param>
        /// <param name="target">The file to apply the policy to.</param>
        public FileRetentionDirective(RetentionPolicy policy, FileInfo target)
            : base(policy, target)
        {
        }

        /// <summary>
        /// Applies a <see cref="RetentionPolicy"/> using a <see cref="ItemRetentionDirective&lt;FileInfo&gt;"/>.
        /// </summary>
        /// <param name="directive">The <see cref="ItemRetentionDirective&lt;FileInfo&gt;"/> containing the 
        /// <see cref="RetentionPolicy"/> /// to apply.</param>
        /// <returns>A <see cref="ItemRetentionResult&lt;FileInfo&gt;"/> containing the result of the action.</returns>
        /// <exception cref="PlatformNotSupportedException">
        /// The current operating system is not Windows NT or later.
        /// </exception>
        /// <exception cref="System.Security.SecurityException">
        /// The caller does not have the required permission.
        /// </exception>
        /// <exception cref="UnauthorizedAccessException">
        /// The path is a directory.
        /// </exception>
        /// <exception cref="IOException">
        /// The data could not be initialized, or the target file is open or memory-mapped on a computer running 
        /// Microsoft Windows NT, or there is an open handle on the file, and the operating system is Windows XP or 
        /// earlier. This open handle can result from enumerating directories and files.
        /// </exception>
        public static PolicyResult ApplyPolicyToItem(ItemRetentionDirective<FileInfo> directive)
        {
            if (directive == null)
            {
                throw new ArgumentNullException("directive");
            }

            PolicyResult retentionResult = PolicyResult.PolicySuccess;
            DateTime retentionCutoff =
                RetentionPolicy.GetRetentionCutoff(
                    DateTime.Now, directive.Policy.Length, directive.Policy.Granularity);

                if (directive.Target.LastWriteTime < retentionCutoff)
                {
                    Trace.TraceInformation(
                        "Deleting '{0}', last modified '{1}'.", directive.Target, directive.Target.LastWriteTime);

                    directive.Target.Delete();
                }
                else
                {
                    retentionResult = PolicyResult.PolicyDoesNotApply;
                }

            return retentionResult;
        }
    }
}
