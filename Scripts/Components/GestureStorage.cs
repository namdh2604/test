using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PDollarGestureRecognizer;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GestureStorage : MonoBehaviour
{
	private static GestureStorage _gestureStorage = null;
	private static Dictionary<string, GestureObject> _gestureDictionary = new Dictionary<string, GestureObject>();
//	private static Dictionary<string, Sprite> _gestureSprite = new Dictionary<string, Sprite>();
//	private static Dictionary<string, Texture2D> _gestureArt = new Dictionary<string, Texture2D>();

	public static GestureStorage GetGestureStorage() 
	{
			return _gestureStorage;
	}

	public static Dictionary<string, GestureObject> GetGestureDictionary() 
	{
			return _gestureDictionary;
	}

	void Awake()
	{
		_gestureStorage = this;
		DontDestroyOnLoad(_gestureStorage);

		List<GestureObject> gestures = new List<GestureObject>();
		
		UnityEngine.Object json = Resources.Load<UnityEngine.Object>("GestureJSON/Gesture_List");
		JObject jsonObject = JObject.Parse(json.ToString());
		
		foreach(var gesture in jsonObject["gestures"])
		{
			GestureObject newGesture = JsonConvert.DeserializeObject<GestureObject>(gesture.ToString());
			gestures.Add(newGesture);
		}
		
		UnityEngine.Object json2 = Resources.Load<UnityEngine.Object>("GestureJSON/Gesture_List_New");
		JObject jsonObject2 = JObject.Parse(json2.ToString());
		
		foreach(var gesture in jsonObject2["gestures"])
		{
			GestureObject newGesture = JsonConvert.DeserializeObject<GestureObject>(gesture.ToString());
			gestures.Add(newGesture);
		}

		foreach(GestureObject gesture in gestures)
		{
			if((!_gestureDictionary.ContainsKey(gesture.gestureName)) && (!_gestureDictionary.ContainsValue(gesture)))
			{
				_gestureDictionary[gesture.gestureName] = gesture;
			}
		}
	}
}
