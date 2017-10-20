using System;

namespace Ockham.Data
{
    // ------------------------------------------------------------------
    // ** HEY, YOU, DEVELOPER! **
    // 
    //  Do not modify this file directly!
    //  
    //  Update and rerun the code generating script Generate_Convert_Aliases_CSharp.ps1
    // ------------------------------------------------------------------

    // Type-specific conversion wrappers for underlying To and Force methods.  
    public static partial class Convert
    {

        /// <summary>
        /// Convert any input value to an equivalent boolean value. Attempting to convert
        /// a value that has no meaningful boolean equivalent, including an empty value,
        /// will cause an <see cref="System.InvalidCastException" /> to be raised.
        /// </summary>
        /// <param name="value">Any value</param>
        /// <seealso cref="Ockham.Data.Convert.To{T}(Object)"/>
        public static bool ToBool(object value)
        {
            return To<bool>(value);
        }


        /// <summary>
        /// Convert any input value to an equivalent date time value. Attempting to convert
        /// a value that has no meaningful date or time equivalent, including an empty value,
        /// will cause an <see cref="System.InvalidCastException" /> to be raised.
        /// </summary>
        /// <param name="value">Any value</param>
        /// <seealso cref="Ockham.Data.Convert.To{T}(Object)"/>
        public static DateTime ToDate(object value)
        {
            return To<DateTime>(value);
        }


        /// <summary>
        /// Convert any input value to an equivalent exact decimal value. Attempting to convert
        /// a value that has no meaningful decimal equivalent, including an empty value,
        /// will cause an <see cref="System.InvalidCastException" /> to be raised.
        /// </summary>
        /// <param name="value">Any value</param>
        /// <seealso cref="Ockham.Data.Convert.To{T}(Object)"/>
        public static decimal ToDec(object value)
        {
            return To<decimal>(value);
        }


        /// <summary>
        /// Convert any input value to an equivalent double-precision floating point number value. Attempting to convert
        /// a value that has no meaningful floating point equivalent, including an empty value,
        /// will cause an <see cref="System.InvalidCastException" /> to be raised.
        /// </summary>
        /// <param name="value">Any value</param>
        /// <seealso cref="Ockham.Data.Convert.To{T}(Object)"/>
        public static double ToDbl(object value)
        {
            return To<double>(value);
        }


        /// <summary>
        /// Convert any input value to an equivalent 128-bit globally unique identifier value. Attempting to convert
        /// a value that has no meaningful GUID equivalent, including an empty value,
        /// will cause an <see cref="System.InvalidCastException" /> to be raised.
        /// </summary>
        /// <param name="value">Any value</param>
        /// <seealso cref="Ockham.Data.Convert.To{T}(Object)"/>
        public static Guid ToGuid(object value)
        {
            return To<Guid>(value);
        }


        /// <summary>
        /// Convert any input value to an equivalent 32-bit signed integer value. Attempting to convert
        /// a value that has no meaningful integer equivalent, including an empty value,
        /// will cause an <see cref="System.InvalidCastException" /> to be raised.
        /// </summary>
        /// <param name="value">Any value</param>
        /// <seealso cref="Ockham.Data.Convert.To{T}(Object)"/>
        public static int ToInt(object value)
        {
            return To<int>(value);
        }


        /// <summary>
        /// Convert any input value to an equivalent 64-bit signed integer value. Attempting to convert
        /// a value that has no meaningful integer equivalent, including an empty value,
        /// will cause an <see cref="System.InvalidCastException" /> to be raised.
        /// </summary>
        /// <param name="value">Any value</param>
        /// <seealso cref="Ockham.Data.Convert.To{T}(Object)"/>
        public static long ToLng(object value)
        {
            return To<long>(value);
        }


        /// <summary>
        /// Convert any input value to an equivalent string value. Attempting to convert
        /// a value that has no meaningful string equivalent, including an empty value,
        /// will cause an <see cref="System.InvalidCastException" /> to be raised.
        /// </summary>
        /// <param name="value">Any value</param>
        /// <seealso cref="Ockham.Data.Convert.To{T}(Object)"/>
        public static string ToStr(object value)
        {
            return To<string>(value);
        }


