
using System;
using System.Collections.Generic;


namespace Unit.Witches.Story.Reset
{
    using NUnit.Framework;

	using Voltage.Story.Reset;
	using Voltage.Witches.Story.Reset;

	using Voltage.Witches.User;
	using Voltage.Witches.Models;

	using Moq;

    [TestFixture]
    public class TestWitchesRouteResetter
    {
		private Player _player;
		private List<string> _availableScenes = new List<string>();

		[SetUp]
		public void Init()
		{
			_player = CreatePlayer ();
		}


		private IStoryResetter CreateResetter()
		{
			_availableScenes = new List<string>
			{
				"/Prologue/Prologue/The Huntress",
				"/Prologue/Prologue/The Aristocrat",
				"/Prologue/Prologue/The Councilman",
				"/Prologue/Prologue/The Soldier",
			};

			return new WitchesRouteResetter (_player, _availableScenes.ToArray());
		}


        [Test]
        public void Constructor()
        {
			var resetter = CreateResetter ();

			Assert.That (resetter, Is.TypeOf<WitchesRouteResetter>());
        }


		[Test]
//		public void Reset_ValidAvailableScenes()
		public void Reset_ValidAvailableScenesCount()
		{
			var resetter = CreateResetter ();

			resetter.Reset ();

//			Assert.That (_player.AvailableScenes, Is.EqualTo (_availableScenes));
			Assert.That (_player.AvailableScenes.Count, Is.EqualTo (5));
		}



		[Test]
		public void Reset_AffinityZeroedOut()
		{
			var resetter = CreateResetter ();

			resetter.Reset ();

			Assert.That (_player.GetAffinity ("A"), Is.EqualTo (0));
		}


		[Test]
		public void Reset_AddedToTotalAffinity()
		{
			var resetter = CreateResetter ();

			resetter.Reset ();

			Assert.That (_player.TotalAffinity, Is.EqualTo (150));
		}


		[Test]
		public void Reset_ThrowOnNullPlayer()
		{
			Assert.Throws<ArgumentNullException> (() => new WitchesRouteResetter (null));
		}


//		[Ignore("deprecated behaviour")]
//		[Test]
//		public void Reset_SceneProgressCleared()
//		{
//			var resetter = CreateResetter ();
//
//			resetter.Reset ();
//
//			Assert.That (_player.CurrentScene, Is.Empty);
//		}


//		[Test]
//		public void Reset_NoOtherSideEffects()
//		{
//
//		}



		private Player CreatePlayer()
		{
            Mock<IPlayerWriter> mock_writer = new Mock<IPlayerWriter>();

			PlayerDataStore data = new PlayerDataStore ()
			{
				userID = "123",
				firstName = "foo",
				lastName = "bar",

				stamina = 2,
				focus = 3,

				currencyGame = 100,
				currencyPremium = 101,
				staminaPotions = 6,

				affinities = new Dictionary<string,int> 
				{
					{"A", 20},
					{"M", 30},
					{"N", 40},
					{"R", 50},
					{"T", 0},
				},
				totalAffinity = 10,
				completedScenes = new List<string>
				{
					"Prologue/Prologue/Answers and More Questions"
				},
					
				availableScenes = new List<string>
				{
					"Prologue/Prologue/Slow Day Swiftly Interrupted"
				},
				currentScene = "Prologue/Prologue/Slow Day Swiftly Interrupted",
				currentNodeID = "12",

				sceneHistory = new List<string>(),
				sceneChoices = new Dictionary<string,string>(),

				closetSpace = 30,
				currentOutfit = new Dictionary<string,string>(),
				inventory = new Dictionary<string,int>(),
				books = new List<Voltage.Witches.Configuration.PlayerSpellbookConfiguration>(),
			};

            return new Player (data, new List<Spellbook> (), mock_writer.Object, null, null);
		}
    }

//	public string userID;
//	
//	public string firstName;
//	public string lastName;
//	public string nickName;
//	
//	public int stamina;
//	public int focus;
//	
//	public int currencyGame;
//	public int currencyPremium;
//	
//	public int alignment;
//	public Dictionary<string, int> affinities = new Dictionary<string, int>();
//	public int totalAffinity;
//	
//	public List<string> availableScenes = new List<string>();
//	public string currentScene = string.Empty;
//	public string currentNodeID = string.Empty;
//	
//	public string currentHowTosScene = string.Empty;
//	
//	
//	public List<string> sceneHistory = new List<string>();
//	public Dictionary<string, string> sceneChoices = new Dictionary<string, string>();
//	
//	public Dictionary<string, string> currentOutfit = new Dictionary<string, string>();
//	
//	public Dictionary<string, int> inventory = new Dictionary<string, int>();
//	
//	public int staminaPotions;
//	
//	public List<PlayerSpellbookConfiguration> books;	// requires Voltage.Witches.Configuration
//	
//	public int closetSpace;		
}