
using System;
using System.Collections;
using System.Collections.Generic;

namespace Voltage.Common.Android.ExpansionFile
{
	using Voltage.Witches.DI;
	using Voltage.Common.Net;
    using Voltage.Witches.Exceptions;

	using System.IO;
	using Voltage.Common.Logging;

	using Voltage.Common.Unity;
	using UnityEngine;

	using Voltage.Witches.Screens;

	public sealed class RemoteOBBFetcher : IExpansionFileFetcher
	{
		private readonly string _url;
		private readonly string _mainExpansionFilePath = string.Empty;
		private readonly IScreenFactory _screenFactory;

		public RemoteOBBFetcher(string url, IScreenFactory screenFactory) 
		{
			if (string.IsNullOrEmpty(url) || screenFactory == null) 
			{
				throw new ArgumentNullException();
			}

			_url = url;
			_screenFactory = screenFactory;

#if UNITY_ANDROID
			GooglePlayDownloader.Init (string.Empty);
			
			string expansionFilePath = GooglePlayDownloader.GetExpansionFilePath();
			_mainExpansionFilePath = GooglePlayDownloader.GetMainOBBPath(expansionFilePath);
#endif
		}


		public void Fetch(Action<Exception> callback)
		{
			AmbientLogger.Current.Log (string.Format("RemoteOBBFetcher::Fetch..."), LogLevel.INFO);

			if (File.Exists(_mainExpansionFilePath))
			{
				AmbientLogger.Current.Log (string.Format("RemoteOBBFetcher::Fetch >>> Main Expansion Exists!"), LogLevel.INFO);
				callback(null);
			}
			else
			{
				AmbientLogger.Current.Log (string.Format("RemoteOBBFetcher::Fetch >>> Main Expansion Not Found"), LogLevel.INFO);
				UnitySingleton.Instance.StartCoroutine(Download(callback));
			}
		}

		private IEnumerator Download(Action<Exception> onComplete)
		{
			string request = string.Format("{0}/{1}", _url, MainExpansionFile);
			WWW www = new WWW(request);

			AmbientLogger.Current.Log(string.Format("RemoteOBBFetcher::Download >>> {0}", request), LogLevel.INFO);
			yield return www;

			Save(www, onComplete);
		}


		private void Save(WWW www, Action<Exception> onComplete)
		{
            if (!string.IsNullOrEmpty(www.error))
            {
                onComplete(new WitchesException(www.error));
                return;
            }

			Debug.Log(string.Format("RemoteOBBFetcher::Save >>> Saving Download...[{0}bytes]", www.bytesDownloaded));

			File.WriteAllBytes(_mainExpansionFilePath, www.bytes);

			Debug.Log(string.Format("RemoteOBBFetcher::Save >>> Saved: {0} [{1}bytes]", _mainExpansionFilePath, new FileInfo(_mainExpansionFilePath).Length));

			ShowRestartDialogue();
		}

		public void ShowRestartDialogue()
		{
			var dialog = _screenFactory.GetDialog<iGUISmartPrefab_SystemPopupDialog>();
			dialog.SetText("Restart To Complete Installation");
			dialog.Display((choice) =>
			               {  
				if(choice == 0)
				{
					Application.Quit();
				}
			});
		}

		public string MainExpansionFile
		{
			get 
			{ 
				AmbientLogger.Current.Log (string.Format("RemoteOBBFetcher::MainExpansionFile >>> {0}", (!string.IsNullOrEmpty(_mainExpansionFilePath)?_mainExpansionFilePath:"null")), LogLevel.INFO);
				int fileNameIndex = _mainExpansionFilePath.LastIndexOf ('/')+1;
				int length = _mainExpansionFilePath.Length - fileNameIndex;
				return _mainExpansionFilePath.Substring(fileNameIndex, length);
			}
		}

		private IEnumerator DisplayProgress(WWW www)
		{
			while (!www.isDone)
			{
				Debug.Log ("\t" + www.progress);
				yield return new WaitForSeconds(1f);
			}
		}
		
		private void DisplayHeaders(WWW www)
		{
			Debug.Log ("RemoteOBBFetcher::DisplayHeaders >>> Headers...");
			
			IDictionary<string,string> headers = www.responseHeaders;
			foreach (KeyValuePair<string,string> header in headers)
			{
				Debug.Log (string.Format("\t{0}: {1}", header.Key, header.Value));
			}
		}




	}
    
}




