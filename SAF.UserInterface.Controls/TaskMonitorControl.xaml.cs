namespace SAF.UserInterface.Controls
{
    using System.Windows.Controls;

    using SAF.ProcessEngine;

    /// <summary>
    /// Interaction logic for TaskStateViewControl.xaml
    /// </summary>
    public partial class TaskMonitorControl : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TaskMonitorControl"/> class with the specified task engine.
        /// </summary>
        /// <param name="monitor">The engine to view.</param>
        public TaskMonitorControl(TaskMonitor monitor)
        {
            this.InitializeComponent();
            this.DataContext = monitor;
        }
    }
}
