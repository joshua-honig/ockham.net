using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Reflection;
using Ockham.Data;

namespace Ockham.Reflection
{
    /// <summary>
    /// Provides utility methods for inspecting types and values 
    /// </summary>
    public static class TypeUtil
    {
        /// <summary>
        /// Test if the input value is a non-null numeric type 
        /// or a string that can be parsed as a number. Valid hexadecimal strings
        /// are not treated as numbers.
        /// </summary>
        /// <remarks>To detect valid hexadecimal strings, use <see cref="IsNumeric(object, ConvertOptions)"/></remarks>
        /// <param name="value"></param> 
        public static bool IsNumeric(object value)
        {
            return IsNumeric(value, (ConvertOptions)0);
        }

        /// <summary>
        /// Test if the input value is a non-null numeric type 
        /// or a string that can be parsed as a number, with detection
        /// of hex strings controlled by ConvertOptions flags
        /// </summary>
        /// <param name="value"></param> 
        /// <param name="options"></param>  
        public static bool IsNumeric(object value, ConvertOptions options)
        {
            if (value == null) return false;
            if (value is string)
            {
                string sValue = (string)value;
                double d; if (double.TryParse(sValue, out d)) return true;
                if (options.HasFlag(ConvertOptions.AllowVBHex) && Regex.IsMatch(sValue, @"^\s*&[hH][0-9a-fA-F]+$")) return true;
                if (options.HasFlag(ConvertOptions.Allow0xHex) && Regex.IsMatch(sValue, @"^\s*0[xX][0-9a-fA-F]+$")) return true;
                return false;
            }
            return IsNumberType(value.GetType());
        }

        /// <summary>
        /// Determine if a type is a static class (Module in VB.Net)
        /// </summary> 
        public static bool IsStaticClass(Type type)
        {
            var typeInfo = type.GetTypeInfo();
            return typeInfo.IsClass && typeInfo.IsAbstract && typeInfo.IsSealed;
        }

        /// <summary>
        /// Determine if the specified type is an integer type. Returns true for enums.
        /// </summary> 
        public static bool IsIntegerType(Type type)
        {
            return _IsNumberType(type, true);
        }

        /// <summary>
        /// Determine if the specified type is a numberic (integer, float, or decimal) type. Returns true for enums.
        /// </summary> 
        public static bool IsNumberType(Type type)
        {
            return _IsNumberType(type, false);
        }

        private static bool _IsNumberType(Type type, bool integersOnly)
        {

            if (type.GetTypeInfo().IsEnum) return true;

            switch (System.Convert.GetTypeCode(type))
            {
                case TypeCode.Byte:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                case TypeCode.SByte:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                    return true;
                case TypeCode.Decimal:
                case TypeCode.Double:
                case TypeCode.Single:
                    return !integersOnly;
            }

            return false;
        }


        /// <summary>
        /// Determine if the specified type inherits from <see cref="System.Nullable{T}"/>
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsNullableOfT(Type type)
        {
            var typeInfo = type.GetTypeInfo();
            return typeInfo.IsGenericType && !typeInfo.IsGenericTypeDefinition && (type.GetGenericTypeDefinition() == typeof(Nullable<>));
        }

        /// <summary>
        /// Determine if the specified type inherits from <see cref="System.Nullable{T}"/>, and return the inner type if it is 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="elementType"></param>
        /// <returns></returns>
        public static bool IsNullableOfT(Type type, ref Type elementType)
        {
            if (IsNullableOfT(type))
            {
                elementType = type.GetTypeInfo().GenericTypeArguments[0];
                return true;
            }
            else
            {
                elementType = null;
                return false;
            }
        }

        /// <summary>
        /// Determine if the specified type implements either <see cref="System.Collections.IEnumerable"/>
        /// or <see cref="System.Collections.Generic.IEnumerable{T}"/>. 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="elementType"></param>
        /// <returns></returns>
        /// <remarks> If the specified type implements <see cref="System.Collections.Generic.IEnumerable{T}"/>
        /// then the element (T) type will be assigned to elementType. If the function returns true
        /// but elementType is null, it means the type implements <see cref="System.Collections.IEnumerable"/>
        /// but not <see cref="System.Collections.Generic.IEnumerable{T}"/>
        /// </remarks>
        public static bool IsEnumerable(Type type, ref Type elementType)
        {
            //if (type == typeof(string)) return false;
            Type[] typeParams = null;
            if (Implements(type, typeof(IEnumerable<>), ref typeParams))
            {
                elementType = typeParams[0];
                return true;
            }
            else
            {
                elementType = null;
                return Implements(type, typeof(System.Collections.IEnumerable));
            }
        }

