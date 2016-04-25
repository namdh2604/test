using UnityEngine;
using System.Collections;

public class AndroidOSDetector {
	public const string AMAZON = "amazon";
	public const string ANDROID = "android";

	public static string DetectAndroidOSType()
	{
		string OS_TYPE;
		AndroidJavaClass buildClass = new AndroidJavaClass("android.os.Build");
		string manufacturer = buildClass.GetStatic<string>("MANUFACTURER");
		string model = buildClass.GetStatic<string>("MODEL");
		Debug.Log(string.Format("MANUFACTURER: {0}\nMODEL: {1}\n", manufacturer, model));
		if (manufacturer.Contains("Amazon")){
			OS_TYPE = AMAZON;
		}
		else{
			OS_TYPE = ANDROID;
		}
		return OS_TYPE;
	}
}
