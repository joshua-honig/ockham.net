using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using System.Diagnostics.CodeAnalysis;

namespace Ockham.Test.Mocks
{

#if NETCOREAPP1_0
#else
    [ExcludeFromCodeCoverage]
#endif
    public class TestClass
    {
        public const string StringConstant = "String constant";

        protected static void ProtectedStatic() { }
        protected static void ProtectedStatic(string stringArg) { }
        protected static void ProtectedStatic(ref int intArg) { intArg = 42; } 
        protected static void ProtectedStatic(int intArgIn, out int intArgOut) { intArgOut = 2 * intArgIn; }
        protected static void ProtectedStatic(int intArg, params object[] paramArgs) { }

        private static string PrivateStatic() { return StringConstant; }
        private static string PrivateStatic(int count)
        {
            if (count <= 0) return "";
            if (count == 1) return StringConstant;
            string[] arr = new string[count];
            for (int i = 0; i < count; i++)
            {
                arr[i] = StringConstant;
            }
            return string.Join("", arr);
        }

        public string Name { get; private set; }

        public TestClass(string name)
        {
            this.Name = name;
        }

        private void PrivateInstance() { }
        private void PrivateInstance(int intArg) { }

        protected string ProtectedInstance() { return this.Name; }
        protected string ProtectedInstance(int count) { return this.Name + PrivateStatic(count); }
    }
}
