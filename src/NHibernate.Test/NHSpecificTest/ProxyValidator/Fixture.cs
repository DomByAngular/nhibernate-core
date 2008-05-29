using System;
using System.Collections;
using NHibernate.Proxy;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.ProxyValidator
{
	[TestFixture]
	public class Fixture
	{
		private void Validate(System.Type type)
		{
			ICollection errors = ProxyTypeValidator.ValidateType(type);
			if (errors != null)
			{
				throw new InvalidProxyTypeException(errors);
			}
		}

		public class ValidClass
		{
			private int privateField;
			protected int protectedField;

			public virtual int SomeProperty
			{
				get { return privateField; }
				set { privateField = value; }
			}

			public virtual void SomeMethod(int arg1, object arg2)
			{
			}

			public virtual event EventHandler VirtualEvent;

			protected void NonVirtualProtectedMethod()
			{
			}

			protected int NonVirtualProtectedProperty
			{
				get { return 0; }
				set { }
			}

			protected event EventHandler NonVirtualProtectedEvent;

			protected void NonVirtualPrivateMethod()
			{
			}

			protected int NonVirtualPrivateProperty
			{
				get { return 0; }
				set { }
			}

			protected event EventHandler NonVirtualPrivateEvent;
		}

		[Test]
		public void ValidClassTest()
		{
			Validate(typeof(ValidClass));
		}

		public class InvalidPrivateConstructor : ValidClass
		{
			private InvalidPrivateConstructor()
			{
			}
		}

		[Test]
		[ExpectedException(typeof(InvalidProxyTypeException))]
		public void PrivateConstructor()
		{
			Validate(typeof(InvalidPrivateConstructor));
		}

		public class InvalidNonVirtualProperty : ValidClass
		{
			public int NonVirtualProperty
			{
				get { return 1; }
				set { }
			}
		}

		[Test]
		[ExpectedException(typeof(InvalidProxyTypeException))]
		public void NonVirtualProperty()
		{
			Validate(typeof(InvalidNonVirtualProperty));
		}

		public class InvalidPublicField : ValidClass
		{
			public int publicField;
		}

		[Test]
		[ExpectedException(typeof(InvalidProxyTypeException))]
		public void PublicField()
		{
			Validate(typeof(InvalidPublicField));
		}

		public class InvalidNonVirtualEvent : ValidClass
		{
			public event EventHandler NonVirtualEvent;
		}

		[Test]
		[ExpectedException(typeof(InvalidProxyTypeException))]
		public void NonVirtualEvent()
		{
			Validate(typeof(InvalidNonVirtualEvent));
		}

		public interface ValidInterface
		{
		}

		[Test]
		public void Interface()
		{
			Validate(typeof(ValidInterface));
		}

		public class MultipleErrors
		{
			private MultipleErrors()
			{
			}

			public int publicField;
			public event EventHandler NonVirtualEvent;

			public int NonVirtualProperty
			{
				get { return 1; }
				set { }
			}
		}

		[Test]
		public void MultipleErrorsReported()
		{
			try
			{
				Validate(typeof(MultipleErrors));
				Assert.Fail("Should have failed validation");
			}
			catch (InvalidProxyTypeException e)
			{
				Assert.IsTrue(e.Errors.Count > 1);
			}
		}

		public class InvalidNonVirtualInternalProperty : ValidClass
		{
			internal int NonVirtualProperty
			{
				get { return 1; }
				set { }
			}
		}

		public class InvalidInternalField : ValidClass
		{
			internal int internalField;
		}

		[Test]
		[ExpectedException(typeof(InvalidProxyTypeException))]
		public void NonVirtualInternal()
		{
			Validate(typeof(InvalidNonVirtualInternalProperty));
		}

		[Test]
		[ExpectedException(typeof(InvalidProxyTypeException))]
		public void InternalField()
		{
			Validate(typeof(InvalidInternalField));
		}

		public class InvalidNonVirtualProtectedProperty : ValidClass
		{
			protected int NonVirtualProperty
			{
				get { return 1; }
				set { }
			}
		}

		[Test]
		public void NonVirtualProtected()
		{
			Validate(typeof(InvalidNonVirtualProtectedProperty));
			Assert.IsTrue(true, "Always should pass, protected members do not need to be virtual.");
		}

		public class InvalidNonVirtualProtectedInternalProperty : ValidClass
		{
			protected internal int NonVirtualProperty
			{
				get { return 1; }
				set { }
			}
		}

		[Test]
		[ExpectedException(typeof(InvalidProxyTypeException))]
		public void NonVirtualProtectedInternal()
		{
			Validate(typeof(InvalidNonVirtualProtectedInternalProperty));
		}
	}
}