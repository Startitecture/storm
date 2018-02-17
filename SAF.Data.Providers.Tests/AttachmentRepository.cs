// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AttachmentRepository.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.Data.Providers.Tests
{
    using Startitecture.Orm.Common;
    using Startitecture.Orm.Query;

    /// <summary>
    /// The attachment repository.
    /// </summary>
    public class AttachmentRepository : EntityRepository<Attachment, AttachmentRow>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AttachmentRepository"/> class.
        /// </summary>
        /// <param name="repositoryProvider">
        /// The repository provider.
        /// </param>
        public AttachmentRepository(IRepositoryProvider repositoryProvider)
            : base(repositoryProvider)
        {
        }

        /// <summary>
        /// Gets a unique item selection for the specified item.
        /// </summary>
        /// <param name="item">
        /// The item to create the selection for.
        /// </param>
        /// <returns>
        /// A <see cref="T:SAF.Data.ItemSelection`1"/> for the specified item.
        /// </returns>
        protected override ItemSelection<AttachmentRow> GetUniqueItemSelection(AttachmentRow item)
        {
            return this.GetKeySelection(item, row => row.AttachmentId);
        }
    }
}