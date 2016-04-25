using System;
using System.Collections;
using System.Collections.Generic;

using Voltage.Witches.Models;

namespace Voltage.Witches.Configuration
{
	public class GamePropertiesConfiguration
	{
		public string Default_Affinity { get; set; }
		public int Default_Free_Currency { get; set; }
		public string Default_User_Book { get; set; }
		public float Default_Ticket_Refresh_Rate { get; set; }
		public int Default_Ticket { get; set; }
		public int Default_Premium_Currency { get ; set; }
		public int Default_Focus { get; set; }
		public int Default_Closet { get; set; }
		public int Default_Preppy { get; set; }
		public int Default_Funky { get; set; }
		public int Default_Rebel { get; set; }
		public float Default_Focus_Refresh_Rate { get; set; }
		public int Cumulative_Max { get; set; }
		public int Witches_Version { get; set; }
		public Dictionary<string,float> Mini_Game_Speed_And_Zones { get; set; }
		public Dictionary<string,Dictionary<string,float>> Mini_Game_Difficulty { get; set; }
		public Dictionary<string,int> Mini_Game_Scoring { get; set; }
		public List<InventoryData> Default_Inventory { get; set; }
        public int Affinity_Per_Premium { get; set; }

		public GamePropertiesConfiguration()
		{
			Mini_Game_Speed_And_Zones = new Dictionary<string,float> ();
			Mini_Game_Difficulty = new Dictionary<string,Dictionary<string,float>>();
			Mini_Game_Scoring = new Dictionary<string,int>();
			Default_Inventory = new List<InventoryData>();
            Affinity_Per_Premium = 400; // Not expected to be used, just a default value to prevent server crashes
		}
	}
}