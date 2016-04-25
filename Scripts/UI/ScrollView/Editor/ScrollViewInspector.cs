using UnityEngine;
using UnityEditor;
using System;
using System.Collections;


namespace Voltage.Witches.UI
{

	[CustomEditor(typeof(ScrollView))]
	public class ScrollViewInspector : Editor 
	{
		private ScrollOptions.Type _type;
		private ScrollOptions.HorizontalDirection _horizontalDirection;
		private ScrollOptions.VerticalDirection _verticalDirection;

		private ScrollView.BackgroundFit _backgroundFitType = ScrollView.BackgroundFit.CONTAINER;		// WIP: maybe not needed
		private Sprite _scrollViewBackground;
		private Sprite _scrollViewMask;

		private Vector2 _cellSize;
		private float _spacing;
		private bool _centerEndElements = true;		// padding

		private bool _snapOnElements = false;
		private bool _loopElements = false;
		private bool _enablePagination = false;
		private int _elementsPerPage = 1;

		private bool _enableScrollbar = false;

		private bool _showReferences = false;



		public override void OnInspectorGUI()
		{
//			ScrollView scrollView = (ScrollView) target;
//
//			HandleScrollType (scrollView);
//
//			HandleCellSettings ();
//			HandleBackgroundSettings ();
//
//			HandleMiscSettings ();
//
//			DisplayRefreshButton (scrollView);


			_showReferences = EditorGUILayout.Foldout (_showReferences, "References");
			if(_showReferences)
			{
				DrawDefaultInspector();
			}

		}

		private void HandleScrollType(ScrollView scrollView)
		{
//			ScrollOptions.Type currentType = _type;
//			ScrollOptions.HorizontalDirection currentHorizDir = _horizontalDirection;
//			ScrollOptions.VerticalDirection currentVerticalDir = _verticalDirection;


			_type = (ScrollOptions.Type)EditorGUILayout.EnumPopup ("Type: ", _type);
			_horizontalDirection = (ScrollOptions.HorizontalDirection)EditorGUILayout.EnumPopup ("Horizontal Direction:", _horizontalDirection);
			_verticalDirection = (ScrollOptions.VerticalDirection)EditorGUILayout.EnumPopup ("Vertical Direction:", _verticalDirection);

//			if(currentType != _type || currentHorizDir != _horizontalDirection || currentVerticalDir != _verticalDirection)
//			{
//				scrollView.SetScrollType(_type, _horizontalDirection, _verticalDirection);
//			}
		}

		private void HandleCellSettings()
		{
			_cellSize = EditorGUILayout.Vector2Field ("Cell Size:", _cellSize);
			_spacing = EditorGUILayout.FloatField ("Spacing:", _spacing);
			_centerEndElements = EditorGUILayout.Toggle ("Center End Elements:", _centerEndElements);
		}

		private void HandleBackgroundSettings()
		{
			_scrollViewBackground = (Sprite)EditorGUILayout.ObjectField ("Background For Scrollview:", _scrollViewBackground, typeof(Sprite), false);
			_scrollViewMask = (Sprite)EditorGUILayout.ObjectField ("Background Mask:", _scrollViewMask, typeof(Sprite), false);
			_backgroundFitType = (ScrollView.BackgroundFit)EditorGUILayout.EnumPopup("Background Fit Type:", _backgroundFitType);
		}

		private void HandleMiscSettings()
		{
			EditorGUILayout.LabelField ("---WIP!---");

			_snapOnElements = EditorGUILayout.Toggle ("Snap on Elements:", _snapOnElements);
			_loopElements = EditorGUILayout.Toggle ("Loop Elements:", _loopElements);
			_enablePagination = EditorGUILayout.Toggle ("Enable Pagination:", _enablePagination);
			_elementsPerPage = EditorGUILayout.IntField ("Elements Per Page:", _elementsPerPage);

			_enableScrollbar = EditorGUILayout.Toggle ("Enable Scrollbar:", _enableScrollbar);
		}

		private void DisplayRefreshButton(ScrollView scrollView)
		{
			GUILayout.Space (10f);
				GUILayout.BeginHorizontal ();
					GUILayout.FlexibleSpace ();
				
					if(GUILayout.Button("Refresh", GUILayout.Width(100f), GUILayout.Height(30f)))
					{
						RefreshScrollView(scrollView);
						EditorUtility.SetDirty(scrollView);
					}
					
					GUILayout.FlexibleSpace ();
				GUILayout.EndHorizontal ();
			GUILayout.Space (10f);
		}

		private void RefreshScrollView(ScrollView scrollView)
		{
			scrollView.SetScrollType (_type, _horizontalDirection, _verticalDirection);
			scrollView.SetCellSize (_cellSize, _spacing);
			scrollView.SetEndElementAlignment (_centerEndElements);
			
			RefreshBackground (scrollView);
			
			scrollView.RefreshScrollDimensions (true);
		}

		private void RefreshBackground(ScrollView scrollView)
		{
			scrollView.SetBackground (_scrollViewBackground, _backgroundFitType);
			scrollView.SetMask (_scrollViewMask);
		}

	}
}