namespace Startitecture.Orm.Testing.Model.DocumentEntities
{
    using Startitecture.Orm.Common;
    using Startitecture.Orm.Schema;

    /// <summary>
    /// The user row.
    /// </summary>
    [TableName("[dbo].[ViewPersons]")]
    [ExplicitColumns]
    public class UserRow : ITransactionContext
    {
        /// <summary>
        /// Gets or sets the user id.
        /// </summary>
        [Column]
        public int UserId { get; set; }

        /// <summary>
        /// Gets or sets the person id.
        /// </summary>
        [Column]
        public int PersonId { get; set; }

        /// <summary>
        /// Gets or sets the account name.
        /// </summary>
        [Column]
        public string AccountName { get; set; }

        /// <summary>
        /// Gets or sets the email.
        /// </summary>
        [Column]
        public string Email { get; set; }

        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        [Column]
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the last name.
        /// </summary>
        [Column]
        public string LastName { get; set; }

        /// <summary>
        /// Gets or sets the first name.
        /// </summary>
        [Column]
        public string FirstName { get; set; }

        /// <summary>
        /// Gets or sets the middle name.
        /// </summary>
        [Column]
        public string MiddleName { get; set; }

        /// <summary>
        /// Gets the transaction provider.
        /// </summary>
        public IRepositoryProvider TransactionProvider { get; private set; }

        /// <summary>
        /// Sets the transaction provider for the current object.
        /// </summary>
        /// <param name="repositoryProvider">
        /// The repository provider to use for the transaction context.
        /// </param>
        public void SetTransactionProvider(IRepositoryProvider repositoryProvider)
        {
            this.TransactionProvider = repositoryProvider;
        }
    }
}
