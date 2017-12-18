// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GroupManagement.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// <summary>
//   Provides static properties and methods for Active Directory group management.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.ActiveDirectory
{
    using System;
    using System.DirectoryServices.AccountManagement;
    using System.Security.Permissions;

    /// <summary>
    /// Provides static properties and methods for Active Directory group management.
    /// </summary>
    public static class GroupManagement
    {
        /// <summary>
        /// Creates directory security principals.
        /// </summary>
        private static readonly Factories.DirectorySecurityPrincipalFactory PrincipalFactory =
            new Factories.DirectorySecurityPrincipalFactory();

        /// <summary>
        /// Creates a group in Active Directory.
        /// </summary>
        /// <param name="container">The container to place the group in.</param>
        /// <param name="name">The name of the group.</param>
        /// <param name="description">A description of the new group.</param>
        /// <param name="groupScope">The <see cref="GroupScope"/> of the new group.</param>
        /// <param name="createAsSecurityGroup">A value indicating whether to create a security group (true) or a 
        /// distribution group (false).</param>
        /// <returns>A <see cref="DirectoryEntity"/> representing the newly created group.</returns>
        [SecurityPermission(SecurityAction.LinkDemand)]
        public static DirectorySecurityPrincipal CreateGroup(
            string container, 
            string name, 
            string description,
            GroupScope groupScope,
            bool createAsSecurityGroup)
        {
            using 
                (PrincipalContext context = 
                    new PrincipalContext(ContextType.Domain, DomainNameContext.DomainName, container))
            {
                using (GroupPrincipal group = new GroupPrincipal(context, name))
                {
                    group.DisplayName = name;
                    group.Description = description;
                    group.GroupScope = groupScope;
                    group.IsSecurityGroup = createAsSecurityGroup;
                    group.Name = name;
                    group.Save();

                    return PrincipalFactory.CreateFrom(group);
                }
            }

            ////string newGroupPath = 
            ////    String.Format(
            ////        "LDAP://CN={0},{1}", name, parent.Properties[Schema.DistinguishedNameAttribute].Value);

            ////if (!DirectoryEntry.Exists(newGroupPath))
            ////{
            ////    DirectoryEntry group = parent.Children.Add("CN=" + name, "group");
            ////    group.Properties[Schema.SamAccountNameAttribute].Value = name;
            ////    group.CommitChanges();
            ////    return group;
            ////}
            ////else 
            ////{
            ////    Trace.TraceInformation("'{0}' already exists", newGroupPath); 
            ////    return new DirectoryEntry(newGroupPath);
            ////}
        }

        /// <summary>
        /// Adds a principal to a group.
        /// </summary>
        /// <param name="group">The group to add the principal to.</param>
        /// <param name="principal">The principal to add.</param>
        [SecurityPermission(SecurityAction.LinkDemand)]
        public static void AddToGroup(GroupPrincipal group, Principal principal)
        {
            if (group == null)
            {
                throw new ArgumentNullException("group");
            }

            if (principal == null)
            {
                throw new ArgumentNullException("principal");
            }

            if (!group.Members.Contains(principal))
            {
                group.Members.Add(principal);
                group.Save();
            }

            ////string objPath = (string)principal.Properties[Schema.DistinguishedNameAttribute].Value;

            ////if (!group.Properties[Schema.MemberAttribute].Contains(objPath))
            ////{
            ////    group.Properties[Schema.MemberAttribute].Add(objPath);
            ////    group.CommitChanges();
            ////}
            ////else
            ////{
            ////    Trace.TraceInformation("'{0}' is already a member of '{1}'", objPath, group.Path);
            ////}
        }

        /// <summary>
        /// Removes an object from a group.
        /// </summary>
        /// <param name="group">The group to remove the principal from.</param>
        /// <param name="principal">The principal to remove.</param>
        [SecurityPermission(SecurityAction.LinkDemand)]
        public static void RemoveFromGroup(GroupPrincipal group, Principal principal)
        {
            if (group == null)
            {
                throw new ArgumentNullException("group");
            }

            if (principal == null)
            {
                throw new ArgumentNullException("principal");
            }

            if (group.Members.Contains(principal))
            {
                group.Members.Remove(principal);
                group.Save();
            }

            ////string objPath = Search.GetProperty(principal, Schema.DistinguishedNameAttribute);
            ////string groupPath = Search.GetProperty(group, Schema.DistinguishedNameAttribute);

            ////if (principal.Properties[Schema.MemberOfAttribute].Contains(groupPath))
            ////{
            ////    Trace.TraceInformation(
            ////        "Removing '{0}' [{1}] from office group '{2}' [{3}]", 
            ////        principal.Path, 
            ////        principal.Guid, 
            ////        group.Path, 
            ////        group.Guid);

            ////    group.Properties[Schema.MemberAttribute].Remove(objPath);
            ////    group.CommitChanges();
            ////}
            ////else
            ////{
            ////    Trace.TraceInformation("'{0}' is not a member of '{1}'", objPath, group.Path);
            ////}
        }
    }
}
