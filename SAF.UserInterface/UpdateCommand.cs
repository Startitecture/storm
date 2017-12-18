// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UpdateCommand.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.UserInterface
{
    using System;
    using System.Windows.Threading;

    using SAF.Core;
    using SAF.Data;
    using SAF.Data.Persistence;
    using SAF.ProcessEngine;

    /// <summary>
    /// Base class for update commands.
    /// </summary>
    /// <typeparam name="TDirective">
    /// The type of directive that the command executes.
    /// </typeparam>
    /// <typeparam name="TSource">
    /// The type of data source used for the update.
    /// </typeparam>
    public abstract class UpdateCommand<TDirective, TSource> : CommandBase<TDirective>, IObservableCommand
        where TSource : IDataSource
    {
        /// <summary>
        /// The data updater that will process the update command.
        /// </summary>
        private readonly IEntityUpdater<TSource> updater;

        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateCommand{TDirective,TSource}"/> class. 
        /// Initializes a new instance of the <see cref="T:SAF.UserInterface.UpdateCommand`2"/> class.
        /// </summary>
        /// <param name="name">
        /// The name of the command.
        /// </param>
        /// <param name="updater">
        /// The entity updater that will execute the command.
        /// </param>
        /// <param name="commandDispatcher">
        /// The dispatcher that will relay command events back to the caller.
        /// </param>
        protected UpdateCommand(string name, IEntityUpdater<TSource> updater, Dispatcher commandDispatcher)
            : base(name, "The command is already executing.")
        {
            if (updater == null)
            {
                throw new ArgumentNullException("updater");
            }

            if (commandDispatcher == null)
            {
                throw new ArgumentNullException("commandDispatcher");
            }

            this.updater = updater;

            this.updater.Process.ProcessStarted += delegate
            {
                commandDispatcher.BeginInvoke(new Action(this.TriggerCanExecuteChanged));
            };

            this.updater.Process.ProcessStopped += delegate 
            {
                commandDispatcher.BeginInvoke(new Action(this.TriggerCanExecuteChanged));
            };
        }

        /// <summary>
        /// Generates a data source for this update command.
        /// </summary>
        /// <param name="directive">
        /// A directive containing the information needed to create the data source.
        /// </param>
        /// <returns>
        /// A data source based on the information provided by the directive.
        /// </returns>
        protected abstract TSource GenerateDataSource(TDirective directive);

        /// <summary>
        /// Indicates whether the command can execute.
        /// </summary>
        /// <param name="directive">
        /// The directive for this command.
        /// </param>
        /// <param name="parameter">
        /// Data used by the command. If the command does not require data to be passed, this 
        /// object can be set to null.
        /// </param>
        /// <returns>
        /// True if this command can be executed; otherwise, false.
        /// </returns>
        protected override sealed bool CanExecute(TDirective directive, object parameter)
        {
            return !this.updater.Process.IsBusy;
        }

        /// <summary>
        /// Invokes the update command.
        /// </summary>
        /// <param name="directive">
        /// The directive for the command.
        /// </param>
        /// <param name="parameter">
        /// The optional parameter associated with this specific execution.
        /// </param>
        protected override sealed void InvokeCommand(TDirective directive, object parameter)
        {
            if (Evaluate.IsNull(directive))
            {
                throw new ArgumentNullException("directive");
            }

            // TODO: Consider removing these throw statments which rewrap the exception unnecessarily.
            try
            {
                TSource dataSource = this.GenerateDataSource(directive);

                if (!Evaluate.IsDefaultValue(dataSource))
                {
                    this.updater.Update(dataSource);
                }
            }
            catch (AccessException ex)
            {
                throw new AccessException(
                    directive, String.Format("The data source could not be processed: {0}", ex.Message), ex);
            }
            catch (BusinessException ex)
            {
                throw new BusinessException(
                    directive, String.Format("The data source could not be processed: {0}", ex.Message), ex);
            }
            catch (OperationException ex)
            {
                throw new OperationException(
                    directive, String.Format("The data source could not be processed: {0}", ex.Message), ex);
            }
        }
    }
}
