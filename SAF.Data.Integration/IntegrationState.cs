namespace SAF.Data.Integration
{
    /// <summary>
    /// Specifies the state of a source update.
    /// </summary>
    public enum IntegrationState
    {
        /// <summary>
        /// The process has not yet started.
        /// </summary>
        NotStarted = 0,

        /// <summary>
        /// The target is being prepared for the update.
        /// </summary>
        PreparingTarget = 3,

        /// <summary>
        /// The target has been prepared for the update.
        /// </summary>
        TargetPrepared = 4,

        /// <summary>
        /// The update has started.
        /// </summary>
        IntegrationStarted = 6,

        /// <summary>
        /// The update has completed.
        /// </summary>
        IntegrationCompleted = 7,

        /// <summary>
        /// The target is finalizing.
        /// </summary>
        FinalizingTarget = 8,

        /// <summary>
        /// The data integration process has stopped.
        /// </summary>
        Stopped = 11
    }
}
