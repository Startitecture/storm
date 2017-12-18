// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PathDialog.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// <summary>
//   Provides thread-safe access to a path dialog.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.UserInterface
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows.Threading;

    /// <summary>
    /// Provides thread-safe access to a path dialog.
    /// </summary>
    /// <typeparam name="TDirective">
    /// The type of directive associated with retrieving a path.
    /// </typeparam>
    public abstract class PathDialog<TDirective> : IPathDialog<TDirective>
    {
        /// <summary>
        /// The window dispatcher associated with this dialog.
        /// </summary>
        private readonly Dispatcher windowDispatcher;

        /// <summary>
        /// Initializes a new instance of the <see cref="PathDialog{TDirective}"/> class. 
        /// Initializes a new instance of the <see cref="T:SAF.UserInterface.PathDialog`1"/> class.
        /// </summary>
        /// <param name="windowDispatcher">
        /// The window dispatcher containing the thread context in which the dialog must be opened.
        /// </param>
        protected PathDialog(Dispatcher windowDispatcher)
        {
            this.windowDispatcher = windowDispatcher;
        }

        /// <summary>
        /// The delegate that executes the path query.
        /// </summary>
        /// <param name="directive">The type of directive associated with the path query.</param>
        /// <returns>The path selected as the result of the query.</returns>
        private delegate IEnumerable<string> PathQueryDelegate(TDirective directive);

        /// <summary>
        /// Occurs when the path dialog returns a result.
        /// </summary>
        public event EventHandler<PathEventArgs> PathDialogReturned;

        /// <summary>
        /// Gets or sets the path to initiate the dialog at.
        /// </summary>
        public string RootPath { get; set; }

        /// <summary>
        /// Retrieves the path from the dialog.
        /// </summary>
        /// <param name="directive">
        /// The directive associated with the dialog.
        /// </param>
        /// <returns>
        /// The path retrieved by the dialog if the result indicates the user chose a path; otherwise null.
        /// </returns>
        public IEnumerable<string> GetPaths(TDirective directive)
        {
            var fileQuery = new PathQueryDelegate(this.OpenDialog);
            var paths = new List<string>();
            
            var returnedPaths = this.windowDispatcher.Invoke(fileQuery, directive) as IEnumerable<string>;

            if (returnedPaths != null)
            {
                paths.AddRange(returnedPaths);
            }

            this.OnPathDialogReturned(new PathEventArgs(paths.Any(), paths));

            return paths;
        }

        /// <summary>
        /// Override this method to return a path as a string as the result of a dialog.
        /// </summary>
        /// <param name="directive">
        /// The directive associated with the dialog request.
        /// </param>
        /// <returns>
        /// The path retrieved by the dialog if the result indicates the user chose a path; otherwise null.
        /// </returns>
        protected abstract IEnumerable<string> OpenDialog(TDirective directive);

        /// <summary>
        /// Triggers the <see cref="M:PathDialogReturned"/> event.
        /// </summary>
        /// <param name="e">
        /// The <see cref="PathEventArgs"/> to associate with the event.
        /// </param>
        private void OnPathDialogReturned(PathEventArgs e)
        {
            var temp = this.PathDialogReturned;

            if (temp != null)
            {
                temp(this, e);
            }
        }
    }
}
