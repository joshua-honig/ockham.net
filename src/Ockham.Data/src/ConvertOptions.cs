using System;

namespace Ockham.Data
{
    /// <summary>
    /// Flag enumeration options for controlling data type conversions executed by <see cref="Ockham.Data.Convert"/>
    /// </summary>
    [Flags]
    public enum ConvertOptions
    {
        /// <summary>
        /// No relaxed conversion options
        /// </summary>
        None = 0,

        /// <summary>
        /// Treat a non-null empty string as null
        /// </summary>
        EmptyStringAsNull = 0x1,

        /// <summary>
        /// Convert null values to the default value when the target type is a value type
        /// </summary>
        NullToValueDefault = 0x2,

        /// <summary>
        /// Alias for None
        /// </summary>
        Strict = 0,

        /// <summary>
        /// Alias for EmpyStringAsNull
        /// </summary>
        Default = 0x1,

        /// <summary>
        /// Alias for EmptyStringAsNull, NullToValueDefault
        /// </summary>
        Relaxed = 0x3,

        /// <summary>
        /// Recognize valid VB-syntax hex strings (&amp;H[0-9A-F]+)
        /// </summary>
        AllowVBHex = 0x4,

        /// <summary>
        /// Recognize valid 0x-prefixed hex strings (0x[0-9A-F]+)
        /// </summary>
        Allow0xHex = 0x8,

        /// <summary>
        /// Alias for AllowVBHex, Allow0xHex
        /// </summary>
        AllowHex = 0xc 
    }
}
