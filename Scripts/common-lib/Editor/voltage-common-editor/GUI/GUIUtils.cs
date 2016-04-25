using System;
using UnityEngine;
using UnityEditor;

namespace Voltage.Common.Utilities
{
	public class GUIUtils
	{
		public static void HorizontalGroup(Action drawMethod)
		{
			EditorGUILayout.BeginHorizontal();
			drawMethod();
			EditorGUILayout.EndHorizontal();
		}

		public static void RightJustify(Action drawMethod)
		{
			HorizontalGroup(delegate() {
				GUILayout.FlexibleSpace();
				drawMethod();
			});
		}

		public static void Center(Action drawMethod)
		{
			HorizontalGroup(delegate() {
				GUILayout.FlexibleSpace();
				drawMethod();
				GUILayout.FlexibleSpace();
			});
		}
	}
}

