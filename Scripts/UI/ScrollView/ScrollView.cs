using UnityEngine;
using System;
using System.Collections.Generic;

namespace Voltage.Witches.UI
{
	using UnityEngine.UI;

//	[Serializable]
	[RequireComponent(typeof(ScrollRect))]
	[RequireComponent(typeof(CanvasGroup))]
	public class ScrollView : MonoBehaviour, IScrollView<GameObject>
	{
		[SerializeField]
		private RectTransform _scrollViewContainer;
		[SerializeField]
		private ScrollRect _scrollRect;

		[SerializeField]
		private Image _backgroundImage;
		[SerializeField]
		private Mask _viewportMask;
		[SerializeField]
		private Image _maskImage;

		[SerializeField]
		private GridLayoutGroup _contentLayoutGrp;

		[SerializeField]
		private CanvasGroup _canvasGrp;


		public enum BackgroundFit
		{
			CONTAINER,
			SPRITE,
			SCROLLWIDTH
		}

		[ExecuteInEditMode]
		private void Awake()
		{
			if(_scrollViewContainer == null)
			{
				throw new NullReferenceException();
			}
			
			if(_scrollRect == null || _scrollRect.content == null || _scrollRect.horizontalScrollbar == null || _scrollRect.verticalScrollbar == null)
			{
				throw new NullReferenceException();
			}
			
			if(_backgroundImage == null || _viewportMask == null || _maskImage == null)
			{
				throw new NullReferenceException();
			}
			
			if(_contentLayoutGrp == null) 
			{
				throw new NullReferenceException();
			}

			if(_canvasGrp == null)
			{
				throw new NullReferenceException();
			}

			RefreshScrollDimensions ();
		}

        public void MakePassive(bool value)
        {
            _scrollRect.enabled = !value;
        }

		public void RefreshScrollDimensions(bool resetPosition=false)	// TODO: need to handle vertical scrollviews too!
		{
			int elementCount = _scrollRect.content.childCount;
			float elementWidth = _contentLayoutGrp.cellSize.x;
//			float elementHeight = _contentLayoutGrp.cellSize.y;
			float elementSpacing = _contentLayoutGrp.spacing.x;
			float elementPadding = _contentLayoutGrp.padding.left;

			float scrollWidth = (elementCount * elementWidth) + ((elementCount-1) * elementSpacing) + (2 * elementPadding);		
			float scrollHeight = _contentLayoutGrp.cellSize.y;

			_scrollRect.content.sizeDelta = new Vector2 (scrollWidth, scrollHeight);

			if(resetPosition)
			{
				_scrollRect.content.anchoredPosition = Vector2.zero;
			}
		}

		public void SetScrollType(ScrollOptions.Type type, ScrollOptions.HorizontalDirection horizontalDir, ScrollOptions.VerticalDirection verticalDir)
		{
			SetHorizontal (horizontalDir);

			// TODO: Vertical
		}

		private void SetHorizontal(ScrollOptions.HorizontalDirection dir)
		{
			switch(dir)
			{
				case ScrollOptions.HorizontalDirection.LEFT_TO_RIGHT:	
					ConfigureContentAnchoring(new HorizontalLeftToRightConfig()); break;

				case ScrollOptions.HorizontalDirection.RIGHT_TO_LEFT:	
				default:
					ConfigureContentAnchoring(new HorizontalRightToLeftConfig()); break;
			}
		}

		private void ConfigureContentAnchoring(IAnchorConfig config)
		{	 
			_scrollRect.content.anchorMax = config.AnchorMax;
			_scrollRect.content.anchorMin = config.AnchorMin;
			_scrollRect.content.pivot = config.Pivot;

			_contentLayoutGrp.childAlignment = config.ChildAlignment;
			_contentLayoutGrp.constraint = config.Constraint;
			_contentLayoutGrp.constraintCount = config.ConstraintCount;
			_contentLayoutGrp.startAxis = config.StartAxis;
			_contentLayoutGrp.startCorner = config.StartCorner;

			_scrollRect.content.anchoredPosition = Vector2.zero;
		}

		private void SetVertical(ScrollOptions.VerticalDirection dir)
		{
			throw new NotImplementedException ();
		}


		public void SetCellSize(Vector2 size, float spacing=0f)
		{
			_contentLayoutGrp.cellSize = size ;
			_contentLayoutGrp.spacing = new Vector2 (spacing, spacing);
		}

		public void SetEndElementAlignment(bool center)
		{
			if(center)
			{
				float viewportWidth = _scrollViewContainer.rect.width;			// _scrollViewContainer.sizeDelta.x;
				float elementWidth = _contentLayoutGrp.cellSize.x;

//				if(viewportWidth > elementWidth)
				{
					int padding = (int)((viewportWidth - elementWidth) / 2f);
					
					_contentLayoutGrp.padding = new RectOffset (padding, padding, padding, padding);
//					return;
				}
			}
			else
			{
				_contentLayoutGrp.padding = new RectOffset(0, 0, 0, 0);
			}
		}



		public void SetBackground (Sprite image, BackgroundFit fitType=BackgroundFit.CONTAINER)
		{
			if(image != null)
			{
				_backgroundImage.sprite = image;
				_backgroundImage.enabled = true;
			}
			else
			{
				_backgroundImage.sprite = null;
				_backgroundImage.enabled= false;

				SetMask(null);
			}

			FitBackground (fitType);
		}

		private void FitBackground(BackgroundFit type)		// maybe not necessary?
		{
			switch(type)
			{
				case BackgroundFit.SCROLLWIDTH:		// TODO
				case BackgroundFit.SPRITE:			// TODO
				case BackgroundFit.CONTAINER:
				default:
					SetRectTransform(_backgroundImage.rectTransform, new RectTransformConfigs.Fill());
					break;
			}
		}


