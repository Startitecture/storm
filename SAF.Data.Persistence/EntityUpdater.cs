namespace SAF.Data.Persistence
{
    using System;
    using System.Data;
    using System.Diagnostics;
    using System.Threading;

    using SAF.Core;
    using SAF.ProcessEngine;

    /// <summary>
    /// Updates entities using a persistence adapter.
    /// </summary>
    /// <typeparam name="T">The type of entity to persist.</typeparam>
    public sealed class EntityUpdater<T> :
        TaskEngine<EntityUpdateDirective<T>, EntityUpdateResult<T>>,
        IPersistenceEngine
    {
        #region Constants

        /// <summary>
        /// Name mask for this class.
        /// </summary>
        private const string NameMask = "Data Updater ({0})";

        #endregion

        #region Fields

        /// <summary>
        /// The total number of entities added to the store.
        /// </summary>
        private long addedItems;

        /// <summary>
        /// The total number of entities modified in the store.
        /// </summary>
        private long modifiedItems;

        /// <summary>
        /// The total number of entities that remained the same.
        /// </summary>
        private long unchangedItems;

        /// <summary>
        /// The total number of entities that were removed.
        /// </summary>
        private long removedItems;

        /// <summary>
        /// The total number of entities updates that were rolled back.
        /// </summary>
        private long rolledBackItems;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="EntityUpdater&lt;T&gt;"/> class.
        /// </summary>
        public EntityUpdater()
            : base("Entity Updater")
        {
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the number of added items this updater has processed.
        /// </summary>
        public long AddedItems
        {
            get { return Interlocked.Read(ref this.addedItems); }
        }

        /// <summary>
        /// Gets the number of modified items this updater has processed.
        /// </summary>
        public long ModifiedItems
        {
            get { return Interlocked.Read(ref this.modifiedItems); }
        }

        /// <summary>
        /// Gets the number of unchanged items this updater has processed.
        /// </summary>
        public long UnchangedItems
        {
            get { return Interlocked.Read(ref this.unchangedItems); } 
        }

        /// <summary>
        /// Gets the number of removed items this updater has processed.
        /// </summary>
        public long RemovedItems
        {
            get { return Interlocked.Read(ref this.removedItems); }
        }

        /// <summary>
        /// Gets the number of rolled back items this updater has processed.
        /// </summary>
        public long RolledBackItems
        {
            get { return Interlocked.Read(ref this.rolledBackItems); }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Adds an entity to be updated in the target dataset.
        /// </summary>
        /// <param name="directive">The entity update directive to process.</param>
        public void UpdateEntity(EntityUpdateDirective<T> directive)
        {
            this.QueueTask(directive);
        }

        /// <summary>
        /// Gets a string representation of the current <see cref="EntityUpdater&lt;T&gt;"/>.
        /// </summary>
        /// <returns>A string indicating the type of entity that is updated.</returns>
        [DebuggerHidden]
        public override string ToString()
        {
            return String.Format(NameMask, typeof(T).Name);
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Consumes a <see cref="EntityUpdateDirective&lt;T&gt;"/> task.
        /// </summary>
        /// <param name="directive">The directive to consume.</param>
        /// <returns>A <see cref="EntityUpdateResult&lt;T&gt;"/> containing the results of the task.</returns>
        protected override EntityUpdateResult<T> ConsumeItem(EntityUpdateDirective<T> directive)
        {
            if (directive == null)
            {
                throw new ArgumentNullException("directive");
            }

            EntityAction result = EntityAction.NoChange;
            Exception error = null;

            try
            {
                result = directive.Adapter.SaveEntity(directive.Entity);
            }
            catch (ArgumentException ex)
            {
                error = ex;
            }
            catch (InvalidOperationException ex)
            {
                error = ex;
            }
            catch (OperationException ex)
            {
                error = ex;
            }
            catch (AccessException ex)
            {
                error = ex;
            }
            catch (BusinessException ex)
            {
                error = ex;
            }
            catch (DataException ex)
            {
                error = ex;
            }
            catch (System.Data.Common.DbException ex)
            {
                error = ex;
            }

            if (error != null)
            {
                result = EntityAction.Rollback;
                Trace.TraceError(ErrorMessages.CouldNotPersistEntity, this, directive.Entity, error);
            }

            EntityUpdateResult<T> entityResult = new EntityUpdateResult<T>(directive, result, error);
            this.ProcessResult(entityResult);
            return entityResult;
        }

        /// <summary>
        /// Processes the result of the task.
        /// </summary>
        /// <param name="taskResult">The result of the task.</param>
        private void ProcessResult(EntityUpdateResult<T> taskResult)
        {
            if (taskResult == null)
            {
                throw new ArgumentNullException("taskResult");
            }

            switch (taskResult.Result)
            {
                case EntityAction.Rollback:
                    Interlocked.Increment(ref this.rolledBackItems);
                    break;
                case EntityAction.NoChange:
                    Interlocked.Increment(ref this.unchangedItems);
                    break;
                case EntityAction.Add:
                    Interlocked.Increment(ref this.addedItems);
                    break;
                case EntityAction.Modify:
                    Interlocked.Increment(ref this.modifiedItems);
                    break;
                case EntityAction.Remove:
                    Interlocked.Increment(ref this.removedItems);
                    break;
            }
        }

        #endregion
    }
}
