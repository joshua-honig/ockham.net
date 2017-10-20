using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using OAssert = Ockham.Test.Assert;
using OData = Ockham.Data;

namespace Ockham.Data.Tests
{
    public class ConvertCoreTests
    {
        private static class ConvertAssert
        {
            public static void Equal<T>(T expected, object actual)
            {
                if (!(actual is T)) throw new Xunit.Sdk.EqualException(expected, actual);
                if (!typeof(T).IsValueType && !!Object.ReferenceEquals(expected, actual)) throw new Xunit.Sdk.EqualException(expected, actual);
                if (!Object.Equals(expected, actual)) throw new Xunit.Sdk.EqualException(expected, actual);
            }
        }

        private class OptionsPermutation
        {
            public OptionsPermutation(ConvertOptions pOptions, bool pIgnoreError, object pDefaultValue)
            {
                this.ConvertOptions = pOptions;
                this.IgnoreError = pIgnoreError;
                this.DefaultValue = pDefaultValue;
            }

            public ConvertOptions ConvertOptions { get; set; }
            public bool IgnoreError { get; set; }
            public object DefaultValue { get; set; }
        }

        private static Func<object, Type, ConvertOptions, bool, object, object> fnTo =
            Ockham.Test.MethodReflection.GetMethodCaller<Func<object, Type, ConvertOptions, bool, object, object>>(typeof(OData.Convert), "_To");

        /// <summary>
        /// Generate a list of all 12 permutations of convert options, ignore error, and default value
        /// </summary> 
        private List<OptionsPermutation> GetOptionsPermutations(object pDefaultValue)
        {
            return new List<OptionsPermutation>()
            {
                new OptionsPermutation(ConvertOptions.None, false, null),
                new OptionsPermutation(ConvertOptions.EmptyStringAsNull, false, null),
                new OptionsPermutation(ConvertOptions.NullToValueDefault, false, null),

                new OptionsPermutation(ConvertOptions.None, true, null),
                new OptionsPermutation(ConvertOptions.EmptyStringAsNull, true, null),
                new OptionsPermutation(ConvertOptions.NullToValueDefault, true, null),

                new OptionsPermutation(ConvertOptions.None, false, pDefaultValue),
                new OptionsPermutation(ConvertOptions.EmptyStringAsNull, false, pDefaultValue),
                new OptionsPermutation(ConvertOptions.NullToValueDefault, false, pDefaultValue),

                new OptionsPermutation(ConvertOptions.None, true, pDefaultValue),
                new OptionsPermutation(ConvertOptions.EmptyStringAsNull, true, pDefaultValue),
                new OptionsPermutation(ConvertOptions.NullToValueDefault, true, pDefaultValue)
            };
        }


        [Fact(DisplayName = "Convert._To:ImmediateReturnNullForNullReference")]
        public void ImmediateReturnNullForNullReference()
        {
            Type ltRefType = typeof(object);
            foreach (var lPermation in GetOptionsPermutations(new object()))
            {
                Assert.Null(fnTo(null, ltRefType, lPermation.ConvertOptions, lPermation.IgnoreError, lPermation.DefaultValue));
            }
        }

        [Fact(DisplayName = "Convert._To:ImmediateReturnTargetRefernceType")]
        public void ImmediateReturnTargetRefernceType()
        {
            string lSourceValue = "foo bar baz";

            foreach (var lPermation in GetOptionsPermutations("a different string"))
            {
                Assert.Same(lSourceValue, fnTo(lSourceValue, typeof(string), lPermation.ConvertOptions, lPermation.IgnoreError, lPermation.DefaultValue));
            }
        }

        [Fact(DisplayName = "Convert._To:ImmediateReturnTargetValueType")]
        public void ImmediateReturnTargetValueType()
        {
            int lIntValue = 42;

            foreach (var lPermation in GetOptionsPermutations(923))
            {
                Assert.Equal(lIntValue, fnTo(lIntValue, typeof(int), lPermation.ConvertOptions, lPermation.IgnoreError, lPermation.DefaultValue));
            }
        }

        [Fact(DisplayName = "Convert._To:Empty input for nullable returns null")]
        public void EmptyInputForNullableReturnsNull()
        {
            Type lNullableType = typeof(int?);

            foreach (var lPermation in GetOptionsPermutations((int?)342))
            {
                Assert.Null(fnTo(null, lNullableType, lPermation.ConvertOptions, lPermation.IgnoreError, lPermation.DefaultValue));
            }
        }

