using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace ImageEditor.MainApp.Converters
{
    class DoubleToIntPercent : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (int)Math.Round((double)value * 100, MidpointRounding.AwayFromZero);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int val;
            try
            {
                val = int.Parse((string)value);
            }
            catch (Exception)
            {
                val = 100;
            }

            if (val < 10) val = 10;
            return val / 100d;
        }
    }
}
