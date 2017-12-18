namespace SAF.UserInterface
{
    using System;
    using System.Globalization;
    using System.Windows.Data;

    using SAF.Data.Integration;

    /// <summary>
    /// Converts process states into status strings.
    /// </summary>
    [ValueConversion(typeof(Enum), typeof(string))]
    public class DataIntegrationStateStatusConverter : IValueConverter
    {
        /// <summary>
        /// Converts a DataIntegrationState into a status message.
        /// </summary>
        /// <param name="value">The DataIntegrationState to convert.</param>
        /// <param name="targetType">The type to convert to.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>Status text that describes the DataIntegrationState.</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is IntegrationState)
            {
                switch ((IntegrationState)value)
                {
                    case IntegrationState.NotStarted:
                        return "Ready.";

                    case IntegrationState.PreparingTarget:
                        return "Preparing target...";

                    case IntegrationState.TargetPrepared:
                        return "Target prepared.";

                    case IntegrationState.IntegrationStarted:
                        return "Updating...";

                    case IntegrationState.IntegrationCompleted:
                        return "Update complete.";

                    case IntegrationState.FinalizingTarget:
                        return "Finalizing target...";

                    case IntegrationState.Stopped:
                        return "Finished.";
                }
            }

            return String.Empty;
        }

        /// <summary>
        /// Returns null. Conversion of status strings back into enumerations is not 
        /// supported.
        /// </summary>
        /// <param name="value">The status text.</param>
        /// <param name="targetType">The type to convert to.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>Null, as this conversion type does not support converting back.</returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
