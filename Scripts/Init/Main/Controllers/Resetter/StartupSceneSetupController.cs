
using System;
using System.Collections.Generic;

namespace Voltage.Witches.DI
{
//	using Voltage.Story.General;
	using Voltage.Common.Startup;
	using Voltage.Witches.Screens;

	using Voltage.Witches.Resetter;
	using Voltage.Witches.Android.DeviceInput;

	using UnityEngine;


    public class StartupSceneSetupController //: IStartupTask
    {

// Could create a separate Android StartupSceneSetupController for this that is bound in the Composition root
// but thought it wasn't worth the effort for this single component
#if UNITY_ANDROID		
		private readonly ScreenNavigationManager _navManager;
		private readonly IScreenFactory _screenFactory;
#endif 

		private readonly IResetter _resetter;
		private readonly float _timeoutDurationInMin;


		public StartupSceneSetupController(ScreenNavigationManager navManager, IScreenFactory screenFactory, IResetter resetter, float timeoutDurationInMin)	
		{
			if(navManager == null || screenFactory == null || resetter == null)
			{
				throw new ArgumentNullException();
			}

#if UNITY_ANDROID
			_navManager = navManager;
			_screenFactory = screenFactory;
#endif 
			_resetter = resetter;
			_timeoutDurationInMin = timeoutDurationInMin;
		}

		public void Execute()	//(Action onComplete)
		{
//			if(IsiOS || IsAndroid)
			{
				GameObject gameObject = CreateGameObject ("ResetMonitor");
				SetupResetMonitor (gameObject);
				SetupDeviceInputMonitor (gameObject);
			}
		}

		private GameObject CreateGameObject(string name)
		{
			return new GameObject (name);
		}

		private void SetupResetMonitor(GameObject gameObject)
		{
			ResetMonitor resetMonitor = gameObject.AddComponent<ResetMonitor> ();
			resetMonitor.Init (_resetter, _timeoutDurationInMin);
		}

		private void SetupDeviceInputMonitor (GameObject gameObject)
		{

#if UNITY_ANDROID
			AndroidBackButtonHandler buttonHandler = gameObject.AddComponent<AndroidBackButtonHandler> ();
			buttonHandler.Init (_navManager, _screenFactory);
#endif 
			
		}
		
    }
    
}




