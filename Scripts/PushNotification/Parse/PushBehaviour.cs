using UnityEngine;
using System;
using System.Collections.Generic;

namespace Voltage.Common.PushNotification
{
	using Parse;

    public class PushBehaviour : MonoBehaviour
    {
		public void EnableParse()
		{
			Setup();
		}

		#if UNITY_IOS
		private void Setup()
		{
			Debug.Log ("Parse Push: Setting up iOS");
			UnityEngine.iOS.NotificationServices.RegisterForNotifications(UnityEngine.iOS.NotificationType.Alert |
			                                                        UnityEngine.iOS.NotificationType.Badge |
			                                                        UnityEngine.iOS.NotificationType.Sound);
		}
		#endif

		#if UNITY_ANDROID
		private void Setup()
		{
			Debug.Log ("Parse Push: Setting up Android");
			ParsePush.ParsePushNotificationReceived += (sender, args) => {
				AndroidJavaClass parseUnityHelper = new AndroidJavaClass("com.parse.ParseUnityHelper");
				AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
				AndroidJavaObject currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
					
				// Call default behavior.
				parseUnityHelper.CallStatic("handleParsePushNotificationReceived", currentActivity, args.StringPayload);
			};
    	}
		#endif
	}

}


