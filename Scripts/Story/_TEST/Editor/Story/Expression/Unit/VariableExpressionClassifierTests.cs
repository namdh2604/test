using NUnit.Framework;
using Voltage.Story.Expressions;

namespace Unit.Story.Expressions
{
	[TestFixture]
	public class VariableExpressionClassifierTests
	{
		private VariableExpressionClassifier _classifier;
		[SetUp]
		public void Init()
		{
			_classifier = new VariableExpressionClassifier();
		}

		[Test]
		public void Classify_WithFavorabilityVariable_ReturnsFavorability()
		{
			VariableCategory classification = _classifier.Classify("Characters/Anastasia/Affinity");
			Assert.AreEqual(VariableCategory.Favorability, classification);
		}

		[Test]
		public void Classify_WithClothingVariable_ReturnsOther()
		{
			VariableCategory classification = _classifier.Classify("Avatar/Hat");
			Assert.AreEqual(VariableCategory.Other, classification);
		}
	}
}

