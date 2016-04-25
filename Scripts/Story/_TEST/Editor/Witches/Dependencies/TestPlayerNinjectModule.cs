//
//using System;
//using System.Collections.Generic;
//
//namespace Voltage.Test.Integration.Witches.Dependencies
//{
//	using Ninject;
//	using Ninject.Modules;
//
//    using NUnit.Framework;
//
//	using Voltage.Common.Logging;
//
//	using Voltage.Story.StoryDivisions;
//	using Voltage.Story.StoryPlayer;
//
//	using Voltage.Witches.Dependencies;
//	using Voltage.Witches.Models;
//
//	using Voltage.Story.Mapper;
//	using Voltage.Witches.Story.Variables;
//	using Voltage.Story.General;
//	using Voltage.Story.Expressions;
//	using Voltage.Story.Text;
//
//	using Voltage.Story.User;
//	
//
//
//    [TestFixture]
//	public class TestPlayerNinjectModule
//    {
//
//		private INinjectModule CreateModule(string firstname, string lastname, string nickname)
//		{
//			Player player = CreatePlayerModel (firstname, lastname, nickname);
//			return CreateModule (player);
//		}
//		private INinjectModule CreateModule(Player player, Story story=null, IEnumerable<NPCModel> npc=null) 
//		{ 
//			Story s = story == null ? new StoryCreator (new ConsoleLogger ()).CreateStory ("default", masterDocRoutesJson, allSceneJsons.ToArray ()) : story;
//			IEnumerable<NPCModel> n = (npc == null ? CreateNPCs () : npc);
//			return new WitchesPlayerDependencies (player, s, n);
//		}
//
//
//		private IKernel CreateKernel(INinjectModule module)
//		{
//			NinjectSettings settings = new NinjectSettings();
//			settings.LoadExtensions = false;
//			settings.UseReflectionBasedInjection = true;
//			
//			return new StandardKernel(settings, module);
//		}
//
//
//        [Test]
//        public void CreatePlayerDependenciesModule()
//        {
//			INinjectModule module = CreateModule("jon", "doe", "johnny");
//			Assert.That (module, Is.Not.Null);
//        }
//
//
//		[Test]
//		public void CreatePlayerDependenciesKernel()
//		{
//			INinjectModule module = CreateModule("jon", "doe", "johnny");
//
//			IKernel kernel = CreateKernel (module);
//			Assert.That (kernel, Is.Not.Null);
//		}
//
//
//		[Test]
//		public void GetLogger()
//		{
//			INinjectModule module = CreateModule("jon", "doe", "johnny");
//			IKernel kernel = CreateKernel (module);
//
//			var logger = kernel.Get<ILogger> ();
//			Assert.That (logger, Is.Not.Null);
//			Assert.That (logger, Is.TypeOf<CompositeLogger> ());
//		}
//
//		[Test]
//		public void GetPlayer()
//		{
//			Player player = CreatePlayerModel ("jon", "doe", "johnny");
//
//			INinjectModule module = CreateModule (player);
//			IKernel kernel = CreateKernel (module);
//			
//			var ninjectPlayer = kernel.Get<Player> ();
//			Assert.That (ninjectPlayer, Is.Not.Null);
//			Assert.That (ninjectPlayer, Is.SameAs (player));
//		}
//
//		[Test]
//		public void GetIPlayerCheckSingleton()
//		{
//			Player player = CreatePlayerModel ("jon", "doe", "johnny");
//			
//			INinjectModule module = CreateModule (player);
//			IKernel kernel = CreateKernel (module);
//			
//			var ninjectPlayer = kernel.Get<Player> ();
//			Assert.That (ninjectPlayer, Is.Not.Null);
//			Assert.That (ninjectPlayer, Is.SameAs (player));
//
//			var iplayer = kernel.Get<IPlayer> ();
//			Assert.That (iplayer, Is.Not.Null);
//			Assert.That (iplayer, Is.SameAs (ninjectPlayer));
//		}
//
//
//		[Test]
//		public void GetPlayerCheckSingleton()
//		{
//			Player player = CreatePlayerModel ("jon", "doe", "johnny");
//			
//			INinjectModule module = CreateModule (player);
//			IKernel kernel = CreateKernel (module);
//			
//			var ninjectPlayerA = kernel.Get<Player> ();
//			Assert.That (ninjectPlayerA, Is.Not.Null);
//			Assert.That (ninjectPlayerA, Is.SameAs (player));
//
//			var ninjectPlayerB = kernel.Get<Player> ();
//			Assert.That (ninjectPlayerA, Is.Not.Null);
//			Assert.That (ninjectPlayerB, Is.SameAs (ninjectPlayerA));
//		}
//
//		[Test]
//		public void GetStory()
//		{
//			Player player = CreatePlayerModel ("jon", "doe", "johnny");
//			Story story = new StoryCreator (new ConsoleLogger ()).CreateStory ("default", masterDocRoutesJson, allSceneJsons.ToArray ());
//
//			INinjectModule module = CreateModule(player, story);
//			IKernel kernel = CreateKernel (module);
//			
//			var ninjectStory = kernel.Get<Story> ();
//			Assert.That (ninjectStory, Is.Not.Null);
//			Assert.That (ninjectStory, Is.SameAs(story));
//		}
//
//		[Test]
//		public void GetIEnumerableNPCModel()
//		{
//			Player player = CreatePlayerModel ("jon", "doe", "johnny");
//			IEnumerable<NPCModel> npcs = CreateNPCs ();
//
//
//			INinjectModule module = CreateModule(player, null, npcs);
//			IKernel kernel = CreateKernel (module);
//			
//			var ninjectNPCs = kernel.Get<IEnumerable<NPCModel>> ();
//			Assert.That (ninjectNPCs, Is.Not.Null);
//			Assert.That (ninjectNPCs, Is.EqualTo(npcs));
////			Assert.That (ninjectNPCs, Is.TypeOf<IEnumerable<NPCModel>> ());
//		}
//
//		[Test]
//		public void GetListNPCModel()
//		{
//			Player player = CreatePlayerModel ("jon", "doe", "johnny");
//			IEnumerable<NPCModel> npcs = CreateNPCs ();
//			
//			
//			INinjectModule module = CreateModule(player, null, npcs);
//			IKernel kernel = CreateKernel (module);
//			
//			var ninjectNPCs = kernel.Get<List<NPCModel>> ();
//			Assert.That (ninjectNPCs, Is.Not.Null);
//			Assert.That (ninjectNPCs, Is.EqualTo(npcs));
////			Assert.That (ninjectNPCs, Is.TypeOf<List<NPCModel>> ());
//		}
//
//
//
//
//		[Test]
//		public void GetVariableMapper()
//		{
//			INinjectModule module = CreateModule("jon", "doe", "johnny");
//			IKernel kernel = CreateKernel (module);
//			
//			var variableMapperA = kernel.Get<WitchesVariableMapper> ();
//			Assert.That (variableMapperA, Is.Not.Null);
//		}
//
//		[Test]
//		public void GetVariableMapperCheckSingleton()
//		{
//			INinjectModule module = CreateModule("jon", "doe", "johnny");
//			IKernel kernel = CreateKernel (module);
//			
//			var variableMapperA = kernel.Get<WitchesVariableMapper> ();
//			Assert.That (variableMapperA, Is.Not.Null);
//			
//			var variableMapperB = kernel.Get<WitchesVariableMapper> ();
//			Assert.That (variableMapperB, Is.Not.Null);

