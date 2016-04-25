
using System;

namespace Unit.Story.Expressions
{
    using NUnit.Framework;

	using Voltage.Story.Expressions;

    [TestFixture]
    public class ExpressionTest
    {
        [Test]
        public void TestSingleExpression()
        {
			Assert.That (new NumeralExpression (1).Evaluate (), Is.True);
			Assert.That (new StringExpression ("foo").Evaluate(), Is.True);
        }

		[Test]
		public void TestCompareTo()
		{
			Assert.That (new NumeralExpression(1).CompareTo(new NumeralExpression(2)), Is.EqualTo(-1));
			Assert.That (new NumeralExpression(1).CompareTo(new StringExpression("0")), Is.EqualTo(1));

			Assert.That (new StringExpression("0").CompareTo (new NumeralExpression(1)), Is.EqualTo(-1));
			Assert.That (new StringExpression("foo").CompareTo(new StringExpression("bar")), Is.EqualTo(1));

			Assert.That (new StringExpression("foo").CompareTo(new StringExpression("foo")), Is.EqualTo(0));
			Assert.That (new NumeralExpression(1).CompareTo(new NumeralExpression(1)), Is.EqualTo(0));
		}

		[Test]
		public void TestAndOperator()
		{
			Expression AndA = new Expression (new StringExpression ("0"), new NumeralExpression (1), new AndOperator ());
			Assert.That (AndA.Evaluate(), Is.True);

			Expression AndB = new Expression (new NumeralExpression(1), new NumeralExpression(2), new AndOperator());
			Assert.That (AndB.Evaluate(), Is.True);

			Expression CompoundAnd = new Expression (AndA, AndB, new AndOperator());
			Assert.That (CompoundAnd.Evaluate (), Is.True);
		}

		[Test]
		public void TestOrOperatorSingle()
		{
			Expression OrA = new Expression (new StringExpression ("0"), new NumeralExpression (1), new OrOperator ());
			Assert.That (OrA.Evaluate (), Is.True);
		}

		[Test]
		public void TestOrOperatorExpressionTrue()
		{
			Expression GreaterThanA = new Expression (new NumeralExpression(1), new NumeralExpression(0), new GreaterThanOperator());
			Expression GreaterThanB = new Expression (new NumeralExpression(0), new NumeralExpression(1), new GreaterThanOperator());

			Expression OrA = new Expression (GreaterThanA, GreaterThanA, new OrOperator ());
			Assert.That (OrA.Evaluate (), Is.True);

			Expression OrB = new Expression (GreaterThanA, GreaterThanB, new OrOperator ());
			Assert.That (OrB.Evaluate (), Is.True);
		}

		[Test]
		public void TestOrOperatorExpressionFalse()
		{
			Expression GreaterThanB = new Expression (new NumeralExpression(0), new NumeralExpression(1), new GreaterThanOperator());
			
			Expression OrA = new Expression (GreaterThanB, GreaterThanB, new OrOperator ());
			Assert.That (OrA.Evaluate (), Is.False);
		}





		[Test]
		public void TestGreaterThanOperatorNumeralNumeral ()
		{
			Expression GreaterThanA = new Expression (new NumeralExpression(1), new NumeralExpression(0), new GreaterThanOperator());
			Assert.That (GreaterThanA.Evaluate(), Is.True);
			
			Expression GreaterThanB = new Expression (new NumeralExpression(0), new NumeralExpression(1), new GreaterThanOperator());
			Assert.That (GreaterThanB.Evaluate(), Is.False);
		}

		[Test]
		public void TestGreaterThanOperatorStringString ()
		{
			Expression GreaterThanC = new Expression (new StringExpression("1"), new StringExpression("0"), new GreaterThanOperator());
			Assert.That (GreaterThanC.Evaluate(), Is.True);
			
			Expression GreaterThanD = new Expression (new StringExpression("0"), new StringExpression("1"), new GreaterThanOperator());
			Assert.That (GreaterThanD.Evaluate(), Is.False);
		}

		[Test]
		public void TestGreaterThanOperatorNumeralString ()
		{
			Expression GreaterThanE = new Expression (new NumeralExpression(1), new StringExpression("0"), new GreaterThanOperator());
			Assert.That (GreaterThanE.Evaluate(), Is.True);
			
			Expression GreaterThanF = new Expression (new NumeralExpression(0), new StringExpression("1"), new GreaterThanOperator());
			Assert.That (GreaterThanF.Evaluate(), Is.False);
		}

