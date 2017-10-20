using System;
using System.Text.RegularExpressions;
using Xunit;
using OAssert = Ockham.Test.Assert;
using XAssert = Xunit.Assert;

namespace Ockham.Test.Test
{
    public class Assert
    {
        public class Throws
        {
            private const string ExceptionMessage = "This is an argument exception";
            private const string ParamName = "theParameter";
            private const long ActualValue = 42;
            private static readonly string ExceptionExactPattern = "^" + Regex.Escape(ExceptionMessage) + "$";
            private static readonly string ExceptionPattern = @"(?i:THIS\s+IS.+ARG)";
            private static readonly string NotMatchingPattern = @"^\dABC$";

            private class MyArgumentException : ArgumentException
            {
                private string _paramName;
                public override string ParamName => _paramName;

                public object ActualValue { get; private set; }

                public MyArgumentException(string paramName, object actualValue, string message) : base(message)
                {
                    this._paramName = paramName;
                    this.ActualValue = actualValue;
                }
            }

            private static void ThrowException()
            {
                throw new MyArgumentException(ParamName, ActualValue, ExceptionMessage);
            }

            [Fact(DisplayName = "Assert.Throws:Match exception message pattern")]
            public void MatchMessagePattern()
            {
                // This should NOT throw an exception (Ockham.Assert.Throws passes)
                // Exact exception type and message
                OAssert.Throws<MyArgumentException>(ThrowException, ExceptionExactPattern);
                // Verifying we got this far
                XAssert.True(true);

                // This should NOT throw an exception (Ockham.Assert.Throws passes)
                // Exact exception type, pattern match
                OAssert.Throws<MyArgumentException>(ThrowException, ExceptionPattern);
                // Verifying we got this far
                XAssert.True(true);

                // This should NOT throw an exception (Ockham.Assert.Throws passes)
                // Parent exception type, exact message
                OAssert.Throws<ArgumentException>(ThrowException, ExceptionExactPattern);
                // Verifying we got this far
                XAssert.True(true);

                // This should NOT throw an exception (Ockham.Assert.Throws passes)
                // Parent exception type, pattern match
                OAssert.Throws<ArgumentException>(ThrowException, ExceptionPattern);
                // Verifying we got this far
                XAssert.True(true);
            }

            [Fact(DisplayName = "Assert.Throws:Reject exception message pattern mismatch")]
            public void RejectMessagePattern()
            {
                Action fnFailAORException = () => OAssert.Throws<MyArgumentException>(ThrowException, NotMatchingPattern);
                Action fnFailArgException = () => OAssert.Throws<ArgumentException>(ThrowException, NotMatchingPattern);

                XAssert.Throws<ArgumentException>(fnFailAORException);
                XAssert.Throws<ArgumentException>(fnFailArgException);

                string expectedMessage = string.Format("Exception message '{0}' did not match expected pattern '{1}'", ExceptionMessage, NotMatchingPattern);
                try
                {
                    fnFailAORException();
                    throw new Exception("Action did not throw an exception!");
                }
                catch (Exception ex)
                {
                    XAssert.Equal(expectedMessage, ex.Message);
                }

                try
                {
                    fnFailArgException();
                    throw new Exception("Action did not throw an exception!");
                }
                catch (Exception ex)
                {
                    XAssert.Equal(expectedMessage, ex.Message);
                }
            }

            [Fact(DisplayName = "Assert.Throws:Reject missing action")]
            public void RejectMissingAction()
            {
                XAssert.Throws<ArgumentNullException>(() => OAssert.Throws<Exception>(null, ""));
            }

            [Fact(DisplayName = "Assert.Throws:Reject invalid regex")]
            public void RejectInvalidRegex()
            {
                Action fnFailPattern = () => OAssert.Throws<Exception>(ThrowException, "^(");

                XAssert.Throws<ArgumentException>(fnFailPattern);

                string expectedMessage = "Error pattern '^(' is not valid regular expressions pattern";
                try
                {
                    fnFailPattern();
                    throw new Exception("Action did not throw an exception!");
                }
                catch (Exception ex)
                {
                    XAssert.Equal(expectedMessage, ex.Message);
                }
            }

            [Fact(DisplayName = "Assert.Throws:Reject wrong exception type")]
            public void RejectWrongExType()
            {
                Action fnFailType = () => OAssert.Throws<DivideByZeroException>(ThrowException, ExceptionExactPattern);

                XAssert.Throws<Exception>(fnFailType);

                string expectedMessage = "Action threw exception of type " + typeof(MyArgumentException).Name + ", which does not inherit from expected exception type " + typeof(DivideByZeroException).Name;
                try
                {
                    fnFailType();
                    throw new Exception("Action did not throw an exception!");
                }
                catch (Exception ex)
                {
                    XAssert.Equal(expectedMessage, ex.Message);
                }
            }

            [Fact(DisplayName = "Assert.Throws:Reject no exception thrown")]
            public void RejectNoException()
            {
                Action fnFailNoException = () => OAssert.Throws<DivideByZeroException>(() => { }, ExceptionExactPattern);

                XAssert.Throws<Exception>(fnFailNoException);

                string expectedMessage = "Action did not throw an exception";
                try
                {
                    fnFailNoException();
                    throw new Exception("Action did not throw an exception!");
                }
                catch (Exception ex)
                {
                    XAssert.Equal(expectedMessage, ex.Message);
                }
            }

            [Fact(DisplayName = "Assert.Throws:Execute exception test")]
            public void ExecuteTest()
            {
                bool closureCalled = false;
                MyArgumentException exCaptured = null;

                Action<MyArgumentException> test = ex =>
                {
                    closureCalled = true;
                    exCaptured = ex;
                };

                OAssert.Throws<MyArgumentException>(ThrowException, test);
                // Verifying we got this far
                XAssert.True(true);

                // Test that test was called:
                XAssert.True(closureCalled);
                XAssert.NotNull(exCaptured);
                XAssert.Equal(ExceptionMessage, exCaptured.Message);
            }

            [Fact(DisplayName = "Assert.Throws:Reject failed exception test")]
            public void RejectFailedTest()
            {
                bool closureCalled = false;
                MyArgumentException exCaptured = null;

                Action<MyArgumentException> test = ex =>
                {
                    closureCalled = true;
                    exCaptured = ex;
                    XAssert.Equal("no", ex.ActualValue);
                };

                Action fnFailTest = () => OAssert.Throws<MyArgumentException>(ThrowException, test);

                XAssert.Throws<Xunit.Sdk.EqualException>(fnFailTest);

                // Test that test was called:
                XAssert.True(closureCalled);
                XAssert.NotNull(exCaptured);
                XAssert.Equal(ExceptionMessage, exCaptured.Message);
            }

        }

    }
}
