
using System;
using System.Collections.Generic;

namespace Integration.Story.Expressions
{
    using NUnit.Framework;

	using Voltage.Story.Expressions;

	using Voltage.Story.Mapper;


    [TestFixture]
    public class VariableExpressionTest
    {

		public class VariableTestMapper : IMapping<string>
		{
			public IDictionary<string,object> Map = new Dictionary<string,object> 
			{
				{"3", 3},
				{"0", 0},
				{"-5", -5},
				{"foobar", "foobar"},
				{"hello world", "hello world"},
				{"three", "3"},
				{"negtwo", "-2"},
				{"true", true},
				{"false", false},
			};

			public bool TryGetValue(string key, out object value)
			{
				object v;
				if (Map.TryGetValue(key, out v))
				{
					value = v;
					return true;
				}

				value = null;
				return false;
			}

			public bool TryGetValue(string key, out int value)
			{
				object v;
				bool success = TryGetValue(key, out v);

				if (!success)
				{
					value = 0;
					return false;
					
				}

				if (v is int)
				{
					value = (int)v;
				}
				else {
					value = (int)Convert.ChangeType(v, typeof(int));
				}

				return true;
			}

			public bool TryGetValue<T> (string key, out T value)
			{
				object v;
				if(Map.TryGetValue(key, out v))
				{
					value = (T)Convert.ChangeType(v, typeof(T));		// value = (T)v;
					return true;
				}
				else
				{
					value = default(T);
					return false;
				}
			}
		}

		[Test]
		public void TestGreaterThanVariableVariableIntTrue()
		{
			VariableTestMapper variableMap = new VariableTestMapper ();

			Expression expA = new Expression (new DefaultVariableExpression ("3", variableMap), new DefaultVariableExpression ("0", variableMap), new GreaterThanOperator ());
			Assert.That (expA.Evaluate(), Is.True);
		}

		[Test]
		public void TestGreaterThanVariableVariableIntFalse()
		{
			VariableTestMapper variableMap = new VariableTestMapper ();
			
            Expression expA = new Expression (new DefaultVariableExpression("-5", variableMap), new DefaultVariableExpression("0", variableMap), new GreaterThanOperator ());
			Assert.That (expA.Evaluate(), Is.False);
		}

		[Test]
		public void TestGreaterThanVariableNumeralIntTrue()
		{
			VariableTestMapper variableMap = new VariableTestMapper ();
			
            Expression expA = new Expression (new DefaultVariableExpression("-5", variableMap), new NumeralExpression(-10), new GreaterThanOperator ());
			Assert.That (expA.Evaluate(), Is.True);

            Expression expB = new Expression (new DefaultVariableExpression("three", variableMap), new NumeralExpression(-10), new GreaterThanOperator ());
			Assert.That (expB.Evaluate(), Is.True);
		}



		[Test]
		public void TestGreaterThanVariableNumeralIntFalse()
		{
			VariableTestMapper variableMap = new VariableTestMapper ();
			
            Expression expA = new Expression (new DefaultVariableExpression("-5", variableMap), new NumeralExpression(10), new GreaterThanOperator ());
			Assert.That (expA.Evaluate(), Is.False);

            Expression expB = new Expression (new DefaultVariableExpression("three", variableMap), new NumeralExpression(10), new GreaterThanOperator ());
			Assert.That (expB.Evaluate(), Is.False);
		}


		[Test]
		public void TestEqualVariableVariableTrue()
		{
			VariableTestMapper variableMap = new VariableTestMapper ();
			
            Expression expA = new Expression (new DefaultVariableExpression("foobar", variableMap), new DefaultVariableExpression("foobar", variableMap), new EqualToOperator());
			Assert.That (expA.Evaluate(), Is.True);
		}
		
		[Test]
		public void TestEqualVariableVariableFalse()
		{
			VariableTestMapper variableMap = new VariableTestMapper ();
			
            Expression expA = new Expression (new DefaultVariableExpression("foobar", variableMap), new DefaultVariableExpression("hello world", variableMap), new EqualToOperator());
			Assert.That (expA.Evaluate(), Is.False);
		}



		[Test]
		public void TestEqualVariableStringTrue()
		{
			VariableTestMapper variableMap = new VariableTestMapper ();

            Expression expA = new Expression (new DefaultVariableExpression("foobar", variableMap), new StringExpression("foobar"), new EqualToOperator());
			Assert.That (expA.Evaluate(), Is.True);
		}
		
		[Test]
		public void TestEqualVariableStringFalse()
		{
			VariableTestMapper variableMap = new VariableTestMapper ();
			
            Expression expA = new Expression (new DefaultVariableExpression("foobar", variableMap), new StringExpression("baz"), new EqualToOperator());
			Assert.That (expA.Evaluate(), Is.False);
		}


		[Test]
		public void TestGreaterThanEqualToVariableNumeralIntTrue()
		{
			VariableTestMapper variableMap = new VariableTestMapper ();
			
            Expression expA = new Expression (new DefaultVariableExpression("-5", variableMap), new NumeralExpression(-5), new GreaterThanEqualToOperator ());
			Assert.That (expA.Evaluate(), Is.True);
			
            Expression expB = new Expression (new DefaultVariableExpression("-5", variableMap), new NumeralExpression(-6), new GreaterThanEqualToOperator ());
			Assert.That (expB.Evaluate(), Is.True);
			
            Expression expC = new Expression (new DefaultVariableExpression("three", variableMap), new NumeralExpression(3), new GreaterThanEqualToOperator ());
			Assert.That (expC.Evaluate(), Is.True);
			
            Expression expD = new Expression (new DefaultVariableExpression("three", variableMap), new NumeralExpression(2), new GreaterThanEqualToOperator ());
			Assert.That (expD.Evaluate(), Is.True);
		}

		[Test]
		public void TestGreaterThanEqualToVariableNumeralIntFalse()
		{
			VariableTestMapper variableMap = new VariableTestMapper ();
			
            Expression expA = new Expression (new DefaultVariableExpression("-5", variableMap), new NumeralExpression(-4), new GreaterThanEqualToOperator ());
			Assert.That (expA.Evaluate(), Is.False);

            Expression expB = new Expression (new DefaultVariableExpression("three", variableMap), new NumeralExpression(4), new GreaterThanEqualToOperator ());
			Assert.That (expB.Evaluate(), Is.False);

		}
    }
}