		[Test]
		public void TestGreaterThanOperatorStringNumeral ()
		{
			Expression GreaterThanG = new Expression (new StringExpression("1"), new NumeralExpression(0), new GreaterThanOperator());
			Assert.That (GreaterThanG.Evaluate(), Is.True);
			
			Expression GreaterThanH = new Expression (new StringExpression("0"), new NumeralExpression(1), new GreaterThanOperator());
			Assert.That (GreaterThanH.Evaluate(), Is.False);

			Expression GreaterThanI = new Expression (new StringExpression("-5"), new NumeralExpression(10), new GreaterThanOperator());
			Assert.That (GreaterThanI.Evaluate(), Is.False);

			Expression GreaterThanJ = new Expression (new StringExpression("-5"), new NumeralExpression(-10), new GreaterThanOperator());
			Assert.That (GreaterThanJ.Evaluate(), Is.True);

			Expression GreaterThanK = new Expression (new StringExpression("10"), new NumeralExpression(-5), new GreaterThanOperator());
			Assert.That (GreaterThanK.Evaluate(), Is.True);
		}

		[Test]
		public void TestGreaterThanOperatorCompound ()
		{
			Expression GreaterThanA = new Expression (new NumeralExpression(1), new NumeralExpression(0), new GreaterThanOperator());
			Expression GreaterThanB = new Expression (new NumeralExpression(0), new NumeralExpression(1), new GreaterThanOperator());
			Expression GreaterThanC = new Expression (new StringExpression("1"), new StringExpression("0"), new GreaterThanOperator());
			
			
			Expression CompoundGreaterThanAndTrue = new Expression (GreaterThanA, GreaterThanC, new AndOperator ());
			Assert.That (CompoundGreaterThanAndTrue.Evaluate (), Is.True);
			
			Expression CompoundGreaterThanAndFalse = new Expression (GreaterThanA, GreaterThanB, new AndOperator ());
			Assert.That (CompoundGreaterThanAndFalse.Evaluate (), Is.False);
			
			Expression DeepCompoundGreaterThanTrue = new Expression (CompoundGreaterThanAndTrue, CompoundGreaterThanAndTrue, new AndOperator ());
			Assert.That (DeepCompoundGreaterThanTrue.Evaluate (), Is.True);
			
			Expression DeepCompoundGreaterThanFalse = new Expression (CompoundGreaterThanAndFalse, CompoundGreaterThanAndTrue, new AndOperator ());
			Assert.That (DeepCompoundGreaterThanFalse.Evaluate (), Is.False);
			
		}

		[Test]
		public void TestGreaterThanOperatorException ()
		{
//			Expression CompoundGreaterThanException = new Expression (GreaterThanA, GreaterThanB, new GreaterThanOperator ());		
//			Assert.That (CompoundGreaterThanException.Evaluate(), Throws.ArgumentException);		// Assert.That (CompoundGreaterThanException.Evaluate(), Throws.TypeOf<ArgumentException>());
		}









		
		[Test]
		public void TestGreaterThanEqualtToOperatorTrue ()
		{
			Expression GreaterThanEqualToA = new Expression (new NumeralExpression(1), new NumeralExpression(0), new GreaterThanEqualToOperator());
			Assert.That (GreaterThanEqualToA.Evaluate(), Is.True);
			
			Expression GreaterThanEqualToB = new Expression (new NumeralExpression(1), new NumeralExpression(1), new GreaterThanEqualToOperator());
			Assert.That (GreaterThanEqualToB.Evaluate(), Is.True);
		}
		
		[Test]
		public void TestGreaterThanEqualToOperatorFalse ()
		{
			Expression GreaterThanEqualToA = new Expression (new NumeralExpression(0), new NumeralExpression(1), new GreaterThanEqualToOperator());
			Assert.That (GreaterThanEqualToA.Evaluate(), Is.False);
		}
		
		[Test]
		public void TestGreaterThanOperatorEqualToException ()
		{
//			Expression CompoundGreaterThanException = new Expression (GreaterThanA, GreaterThanB, new GreaterThanOperator ());		
//			Assert.That (CompoundGreaterThanException.Evaluate(), Throws.ArgumentException);		// Assert.That (CompoundGreaterThanException.Evaluate(), Throws.TypeOf<ArgumentException>());
		}



		[Test]
		public void TestEqualToOperatorStringStringTrue ()
		{
			Expression EqualToA = new Expression (new StringExpression("foo"), new StringExpression("foo"), new EqualToOperator());
			Assert.That (EqualToA.Evaluate (), Is.True);
		}

		[Test]
		public void TestEqualToOperatorStringStringFalse ()
		{
			Expression EqualToA = new Expression (new StringExpression("foo"), new StringExpression("bar"), new EqualToOperator());
			Assert.That (EqualToA.Evaluate (), Is.False);
		}

		[Test]
		public void TestEqualToOperatorNumeralNumeralTrue ()
		{
			Expression EqualToA = new Expression (new NumeralExpression(1), new NumeralExpression(1), new EqualToOperator());
			Assert.That (EqualToA.Evaluate (), Is.True);
		}
		
		[Test]
		public void TestEqualToOperatorNumeralNumeralFalse ()
		{
			Expression EqualToA = new Expression (new NumeralExpression(0), new NumeralExpression(1), new EqualToOperator());
			Assert.That (EqualToA.Evaluate (), Is.False);
		}

		
		[Test]
		public void TestEqualToOperatorNumeralStringTrue ()
		{
			Expression EqualToA = new Expression (new NumeralExpression(0), new StringExpression("0"), new EqualToOperator());
			Assert.That (EqualToA.Evaluate (), Is.True);
		}

