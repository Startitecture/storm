// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Generate.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// <summary>
//   Generates items for tests.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.MessageQueuing.Tests
{
    using System;

    /// <summary>
    /// Generates items for tests.
    /// </summary>
    public static class Generate
    {
        /// <summary>
        /// Creates a priority request.
        /// </summary>
        /// <param name="name">
        /// The name of the request.
        /// </param>
        /// <param name="requestLatencyMillis">
        /// The request latency in milliseconds.
        /// </param>
        /// <returns>
        /// A <see cref="FakeMessage"/>.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// The amount of time left is greater than delay, putting the request time into the future.
        /// </exception>
        public static FakeMessage PriorityMessage(string name, double requestLatencyMillis)
        {
            return PriorityMessage(
                name,
                DateTimeOffset.Now,
                DateTimeOffset.Now.Date.AddDays(2),
                TimeSpan.FromHours(7),
                requestLatencyMillis);
        }

        /// <summary>
        /// Creates a priority request.
        /// </summary>
        /// <param name="name">
        /// The name of the request.
        /// </param>
        /// <param name="deadline">
        /// The deadline.
        /// </param>
        /// <param name="escalationTime">
        /// The escalation Time.
        /// </param>
        /// <returns>
        /// A <see cref="FakeMessage"/>.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// The amount of time left is greater than delay, putting the request time into the future.
        /// </exception>
        public static FakeMessage PriorityMessage(string name, DateTimeOffset deadline, TimeSpan escalationTime)
        {
            return PriorityMessage(name, DateTimeOffset.Now, deadline, escalationTime);
        }

        /// <summary>
        /// Creates a priority request.
        /// </summary>
        /// <param name="name">
        /// The name of the request.
        /// </param>
        /// <param name="requestTime">
        /// The request Time.
        /// </param>
        /// <param name="deadline">
        /// The deadline.
        /// </param>
        /// <param name="escalationTime">
        /// The escalation Time.
        /// </param>
        /// <returns>
        /// A <see cref="FakeMessage"/>.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// The amount of time left is greater than delay, putting the request time into the future.
        /// </exception>
        public static FakeMessage PriorityMessage(string name, DateTimeOffset requestTime, DateTimeOffset deadline, TimeSpan escalationTime)
        {
            return PriorityMessage(name, requestTime, deadline, escalationTime, 5);
        }

        /// <summary>
        /// Creates a priority request.
        /// </summary>
        /// <param name="name">
        /// The name of the request.
        /// </param>
        /// <param name="requestTime">
        /// The request Time.
        /// </param>
        /// <param name="deadline">
        /// The deadline.
        /// </param>
        /// <param name="escalationTime">
        /// The escalation Time.
        /// </param>
        /// <param name="requestLatencyMillis">
        /// The request latency in milliseconds.
        /// </param>
        /// <returns>
        /// A <see cref="FakeMessage"/>.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// The amount of time left is greater than delay, putting the request time into the future.
        /// </exception>
        public static FakeMessage PriorityMessage(
            string name, 
            DateTimeOffset requestTime,
            DateTimeOffset deadline, 
            TimeSpan escalationTime, 
            double requestLatencyMillis)
        {
            return PriorityMessage(name, requestTime, deadline, escalationTime, requestLatencyMillis, false);
        }

        /// <summary>
        /// Creates a priority request.
        /// </summary>
        /// <param name="name">
        /// The name of the request.
        /// </param>
        /// <param name="requestTime">
        /// The request Time.
        /// </param>
        /// <param name="deadline">
        /// The deadline.
        /// </param>
        /// <param name="escalationTime">
        /// The escalation Time.
        /// </param>
        /// <param name="requestLatencyMillis">
        /// The request latency in milliseconds.
        /// </param>
        /// <param name="failRequest">
        /// The fail Request.
        /// </param>
        /// <returns>
        /// A <see cref="FakeMessage"/>.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// The amount of time left is greater than delay, putting the request time into the future.
        /// </exception>
        public static FakeMessage PriorityMessage(
            string name, 
            DateTimeOffset requestTime,
            DateTimeOffset deadline, 
            TimeSpan escalationTime, 
            double requestLatencyMillis,
            bool failRequest)
        {
            return new FakeMessage(requestTime, deadline, escalationTime)
                       {
                           Name = name,
                           RequestLatency = TimeSpan.FromMilliseconds(requestLatencyMillis),
                           RequestShouldFail = failRequest
                       };
        }

        /// <summary>
        /// Creates a priority request.
        /// </summary>
        /// <param name="name">
        /// The name of the request.
        /// </param>
        /// <param name="requestTime">
        /// The request Time.
        /// </param>
        /// <param name="deadline">
        /// The deadline.
        /// </param>
        /// <param name="escalationTime">
        /// The escalation Time.
        /// </param>
        /// <param name="requestLatencyMillis">
        /// The request latency in milliseconds.
        /// </param>
        /// <returns>
        /// A <see cref="FakeMessage"/>.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// The amount of time left is greater than delay, putting the request time into the future.
        /// </exception>
        public static UnhandledExceptionMessage FailureRequest(
            string name, DateTimeOffset requestTime, DateTimeOffset deadline, TimeSpan escalationTime, double requestLatencyMillis)
        {
            return new UnhandledExceptionMessage(requestTime, deadline, escalationTime)
                       {
                           Name = name,
                           RequestLatency = TimeSpan.FromMilliseconds(requestLatencyMillis),
                       };
        }
    }
}