//			Console.WriteLine (variableMapperB.GetType ());
//			Assert.That (variableMapperB, Is.SameAs (variableMapperA));
//		}
//
//		[Test]
//		public void GetIMappingCheckSingleton()
//		{
//			INinjectModule module = CreateModule("jon", "doe", "johnny");
//			IKernel kernel = CreateKernel (module);
//			
//			var variableMapperA = kernel.Get<IMapping<string>> ();
//			Assert.That (variableMapperA, Is.Not.Null);
//			
//			var variableMapperB = kernel.Get<WitchesVariableMapper> ();
//			Assert.That (variableMapperB, Is.Not.Null);
//			Assert.That (variableMapperB, Is.SameAs (variableMapperA));
//		}
//
//
//		[Test]
//		public void GetExpressionParser()
//		{
//			INinjectModule module = CreateModule("jon", "doe", "johnny");
//			IKernel kernel = CreateKernel (module);
//			
//			var expressionParser = kernel.Get<ExpressionParser> ();
//			Assert.That (expressionParser, Is.Not.Null);
//		}
//
//		[Test]
//		public void GetIParser()
//		{
//			INinjectModule module = CreateModule("jon", "doe", "johnny");
//			IKernel kernel = CreateKernel (module);
//			
//			var expressionParser = kernel.Get<IParser<ExpressionState>> ();
//			Assert.That (expressionParser, Is.Not.Null);
//			Assert.That (expressionParser, Is.TypeOf<ExpressionParser> ());
//		}
//
//
//		[Test]
//		public void GetExpressionFactory()
//		{
//			INinjectModule module = CreateModule("jon", "doe", "johnny");
//			IKernel kernel = CreateKernel (module);
//			
//			var expressionParser = kernel.Get<ExpressionFactory> ();
//			Assert.That (expressionParser, Is.Not.Null);
//		}
//		
//		[Test]
//		public void GetIExpressionFactory()
//		{
//			INinjectModule module = CreateModule("jon", "doe", "johnny");
//			IKernel kernel = CreateKernel (module);
//			
//			var expressionParser = kernel.Get<IExpressionFactory> ();
//			Assert.That (expressionParser, Is.Not.Null);
//			Assert.That (expressionParser, Is.TypeOf<ExpressionFactory> ());
//		}
//
//		[Test]
//		public void TwoPlayerModuleNotTheSamePlayer()
//		{
//			INinjectModule moduleA = CreateModule("jon", "doe", "johnny");
//			IKernel kernelA = CreateKernel (moduleA);
//
//			INinjectModule moduleB = CreateModule("jon", "doe", "johnny");
//			IKernel kernelB = CreateKernel (moduleB);
//
//			Player playerA = kernelA.Get<Player> ();
//			Player playerB = kernelB.Get<Player> ();
//			Assert.That (playerA, Is.Not.SameAs (playerB));
//		}
//
//
//
//		[Test]
//		public void GetTextParser()
//		{
//			INinjectModule module = CreateModule("jon", "doe", "johnny");
//			IKernel kernel = CreateKernel (module);
//			
//			var textParser = kernel.Get<VariableTextParser> ();
//			Assert.That (textParser, Is.Not.Null);
//		}
//
//		[Test]
//		public void GetTextParserByInterface()
//		{
//			INinjectModule module = CreateModule("jon", "doe", "johnny");
//			IKernel kernel = CreateKernel (module);
//			
//			var textParser = kernel.Get<IParser<string>> ();
//			Assert.That (textParser, Is.Not.Null);
//			Assert.That (textParser, Is.TypeOf<VariableTextParser> ());
//		}
//
//
//		[Test]
//		public void TextParserVariableMapperSame()
//		{
//			INinjectModule module = CreateModule("jon", "doe", "johnny");
//			IKernel kernel = CreateKernel (module);
//			
//			var textParser = kernel.Get<VariableTextParser> ();
//			var variableMapper = kernel.Get<WitchesVariableMapper> ();
//
//			Assert.That (textParser.VariableMapper, Is.SameAs (variableMapper));
//		}
//
//
//
//
//		private IEnumerable<NPCModel> CreateNPCs()
//		{
//			return new List<NPCModel> { new NPCModel("0", "bob","fisher",true), new NPCModel("1", "mahatma","gandhi",false) };
//		}
//		
//		private Player CreatePlayerModel (string first, string last, string nick)
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
//    }
//}

