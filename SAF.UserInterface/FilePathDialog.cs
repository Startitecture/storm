namespace SAF.UserInterface
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Windows.Threading;

    using Microsoft.Win32;

    /// <summary>
    /// Provides thread-safe access to a Win32 file path dialog.
    /// </summary>
    public class FilePathDialog : PathDialog<OpenFilesDirective>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FilePathDialog"/> class.
        /// </summary>
        /// <param name="windowDispatcher">
        /// The window dispatcher containing the thread context in which the dialog must be opened.
        /// </param>
        public FilePathDialog(Dispatcher windowDispatcher)
            : base(windowDispatcher)
        {
        }

        /// <summary>
        /// Retrieves a file path using the Win32 provider.
        /// </summary>
        /// <param name="directive">The <see cref="OpenFilesDirective"/> associated with the file path request.</param>
        /// <returns>The path selected by the user, or null if the user did not select a path.</returns>
        protected override IEnumerable<string> OpenDialog(OpenFilesDirective directive)
        {
            if (directive == null)
            {
                throw new ArgumentNullException("directive");
            }

            var types = from ext in directive.Extensions
                        select String.Format("*{0}", ext);

            string rootPath = Directory.Exists(this.RootPath) ? this.RootPath : String.Empty;

            OpenFileDialog fileDialog = 
                new OpenFileDialog()
                {
                    InitialDirectory = rootPath,
                    Multiselect = true,
                    Filter = String.Format("{0}|{1}", directive.Description, String.Join(";", types.ToArray())),
                    DefaultExt = directive.Extensions.Count() > 0 ? directive.Extensions.First() : ".*"
                };

            bool? result = fileDialog.ShowDialog();

            if (result == true)
            {
                this.RootPath =
                    Path.GetDirectoryName(
                        fileDialog.FileNames.FirstOrDefault() ?? fileDialog.FileName ?? Environment.CurrentDirectory);

                return fileDialog.FileNames;
            }
            else
            {
                return new List<string>();
            }
        }
    }
}
