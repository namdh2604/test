
using System;
using System.Collections.Generic;

namespace Unit.Witches.Models
{
    using NUnit.Framework;

	using Voltage.Witches.Models;
	using Voltage.Witches.User;

    [TestFixture]
    public class TestPlayerAffinity
    {
        private Player CreatePlayer(PlayerDataStore data)
        {
            return new Player(data, null, null, null, null);
        }

		[Test]
		public void GetAffinityA()
		{
			Dictionary<string,int> affinity = new Dictionary<string,int> { {"a",3}, {"b",4}, {"c",5} };
			PlayerDataStore data = new PlayerDataStore
			{
				firstName = "foo",
				lastName = "bar",
				userID = "0",
				stamina = 10,
				affinities = affinity
			};
            Player player = CreatePlayer(data);

			Assert.That (player.GetAffinity("a"), Is.EqualTo (3));
		}

		[Test]
		public void GetAffinityB()
		{
			Dictionary<string,int> affinity = new Dictionary<string,int> { {"a",3}, {"b",4}, {"c",5} };
			PlayerDataStore data = new PlayerDataStore
			{
				firstName = "foo",
				lastName = "bar",
				userID = "0",
				stamina = 10,
				affinities = affinity
			};
            Player player = CreatePlayer(data);
			
			Assert.That (player.GetAffinity("b"), Is.EqualTo (4));
		}

		[Test]
		public void GetAffinityDoesntExist()
		{
			Dictionary<string,int> affinity = new Dictionary<string,int> { {"a",3}, {"b",4}, {"c",5} };
			PlayerDataStore data = new PlayerDataStore
			{
				firstName = "foo",
				lastName = "bar",
				userID = "0",
				stamina = 10,
				affinities = affinity
			};
            Player player = CreatePlayer(data);
			
			Assert.Throws<KeyNotFoundException> (() => player.GetAffinity("d"));
		}

		[Test]
		public void SetAffinityAdd()
		{
			Dictionary<string,int> affinity = new Dictionary<string,int> { {"a",3}, {"b",4}, {"c",5} };
			PlayerDataStore data = new PlayerDataStore
			{
				firstName = "foo",
				lastName = "bar",
				userID = "0",
				stamina = 10,
				affinities = affinity
			};
            Player player = CreatePlayer(data);
			Assert.That (player.GetAffinity("a"), Is.EqualTo (3));

			player.AddAffinity ("a", 10);
			Assert.That (player.GetAffinity("a"), Is.EqualTo (13));
		}

		[Test]
		public void SetAffinitySubtractGreaterThanZero()
		{
			Dictionary<string,int> affinity = new Dictionary<string,int> { {"a",3}, {"b",4}, {"c",5} };
			PlayerDataStore data = new PlayerDataStore
			{
				firstName = "foo",
				lastName = "bar",
				userID = "0",
				stamina = 10,
				affinities = affinity
			};
            Player player = CreatePlayer(data);
			Assert.That (player.GetAffinity("a"), Is.EqualTo (3));
			
			player.AddAffinity ("a", -2);
			Assert.That (player.GetAffinity("a"), Is.EqualTo (1));
		}

		[Test]
		public void SetAffinitySubtractLessThanZero()
		{
			Dictionary<string,int> affinity = new Dictionary<string,int> { {"a",3}, {"b",4}, {"c",5} };
			PlayerDataStore data = new PlayerDataStore
			{
				firstName = "foo",
				lastName = "bar",
				userID = "0",
				stamina = 10,
				affinities = affinity
			};
            Player player = CreatePlayer(data);
			Assert.That (player.GetAffinity("a"), Is.EqualTo (3));
			
			player.AddAffinity ("a", -10);
			Assert.That (player.GetAffinity("a"), Is.EqualTo (0));
		}

		[Test]
		public void SetAffinityAddNoAffinitySideEffects()
		{
			Dictionary<string,int> affinity = new Dictionary<string,int> { {"a",3}, {"b",4}, {"c",5} };
			PlayerDataStore data = new PlayerDataStore
			{
				firstName = "foo",
				lastName = "bar",
				userID = "0",
				stamina = 10,
				affinities = affinity
			};
            Player player = CreatePlayer(data);
			Assert.That (player.GetAffinity("a"), Is.EqualTo (3));
			Assert.That (player.GetAffinity ("c"), Is.EqualTo (5));
			
			player.AddAffinity ("a", 10);
			Assert.That (player.GetAffinity("a"), Is.EqualTo (13));
			Assert.That (player.GetAffinity ("c"), Is.EqualTo (5));
		}



		[Test]
		public void GetTotalPriorAffinity()
		{
			Dictionary<string,int> affinity = new Dictionary<string,int> { {"a",3}, {"b",4}, {"c",5} };
			PlayerDataStore data = new PlayerDataStore
			{
				firstName = "foo",
				lastName = "bar",
				userID = "0",
				stamina = 10,
				affinities = affinity,
				totalAffinity = 10
			};
            Player player = CreatePlayer(data);
			Assert.That (player.TotalPriorAffinity, Is.EqualTo(10));
		}


		[Test]
		public void GetTotalAffinity()
		{
			Dictionary<string,int> affinity = new Dictionary<string,int> { {"a",3}, {"b",4}, {"c",5} };
			PlayerDataStore data = new PlayerDataStore
			{
				firstName = "foo",
				lastName = "bar",
				userID = "0",
				stamina = 10,
				affinities = affinity,
				totalAffinity = 10
			};
            Player player = CreatePlayer(data);
			Assert.That (player.TotalAffinity, Is.EqualTo(22));
		}

		[Test]
		public void GetTotalAffinityAfterAdd()
		{
			Dictionary<string,int> affinity = new Dictionary<string,int> { {"a",3}, {"b",4}, {"c",5} };
			PlayerDataStore data = new PlayerDataStore
			{
				firstName = "foo",
				lastName = "bar",
				userID = "0",
				stamina = 10,
				affinities = affinity,
				totalAffinity = 10
			};
            Player player = CreatePlayer(data);
			Assert.That (player.TotalAffinity, Is.EqualTo(22));

			player.AddAffinity ("a", 10);
			Assert.That (player.TotalAffinity, Is.EqualTo(32));
		}

    }
}





