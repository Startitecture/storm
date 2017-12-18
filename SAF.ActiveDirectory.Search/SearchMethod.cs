namespace SAF.ActiveDirectory.Search
{
    /// <summary>
    /// Search method for searching Active Directory.
    /// </summary>
    public enum SearchMethod
    {
        /// <summary>
        /// DirectoryServices API search method.
        /// </summary>
        DirectoryServices = 0,

        /// <summary>
        /// Native LDAP search method.
        /// </summary>
        Ldap = 1
    }
}
