
using System;
using System.Collections;
using System.Collections.Generic;

namespace Voltage.Common.Android.ExpansionFile
{
	using Voltage.Common.Logging;
	using Voltage.Witches.DI;	// this namespace will change!
	using System.IO;

	using UnityEngine;
	using Voltage.Common.Unity;

	using Voltage.Witches.Screens;
		

	public interface IExpansionFileFetcher
    {
        void Fetch(Action<Exception> callback);
    }


    public class GooglePlayOBBFetcher : IExpansionFileFetcher
    {
		private readonly string _mainExpansionPath = string.Empty;
		private readonly IScreenFactory _screenFactory;

		public GooglePlayOBBFetcher(string publicKey, IScreenFactory screenFactory)
		{
			if(string.IsNullOrEmpty(publicKey) || screenFactory == null)
			{
				throw new ArgumentNullException();
			}
#if UNITY_ANDROID
			GooglePlayDownloader.Init (publicKey);

			string expansionPath = GooglePlayDownloader.GetExpansionFilePath();
			_mainExpansionPath = GooglePlayDownloader.GetMainOBBPath(expansionPath);
#endif
			_screenFactory = screenFactory;
		}

		public void Fetch(Action<Exception> callback)
		{
			AmbientLogger.Current.Log (string.Format("GooglePlayOBBFetcher::Fetch..."), LogLevel.INFO);

//			if(Application.platform == RuntimePlatform.Android && !Debug.isDebugBuild)

			if(!File.Exists(_mainExpansionPath))
			{
				AmbientLogger.Current.Log (string.Format("GooglePlayOBBFetcher::Fetch >>> Main Expansion Not Found"), LogLevel.INFO);

				UnitySingleton.Instance.StartCoroutine(DownloadExpansionFile(_mainExpansionPath, callback));
			}
			else
			{
				AmbientLogger.Current.Log (string.Format("GooglePlayOBBFetcher::Fetch >>> Main Expansion Exists!"), LogLevel.INFO);
				callback(null);
			}
		}


		private IEnumerator DownloadExpansionFile(string expansionPath, Action<Exception> errorHandler)
		{
			AmbientLogger.Current.Log (string.Format("GooglePlayOBBFetcher::DownloadExpansionFile >>> Downloading [{0}] from GooglePlay...", expansionPath), LogLevel.INFO);
#if UNITY_ANDROID			
            // note -- this call will only use the callback as an error handler. The rest of this method determines the completion thru polling
			GooglePlayDownloader.FetchOBB(errorHandler);
#endif
			
			AmbientLogger.Current.Log (string.Format("GooglePlayOBBFetcher::DownloadExpansionFile >>> waiting for download...."), LogLevel.INFO);
			
			while (!File.Exists(expansionPath))	// doesn't really run in a loop during download, as app is put in the background...but needs time before downloader launches
			{
				AmbientLogger.Current.Log (string.Format("\tGooglePlayOBBFetcher::DownloadExpansionFile >>> downloading..."), LogLevel.INFO);
				yield return new WaitForSeconds(0.1f);
			}

			bool fileExists = File.Exists (expansionPath);
			AmbientLogger.Current.Log (string.Format("GooglePlayOBBFetcher::DownloadExpansionFile >>> Expansion Retrieved? [{0}]", fileExists), LogLevel.INFO);
			AmbientLogger.Current.Log (string.Format ("GoooglePlayOBBFetcher::DownloadExpansionFile >>> {0} [{1}bytes]", expansionPath, (fileExists?new FileInfo(expansionPath).Length.ToString():"0")), LogLevel.INFO);

			ShowRestartDialogue ();
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

    }
    
}




