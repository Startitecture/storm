namespace SAF.ActiveDirectory.Search
{
    using System;
    using System.DirectoryServices;
    using System.DirectoryServices.AccountManagement;
    using System.Security.Permissions;

    using SAF.ActiveDirectory;

    /// <summary>
    /// Creates directory entries from a variety of inputs.
    /// </summary>
    public static class DirectoryEntryFactory
    {
        /// <summary>
        /// Gets the DirectoryEntry associated with the provided distinguished name.
        /// </summary>
        /// <param name="distinguishedName">The distinguished name of the entry</param>
        /// <returns>The directory entry associated with the provided 
        /// distinguished name, or null if no result was found.</returns>
        public static DirectoryEntry CreateFrom(string distinguishedName)
        {
            if (distinguishedName == null)
            {
                throw new ArgumentNullException("distinguishedName");
            }

            if (!distinguishedName.StartsWith(NamingContext.LdapProtocolSpecifier))
            {
                distinguishedName = NamingContext.LdapProtocolSpecifier + distinguishedName;
            }

            return new DirectoryEntry(distinguishedName);
        }

        /// <summary>
        /// Gets the directory entry with the specified object GUID.
        /// </summary>
        /// <param name="value">The GUID of the entry to retrieve</param>
        /// <returns>The directory entry of the object with the specified GUID,
        /// or null if the GUID does not exist in the directory.</returns>
        public static DirectoryEntry CreateFrom(Guid value)
        {
            if (value == null)
            {
                throw new ArgumentNullException("objectGuid");
            }

            return CreateFrom(String.Format("<GUID={0}>", value.ToString("D")));
        }

        /// <summary>
        /// Gets the directory entry associated with the specified <see cref="DirectoryEntity"/>.
        /// </summary>
        /// <param name="entity">The <see cref="DirectoryEntity"/> to search for.</param>
        /// <returns>
        /// The <see cref="DirectoryEntry"/> of the specified <see cref="DirectoryEntity"/>, or null if no directory 
        /// entry could be found.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// Both the <see cref="M:DirectoryObject.DirectoryGuid"/> and <see cref="M:DirectoryObject.DistinguishedName"/> 
        /// properties of <paramref name="entity"/> are unset.
        /// </exception>
        public static DirectoryEntry CreateFrom(DirectoryObject entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }

            if (entity.DirectoryGuid == null)
            {
                if (String.IsNullOrEmpty(entity.DistinguishedName))
                {
                    throw new ArgumentException(ErrorMessages.DirectoryIdentifersRequired, "entity");
                }
                else
                {
                    return CreateFrom(entity.DistinguishedName);
                }
            }
            else
            {
                return CreateFrom(entity.DirectoryGuid);
            }
        }

        /// <summary>
        /// Gets the <see cref="DirectoryEntry"/> associated with the specified <see cref="Principal"/>.
        /// </summary>
        /// <param name="principal">The <see cref="Principal"/> to retrieve a <see cref="DirectoryEntry"/> for.</param>
        /// <returns>The <see cref="DirectoryEntry"/> associated with the specified <see cref="Principal"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="principal"/> is null.</exception>
        /// <exception cref="ArgumentException">
        /// The principal's context does not support GUIDs or the principal has not been instantiated.
        /// </exception>
        [DirectoryServicesPermission(SecurityAction.LinkDemand)]
        public static DirectoryEntry CreateFrom(Principal principal)
        {
            if (principal == null)
            {
                throw new ArgumentNullException("principal");
            }

            if (principal.Guid.HasValue)
            {
                return CreateFrom(principal.Guid.Value);
            }
            else
            {
                throw new ArgumentException(
                    "The principal's context does not support GUIDs, or the principal has not been instantiated.",
                    "principal");
            }
        }

        /// <summary>
        /// Gets the first directory entry matching the specfied unique DirectiveID property 
        /// and unique DirectiveID.
        /// </summary>
        /// <param name="uniqueIdProperty">The property that contains a unique
        /// identifier attribute</param>
        /// <param name="uniqueId">The unique DirectiveID for this entry</param>
        /// <returns>The first directory entry matching the criteria, or null
        /// if no entry is found.</returns>
        [DirectoryServicesPermission(SecurityAction.LinkDemand)]
        public static DirectoryEntry CreateFrom(string uniqueIdProperty, string uniqueId)
        {
            return CreateFrom(uniqueIdProperty, uniqueId, NamingContext.DefaultNamingContext);
        }

        /// <summary>
        /// Gets the first directory entry matching the specfied unique DirectiveID property 
        /// and unique DirectiveID.
        /// </summary>
        /// <param name="uniqueIdProperty">The property that contains a unique
        /// identifier attribute</param>
        /// <param name="uniqueId">The unique DirectiveID for this entry</param>
        /// <param name="defaultNamingContext">The default naming context to search</param>
        /// <param name="objectCategories">The categories of object to search for</param>
        /// <returns>/// The first directory entry matching the criteria, or null if no entry is found.</returns>
        [DirectoryServicesPermission(SecurityAction.LinkDemand)]
        public static DirectoryEntry CreateFrom(
            string uniqueIdProperty, 
            string uniqueId, 
            string defaultNamingContext, 
            params string[] objectCategories)
        {
            if (String.IsNullOrEmpty(uniqueId))
            {
                throw new ArgumentNullException("uniqueIdProperty");
            }

            if (String.IsNullOrEmpty(uniqueId))
            {
                throw new ArgumentNullException("uniqueId");
            }

            DirectoryQuery query = new DirectoryQuery()
            {
                BaseDirectoryPath = defaultNamingContext,
                Scope = SearchScope.Subtree,
                Filter = new QueryFilter(null, uniqueIdProperty, uniqueId, objectCategories)
                ////SearchAttribute = uniqueIdProperty,
                ////SearchMask = uniqueId
            };

            ////query.ObjectCategories.AddRange(objectCategories);
            query.Properties.AddRange(ObjectProperty.CommonProperties);

            using (SearchResultCollection results = DirectoryQuery.QueryDirectoryServices(query))
            {
                if (results == null || results.Count == 0)
                {
                    return null;
                }
                else if (results.Count == 1)
                {
                    return results[0].GetDirectoryEntry();
                }
                else
                {
                    string message =
                        String.Format(
                            "Unique ID property '{0}' returned multiple results. Select a property that is actually unique.",
                            uniqueIdProperty);

                    throw new ArgumentException(message, "uniqueIdProperty");
                }
            }
        }
    }
}
