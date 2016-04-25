
using System;
using System.Collections.Generic;

namespace Unit.Witches.DI
{
    using NUnit.Framework;

	using Voltage.Witches.DI;
	using Voltage.Story.Configurations;

	using TermLevel = Voltage.Story.StoryDivisions.Scene.TermLevel;

    [TestFixture]
    public class TestMasterStoryDataParser
    {
        [Test]
        public void Construct()
        {
			var parser = new MasterStoryDataParser ();

			Assert.That (parser, Is.TypeOf<MasterStoryDataParser> ());
        }



		[Test]
		public void Parse_ValidSimpleJson_ValidDescription()
		{
			var parser = new MasterStoryDataParser ();

			MasterStoryData data = parser.Parse (_simpleMasterStoryData);

			Assert.That (data.SceneDescriptions["Test Route/Test Arc/Test Scene"], Is.StringMatching("foobar"));
		}

		[Test]
		public void Parse_ValidNullJson_EmptyStringDescription()
		{
			var parser = new MasterStoryDataParser ();
			
			MasterStoryData data = parser.Parse (_nullMasterStoryData);
			
			Assert.That (data.SceneDescriptions["Test Route/Test Arc/Test Scene"], Is.Empty);
		}

		[Test]
		public void Parse_ValidMultipleJson_ValidDescription()
		{
			var parser = new MasterStoryDataParser ();
			
			MasterStoryData data = parser.Parse (_multipleMasterStoryData);
			
			Assert.That (data.SceneDescriptions["Test Route/Another Arc/Another Scene"], Is.StringMatching("baz"));
		}

//		[Test]
//		public void Parse_IncorrectScene_Throws()
//		{
//			var parser = new MasterStoryDataParser ();
//			
//			MasterStoryData data = parser.Parse (_simpleMasterStoryData);
//			
//			Assert.Throws<KeyNotFoundException>(() => data.SceneDescription["Route/Arc/Scene Does Not Exist"]);
//		}


		[Test]
		public void Parse_CustomFormat_ValidDescription()
		{
			var parser = new MasterStoryDataParser ("{0}.{1}.{2}");
			
			MasterStoryData data = parser.Parse (_simpleMasterStoryData);
			
			Assert.That (data.SceneDescriptions["Test Route.Test Arc.Test Scene"], Is.StringMatching("foobar"));
		}

		[Test]
		public void Parse_NoTerminationLevel_DefaultTerminationLevel()
		{
			var parser = new MasterStoryDataParser ();
			
			MasterStoryData data = parser.Parse (_simpleMasterStoryData);

			Assert.That (data.SceneTerminationLevels["Test Route/Test Arc/Test Scene"], Is.EqualTo (TermLevel.None));
		}

		[Test]
		public void Parse_ZeroTerminationLevel_DefaultTerminationLevel()
		{
			var parser = new MasterStoryDataParser ();
			
			MasterStoryData data = parser.Parse (_terminateLevelZeroMasterStoryData);
			
			Assert.That (data.SceneTerminationLevels["Test Route/Test Arc/Test Scene"], Is.EqualTo (TermLevel.None));
		}

		[Test]
		public void Parse_TwoTerminationLevel_ProperTerminationLevel()
		{
			var parser = new MasterStoryDataParser ();
			
			MasterStoryData data = parser.Parse (_terminateLevelTwoMasterStoryData);

			TermLevel expectedLevel = (TermLevel)2;
			
			Assert.That (data.SceneTerminationLevels["Test Route/Test Arc/Test Scene"], Is.EqualTo (expectedLevel));
		}

		[Ignore("Functionality (temporarily?) disabled, field may not be appropriate")]
		[Test]
		public void Parse_ValidMultipleArcsJson_ValidArcCount()
		{
//			var parser = new MasterStoryDataParser ();
//			
//			MasterStoryData data = parser.Parse (_multipleMasterStoryData);
//			
//			Assert.That (data.StoryDic["Test Route"].Count, Is.EqualTo(2));
		}

		[Ignore("Functionality (temporarily?) disabled, field may not be appropriate")]
		public void Parse_ValidJson_ValidScene()
		{
//			var parser = new MasterStoryDataParser ();
//			
//			MasterStoryData data = parser.Parse (_simpleMasterStoryData);
//			
//			Assert.That (data.StoryDic["Test Route"]["Test Arc"][0], Is.StringMatching("Test Scene"));
		}


		[Test]
		public void Parse_ListSelectionNames_ValidListOfSelectionNames()
		{
			var parser = new MasterStoryDataParser ();
			
			MasterStoryData data = parser.Parse (_selectionNamesMasterStoryData);

			Assert.That (data.SceneNamedSelections.Count, Is.EqualTo (2));
		}

		[Test]
		public void Parse_ListSelectionNames_ValidType()
		{
			var parser = new MasterStoryDataParser ();
			
			MasterStoryData data = parser.Parse (_selectionNamesMasterStoryData);
			
			Assert.That (data.SceneNamedSelections, Is.TypeOf<List<NamedSelectionData>>());
		}

