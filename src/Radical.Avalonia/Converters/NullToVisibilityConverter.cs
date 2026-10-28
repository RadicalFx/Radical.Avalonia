using Radical.Conversions;
using System;
using Avalonia.Data.Converters;
using Avalonia.Markup.Xaml;

namespace Radical.Avalonia.Converters
{
    public class NullToVisibilityConverter : IValueConverter
    {
        public NullToVisibilityConverter()
        {
            NullValue = false;
        }

        public bool NullValue
        {
            get;
            set;
        }

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (!bool.TryParse(parameter as string, out bool inverted))
            {
                inverted = false;
            }

            //bool flag = false;
            //if( value is bool )
            //{
            //    flag = ( bool )value;
            //}
            //else if( value is bool? )
            //{
            //    var nullable = ( bool? )value;
            //    flag = nullable.HasValue ? nullable.Value : false;
            //}

            if (inverted)
            {
                return (value == null ? true : NullValue);
            }
            else
            {
                return (value == null ? NullValue : true);
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}