// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LimitedResourceQueuingPolicy.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace SAF.MessageQueuing
{
    using System;
    using System.Linq;

    using SAF.Core;
    using SAF.Observer;
    using SAF.StringResources;

    /// <summary>
    /// A queuing policy that limits queue creation based on a limited resource model. The policy considers CPU usage, queue response 
    /// time, queue failure rate and queue length.
    /// </summary>
    public class LimitedResourceQueuingPolicy : IQueuingPolicy
    {
        #region Static Fields

        /// <summary>
        /// The non aborted queue predicate.
        /// </summary>
        private static readonly Func<IPriorityQueueState, bool> NonAbortedQueuePredicate = x => !x.QueueAborted;

        /// <summary>
        /// The request latency selector.
        /// </summary>
        private static readonly Func<IPriorityQueueState, TimeSpan> RequestLatencySelector = x => x.AverageRequestLatency;

        /// <summary>
        /// The response latency selector.
        /// </summary>
        private static readonly Func<IPriorityQueueState, TimeSpan> ResponseLatencySelector = x => x.AverageResponseLatency;

        /// <summary>
        /// The total seconds selector.
        /// </summary>
        private static readonly Func<TimeSpan, double> TotalSecondsSelector = x => x.TotalSeconds;

        /// <summary>
        /// The default queuing policy.
        /// </summary>
        private static readonly LimitedResourceQueuingPolicy DefaultQueuingPolicy = new LimitedResourceQueuingPolicy(PerformanceMonitor.Current);

        #endregion

        #region Fields

        /// <summary>
        /// The performance monitor.
        /// </summary>
        private readonly IPerformanceMonitor performanceMonitor;

        /// <summary>
        /// The max CPU allowable for queue creation.
        /// </summary>
        private double maxProcessorUsage;

        /////// <summary>
        /////// The max failure rate.
        /////// </summary>
        ////private double maxFailureRate;

        /// <summary>
        /// The min queue count.
        /// </summary>
        private int minQueueCount;

        /// <summary>
        /// The max queue count.
        /// </summary>
        private int maxQueueCount;

        #endregion

        #region Constructors and Destructors

        /////// <summary>
        /////// Initializes a new instance of the <see cref="LimitedResourceQueuingPolicy"/> class.
        /////// </summary>
        ////public LimitedResourceQueuingPolicy()
        ////    : this(Resolve<IPerformanceMonitor>.Default.For(() => PerformanceMonitor.Current))
        ////{
        ////}

        /// <summary>
        /// Initializes a new instance of the <see cref="LimitedResourceQueuingPolicy"/> class.
        /// </summary>
        /// <param name="performanceMonitor">
        /// The performance monitor.
        /// </param>
        public LimitedResourceQueuingPolicy(IPerformanceMonitor performanceMonitor)
        {
            this.performanceMonitor = performanceMonitor;
            this.MaxProcessorUsage = Evaluate.GetConfiguredValue(this, policy => policy.MaxProcessorUsage, 75, Double.Parse);
            this.MinQueueCount = Evaluate.GetConfiguredValue(this, policy => policy.MinQueueCount, 1, Int32.Parse);
            this.MaxQueueCount = Evaluate.GetConfiguredValue(
                this, policy => policy.MaxQueueCount, Environment.ProcessorCount * 3, Int32.Parse);
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the default queuing policy.
        /// </summary>
        public static LimitedResourceQueuingPolicy Default
        {
            get
            {
                return DefaultQueuingPolicy;
            }
        }

        /// <summary>
        /// Gets or sets the maximum CPU usage allowable for queue creation. If average CPU usage is greater than this number then 
        /// queue creation will be denied.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="value"/> is not between 0 and 100 inclusive.
        /// </exception>
        public double MaxProcessorUsage
        {
            get
            {
                return this.maxProcessorUsage;
            }

            set
            {
                if (value > 100 || value < 0)
                {
                    throw new ArgumentOutOfRangeException(
                        "value", String.Format(ValidationMessages.ValueMustBeBetweenInclusive, 0, 100));
                }

                this.maxProcessorUsage = value;
            }
        }

        /// <summary>
        /// Gets or sets the minimum queue count. Regardless of other factors, at least this many queues are allowed.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="value"/> is less than one.
        /// </exception>
        public int MinQueueCount
        {
            get
            {
                return this.minQueueCount;
            }

            set
            {
                if (value < 1)
                {
                    throw new ArgumentOutOfRangeException("value", ValidationMessages.MinQueueCountLessThanOne);
                }

                this.minQueueCount = value;
            }
        }

        /// <summary>
        /// Gets or sets the minimum queue count. Regardless of other factors, at least this many queues are allowed.
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

                if (value < this.minQueueCount)
                {
                    throw new ArgumentOutOfRangeException("value", ValidationMessages.MinQueueCountGreaterThanMaxQueueCount);
                }

                this.maxQueueCount = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to trim idle queues. If true, only one idle queue will be allowed to remain in the
        /// idle queue collection for the queue pool.
        /// </summary>
        public bool TrimIdleQueues { get; set; }

        #endregion

        #region Public Methods and Operators

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

            var activeQueues = poolState.PoolStates.Where(NonAbortedQueuePredicate).ToList();
            var queueName = poolState.Name;
            var nextQueueCount = activeQueues.Count + 1;

            if (poolState.QueueCount < this.MinQueueCount)
            {
                var reason = String.Format(ActionMessages.AllowQueueForMinCount, queueName, nextQueueCount, this.minQueueCount);
                return new PolicyDecision(true, reason);
            }

            if (poolState.QueueCount >= this.MaxQueueCount)
            {
                var reason = String.Format(
                    ActionMessages.DenyQueueCreationForMaxCount, 
                    queueName, 
                    poolState.QueueCount, 
                    this.maxQueueCount);

                return new PolicyDecision(false, reason);
            }

            var requestLatency = activeQueues.Any() ? activeQueues.Select(RequestLatencySelector).Average(TotalSecondsSelector) : 0;
            var responseLatency = activeQueues.Any()
                                       ? activeQueues.Select(ResponseLatencySelector).Average(TotalSecondsSelector)
                                       : TimeSpan.MaxValue.TotalSeconds;

            if (responseLatency < requestLatency)
            {
                var reason = String.Format(
                    ActionMessages.DenyQueueCreationForResponseLatency, 
                    poolState.Name, 
                    nextQueueCount, 
                    requestLatency, 
                    responseLatency);

                return new PolicyDecision(false, reason);
            }

            double cpuAverage = this.performanceMonitor.ProcessorUsageSamples.Any()
                                    ? this.performanceMonitor.ProcessorUsageSamples.Average()
                                    : 0;

            if (cpuAverage > this.MaxProcessorUsage)
            {
                var reason = String.Format(ActionMessages.DenyQueueCreationForResourcesCpu, queueName, nextQueueCount, cpuAverage);
                return new PolicyDecision(false, reason);
            }
            else
            {
                var reason = String.Format(
                    ActionMessages.AllowQueueCreationForResponseLatency, 
                    queueName, 
                    nextQueueCount, 
                    requestLatency, 
                    responseLatency);

                return new PolicyDecision(true, reason);
            }
        }

        #endregion
    }
}