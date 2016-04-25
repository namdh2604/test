
using System;

namespace Unit.Witches.User
{
    using NUnit.Framework;
	using Voltage.Witches.User;

	using Voltage.Witches.DI;

    [TestFixture]
	public class TestServerPlayerDataParser
    {
		[Ignore("PlayerDataStore now includes Outfit, which currently has a dependency on Unity")]
        [Test]
        public void Parse_Json_ValidType()
        {
			ServerPlayerDataParser parser = new ServerPlayerDataParser ();

			var data = parser.Parse (json);

			Assert.That (data, Is.TypeOf<PlayerDataStore> ());
        }

		[Ignore("PlayerDataStore now includes Outfit, which currently has a dependency on Unity")]
		[Test]
		public void Parse_Json_CountOfCharacterAffinities()
		{
			ServerPlayerDataParser parser = new ServerPlayerDataParser ();
			
			var data = parser.Parse (json);
			
			Assert.That (data.affinities.Count, Is.EqualTo(5));
		}

		[Ignore("PlayerDataStore now includes Outfit, which currently has a dependency on Unity")]
		[Test]
		public void Parse_Json_HasBookID()
		{
			ServerPlayerDataParser parser = new ServerPlayerDataParser ();
			
			var data = parser.Parse (json);

			Assert.That (data.books[0].Id, Is.StringMatching("54da8a3e6f983f60ee01f778"));
		}


		private string json = @"
			{
			    ""last_name"": """",
			    ""first_name"": """",
			    ""closet_space"": 30,
			    ""stamina_potion"": 0,
			    ""avatar_items"": [
			        
			        ],
			    ""character_affinities"": {
				        ""A"": 0,
						""M"": 0,
						""N"": 0,
						""R"": 0,
						""T"": 0,
		        },
			    ""node_id"": """",
			    ""howtos_scene_id"": """",
			    ""premium_currency"": 0,
			    ""phone_id"": ""78590777"",
			    ""focus"": 5,
			    ""stamina"": 5,
			    ""currency"": 0,
			    ""books"": [
			        {
			        ""recipes"": [
			            
			            ],
			        ""id"": ""54da8a3e6f983f60ee01f778"",
			        ""is_complete"": false
			        }
			        ],
			    ""total_affinity"": 0,
			    ""inventory"": {},
			    ""scene_id"": """",
			    ""mail_badge"": 0,
			    ""alignment"": 0
			}
		";
    }
}