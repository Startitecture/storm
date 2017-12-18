namespace SAF.Data.Persistence
{
    using System;

    /// <summary>
    /// Contains instructions for archiving items.
    /// </summary>
    /// <typeparam name="TContainer">The type of container that will contain the archived items.</typeparam>
    public class ArchivePolicy<TContainer>
    {
        /// <summary>
        /// Date format to apply to the archive items.
        /// </summary>
        private string dateFormat;

        /// <summary>
        /// Initializes a new instance of the <see cref="ArchivePolicy{TContainer}"/> class
        /// with the specified name format and target container.
        /// </summary>
        /// <param name="dateFormat">The date format to apply to the archived items.</param>
        /// <param name="archiveContainer">The container to store archived items in.</param>
        public ArchivePolicy(string dateFormat, TContainer archiveContainer)
        {
            this.dateFormat = dateFormat;
            this.ArchiveContainer = archiveContainer;
        }

        /// <summary>
        /// Gets or sets the date format to apply to the archived items.
        /// </summary>
        public string DateFormat
        {
            get { return this.dateFormat; }
            set { this.dateFormat = value == null ? String.Empty : value; }
        }

        /// <summary>
        /// Gets or sets the container to store archived items in.
        /// </summary>
        public TContainer ArchiveContainer { get; set; }
    }
}