        [Fact(DisplayName = "Convert._To:Non-empty input for nullable returns underlying value (int)")]
        public void NonEmptyInputForNullableReturnsUnderlyingValue_Int()
        {
            Type lNullableType = typeof(int?);

            foreach (var lPermation in GetOptionsPermutations((int?)342))
            {
                foreach (object lInput in ConvertTestData.Int49Inputs)
                {
                    object lResult = fnTo(lInput, lNullableType, lPermation.ConvertOptions, lPermation.IgnoreError, lPermation.DefaultValue);
                    Assert.IsAssignableFrom<int>(lResult);
                    Assert.Equal(49, (int)lResult);
                }
            }
        }

        [Fact(DisplayName = "Convert._To:Non-empty input for nullable returns underlying value (enum)")]
        public void NonEmptyInputForNullableReturnsUnderlyingValue_Enum()
        {
            Type lNullableType = typeof(TestShortEnum?);

            foreach (var lPermation in GetOptionsPermutations((TestShortEnum?)TestShortEnum.One))
            {
                foreach (object lInput in ConvertTestData.Int49Inputs)
                {
                    object lResult = fnTo(lInput, lNullableType, lPermation.ConvertOptions, lPermation.IgnoreError, lPermation.DefaultValue);
                    Assert.IsAssignableFrom<TestShortEnum>(lResult);
                    Assert.Equal(TestShortEnum.FortyNine, (TestShortEnum)lResult);
                }
            }
        }

        [Fact(DisplayName = "Convert._To:NullToValueDefault")]
        public void NullToValueDefault()
        {
            // Int, no explicit default
            ConvertAssert.Equal(0, fnTo(null, typeof(int), ConvertOptions.NullToValueDefault, false, null));
            ConvertAssert.Equal(0, fnTo(DBNull.Value, typeof(int), ConvertOptions.NullToValueDefault, false, null));
            ConvertAssert.Equal(0, fnTo(string.Empty, typeof(int), ConvertOptions.NullToValueDefault | ConvertOptions.EmptyStringAsNull, false, null));

            // Int, explicit default
            ConvertAssert.Equal(42, fnTo(null, typeof(int), ConvertOptions.NullToValueDefault, false, 42));
            ConvertAssert.Equal(42, fnTo(DBNull.Value, typeof(int), ConvertOptions.NullToValueDefault, false, 42));
            ConvertAssert.Equal(42, fnTo(string.Empty, typeof(int), ConvertOptions.NullToValueDefault | ConvertOptions.EmptyStringAsNull, false, 42));

            // Enum, no explicit default
            ConvertAssert.Equal((TestShortEnum)0, fnTo(null, typeof(TestShortEnum), ConvertOptions.NullToValueDefault, false, null));
            ConvertAssert.Equal((TestShortEnum)0, fnTo(DBNull.Value, typeof(TestShortEnum), ConvertOptions.NullToValueDefault, false, null));
            ConvertAssert.Equal((TestShortEnum)0, fnTo(string.Empty, typeof(TestShortEnum), ConvertOptions.NullToValueDefault | ConvertOptions.EmptyStringAsNull, false, null));

            // Enum, explicit default
            ConvertAssert.Equal(TestShortEnum.One, fnTo(null, typeof(TestShortEnum), ConvertOptions.NullToValueDefault, false, TestShortEnum.One));
            ConvertAssert.Equal(TestShortEnum.One, fnTo(DBNull.Value, typeof(TestShortEnum), ConvertOptions.NullToValueDefault, false, TestShortEnum.One));
            ConvertAssert.Equal(TestShortEnum.One, fnTo(string.Empty, typeof(TestShortEnum), ConvertOptions.NullToValueDefault | ConvertOptions.EmptyStringAsNull, false, TestShortEnum.One));

        }

        // Ensure the IgnoreError causes the default value to be returned on null input, even WITHOUT the ConvertOptions.NullToValueDefault flag

