
using System;
using System.Collections.Generic;

namespace Integration.Story.Expressions
{
    using NUnit.Framework;

	using Voltage.Story.Expressions;
	using Voltage.Story.Mapper;

    [TestFixture]
    public class ExpressionFactoryTest
    {
		public class VariableTestMapper : IMapping<string>
		{
			public IDictionary<string,object> Map = new Dictionary<string,object> 
			{
				{"MC/First", "jon"},
				{"MC/Last", "doe"},
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

				value = (success) ? (int)v : 0;

				return success;
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
        public void TestNumeralNumeralExpressionTrue()
        {
			ExpressionParser parser = new ExpressionParser ();
			ExpressionFactory factory = new ExpressionFactory(parser, new VariableTestMapper());

			ExpressionState state = parser.Parse (numeralExpressionTrue);

			IExpression expression = factory.CreateExpression (state);
			Assert.That (expression, Is.Not.Null);
			Assert.That (expression, Is.TypeOf<Expression> ());
			Assert.That ((expression as Expression).Left, Is.TypeOf<NumeralExpression> ());
			Assert.That ((expression as Expression).Right, Is.TypeOf<NumeralExpression> ());
			Assert.That ((expression as Expression).Operator, Is.TypeOf<GreaterThanOperator> ());
			Assert.That (((expression as Expression).Left as NumeralExpression).Value, Is.EqualTo(2));
			Assert.That (((expression as Expression).Right as NumeralExpression).Value, Is.EqualTo(1));
			Assert.That (expression.Evaluate (), Is.True);
        }

		[Test]
		public void TestStringStringExpressionTrue()
		{
			ExpressionParser parser = new ExpressionParser ();
			ExpressionFactory factory = new ExpressionFactory(parser, new VariableTestMapper());
			
			ExpressionState state = parser.Parse (stringExpressionTrue);
			
			IExpression expression = factory.CreateExpression (state);
			Assert.That (expression, Is.Not.Null);
			Assert.That (expression, Is.TypeOf<Expression> ());
			Assert.That ((expression as Expression).Left, Is.TypeOf<StringExpression> ());
			Assert.That ((expression as Expression).Right, Is.TypeOf<StringExpression> ());
			Assert.That ((expression as Expression).Operator, Is.TypeOf<EqualToOperator> ());
			Assert.That (((expression as Expression).Left as StringExpression).Value, Is.StringMatching("foobar"));
			Assert.That (((expression as Expression).Right as StringExpression).Value, Is.StringMatching("foobar"));
			Assert.That (expression.Evaluate (), Is.True);
		}

		[Test]
		public void TestVariableVariableExpressionTrue()
		{
			ExpressionParser parser = new ExpressionParser ();
			ExpressionFactory factory = new ExpressionFactory(parser, new VariableTestMapper());
			
			ExpressionState state = parser.Parse (variableExpressionTrue);
			
			IExpression expression = factory.CreateExpression (state);
			Assert.That (expression, Is.Not.Null);
			Assert.That (expression, Is.TypeOf<Expression> ());
            Assert.That ((expression as Expression).Left, Is.InstanceOf<VariableExpression> ());
            Assert.That ((expression as Expression).Right, Is.InstanceOf<VariableExpression> ());
			Assert.That ((expression as Expression).Operator, Is.TypeOf<NotEqualToOperator> ());
			Assert.That (((expression as Expression).Left as VariableExpression).Variable, Is.StringMatching("MC/First"));
			Assert.That (((expression as Expression).Right as VariableExpression).Variable, Is.StringMatching("MC/Last"));
			Assert.That (expression.Evaluate (), Is.True);
		}

		[Test]
		public void TestVariableVariableExpressionFalse()
		{
			ExpressionParser parser = new ExpressionParser ();
			ExpressionFactory factory = new ExpressionFactory(parser, new VariableTestMapper());
			
			ExpressionState state = parser.Parse (variableExpressionFalse);
			
			IExpression expression = factory.CreateExpression (state);
			Assert.That (expression, Is.Not.Null);
			Assert.That (expression, Is.TypeOf<Expression> ());
            Assert.That ((expression as Expression).Left, Is.InstanceOf<VariableExpression> ());
            Assert.That ((expression as Expression).Right, Is.InstanceOf<VariableExpression> ());
			Assert.That ((expression as Expression).Operator, Is.TypeOf<NotEqualToOperator> ());
			Assert.That (((expression as Expression).Left as VariableExpression).Variable, Is.StringMatching("MC/First"));
			Assert.That (((expression as Expression).Right as VariableExpression).Variable, Is.StringMatching("MC/First"));
			Assert.That (expression.Evaluate (), Is.False);
		}

		[Test]
		public void TestExpressionStringExpressionTrue()
		{
			ExpressionParser parser = new ExpressionParser ();
			ExpressionFactory factory = new ExpressionFactory(parser, new VariableTestMapper());
			
			ExpressionState state = parser.Parse (expressionStringExpressionTrue);
			
			IExpression expression = factory.CreateExpression (state);
			Assert.That (expression, Is.Not.Null);
			Assert.That (expression, Is.TypeOf<Expression> ());
			Assert.That ((expression as Expression).Left, Is.TypeOf<Expression> ());
			Assert.That ((expression as Expression).Right, Is.TypeOf<StringExpression> ());
			Assert.That ((expression as Expression).Operator, Is.TypeOf<AndOperator> ());
			Assert.That (((expression as Expression).Left as Expression).Left, Is.TypeOf<StringExpression> ());
			Assert.That (((expression as Expression).Left as Expression).Right, Is.TypeOf<StringExpression> ());
			Assert.That ((((expression as Expression).Left as Expression).Left as StringExpression).Value, Is.StringMatching("foobar"));
			Assert.That ((((expression as Expression).Left as Expression).Right as StringExpression).Value, Is.StringMatching("foobar"));
			Assert.That (((expression as Expression).Right as StringExpression).Value, Is.StringMatching("foobar"));
			Assert.That (expression.Evaluate (), Is.True);
		}


		[Test]
		public void TestCompositeExpressionTrue()
		{
			ExpressionParser parser = new ExpressionParser ();
			ExpressionFactory factory = new ExpressionFactory(parser, new VariableTestMapper());
			
			ExpressionState state = parser.Parse (compositeExpression);
			
			IExpression expression = factory.CreateExpression (state);
			Assert.That (expression, Is.Not.Null);
			Assert.That (expression, Is.TypeOf<Expression> ());


			Assert.That ((expression as Expression).Left, Is.TypeOf<Expression> ());
            Assert.That (((expression as Expression).Left as Expression).Left, Is.InstanceOf<StringExpression> ());
            Assert.That (((expression as Expression).Left as Expression).Right, Is.InstanceOf<StringExpression> ());
				Assert.That ((((expression as Expression).Left as Expression).Left as StringExpression).Value, Is.StringMatching("foobar"));
				Assert.That ((((expression as Expression).Left as Expression).Right as StringExpression).Value, Is.StringMatching("foobar"));

			Assert.That ((expression as Expression).Right, Is.TypeOf<Expression> ());
				Assert.That (((expression as Expression).Right as Expression).Left, Is.TypeOf<Expression> ());
				Assert.That ((((expression as Expression).Right as Expression).Left as Expression).Left, Is.TypeOf<NumeralExpression> ());
				Assert.That (((((expression as Expression).Right as Expression).Left as Expression).Left as NumeralExpression).Value, Is.EqualTo(2));
				Assert.That ((((expression as Expression).Right as Expression).Left as Expression).Right, Is.TypeOf<NumeralExpression> ());
				Assert.That (((((expression as Expression).Right as Expression).Left as Expression).Right as NumeralExpression).Value, Is.EqualTo(1));
				Assert.That ((((expression as Expression).Right as Expression).Left as Expression).Operator, Is.TypeOf<GreaterThanOperator> ());
				Assert.That ((((expression as Expression).Right as Expression).Left as Expression).Evaluate(), Is.True);

				Assert.That (((expression as Expression).Right as Expression).Right, Is.TypeOf<Expression> ());
            Assert.That ((((expression as Expression).Right as Expression).Right as Expression).Left, Is.InstanceOf<VariableExpression> ());
				Assert.That (((((expression as Expression).Right as Expression).Right as Expression).Left as VariableExpression).Variable, Is.StringMatching("MC/First"));
            Assert.That ((((expression as Expression).Right as Expression).Right as Expression).Right, Is.InstanceOf<VariableExpression> ());
				Assert.That (((((expression as Expression).Right as Expression).Right as Expression).Right as VariableExpression).Variable, Is.StringMatching("MC/Last"));
				Assert.That ((((expression as Expression).Right as Expression).Right as Expression).Operator, Is.TypeOf<NotEqualToOperator> ());
				Assert.That ((((expression as Expression).Right as Expression).Right as Expression).Evaluate(), Is.True);

			Assert.That ((expression as Expression).Operator, Is.TypeOf<AndOperator> ());
			Assert.That (expression.Evaluate (), Is.True);
		}


		
		private string numeralExpressionTrue = @"
		{
			""_class"": ""Expression"",
			""left"": ""2"",
			""right"": ""1"",
			""op"": "">""
		}
		";
		
		
//		private string numeralExpressionFalse = @"
//		{
//			""_class"": ""Expression"",
//			""left"": ""2"",
//			""right"": ""1"",
//			""op"": ""<""
//		}
//		";
		
		private string stringExpressionTrue = @"
		{
			""_class"": ""Expression"",
			""left"": ""\""foobar\"""",
			""right"": ""\""foobar\"""",
			""op"": ""=""
		}
		";
		
//		private string stringExpressionFalse = @"
//		{
//			""_class"": ""Expression"",
//			""left"": ""\""foobar\"""",
//			""right"": ""\""hello world\"""",
//			""op"": ""=""
//		}
//		";
		
		
//		private string numeralNullExpressionTrue = @"
//		{
//			""_class"": ""Expression"",
//			""left"": ""2"",
//			""right"": null,
//		}
//		";
		
		
		private string variableExpressionTrue = @"
		{
			""_class"": ""Expression"",
			""left"": {
				""_class"": ""Variable"",
				""text"": ""MC/First""
			},
			""right"": {
				""_class"": ""Variable"",
				""text"": ""MC/Last""
			},
			""op"": ""!=""
		}
		";

		private string variableExpressionFalse = @"
		{
			""_class"": ""Expression"",
			""left"": {
				""_class"": ""Variable"",
				""text"": ""MC/First""
			},
			""right"": {
				""_class"": ""Variable"",
				""text"": ""MC/First""
			},
			""op"": ""!=""
		}
		";
		
		
		private string expressionStringExpressionTrue = @"
		{
			""_class"": ""Expression"",
			""left"": {
						""_class"": ""Expression"",
						""left"": ""\""foobar\"""",
						""right"": ""\""foobar\"""",
						""op"": ""=""
					},
			""right"": ""\""foobar\"""",
			""op"": ""AND""
		}
		";
		
		
		private string compositeExpression = @"
		{
			""_class"": ""Expression"",
			""left"": {
				""_class"": ""Expression"",
				""left"": ""\""foobar\"""",
				""right"": ""\""foobar\"""",
				""op"": ""=""
			},
			""right"": {
				""_class"": ""Expression"",
				""left"": {
					""_class"": ""Expression"",
					""left"": ""2"",
					""right"": ""1"",
					""op"": "">""
				},
				""right"": {
					""_class"": ""Expression"",
					""left"": {
						""_class"": ""Variable"",
						""text"": ""MC/First""
					},
					""right"": {
						""_class"": ""Variable"",
						""text"": ""MC/Last""
					},
					""op"": ""!=""
				},
				""op"": ""AND""
			},
			""op"": ""AND""
		}
		";
    }
}