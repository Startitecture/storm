namespace SAF.LocalMachine
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using SAF.Core;
    using SAF.Data.Persistence;
    using SAF.ProcessEngine;

    /// <summary>
    /// Applies a retention policy to a directory.
    /// </summary>
    public class FolderRetentionPolicyCommand : CommandBase<FolderRetentionPolicyDirective>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FolderRetentionPolicyCommand"/> class.
        /// </summary>
        public FolderRetentionPolicyCommand()
            : base("Apply Retention to")
        {
        }

        /// <summary>
        /// Determines whether the command can execute in the current context.
        /// </summary>
        /// <param name="directive">The directive associated with the command.</param>
        /// <param name="parameter">The optional parameter associated with the command.</param>
        /// <returns><c>true</c> if the command can execute in the current context; otherwise <c>false</c>.</returns>
        protected override bool CanExecute(FolderRetentionPolicyDirective directive, object parameter)
        {
            if (directive == null)
            {
                throw new ArgumentNullException("directive");
            }

            return directive.Target.Exists;
        }

        /// <summary>
        /// Executes the <see cref="FolderRetentionPolicyCommand"/> with the specified <see cref="FolderRetentionPolicyDirective"/>.
        /// </summary>
        /// <param name="directive">The <see cref="FolderRetentionPolicyDirective"/> to execute.</param>
        /// <param name="parameter">Data used by the command. If the command does not require data to be passed, this 
        /// object can be set to null.</param>
        protected override void InvokeCommand(FolderRetentionPolicyDirective directive, object parameter)
        {
            if (directive == null)
            {
                throw new ArgumentNullException("directive");
            }

            IEnumerable<FileRetentionResult> result = FolderRetentionPolicyDirective.ApplyPolicyToContainer(directive);

            if (result.Any(x => x.Result == PolicyResult.PolicyFailure))
            {
                var failures = from x in result
                               where x.Result == PolicyResult.PolicyFailure
                               select String.Format("{0}: {1}", x.Directive.Target.Name, x.ItemError.Message);

                string message =
                    String.Format(
                        "The retention policy failed on one or more items in {0}:{1}{2}",
                        directive.Target,
                        Environment.NewLine,
                        String.Join(Environment.NewLine, failures.ToArray()));

                throw new OperationException(
                    directive,
                    message,
                    result.Select(x => x.ItemError).FirstOrDefault());
            }
        }
    }
}
