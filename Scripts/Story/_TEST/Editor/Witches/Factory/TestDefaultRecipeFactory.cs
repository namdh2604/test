using System;

namespace Unit.Witches.Factory
{
	using NUnit.Framework;
	using Moq;
	
	using Voltage.Witches.Factory;
	using Voltage.Story.General;
	
	using Voltage.Witches.Models;
	using Voltage.Witches.Configuration;

	
	[TestFixture]
	public class TestDefaultRecipeFactory
	{
		Mock<IRecipeFactoryNew> _mockRecipeFactory;		// really need to test this!
		Mock<RecipesConfiguration> _mockRecipeConfig;	

		[SetUp]
		public void Init()
		{
			_mockRecipeFactory = new Mock<IRecipeFactoryNew>();
			_mockRecipeConfig = new Mock<RecipesConfiguration> ();
		}


		private IFactory<string,IRecipe> CreateFactory()
		{
			return new DefaultRecipeFactory (_mockRecipeConfig.Object, _mockRecipeFactory.Object);
		}
		
		[Test]
		public void Constructor()
		{
			var factory = CreateFactory ();

			Assert.That (factory, Is.InstanceOf<DefaultRecipeFactory> ());
		}

//		[Test]
//		public void Create_ValidID_ValidRecipe()
//		{
////			mockRecipeFactory.
//
//			var factory = CreateFactory ();
//
//		}
	}
}