		private void SetRectTransform (RectTransform rectTransform, RectTransformConfigs.IConfigs config)
		{
			rectTransform.anchoredPosition = config.LeftTop;
			rectTransform.sizeDelta = config.RightBottom;
			rectTransform.anchorMin = config.AnchorMin;
			rectTransform.anchorMax = config.AnchorMax;
			rectTransform.pivot = config.Pivot;
			rectTransform.rotation = Quaternion.Euler (config.RotationEuler);
			rectTransform.localScale = config.Scale;
		}

		public void SetMask(Sprite mask)
		{
			if(mask != null)
			{
				_maskImage.sprite = mask;
				_maskImage.enabled = true;
				_viewportMask.enabled = true;
			}
			else
			{
				_maskImage.sprite = null;
				_maskImage.enabled = false;
				_viewportMask.enabled = false;
			}
		}


		public GameObject GetElementAt (int index)
		{
			return _scrollRect.content.GetChild (index).gameObject;
		}


		public GameObject GoToElementAt (int index)
		{
//			Transform target = _scrollRect.content.GetChild (index);
//			_scrollRect.content.anchoredPosition = //target.GetComponent<RectTransform> ();
//			return target.gameObject;

			throw new NotImplementedException ();
		}

		public GameObject GoToElement(GameObject element)
		{
			throw new NotImplementedException ();
		}


		public void AddElement(GameObject element, bool refresh=true)
		{
			element.transform.SetParent (_scrollRect.content);			// can throw NRE when element is null
			element.transform.localScale = Vector3.one;

			if(refresh)
			{
				RefreshScrollDimensions ();
			}
		}

		public void InsertElementAt (int index, GameObject element)		// if index goes beyond element count, element is added at end...when below, then to the start
		{
			AddElement (element, false);
			element.transform.SetSiblingIndex (index);

			RefreshScrollDimensions ();
		}

		public void Populate(IList<GameObject> elements)
		{
			// clear first?

			foreach(GameObject element in elements)
			{
				AddElement(element, false);
			}

			RefreshScrollDimensions ();
		}


		public void RemoveElement(GameObject element, bool refresh=true)	
		{
			element.transform.SetParent(null);			// element is not guaranteed to be destroyed before RefreshScrollDimension is called
			Destroy (element);												

			if(refresh)
			{
				RefreshScrollDimensions ();
			}
		}

		public void RemoveElementAt (int index)
		{
			Transform element = _scrollRect.content.GetChild (index);		// can throw out of bounds exception when no elements

			RemoveElement (element.gameObject);
		}

		public void RemoveFirst()
		{
			RemoveElementAt (0);
		}

		public void RemoveLast()
		{
			RemoveElementAt (_scrollRect.content.childCount - 1);
		}

		public void Clear()
		{
			List<GameObject> elementList = new List<GameObject> ();

			foreach(Transform element in _scrollRect.content)
			{
				elementList.Add (element.gameObject);
			}

			foreach(GameObject element in elementList)
			{
				RemoveElement(element, false);
			}

			RefreshScrollDimensions (true);
		}


		public void EnableScrolling(bool enable)
		{
			_scrollRect.horizontal = enable;		// TODO: handle vertical
		}

		public void EnableScrollView(bool enable)
		{
			EnableScrolling (enable);
			_canvasGrp.interactable = enable;
		}


		private interface IAnchorConfig
		{
			Vector2 AnchorMax { get; }
			Vector2 AnchorMin { get; }
			Vector2 Pivot { get; }

			TextAnchor ChildAlignment { get; }
			GridLayoutGroup.Constraint Constraint { get; }
			int ConstraintCount { get; }
			GridLayoutGroup.Axis StartAxis { get; }

			GridLayoutGroup.Corner StartCorner { get; }
		}

		private struct HorizontalRightToLeftConfig : IAnchorConfig
		{
			public Vector2 AnchorMax { get { return new Vector2 (1f, 0.5f); } }
			public Vector2 AnchorMin { get { return new Vector2 (1f, 0.5f); } }
			public Vector2 Pivot { get { return new Vector2 (1f, 0.5f); } }

			public TextAnchor ChildAlignment { get { return TextAnchor.MiddleRight; } }
			public GridLayoutGroup.Constraint Constraint { get { return GridLayoutGroup.Constraint.FixedRowCount; } }
			public int ConstraintCount { get { return 1; } }
			public GridLayoutGroup.Axis StartAxis { get { return GridLayoutGroup.Axis.Horizontal; } }
			public GridLayoutGroup.Corner StartCorner { get { return GridLayoutGroup.Corner.UpperRight; } }	
		}

		private struct HorizontalLeftToRightConfig : IAnchorConfig
		{
			public Vector2 AnchorMax { get { return new Vector2 (0f, 0.5f); } }
			public Vector2 AnchorMin { get { return new Vector2 (0f, 0.5f); } }
			public Vector2 Pivot { get { return new Vector2 (0f, 0.5f); } }
			
			public TextAnchor ChildAlignment { get { return TextAnchor.MiddleLeft; } }
			public GridLayoutGroup.Constraint Constraint { get { return GridLayoutGroup.Constraint.FixedRowCount; } }
			public int ConstraintCount { get { return 1; } }
			public GridLayoutGroup.Axis StartAxis { get { return GridLayoutGroup.Axis.Horizontal; } }
			public GridLayoutGroup.Corner StartCorner { get { return GridLayoutGroup.Corner.UpperLeft; } }	
		}



	}
}
