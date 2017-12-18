namespace SAF.LocalMachine
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Linq;

    using Microsoft.Win32;

    using SAF.Data;

    /// <summary>
    /// Contains instructions for querying a local registry.
    /// </summary>
    public class RegistryQuery : DataSource
    {
        /// <summary>
        /// Retrieves product installation information from the registry.
        /// </summary>
        public static readonly RegistryQuery ProductQuery = InitializeProductQuery();

        /// <summary>
        /// Initializes a new instance of the <see cref="RegistryQuery"/> class.
        /// </summary>
        public RegistryQuery()
            : base("Registry Query")
        {
            this.IncludeValueNames = new StringCollection();
            this.RequiredValueNames = new StringCollection();
            this.KeyValueExclusions = new RegistryValueCollection();
        }

        /// <summary>
        /// Gets the location of the registry query.
        /// </summary>
        public override string Location
        {
            get { return this.SubkeyPath; }
        }

        /// <summary>
        /// Gets the last modified date of the file.
        /// </summary>
        public override DateTimeOffset LastModified
        {
            get { return DateTimeOffset.Now; }
        }

        /// <summary>
        /// Gets or sets the hive to search.
        /// </summary>
        public RegistryHive Hive { get; set; }

        /// <summary>
        /// Gets or sets the subkey to query.
        /// </summary>
        public string SubkeyPath { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to search subkeys (one level only) of the specified <see cref="SubkeyPath"/> 
        /// (true) or the subkey itself (false).
        /// </summary>
        public bool SearchSubkeys { get; set; }

        /// <summary>
        /// Gets a list of values to include in output.
        /// </summary>
        public StringCollection IncludeValueNames { get; private set; }

        /// <summary>
        /// Gets a list of required values.
        /// </summary>
        public StringCollection RequiredValueNames { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating whether all names in <see cref="RequiredValueNames"/> must be present (true)
        /// or only one of them (false).
        /// </summary>
        public bool RequireAllRequiredValues { get; set; }

        /// <summary>
        /// Gets a collection of registry keys to exclude if they contain certain values.
        /// </summary>
        public RegistryValueCollection KeyValueExclusions { get; private set; }

        /// <summary>
        /// Retrieves a <see cref="RegistryKey"/> based on the specified <see cref="RegistryHive"/>.
        /// </summary>
        /// <param name="hive">The <see cref="RegistryHive"/> to retrieve a <see cref="RegistryKey"/> for.</param>
        /// <returns>A <see cref="RegistryKey"/> for the specified <see cref="RegistryHive"/>.</returns>
        public static RegistryKey ConvertHiveToKey(RegistryHive hive)
        {
            switch (hive)
            {
                case RegistryHive.ClassesRoot:
                    return Registry.ClassesRoot;

                case RegistryHive.CurrentConfig:
                    return Registry.CurrentConfig;

                case RegistryHive.CurrentUser:
                    return Registry.CurrentUser;

                case RegistryHive.LocalMachine:
                    return Registry.LocalMachine;

                case RegistryHive.PerformanceData:
                    return Registry.PerformanceData;

                case RegistryHive.Users:
                    return Registry.Users;

                default:
                    return null;
            }
        }

        /// <summary>
        /// Generates a list of <see cref="RegistryValue"/>s that can be used to create an exclusion list.
        /// </summary>
        /// <param name="valueName">The name of the value.</param>
        /// <param name="values">The values, that if present, should exclude the key from being processed.</param>
        /// <returns>A list of <see cref="RegistryValue"/>s matching the specified value names and values.</returns>
        public static IEnumerable<RegistryValue> GenerateKeyValueExclusions(string valueName, params object[] values)
        {
            return from x in values
                   select new RegistryValue() { Name = valueName, Data = x };
        }

        /// <summary>
        /// Queries the local computer using the specified <see cref="RegistryQuery"/>.
        /// </summary>
        /// <param name="query">The query to use.</param>
        /// <returns>
        /// A collection of <see cref="RegistryValueCollection"/>s containing the values of the queried keys.
        /// </returns>
        public static IEnumerable<RegistryValueCollection> QueryLocal(RegistryQuery query)
        {
            if (query == null)
            {
                throw new ArgumentNullException("query");
            }

            List<RegistryValueCollection> keys = new List<RegistryValueCollection>();

            try
            {
                using (RegistryKey hive = RegistryQuery.ConvertHiveToKey(query.Hive),
                                   source = hive.OpenSubKey(query.SubkeyPath))
                {
                    if (query.SearchSubkeys)
                    {
                        foreach (string subKeyName in source.GetSubKeyNames())
                        {
                            try
                            {
                                using (RegistryKey subKey = source.OpenSubKey(subKeyName))
                                {
                                    RegistryValueCollection values = RetrieveValues(query, subKey);

                                    if (values != null)
                                    {
                                        keys.Add(values);
                                    }
                                }
                            }
                            catch (System.Security.SecurityException ex)
                            {
                                System.Diagnostics.Trace.TraceError(
                                    "Unable to retrieve values from {0} {1}: {2}", query.Hive, query.SubkeyPath, ex);
                            }
                            catch (System.IO.IOException ex)
                            {
                                System.Diagnostics.Trace.TraceError(
                                    "Unable to retrieve values from {0} {1}: {2}", query.Hive, query.SubkeyPath, ex);
                            }
                            catch (UnauthorizedAccessException ex)
                            {
                                System.Diagnostics.Trace.TraceError(
                                    "Unable to retrieve values from {0} {1}: {2}", query.Hive, query.SubkeyPath, ex);
                            }
                        }
                    }
                    else
                    {
                        RegistryValueCollection values = RetrieveValues(query, source);

                        if (values != null)
                        {
                            keys.Add(values);
                        }
                    }
                }
            }
            catch (System.Security.SecurityException ex)
            {
                System.Diagnostics.Trace.TraceError(
                    "Unable to retrieve values from {0} {1}: {2}", query.Hive, query.SubkeyPath, ex);
            }
            catch (System.IO.IOException ex)
            {
                System.Diagnostics.Trace.TraceError(
                    "Unable to retrieve values from {0} {1}: {2}", query.Hive, query.SubkeyPath, ex);
            }
            catch (UnauthorizedAccessException ex)
            {
                System.Diagnostics.Trace.TraceError(
                    "Unable to retrieve values from {0} {1}: {2}", query.Hive, query.SubkeyPath, ex);
            }

            return keys;
        }

        /// <summary>
        /// Retrieves values from a <see cref="RegistryKey"/> using the specified <see cref="RegistryQuery"/>.
        /// </summary>
        /// <param name="query">The <see cref="RegistryQuery"/> to use to search the key.</param>
        /// <param name="key">The <see cref="RegistryKey"/> to query.</param>
        /// <returns>A collection of <see cref="RegistryValue"/>s containing the values in the key that matched the 
        /// query, or null if no values matched.</returns>
        /// <remarks>This method ignores the <see cref="M:SearchSubKeys"/> property of the <see cref="RegistryQuery"/>; 
        /// only the provided registry key is searched.</remarks>
        /// <exception cref="System.Security.SecurityException">
        /// The user does not have the permissions required to read from the registry key.
        /// </exception>
        /// <exception cref="ObjectDisposedException">
        /// The <see cref="RegistryKey"/> that contains the specified value is closed (closed keys cannot be accessed).
        /// </exception>
        /// <exception cref="System.IO.IOException">
        /// The <see cref="RegistryKey"/> that contains the specified value has been marked for deletion.
        /// </exception>
        /// <exception cref="UnauthorizedAccessException">
        /// The user does not have the necessary registry rights.
        /// </exception>
        public static RegistryValueCollection RetrieveValues(RegistryQuery query, RegistryKey key)
        {
            if (query == null)
            {
                throw new ArgumentNullException("query");
            }

            if (key == null)
            {
                throw new ArgumentNullException("key");
            }

            RegistryValueCollection keyValues = new RegistryValueCollection();

            foreach (string valueName in key.GetValueNames())
            {
                if
                    (query.IncludeValueNames.Count == 0 ||
                        query.IncludeValueNames.Contains(valueName) ||
                            query.RequiredValueNames.Contains(valueName))
                {
                    keyValues.Add(
                        new RegistryValue()
                        {
                            ParentKey = key.Name,
                            Name = valueName,
                            Data = key.GetValue(valueName),
                            ValueKind = key.GetValueKind(valueName)
                        });
                }
            }

            return keyValues;
        }

        /// <summary>
        /// Determines whether a <see cref="RegistryKey"/> contains the values required in the specified <see cref="RegistryQuery"/>.
        /// </summary>
        /// <param name="query">The <see cref="RegistryQuery"/> containing the required values.</param>
        /// <param name="key">The <see cref="RegistryKey"/> to check.</param>
        /// <returns><c>true</c> if the required values exist as specified; otherwise <c>false</c>.</returns>
        public static bool HasRequiredValues(RegistryQuery query, RegistryKey key)
        {
            if (query == null)
            {
                throw new ArgumentNullException("query");
            }

            if (key == null)
            {
                throw new ArgumentNullException("key");
            }

            if (query.RequiredValueNames.Count > 0)
            {
                List<string> values = new List<string>(key.GetValueNames());
                string[] requiredValues = new string[query.RequiredValueNames.Count];
                query.RequiredValueNames.CopyTo(requiredValues, 0);

                // If all values are required, then check that all value names are present. Otherwise we only need one 
                // value name present.
                bool hasRequired =
                    query.RequireAllRequiredValues ?
                    values.Intersect(requiredValues).Count() == requiredValues.Distinct().Count() :
                    values.Any(x => query.RequiredValueNames.Contains(x));

                return hasRequired;
            }

            return true;
        }

        /// <summary>
        /// Determines whether a <see cref="RegistryKey"/> contains values excluded in the specified <see cref="RegistryQuery"/>.
        /// </summary>
        /// <param name="query">The <see cref="RegistryQuery"/> containing the excluded values.</param>
        /// <param name="key">The <see cref="RegistryKey"/> to check.</param>
        /// <returns><c>true</c> if the excluded values exist as specified; otherwise <c>false</c>.</returns>
        public static bool ContainsExcludedValues(RegistryQuery query, RegistryKey key)
        {
            if (query == null)
            {
                throw new ArgumentNullException("query");
            }

            if (key == null)
            {
                throw new ArgumentNullException("key");
            }

            List<string> values = new List<string>(key.GetValueNames());

            // Check if an excluded value causes a key to be skipped. Multiple values can
            // be defined per key.
            foreach (RegistryValue value in query.KeyValueExclusions)
            {
                string valueString = Convert.ToString(key.GetValue(value.Name));
                string excludeString = Convert.ToString(value.Data);

                if (values.Contains(value.Name) && 0 == String.CompareOrdinal(valueString, excludeString))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Initializes the <see cref="M:ProductQuery"/> field.
        /// </summary>
        /// <returns>A <see cref="RegistryQuery"/> that queries for installed products.</returns>
        private static RegistryQuery InitializeProductQuery()
        {
            RegistryQuery query = new RegistryQuery()
            {
                Hive = RegistryHive.LocalMachine,
                SubkeyPath = @"Software\Microsoft\Windows\CurrentVersion\Uninstall\",
                RequireAllRequiredValues = false,
                SearchSubkeys = true
            };

            query.IncludeValueNames.AddRange(
                new string[] 
                { 
                    "UninstallString", 
                    "Publisher", 
                    "DisplayVersion", 
                    "InstallLocation", 
                    "QuietUninstallString", 
                    "ParentKeyName" 
                });

            query.RequiredValueNames.AddRange(
                new string[] { "DisplayName", "Publisher" });

            return query;
        }
    }
}
