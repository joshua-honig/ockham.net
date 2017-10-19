# Ockham.Test
A small set of unit testing utilities. Ockham.Net uses Xunit for unit testing, including the unit tests of this 
library, but the utilities themselves are framework agnostic and have no dependencies.
  
## The Problem
Every Ockham component should solve a clear problem that is not solved in the .Net BCL, or in the particular libraries it is meant to augment. 

The utilities in this library address two simple problems encountered when unit testing: Testing exception properties and testing non-public methods.

### 1. `Assert.Throws`: Testing exception messages against patterns, arbitrary exception properties ###
All major .Net unit testing frameworks provide simple, semantic mechanisms for testing for expected exceptions
of a particular type. They do not provide means of testing the contents of the exception message, or other 
properties on the exception. The overloads of Ockham.Test.Assert.Throws provide mechanisms for doing one or both of these things.

The point here is that the content of error messages and / or additional information attached to an exeption
represent testable requirements in and of themselves. Such requirements can be tested by hand:

	[Test/Fact/TestMethod/etc.]
    public void ExceptionTest() 
	{
		bool exceptionWasThrown = false;
		try 
		{
			/* do stuff */
		} 
		catch (ExpectedExceptionType ex) 
		{
			exceptionWasThrown = true;

			// TEST: Was the correct helpful exception message produced?
			Assert.True(Regex.IsMatch(ex.Message, @"some pattern"));

			// TEST: Was the HelpfulProperty correctly set?
			Assert.Equal("expected value", ex.HelpfulProperty)
		}

		Assert.True(exceptionWasThrown); 
	}

Writing this more succinctly and semantically is helpful for the same reason that all the frameworks provide 
a succinct way of testing for an expected exception by type

    [Test/Fact/TestMethod/etc.]
    public void ExceptionTest() 
	{
		// Simple test for message pattern is still just one line
		Ockham.Test.Assert.Throws(() => /* do stuff */, @"some pattern");

		// So is a test for a single arbitrary assertion about the exception itself
		Ockham.Test.Assert.Throws(() => /* do stuff */, ex => Assert.Equal("expected value", ex.HelpfulProperty));

		// It may be a matter of taste whether the following is more or less "readable" than the long form:
		Ockham.Test.Assert.Throws(() => /* do stuff */, @"some pattern", ex => {
			Assert.Equal("expected value", ex.HelpfulProperty);
			Assert.NotNull(ex.ImportantData);
		});
	}

### 2. `MethodReflection`: Testing private and protected methods ###
Yes, yes, in general you should not need to test private and protected methods. They are 
implementation details irrelevant to the testable public surface area of your code. 

Except when they are not. Like when you want to compose the public surface area from 
several important but private / protected / internal components. A good example is the private static 
_To method in the Ockham.Convert class. It provides a single, consolidated logic for fast but 
flexible basic data type conversions, but its full signature is not meant to be public. Instead, 
a wide range of public methods expose different variations of the underlying conversion logic. 
That project includes a wide array of tests on those public conversion APIs, but it ALSO include
lots of tests to ensure that this single crucial method works exactly as intended. 

The static `Ockham.Test.MethodReflection` class provides simple ways of generating callable delegates for
non-public methods by matching the signature against a strongly typed delegate. The idea here is *you* wrote the code, so *you* 
know the details of the method you want to test. In fact, you can mostly copy and paste the signature of the 
actual method into your test class, replace the access keyword with 'delegate', and you are good to go:

    // In some useful class
	public static class VeryUseful {
		private static bool _Implementation(int someArg, string anotherArg, Dictionary<long, Thing> thingMap, out failureMessage) {
			/* code ... */
		}

		protected string DoStuff(int someArg, string anotherArg, params Thing[] things) {
			/* code ... */
		}
	}

	// In your test class
	public class VeryUsefulTests {
	
		// Copied and pasted from VeryUseful.cs:
		delegate bool _Implementation(int someArg, string anotherArg, Dictionary<long, Thing> thingMap, out failureMessage);
		delegate bool ImplementationDelegate(int someArg, string anotherArg, Dictionary<long, Thing> thingMap, out failureMessage);
		delegate string DoStuff(int someArg, string anotherArg, params Thing[] things);

		[Test/Fact/TestMethod/etc.]
		public void Implementation_Case1() 
		{
			// If the name of the delegate type matches the target method, you don't need to provide the name
			var _impl = Ockham.Test.MethodReflection.GetMethodCaller<_Implementation, VeryUseful>();

			// But you can always specify the name explicitly
			var impl2 = Ockham.Test.MethodReflection.GetMethodCaller<ImplementationDelegate, VeryUseful>("_Implementation");

			// Instance methods of course are also supported
			var useful = new VeryUseful();
			var doStuff = Ockham.Test.MethodReflection.GetMethodCaller<DoStuff, VeryUseful>(useful);
		}
	}

The docs provide more complete examples.