        [Fact(DisplayName = "Convert._To:NullToValueDefault_IgnoreError")]
        public void NullToValueDefault_IgnoreError()
        {

            // Int, no explicit default
            ConvertAssert.Equal(0, fnTo(null, typeof(int), ConvertOptions.None, true, null));
            ConvertAssert.Equal(0, fnTo(DBNull.Value, typeof(int), ConvertOptions.None, true, null));
            ConvertAssert.Equal(0, fnTo(string.Empty, typeof(int), ConvertOptions.EmptyStringAsNull, true, null));

            // Int, explicit default
            ConvertAssert.Equal(42, fnTo(null, typeof(int), ConvertOptions.None, true, 42));
            ConvertAssert.Equal(42, fnTo(DBNull.Value, typeof(int), ConvertOptions.None, true, 42));
            ConvertAssert.Equal(42, fnTo(string.Empty, typeof(int), ConvertOptions.EmptyStringAsNull, true, 42));

            // Enum, no explicit default
            ConvertAssert.Equal((TestShortEnum)0, fnTo(null, typeof(TestShortEnum), ConvertOptions.None, true, null));
            ConvertAssert.Equal((TestShortEnum)0, fnTo(DBNull.Value, typeof(TestShortEnum), ConvertOptions.None, true, null));
            ConvertAssert.Equal((TestShortEnum)0, fnTo(string.Empty, typeof(TestShortEnum), ConvertOptions.EmptyStringAsNull, true, null));

            // Enum, explicit default
            ConvertAssert.Equal(TestShortEnum.One, fnTo(null, typeof(TestShortEnum), ConvertOptions.None, true, TestShortEnum.One));
            ConvertAssert.Equal(TestShortEnum.One, fnTo(DBNull.Value, typeof(TestShortEnum), ConvertOptions.None, true, TestShortEnum.One));
            ConvertAssert.Equal(TestShortEnum.One, fnTo(string.Empty, typeof(TestShortEnum), ConvertOptions.EmptyStringAsNull, true, TestShortEnum.One));

        }

        [Fact(DisplayName = "Convert._To:NullToValueTypeRaisesError")]
        public void NullToValueTypeRaisesError()
        {
            Assert.Throws<ArgumentNullException>(() => fnTo(null, typeof(int), ConvertOptions.None, false, null));
            Assert.Throws<ArgumentNullException>(() => fnTo(DBNull.Value, typeof(int), ConvertOptions.None, false, null));
            Assert.Throws<ArgumentNullException>(() => fnTo(string.Empty, typeof(int), ConvertOptions.EmptyStringAsNull, false, null));

            Assert.Throws<ArgumentNullException>(() => fnTo(null, typeof(TestShortEnum), ConvertOptions.None, false, null));
            Assert.Throws<ArgumentNullException>(() => fnTo(DBNull.Value, typeof(TestShortEnum), ConvertOptions.None, false, null));
            Assert.Throws<ArgumentNullException>(() => fnTo(string.Empty, typeof(TestShortEnum), ConvertOptions.EmptyStringAsNull, false, null));
        }

        [Fact(DisplayName = "Convert._To:IgnoreErrorsReturnsDefuault")]
        public void Invalid_IgnoreErrorsReturnsDefuault()
        {
            object lInput = new System.Data.DataTable();

            // Int, no explicit default
            ConvertAssert.Equal(0, fnTo(string.Empty, typeof(int), ConvertOptions.None, true, null));
            ConvertAssert.Equal(0, fnTo(lInput, typeof(int), ConvertOptions.None, true, null));

            // Int, explicit default
            ConvertAssert.Equal(42, fnTo(string.Empty, typeof(int), ConvertOptions.None, true, 42));
            ConvertAssert.Equal(42, fnTo(lInput, typeof(int), ConvertOptions.None, true, 42));

            // Enum, no explicit default
            ConvertAssert.Equal((TestShortEnum)0, fnTo(string.Empty, typeof(TestShortEnum), ConvertOptions.None, true, null));
            ConvertAssert.Equal((TestShortEnum)0, fnTo(lInput, typeof(TestShortEnum), ConvertOptions.None, true, null));

            // Enum, explicit default
            ConvertAssert.Equal(TestShortEnum.One, fnTo(string.Empty, typeof(TestShortEnum), ConvertOptions.None, true, TestShortEnum.One));
            ConvertAssert.Equal(TestShortEnum.One, fnTo(lInput, typeof(TestShortEnum), ConvertOptions.None, true, TestShortEnum.One));
        }

        [Fact(DisplayName = "Convert._To:HeedErrorsRaisesException")]
        public void Invalid_HeedErrorsRaisesException()
        {
            object lInput = new System.Data.DataTable();

            Assert.Throws<InvalidCastException>(() => fnTo(string.Empty, typeof(int), ConvertOptions.None, false, null));
            Assert.Throws<InvalidCastException>(() => fnTo(lInput, typeof(int), ConvertOptions.None, false, null));

            Assert.Throws<ArgumentException>(() => fnTo(string.Empty, typeof(TestShortEnum), ConvertOptions.None, false, null));
            Assert.Throws<InvalidCastException>(() => fnTo(lInput, typeof(TestShortEnum), ConvertOptions.None, false, null));
        }
    }
}
