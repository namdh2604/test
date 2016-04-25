using UnityEngine;
using System;
using System.Collections.Generic;

namespace Test.Net
{
	using Voltage.Common.Logging;

	using Voltage.Common.Net;
	using Voltage.Witches.Net;

    public class TestNetworkController : MonoBehaviour
    {

		private void OnGUI()
		{
			GUILayout.Space (50f);
			GUILayout.BeginHorizontal ();
				if(GUILayout.Button("Send Success"))
				{
					Send ("http://curses.en.my-romance.com/witches/get_environment/1", new Dictionary<string,string>{ {"build","1.0.0_d"}, {"device", "Android"}});
				}
				GUILayout.Space (10f);
				if(GUILayout.Button("Send Failed"))
				{
					Send ("http://curses.en.my-romance.com/witches/get_environment/1", new Dictionary<string,string>{ {"build","0.0.0"}, {"device", "FAIL"}});
				}
				GUILayout.Space (10f);
				if(GUILayout.Button("Send Timeout"))
				{
					Get ("http://172.16.100.205/assets/", new Dictionary<string,string> {{"assets", "main.3.com.voltage.curse.en.obb"}}, 1);	
				}
				GUILayout.Space (10f);
				if(GUILayout.Button("Send Download"))
				{
					Get ("http://172.16.100.205/assets/", new Dictionary<string,string> {{"assets", "test.txt"}});	
				}
				GUILayout.Space (10f);
				if(GUILayout.Button("Send BadRequest"))
				{
					Get ("http://curses.en.my-romance.com/witches/get_bad_request/1");
				}
			GUILayout.EndHorizontal ();
		}

		private INetworkTimeoutController<WitchesRequestResponse> _networkController;

		private void Awake()
		{
			AmbientLogger.Current = new UnityLogger ();
			_networkController = new WitchesNetworkRetryController (new WitchesNetworkResponseController ());
//			_networkController = new WitchesBaseNetworkController(new WitchesNetworkRetryController (new WitchesNetworkConnectionCheckController ( new WitchesNetworkLoggingController(AmbientLogger.Current, new WitchesNetworkResponseController ()))));
		}

		private void Get (string url, IDictionary<string,string> parms=null, int timeout=30)
		{
			_networkController.Receive(url, (parms!=null?parms:new Dictionary<string,string>()), OnSuccess, OnFail, timeout);
		}

		private void Send (string url, IDictionary<string,string> parms=null)
		{
			_networkController.Send(url, (parms!=null?parms:new Dictionary<string,string>()), OnSuccess, OnFail);
		}


		private void OnSuccess(INetworkPayload response)
		{
			Debug.Log (string.Format("Test Successful <{0}>!\n{1}", response.Request.URL, response.Text));
		}

		private void OnFail(WitchesRequestResponse response)
		{
			Debug.Log (string.Format("Test Failed <{0}> {1}!\n{2}", response.Request.URL, response.Status.ToString(), response.Text));
		}
    }

}


