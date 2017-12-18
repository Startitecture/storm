namespace SAF.Data.Persistence
{
    /// <summary>
    /// Contains instructions for applying a retention policy to a specific item.
    /// </summary>
    /// <typeparam name="TItem">The type of item the policy is applied to.</typeparam>
    public class ItemRetentionDirective<TItem>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ItemRetentionDirective&lt;TItem&gt;"/> class with the 
        /// specified policy and target.
        /// </summary>
        /// <param name="policy">The policy to apply.</param>
        /// <param name="target">The target to apply the policy to.</param>
        public ItemRetentionDirective(RetentionPolicy policy, TItem target)
        {
            this.Policy = policy;
            this.Target = target;
        }

        /// <summary>
        /// Gets the policy that will be applied.
        /// </summary>
        public RetentionPolicy Policy { get; private set; }

        /// <summary>
        /// Gets the target to apply the policy to.
        /// </summary>
        public TItem Target { get; private set; }
    }
}
