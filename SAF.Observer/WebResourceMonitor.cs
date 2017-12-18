// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WebResourceMonitor.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// <summary>
//   Maintains the status of a web resource.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.Observer
{
    using System;
    using System.Linq;
    using System.Net;
    using System.Security;

    using SAF.Core;

    /// <summary>
    /// Maintains the status of a web resource.
    /// </summary>
    public class WebResourceMonitor : ResourceMonitor
    {
        #region Static Fields

        /// <summary>
        /// The wait time.
        /// </summary>
        private static readonly TimeSpan WaitTime = new TimeSpan(TimeSpan.TicksPerSecond * 15);

        #endregion

        #region Fields

        /// <summary>
        /// The response times.
        /// </summary>
        private readonly TimedEventHistory requestHistory = new TimedEventHistory { MaxRecords = 30 };

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="WebResourceMonitor"/> class.
        /// </summary>
        /// <param name="location">
        /// The location.
        /// </param>
        public WebResourceMonitor(string location)
            : base(location, TimeSpan.FromSeconds(15))
        {
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets a value indicating whether the service resource is available.
        /// </summary>
        /// <returns>
        /// <c>true</c> if the resource is available; otherwise, <c>false</c>.
        /// </returns>
        protected override StatusResult CheckAvailability()
        {
            var timedEvent = TimedEventHistory.Start();
            HttpWebResponse response;

            try
            {
                WebRequest webRequest = WebRequest.Create(new Uri(this.Location));

                // Math.Truncate ensures the timeout is always less than
                webRequest.Timeout = Convert.ToInt32(Math.Truncate(WaitTime.TotalSeconds)) * 1000;
                webRequest.Credentials = CredentialCache.DefaultCredentials;
                response = webRequest.GetResponse() as HttpWebResponse;
                this.requestHistory.Stop(timedEvent);
            }
            catch (NotSupportedException ex)
            {
                this.requestHistory.Stop(timedEvent);
                return new StatusResult { IsAvailable = false, StatusError = ex };
            }
            catch (SecurityException ex)
            {
                this.requestHistory.Stop(timedEvent);
                return new StatusResult { IsAvailable = false, StatusError = ex };
            }
            catch (ProtocolViolationException ex)
            {
                this.requestHistory.Stop(timedEvent);
                return new StatusResult { IsAvailable = false, StatusError = ex };
            }
            catch (WebException ex)
            {
                this.requestHistory.Stop(timedEvent);
                return new StatusResult { IsAvailable = false, StatusError = ex };
            }
            catch (InvalidOperationException ex)
            {
                this.requestHistory.Stop(timedEvent);
                return new StatusResult { IsAvailable = false, StatusError = ex };
            }
            catch
            {
                this.requestHistory.Stop(timedEvent);
                throw;
            }

            var availability = response != null && response.StatusCode == HttpStatusCode.OK;
            return new StatusResult { IsAvailable = availability };
        }

        /// <summary>
        /// Calculates the weight of accessing the resource.
        /// </summary>
        /// <returns>
        /// A <see cref="double"/> that represents the relative weight of accessing the resource.
        /// </returns>
        protected override double CalculateWeight()
        {
            return this.requestHistory.ResponseTimes.Any()
                       ? this.requestHistory.ResponseTimes.Average(x => x.TotalMilliseconds)
                       : Double.MaxValue;
        }

        #endregion
    }
}