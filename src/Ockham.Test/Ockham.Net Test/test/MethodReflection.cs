using Ockham.Test.Mocks;
using System;
using System.Reflection;
using Xunit;
using MR = Ockham.Test.MethodReflection;
using XAssert = Xunit.Assert;

namespace Ockham.Test.Test
{
    public class MethodReflection
    {
        delegate void ProtectedStatic();
        delegate void ProtectedStatic_string(string stringArg);
        delegate void ProtectedStatic_int(int intArg);
        delegate void ProtectedStatic_refint(ref int intArg);
        delegate void ProtectedStatic_outint(out int intArg);
        delegate void ProtectedStatic_inoutint(int intArgIn, out int intArgOut);
        delegate void ProtectedStatic_inrefint(int intArgIn, ref int intArgOut);
        delegate void ProtectedStatic_params(int intArg, params object[] paramArgs);

        delegate string PrivateStatic();
        delegate string PrivateStatic_int(int count);

        delegate void PrivateInstance();
        delegate void PrivateInstance_int(int intArg);

        delegate string ProtectedInstance();
        delegate string ProtectedInstance_int(int intArg);

        public class GetMethodInfo
        {

            [Fact(DisplayName = "MethodReflection.GetMethodInfo:Find protected static methods by inferred name")]
            public void ProtectedStatic_InferredName()
            {
                MethodInfo methodInfo = null;

                // Fully generic overload
                methodInfo = MR.GetMethodInfo<TestClass, ProtectedStatic>(true);
                XAssert.NotNull(methodInfo);

                // Partially generic overload
                methodInfo = MR.GetMethodInfo<ProtectedStatic>(typeof(TestClass), true);
                XAssert.NotNull(methodInfo);
            }

            [Fact(DisplayName = "MethodReflection.GetMethodInfo:Find protected static methods by name")]
            public void ProtectedStatic_ExplicitName()
            {
                MethodInfo methodInfo = null;

                // Case-sensitive name
                methodInfo = MR.GetMethodInfo<ProtectedStatic_string>(typeof(TestClass), "ProtectedStatic", true);
                XAssert.NotNull(methodInfo);

                // Case-sensitive name - should fail 
                XAssert.Throws<MissingMethodException>(() => MR.GetMethodInfo<ProtectedStatic_string>(typeof(TestClass), "protectedstatic", true));

                // Case-insensitive name 
                methodInfo = MR.GetMethodInfo<ProtectedStatic_string>(typeof(TestClass), "protectedstatic", true, true);
                XAssert.NotNull(methodInfo);
            }

            [Fact(DisplayName = "MethodReflection.GetMethodInfo:Find private static methods by inferred name")]
            public void PrivateStatic_InferredName()
            {
                MethodInfo methodInfo = null;

                // Fully generic overload
                methodInfo = MR.GetMethodInfo<TestClass, PrivateStatic>(true);
                XAssert.NotNull(methodInfo);

                // Partially generic overload
                methodInfo = MR.GetMethodInfo<PrivateStatic>(typeof(TestClass), true);
                XAssert.NotNull(methodInfo);
            }

            [Fact(DisplayName = "MethodReflection.GetMethodInfo:Find private static methods by name")]
            public void PrivateStatic_ExplicitName()
            {
                MethodInfo methodInfo = null;

                // Case-sensitive name
                methodInfo = MR.GetMethodInfo<PrivateStatic_int>(typeof(TestClass), "PrivateStatic", true);
                XAssert.NotNull(methodInfo);

                // Case-sensitive name - should fail 
                XAssert.Throws<MissingMethodException>(() => MR.GetMethodInfo<PrivateStatic_int>(typeof(TestClass), "privatestatic", true));

                // Case-insensitive name 
                methodInfo = MR.GetMethodInfo<PrivateStatic_int>(typeof(TestClass), "privatestatic", true, true);
                XAssert.NotNull(methodInfo);
            }

            [Fact(DisplayName = "MethodReflection.GetMethodInfo:Find static methods with ref parameter")]
            public void ProtectedStatic_RefParam()
            {
                // Signature with normal param should not match signature with ref param
                XAssert.Throws<MissingMethodException>(() => MR.GetMethodInfo<ProtectedStatic_int>(typeof(TestClass), "ProtectedStatic", true));

                // Signature with out param should not match signature with ref param
                XAssert.Throws<MissingMethodException>(() => MR.GetMethodInfo<ProtectedStatic_outint>(typeof(TestClass), "ProtectedStatic", true));

                // Match on ref
                var methodInfo = MR.GetMethodInfo<ProtectedStatic_refint>(typeof(TestClass), "ProtectedStatic", true, true);
                XAssert.NotNull(methodInfo);
            }

            [Fact(DisplayName = "MethodReflection.GetMethodInfo:Find static methods with out parameter")]
            public void ProtectedStatic_OutParam()
            {
                // Signature with out param should not match signature with ref param
                XAssert.Throws<MissingMethodException>(() => MR.GetMethodInfo<ProtectedStatic_inrefint>(typeof(TestClass), "ProtectedStatic", true));

                var methodInfo = MR.GetMethodInfo<ProtectedStatic_inoutint>(typeof(TestClass), "ProtectedStatic", true);
                XAssert.NotNull(methodInfo);
            }

