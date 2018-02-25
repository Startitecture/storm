namespace Startitecture.Orm.Testing.Model
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// The WorkflowFormInstanceService interface.
    /// </summary>
    public interface IWorkflowFormInstanceService
    {
        /// <summary>
        /// Gets the workflow form instances for the specified <paramref name="workflowInstance"/>.
        /// </summary>
        /// <param name="workflowInstance">
        /// The workflow instance to get form instances for.
        /// </param>
        /// <returns>
        /// An <see cref="IEnumerable{T}"/> of <see cref="SAF.Data.Providers.Tests.FieldsModel.WorkflowFormInstance"/> items.
        /// </returns>
        IEnumerable<WorkflowFormInstance> GetInstances(WorkflowInstance workflowInstance);

        /// <summary>
        /// Saves a form instance to a repository.
        /// </summary>
        /// <param name="formInstance">
        /// The form instance to save.
        /// </param>
        /// <returns>
        /// The saved <see cref="SAF.Data.Providers.Tests.FieldsModel.WorkflowFormInstance"/>.
        /// </returns>
        WorkflowFormInstance SaveInstance(WorkflowFormInstance formInstance);

        /// <summary>
        /// Gets the <see cref="SAF.Data.Providers.Tests.FieldsModel.WorkflowFormInstance"/> associated with the specified <paramref name="id"/>.
        /// </summary>
        /// <param name="id">
        /// The ID of the instance to get.
        /// </param>
        /// <returns>
        /// The <see cref="SAF.Data.Providers.Tests.FieldsModel.WorkflowFormInstance"/> associated with the specified <paramref name="id"/>, or null if no instance is found.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="id"/> is less than 1.
        /// </exception>
        WorkflowFormInstance GetInstance(long id);

        /// <summary>
        /// Adds a <see cref="FormLayout"/> to the specified <paramref name="instance"/>
        /// </summary>
        /// <param name="instance">
        /// The instance to add the layout to.
        /// </param>
        /// <param name="layout">
        /// The layout to add.
        /// </param>
        /// <returns>
        /// The newly created <see cref="SAF.Data.Providers.Tests.FieldsModel.WorkflowFormInstance"/>.
        /// </returns>
        WorkflowFormInstance AddInstance(WorkflowInstance instance, FormLayout layout);
    }
}