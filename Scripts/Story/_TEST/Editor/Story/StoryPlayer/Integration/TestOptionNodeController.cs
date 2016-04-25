//
//using System;
//using System.Collections.Generic;
//
//namespace Voltage.Test.Integration.Story.Models.Nodes.Controllers
//{
//    using NUnit.Framework;
//
//	using Ninject;
//	using Ninject.Modules;
//
//	using Voltage.Common.Logging;
//
//	using Voltage.Witches.Models;
//	using Voltage.Witches.Story.StoryPlayer;
//	using Voltage.Witches.Dependencies;
//	using Voltage.Story.StoryPlayer;
//	using Voltage.Story.Models.Data;
//	using Voltage.Story.Views;
//	using Voltage.Story.StoryDivisions;
//
//	using Scene = Voltage.Story.StoryDivisions.Scene;
////	using Voltage.Story.Models.Nodes.Extensions;
//
//    [TestFixture]
//    public class TestOptionNodeController
//    {
//
//
//		[Test]
//		public void GetWitchesStoryPlayer()
//		{
//			Story story = new StoryCreator (new ConsoleLogger ()).CreateStory ("default", masterDocRoutesJson, allSceneJsons.ToArray ());
//
//			INinjectModule playerModule = CreatePlayerModule ("jon", "doe", "johnny", story);
//			INinjectModule storyPlayerModule = new WitchesStoryPlayerDependencies (new StubLayoutDisplay (), () => Console.WriteLine ("fin"), () => Console.WriteLine ("failed"));
//			
//			IKernel kernel = CreateKernel (playerModule, storyPlayerModule);
//			
//			var storyPlayer = kernel.Get<WitchesStoryPlayer> ();	
//			Assert.That (storyPlayer, Is.Not.Null);
//		}
//
//
//		[Test]
//		public void SelectChoiceA_AddAffinity()
//		{
//			Story story = new StoryCreator (new ConsoleLogger ()).CreateStory ("default", masterDocRoutesJson, allSceneJsons.ToArray ());
//
//			INinjectModule playerModule = CreatePlayerModule ("jon", "doe", "johnny", story);
//			INinjectModule storyPlayerModule = new WitchesStoryPlayerDependencies (new StubLayoutDisplay (), () => Console.WriteLine ("fin"), () => Console.WriteLine ("failed"));
//			
//			IKernel kernel = CreateKernel (playerModule, storyPlayerModule);
//			var storyPlayer = kernel.Get<WitchesStoryPlayer> ();	
//
//			Scene scene = story.GetScene ("Nik-Ty Route Main Story", "NT Salem", "Trail of Breadcrumbs");
//
//			Player player = kernel.Get<Player> ();
//
//			Assert.That (player.GetAffinity ("1"), Is.EqualTo (4)); 
//
//			storyPlayer.StartScene (scene, 4);
//			storyPlayer.Next (0);
//
//			Assert.That (player.GetAffinity ("1"), Is.EqualTo (6)); 
//
//		}
//
//		[Test]
//		public void SelectChoiceA_AddAlignment()
//		{
//			Story story = new StoryCreator (new ConsoleLogger ()).CreateStory ("default", masterDocRoutesJson, allSceneJsons.ToArray ());
//			
//			INinjectModule playerModule = CreatePlayerModule ("jon", "doe", "johnny", story);
//			INinjectModule storyPlayerModule = new WitchesStoryPlayerDependencies (new StubLayoutDisplay (), () => Console.WriteLine ("fin"), () => Console.WriteLine ("failed"));
//			
//			IKernel kernel = CreateKernel (playerModule, storyPlayerModule);
//			var storyPlayer = kernel.Get<WitchesStoryPlayer> ();	
//			
//			Scene scene = story.GetScene ("Nik-Ty Route Main Story", "NT Salem", "Trail of Breadcrumbs");
//			
//			Player player = kernel.Get<Player> ();
//			
//			Assert.That (player.Alignment, Is.EqualTo (7)); 
//			
//			storyPlayer.StartScene (scene, 4);
//			storyPlayer.Next (2);
//			
//			Assert.That (player.Alignment, Is.EqualTo (10)); 
//		}
//
//
//		[Test]
//		public void SelectChoiceA_SubtractAlignment()
//		{
//			Story story = new StoryCreator (new ConsoleLogger ()).CreateStory ("default", masterDocRoutesJson, allSceneJsons.ToArray ());
//			
//			INinjectModule playerModule = CreatePlayerModule ("jon", "doe", "johnny", story);
//			INinjectModule storyPlayerModule = new WitchesStoryPlayerDependencies (new StubLayoutDisplay (), () => Console.WriteLine ("fin"), () => Console.WriteLine ("failed"));
//			
//			IKernel kernel = CreateKernel (playerModule, storyPlayerModule);
//			var storyPlayer = kernel.Get<WitchesStoryPlayer> ();	
//			
//			Scene scene = story.GetScene ("Nik-Ty Route Main Story", "NT Salem", "Trail of Breadcrumbs");
//			
//			Player player = kernel.Get<Player> ();
//			
//			Assert.That (player.Alignment, Is.EqualTo (7)); 
//			
//			storyPlayer.StartScene (scene, 4);
//			storyPlayer.Next (1);
//			
//			Assert.That (player.Alignment, Is.EqualTo (5)); 
//		}
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
//		private INinjectModule CreatePlayerModule(string first, string last, string nickname, Story story) 
//		{ 
//			Player player = CreatePlayer (first, last, nickname);
//			IEnumerable<NPCModel> npcs = CreateNPCs ();
//			
//			return new WitchesPlayerDependencies (player, story, npcs);
//		}
//		
//		
//		private Player CreatePlayer (string first, string last, string nick)
//		{
//			Dictionary<string,int> affinity = new Dictionary<string,int> { {"0",3}, {"1",4} };
//			IDictionary<string,object> data = new Dictionary<string,object>
//			{ 
//				{"firstName", first}, 
//				{"lastName", last},
//				{"nickName", nick},
//				{"userID", "0"},
//				{"alignment", 7},
//				{"stamina", 10},
//				{"affinity", affinity},
//				{"pastAffinity", 10},
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
//									""dialogue 1""
//								]
//							}},
//							{{
//								""_class"": ""Dialogue"",
//								""speaker"": null,
//								""text"": [
//									""dialogue 2""
//								]
//							}},
//							{{
//								""_class"": ""Dialogue"",
//								""speaker"": null,
//								""text"": [
//									""dialogue 3""
//								]
//							}},
//							{{
//								""_class"": ""Selection"",
//								""prompt"": ""choice"",
//								""children"": [
//								    {{
//									""_class"": ""Option"",
//									""text"": [
//									    ""Choice A""
//									    ],
//									""effects"": {{
//										""M"": 2
//										}},
//									""children"": [
//									    {{
//										""_class"": ""Dialogue"",
//										""speaker"": ""USER_FIRST"",
//										""text"": [
//										    ""I thought you'd never ask, but alas, I'd have no one to guard my post.""
//										    ]
//									    }},
//									    {{
//										""_class"": ""Dialogue"",
//										""speaker"": ""Melanie"",
//										""text"": [
//										    ""Come on, you can't say no to me...""
//										    ]
//									    }},
//									    {{
//										""_class"": ""Dialogue"",
//										""speaker"": null,
//										""text"": [
//										    ""Melanie batted her long eyelashes.""
//										    ]
//									    }},
//									    {{
//										""_class"": ""Dialogue"",
//										""speaker"": ""USER_FIRST"",
//										""text"": [
//										    ""Sorry, "",
//										    {{
//											""text"": ""USER2M_NICKNAME"",
//											""_class"": ""Variable""
//										    }},
//										    "", you know I would if I could! But with my luck, Dad'll pick that <i>exact</i> moment to pop in and see how things are going.""
//										    ]
//									    }}
//									    ]
//								    }},
//								    {{
//									""_class"": ""Option"",
//									""text"": [
//									    ""Choice B""
//									    ],
//									""effects"": {{
//										""D"": 2
//										}},
//									""children"": [
//									    {{
//										""_class"": ""Dialogue"",
//										""speaker"": ""USER_FIRST"",
//										""text"": [
//										    ""You know I can't, "",
//										    {{
//											""text"": ""USER2M_NICKNAME"",
//											""_class"": ""Variable""
//										    }},
//										    ""! Dad would murder me!""
//										    ]
//									    }},
//									    {{
//										""_class"": ""Dialogue"",
//										""speaker"": ""Melanie"",
//										""text"": [
//										    ""It's not like we're teenagers anymore. He'll get it if you take a short break.""
//										    ]
//									    }},
//									    {{
//										""_class"": ""Dialogue"",
//										""speaker"": ""USER_FIRST"",
//										""text"": [
//										    ""You just want to get me over to the café so you can gossip with Trinity, don't you?""
//										    ]
//									    }}
//									    ]
//								    }},
//								    {{
//									""_class"": ""Option"",
//									""text"": [
//									    ""Choice C""
//									    ],
//									""effects"": {{
//										""L"": 3
//										}},
//									""children"": [
//									    {{
//										""_class"": ""Dialogue"",
//										""speaker"": ""USER_FIRST"",
//										""text"": [
//										    ""That might mean trouble.""
//										    ]
//									    }},
//									    {{
//										""_class"": ""Dialogue"",
//										""speaker"": ""Melanie"",
//										""text"": [
//										    ""When was the last time I got us into trouble, huh?""
//										    ]
//									    }},
//									    {{
//										""_class"": ""Dialogue"",
//										""speaker"": ""USER_FIRST"",
//										""text"": [
//										    ""When we got thrown out of the theater for being too loud!""
//										    ]
//									    }},
//									    {{
//										""_class"": ""Dialogue"",
//										""speaker"": ""Melanie"",
//										""text"": [
//										    ""Well, that movie was terrible, and we made it better.""
//										    ]
//									    }}
//									    ]
//								    }}
//								    ]
//							}},
//							{{
//								""_class"": ""Dialogue"",
//								""speaker"": null,
//								""text"": [
//									""dialogue last""
//								]
//							}},
//
//						]
//		}}
//		";
//    }
//}