            [Fact(DisplayName = "MethodReflection.GetMethodInfo:Find protected instance methods by inferred name")]
            public void ProtectedInstance_InferredName()
            {
                MethodInfo methodInfo = null;

                // Partially generic overload
                methodInfo = MR.GetMethodInfo<ProtectedInstance>(typeof(TestClass), false);
                XAssert.NotNull(methodInfo);
            }

            [Fact(DisplayName = "MethodReflection.GetMethodInfo:Find protected instance methods by name")]
            public void ProtectedInstance_ExplicitName()
            {
                MethodInfo methodInfo = null;

                // Case-sensitive name
                methodInfo = MR.GetMethodInfo<ProtectedInstance_int>(typeof(TestClass), "ProtectedInstance", false);
                XAssert.NotNull(methodInfo);

                // Case-sensitive name - should fail 
                XAssert.Throws<MissingMethodException>(() => MR.GetMethodInfo<ProtectedInstance_int>(typeof(TestClass), "protectedinstance", false));

                // Case-insensitive name 
                methodInfo = MR.GetMethodInfo<ProtectedInstance_int>(typeof(TestClass), "protectedinstance", false, true);
                XAssert.NotNull(methodInfo);
            }

            [Fact(DisplayName = "MethodReflection.GetMethodInfo:Find private instance methods by inferred name")]
            public void PrivateInstance_InferredName()
            {
                MethodInfo methodInfo = null;

                // Partially generic overload
                methodInfo = MR.GetMethodInfo<PrivateInstance>(typeof(TestClass), false);
                XAssert.NotNull(methodInfo);
            }

            [Fact(DisplayName = "MethodReflection.GetMethodInfo:Find private instance methods by name")]
            public void PrivateInstance_ExplicitName()
            {
                MethodInfo methodInfo = null;

                // Case-sensitive name
                methodInfo = MR.GetMethodInfo<PrivateInstance_int>(typeof(TestClass), "PrivateInstance", false);
                XAssert.NotNull(methodInfo);

                // Case-sensitive name - should fail 
                XAssert.Throws<MissingMethodException>(() => MR.GetMethodInfo<PrivateInstance_int>(typeof(TestClass), "privateinstance", false));

                // Case-insensitive name 
                methodInfo = MR.GetMethodInfo<PrivateInstance_int>(typeof(TestClass), "privateinstance", false, true);
                XAssert.NotNull(methodInfo);
            }
        }

        public class GetMethodCaller
        {

            private const string THE_NAME = "The name";

            [Fact(DisplayName = "MethodReflection.GetMethodCaller:Invoke protected static methods by inferred name")]
            public void ProtectedStatic_InferredName()
            {
                ProtectedStatic dg = null;

                // Fully generic overload
                dg = MR.GetMethodCaller<TestClass, ProtectedStatic>();
                XAssert.NotNull(dg);

                // Partially generic overload
                dg = MR.GetMethodCaller<ProtectedStatic>(typeof(TestClass));
                XAssert.NotNull(dg);
            }

            [Fact(DisplayName = "MethodReflection.GetMethodCaller:Invoke protected static methods by name")]
            public void ProtectedStatic_ExplicitName()
            {
                ProtectedStatic_string dg = null;

                // Case-sensitive name
                dg = MR.GetMethodCaller<ProtectedStatic_string>(typeof(TestClass), "ProtectedStatic");
                XAssert.NotNull(dg);

                // Case-sensitive name - should fail 
                XAssert.Throws<MissingMethodException>(() => MR.GetMethodCaller<ProtectedStatic_string>(typeof(TestClass), "protectedstatic"));

                // Case-insensitive name 
                dg = MR.GetMethodCaller<ProtectedStatic_string>(typeof(TestClass), "protectedstatic", true);
                XAssert.NotNull(dg);
            }

            [Fact(DisplayName = "MethodReflection.GetMethodCaller:Invoke private static methods by inferred name")]
            public void PrivateStatic_InferredName()
            {
                PrivateStatic dg = null;

                // Fully generic overload
                dg = MR.GetMethodCaller<TestClass, PrivateStatic>();
                XAssert.NotNull(dg);
                XAssert.Equal(TestClass.StringConstant, dg());

                // Partially generic overload
                dg = MR.GetMethodCaller<PrivateStatic>(typeof(TestClass));
                XAssert.NotNull(dg);
                XAssert.Equal(TestClass.StringConstant, dg());
            }

