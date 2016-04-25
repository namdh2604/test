using UnityEngine;
using UnityEditor;
using System;
using System.Collections;

namespace Voltage.Witches.UI
{
//	using UnityEngine.UI;

	[CustomEditor(typeof(UIRibbonView))]
	public class UIRibbonViewEditor : Editor
    {

		public iTween.EaseType _ribbonEaseType = iTween.EaseType.easeOutBounce;
		public iTween.EaseType _fadeEaseType = iTween.EaseType.easeOutQuad;

		public override void OnInspectorGUI()
		{
//			HandleEaseTypes ();

			DrawDefaultInspector();
		}

//		private void HandleEaseTypes()
//		{
//			iTween.EaseType currentRibbonEaseType = _ribbonEaseType;
//			iTween.EaseType currentFadeEaseType = _fadeEaseType;
//			
//			_ribbonEaseType = (iTween.EaseType)EditorGUILayout.EnumPopup ("Ribbon Ease Type", _ribbonEaseType);
//			_fadeEaseType = (iTween.EaseType)EditorGUILayout.EnumPopup ("Fade Ease Type", _fadeEaseType);
//
//			if(currentRibbonEaseType != _ribbonEaseType)
//			{
//
//			}
//
//			if(currentFadeEaseType != _fadeEaseType)
//			{
//
//			}
//
//		}

    }

}


