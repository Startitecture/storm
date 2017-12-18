namespace SAF.UserInterface
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Windows.Data;

    /// <summary>
    /// Converts a worker monitor item state into a status string as specified by the
    /// caller.
    /// </summary>
    public class MonitorItemStateConverter : IMultiValueConverter
    {
        /// <summary>
        /// Converts total, processed, skipped and failed counts into a status string.
        /// </summary>
        /// <param name="values">Total, processed, skipped and failed item counts</param>
        /// <param name="targetType">The type of the binding target property</param>
        /// <param name="parameter">The format string to use to format the status
        /// text. {0}=total, {1}=processed, {2}=skipped, {3}=failed, {4}=tasks per second.</param>
        /// <param name="culture">The culture to use in the converter</param>
        /// <returns>A status string with the specified format.</returns>
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values == null)
            {
                throw new ArgumentNullException("values");
            }

            return String.Format((string)parameter, values[0], values[1], values[2], values[3], values[4]);
        }

        /// <summary>
        /// Attempts to parse out total, processed, skipped and failed counts from a
        /// status string.
        /// </summary>
        /// <param name="value">The status text</param>
        /// <param name="targetTypes">The array of types to convert to. The array 
        /// length indicates the number and types of values that are suggested for 
        /// the method to return.</param>
        /// <param name="parameter">The format string to use to format the status
        /// text. {0}=total, {1}=processed, {2}=skipped, {3}=failed.</param>
        /// <param name="culture">The culture to use in the converter</param>
        /// <returns>An array of integers that represent the total, processed,
        /// skipped and failed items, in that order.</returns>
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            object[] intValues = new object[5];

            // First get the parameters, including their order.
            List<string> paramStrs = new List<string>();
            int[] paramOrder = new int[5];
            int index = 0;
            string paramMatch = @"\{\d\}";

            for
                (Match m = Regex.Match(
                    (string)parameter, paramMatch); m.Success; m = m.NextMatch())
            {
                paramOrder[index] =
                    System.Convert.ToInt32(
                        m.Value.Replace("{", String.Empty).Replace("}", String.Empty));

                paramStrs.Add(m.Value);
                index++;
            }

            // Then split the parameter string using the parameters.
            string[] formatTokens =
                ((string)parameter).Split(
                    paramStrs.ToArray(), StringSplitOptions.RemoveEmptyEntries);

            // Then split the value string using the parameter format tokens.
            string[] strValues =
                ((string)value).Split(
                    formatTokens.ToArray(), StringSplitOptions.RemoveEmptyEntries);

            for (int i = 0; i < paramOrder.Length; i++)
            {
                intValues[paramOrder[i]] = Decimal.Parse(strValues[i]);
            }

            return intValues;
        }
    }
}
