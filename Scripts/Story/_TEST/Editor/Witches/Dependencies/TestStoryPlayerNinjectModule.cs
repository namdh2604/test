//
//using System;
//using System.Collections.Generic;
//
//namespace Voltage.Test.Integration.Witches.Dependencies
//{
//    using NUnit.Framework;
//
//	using Ninject;
//	using Ninject.Modules;
//
//	using Voltage.Common.Logging;
//
//	using Voltage.Story.StoryPlayer;
//	using Story = Voltage.Story.StoryDivisions.Story;
//
//	using Voltage.Witches.Story.StoryPlayer;
//	using Voltage.Witches.Dependencies;
//	using Voltage.Witches.Models;
//	using Voltage.Story.User;
//	using Voltage.Story.Models.Nodes.Controllers;
//
//	using Voltage.Story.Views;
//	using Voltage.Story.Models.Data;
//
//	using Voltage.Story.Effects;
//	using Voltage.Witches.Story.Effects;
//
//	using Voltage.Story.StoryDivisions;
//	using Voltage.Witches.Story.Models.Nodes.Controllers;
//
//
//    [TestFixture]
//    public class TestStoryPlayerNinjectModule
//    {
//        [Test]
//		public void CreateStoryPlayerDependenciesModule()
//        {
//			INinjectModule storyPlayerModule = new WitchesStoryPlayerDependencies (new StubLayoutDisplay (), () => Console.WriteLine ("fin"), () => Console.WriteLine ("failed"));
//			Assert.That (storyPlayerModule, Is.Not.Null);
//        }
//
//
//		[Test]
//		public void CreateKernel()
//		{
//			INinjectModule playerModule = CreatePlayerModule ("jon", "doe", "johnny");
//			INinjectModule storyPlayerModule = new WitchesStoryPlayerDependencies (new StubLayoutDisplay (), () => Console.WriteLine ("fin"), () => Console.WriteLine ("failed"));
//
//			IKernel kernel = CreateKernel (playerModule, storyPlayerModule);
//			Assert.That (kernel, Is.Not.Null);
//		}
//
//		[Test]
//		public void GetLogger()
//		{
//			INinjectModule playerModule = CreatePlayerModule ("jon", "doe", "johnny");
//			INinjectModule storyPlayerModule = new WitchesStoryPlayerDependencies (new StubLayoutDisplay (), () => Console.WriteLine ("fin"), () => Console.WriteLine ("failed"));
//			
//			IKernel kernel = CreateKernel (playerModule, storyPlayerModule);
//
//			var logger = kernel.Get<ILogger> ();
//			Assert.That (logger, Is.Not.Null);
//			Assert.That (logger, Is.TypeOf<CompositeLogger> ());
//		}
//
//
//		[Test]
//		public void GetPlayerModuleStory()
//		{
//			INinjectModule playerModule = CreatePlayerModule ("jon", "doe", "johnny");
//			INinjectModule storyPlayerModule = new WitchesStoryPlayerDependencies (new StubLayoutDisplay (), () => Console.WriteLine ("fin"), () => Console.WriteLine ("failed"));
//			
//			IKernel kernel = CreateKernel (playerModule, storyPlayerModule);
//			
//			var story = kernel.Get<Story> ();	// NOTE: actually from playerModule
//			Assert.That (story, Is.Not.Null);
//			Assert.That (story.Name, Is.StringMatching ("default"));
//		}
//
//
//		[Test]
//		public void GetPlayerModulePlayer()
//		{
//			INinjectModule playerModule = CreatePlayerModule ("jon", "doe", "johnny");
//			INinjectModule storyPlayerModule = new WitchesStoryPlayerDependencies (new StubLayoutDisplay (), () => Console.WriteLine ("fin"), () => Console.WriteLine ("failed"));
//			
//			IKernel kernel = CreateKernel (playerModule, storyPlayerModule);
//			
//			var player = kernel.Get<Player> ();	// NOTE: actually from playerModule
//			Assert.That (player, Is.Not.Null);
//			Assert.That (player.FirstName, Is.StringMatching ("jon"));
//		}
//
//		[Test]
//		public void GetDialogueNodeController()
//		{
//			INinjectModule playerModule = CreatePlayerModule ("jon", "doe", "johnny");
//			INinjectModule storyPlayerModule = new WitchesStoryPlayerDependencies (new StubLayoutDisplay (), () => Console.WriteLine ("fin"), () => Console.WriteLine ("failed"));
//			
//			IKernel kernel = CreateKernel (playerModule, storyPlayerModule);
//			
//			var controller = kernel.Get<DialogueNodeController> ();	
//			Assert.That (controller, Is.Not.Null);
//			Assert.That (controller, Is.TypeOf<DialogueNodeController>());
//		}
//
//		[Test]
//		public void GetSelectionNodeController()
//		{
//			INinjectModule playerModule = CreatePlayerModule ("jon", "doe", "johnny");
//			INinjectModule storyPlayerModule = new WitchesStoryPlayerDependencies (new StubLayoutDisplay (), () => Console.WriteLine ("fin"), () => Console.WriteLine ("failed"));
//			
//			IKernel kernel = CreateKernel (playerModule, storyPlayerModule);
//			
//			var controller = kernel.Get<SelectionNodeController> ();	
//			Assert.That (controller, Is.Not.Null);
//			Assert.That (controller, Is.TypeOf<SelectionNodeController>());
//		}
//
//		[Test]
//		public void GetConditionalNodeController()
//		{
//			INinjectModule playerModule = CreatePlayerModule ("jon", "doe", "johnny");
//			INinjectModule storyPlayerModule = new WitchesStoryPlayerDependencies (new StubLayoutDisplay (), () => Console.WriteLine ("fin"), () => Console.WriteLine ("failed"));
//			
//			IKernel kernel = CreateKernel (playerModule, storyPlayerModule);
//			
//			var controller = kernel.Get<ConditionalNodeController> ();	
//			Assert.That (controller, Is.Not.Null);
//			Assert.That (controller, Is.TypeOf<ConditionalNodeController>());
//		}
//
//		[Test]
//		public void GetWitchesStoryPlayer()
//		{
//			INinjectModule playerModule = CreatePlayerModule ("jon", "doe", "johnny");
//			INinjectModule storyPlayerModule = new WitchesStoryPlayerDependencies (new StubLayoutDisplay (), () => Console.WriteLine ("fin"), () => Console.WriteLine ("failed"));
//			
//			IKernel kernel = CreateKernel (playerModule, storyPlayerModule);
//			
//			var storyPlayer = kernel.Get<WitchesStoryPlayer> ();	
//			Assert.That (storyPlayer, Is.Not.Null);
//			Assert.That (storyPlayer, Is.TypeOf<WitchesStoryPlayer>());
//		}
//
//		[Test]
//		public void GetIStoryPlayer()
//		{
//			INinjectModule playerModule = CreatePlayerModule ("jon", "doe", "johnny");
//			INinjectModule storyPlayerModule = new WitchesStoryPlayerDependencies (new StubLayoutDisplay (), () => Console.WriteLine ("fin"), () => Console.WriteLine ("failed"));
//			
//			IKernel kernel = CreateKernel (playerModule, storyPlayerModule);
//			
//			var iStoryPlayer = kernel.Get<IStoryPlayer> ();	
//			Assert.That (iStoryPlayer, Is.Not.Null);
//			Assert.That (iStoryPlayer, Is.TypeOf<WitchesStoryPlayer>());
//		}
//
//
//		[Test]
//		public void GetStoryPlayerNewInstance()
//		{
//			INinjectModule playerModule = CreatePlayerModule ("jon", "doe", "johnny");
//			INinjectModule storyPlayerModule = new WitchesStoryPlayerDependencies (new StubLayoutDisplay (), () => Console.WriteLine ("fin"), () => Console.WriteLine ("failed"));
//			
//			IKernel kernel = CreateKernel (playerModule, storyPlayerModule);
//			
//			var witchesStoryPlayerA = kernel.Get<WitchesStoryPlayer> ();
//			Assert.That (witchesStoryPlayerA, Is.Not.Null);
//			Assert.That (witchesStoryPlayerA, Is.TypeOf<WitchesStoryPlayer>());
//			
//			var witchesStoryPlayerB = kernel.Get<WitchesStoryPlayer> ();	
//			Assert.That (witchesStoryPlayerB, Is.Not.Null);
//			Assert.That (witchesStoryPlayerB, Is.TypeOf<WitchesStoryPlayer>());
//
//			Assert.That (witchesStoryPlayerB, Is.Not.SameAs (witchesStoryPlayerA));
//		}
//
//
//		[Test]
//		public void GetWitchesResolver()
//		{
//			INinjectModule playerModule = CreatePlayerModule ("jon", "doe", "johnny");
//			INinjectModule storyPlayerModule = new WitchesStoryPlayerDependencies (new StubLayoutDisplay (), () => Console.WriteLine ("fin"), () => Console.WriteLine ("failed"));
//			
//			IKernel kernel = CreateKernel (playerModule, storyPlayerModule);
//			
//			var resolver = kernel.Get<WitchesEffectResolver> ();	
//			Assert.That (resolver, Is.Not.Null);
//			Assert.That (resolver, Is.TypeOf<WitchesEffectResolver>());
//		}
//
//
//		[Test]
//		public void GetIResolver()
//		{
//			INinjectModule playerModule = CreatePlayerModule ("jon", "doe", "johnny");
//			INinjectModule storyPlayerModule = new WitchesStoryPlayerDependencies (new StubLayoutDisplay (), () => Console.WriteLine ("fin"), () => Console.WriteLine ("failed"));
//			
//			IKernel kernel = CreateKernel (playerModule, storyPlayerModule);
//			
//			var resolver = kernel.Get<IResolver> ();	
//			Assert.That (resolver, Is.Not.Null);
//			Assert.That (resolver, Is.TypeOf<WitchesEffectResolver>());
//		}
//
//
//		[Test]
//		public void GetOptionNodeController()
//		{
//			INinjectModule playerModule = CreatePlayerModule ("jon", "doe", "johnny");
//			INinjectModule storyPlayerModule = new WitchesStoryPlayerDependencies (new StubLayoutDisplay (), () => Console.WriteLine ("fin"), () => Console.WriteLine ("failed"));
//			
//			IKernel kernel = CreateKernel (playerModule, storyPlayerModule);
//			
//			var controller = kernel.Get<OptionNodeController> ();	
//			Assert.That (controller, Is.Not.Null);
//			Assert.That (controller, Is.TypeOf<OptionNodeController>());
//			Assert.That (controller.Resolver, Is.Not.Null);
//		}
//
//		[Test]
//		public void GetBranchNodeController()
//		{
//			INinjectModule playerModule = CreatePlayerModule ("jon", "doe", "johnny");
//			INinjectModule storyPlayerModule = new WitchesStoryPlayerDependencies (new StubLayoutDisplay (), () => Console.WriteLine ("fin"), () => Console.WriteLine ("failed"));
//			
//			IKernel kernel = CreateKernel (playerModule, storyPlayerModule);
//			
//			var controller = kernel.Get<BranchNodeController> ();	
//			Assert.That (controller, Is.Not.Null);
//			Assert.That (controller, Is.TypeOf<BranchNodeController>());
//		}
//
//
//
//
//		[Test]
//		public void GetLockNodeController()
//		{
//			INinjectModule playerModule = CreatePlayerModule ("jon", "doe", "johnny");
//			INinjectModule storyPlayerModule = new WitchesStoryPlayerDependencies (new StubLayoutDisplay (), () => Console.WriteLine ("fin"), () => Console.WriteLine ("failed"));
//			
//			IKernel kernel = CreateKernel (playerModule, storyPlayerModule);
//			
//			var controller = kernel.Get<WitchesLockNodeController> ();	
//			Assert.That (controller, Is.Not.Null);
//			Assert.That (controller, Is.TypeOf<WitchesLockNodeController>());
//		}
//
//		[Test]
//		public void GetUnlockNodeController()
//		{
//			INinjectModule playerModule = CreatePlayerModule ("jon", "doe", "johnny");
//			INinjectModule storyPlayerModule = new WitchesStoryPlayerDependencies (new StubLayoutDisplay (), () => Console.WriteLine ("fin"), () => Console.WriteLine ("failed"));
//			
//			IKernel kernel = CreateKernel (playerModule, storyPlayerModule);
//			
//			var controller = kernel.Get<WitchesUnlockNodeController> ();	
//			Assert.That (controller, Is.Not.Null);
//			Assert.That (controller, Is.TypeOf<WitchesUnlockNodeController>());
//		}
//
//		[Test]
//		public void GetISceneHeaderFactory()
//		{
//			INinjectModule playerModule = CreatePlayerModule ("jon", "doe", "johnny");
//			INinjectModule storyPlayerModule = new WitchesStoryPlayerDependencies (new StubLayoutDisplay (), () => Console.WriteLine ("fin"), () => Console.WriteLine ("failed"));
//			
//			IKernel kernel = CreateKernel (playerModule, storyPlayerModule);
//			
//			var controller = kernel.Get<ISceneHeaderFactory> ();	
//			Assert.That (controller, Is.Not.Null);
//			Assert.That (controller, Is.TypeOf<MockSceneHeaderFactory>());
//		}
//
//
//
//
//
//
//
//
//
//
//
//
//		private class StubLayoutDisplay : ILayoutDisplay
//		{
//			public void DisplayDialogue(DialogueNodeViewData node, Action<List<string>> readyCallback, Action<int> inputCallback) {}
//			public void DisplaySelection(SelectionNodeViewData node, Action<List<string>> readyCallback, Action<int> inputCallback) {}
//		}
//
//
//		private IKernel CreateKernel(params INinjectModule[] modules)
//		{
//			NinjectSettings settings = new NinjectSettings();
//			settings.LoadExtensions = false;
//			settings.UseReflectionBasedInjection = true;
//			
//			return new StandardKernel(settings, modules);
//		}
//
//
//		private INinjectModule CreatePlayerModule(string first, string last, string nickname) 
//		{ 
//			Player player = CreatePlayer (first, last, nickname);
//			Story story = new StoryCreator (new ConsoleLogger ()).CreateStory ("default", masterDocRoutesJson, allSceneJsons.ToArray ());
//			IEnumerable<NPCModel> npcs = CreateNPCs ();
//
//			return new WitchesPlayerDependencies (player, story, npcs);
//		}
//
//
//		private Player CreatePlayer (string first, string last, string nick)
//		{
//			Dictionary<string,object> data = new Dictionary<string,object>
//			{
//				{"firstName", first},
//				{"lastName", last},
//				{"nickName", nick}
//			};
//			
//			return new Player (data);
//		}
//		
//		private IEnumerable<NPCModel> CreateNPCs()
//		{
//			return new List<NPCModel> { new NPCModel("0", "bob","fisher",true), new NPCModel("1", "mahatma","gandhi",false) };
//		}
//	
//		
//		
//		private string masterDocRoutesJson = @"
//			{
//			""routes"": {
//			   ""Nik-Ty Route Main Story"": {
//			     ""arcs"": {
//			       ""NT Salem"": {
//			         ""scenes"": [
//			           ""Trail of Breadcrumbs"",
//			           ""A Broken Sky""
//			          ]
//			        },
//			       ""NT Czech Republic"": {
//			         ""scenes"": [
//			           ""Born to be Blue"",
//			           ""Keeping Friends Close""
//			          ]
//			        },
//			       ""NT Ireland"": {
//			         ""scenes"": [
//			           ""Ireland or Bust"",
//			           ""At Ease"",
//			           ""Indulgence""
//			          ]
//			        }
//			      }
//			    },
//			   ""Rhys-Ty Main Story"": {
//			     ""arcs"": {
//			       ""RT Czech Republic"": {
//			         ""scenes"": [
//			           ""Lost in the signs""
//			          ]
//			        },
//			       ""RT Ireland"": {
//			         ""scenes"": [
//			           ""Arrival in Ireland"",
//			           ""The Possibilities"",
//			           ""A Broken Sky"",
//			           ""Dark Cloud""
//			          ]
//			        }
//			      }
//			    },
//			   ""Rhys-Ana Main Story"": {
//			     ""arcs"": {
//			       ""RA Germany"": {
//			         ""scenes"": [
//			           ""The Apothecary"",
//			           ""Debtors""
//			          ]
//			        },
//			      }
//			    },
//			   ""Shared Main Story"": {
//			     ""arcs"": {
//			       ""Shared Ireland"": {
//			         ""scenes"": [
//			          ]
//			        },
//			       ""Shared Prologue"": {
//			         ""scenes"": [
//			           ""Slow Day Swiftly Interrupted"",
//			           ""Mending Luna"",
//			           ""A Lesson in Consequence"",
//			           ""Trail of Breadcrumbs"",
//			           ""The Private Collection""
//			          ]
//			        },
//			       ""Shared Czech Republic"": {
//			         ""scenes"": [
//			           ""The Third Dream"",
//			           ""House of Cards"",
//			          ]
//			        },
//			       ""Shared Salem"": {
//			         ""scenes"": [
//			           ""The Last Dream""
//			          ]
//			        },
//			      }
//			    }
//			  }
//			}
//		";
//		
//		
//		private static string GetSceneJson (string route, string arc, string scene)
//		{
//			return string.Format (sceneTemplateJson, route, arc, scene);
//		}
//		
//		private List<string> allSceneJsons = new List<string>
//		{
//			GetSceneJson("Nik-Ty Route Main Story", "NT Salem", "Trail of Breadcrumbs"),
//			GetSceneJson("Nik-Ty Route Main Story", "NT Salem", "A Broken Sky"),
//			GetSceneJson("Nik-Ty Route Main Story", "NT Czech Republic", "Born to be Blue"),
//			GetSceneJson("Nik-Ty Route Main Story", "NT Czech Republic", "Keeping Friends Close"),
//			GetSceneJson("Nik-Ty Route Main Story", "NT Ireland", "Ireland or Bust"),
//			GetSceneJson("Nik-Ty Route Main Story", "NT Ireland", "At Ease"),
//			GetSceneJson("Nik-Ty Route Main Story", "NT Ireland", "Indulgence"),
//			
//			GetSceneJson("Rhys-Ty Main Story", "RT Czech Republic", "Lost in the signs"),
//			GetSceneJson("Rhys-Ty Main Story", "RT Ireland", "Arrival in Ireland"),
//			GetSceneJson("Rhys-Ty Main Story", "RT Ireland", "The Possibilities"),
//			GetSceneJson("Rhys-Ty Main Story", "RT Ireland", "A Broken Sky"),
//			GetSceneJson("Rhys-Ty Main Story", "RT Ireland", "Dark Cloud"),
//			
//			GetSceneJson("Rhys-Ana Main Story", "RA Germany", "The Apothecary"),
//			GetSceneJson("Rhys-Ana Main Story", "RA Germany", "Debtors"),
//			
//			GetSceneJson("Shared Main Story", "Shared Prologue", "Slow Day Swiftly Interrupted"),
//			GetSceneJson("Shared Main Story", "Shared Prologue", "Mending Luna"),
//			GetSceneJson("Shared Main Story", "Shared Prologue", "A Lesson in Consequence"),
//			GetSceneJson("Shared Main Story", "Shared Prologue", "Trail of Breadcrumbs"),
//			GetSceneJson("Shared Main Story", "Shared Prologue", "The Private Collection"),
//			GetSceneJson("Shared Main Story", "Shared Czech Republic", "The Third Dream"),
//			GetSceneJson("Shared Main Story", "Shared Czech Republic", "House of Cards"),
//			GetSceneJson("Shared Main Story", "Shared Salem", "The Last Dream"),
//		};
//		
//		
//		private static string sceneTemplateJson = @" 
//		{{
//			""header"": 	{{
//								""route"": ""{0}"",
//								""arc"": ""{1}"",
//								""scene"": ""{2}""
//
//		    }},
//			""data"": 	[
//							{{
//								""_class"": ""Dialogue"",
//								""speaker"": null,
//								""text"": [
//									""lorem ipsum""
//								]
//							}}
//						]
//		}}
//		";
//
//    }
//}

