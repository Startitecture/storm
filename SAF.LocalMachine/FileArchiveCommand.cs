namespace SAF.LocalMachine
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using SAF.Core;
    using SAF.ProcessEngine;

    /// <summary>
    /// A command that archives a data source contained in a single file.
    /// </summary>
    public class FileArchiveCommand : CommandBase<FileArchiveDirective>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FileArchiveCommand"/> class.
        /// </summary>
        public FileArchiveCommand()
            : base("Archive files")
        {
        }

        /// <summary>
        /// Determines whether the command can execute in the current context.
        /// </summary>
        /// <param name="directive">The directive associated with the command.</param>
        /// <param name="parameter">The optional parameter associated with the command.</param>
        /// <returns><c>true</c> if the command can execute in the current context; otherwise <c>false</c>.</returns>
        protected override bool CanExecute(FileArchiveDirective directive, object parameter)
        {
            if (directive == null)
            {
                throw new ArgumentNullException("directive");
            }

            return directive.Policy.ArchiveContainer.Exists;
        }

        /// <summary>
        /// Executes the command with the specified parameters.
        /// </summary>
        /// <param name="directive">The parameters to use to execute the command.</param>
        /// <param name="parameter">Data used by the command. If the command does not require data to be passed, this 
        /// object can be set to null.</param>
        protected override void InvokeCommand(FileArchiveDirective directive, object parameter)
        {
            IEnumerable<FileArchiveResult> results = FileArchiveDirective.ApplyPolicyToItems(directive);

            var errors = results.Where(x => x.ResultState == ResultState.Error);
            var archiveResults = errors as IList<FileArchiveResult> ?? errors.ToList();

            if (archiveResults.Any() == false)
            {
                return;
            }

            var errorMessages =
                archiveResults.Select(x => String.Format("{0}: {1}", x.Source, x.ItemError == null ? "Unknown" : x.ItemError.Message))
                    .ToArray();

            throw new BusinessException(directive, errorMessages);
        }
    }
}
