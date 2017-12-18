namespace SAF.UserInterface.Controls
{
    using System;

    using SAF.Data.Integration;
    using SAF.UserInterface;

    using UserControl = System.Windows.Controls.UserControl;

    /// <summary>
    /// Interaction logic for DataStoreImporterControl.xaml
    /// </summary>
    public partial class DataIntegrationControl : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DataIntegrationControl"/> class with the specified 
        /// <see cref="IIntegrationController"/>.
        /// </summary>
        /// <param name="viewModel">The <see cref="IDataIntegrationViewModel"/> that this control will manage.</param>
        public DataIntegrationControl(IDataIntegrationViewModel viewModel)
        {
            if (viewModel == null)
            {
                throw new ArgumentNullException("viewModel");
            }

            this.InitializeComponent();
            this.DataContext = viewModel;
            this.TaskEnginePanel.Children.Add(new TaskMonitorControl(viewModel.ConverterMonitor));
            this.TaskEnginePanel.Children.Add(new TaskMonitorControl(viewModel.UpdateMonitor));
        }
    }
}
