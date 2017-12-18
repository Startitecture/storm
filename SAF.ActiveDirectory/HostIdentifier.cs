// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HostIdentifier.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// <summary>
//   Contains properties that can be used to look up a computer in Active Directory.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.ActiveDirectory
{
    using System;
    using System.Collections.Generic;

    using SAF.Core;

    /// <summary>
    /// Contains properties that can be used to look up a computer in Active Directory.
    /// </summary>
    public class HostIdentifier : AccountIdentifier
    {
        /// <summary>
        /// The <see cref="HostIdentifier"/> of the local machine.
        /// </summary>
        private static readonly HostIdentifier CurrentId = new HostIdentifier
                                                               {
                                                                   AccountName = NetworkEnvironment.HostName + "$",
                                                                   Name = NetworkEnvironment.HostName,
                                                                   Domain = NetworkEnvironment.DomainName
                                                               };

        /// <summary>
        /// Gets the <see cref="HostIdentifier"/> of the local machine.
        /// </summary>
        public static HostIdentifier Current
        {
            get { return CurrentId; }
        }

        /// <summary>
        /// Gets or sets the name of the computer.
        /// </summary>
        public string Name { get; set; }

        #region Base Class Overrides

        /// <summary>
        /// Returns a <see cref="String"/> that represents the current <see cref="HostIdentifier"/>.
        /// </summary>
        /// <returns>A <see cref="String"/> representation of this <see cref="HostIdentifier"/>.</returns>
        public override string ToString()
        {
            return String.IsNullOrEmpty(this.Name) ? base.ToString() : this.Name;
        }

        /// <summary>
        /// Determines whether the specified <see cref="Object"/> is equal to the current <see cref="HostIdentifier"/>.
        /// </summary>
        /// <param name="obj">The object to compare.</param>
        /// <returns>
        /// <c>true</c> if the specified <see cref="Object"/> is equal to the current <see cref="HostIdentifier"/>; otherwise, 
        /// <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            if (base.Equals(obj))
            {
                return true;
            }

            return (obj is HostIdentifier) && this.Equals(obj as HostIdentifier);
        }

        /// <summary>
        /// Determines whether the specified <see cref="HostIdentifier"/> is equal to the current <see cref="HostIdentifier"/>.
        /// </summary>
        /// <param name="obj">The <see cref="HostIdentifier"/> to compare.</param>
        /// <returns>
        /// <c>true</c> if the specified <see cref="HostIdentifier"/> is equal to the current <see cref="HostIdentifier"/>; otherwise, 
        /// <c>false</c>.
        /// </returns>
        public bool Equals(HostIdentifier obj)
        {
            if (obj == null)
            {
                return false;
            }

            return base.Equals(obj) && this.Name.Equals(obj.Name);
        }

        /// <summary>
        /// Serves as a hash function for a particular type.
        /// </summary>
        /// <returns>A hash code for the current <see cref="HostIdentifier"/>.</returns>
        public override int GetHashCode()
        {
            return
                base.GetHashCode() ^
                Evaluate.GenerateHashCode(this.Name);
        }

        /// <summary>
        /// Returns a list of validation errors for the current entity.
        /// </summary>
        /// <param name="messages">
        /// A list of existing messages. New messages should be added to this list.
        /// </param>
        /// <returns>
        /// A collection of validation errors for the current entity. If the list is empty, the entity is correctly formed.
        /// </returns>
        protected override IEnumerable<string> Validate(IList<string> messages)
        {
            if (messages == null)
            {
                throw new ArgumentNullException("messages");
            }

            if (String.IsNullOrWhiteSpace(this.Name))
            {
                messages.Add(ValidationMessages.HostIdentifierRequiresName);
            }

            return messages;
        }

        #endregion
    }
}
