
using System;

namespace Unit.Story.Expressions
{
    using NUnit.Framework;

	using Voltage.Story.Expressions;

	using Newtonsoft.Json;
	using Newtonsoft.Json.Linq;

    [TestFixture]
    public class ExpressionParserTest
    {
        [Test]
        public void TestNumeralParse()
        {
			ExpressionParser parser = new ExpressionParser ();
			ExpressionState state = parser.Parse (numeralExpressionTrue);

			Assert.That (state.LeftType, Is.EqualTo(ExpressionType.NUMERAL));	// Assert.That (state.LeftType, Is.TypeOf (typeof(NumeralExpression)));
			Assert.That (state.Left, Is.StringMatching ("2"));
			Assert.That (state.RightType, Is.EqualTo(ExpressionType.NUMERAL));	// Assert.That (state.RightType, Is.TypeOf (typeof(NumeralExpression)));
			Assert.That (state.Right, Is.StringMatching ("1"));
			Assert.That (state.Operator, Is.StringMatching (">"));

        }

		[Test]
		public void TestStringParse()
		{
			ExpressionParser parser = new ExpressionParser ();
			ExpressionState state = parser.Parse (stringExpressionFalse);
			
			Assert.That (state.LeftType, Is.EqualTo(ExpressionType.STRING));	// Assert.That (state.LeftType, Is.TypeOf (typeof(NumeralExpression)));
			Assert.That (state.Left, Is.StringMatching ("foobar"));
			Assert.That (state.RightType, Is.EqualTo(ExpressionType.STRING));	// Assert.That (state.RightType, Is.TypeOf (typeof(NumeralExpression)));
			Assert.That (state.Right, Is.StringMatching ("hello world"));
			Assert.That (state.Operator, Is.StringMatching ("="));
			
		}


		[Test]
		public void TestVariableParse()
		{
			ExpressionParser parser = new ExpressionParser ();
			ExpressionState state = parser.Parse (variableExpressionTrue);
			
			Assert.That (state.LeftType, Is.EqualTo(ExpressionType.VARIABLE));	
			Assert.That (state.Left, Is.StringMatching ("MC/First"));
			Assert.That (state.RightType, Is.EqualTo(ExpressionType.VARIABLE));	
			Assert.That (state.Right, Is.StringMatching ("MC/Last"));
			Assert.That (state.Operator, Is.StringMatching ("!="));
			
		}

		[Test]
		public void TestSingleParse()
		{
			ExpressionParser parser = new ExpressionParser ();
			ExpressionState state = parser.Parse (numeralNullExpressionTrue);
			
			Assert.That (state.LeftType, Is.EqualTo(ExpressionType.NUMERAL));
			Assert.That (state.Left, Is.StringMatching ("2"));
			Assert.That (state.RightType, Is.EqualTo(ExpressionType.NONE));
			Assert.That (state.Right, Is.StringMatching (string.Empty));
			Assert.That (state.Operator, Is.StringMatching (string.Empty));
			
		}

		[Test]
		public void TestExpression()
		{
			ExpressionParser parser = new ExpressionParser ();
			ExpressionState state = parser.Parse (expressionStringExpressionTrue);
			
			Assert.That (state.LeftType, Is.EqualTo(ExpressionType.EXPRESSION));
//			Assert.That (state.Left, Is.StringMatching (""));
			Assert.That (state.RightType, Is.EqualTo(ExpressionType.STRING));
			Assert.That (state.Right, Is.StringMatching ("foobar"));
			Assert.That (state.Operator, Is.StringMatching ("AND"));
		}


		[Test]
		public void TestCompositeExpression()
		{
			ExpressionParser parser = new ExpressionParser ();
			ExpressionState state = parser.Parse (compositeExpression);
			
			Assert.That (state.LeftType, Is.EqualTo(ExpressionType.EXPRESSION));
//			Assert.That (state.Left, Is.StringMatching (""));
			Assert.That (state.RightType, Is.EqualTo(ExpressionType.EXPRESSION));
//			Assert.That (state.Right, Is.StringMatching (""));
			Assert.That (state.Operator, Is.StringMatching ("AND"));
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

//		private string stringExpressionTrue = @"
//		{
//			""_class"": ""Expression"",
//			""left"": ""\""foobar\"""",
//			""right"": ""\""foobar\"""",
//			""op"": ""=""
//		}
//		";

		private string stringExpressionFalse = @"
		{
			""_class"": ""Expression"",
			""left"": ""\""foobar\"""",
			""right"": ""\""hello world\"""",
			""op"": ""=""
		}
		";


		private string numeralNullExpressionTrue = @"
		{
			""_class"": ""Expression"",
			""left"": ""2"",
			""right"": null,
		}
		";


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