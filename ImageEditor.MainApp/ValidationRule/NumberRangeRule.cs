using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace ImageEditor.MainApp.ValidationRule
{
    class NumberRangeRule : System.Windows.Controls.ValidationRule
    {
        internal enum Types
        {
            Double, Int, UInt, Byte
        }
        public Types Type { get; set; } = Types.Double;

        public double? GreaterThanEqual { get; set; } = null;
        public double? LessThanEqual { get; set; } = null;
        public double? GreaterThan { get; set; } = null;
        public double? LessThan { get; set; } = null;

        public Array Excludes { get; set; }

        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            double val = 0;

            switch (Type)
            {
                case Types.Double:
                    try
                    {
                        val = double.Parse((string)value);
                    }
                    catch (Exception)
                    {
                        return new ValidationResult(false, $"Incorrect format");
                    }
                    break;
                case Types.Int:
                    try
                    {
                        val = int.Parse((string)value);
                    }
                    catch (Exception)
                    {
                        return new ValidationResult(false, $"Incorrect format");
                    }
                    break;
                case Types.UInt:
                    try
                    {
                        val = uint.Parse((string)value);
                    }
                    catch (Exception)
                    {
                        return new ValidationResult(false, $"Incorrect format");
                    }
                    break;
                case Types.Byte:
                    try
                    {
                        val = byte.Parse((string)value);
                    }
                    catch (Exception)
                    {
                        return new ValidationResult(false, $"Incorrect format");
                    }
                    break;
            }

            if (Excludes != null)
            {
                foreach (double exclude in Excludes)
                {
                    if (val.CompareTo(exclude) == 0) return new ValidationResult(false, $"Match excluded numbers");
                }
            }

            if (
                (GreaterThanEqual != null && GreaterThanEqual > val) ||
                (LessThanEqual != null && LessThanEqual < val) ||
                (GreaterThan != null && GreaterThan >= val) ||
                (LessThan != null && LessThan <= val)
                )
                return new ValidationResult(false, $"Out of range");

            return ValidationResult.ValidResult;
        }
    }
}
