
using System;

namespace Unit.Witches.Story.StoryDivisions.Parsers
{
    using NUnit.Framework;
	using Moq;

	using Voltage.Story.StoryDivisions;
	using Voltage.Witches.Story.StoryDivisions.Parsers;
	using Voltage.Witches.Models.MissionRequirements;
	using Voltage.Story.General;
	using Voltage.Story.Expressions;

    [TestFixture]
    public class TestSceneHeaderParser
    {
		private Mock<IMissionRequirementParser> _mockRequirementParser;
		private Mock<IParser<ExpressionState>> _mockExpressionParser;

		[SetUp]
		public void Init()
		{
			_mockRequirementParser = new Mock<IMissionRequirementParser> ();
			_mockExpressionParser = new Mock<IParser<ExpressionState>> ();
		}

		private IParser<SceneHeader> CreateSceneHeaderParser()
		{
			return new SceneHeaderParser (_mockRequirementParser.Object, _mockExpressionParser.Object);
		}


        [Test]
        public void Constructor()
        {
			var parser = CreateSceneHeaderParser ();

			Assert.That (parser, Is.TypeOf<SceneHeaderParser> ());
        }

		[Test]
		public void Parse_ValidSceneJson_ValidSceneHeader()
		{
			var parser = CreateSceneHeaderParser ();

			var header = parser.Parse (_json);

			Assert.That (header, Is.TypeOf<SceneHeader> ());
		}

		[Test]
		public void Parse_ValidSceneJson_ProperScene()
		{
			var parser = CreateSceneHeaderParser ();
			
			var header = parser.Parse (_json);
			
			Assert.That (header.Scene, Is.StringMatching("Test Scene"));
		}

		[Test]
		public void Parse_ValidSceneJson_ProperRoute()
		{
			var parser = CreateSceneHeaderParser ();
			
			var header = parser.Parse (_json);
			
			Assert.That (header.Route, Is.StringMatching("Test Route"));
		}

		[Test]
		public void Parse_ValidSceneJson_ProperArc()
		{
			var parser = CreateSceneHeaderParser ();
			
			var header = parser.Parse (_json);
			
			Assert.That (header.Arc, Is.StringMatching("Test Arc"));
		}

		[Test]
		public void Parse_ValidSceneJson_ProperRequirementCount()
		{
			var parser = CreateSceneHeaderParser ();
			
			var header = parser.Parse (_json);
			
			Assert.That (header.Requirements.Count, Is.EqualTo(1));
		}

		[Ignore("ignore until mock of IMissionRequirement is resolved and Requirements are finalized")]
		[Test]
		public void Parse_ValidSceneJson_ProperRequirementType()
		{
			Mock<Voltage.Witches.Models.MissionRequirements.IMissionRequirement> _mockRequirement = new Mock<Voltage.Witches.Models.MissionRequirements.IMissionRequirement> ();
			_mockRequirementParser.Setup (reqParser => reqParser.Parse (It.IsAny<string> ())).Returns (_mockRequirement.Object);	

			var parser = CreateSceneHeaderParser ();
			
			var header = parser.Parse (_json);
			
			Assert.That (header.Requirements[0], Is.TypeOf<Voltage.Witches.Models.MissionRequirements.IMissionRequirement>());
		}


		[Test]
		public void Parse_NoRequirements_ProperRequirementCount()
		{
			var parser = CreateSceneHeaderParser ();
			
			var header = parser.Parse (_noRequirementsJson);
			
			Assert.That (header.Requirements.Count, Is.EqualTo(0));
		}




		private string _json = @"
			{
				""header"": {
					""reqs"": [
						{
							""_class"": ""Expression"",
							""left"": {
								""_class"": ""Variable"",
								""text"": ""Selections/Nik-Ty Route Main Story/NT Ireland/The Scottish Play/Default""
							},
							""right"": ""\""A\"""",
							""op"": ""=""
						}
					],
					""route"": ""Test Route"",
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
							""background"": ""Austria Ball Dancers Close"",
							""speechBox"": ""Dialogue Left"",
							""music"": ""Action""
						}
					}
				]
			}				
		";

		private string _noRequirementsJson = @"
			{
				""header"": {
					""reqs"": [
					],
					""route"": ""Test Route"",
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
							""background"": ""Austria Ball Dancers Close"",
							""speechBox"": ""Dialogue Left"",
							""music"": ""Action""
						}
					}
				]
			}				
		";

    }
}





//			private string _json = @"
//				{
//					""header"": {
//						""reqs"": [
//							{
//								""_class"": ""Expression"",
//								""left"": ""2"",
//								""right"": ""1"",
//								""op"": "">""
//							},
//							{
//								""_class"": ""Expression"",
//								""left"": {
//									""_class"": ""Variable"",
//									""text"": ""MC/First""
//								},
//								""right"": {
//									""_class"": ""Variable"",
//									""text"": ""MC/Last""
//								},
//								""op"": ""!=""
//							},
//							{
//								""_class"": ""Expression"",
//								""left"": ""\""foobar\"""",
//								""right"": ""\""foobar\"""",
//								""op"": ""=""
//							}
//						],
//						""route"": ""Prologue"",
//						""arc"": ""Prologue"",
//						""scene"": ""A Fallen Enemy""
//					},
//					""data"": [
//						{
//							""_class"": ""Dialogue"",
//							""speaker"": null,
//							""text"": [
//								""Hello World""
//							],
//							""data"": {
//								""background"": ""Austria Ball Dancers Close"",
//								""speechBox"": ""Dialogue Left"",
//								""music"": ""Action""
//							}
//						}
//					]
//				}
//		";


