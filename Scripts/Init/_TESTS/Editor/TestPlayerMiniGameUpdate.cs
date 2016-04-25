
using System;
using System.Collections.Generic;

namespace Integration.Witches.User
{
    using NUnit.Framework;
	using Moq;

	using Voltage.Story.User;
	using Voltage.Witches.User;
	using Voltage.Witches.Models;
	using Voltage.Witches.Configuration;


	using System.Linq;

    [TestFixture]
    public class TestPlayerMiniGameUpdate
    {
//		private Mock<IPlayer> _mockPlayer;
//		private Mock<PlayerDataStore> _mockDataStore;
        private Mock<IPlayerWriter> _mockWriter;
//		private Mock<PlayerStaminaManager> _mockStaminaMgr;
//		private Mock<PlayerFocusManager> _mockFocusMgr;


		private Spellbook _bookA;
		private Spellbook _bookB;
		private List<Spellbook> _bookList;
//		private List<PlayerSpellbookConfiguration> _playerSpellbooks;
		private PlayerDataStore _dataStore;

		[SetUp]
		public void Init()
		{
//			_mockDataStore = new Mock<PlayerDataStore> ();
            _mockWriter = new Mock<IPlayerWriter>();
//			_mockStaminaMgr = new Mock<PlayerStaminaManager> ();
//			_mockFocusMgr = new Mock<PlayerFocusManager> ();

//			Mock<Recipe> recipeA = new Mock<Recipe> ();
//			recipeA.SetupGet (r => r.Id).Returns ("recipe1");
//			Mock<Recipe> recipeB = new Mock<Recipe> ();
//			recipeB.SetupGet (r => r.Id).Returns ("recipe2");
//			Mock<Spellbook> mockBookA = new Mock<Spellbook> ();
//			mockBookA.Setup(

			SetupSpellBooks ();
		}

		private void SetupSpellBooks()
		{
			_bookA = new Spellbook ("test book A", "12345");
			Recipe recipeA = new Recipe ("recipe A")
			{
				Id = "recipe1"
			};
			Recipe recipeB = new Recipe ("recipe B")
			{
				Id = "recipe2"
			};
			_bookA.AddRecipe (recipeA);
			_bookA.AddRecipe (recipeB);

			_bookB = new Spellbook ("test book B", "67890");
			Recipe recipeC = new Recipe ("recipe C")
			{
				Id = "recipe3"
			};
			Recipe recipeD = new Recipe ("recipe D")
			{
				Id = "recipe4"
			};
			_bookB.AddRecipe (recipeC);
			_bookB.AddRecipe (recipeD);

			
			_bookList = new List<Spellbook> ()
			{
				_bookA, _bookB,
			};

//			_playerSpellbooks = new List<PlayerSpellbookConfiguration> ()
//			{
//				new PlayerSpellbookConfiguration(_bookA),
//				new PlayerSpellbookConfiguration(_bookB)
//			};
			_dataStore = new PlayerDataStore ()
			{
				books = new List<PlayerSpellbookConfiguration>()
				{
					new PlayerSpellbookConfiguration(_bookA),
					new PlayerSpellbookConfiguration(_bookB)
				}
			};
		}




		private Player CreatePlayer()
		{
//			return new Player (_mockDataStore.Object, _spellbooks, _mockSerializer.Object, _mockStaminaMgr.Object, _mockFocusMgr.Object);
            return new Player (_dataStore, _bookList, _mockWriter.Object, new PlayerStaminaManager(1f,100,_dataStore), new PlayerFocusManager(1f,100,_dataStore));
		}


        [Test]
		public void UpdateMiniGameProgress_ValidRecipeAndHigherStage_UpdateRecipe()
        {
//			_mockDataStore.SetupGet (data => data.books).Returns(_playerSpellbooks);		// not property
//			_mockDataStore.SetReturnsDefault<List<PlayerSpellbookConfiguration>>(_playerSpellbooks);

			var player = CreatePlayer ();

			string recipeID = "recipe3";

//			_mockPlayer.Object.UpdateMiniGameProgress(recipeID, CompletionStage.SECOND);
			player.UpdateMiniGameProgress (recipeID, CompletionStage.SECOND);

			Assert.That (player.GetBooks().SelectMany (b => b.Recipes).FirstOrDefault (r => r.Id == recipeID).CurrentStage, Is.EqualTo(CompletionStage.SECOND));
        }

		[Test]
		public void UpdateMiniGameProgress_ValidRecipeAndLowerStage_DoesNotUpdateRecipe()
		{
			var player = CreatePlayer ();
			
			string recipeID = "recipe1";
			
//			_mockPlayer.Object.UpdateMiniGameProgress(recipeID, CompletionStage.SECOND);
			player.UpdateMiniGameProgress (recipeID, CompletionStage.SECOND);
			player.UpdateMiniGameProgress (recipeID, CompletionStage.FIRST);
			
			Assert.That (player.GetBooks().SelectMany (b => b.Recipes).FirstOrDefault (r => r.Id == recipeID).CurrentStage, Is.EqualTo(CompletionStage.SECOND));
		}

//		[Test]
//		public void UpdateMiniGameProgress_ValidRecipeAndLowerStage_DoesNotUpdateRecipe()
//		{
//			var player = CreatePlayer ();
//			
//			string recipeID = "invalidID";
//
////			_mockPlayer.Object.UpdateMiniGameProgress(recipeID, CompletionStage.SECOND);
//			player.UpdateMiniGameProgress (recipeID, CompletionStage.SECOND);
//			
//
//		}


    }
}