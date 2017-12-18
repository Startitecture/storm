// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WmiProxy.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// <summary>
//   Acts as a proxy for a local WMI service.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.LocalMachine
{
    using System;
    using System.Linq;
    using System.Management;

    using SAF.Data.Persistence;

    /// <summary>
    /// Acts as a proxy for a local WMI service.
    /// </summary>
    public sealed class WmiProxy : DataProxy<WmiQuery, WmiResult>
    {
        /// <summary>
        /// The local scope for this proxy.
        /// </summary>
        private readonly ManagementScope managementScope = WmiQuery.LocalManagementScope.Clone();

        /// <summary>
        /// Loads data into the data store.
        /// </summary>
        /// <param name="dataSource">The <see cref="WmiQuery"/> to use as a data source.</param>
        protected override void EmitItems(WmiQuery dataSource)
        {
            if (dataSource == null)
            {
                throw new ArgumentNullException("dataSource");
            }

            if (!this.managementScope.IsConnected)
            {
                this.managementScope.Connect();
            }

            ObjectQuery oq = WmiQuery.CreateClassQuery(dataSource.ObjectClass);

            using (var searcher = new ManagementObjectSearcher(this.managementScope, oq))
            {
                using (ManagementObjectCollection moc = searcher.Get())
                {
                    var managementObjects =
                        moc.Cast<ManagementObject>()
                            .Where(
                                mgmtObj =>
                                WmiQuery.HasUniqueProperty(dataSource, mgmtObj) && WmiQuery.HasRequiredProperties(dataSource, mgmtObj));

                    foreach (var mgmtObj in managementObjects)
                    {
                        this.EmitItem(new WmiResult(dataSource, WmiQuery.RetrieveProperties(dataSource, mgmtObj)));
                    }
                }
            }
        }
    }
}
