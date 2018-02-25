namespace Startitecture.Orm.Repository.Tests
{
    using FluentValidation;

    using Startitecture.Orm.Testing.Model;

    /// <summary>
    /// The workflow form instance validator.
    /// </summary>
    public class WorkflowFormInstanceValidator : AbstractValidator<WorkflowFormInstance>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WorkflowFormInstanceValidator"/> class.
        /// </summary>
        public WorkflowFormInstanceValidator()
        {
            this.RuleFor(instance => instance.WorkflowFormInstanceId).GreaterThan(0).When(instance => instance.WorkflowFormInstanceId.HasValue);
            this.RuleFor(instance => instance.FormLayoutId).NotNull().GreaterThan(0);
            this.RuleFor(instance => instance.WorkflowInstanceId).NotNull().GreaterThan(0);
            this.RuleFor(instance => instance.InstanceGuid).NotEmpty();
            this.RuleFor(instance => instance.Order).GreaterThan((short)0);
        }
    }
}