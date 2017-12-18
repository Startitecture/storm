// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StaticQueuingPolicy.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace SAF.MessageQueuing
{
    using System;
    using System.Configuration;
    using System.Runtime.Caching;

    using SAF.Core;
    using SAF.StringResources;

    /// <summary>
    /// A queuing policy that allows a maximum number of queues based on configuration or processor count.
    /// </summary>
    /// <remarks>
    /// Maximum limits for specific queues can be configured by adding AppSettings using the naming convention 
    /// {QueueName}.MaxQueueCount and setting the value to the maximum number of queues to allow concurrently.
    /// </remarks>
    public class StaticQueuingPolicy : IQueuingPolicy
    {
        /// <summary>
        /// The static queuing policy.
        /// </summary>
        public static readonly StaticQueuingPolicy DefaultPolicy = new StaticQueuingPolicy();

        /// <summary>
        /// The queue max setting format.
        /// </summary>
        private const string QueueMaxSettingFormat = "{0}.MaxQueueCount";

        /// <summary>
        /// The app setting cache policy.
        /// </summary>
        private static readonly CacheItemPolicy AppSettingCachePolicy = new CacheItemPolicy
                                                                            {
                                                                                SlidingExpiration = TimeSpan.FromMinutes(2)
                                                                            };

        /// <summary>
        /// The memory cache.
        /// </summary>
        private readonly ObjectCache memoryCache = MemoryCache.Default;

        /// <summary>
        /// The max queue count.
        /// </summary>
        private int maxQueueCount;

        /// <summary>
        /// Initializes a new instance of the <see cref="StaticQueuingPolicy"/> class.
        /// </summary>
        public StaticQueuingPolicy()
        {
            this.MaxQueueCount = Environment.ProcessorCount * 3;
        }

        /// <summary>
        /// Gets or sets the maximum queue count.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="value"/> is less than one.
        /// </exception>
        public int MaxQueueCount
        {
            get
            {
                return this.maxQueueCount;
            }

            set
            {
                if (value < 1)
                {
                    throw new ArgumentOutOfRangeException("value", ValidationMessages.MaxQueueCountLessThanOne);
                }

                this.maxQueueCount = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to trim idle queues. If true, only one idle queue will be allowed to remain in the
        /// idle queue collection for the queue pool.
        /// </summary>
        public bool TrimIdleQueues { get; set; }

        /// <summary>
        /// Determines whether queue creation should be allowed based on the current state of the queues.
        /// </summary>
        /// <param name="poolState">
        /// The pool state to examine.
        /// </param>
        /// <returns>
        /// <c>true</c> if creating another queue is allowed by the current policy; otherwise, <c>false</c>.
        /// </returns>
        public PolicyDecision AllowQueueCreation(IObservablePoolState poolState)
        {
            if (poolState == null)
            {
                throw new ArgumentNullException("poolState");
            }

            var queueName = poolState.Name;

            if (poolState.QueueCount == 0)
            {
                return new PolicyDecision(true, String.Format(ActionMessages.AllowQueueCreationForEmptyPool, queueName));
            }

            var configSetting = String.Format(QueueMaxSettingFormat, queueName);
            var cachedValue = this.memoryCache.Get(configSetting);

            if (cachedValue is int)
            {
                this.maxQueueCount = (int)cachedValue;
            }
            else
            {
                var configuredMaxCount = ConfigurationManager.AppSettings.GetValue(configSetting, this.MaxQueueCount, Int32.Parse);
                this.maxQueueCount = Math.Max(1, configuredMaxCount);
                this.memoryCache.Set(configSetting, this.maxQueueCount, AppSettingCachePolicy);
            }

            var allowQueueCreation = poolState.QueueCount < this.maxQueueCount;
            var format = allowQueueCreation
                             ? ActionMessages.AllowQueueCreationForMaxCount
                             : ActionMessages.DenyQueueCreationForMaxCount;

            var reason = String.Format(format, queueName, poolState.QueueCount, this.maxQueueCount);
            return new PolicyDecision(allowQueueCreation, reason);
        }
    }
}