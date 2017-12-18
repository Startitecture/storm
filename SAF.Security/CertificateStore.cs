// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CertificateStore.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// <summary>
//   Defines the CertificateStore type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.Security
{
    using System;
    using System.Security.Cryptography.X509Certificates;

    using SAF.Core;
    using SAF.StringResources;

    /// <summary>
    /// The certificate store.
    /// </summary>
    public static class CertificateStore
    {
        /// <summary>
        /// Loads the specified certificate.
        /// </summary>
        /// <param name="certificateSubjectName">
        /// The certificate subject name.
        /// </param>
        /// <returns>
        /// The <see cref="System.Security.Cryptography.X509Certificates.X509Certificate2"/> in the local machine store that matches the subject name.
        /// </returns>
        /// <exception cref="ApplicationConfigurationException">
        /// The specified certificate could not be found in the local machine store.
        /// </exception>
        public static X509Certificate2 LoadCertificate(string certificateSubjectName)
        {
            return LoadCertificate(certificateSubjectName, StoreName.My);
        }

        /// <summary>
        /// Loads the specified certificate.
        /// </summary>
        /// <param name="certificateSubjectName">
        /// The certificate subject name.
        /// </param>
        /// <param name="storeName">
        /// The enumerated name of the store containing the certificate.
        /// </param>
        /// <returns>
        /// The <see cref="System.Security.Cryptography.X509Certificates.X509Certificate2"/> in the local machine store that matches the subject name.
        /// </returns>
        /// <exception cref="ApplicationConfigurationException">
        /// The specified certificate could not be found in the local machine store.
        /// </exception>
        public static X509Certificate2 LoadCertificate(string certificateSubjectName, StoreName storeName)
        {
            return LoadCertificate(certificateSubjectName, storeName, StoreLocation.LocalMachine);
        }

        /// <summary>
        /// Loads the specified certificate.
        /// </summary>
        /// <param name="certificateSubjectName">
        /// The certificate subject name.
        /// </param>
        /// <param name="storeName">
        /// The enumerated name of the store containing the certificate.
        /// </param>
        /// <param name="storeLocation">
        /// The location of the store containing the certificate.
        /// </param>
        /// <returns>
        /// The <see cref="System.Security.Cryptography.X509Certificates.X509Certificate2"/> in the local machine store that matches the subject name.
        /// </returns>
        /// <exception cref="ApplicationConfigurationException">
        /// The specified certificate could not be found in the local machine store.
        /// </exception>
        public static X509Certificate2 LoadCertificate(string certificateSubjectName, StoreName storeName, StoreLocation storeLocation)
        {
            var store = new X509Store(storeName, storeLocation);
            store.Open(OpenFlags.ReadOnly);
            var matching = store.Certificates.Find(X509FindType.FindBySubjectName, certificateSubjectName, false);

            if (matching.Count == 0)
            {
                throw new ApplicationConfigurationException(
                    String.Format(ErrorMessages.CertificateNotFound, certificateSubjectName),
                    typeof(CertificateStore).Name);
            }

            return matching[0];
        }
    }
}
