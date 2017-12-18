namespace SAF.UserInterface
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Contains data related to a path event.
    /// </summary>
    public class PathEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PathEventArgs"/> class.
        /// </summary>
        /// <param name="userAccepted">A value indicating whether the user accepted the path.</param>
        /// <param name="paths">The path associated with the event.</param>
        public PathEventArgs(bool userAccepted, IEnumerable<string> paths)
        {
            this.UserAccepted = userAccepted;
            this.Paths = new List<string>(paths);
        }

        /// <summary>
        /// Gets a value indicating whether the user accepted the path.
        /// </summary>
        public bool UserAccepted { get; private set; }

        /// <summary>
        /// Gets the patch associated with the event.
        /// </summary>
        public IEnumerable<string> Paths { get; private set; }
    }
}
