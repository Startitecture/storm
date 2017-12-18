namespace SAF.EntityServices
{
    using System.Runtime.Serialization;

    /// <summary>
    /// Base class for encapsulating entity operations.
    /// </summary>
    /// <remarks>
    /// In order to gain the most from inheriting this class, subclasses should encapsulate enough information to 
    /// support the largest set of entity operations possible.
    /// </remarks>
    /// <typeparam name="T">The type of entity to encapsulate operations for.</typeparam>
    [DataContract]
    public abstract class EntityOperationBase<T>
    {
        /// <summary>
        /// The entity that is the target of the operation.
        /// </summary>
        private T entity;

        /// <summary>
        /// Gets or sets the entity on which operations will be performed.
        /// </summary>
        [DataMember]
        public T Entity
        {
            get 
            { 
                return this.entity; 
            }

            set 
            {
                this.entity = value;
                this.SetEntity(this.entity); 
            }
        }

        /// <summary>
        /// Gets or sets the surrogate ID of the entity.
        /// </summary>
        [DataMember]
        public abstract long EntityId { get; set; }

        /// <summary>
        /// Gets the entity's identifying name.
        /// </summary>
        public abstract string EntityName { get; }

        /// <summary>
        /// Gets a natural property of the entity that makes it unique in conjunction with the name.
        /// </summary>
        public abstract object EntityIdentityProperty { get; }

        /// <summary>
        /// Gets a value indicating whether this entity exists in a data store.
        /// </summary>
        public abstract bool EntityExists 
        { 
            get; 
        }

        /// <summary>
        /// Gets or sets the organizational context ID of the operation.
        /// </summary>
        /// <remarks>
        /// The organization context is a data container based on organizational structure, such as a location, 
        /// department, division, region, facility, campus, etc.
        /// </remarks>
        [DataMember]
        public abstract long OrganizationalContextId { get; set; }

        /// <summary>
        /// Performs additional operations on the entity to ensure data integrity.
        /// </summary>
        /// <param name="item">The entity to set.</param>
        protected abstract void SetEntity(T item);
    }
}