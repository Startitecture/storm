// -----------------------------------------------------------------------
// <copyright file="CommandCollection.cs" company="Microsoft">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace SAF.ProcessEngine
{
    using System.Collections.Generic;

    using SAF.Core;

    /// <summary>
    /// Contains a collection of commands.
    /// </summary>
    public class CommandCollection : SerializableCollection<IExecutable>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CommandCollection"/> class.
        /// </summary>
        public CommandCollection()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandCollection"/> class.
        /// </summary>
        /// <param name="values">The commands for the the collection to execute.</param>
        public CommandCollection(IEnumerable<IExecutable> values)
            : base(values)
        {
        }
    }
}