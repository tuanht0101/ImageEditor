using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace ImageEditor.MainApp.ValidationRule
{
    class ColorRule : System.Windows.Controls.ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            try
            {
                ColorTranslator.FromHtml((string)value);
                return ValidationResult.ValidResult;
            } catch
            {
                return new ValidationResult(false, $"Incorrect format");
            }
        }
    }
}
