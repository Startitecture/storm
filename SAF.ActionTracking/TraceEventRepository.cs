// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TraceEventRepository.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace SAF.ActionTracking
{
    using System;
    using System.Diagnostics;

    using SAF.Core;

    /// <summary>
    /// An event repository for tracing events.
    /// </summary>
    public class TraceEventRepository : IEventRepository
    {
        /// <summary>
        /// The information format.
        /// </summary>
        private const string InformationFormat = "[{0}] {1} from {2} by {3} at {4} (took {5})";

        /// <summary>
        /// The error format.
        /// </summary>
        private const string ErrorFormat = "[{0}] {1} from {2} by {3} at {4} (took {5}) failed: {6}";

        /// <summary>
        /// The suppress successful events.
        /// </summary>
        private readonly bool suppressSuccessfulEvents;

        /// <summary>
        /// Initializes a new instance of the <see cref="TraceEventRepository"/> class.
        /// </summary>
        public TraceEventRepository()
            : this(false)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TraceEventRepository"/> class.
        /// </summary>
        /// <param name="suppressSuccessfulEvents">
        /// The suppress successful events.
        /// </param>
        public TraceEventRepository(bool suppressSuccessfulEvents)
        {
            this.suppressSuccessfulEvents = suppressSuccessfulEvents;
        }

        /// <summary>
        /// Gets or sets the connection string. 
        /// </summary>
        public string Connection { get; set; }

        /// <summary>
        /// Gets or sets the amount of time to wait for failing save actions. The repository will retry until this time is reached.
        /// </summary>
        public TimeSpan WaitTimeout { get; set; }

        /// <summary>
        /// Saves an item to the database.
        /// </summary>
        /// <param name="item">
        /// The entity to save.
        /// </param>
        /// <returns>
        /// Returns the <see cref="SAF.ActionTracking.ActionEvent"/> specified in <paramref name="item"/>.
        /// </returns>
        public ActionEvent Save(ActionEvent item)
        {
            if (item == null)
            {
                throw new ArgumentNullException("item");
            }

            if (item.ErrorInfo == ErrorInfo.Empty)
            {
                if (this.suppressSuccessfulEvents == false)
                {
                    Trace.TraceInformation(
                        InformationFormat, 
                        item.Request.GlobalIdentifier, 
                        item.Request.Description, 
                        item.Request.ActionSource, 
                        item.UserAccount, 
                        item.InitiationTime, 
                        item.CompletionTime - item.InitiationTime);
                }
            }
            else
            {
                Trace.TraceError(
                    ErrorFormat, 
                    item.Request.GlobalIdentifier, 
                    item.Request.Description, 
                    item.Request.ActionSource, 
                    item.UserAccount, 
                    item.InitiationTime, 
                    item.CompletionTime - item.InitiationTime, 
                    item.ErrorInfo.ErrorMessage);
            }

            return item;
        }

        /// <summary>
        /// Gets the ID for the specified action identifier.
        /// </summary>
        /// <param name="actionIdentifier">
        /// The action identifier.
        /// </param>
        /// <returns>
        /// The action, or null if no ID is found.
        /// </returns>
        public ActionEvent GetById(Guid actionIdentifier)
        {
            throw new NotSupportedException();
        }
    }
}
