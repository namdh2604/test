
using System;
using System.Collections.Generic;

namespace Voltage.Witches.DI
{


	public class WitchesJsonPackage //: IJsonPackage
	{
//		public Dictionary<string,string> Data = new Dictionary<string, string> ();

//		public string UserID { get; set; }

		public string Master { get; set; }
		public string Avatar { get; set; }
		public string Potions { get; set; }
		public string Recipes { get; set; }
		public string UserData { get; set; }
		public string PlayerState { get; set; }

		public string GameResources { get; set; }

		public string StoryMain { get; set; }
        public IDictionary<string, string> StoryScenes { get; set; }
		
		public bool HasPlayerData
		{
			get { return !string.IsNullOrEmpty (PlayerState) && !string.IsNullOrEmpty (UserData); } 
		}
		
		public bool HasGameData
		{
			get { return !string.IsNullOrEmpty (Master) && !string.IsNullOrEmpty (Avatar) && !string.IsNullOrEmpty(Potions) && !string.IsNullOrEmpty(Recipes); } 
		}
		
		public bool IsComplete
		{
			get { return HasPlayerData && HasGameData && !string.IsNullOrEmpty(StoryMain) && !string.IsNullOrEmpty(GameResources) && StoryScenes != null; }
		}
		
	}
    
}




