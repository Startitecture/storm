// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AuditConfigurationSection.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// <summary>
//   A configuration element for entity services.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.ActionTracking
{
    using System;
    using System.Configuration;

    /// <summary>
    /// A configuration element for entity services.
    /// </summary>
    public class AuditConfigurationSection : ConfigurationSection
    {
        /// <summary>
        /// The default entity services configuration group.
        /// </summary>
        public const string ConfigurationPath = "audit/auditConfiguration";

        /// <summary>
        /// The event repository connection element.
        /// </summary>
        private const string EventRepositoryConnectionElement = "eventRepositoryConnection";

        /// <summary>
        /// The event repository type element.
        /// </summary>
        private const string EventRepositoryTypeElement = "eventRepositoryType";

        /// <summary>
        /// Gets or sets the event repository type.
        /// </summary>
        [ConfigurationProperty(EventRepositoryTypeElement, IsRequired = true)]
        public string EventRepositoryType
        {
            get
            {
                return Convert.ToString(this[EventRepositoryTypeElement]);
            }

            set
            {
                this[EventRepositoryTypeElement] = value;
            }
        }

        /// <summary>
        /// Gets or sets the event repository connection.
        /// </summary>
        [ConfigurationProperty(EventRepositoryConnectionElement, IsRequired = true)]
        public string EventRepositoryConnection
        {
            get
            {
                return Convert.ToString(this[EventRepositoryConnectionElement]);
            }

            set
            {
                this[EventRepositoryConnectionElement] = value;
            }
        }
    }
}
