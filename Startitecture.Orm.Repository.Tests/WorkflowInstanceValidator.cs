namespace Startitecture.Orm.Repository.Tests
{
    using System;

    using FluentValidation;

    using Startitecture.Orm.Testing.Model;

    /// <summary>
    /// The workflow instance validator.
    /// </summary>
    public class WorkflowInstanceValidator : AbstractValidator<WorkflowInstance>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WorkflowInstanceValidator"/> class.
        /// </summary>
        public WorkflowInstanceValidator()
        {
            this.RuleFor(instance => instance.WorkflowInstanceId).GreaterThan(0).When(instance => instance.WorkflowInstanceId.HasValue);
            this.RuleFor(instance => instance.Subject).NotEmpty();
            this.RuleFor(instance => instance.InitiatedBy).NotEmpty();
            this.RuleFor(instance => instance.InitiationTime).GreaterThan(DateTimeOffset.MinValue);
        }
    }
}