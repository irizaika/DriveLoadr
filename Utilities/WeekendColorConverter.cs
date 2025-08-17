using System;
using System.Globalization;
using Microsoft.Maui.Controls;

namespace DriveLoadr.Utilities
{

    public class WeekendColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool isWeekend)
                return isWeekend ? Colors.Red : Colors.Black;

            return Colors.Black; // fallback
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

