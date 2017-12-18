namespace SAF.UserInterface
{
    using System;
    using System.Globalization;
    using System.Windows.Data;
    using System.Windows.Media;

    using SAF.ProcessEngine;

    /// <summary>
    /// Converts a <see cref="UserResultState"/> into a brush.
    /// </summary>
    [ValueConversion(typeof(UserResultState), typeof(Brush))]
    public class ProcessResultStateBrushConverter : IValueConverter
    {
        /// <summary>
        /// Converts a ProcessResultState into a foreground brush for a ProgressBar.
        /// </summary>
        /// <param name="value">The ProcessResultState to convert.</param>
        /// <param name="targetType">The type to convert to.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>A brush that visually represents the ProcessResultState.</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }

            if (value is UserResultState)
            {
                switch ((UserResultState)value)
                {
                    case UserResultState.Nominal:
                        return Brushes.DarkGreen;

                    case UserResultState.TaskError:
                        return Brushes.Orange;

                    case UserResultState.ProcessError:
                        return Brushes.Red;
                }
            }

            return Brushes.DarkGreen;
        }

        /// <summary>
        /// Returns null. Conversion of Brushes back into enumerations is not supported.
        /// </summary>
        /// <param name="value">The foreground brush.</param>
        /// <param name="targetType">The type to convert to.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>Null, as this conversion type does not support converting back.</returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
