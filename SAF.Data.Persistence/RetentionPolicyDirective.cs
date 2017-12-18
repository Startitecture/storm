namespace SAF.Data.Persistence
{
    using System;

    /// <summary>
    /// Contains instructions for applying a file retention policy.
    /// </summary>
    /// <typeparam name="TContainer">The type of container that the policy will apply to.</typeparam>
    public class RetentionPolicyDirective<TContainer>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RetentionPolicyDirective&lt;TContainer&gt;"/> class.
        /// </summary>
        /// <param name="policy">The policy to apply.</param>
        /// <param name="target">The container to apply the policy to.</param>
        protected RetentionPolicyDirective(RetentionPolicy policy, TContainer target)
        {
            if (policy == null)
            {
                throw new ArgumentNullException("policy");
            }

            if (target == null)
            {
                throw new ArgumentNullException("target");
            }

            this.Policy = policy;
            this.Target = target;
        }

        /// <summary>
        /// Gets the retention policy to apply.
        /// </summary>
        public RetentionPolicy Policy { get; private set; }

        /// <summary>
        /// Gets the container to apply the policy to.
        /// </summary>
        public TContainer Target { get; private set; }
    }
}
