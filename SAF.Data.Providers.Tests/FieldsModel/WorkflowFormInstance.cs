// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WorkflowFormInstance.cs" company="">
//   
// </copyright>
// <summary>
//   The workflow form instance.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.Data.Providers.Tests.FieldsModel
{
    using System;
    using System.Collections.Generic;

    using JetBrains.Annotations;

    using SAF.Core;
    using SAF.Testing.Common;

    /// <summary>
    /// The workflow form instance.
    /// </summary>
    public class WorkflowFormInstance : IEquatable<WorkflowFormInstance>, IOrderedElement
    {
        /// <summary>
        /// The comparison properties.
        /// </summary>
        private static readonly Func<WorkflowFormInstance, object>[] ComparisonProperties =
            {
                item => item.WorkflowInstance,
                item => item.FormLayout
            };

        /// <summary>
        /// Initializes a new instance of the <see cref="WorkflowFormInstance"/> class.
        /// </summary>
        /// <param name="formLayout">
        /// The form layout.
        /// </param>
        public WorkflowFormInstance([NotNull] FormLayout formLayout)
        {
            if (formLayout == null)
            {
                throw new ArgumentNullException(nameof(formLayout));
            }

            this.FormLayout = formLayout;
            this.InstanceGuid = Guid.NewGuid();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WorkflowFormInstance"/> class.
        /// </summary>
        /// <param name="formLayout">
        /// The form layout.</param>
        /// <param name="workflowInstance">
        /// The workflow instance.
        /// </param>
        /// <param name="workflowFormInstanceId">
        /// The workflow form instance id.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="workflowInstance"/> is null.
        /// </exception>
        public WorkflowFormInstance([NotNull] FormLayout formLayout, [NotNull] WorkflowInstance workflowInstance, long? workflowFormInstanceId)
        {
            if (formLayout == null)
            {
                throw new ArgumentNullException(nameof(formLayout));
            }

            if (workflowInstance == null)
            {
                throw new ArgumentNullException(nameof(workflowInstance));
            }

            this.FormLayout = formLayout;
            this.WorkflowInstance = workflowInstance;
            this.WorkflowFormInstanceId = workflowFormInstanceId;
        }

        /// <summary>
        /// Gets the workflow form instance id.
        /// </summary>
        public long? WorkflowFormInstanceId { get; private set; }

        /// <summary>
        /// Gets the workflow instance.
        /// </summary>
        public WorkflowInstance WorkflowInstance { get; private set; }

        /// <summary>
        /// Gets the workflow instance id.
        /// </summary>
        public long? WorkflowInstanceId
        {
            get
            {
                return this.WorkflowInstance?.WorkflowInstanceId;
            }
        }

        /// <summary>
        /// Gets the form layout.
        /// </summary>
        public FormLayout FormLayout { get; private set; }

        /// <summary>
        /// Gets the form layout id.
        /// </summary>
        public int? FormLayoutId
        {
            get
            {
                return this.FormLayout?.FormLayoutId;
            }
        }

        /// <summary>
        /// Gets the instance GUID for this form instance.
        /// </summary>
        public Guid InstanceGuid { get; private set; }

        /// <summary>
        /// Gets the order of the form instance in the workflow.
        /// </summary>
        public short Order { get; private set; }

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
        public static bool operator ==(WorkflowFormInstance valueA, WorkflowFormInstance valueB)
        {
            return EqualityComparer<WorkflowFormInstance>.Default.Equals(valueA, valueB);
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
        public static bool operator !=(WorkflowFormInstance valueA, WorkflowFormInstance valueB)
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
            return $"{this.WorkflowInstance?.Subject}:{this.FormLayout.Name}";
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
        public bool Equals(WorkflowFormInstance other)
        {
            return Evaluate.Equals(this, other, ComparisonProperties);
        }

        #endregion

        /// <summary>
        /// Sets order of the current element.
        /// </summary>
        /// <param name="searcher">
        /// The ordered element searcher.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="searcher"/> is null.
        /// </exception>
        /// <exception cref="BusinessException">
        /// The current element is not in the searcher's list.
        /// </exception>
        public void SetOrder(IOrderedElementSearcher searcher)
        {
            if (searcher == null)
            {
                throw new ArgumentNullException(nameof(searcher));
            }

            var order = searcher.GetOrder(this);

            if (order < 0)
            {
                throw new BusinessException(this, FieldsMessages.OrderedElementNotInList);
            }

            this.Order = order;
        }

        /// <summary>
        /// Adds the current workflow form instance to the workflow instance.
        /// </summary>
        /// <param name="instance">
        /// The workflow instance to add this form instance to.
        /// </param>
        /// <param name="order">
        /// The order of the form instance.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="instance"/> is null.
        /// </exception>
        internal void AddToInstance([NotNull] WorkflowInstance instance, short order)
        {
            if (instance == null)
            {
                throw new ArgumentNullException(nameof(instance));
            }

            this.WorkflowInstance = instance;
            this.Order = order;
        }

        /// <summary>
        /// Adds a form layout to the current instance.
        /// </summary>
        /// <param name="layout">
        /// The layout.
        /// </param>
        /// <param name="order">
        /// The order of the layout.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="layout"/> is null.
        /// </exception>
        internal void AddFormLayout([NotNull] FormLayout layout, short order)
        {
            if (layout == null)
            {
                throw new ArgumentNullException(nameof(layout));
            }

            this.FormLayout = layout;
            this.Order = order;
        }
    }
}