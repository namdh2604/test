using UnityEngine;
using System;
using System.Collections;

namespace Voltage.Witches.UI
{
	// TODO: Temporary until I can serialize custom ScrollView inspector:ScrollViewInspector
	[RequireComponent(typeof(ScrollView))]
	public class ScrollViewEditor : MonoBehaviour 
	{

		public ScrollOptions.Type _type;
		public ScrollOptions.HorizontalDirection _horizontalDirection;
		public ScrollOptions.VerticalDirection _verticalDirection;

		public Vector2 _cellSize;
		public float _spacing;
		public bool _centerEndElements = true;		// padding

		public Sprite _scrollViewBackground;
		public Sprite _scrollViewMask;
		public ScrollView.BackgroundFit _backgroundFitType = ScrollView.BackgroundFit.CONTAINER;		// WIP: maybe not needed


		public bool _snapOnElements = false;
		public bool _loopElements = false;
		public bool _enablePagination = false;
		public int _elementsPerPage = 1;

		public bool _enableScrollbar = false;


		[SerializeField]
		private ScrollView _scrollView;

		private void Start()	// maybe Awake()?
		{
			Refresh ();
		}

		private void GetScrollView()
		{
			_scrollView = GetComponent<ScrollView> () as ScrollView;
			
			if(_scrollView == null)
			{
				throw new NullReferenceException();
			}
		}

		private void RefreshScrollView()
		{
			_scrollView.SetScrollType (_type, _horizontalDirection, _verticalDirection);
			_scrollView.SetCellSize (_cellSize, _spacing);
			_scrollView.SetEndElementAlignment (_centerEndElements);

			RefreshBackground ();

			_scrollView.RefreshScrollDimensions (true);
		}

		private void RefreshBackground()
		{
			_scrollView.SetBackground (_scrollViewBackground, _backgroundFitType);
			_scrollView.SetMask (_scrollViewMask);
		}


		public void Refresh()
		{
			GetScrollView ();
			RefreshScrollView ();
		}
	}
}