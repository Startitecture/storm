namespace SAF.LocalMachine
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;

    using SAF.Data.Persistence;

    /// <summary>
    /// Contains an archive policy and a file to archive.
    /// </summary>
    public class FileArchiveDirective
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FileArchiveDirective"/> class.
        /// </summary>
        /// <param name="policy">The archive policy to use for archiving the files.</param>
        /// <param name="sourceFiles">The files to archive.</param>
        public FileArchiveDirective(ArchivePolicy<DirectoryInfo> policy, params FileInfo[] sourceFiles)
        {
            if (policy == null)
            {
                throw new ArgumentNullException("policy");
            }

            if (sourceFiles == null)
            {
                throw new ArgumentNullException("sourceFiles");
            }

            this.Policy = policy;
            this.Sources = sourceFiles;
        }

        /// <summary>
        /// Gets the archive policy to use for archiving the file.
        /// </summary>
        public ArchivePolicy<DirectoryInfo> Policy { get; private set; }

        /// <summary>
        /// Gets the files to archive.
        /// </summary>
        public IEnumerable<FileInfo> Sources { get; private set; }

        /// <summary>
        /// Archives a file using the specified <see cref="FileArchiveDirective"/>.
        /// </summary>
        /// <param name="directive">The <see cref="FileArchiveDirective"/> containing the <see cref="FileArchivePolicy"/>
        /// to apply.</param>
        /// <returns>A <see cref="FileArchiveResult"/> containing the result of the action.</returns>
        public static IEnumerable<FileArchiveResult> ApplyPolicyToItems(FileArchiveDirective directive)
        {
            if (directive == null)
            {
                throw new ArgumentNullException("directive");
            }

            List<FileArchiveResult> results = new List<FileArchiveResult>();

            foreach (var item in directive.Sources)
            {
                ArchiveResult result = ArchiveResult.Success;
                Exception error = null;
                FileInfo targetFile = null;
                string sanitizedNameFormat = directive.Policy.DateFormat;

                // Sanitize the date format string.
                foreach (char c in System.IO.Path.GetInvalidFileNameChars())
                {
                    sanitizedNameFormat = sanitizedNameFormat.Replace(c, '_');
                }

                try
                {
                    string newLocation = String.Format(
                        "{0}{1}{2}{3}{4}",
                        directive.Policy.ArchiveContainer.FullName,
                        System.IO.Path.DirectorySeparatorChar,
                        System.IO.Path.GetFileNameWithoutExtension(item.Name),
                        item.LastWriteTime.ToString(sanitizedNameFormat),
                        item.Extension);

                    targetFile = new FileInfo(newLocation);
                }
                catch (ArgumentException ex)
                {
                    result = ArchiveResult.Failed;
                    error = ex;
                }
                catch (PathTooLongException ex)
                {
                    result = ArchiveResult.Failed;
                    error = ex;
                }
                catch (NotSupportedException ex)
                {
                    result = ArchiveResult.Failed;
                    error = ex;
                }
                catch (System.Security.SecurityException ex)
                {
                    result = ArchiveResult.AccessDenied;
                    error = ex;
                }
                catch (UnauthorizedAccessException ex)
                {
                    result = ArchiveResult.AccessDenied;
                    error = ex;
                }
                finally
                {
                    if (error != null)
                    {
                        Trace.TraceError("Could not access file '{0}': {1}", item, error);
                    }
                }

                try
                {
                    // Create the directory if it does not exist.
                    if (!Directory.Exists(targetFile.DirectoryName))
                    {
                        Directory.CreateDirectory(targetFile.DirectoryName);
                    }

                    // Copy rather than move, with overwrite. Use the File static method
                    // so that the original FileInfo is retained.
                    Trace.TraceInformation("Copying '{0}' to '{1}'.", item, targetFile);
                    File.Copy(item.FullName, targetFile.FullName, true);
                }
                catch (ArgumentException ex)
                {
                    result = ArchiveResult.Failed;
                    error = ex;
                }
                catch (FileNotFoundException ex)
                {
                    result = ArchiveResult.Failed;
                    error = ex;
                }
                catch (DirectoryNotFoundException ex)
                {
                    result = ArchiveResult.TargetContainerDoesNotExist;
                    error = ex;
                }
                catch (PathTooLongException ex)
                {
                    result = ArchiveResult.Failed;
                    error = ex;
                }
                catch (NotSupportedException ex)
                {
                    result = ArchiveResult.Failed;
                    error = ex;
                }
                catch (UnauthorizedAccessException ex)
                {
                    result = ArchiveResult.AccessDenied;
                    error = ex;
                }
                catch (IOException ex)
                {
                    result = ArchiveResult.Failed;
                    error = ex;
                }
                finally
                {
                    if (error != null)
                    {
                        Trace.TraceError(
                            "Could not archive file '{0}' to '{1}': {2}", item, targetFile, error);
                    }
                }
                
                results.Add(new FileArchiveResult(directive, item, targetFile, result, error));
            }

            return results;
        }

        /// <summary>
        /// Returns the string representation of this <see cref="FileArchiveDirective"/>.
        /// </summary>
        /// <returns>The path of the file and the target directory.</returns>
        public override string ToString()
        {
            return String.Format("'{0}' in '{1}'", this.Sources, this.Policy.ArchiveContainer);
        }
    }
}
