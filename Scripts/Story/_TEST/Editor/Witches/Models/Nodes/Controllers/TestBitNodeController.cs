
using System;
using System.Collections.Generic;

namespace Integration.Witches.Story.Models.Nodes.Controllers
{
    using NUnit.Framework;

	using Voltage.Common.Logging;
	using Voltage.Witches.Story.Models.Nodes.Controllers;
	using Voltage.Story.Models.Nodes.Controllers;
	using Voltage.Story.StoryPlayer;
	using Voltage.Story.StoryDivisions;
	using Voltage.Story.Models.Nodes;
	using Voltage.Witches.Models;
	using Voltage.Witches.Net;
	using Voltage.Common.Net;

	using Voltage.Witches.User;
	using Voltage.Witches.Screens;

	using Moq;
	using Voltage.Witches.Converters;

	using StoryPlayerUIScreen = Voltage.Witches.UI.StoryPlayerUIScreen;

    [TestFixture]
    public class TestBitNodeController
    {
//		Mock<StoryPlayerUIScreen> _mockStoryPlayerScreen = new Mock<StoryPlayerUIScreen> ();

//		Mock<CompositeStoryPlayerScreen> _mockCompositeScreen = new Mock<CompositeStoryPlayerScreen>();
		private Player _player;
		private Voltage.Story.StoryDivisions.Scene _scene;
		private StoryPlayerBasic _storyPlayer;

//		[SetUp]
//		public void Init()
//		{
//			PlayerDataStore data = new PlayerDataStore
//			{
//				firstName = "foo",
//				lastName = "bar",
//				stamina = 10,
//			};
//			_player = new Player ("testPlayer", null, null, data, null);
//
//			_networkControllerSpy = new NetworkControllerSpy ();
//			WitchesNetworkController witchesNetworkController = new WitchesNetworkController (_networkControllerSpy);
//			
//			Dictionary<Type,INodeController> nodeControllers = new Dictionary<Type,INodeController> ()
//			{
//				{typeof(Voltage.Story.StoryDivisions.Scene), new SceneNodeController()},
//				{typeof(BitNode), new WitchesBitNodeController(_player, witchesNetworkController, new ConsoleLogger())}
//			};
//			
//			_storyPlayer = new StoryPlayerBasic (new ConsoleLogger (), nodeControllers, () => Console.WriteLine ("Fin"));
//			
//			Story story = new StoryCreator (new ConsoleLogger()).CreateStory ("default", _masterDoc, _sceneJson);
//			_scene = story.GetScene ("StubRoute", "StubArc", "StubScene"); 
//		}



		public void InitWithStamina()
		{
			PlayerDataStore data = new PlayerDataStore
			{
				firstName = "foo",
				lastName = "bar",
				stamina = 10,
			};
            PlayerStaminaManager staminaManager = new PlayerStaminaManager(60.0f, 5, data);
			_player = new Player (data, null, null, staminaManager, null);

			Dictionary<Type,INodeController> nodeControllers = new Dictionary<Type,INodeController> ()
			{
				{typeof(Voltage.Story.StoryDivisions.Scene), new SceneNodeController()},
                {typeof(BitNode), new WitchesBitNodeController(_player, new ConsoleLogger()) }
			};
			
			_storyPlayer = new StoryPlayerBasic (new ConsoleLogger (), nodeControllers, () => Console.WriteLine ("Fin"));
			
            var sceneDict = ConstructSceneDictionary(_sceneJson);
			Story story = new StoryCreator (new ConsoleLogger()).CreateStory ("default", _masterDoc, sceneDict);
			_scene = story.GetScene ("StubRoute", "StubArc", "StubScene"); 
		}

		public void InitWithNoStamina()
		{
			PlayerDataStore data = new PlayerDataStore
			{
				firstName = "foo",
				lastName = "bar",
				stamina = 0,
			};
            PlayerStaminaManager staminaManager = new PlayerStaminaManager(60.0f, 5, data);
			_player = new Player(data, null, null, staminaManager, null);


			Dictionary<Type,INodeController> nodeControllers = new Dictionary<Type,INodeController> ()
			{
				{typeof(Voltage.Story.StoryDivisions.Scene), new SceneNodeController()},
                {typeof(BitNode), new WitchesBitNodeController(_player, new ConsoleLogger())}
			};
			
			_storyPlayer = new StoryPlayerBasic (new ConsoleLogger (), nodeControllers, () => Console.WriteLine ("Fin"));
			
            var sceneDict = ConstructSceneDictionary(_sceneJson);
            Story story = new StoryCreator (new ConsoleLogger()).CreateStory ("default", _masterDoc, sceneDict);
			_scene = story.GetScene ("StubRoute", "StubArc", "StubScene"); 
		}



        [Test]
        [Ignore("Stamina will only be deducted when the tutorial is disabled -- that functionality doesnt yet exist")]
        public void Execute_HasStamina_DeductOneStamina()
        {
			InitWithStamina ();

			_storyPlayer.StartScene (_scene);

			Assert.That (_player.Stamina, Is.EqualTo (9));
        }

		[Test]
		[Ignore("Need to Update/Change/Replace Test...time willing")]
		public void Execute_NotEnoughStamina_StaminaUnchanged()
		{
			InitWithNoStamina ();

			_storyPlayer.StartScene (_scene);
			
			Assert.That (_player.Stamina, Is.EqualTo (0));
		}

		private class NetworkControllerSpy : INetworkTimeoutController<WitchesRequestResponse>
		{
			public bool Sent { get; private set; }

			public virtual INetworkTransportLayer Send (string url, IDictionary<string,string> parms, Action<WitchesRequestResponse> onSuccess, Action<WitchesRequestResponse> onFailure, int timeout=0)
			{
				Sent = true;

				return new TransportRequestStub ();
			}

			public virtual INetworkTransportLayer Receive (string url, IDictionary<string,string> parms, Action<WitchesRequestResponse> onSuccess, Action<WitchesRequestResponse> onFailure, int timeout=0)
			{
				return new TransportRequestStub ();
			}
		}

		private class TransportRequestStub : INetworkTransportLayer
		{
			public void Send() {}
			public float Progress { get; set; }
		}

        private IDictionary<string, string> ConstructSceneDictionary(string sceneJson)
        {
            return new Dictionary<string, string>() { {"dummy", sceneJson} };
        }

		

		private string _sceneJson = @"
			{
				""header"": {
					""route"": ""StubRoute"",
					""arc"": ""StubArc"",
					""scene"": ""StubScene"",
					""reqs"": [],
				},
				""data"": [
					{
						""_class"": ""Bit""
					}
				]
			}		
		";

		private string _masterDoc = @"
			{
				""routes"": 
				{
					""StubRoute"": 
					{
						""arcs"": 
						{
							""StubArc"": 
							{
								""scenes"": 
								{
									""StubScene"" : { ""description"": ""Some Description"" }
								}
							},
						}
					}
				}
			}		
		";


//		private string _json = @"
//			{
//				""header"": {
//					""Route"": ""Main"",
//					""Arc"": ""Prologue"",
//					""Group"": {
//						""id"": 1
//					},
//					""Scene"": {
//						""id"": 1,
//						""name"": ""Slow Day Swiftly Interrupted""
//					},
//					""reqs"": []
//				},
//				""data"": [
//					{
//						""_class"": ""Dialogue"",
//						""speaker"": null,
//						""text"": [
//							""Line Text""
//						],
//						""data"": {}
//					},
//					{
//						""_class"": ""Bit""
//					}
//				]
//			}		
//		";

    }
}