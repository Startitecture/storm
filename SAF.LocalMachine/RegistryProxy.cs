// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RegistryProxy.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// <summary>
//   Acts as a proxy for a Windows registry.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.LocalMachine
{
    using System;

    using Microsoft.Win32;

    using SAF.Data.Persistence;
    using SAF.ProcessEngine;

    /// <summary>
    /// Acts as a proxy for a Windows registry.
    /// </summary>
    public sealed class RegistryProxy : DataProxy<RegistryQuery, RegistryValueCollection>
    {
        /// <summary>
        /// Extracts data from the data source.
        /// </summary>
        /// <param name="dataSource">The data source to extract data from.</param>
        protected override void EmitItems(RegistryQuery dataSource)
        {
            if (dataSource == null)
            {
                throw new ArgumentNullException("dataSource");
            }

            try
            {
                // TODO: Use RegistryQuery.QueryLocal.
                using 
                    (RegistryKey source = 
                        RegistryQuery.ConvertHiveToKey(dataSource.Hive).OpenSubKey(dataSource.SubkeyPath))
                {
                    if (dataSource.SearchSubkeys && source != null)
                    {
                        foreach (string subKeyName in source.GetSubKeyNames())
                        {
                            this.EmitSubKeyValues(dataSource, source, subKeyName);
                        }
                    }
                    else
                    {
                        this.EmitItem(RegistryQuery.RetrieveValues(dataSource, source));
                    }
                }
            }
            catch (System.Security.SecurityException ex)
            {
                this.OnRetrievalFailed(new FailedItemEventArgs<RegistryQuery>(dataSource, ex));
            }
            catch (ObjectDisposedException ex)
            {
                this.OnRetrievalFailed(new FailedItemEventArgs<RegistryQuery>(dataSource, ex));
            }
            catch (System.IO.IOException ex)
            {
                this.OnRetrievalFailed(new FailedItemEventArgs<RegistryQuery>(dataSource, ex));
            }
            catch (UnauthorizedAccessException ex)
            {
                this.OnRetrievalFailed(new FailedItemEventArgs<RegistryQuery>(dataSource, ex));
            }
        }

        /// <summary>
        /// Emits subkey values of the specified <see cref="RegistryKey"/> using the specified <see cref="RegistryQuery"/>.
        /// </summary>
        /// <param name="query">The <see cref="RegistryQuery"/> to use.</param>
        /// <param name="key">The <see cref="RegistryKey"/> containing the subkey to search.</param>
        /// <param name="subKeyName">The subkey to query.</param>
        private void EmitSubKeyValues(RegistryQuery query, RegistryKey key, string subKeyName)
        {
            try
            {
                using (RegistryKey subKey = key.OpenSubKey(subKeyName))
                {
                    if (!RegistryQuery.ContainsExcludedValues(query, subKey) && RegistryQuery.HasRequiredValues(query, subKey))
                    {
                        this.EmitItem(RegistryQuery.RetrieveValues(query, subKey));
                    }
                }
            }
            catch (System.Security.SecurityException ex)
            {
                this.OnRetrievalFailed(new FailedItemEventArgs<RegistryQuery>(query, ex));
            }
            catch (ObjectDisposedException ex)
            {
                this.OnRetrievalFailed(new FailedItemEventArgs<RegistryQuery>(query, ex));
            }
            catch (System.IO.IOException ex)
            {
                this.OnRetrievalFailed(new FailedItemEventArgs<RegistryQuery>(query, ex));
            }
            catch (UnauthorizedAccessException ex)
            {
                this.OnRetrievalFailed(new FailedItemEventArgs<RegistryQuery>(query, ex));
            }
        }
    }
}
