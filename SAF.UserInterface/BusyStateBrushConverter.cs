namespace SAF.UserInterface
{
    using System;
    using System.Globalization;
    using System.Windows.Data;
    using System.Windows.Media;

    /// <summary>
    /// Converts a boolean value into a busy state brush.
    /// </summary>
    [ValueConversion(typeof(bool), typeof(Brush))]
    public class BusyStateBrushConverter : IValueConverter
    {
        /// <summary>
        /// Converts a boolean value into a busy state brush.
        /// </summary>
        /// <param name="value">The boolean value.</param>
        /// <param name="targetType">The type to convert to.</param>
        /// <param name="parameter">The parameter associated with the conversion.</param>
        /// <param name="culture">The culture to use during conversion.</param>
        /// <returns>The converted object.</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((bool)value)
            {
                return Brushes.LightGreen;
            }
            else
            {
                return Brushes.DarkGreen;
            }
        }

        /// <summary>
        /// Converts an object back into the original object. This is not supported for BusyState.
        /// </summary>
        /// <param name="value">The converted value.</param>
        /// <param name="targetType">The type to convert back to.</param>
        /// <param name="parameter">The parameter associated with the conversion.</param>
        /// <param name="culture">The culture to use during conversion.</param>
        /// <returns>The original object.</returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
