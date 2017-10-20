using Ockham.Reflection;
using System;
using System.Text.RegularExpressions;
using System.Reflection;
using VBConvert = Microsoft.VisualBasic.CompilerServices.Conversions;

namespace Ockham.Data
{

    // The core implementation of converting an input value to a target type. All public type
    // conversion overloads on Ockham.Convert should internally call the _To method 
    [Microsoft.VisualBasic.CompilerServices.StandardModule] // Mark as a VB module so methods can be called without the class name from VB
    public static partial class Convert
    {
        private static object _To(object value, Type targetType, ConvertOptions options, bool ignoreError, object defaultValue)
        {
            var typeInfo = targetType.GetTypeInfo();

            // Quick check #1: If input is null reference and target type is a reference type we can always safely return nothing
            if ((value == null) && (!typeInfo.IsValueType)) return null;

            // Quick check #2: If the input already is of the target type the return it immediately
            if (TypeUtil.IsInstanceOfType(targetType, value)) return value;

            if (typeInfo.IsValueType)
            {
                // ---------------------------------------------------------------------------
                //  Special treatment of Nullable<T>. Although both C# and VB allow assignment
                //  and comparison of null literal to Nullable<T> values, the underlying type
                //  is actually a non-nullable struct (value type), thus we handle nullable
                //  types inside the section that handles value types
                // ---------------------------------------------------------------------------
                if (TypeUtil.IsNullableOfT(targetType))
                {
                    if (_IsEmpty(value, options))
                    {
                        // Nullable<T> of T is technically a struct, so IsValueType is true. However,
                        // semantics and implementation allow it to be treated as a reference type,
                        // so return null if the input is empty
                        return null;
                    }
                    else
                    {
                        // Input is not an empty value, so convert to the underying type
                        try
                        {
                            // Note: DON'T ignore error on underlying conversion type
                            return _To(value, Nullable.GetUnderlyingType(targetType), options, false, null);
                        }
                        catch
                        {
                            if (ignoreError) return defaultValue ?? null;
                            throw;
                        }
                    }
                } // end IsNullable<T>

                // If input is empty, we can immediately return default or throw an exception
                if (_IsEmpty(value, options))
                {
                    if (options.HasFlag(ConvertOptions.NullToValueDefault))
                    {
                        //This is how we return the default value of a value type:
                        return defaultValue ?? Activator.CreateInstance(targetType);
                    }
                    else
                    {
                        if (ignoreError) return defaultValue ?? Activator.CreateInstance(targetType);
                        throw new ArgumentNullException("value", "Cannot convert empty input value to value type " + targetType.FullName);
                    }
                }

                // We only got this far if the target type is a value type and the input value is not empty
                try
                {
                    // ---------------------------------------------------------------------------
                    //  Special treatment of Enums
                    // ---------------------------------------------------------------------------
                    if (typeInfo.IsEnum)
                    {
                        // Test IsNumeric BEFORE testing string so that numeric strings are
                        // treated as numbers, not as enum member names
                        if (TypeUtil.IsNumeric(value, options))
                        {
                            // If the input is a number or *numeric string*, first convert the 
                            // input to an enum number value, then cast it using Enum.ToObject

                            // Note: DON'T ignore error on underlying conversion type
                            var rawValue = _To(value, Enum.GetUnderlyingType(targetType), options, false, null);
                            return Enum.ToObject(targetType, rawValue);
                        }
                        else if (value is string)
                        {
                            // Input is a string but Information.IsNumeric did not recognize it
                            // as a number. So, treat the input as an enum member name
                            string sEnumName = Regex.Replace((string)value, @"[\s\r\n]+", "");
                            return Enum.Parse(targetType, sEnumName, true);
                        }
                        else
                        {
                            // Fallback: Attempt to convert the input to the underlying numeric type, even if
                            // Information.IsNumeric returned false

                            // Note: DON'T ignore error on underlying conversion type
                            var rawValue = _To(value, Enum.GetUnderlyingType(targetType), options, false, null);
                            return Enum.ToObject(targetType, rawValue);
                        }
                    } // end IsEnum

                    // ---------------------------------------------------------------------------
                    //  Use hand-written conversion functions for specific types
                    // ---------------------------------------------------------------------------
                    if (targetType == typeof(Guid))
                    {
                        // Use special conversion logic for converting to a Guid
                        return _ToGuid(value);
                    }
                    else if (targetType == typeof(TimeSpan))
                    {
                        // Use special conversion logic for converting to a TimeSpan
                        return _ToTimeSpan(value);
                    }
                    else if (targetType == typeof(bool))
                    {
                        // Use special conversion logic for converting to Boolean
                        return _ToBool(value);
                    }

                    // ---------------------------------------------------------------------------
                    //  Invoke IConvertible implementation, if any
                    // ---------------------------------------------------------------------------
                    IConvertible iConvertible = value as IConvertible;
                    if (iConvertible != null)
                    {
                        // Use the System.ChangeType method, which makes full use of any IConvertible implementation on the target type
                        try
                        {
                            return System.Convert.ChangeType(value, targetType);
                        }
                        catch
                        {
                            // Ignore exception and fall back to VBConvert implementation
                        }
                    }

                    // ---------------------------------------------------------------------------
                    //  Fall back to VBConvert methods
                    // ---------------------------------------------------------------------------
                    switch (System.Convert.GetTypeCode(targetType))
                    {
                        case TypeCode.Boolean:
                            // Use custom ToBool method
                            return _ToBool(value);
                        case TypeCode.Byte:
                            return VBConvert.ToByte(value);
                        case TypeCode.Char:
                            return VBConvert.ToChar(value);
                        case TypeCode.DateTime:
                            return VBConvert.ToDate(value);
                        case TypeCode.Decimal:
                            return VBConvert.ToDecimal(value);
                        case TypeCode.Double:
                            return VBConvert.ToDouble(value);
                        case TypeCode.Int16:
                            return VBConvert.ToShort(value);
                        case TypeCode.Int32:
                            return VBConvert.ToInteger(value);
                        case TypeCode.Int64:
                            return VBConvert.ToLong(value);
                        case TypeCode.SByte:
                            return VBConvert.ToSByte(value);
                        case TypeCode.Single:
                            return VBConvert.ToSingle(value);
                        case TypeCode.String:
                            return VBConvert.ToString(value);
                        case TypeCode.UInt16:
                            return VBConvert.ToUShort(value);
                        case TypeCode.UInt32:
                            return VBConvert.ToUInteger(value);
                        case TypeCode.UInt64:
                            return VBConvert.ToULong(value);

                    }

                    // ---------------------------------------------------------------------------
                    //  Fall back to VBConvert.ChangeType
                    // ---------------------------------------------------------------------------
                    return VBConvert.ChangeType(value, targetType);
                }
                catch
                {
                    // Conversion of non-empty value to value type failed.
                    if (ignoreError) return defaultValue ?? Activator.CreateInstance(targetType);
                    throw;
                }
            } // end IsValueType
            else
            {
                // Reference types...not much to do here besides check for empty values

                if (_IsEmpty(value, options))
                {
                    return null;
                }

                try
                {
                    //  Fall back to VBConvert.ChangeType
                    return VBConvert.ChangeType(value, targetType);
                }
                catch
                {
                    if (ignoreError) return defaultValue ?? null;
                    throw;
                }
            }
        }
    }
}
