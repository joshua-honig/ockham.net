using Ockham.Reflection;
using System;

namespace Ockham.Data
{
    // Methods for detecting empty values, and converting to or from empty value types
    public static partial class Convert
    {
        /// <summary>
        /// Returns true if the object is null or DBNull
        /// </summary>
        /// <param name="value">A value of any type</param> 
        public static bool IsNull(object value)
        {
            return value == null || value is DBNull;
        }

        /// <summary>
        /// Returns true if the object is null, DBNull, or an empty string
        /// </summary>
        /// <param name="value">A value of any type</param> 
        public static bool IsNullOrEmpty(object value)
        {
            return value == null || value is DBNull || (value is string && ((string)value == string.Empty));
        }

        /// <summary>
        /// Returns true if the object is null, DBNull, or an empty or whitespace string
        /// </summary>
        /// <param name="value">A value of any type</param> 
        public static bool IsNullOrWhitespace(object value)
        {
            if (value == null) return true;
            if (value is DBNull) return true;
            if (value is string)
            {
                string sValue = (string)value;
                if (sValue == string.Empty) return true;
                if (sValue.Length == 0) return true;
                if (sValue.Trim().Length == 0) return true;
            }
            return false;
        }

        /// <summary>
        /// Converts null, DBNull, or an empty string to null
        /// </summary>
        /// <param name="value">A value of any type</param>
        /// <param name="emptyStringAsNull">Whether to treat empty strings as null</param> 
        public static object ToNull(object value, bool emptyStringAsNull = true)
        {
            return (_IsEmpty(value, emptyStringAsNull) ? null : value);
        }

        /// <summary>
        /// Converts null, DBNull, or an empty string to DBNull
        /// </summary>
        /// <param name="value">A value of any type</param>
        /// <param name="emptyStringAsNull">Whether to treat empty strings as null</param> 
        public static object ToDBNull(object value, bool emptyStringAsNull = true)
        {
            return (_IsEmpty(value, emptyStringAsNull) ? DBNull.Value : value);
        }

        /// <summary>
        /// Converts the default value of a value type to DBNull
        /// </summary> 
        private static object DefaultToDBNull<T>(T value) where T : struct
        {
            T defaultValue = default(T);
            if (object.Equals(value, defaultValue))
            {
                return DBNull.Value;
            }
            else
            {
                return value;
            }
        }

        /// <summary>
        /// Converts the default value of a value type to DBNull
        /// </summary> 
        private static object DefaultToDBNull<T>(Nullable<T> value) where T : struct
        {
            if (!value.HasValue) return DBNull.Value;
            return DefaultToDBNull(value.Value);
        }

        /// <summary>
        /// Converts the provided default value of a value type to DBNull
        /// </summary> 
        public static object DefaultToDBNull<T>(T value, T defaultValue) where T : struct
        {
            if (object.Equals(value, defaultValue))
            {
                return DBNull.Value;
            }
            else
            {
                return value;
            }
        }

        /// <summary>
        /// Converts the default value of a value type to DBNull
        /// </summary> 
        private static object DefaultToDBNull<T>(Nullable<T> value, T defaultValue) where T : struct
        {
            if (!value.HasValue) return DBNull.Value;
            return DefaultToDBNull(value.Value, defaultValue);
        }

        private static bool _IsEmpty(object value, bool emptyStringAsNull)
        {
            if (value == null) return true;
            if (value is DBNull) return true;
            if (emptyStringAsNull && (value is string) && ((string)value) == string.Empty) return true;
            return false;
        }

        private static bool _IsEmpty(object value, ConvertOptions options)
        {
            return _IsEmpty(value, options.HasFlag(ConvertOptions.EmptyStringAsNull));
        }

    }
}