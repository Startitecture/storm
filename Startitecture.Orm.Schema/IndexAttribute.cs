// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IndexAttribute.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.Schema
{
    using System;

    /// <summary>
    /// The index attribute.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public sealed class IndexAttribute : Attribute
    {
        /// <summary>
        /// The instance GUID.
        /// </summary>
        private readonly Guid instanceGuid = Guid.NewGuid();

        /// <summary>
        /// Initializes a new instance of the <see cref="IndexAttribute"/> class.
        /// </summary>
        public IndexAttribute()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="IndexAttribute"/> class.
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        public IndexAttribute(string name)
            : this(name, -1)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="IndexAttribute"/> class.
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <param name="order">
        /// The column order.
        /// </param>
        public IndexAttribute(string name, int order)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(name));
            }

            this.Name = name;
            this.Order = order;
        }

        /// <summary>
        /// Gets or sets a value indicating whether is clustered.
        /// </summary>
        public bool IsClustered { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether is clustered configured.
        /// </summary>
        public bool IsClusteredConfigured { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether is unique.
        /// </summary>
        public bool IsUnique { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether is unique configured.
        /// </summary>
        public bool IsUniqueConfigured { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the order.
        /// </summary>
        public int Order { get; set; }

        /// <summary>
        /// The type id.
        /// </summary>
        public override object TypeId => this.instanceGuid;
    }
}