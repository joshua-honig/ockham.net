using System;
using System.Collections.Generic;

namespace Ockham.Data.Tests
{
    public enum TestShortEnum : short
    {
        One = 1,
        FortyNine = 49
    }
     
    public static class ConvertTestData
    {

        public static readonly List<object> Int49Inputs = new List<object>()
        {
            49m,            // decimal
            (byte)49,       // byte
            (ushort)49,     // ushort
            49L,            // long
            49D,            // double
            "49",           // simple string
            //"0x31",         // C# hex string
            //" &h31\r\n",    // VB hex string with whitespace
            "4.9e+1",       // Scientific
            TestShortEnum.FortyNine  // Enum
        };

        public static readonly List<object> Int49Numbers = new List<object>()
        {
            49m,            // decimal
            (byte)49,       // byte
            (ushort)49,     // ushort
            49L,            // long
            49D,            // double
            TestShortEnum.FortyNine  // Enum
        };

        public static readonly List<object> EmptyInputs = new List<object>() { null, DBNull.Value };

    }
}
