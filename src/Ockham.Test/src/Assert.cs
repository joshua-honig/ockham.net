using System;
using System.Text.RegularExpressions;

namespace Ockham.Test
{
    /// <summary>
    /// Utility methods to aid in unit testing. 
    /// </summary>
    public static class Assert
    {

        /// <summary>
        /// Test whether invoking the provided <paramref name="action"/> raises an exception of type <typeparamref name="TException"/>
        /// with a message matching <paramref name="errorPattern"/> 
        /// </summary>
        /// <typeparam name="TException"></typeparam>
        /// <param name="action"></param>
        /// <param name="errorPattern">A regular expression to match against the <see cref="System.Exception.Message"/> property of the raised exception</param> 
        public static void Throws<TException>(Action action, string errorPattern) where TException : Exception
        {
            Assert.Throws<TException>(action, errorPattern, null);
        }

        /// <summary>
        /// Test whether invoking the provided <paramref name="action"/> raises an exception of type <typeparamref name="TException"/>
        /// and also perform additional tests on the exception object
        /// </summary>
        /// <typeparam name="TException"></typeparam>
        /// <param name="action"></param> 
        /// <param name="exceptionTests">An action which performs additional test on the exception that was raised</param>
        public static void Throws<TException>(Action action, Action<TException> exceptionTests) where TException : Exception
        {
            Assert.Throws<TException>(action, null, exceptionTests);
        }

        /// <summary>
        /// Test whether invoking the provided <paramref name="action"/> raises an exception of type <typeparamref name="TException"/>
        /// with a message matching <paramref name="errorPattern"/>, and also perform additional tests on the exception object
        /// </summary>
        /// <typeparam name="TException"></typeparam>
        /// <param name="action"></param>
        /// <param name="errorPattern">A regular expression to match against the <see cref="System.Exception.Message"/> property of the raised exception</param>
        /// <param name="exceptionTests">An action which performs additional test on the exception that was raised</param>
        public static void Throws<TException>(Action action, string errorPattern, Action<TException> exceptionTests) where TException : Exception
        {
            if (action == null) throw new ArgumentNullException("action");

            Regex messageRx = null;
            if (errorPattern != null)
            {
                try
                {
                    messageRx = new Regex(errorPattern);
                }
                catch (Exception ex)
                {
                    throw new ArgumentException("Error pattern '" + errorPattern + "' is not valid regular expressions pattern", ex);
                }
            }

            try
            {
                action();
            }
            catch (TException ex)
            {
                if (errorPattern != null)
                {
                    if (!messageRx.IsMatch(ex.Message))
                    {
                        throw new ArgumentException(string.Format("Exception message '{0}' did not match expected pattern '{1}'", ex.Message, errorPattern), ex);
                    }
                }

                exceptionTests?.Invoke(ex);

                // Test passes
                return;
            }
            catch (Exception ex)
            {
                throw new Exception("Action threw exception of type " + ex.GetType().Name + ", which does not inherit from expected exception type " + typeof(TException).Name, ex);
            }

            throw new Exception("Action did not throw an exception");
        }
    }
}
