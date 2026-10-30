using Radical.Validation;
using Radical.Avalonia.Behaviors;
using System;

namespace Radical.Avalonia.Converters
{
    public sealed class BooleanBusyStatusConverter : AbstractSingletonConverter
    {
        /// <summary>
        /// Converts a value.
        /// </summary>
        /// <param name="value">The value produced by the binding source.</param>
        /// <param name="targetType">The type of the binding target property.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>
        /// A converted value. If the method returns null, the valid null value is used.
        /// </returns>
        public override object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            Ensure.That(value).IsNotNull().IsTrue(o => o is bool);
            Ensure.That(targetType).IsNotNull().Is(typeof(BusyStatus));

            var actual = (bool)value;
            var status = actual ? BusyStatus.Busy : BusyStatus.Idle;

            return status;
        }

        /// <summary>
        /// Converts a value.
        /// </summary>
        /// <param name="value">The value that is produced by the binding target.</param>
        /// <param name="targetType">The type to convert to.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>
        /// A converted value. If the method returns null, the valid null value is used.
        /// </returns>
        public override object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            Ensure.That(value).IsNotNull().IsTrue(o => o is BusyStatus);
            Ensure.That(targetType).IsNotNull().Is(typeof(bool));

            var status = (BusyStatus)value;
            switch (status)
            {
                case BusyStatus.Idle:
                    return false;

                case BusyStatus.Busy:
                    return true;
            }

            throw new InvalidOperationException();
        }
    }
}
