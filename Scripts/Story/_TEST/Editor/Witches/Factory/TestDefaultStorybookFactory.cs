
using System;
using System.Collections.Generic;

namespace Unit.Witches.Factory
{
    using NUnit.Framework;
	using Moq;

	using Voltage.Witches.Factory;
	using Voltage.Story.General;

	using Voltage.Witches.Models;
	using Voltage.Witches.Configuration;

    [TestFixture]
    public class TestDefaultStorybookFactory
    {
		private Mock<BooksConfiguration> _mockBookConfig;
//		private Mock<IFactory<string,IRecipe>> _mockRecipeFactory;

		private Mock<ISpellbookFactoryNew> _mockSpellbookFactory;

		[SetUp]
		public void Init()
		{
			_mockBookConfig = new Mock<BooksConfiguration> ();
//			_mockRecipeFactory = new Mock<IFactory<string,IRecipe>> ();

			_mockSpellbookFactory = new Mock<ISpellbookFactoryNew> ();
		}

		private IFactory<string,ISpellbook> CreateFactory()
		{
//			return new DefaultSpellbookFactory(_mockBookConfig.Object, _mockRecipeFactory.Object);
			return new DefaultSpellbookFactory(_mockBookConfig.Object, _mockSpellbookFactory.Object);
		}

        [Test]
        public void Constructor()
        {
			var factory = CreateFactory ();

			Assert.That (factory, Is.InstanceOf<DefaultSpellbookFactory> ());
        }


		[Test]
		public void Create_ValidID_ValidAdapter()
		{
			var factory = new DefaultSpellbookFactory (_bookConfig, _mockSpellbookFactory.Object);
			
			factory.Create ("12345");
			
			_mockSpellbookFactory.Verify (sf => sf.Create (It.Is<SpellbookRefConfig> (arg => arg.Id == _configID)));
		}


//		[Ignore("may want to update factory to fit this test in the future")]
//		[Test]
//		public void Create_ValidID_ValidRecipeCount()
//		{
//			string bookID = "12345";
//
//			_mockBookConfig.SetupGet (config => config.Books_Reference[It.Is<string> (id => id == bookID)]).Returns (_spellbookRefConfigg);	// non-virtual method
//			_mockRecipeFactory.Setup (recipeFactory => recipeFactory.Create (It.IsAny<string> ())).Returns (new Recipe ("test"));
//
//			var factory = CreateFactory ();
//
//			ISpellbook book = factory.Create (bookID);
//
//			Assert.That (book.Recipes.Count, Is.EqualTo (2)); 
//		}
//
//		[Ignore("may want to update factory to fit this test in the future")]
//		[Test]
//		public void Create_ValidID_ProperBookName()
//		{
//			string bookID = "12345";
//			
//			_mockBookConfig.SetupGet (config => config.Books_Reference[It.Is<string> (id => id == bookID)]).Returns (_spellbookRefConfig);	// non-virtual method
//			_mockRecipeFactory.Setup (recipeFactory => recipeFactory.Create (It.IsAny<string> ())).Returns (new Recipe ("test"));
//			
//			var factory = CreateFactory ();
//
//			ISpellbook book = factory.Create (bookID);
//
//			Assert.That (book.Name, Is.StringMatching(_spellbookRefConfig.Name)); 
//		}



		private static string _configID = "12345";

		private static SpellbookRefConfig _spellbookRefConfig = new SpellbookRefConfig(_configID)
		{
			Name = "test spellbook",
			IsAvailable = true,
			Recipes = new List<string>{"54da8aca6f983f60ee01f78a", "54da8ad66f983f60ee01f78b"},
		};

		private BooksConfiguration _bookConfig = new BooksConfiguration ()
		{
			Books_Reference = new Dictionary<string,SpellbookRefConfig> { {_configID, _spellbookRefConfig}}
		};





    }
}