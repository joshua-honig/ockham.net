using System;
using System.Text.RegularExpressions;
using Ockham.Reflection;
using VBConvert = Microsoft.VisualBasic.CompilerServices.Conversions;

namespace Ockham.Data
{
    // Private implementations of conversions to specific types
    public static partial class Convert
    {
        private const string RX_MINUTES = @"^(?<minutes>[0-9]{1,2}):[0-9]{2}(\.[0-9]+)?$";

        private static bool _ToBool(object value)
        {
            if (value is string)
            {
                switch (((string)value).ToLower().Trim())
                {
                    case "x":
                    case "t":
                    case "y":
                    case "yes":
                    case "true":
                    case "1":
                    case "-1":
                    case "yea":
                    case "aye":
                    case "on":
                    case "checked":
                        return true;
                    case "":
                    case "f":
                    case "n":
                    case "no":
                    case "false":
                    case "0":
                    case "nay":
                    case "off":
                    case "unchecked":
                        return false;

                }
            }

            return VBConvert.ToBoolean(value);
        }

        private static Guid _ToGuid(object value)
        {
            if (value is string)
            {
                return new Guid((string)value);
            }
            else if (value is byte[])
            {
                return new Guid((byte[])value);
            }

            // If input is not a Guid, this will throw an invalid cast exception in the localized language:
            return (Guid)value;
        }

        private static TimeSpan _ToTimeSpan(object value)
        {

            if (value is string && Regex.IsMatch((string)value, @"^\s*\d+\.\d+\s*$"))
            {
                // Treat decimal string as seconds, not ticks
                return TimeSpan.FromSeconds(To<double>(value));
            }
            else if (TypeUtil.IsNumeric(value) || (value != null && TypeUtil.IsNumberType(value.GetType())))
            {
                return TimeSpan.FromTicks(To<long>(value));
            }
            else if (value is string)
            {
                string sValue = (string)value;
                var m = Regex.Match(sValue, RX_MINUTES);
                if (m.Success)
                {
                    // The input is a minutes : seconds string. The built-in TimeSpan.Parse requires a leading
                    // hours segment as well. Append the 0: hours segment, and also pad the minutes segment with
                    // a leading 0 if necessary
                    sValue = "0:" + (m.Groups["minutes"].Value.Length == 2 ? "" : "0") + sValue;
                }

                return TimeSpan.Parse(sValue);
            }
            else if (value is DateTime)
            {
                return TimeSpan.FromTicks(((DateTime)value).Ticks);
            }

            // If the input is not a TimeSpan, this will throw an invalid cast exception in the localized language:
            return (TimeSpan)value;
        }
    }
}
