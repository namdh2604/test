using UnityEngine;
using System;
using System.Collections.Generic;

namespace Voltage.Witches.DebugTool
{
	using Analytics;

    public class TestFlurry : MonoBehaviour
    {

		private void OnGUI()
		{
			GUILayout.Space (150f);
			if(GUILayout.Button("Send Flurry Dic"))
			{
				string eventName = "TEST_DIC_STARSTONESHOPBUY";
//				string keys = "player_starstones_balance\niap_paid\nplayer_scenes\nstarstones_id\nplayer_id\nplayer_coins_balance";
//				string values = "0\n4.99\n\n123456789abcd\n46660270\n0";
				Dictionary<string,string> parms = new Dictionary<string,string>
				{
					{"iap_paid", "19.99"},
					{"player_starstones_balance", "0"},
					{"player_scenes", "Prologue/Prologue/Mending Luna"},
					{"starstones_id", "a1b2c3d4"},
					{"player_id", "12345"},
					{"player_coins_balance", "0"}
				};

				UnityEngine.Debug.Log ("TESTING FLURRY DIC IOS!");
				FlurryIOS.LogEvent(eventName, parms);
			}
			GUILayout.Space (50f);
			if(GUILayout.Button("Send Flurry String"))
			{
				string eventName = "TEST_STR_STARSTONESHOPBUY";
				string keys = "player_starstones_balance\niap_paid\nplayer_scenes\nstarstones_id\nplayer_id\nplayer_coins_balance";
				string values = "0\n4.99\n\n123456789abcd\n46660270\n0";
				UnityEngine.Debug.Log ("TESTING FLURRY STR IOS!");
				FlurryIOS.LogEvent(eventName, keys, values);
			}
			GUILayout.Space (50f);
			if(GUILayout.Button("Send Flurry String Newline"))
			{
				string eventName = "TEST_STR_NEWLINE_STARSTONESHOPBUY";
				string keys = "\niap_paid\nplayer_scenes\nstarstones_id\nplayer_id\nplayer_coins_balance";
				string values = "\n4.99\n\n123456789abcd\n46660270\n0";
				UnityEngine.Debug.Log ("TESTING FLURRY STR NL IOS!");
				FlurryIOS.LogEvent(eventName, keys, values);
			}

		}
    }

}


