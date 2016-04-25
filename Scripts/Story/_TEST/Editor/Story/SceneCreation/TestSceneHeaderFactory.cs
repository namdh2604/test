
using System;
using System.Collections.Generic;

namespace Unit.Witches.Story.StoryDivisions.Factory
{
    using NUnit.Framework;
	using Moq;

	using Voltage.Story.General;
	using Voltage.Story.StoryDivisions;
	using Voltage.Witches.Story.StoryDivisions.Factory;
	using Voltage.Witches.Services;
	using Voltage.Witches.Story;
    using Voltage.Witches.Exceptions;

	using SceneHeaderFactory = Voltage.Witches.Story.StoryDivisions.Factory.SceneHeaderFactory;

	using Voltage.Story.Configurations;
	using TermLevel = Voltage.Story.StoryDivisions.Scene.TermLevel;

    [TestFixture]
    public class TestSceneHeaderFactory
    {
//		private string _path;
		
		private Mock<IFilesystemService> _mockFilesystemService;
//		private Mock<ISceneDiscoveryService> _mockDiscoveryService;
		private Mock<IParser<SceneHeader>> _mockHeaderParser;

		[SetUp]
		public void Init()
		{
//			_path = "root";
			
			_mockFilesystemService = new Mock<IFilesystemService> ();
//			_mockDiscoveryService = new Mock<ISceneDiscoveryService> ();
			_mockHeaderParser = new Mock<IParser<SceneHeader>> ();
		}

		private IFactory<string,SceneHeader> CreateFactory()
		{
			return new SceneHeaderFactory (_masterStoryData, _mockHeaderParser.Object, _mockFilesystemService.Object);
		}

        [Test]
        public void Construct()
        {
			var factory = CreateFactory();

			Assert.That (factory, Is.TypeOf<SceneHeaderFactory> ());
        }

		[Test]
		public void Create_ValidPath_ValidSceneHeader()
		{
			_mockHeaderParser.Setup (p => p.Parse (It.IsAny<string>())).Returns (new SceneHeader("Test Route", "Test Arc", "Test Scene", "Description",string.Empty));
			var factory = CreateFactory ();
	
			var header = factory.Create ("Test Route/Test Arc/Test Scene");

			Assert.That (header, Is.TypeOf<SceneHeader>());
		}

		[Test]
		public void Create_ValidPath_HeaderHasPreviewImagePath()
		{
			_mockHeaderParser.Setup (p => p.Parse (It.IsAny<string> ())).Returns (new SceneHeader("Test Route", "Test Arc", "Test Scene", "Description","some/location/on/device/image.png"));
			var factory = CreateFactory ();
			
			var header = factory.Create ("Test Route/Test Arc/Test Scene");
			System.Console.WriteLine(header.PolaroidPath);
			Assert.That (header.PolaroidPath, Is.StringMatching("Polaroids/image"));
		}


		[Test]
		public void Create_InvalidPath_ThrowsException()	
		{
			var factory = CreateFactory ();
			
            Assert.Throws<WitchesException>(() => factory.Create ("Wrong Route/Wrong Arc/Wrong Scene"));
		}


//		[Test]
//		public void Create_ValidPath_ValidSceneHeader()	// not really a valid test
//		{
//			Mock<SceneHeader> mockHeader = new Mock<SceneHeader> ();
//			_mockHeaderParser.Setup (p => p.Parse (It.IsAny<string> ())).Returns (mockHeader.Object);
//
//			var factory = CreateFactory();
//
//			var header = factory.Create ("Route/Arc/Scene");
//
//			Assert.That (header, Is.TypeOf<SceneHeader> ());
//		}

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
			},
			PreviewImages = new Dictionary<string,string>
			{
				{"Test Route/Test Arc/Test Scene", "some/location/on/device/image.png"},
			}
		};


    }
}