
using System;
using System.Collections.Generic;

namespace Unit.Witches.Models
{
    using NUnit.Framework;

	using Voltage.Witches.Models;
	using Voltage.Witches.User;

    [TestFixture]
    public class TestNPC
    {
        [Test]
        public void TestLastName()
        {
			PlayerDataStore data = new PlayerDataStore
			{
				firstName = "hello",
				lastName = "world"
			};
			Player player = new Player (data, null, null, null, null);
			List<NPCModel> npcs = new List<NPCModel> { new NPCModel("0", "foo", "bar"), new NPCFamily("1", "hello", player) };

			Assert.That (npcs [0].LastName, Is.StringMatching ("bar"));
			Assert.That (npcs [1].LastName, Is.StringMatching ("world"));
        }
    }
}