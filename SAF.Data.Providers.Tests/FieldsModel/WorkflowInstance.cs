namespace SAF.Data.Providers.Tests.FieldsModel
{
    using System;
    using System.Collections.Generic;

    using JetBrains.Annotations;

    using Startitecture.Core;

    /// <summary>
    /// The workflow instance.
    /// </summary>
    public class WorkflowInstance : IEquatable<WorkflowInstance>
    {
        /// <summary>
        /// The comparison properties.
        /// </summary>
        private static readonly Func<WorkflowInstance, object>[] ComparisonProperties =
            {
                item => item.Subject,
                item => item.InitiatedBy,
                item => item.InitiationTime
            };

        /// <summary>
        /// The workflow form instances.
        /// </summary>
        private readonly SortedSet<WorkflowFormInstance> workflowFormInstances =
            new SortedSet<WorkflowFormInstance>(Singleton<OrderedElementComparer>.Instance);

        /// <summary>
        /// Initializes a new instance of the <see cref="WorkflowInstance"/> class.
        /// </summary>
        public WorkflowInstance()
            : this(null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WorkflowInstance"/> class.
        /// </summary>
        /// <param name="workflowInstanceId">
        /// The workflow instance id.
        /// </param>
        public WorkflowInstance(long? workflowInstanceId)
        {
            this.WorkflowInstanceId = workflowInstanceId;
        }

        /// <summary>
        /// Gets the workflow instance id.
        /// </summary>
        public long? WorkflowInstanceId { get; private set; }

        /// <summary>
        /// Gets or sets the subject.
        /// </summary>
        public string Subject { get; set; }

        /// <summary>
        /// Gets the initiated by.
        /// </summary>
        public Person InitiatedBy { get; private set; }

        /// <summary>
        /// Gets the initiation time.
        /// </summary>
        public DateTimeOffset InitiationTime { get; private set; }

        /// <summary>
        /// The workflow form instances.
        /// </summary>
        public IEnumerable<WorkflowFormInstance> WorkflowFormInstances => this.workflowFormInstances;

        #region Equality and Comparison Methods

        /// <summary>
        /// Determines if two values of the same type are equal.
        /// </summary>
        /// <param name="valueA">
        /// The first value to compare.
        /// </param>
        /// <param name="valueB">
        /// The second value to compare.
        /// </param>
        /// <returns>
        /// <c>true</c> if the values are equal; otherwise, <c>false</c>.
        /// </returns>
        public static bool operator ==(WorkflowInstance valueA, WorkflowInstance valueB)
        {
            return EqualityComparer<WorkflowInstance>.Default.Equals(valueA, valueB);
        }

        /// <summary>
        /// Determines if two values of the same type are not equal.
        /// </summary>
        /// <param name="valueA">
        /// The first value to compare.
        /// </param>
        /// <param name="valueB">
        /// The second value to compare.
        /// </param>
        /// <returns>
        /// <c>true</c> if the values are not equal; otherwise, <c>false</c>.
        /// </returns>
        public static bool operator !=(WorkflowInstance valueA, WorkflowInstance valueB)
        {
            return !(valueA == valueB);
        }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        /// A string that represents the current object.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override string ToString()
        {
            return this.Subject;
        }

        /// <summary>
        /// Serves as the default hash function. 
        /// </summary>
        /// <returns>
        /// A hash code for the current object.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override int GetHashCode()
        {
            return Evaluate.GenerateHashCode(this, ComparisonProperties);
        }

        /// <summary>
        /// Determines whether the specified object is equal to the current object.
        /// </summary>
        /// <returns>
        /// true if the specified object  is equal to the current object; otherwise, false.
        /// </returns>
        /// <param name="obj">The object to compare with the current object. </param><filterpriority>2</filterpriority>
        public override bool Equals(object obj)
        {
            return Evaluate.Equals(this, obj);
        }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.
        /// </returns>
        /// <param name="other">An object to compare with this object.</param>
        public bool Equals(WorkflowInstance other)
        {
            return Evaluate.Equals(this, other, ComparisonProperties);
        }

        #endregion

        /// <summary>
        /// Initiates the current workflow.
        /// </summary>
        /// <param name="initiator">
        /// The initiator of the workflow.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="initiator"/> is null.
        /// </exception>
        /// <exception cref="BusinessException">
        /// The workflow has already been initiated.
        /// </exception>
        public void Initiate([NotNull] Person initiator)
        {
            if (initiator == null)
            {
                throw new ArgumentNullException(nameof(initiator));
            }

            if (this.InitiationTime > DateTimeOffset.MinValue)
            {
                throw new BusinessException(this, FieldsMessages.WorkflowAlreadyInitiated);
            }

            this.InitiatedBy = initiator;
            this.InitiationTime = DateTimeOffset.Now;
        }

        /// <summary>
        /// Adds a form to the workflow instance.
        /// </summary>
        /// <param name="layout">
        /// The form layout to add.
        /// </param>
        /// <returns>
        /// The <see cref="WorkflowFormInstance"/> added to the workflow instance.
        /// </returns>
        public WorkflowFormInstance AddLayout([NotNull] FormLayout layout)
        {
            if (layout == null)
            {
                throw new ArgumentNullException(nameof(layout));
            }

            var formInstance = new WorkflowFormInstance(layout);
            var order = (short)(this.workflowFormInstances.Max?.Order + 1 ?? 1);
            formInstance.AddToInstance(this, order);
            this.workflowFormInstances.Add(formInstance);
            return formInstance;
        }

        /// <summary>
        /// Loads all of the workflow form instances for the current instance.
        /// </summary>
        /// <param name="formInstanceService">
        /// The form instance service.
        /// </param>
        public void Load([NotNull] IWorkflowFormInstanceService formInstanceService)
        {
            if (formInstanceService == null)
            {
                throw new ArgumentNullException(nameof(formInstanceService));
            }

            var formInstances = formInstanceService.GetInstances(this);
            this.workflowFormInstances.Clear();

            foreach (var instance in formInstances)
            {
                instance.AddToInstance(this, (short)(instance.Order > 0 ? instance.Order : this.workflowFormInstances.Count + 1));

                if (this.workflowFormInstances.Add(instance) == false)
                {
                    throw new BusinessException(instance, FieldsMessages.SortedItemAlreadyAddedToCollection);
                }
            }
        }
    }
}