        /// <summary>
        /// Convert any input value to an equivalent timespan value. Attempting to convert
        /// a value that has no meaningful timespan equivalent, including an empty value,
        /// will cause an <see cref="System.InvalidCastException" /> to be raised.
        /// </summary>
        /// <param name="value">Any value</param>
        /// <seealso cref="Ockham.Data.Convert.To{T}(Object)"/>
        public static TimeSpan ToTimeSpan(object value)
        {
            return To<TimeSpan>(value);
        }


        /// <summary>
        /// Convert any input value to an equivalent boolean value. If the input is empty
        /// or has no meaningful boolean equivalent, False is returned.
        /// </summary>
        /// <param name="value">Any value</param>
        /// <seealso cref="Ockham.Data.Convert.Force{T}(Object)"/>
        public static bool ForceBool(object value)
        {
            return Force<bool>(value);
        }


        /// <summary>
        /// Convert any input value to an equivalent date time value. If the input is empty
        /// or has no meaningful date or time equivalent, DateTime.MinValue is returned.
        /// </summary>
        /// <param name="value">Any value</param>
        /// <seealso cref="Ockham.Data.Convert.Force{T}(Object)"/>
        public static DateTime ForceDate(object value)
        {
            return Force<DateTime>(value);
        }


        /// <summary>
        /// Convert any input value to an equivalent exact decimal value. If the input is empty
        /// or has no meaningful decimal equivalent, 0 is returned.
        /// </summary>
        /// <param name="value">Any value</param>
        /// <seealso cref="Ockham.Data.Convert.Force{T}(Object)"/>
        public static decimal ForceDec(object value)
        {
            return Force<decimal>(value);
        }


        /// <summary>
        /// Convert any input value to an equivalent double-precision floating point number value. If the input is empty
        /// or has no meaningful floating point equivalent, 0 is returned.
        /// </summary>
        /// <param name="value">Any value</param>
        /// <seealso cref="Ockham.Data.Convert.Force{T}(Object)"/>
        public static double ForceDbl(object value)
        {
            return Force<double>(value);
        }


        /// <summary>
        /// Convert any input value to an equivalent 128-bit globally unique identifier value. If the input is empty
        /// or has no meaningful GUID equivalent, Guid.Empty is returned.
        /// </summary>
        /// <param name="value">Any value</param>
        /// <seealso cref="Ockham.Data.Convert.Force{T}(Object)"/>
        public static Guid ForceGuid(object value)
        {
            return Force<Guid>(value);
        }


        /// <summary>
        /// Convert any input value to an equivalent 32-bit signed integer value. If the input is empty
        /// or has no meaningful integer equivalent, 0 is returned.
        /// </summary>
        /// <param name="value">Any value</param>
        /// <seealso cref="Ockham.Data.Convert.Force{T}(Object)"/>
        public static int ForceInt(object value)
        {
            return Force<int>(value);
        }


        /// <summary>
        /// Convert any input value to an equivalent 64-bit signed integer value. If the input is empty
        /// or has no meaningful integer equivalent, 0 is returned.
        /// </summary>
        /// <param name="value">Any value</param>
        /// <seealso cref="Ockham.Data.Convert.Force{T}(Object)"/>
        public static long ForceLng(object value)
        {
            return Force<long>(value);
        }


        /// <summary>
        /// Convert any input value to an equivalent string value. If the input is empty
        /// or has no meaningful string equivalent, null is returned.
        /// </summary>
        /// <param name="value">Any value</param>
        /// <seealso cref="Ockham.Data.Convert.Force{T}(Object)"/>
        public static string ForceStr(object value)
        {
            return Force<string>(value);
        }


        /// <summary>
        /// Convert any input value to an equivalent timespan value. If the input is empty
        /// or has no meaningful timespan equivalent, Timespan.zero is returned.
        /// </summary>
        /// <param name="value">Any value</param>
        /// <seealso cref="Ockham.Data.Convert.Force{T}(Object)"/>
        public static TimeSpan ForceTimeSpan(object value)
        {
            return Force<TimeSpan>(value);
        }


        /// <summary>
        /// Convert any input value to an equivalent boolean value. If the input is empty
        /// or has no meaningful boolean equivalent the provided default value is returned. 
        /// </summary>
        /// <param name="value">Any value</param>
        /// <param name="defaultValue">A value to return if the input is empty or cannot be converted</param>
        /// <seealso cref="Ockham.Data.Convert.Force{T}(Object, T)"/>
        public static bool ForceBool(object value, bool defaultValue)
        {
            return Force<bool>(value, defaultValue);
        }


