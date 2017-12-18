// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataResourceMonitor.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// <summary>
//   Maintains the status of a data resource.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.Observer
{
    using System;
    using System.Configuration;
    using System.Data.Common;
    using System.Linq;

    using SAF.Core;
    using SAF.Data;

    /// <summary>
    /// Maintains the status of a data resource.
    /// </summary>
    public class DataResourceMonitor : ResourceMonitor
    {
        #region Fields

        /// <summary>
        /// The connection history.
        /// </summary>
        private readonly TimedEventHistory connectionHistory = new TimedEventHistory { MaxRecords = 10 };

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="DataResourceMonitor"/> class.
        /// </summary>
        /// <param name="connection">
        /// The connection.
        /// </param>
        public DataResourceMonitor(string connection)
            : base(connection, TimeSpan.FromMinutes(1))
        {
        }

        #endregion

        /// <summary>
        /// Gets a value indicating whether the service resource is available.
        /// </summary>
        /// <returns>
        /// <c>true</c> if the resource is available; otherwise, <c>false</c>.
        /// </returns>
        protected override StatusResult CheckAvailability()
        {
            var timedEvent = TimedEventHistory.Start();

            try
            {
                using (DbConnection dataConnection = ConfigurationManager.ConnectionStrings.GetConnection(this.Location))
                {
                    dataConnection.Open();
                }

                this.connectionHistory.Stop(timedEvent);
                return new StatusResult { IsAvailable = true };
            }
            catch (DomainException ex)
            {
                // TODO: Invert the control here with ReportIfNewException. Use an ErrorHandler.
                this.connectionHistory.Stop(timedEvent);
                return new StatusResult { IsAvailable = false, StatusError = ex };
            }
            catch (ArgumentException ex)
            {
                this.connectionHistory.Stop(timedEvent);
                return new StatusResult { IsAvailable = false, StatusError = ex };
            }
            catch (DbException ex)
            {
                this.connectionHistory.Stop(timedEvent);
                return new StatusResult { IsAvailable = false, StatusError = ex };
            }
            catch (Exception)
            {
                this.connectionHistory.Stop(timedEvent);
                throw;
            }
        }

        /// <summary>
        /// Calculates the weight of accessing the resource.
        /// </summary>
        /// <returns>
        /// A <see cref="double"/> that represents the relative weight of accessing the resource.
        /// </returns>
        protected override double CalculateWeight()
        {
            return this.connectionHistory.ResponseTimes.Any()
                       ? this.connectionHistory.ResponseTimes.Average(x => x.TotalMilliseconds)
                       : Double.MaxValue;
        }
    }
}