            [Fact(DisplayName = "MethodReflection.GetMethodCaller:Invoke private static methods by name")]
            public void PrivateStatic_ExplicitName()
            {
                PrivateStatic_int dg = null;
                string lConst3 = TestClass.StringConstant + TestClass.StringConstant + TestClass.StringConstant;

                // Case-sensitive name
                dg = MR.GetMethodCaller<PrivateStatic_int>(typeof(TestClass), "PrivateStatic");
                XAssert.NotNull(dg);
                XAssert.Equal(lConst3, dg(3));

                // Case-sensitive name - should fail 
                XAssert.Throws<MissingMethodException>(() => MR.GetMethodCaller<PrivateStatic_int>(typeof(TestClass), "privatestatic"));

                // Case-insensitive name 
                dg = MR.GetMethodCaller<PrivateStatic_int>(typeof(TestClass), "privatestatic", true);
                XAssert.NotNull(dg);
                XAssert.Equal(lConst3, dg(3));
            }

            [Fact(DisplayName = "MethodReflection.GetMethodCaller:Invoke static methods with ref parameter")]
            public void ProtectedStatic_RefParam()
            {
                // Signature with normal param should not match signature with ref param
                XAssert.Throws<MissingMethodException>(() => MR.GetMethodCaller<ProtectedStatic_int>(typeof(TestClass), "ProtectedStatic"));

                // Signature with out param should not match signature with ref param
                XAssert.Throws<MissingMethodException>(() => MR.GetMethodCaller<ProtectedStatic_outint>(typeof(TestClass), "ProtectedStatic"));

                // Match on ref
                var dg = MR.GetMethodCaller<ProtectedStatic_refint>(typeof(TestClass), "ProtectedStatic");
                XAssert.NotNull(dg);

                int val = 0;
                dg(ref val);
                XAssert.Equal(42, val);
            }

            [Fact(DisplayName = "MethodReflection.GetMethodCaller:Invoke static methods with out parameter")]
            public void ProtectedStatic_OutParam()
            {
                // Signature with out param should not match signature with ref param
                XAssert.Throws<MissingMethodException>(() => MR.GetMethodCaller<ProtectedStatic_inrefint>(typeof(TestClass), "ProtectedStatic"));

                var dg = MR.GetMethodCaller<ProtectedStatic_inoutint>(typeof(TestClass), "ProtectedStatic");
                XAssert.NotNull(dg);

                int val = 0;
                dg(21, out val);
                XAssert.Equal(42, val);
            }

            [Fact(DisplayName = "MethodReflection.GetMethodCaller:Invoke protected instance methods by inferred name")]
            public void ProtectedInstance_InferredName()
            {
                ProtectedInstance dg = null;
                TestClass instance = new TestClass(THE_NAME);

                // Partially generic overload
                dg = MR.GetMethodCaller<ProtectedInstance>(instance, typeof(TestClass));
                XAssert.NotNull(dg);
                XAssert.Equal(THE_NAME, dg());
            }

            [Fact(DisplayName = "MethodReflection.GetMethodCaller:Invoke protected instance methods by name")]
            public void ProtectedInstance_ExplicitName()
            {
                ProtectedInstance_int dg = null;
                TestClass instance = new TestClass(THE_NAME);
                string lExpected = THE_NAME + TestClass.StringConstant + TestClass.StringConstant + TestClass.StringConstant;

                // Case-sensitive name
                dg = MR.GetMethodCaller<ProtectedInstance_int>(instance, typeof(TestClass), "ProtectedInstance");
                XAssert.NotNull(dg);
                XAssert.Equal(lExpected, dg(3));

                // Case-sensitive name - should fail 
                XAssert.Throws<MissingMethodException>(() => MR.GetMethodCaller<ProtectedInstance_int>(typeof(TestClass), "protectedinstance"));

                // Case-insensitive name 
                dg = MR.GetMethodCaller<ProtectedInstance_int>(instance, typeof(TestClass), "protectedinstance", true);
                XAssert.NotNull(dg);
                XAssert.Equal(lExpected, dg(3));
            }

            [Fact(DisplayName = "MethodReflection.GetMethodCaller:Invoke private instance methods by inferred name")]
            public void PrivateInstance_InferredName()
            {
                PrivateInstance dg = null;
                TestClass instance = new TestClass(THE_NAME);

                // Partially generic overload
                dg = MR.GetMethodCaller<PrivateInstance>(instance, typeof(TestClass));
                XAssert.NotNull(dg);
            }

            [Fact(DisplayName = "MethodReflection.GetMethodCaller:Invoke private instance methods by name")]
            public void PrivateInstance_ExplicitName()
            {
                PrivateInstance_int dg = null;
                TestClass instance = new TestClass(THE_NAME);

                // Case-sensitive name
                dg = MR.GetMethodCaller<PrivateInstance_int>(instance, typeof(TestClass), "PrivateInstance");
                XAssert.NotNull(dg);

                // Case-sensitive name - should fail 
                XAssert.Throws<MissingMethodException>(() => MR.GetMethodCaller<PrivateInstance_int>(instance, typeof(TestClass), "privateinstance"));

                // Case-insensitive name 
                dg = MR.GetMethodCaller<PrivateInstance_int>(instance, typeof(TestClass), "privateinstance", true);
                XAssert.NotNull(dg);
            }
        }
    }
}
