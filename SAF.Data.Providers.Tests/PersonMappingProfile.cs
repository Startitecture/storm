// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PersonMappingProfile.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.Data.Providers.Tests
{
    /// <summary>
    /// The person mapping profile.
    /// </summary>
    public class PersonMappingProfile : EntityMappingProfile<Person, ActionPrincipalRow>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PersonMappingProfile" /> class.
        /// </summary>
        public PersonMappingProfile()
        {
            this.SetPrimaryKey(person => person.PersonId, row => row.PersonId)
                .SetUniqueKey(person => person.AccountName)
                .IgnoreForDataItem(row => row.UserId, row => row.AccountName);
        }
    }
}