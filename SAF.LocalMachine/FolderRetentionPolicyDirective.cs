namespace SAF.LocalMachine
{
    using System;
    using System.Collections.Generic;
    using System.IO;

    using SAF.Data.Persistence;

    /// <summary>
    /// Contains instructions for applying a retention policy to a file system directory.
    /// </summary>
    public class FolderRetentionPolicyDirective : RetentionPolicyDirective<DirectoryInfo>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FolderRetentionPolicyDirective"/> class.
        /// </summary>
        /// <param name="policy">The policy to apply.</param>
        /// <param name="target">The target to apply the policy to.</param>
        public FolderRetentionPolicyDirective(RetentionPolicy policy, DirectoryInfo target)
            : base(policy, target)
        {
        }

        /// <summary>
        /// Applies a <see cref="RetentionPolicyDirective&lt;DirectoryInfo&gt;"/> to a folder.
        /// </summary>
        /// <param name="directive">The <see cref="RetentionPolicyDirective&lt;DirectoryInfo&gt;"/> to apply.</param>
        /// <returns>The result of the policy application.</returns>
        /// <exception cref="DirectoryNotFoundException">
        /// The path is invalid, such as being on an unmapped drive.
        /// </exception>
        public static IEnumerable<FileRetentionResult> ApplyPolicyToContainer(
            RetentionPolicyDirective<DirectoryInfo> directive)
        {
            if (directive == null)
            {
                throw new ArgumentNullException("directive");
            }

            FileInfo[] files = directive.Target.GetFiles();
            List<FileRetentionResult> itemResults = new List<FileRetentionResult>();

            foreach (FileInfo file in files)
            {
                FileRetentionDirective fileDirective = new FileRetentionDirective(directive.Policy, file);
                Exception error = null;
                PolicyResult itemResult = PolicyResult.PolicySuccess;
                DateTimeOffset effectiveDate = DateTimeOffset.Now;

                try
                {
                    itemResult = FileRetentionDirective.ApplyPolicyToItem(fileDirective);
                }
                catch (PlatformNotSupportedException ex)
                {
                    error = ex;
                }
                catch (System.Security.SecurityException ex)
                {
                    error = ex;
                }
                catch (UnauthorizedAccessException ex)
                {
                    error = ex;
                }
                catch (IOException ex)
                {
                    error = ex;
                }

                if (error != null)
                {
                    itemResult = PolicyResult.PolicyFailure;
                }

                itemResults.Add(new FileRetentionResult(fileDirective, itemResult, effectiveDate, error));
            }

            return itemResults;
        }
    }
}
