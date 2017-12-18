namespace SAF.UserInterface
{
    using System;
    using System.Collections.Generic;
    using System.Windows.Forms;
    using System.Windows.Threading;

    /// <summary>
    /// Provides thread-safe access to a Windows Forms folder path dialog.
    /// </summary>
    public class FolderPathDialog : PathDialog<OpenFolderDirective>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FolderPathDialog"/> class.
        /// </summary>
        /// <param name="windowDispatcher">
        /// The window dispatcher containing the thread context in which the dialog must be opened.
        /// </param>
        public FolderPathDialog(Dispatcher windowDispatcher)
            : base(windowDispatcher)
        {
        }

        /// <summary>
        /// Retrieves a folder path usnig the System.Windows.Forms folder path dialog.
        /// </summary>
        /// <param name="directive">The <see cref="OpenFolderDirective"/> associated with the path request.</param>
        /// <returns>The path selected by the user, or null if the user did not select a path.</returns>
        protected override IEnumerable<string> OpenDialog(OpenFolderDirective directive)
        {
            if (directive == null)
            {
                throw new ArgumentNullException("directive");
            }

            using (FolderBrowserDialog folderDialog = new FolderBrowserDialog())
            {
                folderDialog.ShowNewFolderButton = false;
                folderDialog.Description = directive.Description;
                DialogResult result = folderDialog.ShowDialog();

                if (result == DialogResult.OK && !string.IsNullOrEmpty(folderDialog.SelectedPath))
                {
                    string selectedPath = folderDialog.SelectedPath;
                    this.RootPath = selectedPath;
                    return new string[] { selectedPath };
                }
                else
                {
                    return new List<string>();
                }
            }
        }
    }
}