        /// <summary>
        /// Convert any input value to an equivalent date time value. If the input is empty
        /// or has no meaningful date or time equivalent the provided default value is returned. 
        /// </summary>
        /// <param name="value">Any value</param>
        /// <param name="defaultValue">A value to return if the input is empty or cannot be converted</param>
        /// <seealso cref="Ockham.Data.Convert.Force{T}(Object, T)"/>
        public static DateTime ForceDate(object value, DateTime defaultValue)
        {
            return Force<DateTime>(value, defaultValue);
        }


        /// <summary>
        /// Convert any input value to an equivalent exact decimal value. If the input is empty
        /// or has no meaningful decimal equivalent the provided default value is returned. 
        /// </summary>
        /// <param name="value">Any value</param>
        /// <param name="defaultValue">A value to return if the input is empty or cannot be converted</param>
        /// <seealso cref="Ockham.Data.Convert.Force{T}(Object, T)"/>
        public static decimal ForceDec(object value, decimal defaultValue)
        {
            return Force<decimal>(value, defaultValue);
        }


        /// <summary>
        /// Convert any input value to an equivalent double-precision floating point number value. If the input is empty
        /// or has no meaningful floating point equivalent the provided default value is returned. 
        /// </summary>
        /// <param name="value">Any value</param>
        /// <param name="defaultValue">A value to return if the input is empty or cannot be converted</param>
        /// <seealso cref="Ockham.Data.Convert.Force{T}(Object, T)"/>
        public static double ForceDbl(object value, double defaultValue)
        {
            return Force<double>(value, defaultValue);
        }


        /// <summary>
        /// Convert any input value to an equivalent 128-bit globally unique identifier value. If the input is empty
        /// or has no meaningful GUID equivalent the provided default value is returned. 
        /// </summary>
        /// <param name="value">Any value</param>
        /// <param name="defaultValue">A value to return if the input is empty or cannot be converted</param>
        /// <seealso cref="Ockham.Data.Convert.Force{T}(Object, T)"/>
        public static Guid ForceGuid(object value, Guid defaultValue)
        {
            return Force<Guid>(value, defaultValue);
        }


        /// <summary>
        /// Convert any input value to an equivalent 32-bit signed integer value. If the input is empty
        /// or has no meaningful integer equivalent the provided default value is returned. 
        /// </summary>
        /// <param name="value">Any value</param>
        /// <param name="defaultValue">A value to return if the input is empty or cannot be converted</param>
        /// <seealso cref="Ockham.Data.Convert.Force{T}(Object, T)"/>
        public static int ForceInt(object value, int defaultValue)
        {
            return Force<int>(value, defaultValue);
        }


        /// <summary>
        /// Convert any input value to an equivalent 64-bit signed integer value. If the input is empty
        /// or has no meaningful integer equivalent the provided default value is returned. 
        /// </summary>
        /// <param name="value">Any value</param>
        /// <param name="defaultValue">A value to return if the input is empty or cannot be converted</param>
        /// <seealso cref="Ockham.Data.Convert.Force{T}(Object, T)"/>
        public static long ForceLng(object value, long defaultValue)
        {
            return Force<long>(value, defaultValue);
        }


        /// <summary>
        /// Convert any input value to an equivalent string value. If the input is empty
        /// or has no meaningful string equivalent the provided default value is returned. 
        /// </summary>
        /// <param name="value">Any value</param>
        /// <param name="defaultValue">A value to return if the input is empty or cannot be converted</param>
        /// <seealso cref="Ockham.Data.Convert.Force{T}(Object, T)"/>
        public static string ForceStr(object value, string defaultValue)
        {
            return Force<string>(value, defaultValue);
        }


        /// <summary>
        /// Convert any input value to an equivalent timespan value. If the input is empty
        /// or has no meaningful timespan equivalent the provided default value is returned. 
        /// </summary>
        /// <param name="value">Any value</param>
        /// <param name="defaultValue">A value to return if the input is empty or cannot be converted</param>
        /// <seealso cref="Ockham.Data.Convert.Force{T}(Object, T)"/>
        public static TimeSpan ForceTimeSpan(object value, TimeSpan defaultValue)
        {
            return Force<TimeSpan>(value, defaultValue);
        } 

    }
}
