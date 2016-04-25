using System;
using System.Collections;
using System.Collections.Generic;

public class GestureObject 
{
	public string gestureName = string.Empty;
	public int strokeCount = 0;
	public bool isTargetGesture = false;
	public string artFile = string.Empty;
	public List<StrokeObject> strokes = new List<StrokeObject>();

	public class StrokeObject
	{
		public int strokeID = 0;
		public List<int> gridIDs = new List<int>();
	}
}