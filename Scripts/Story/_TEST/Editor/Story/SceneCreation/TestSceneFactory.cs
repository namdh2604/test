
using System;
using System.Collections.Generic;

namespace Unit.Witches.Story.StoryDivisions.Factory
{
    using NUnit.Framework;
	using Moq;

	using Voltage.Witches.Story.StoryDivisions.Factory;

	using Voltage.Story.General;
	using Voltage.Witches.Story;
	using Voltage.Story.StoryDivisions;
	using Voltage.Story.Configurations;
	using Voltage.Witches.Services;

	using TermLevel = Voltage.Story.StoryDivisions.Scene.TermLevel;

    [TestFixture]
    public class TestSceneFactory
    {
//		private string _path;

		private Mock<IFilesystemService> _mock_filesystemService;
//		private Mock<ISceneDiscoveryService> _mock_discoveryService;

		[SetUp]
		public void Init()
		{
//			_path = "root";

			_mock_filesystemService = new Mock<IFilesystemService> ();
//			_mock_discoveryService = new Mock<ISceneDiscoveryService> ();
		}


		private SceneFactory CreateFactory()
		{
			return new SceneFactory (_masterStoryData, _mock_filesystemService.Object);
		}

        [Test]
        public void Constructor()
        {
			var factory = CreateFactory ();

			Assert.That (factory, Is.TypeOf<SceneFactory> ());
        }



		[Test]
		public void Create_ValidPath_CorrectScene()
		{
			string scenePath = "Test Route/Test Arc/Test Scene";
			string externalFilePath = "some/location/on/device";
//			_mock_discoveryService.Setup (service => service.DiscoverScenes (_path)).Returns (new Dictionary<string, string> {{scenePath, externalFilePath}});
			_mock_filesystemService.Setup (service => service.ReadAllText (It.Is<string> ((str) => str == externalFilePath))).Returns (_json);
			
			SceneFactory factory = CreateFactory ();
			
			Scene scene = factory.Create (scenePath);
			
			Assert.That (scene.Name, Is.StringMatching("Test Scene"));
		}


		[Test]
		public void Create_ValidPath_ProperDescription()
		{
			string scenePath = "Test Route/Test Arc/Test Scene";
			string externalFilePath = "some/location/on/device";
//			_mock_discoveryService.Setup (service => service.DiscoverScenes (_path)).Returns (new Dictionary<string, string> {{scenePath, externalFilePath}});
			_mock_filesystemService.Setup (service => service.ReadAllText (It.Is<string> ((str) => str == externalFilePath))).Returns (_json);
			
			SceneFactory factory = CreateFactory ();
			
			Scene scene = factory.Create (scenePath);
			
			Assert.That (scene.Description, Is.StringMatching("hello world"));
		}

		[Test]
		public void Create_ValidPath_ProperTerminationLevel()
		{
			string scenePath = "Test Route/Test Arc/Test Scene";
			string externalFilePath = "some/location/on/device";
//			_mock_discoveryService.Setup (service => service.DiscoverScenes (_path)).Returns (new Dictionary<string, string> {{scenePath, externalFilePath}});
			_mock_filesystemService.Setup (service => service.ReadAllText (It.Is<string> ((str) => str == externalFilePath))).Returns (_json);
			
			SceneFactory factory = CreateFactory ();
			
			Scene scene = factory.Create (scenePath);
			
			Assert.That (scene.TerminationLevel, Is.EqualTo(TermLevel.Route));
		}



		private string _json = @"
				{
				    ""header"": {
				        ""route"": ""Test Route"",
				        ""reqs"": [],
				        ""arc"": ""Test Arc"",
				        ""scene"": ""Test Scene""
				    },
				    ""data"": [
				        {
				            ""_class"": ""Dialogue"",
				            ""speaker"": null,
				            ""text"": [
				                ""Hello World""
				            ],
				            ""data"": {
				                ""left"": {
				                    ""enabled"": false
				                },
								""background"": ""Werbury Downtown"",
				                ""speechBox"": ""Narration""
				            }
				        },
				    ]
				}
		";

		private MasterStoryData _masterStoryData = new MasterStoryData
		{
			SceneDescriptions = new Dictionary<string,string>
			{
				{"Test Route/Test Arc/Test Scene", "hello world"},
			},
			SceneTerminationLevels = new Dictionary<string,TermLevel>
			{
				{"Test Route/Test Arc/Test Scene", TermLevel.Route},
			},
			SceneToFileMap = new Dictionary<string,string>
			{
				{"Test Route/Test Arc/Test Scene", "some/location/on/device"},
			}
		};



    }
}