        /// <summary>
        /// Determine if the specified type implements a particular interface
        /// </summary> 
        public static bool Implements(Type type, Type interfaceType)
        {
            Type[] typeParams = null;
            return Implements(type, interfaceType, ref typeParams);
        }

        /// <summary>
        /// Determine if the specified type implements a particular interface
        /// </summary> 
        /// <remarks> If interface is an open generic interface and type is a constructed
        /// version of it, typeArguments will contain the array of generic type parameters
        /// </remarks>
        public static bool Implements(Type type, Type interfaceType, ref Type[] typeArguments)
        {
            var typeInfo = type.GetTypeInfo();
            var interfaceInfo = interfaceType.GetTypeInfo();

            typeArguments = null;
            if (!interfaceInfo.IsInterface) throw new ArgumentException("interface must be an interface type");

            var allInterfaces = typeInfo.ImplementedInterfaces.ToList();
            if (type.GetTypeInfo().IsInterface) allInterfaces.Insert(0, type);

            if (interfaceInfo.IsGenericTypeDefinition)
            {
                foreach (Type implementedInterface in allInterfaces)
                {
                    if (interfaceInfo.IsGenericType)
                    {
                        Type interfaceTypeDef = implementedInterface.GetGenericTypeDefinition();
                        if (interfaceTypeDef == interfaceType)
                        {
                            typeArguments = interfaceInfo.GenericTypeArguments;
                            return true;
                        }
                    }
                }
                return false;
            }
            else
            {
                return allInterfaces.Contains(interfaceType);
            }
        }

        /// <summary>
        /// Determine if the specified type is a constructed generic of a given generic type definition
        /// </summary>  
        public static bool IsConstructedType(Type type, Type genericTypeDefinition)
        {
            Type[] typeArray = null;
            return TypeUtil.IsConstructedType(type, genericTypeDefinition, ref typeArray);
        }

        /// <summary>
        /// Determine if the specified type is a constructed generic of a given generic type definition. If 
        /// true, the array of generic type parameters is returned in typeArguments.
        /// </summary>  
        public static bool IsConstructedType(Type type, Type genericTypeDefinition, ref Type[] typeArguments)
        {
            var genericInfo = genericTypeDefinition.GetTypeInfo();
            if (!genericInfo.IsGenericTypeDefinition)
            {
                throw new ArgumentException("genericTypeDefinition must be a generic type definition");
            }
            if (genericInfo.IsGenericType)
            {
                if (type.GetGenericTypeDefinition() == genericTypeDefinition)
                {
                    typeArguments = genericInfo.GenericTypeArguments;
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Determine if the value represents the default value (Nothing in Visual Basic) for the value's type. 
        /// A value of null (Nothing in Visual Basic) will always return true.
        /// </summary> 
        public static bool IsDefault(object value)
        {
            if (value == null) return true;
            Type type = value.GetType();
            if (type.GetTypeInfo().IsValueType)
            {
                object defaultValue = Activator.CreateInstance(type);
                return object.Equals(value, defaultValue);
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Return the default value of any type. Equivalent to the C# generic keyword default(T)
        /// </summary> 
        public static object Default(Type type)
        {
            if (type.GetTypeInfo().IsValueType)
            {
                return Activator.CreateInstance(type);
            }
            else
            {
                return null;
            }
        }

#if NETSTANDARD1_3
        /// <summary>
        /// Determine if the provided value is an instance of the provided type
        /// </summary>
        /// <param name="type"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsInstanceOfType(Type type, object value)
        {
            if (value == null) return false;
            Type valType = value.GetType();
            return valType == type || valType.GetTypeInfo().IsSubclassOf(type);
        }
#else
        internal static bool IsInstanceOfType(Type type, object value)
        {
            return type.IsInstanceOfType(value);
        }
#endif




    }
}