		[Test]
		public void Parse_EmptyListSelectionNames_EmptyListOfSelectionNames()
		{
			var parser = new MasterStoryDataParser ();
			
			MasterStoryData data = parser.Parse (_emptySelectionNamesMasterStoryData);
			
			Assert.That (data.SceneNamedSelections.Count, Is.EqualTo (0));
		}

		[Test]
		public void Parse_PreviewImage_ValidDictionaryOfPreviewImages()
		{
			var parser = new MasterStoryDataParser ();
			
			MasterStoryData data = parser.Parse (_previewPathMasterStoryData);
			
			Assert.That (data.PreviewImages["Test Route/Test Arc/Test Scene"], Is.StringMatching("test/path/image.png"));
		}

		[Test]
		public void Parse_EmptyPreviewImage_EmptyPreviewImage()
		{
			var parser = new MasterStoryDataParser ();
			
			MasterStoryData data = parser.Parse (_emptyPreviewPathMasterStoryData);
			
			Assert.That (data.PreviewImages["Test Route/Test Arc/Test Scene"], Is.Empty);
		}



		private string _simpleMasterStoryData = @"
				{
				    ""routes"": 
				    {
				        ""Test Route"": 
				        {
				            ""arcs"": 
				            {
				                ""Test Arc"": 
				                {
				                    ""scenes"": 
				                    {
				                        ""Test Scene"": 
				                        {
				                            ""description"": ""foobar""
				                        },
				                    }
				                },
				            }
				        },
				    }
				}
		";



		private string _selectionNamesMasterStoryData = @"
				{
				    ""routes"": 
				    {
				        ""Test Route"": 
				        {
				            ""arcs"": 
				            {
				                ""Test Arc"": 
				                {
				                    ""scenes"": 
				                    {
				                        ""Test Scene"": 
				                        {
				                            ""description"": ""foobar"",
											""selections"": [""Default"",""TestName""]
				                        },
				                    }
				                },
				            }
				        },
				    }
				}
		";

		private string _emptySelectionNamesMasterStoryData = @"
				{
				    ""routes"": 
				    {
				        ""Test Route"": 
				        {
				            ""arcs"": 
				            {
				                ""Test Arc"": 
				                {
				                    ""scenes"": 
				                    {
				                        ""Test Scene"": 
				                        {
				                            ""description"": ""foobar"",
											""selections"": []
				                        },
				                    }
				                },
				            }
				        },
				    }
				}
		";

		private string _previewPathMasterStoryData = @"
				{
				    ""routes"": 
				    {
				        ""Test Route"": 
				        {
				            ""arcs"": 
				            {
				                ""Test Arc"": 
				                {
				                    ""scenes"": 
				                    {
				                        ""Test Scene"": 
				                        {
				                            ""description"": ""foobar"",
											""previewPath"": ""test/path/image.png""
				                        },
				                    }
				                },
				            }
				        },
				    }
				}
		";

		private string _emptyPreviewPathMasterStoryData = @"
				{
				    ""routes"": 
				    {
				        ""Test Route"": 
				        {
				            ""arcs"": 
				            {
				                ""Test Arc"": 
				                {
				                    ""scenes"": 
				                    {
				                        ""Test Scene"": 
				                        {
				                            ""description"": ""foobar"",
											""previewPath"": """"
				                        },
				                    }
				                },
				            }
				        },
				    }
				}
		";


		private string _terminateLevelZeroMasterStoryData = @"
				{
				    ""routes"": 
				    {
				        ""Test Route"": 
				        {
				            ""arcs"": 
				            {
				                ""Test Arc"": 
				                {
				                    ""scenes"": 
				                    {
				                        ""Test Scene"": 
				                        {
				                            ""description"": ""foobar"",
											""terminates"": 0,
				                        },
				                    }
				                },
				            }
				        },
				    }
				}
		";

		private string _terminateLevelTwoMasterStoryData = @"
				{
				    ""routes"": 
				    {
				        ""Test Route"": 
				        {
				            ""arcs"": 
				            {
				                ""Test Arc"": 
				                {
				                    ""scenes"": 
				                    {
				                        ""Test Scene"": 
				                        {
				                            ""description"": ""foobar"",
											""terminates"": 2,
				                        },
				                    }
				                },
				            }
				        },
				    }
				}
		";


		private string _nullMasterStoryData = @"
				{
				    ""routes"": 
				    {
				        ""Test Route"": 
				        {
				            ""arcs"": 
				            {
				                ""Test Arc"": 
				                {
				                    ""scenes"": 
				                    {
				                        ""Test Scene"": 
				                        {
				                            ""description"": null
				                        },
				                    }
				                },
				            }
				        },
				    }
				}
		";

		private string _multipleMasterStoryData = @"
				{
				    ""routes"": 
				    {
				        ""Test Route"": 
				        {
				            ""arcs"": 
				            {
				                ""Test Arc"": 
				                {
				                    ""scenes"": 
				                    {
				                        ""Test Scene"": 
				                        {
				                            ""description"": ""foobar""
				                        },
				                    }
				                },
								""Another Arc"":
								{
									""scenes"": 
				                    {
				                        ""Another Scene"": 
				                        {
				                            ""description"": ""baz""
				                        },
				                    }
								},
				            }
				        },
				    }
				}
		";


    }
}