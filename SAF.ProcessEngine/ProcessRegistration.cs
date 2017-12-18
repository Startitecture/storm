namespace SAF.ProcessEngine
{
    /// <summary>
    /// Contains information about a registered process.
    /// </summary>
    internal class ProcessRegistration
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProcessRegistration"/> class with the specified process and
        /// process type.
        /// </summary>
        /// <param name="process">The process to register.</param>
        /// <param name="processType">The type of process to register.</param>
        public ProcessRegistration(IProcessEngine process, ProcessType processType)
        {
            this.Process = process;
            this.ProcessType = processType;
        }

        /// <summary>
        /// Gets the registered process.
        /// </summary>
        public IProcessEngine Process { get; private set; }

        /// <summary>
        /// Gets the type of process registered.
        /// </summary>
        public ProcessType ProcessType { get; private set; }
    }
}