		[Test]
		public void TestEqualToOperatorNumeralStringFalse ()
		{
			Expression EqualToA = new Expression (new NumeralExpression(0), new StringExpression("1"), new EqualToOperator());
			Assert.That (EqualToA.Evaluate (), Is.False);
		}

		[Test]
		public void TestEqualToOperatorStringNumeralTrue ()
		{
			Expression EqualToA = new Expression (new StringExpression("0"), new NumeralExpression(0), new EqualToOperator());
			Assert.That (EqualToA.Evaluate (), Is.True);
		}
		
		[Test]
		public void TestEqualToOperatorStringNumeralFalse ()
		{
			Expression EqualToA = new Expression (new StringExpression("1"), new NumeralExpression(0), new EqualToOperator());
			Assert.That (EqualToA.Evaluate (), Is.False);
		}

		[Test]
		public void TestEqualToOperatorExpressionTrue()
		{
			Expression EqualToA = new Expression (new StringExpression("foo"), new StringExpression("foo"), new EqualToOperator());
			Expression EqualToB = new Expression (new NumeralExpression(1), new NumeralExpression(1), new EqualToOperator());

			Expression EqualToCompound = new Expression (EqualToA, EqualToB, new EqualToOperator());
			Assert.That (EqualToCompound.Evaluate (), Is.True);
		}

		[Test]
		public void TestEqualToOperatorExpressionFalse()
		{
			Expression EqualToA = new Expression (new StringExpression("foo"), new StringExpression("bar"), new EqualToOperator());
			Expression EqualToB = new Expression (new NumeralExpression(1), new NumeralExpression(1), new EqualToOperator());
			
			Expression EqualToCompound = new Expression (EqualToA, EqualToB, new EqualToOperator());
			Assert.That (EqualToA.Evaluate(), Is.False);
			Assert.That (EqualToB.Evaluate(), Is.True);
			Assert.That (EqualToCompound.Evaluate (), Is.False);
		}

//		[Test]
//		public void TestStringLiteralInputFalse()
//		{
//			string literal = "\"foobar\"";	// @
//
//			Expression EqualToA = new Expression (new StringExpression("foobar"), new StringExpression(literal), new EqualToOperator());
//			Assert.That (EqualToA.Evaluate (), Is.False);
//		}


		[Test]
		public void TestNotEqualToOperatorStringStringTrue()
		{
			Expression NotEqualToA = new Expression (new StringExpression("foo"), new StringExpression("bar"), new NotEqualToOperator());
			Assert.That (NotEqualToA.Evaluate (), Is.True);
		}

		[Test]
		public void TestNotEqualToOperatorStringStringFalse()
		{
			Expression NotEqualToA = new Expression (new StringExpression("foo"), new StringExpression("foo"), new NotEqualToOperator());
			Assert.That (NotEqualToA.Evaluate (), Is.False);
		}

		[Test]
		public void TestNotEqualToOperatorNumeralNumeralTrue()
		{
			Expression NotEqualToA = new Expression (new NumeralExpression(1), new NumeralExpression(0), new NotEqualToOperator());
			Assert.That (NotEqualToA.Evaluate (), Is.True);
		}
		
		[Test]
		public void TestNotEqualToOperatorNumeralNumeralFalse()
		{
			Expression NotEqualToA = new Expression (new NumeralExpression(1), new NumeralExpression(1), new NotEqualToOperator());
			Assert.That (NotEqualToA.Evaluate (), Is.False);
		}


		[Test]
		public void TestNotEqualToOperatorExpressionTrue()
		{
			Expression EqualToA = new Expression (new StringExpression("foo"), new StringExpression("bar"), new EqualToOperator());
			Expression EqualToB = new Expression (new NumeralExpression(1), new NumeralExpression(1), new EqualToOperator());
			
			Expression EqualToCompound = new Expression (EqualToA, EqualToB, new NotEqualToOperator());
			Assert.That (EqualToA.Evaluate(), Is.False);
			Assert.That (EqualToB.Evaluate(), Is.True);
			Assert.That (EqualToCompound.Evaluate (), Is.True);
		}

		[Test]
		public void TestNotEqualToOperatorExpressionFalse()
		{
			Expression EqualToA = new Expression (new StringExpression("foo"), new StringExpression("foo"), new EqualToOperator());
			Expression EqualToB = new Expression (new NumeralExpression(1), new NumeralExpression(1), new EqualToOperator());
			
			Expression EqualToCompound = new Expression (EqualToA, EqualToB, new NotEqualToOperator());
			Assert.That (EqualToA.Evaluate(), Is.True);
			Assert.That (EqualToB.Evaluate(), Is.True);
			Assert.That (EqualToCompound.Evaluate (), Is.False);
		}


    }
}