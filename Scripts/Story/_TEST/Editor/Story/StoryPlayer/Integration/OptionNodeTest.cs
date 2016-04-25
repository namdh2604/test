
using System;
using System.Collections.Generic;

namespace Integration.Story.Models.Nodes
{
    using NUnit.Framework;
	using Newtonsoft.Json.Linq;

	using Voltage.Story.Models.Nodes;
	using Voltage.Story.Models.Nodes.ID;

    [TestFixture]
    public class OptionNodeTest
    {
        [Test]
        public void CreateOptionNode()
        {
			string json = @"
				{
					""_class"": ""Option"",
					""text"": [
						""Option Text""
					],
					""effects"": {
						""L"": 2
					},
					""children"": [
						{
							""_class"": ""Dialogue"",
							""speaker"": null,
							""text"": [
								""side of the light""
							],
							""data"": { ""background"" : ""Austria Hotel Day"" }
						}
					]
				}
			";

			JToken token = JToken.Parse (json);

			OptionNode optionNode = new OptionNode (token, null, new SimpleNumIncrementIDGenerator ());
			Assert.That (optionNode, Is.Not.Null);
        }


		[Test]
		public void OptionNodeConstructorTextAssignment()
		{
			string json = @"
				{
					""_class"": ""Option"",
					""text"": [
						""Option Text""
					],
					""effects"": {
						""L"": 2
					},
					""children"": [
						{
							""_class"": ""Dialogue"",
							""speaker"": null,
							""text"": [
								""side of the light""
							],
							""data"": { ""background"": ""Austria Hotel Day"" }
						}
					]
				}
			";
			
			JToken token = JToken.Parse (json);
			
			OptionNode optionNode = new OptionNode (token, null, new SimpleNumIncrementIDGenerator ());
			Assert.That (optionNode.Text, Is.StringMatching ("Option Text"));
		}


		[Test]
		public void OptionNodeConstructorEffectsAssignment()
		{
			string json = @"
				{
					""_class"": ""Option"",
					""text"": [
						""Option Text""
					],
					""effects"": {
						""L"": 2
					},
					""children"": [
						{
							""_class"": ""Dialogue"",
							""speaker"": null,
							""text"": [
								""side of the light""
							],
							""data"": { ""background"": ""Austria Hotel Day"" }
						}
					]
				}
			";
			JToken token = JToken.Parse (json);
			OptionNode optionNode = new OptionNode (token, null, new SimpleNumIncrementIDGenerator ());

			Dictionary<string,int> compare = new Dictionary<string,int> { {"L", 2} };
			Assert.That (optionNode.Effects, Is.EqualTo (compare));
		}

    }
}