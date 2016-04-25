using UnityEngine;
using UnityEditor;
using System;
using System.Collections;


namespace Voltage.Witches.UI
{

	// TODO: Temporary until I can serialize custom ScrollView inspector:ScrollViewInspector
	[CustomEditor(typeof(ScrollViewEditor))]
	public class ScrollViewEditorInspector : Editor 
	{
		public override void OnInspectorGUI()
		{
			DrawDefaultInspector();

			GUILayout.Space (10f);
			GUILayout.BeginHorizontal ();
				GUILayout.FlexibleSpace ();

					ScrollViewEditor scrollViewEditor = (ScrollViewEditor)target;
					if(GUILayout.Button("Refresh", GUILayout.Width(100f), GUILayout.Height(30f)))
					{
						scrollViewEditor.Refresh();
					EditorUtility.SetDirty(scrollViewEditor);
					}

				GUILayout.FlexibleSpace ();
			GUILayout.EndHorizontal ();
			GUILayout.Space (10f);
		}
	}
}