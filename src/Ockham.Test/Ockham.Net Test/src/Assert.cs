using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace Ockham.Test
{
    /// <summary>
    /// Utility methods to aid in unit testing. 
    /// </summary>
    public static class Assert
    {
        /// <summary>
        /// Determine if two arrays have the same elements in the same order
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static bool ArraysEqual<T>(T[] a, T[] b)
        {
            if (a == null && b == null) return true;
            if (a == null || b == null) return false;
            if (a.Length != b.Length) return false;
            for (int i = 0; i < a.Length; i++)
            {
                if (!Object.Equals(a[i], b[i])) return false;
            }
            return true;
        }

        /// <summary>
        /// Test whether two arrays have the same elements in the same order
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expected"></param>
        /// <param name="actual"></param>
        /// <param name="fnAssertEqual"></param>
        public static void ArraysEqual<T>(T[] expected, T[] actual, Action<T, T> fnAssertEqual)
        {
            if (expected == null && actual == null) return;
            if (expected == null) throw new Exception("Expected null reference");
            if (actual == null) throw new Exception("Actual is a null reference");
            if (expected.Length != actual.Length) throw new Exception($"Expected length {expected.Length}, but actual length was {actual.Length}");
            for (int i = 0; i < expected.Length; i++)
            {
                fnAssertEqual(expected[i], actual[i]);
            }
        }

        /// <summary>
        /// Test whether invoking the provided <paramref name="action"/> raises and exception of type <typeparamref name="TException"/>
        /// </summary>
        /// <typeparam name="TException"></typeparam>
        /// <param name="action"></param>
        public static void Throws<TException>(Action action) where TException : Exception
        {
            Throws<TException>(null, action);
        }

        /// <summary>
        /// Test whether invoking the provided <paramref name="action"/> raises and exception of type <typeparamref name="TException"/>
        /// with a message matching <paramref name="errorPattern"/>
        /// </summary>
        /// <typeparam name="TException"></typeparam>
        /// <param name="errorPattern">A regular expression to match against the <see cref="System.Exception.Message"/> property of the raised exception</param>
        /// <param name="action"></param>
        public static void Throws<TException>(string errorPattern, Action action) where TException : Exception
        {
            Regex messageRx = null;
            if (errorPattern != null)
            {
                try
                {
                    messageRx = new Regex(errorPattern);
                }
                catch (Exception ex)
                {
                    throw new Exception("Error pattern '" + errorPattern + "' is not valid regular expressions pattern", ex);
                }
            }

            try
            {
                action();
            }
            catch (TException ex)
            {
                if (errorPattern == null)
                {
                    // Test passes
                    return;
                }
                else
                {
                    if (messageRx.IsMatch(ex.Message))
                    {
                        // Test passes
                        return;
                    }
                    else
                    {
                        throw new Exception(string.Format("Exception message '{0}' did not match expected pattern '{1}'", ex.Message, errorPattern), ex);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Action throw exception of type " + ex.GetType().Name, ex);
            }

            throw new Exception("Action did not throw an exception");
        }

        /// <summary>
        /// Test whether a value is exactly the same type and of equal value to an expected value
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expected"></param>
        /// <param name="actual"></param>
        /// <param name="fnAssertEqual"></param>
        public static void AreEqualSameType<T>(T expected, object actual, Action<T, T> fnAssertEqual)
        {
            if (actual == null)
            {
                if (expected == null) return;
                throw new Exception(string.Format("Actual value was null"));
            }
            else
            {
                if (expected == null) throw new Exception(string.Format("Expected null"));
            }

            if (actual.GetType() != typeof(T)) throw new Exception($"Provided value is of type {actual.GetType().FullName}, but expected value of type {typeof(T).FullName}");
            fnAssertEqual(expected, (T)actual);
        }
    }
}
