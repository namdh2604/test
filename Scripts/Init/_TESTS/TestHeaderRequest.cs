using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Test.Header
{

    public class TestHeaderRequest : MonoBehaviour
    {

		private void OnGUI()
		{
			if(GUILayout.Button("Test"))
			{
				TestAcceptHeaderGetRequest();
			}
			if(GUILayout.Button("Test Normal"))
			{
				TestDefaultGetRequest();
			}
		}



		private void TestDefaultGetRequest()
		{
//			WWWForm form = new WWWForm ();
//			form.AddField ("foo", "bar");
//			form.AddField ("build", "1.0.0_d");
//			form.AddField ("device", "Android");
		

//			WWW request = new WWW ("http://httpbin.org/headers", form.data);
			WWW request = new WWW ("http://httpbin.org/headers");
			
			StartCoroutine (TestRequestRoutine (request));
		}


		private void TestAcceptHeaderGetRequest()
		{
//			WWWForm form = new WWWForm ();
//			form.AddField ("foo", "bar");
//			form.AddField ("build", "1.0.0_d");
//			form.AddField ("device", "Android");
			
			Dictionary<string,string> header = new Dictionary<string, string>
			{
				{"Accept", "application/json; version=2"}
			};
		
//			WWW request = new WWW ("http://httpbin.org/headers", form.data, header);
			WWW request = new WWW ("http://httpbin.org/headers", null, header);

			StartCoroutine (TestRequestRoutine (request));
		}

		private IEnumerator TestRequestRoutine(WWW www)
		{
			Debug.Log ("Request! " + www.url);

			yield return www;

			Debug.Log ("Response: " + www.text);
		}




    }

}


