using System;
using UnityEngine;
using iGUI;
using System.Collections.Generic;

using Voltage.Common.DebugTool.Timer;

public class ElementScrollView : MonoBehaviour
{
    public float _paddingPercent = 0.0f;

    private iGUIScrollView _scrollView;

    private float _containerWidth;
    private float _containerHeight;
    private List<iGUIElement> _elements;

//	public List<iGUIElement> Elements { get { return _elements; } }


	private Rect _elementAbsoluteRect;


    private iGUIElement _paddingElement;

    protected void Awake()
    {
        _scrollView = GetComponent<iGUIScrollView>();
    }

    public int ItemCount { get { return _elements.Count; } }

    protected void Start()
    {
        _containerWidth = _scrollView.getAbsoluteRect().width;
        _containerHeight = _scrollView.getAbsoluteRect().height;


        _elements = new List<iGUIElement>();

        var items = _scrollView.items;

        for (int i = 0; i < items.Length; ++i)
        {
            var element = items[i];
            if (!IsPaddingElement(element))
            {
				//HACK This was to resolve an issue wherein the scenecards scaled bizarrely after a second card was loaded
				//I'm keeping the relative positioning, but using the absolute values for the widht and height to mainatina a uniform scale
				var relative = element.positionAndSize;
				var absolute = element.getAbsoluteRect();
				_elementAbsoluteRect = new Rect(relative.x,relative.y,absolute.width,absolute.height);

				element.setPositionAndSize(_elementAbsoluteRect);

                _elements.Add(element);
            }
        }

        if (_paddingElement == null)
        {
            InsertPaddingAlignment();
        }

        RemoveScrollBarArea();

        foreach (var element in _elements)
        {
            element.refreshRect();
        }

        UpdateView();
    }


	
    private const string PADDING_NAME = "Padding";
    private iGUIElement CreatePaddingElement(float width)
    {
        float paddingHeight = _containerHeight;

        iGUIImage paddingImage = _scrollView.addElement<iGUIImage>();
        paddingImage.setOrder(-1);
        paddingImage.setPositionAndSize(new Rect(0.0f, 0.0f, width, paddingHeight));
        paddingImage.name = PADDING_NAME;
        paddingImage.variableName = PADDING_NAME;

        return paddingImage;
    }

    private bool IsPaddingElement(iGUIElement element)
    {
        return (element.name == PADDING_NAME);
    }

    public void Refresh()
    {
        _elements = new List<iGUIElement>();

        var items = _scrollView.items;

        for (int i = 0; i < items.Length; ++i)
        {
            iGUIElement element = items[i];
            if (!IsPaddingElement(element))
            {
                _elements.Add(element);
            }
        }

        if (_paddingElement == null)
        {
            InsertPaddingAlignment();
        }

        UpdateView();

        _scrollView.scrollPosition = Vector2.zero;
    }


    public T AddElement<T>() where T : iGUIElement
    {
        var element = _scrollView.addElement<T>();
        if (element.getAbsoluteRect().width > _containerWidth)
        {
            GameObject.Destroy(element.gameObject);
            throw new Exception("Cannot add an element greater than the size of the view area");
        }

        _elements.Add(element);

        Refresh();

        return element;
    }

    public T AddElement<T>(iGUIElement element) where T : iGUIElement
    {
        if (element.getAbsoluteRect().width > _containerWidth)
        {
            GameObject.Destroy(element.gameObject);
            throw new Exception("Cannot add an element greater than the size of the view area");
        }

        var newElement = _scrollView.addElement<T>();
//        newElement.setPositionAndSize(element.positionAndSize);
//        newElement.setAspectRatio(element.elementAspectRatio);
		//HACK This was to resolve an issue wherein the scenecards scaled bizarrely after a second card was loaded
		//I'm keeping the relative positioning, but using the absolute values for the widht and height to mainatina a uniform scale
		newElement.setPositionAndSize(_elementAbsoluteRect);
		newElement.setAspectRatio(element.elementAspectRatio);

        _elements.Add(newElement);

        Refresh();

        return newElement;
    }

    public void AddSmartObject(string smartObjectName)
    {
        var element = _scrollView.addSmartObject(smartObjectName);
        if (element.getAbsoluteRect().width > _containerWidth)
        {
            GameObject.Destroy(element.gameObject);
            throw new Exception("Cannot add an element greater than the size of the view area");
        }

        _elements.Add(element);

        Refresh();
    }

    public void RemoveElement(iGUIElement element)
    {
        _scrollView.removeElement(element);
    }



	public iGUIElement GetItem(int index)
	{
		return _scrollView.allItems[index];
	}

	public void Clear()
	{
		_scrollView.removeAll ();
	}

	public void RemoveAll(Predicate<iGUIElement> predicate)	// basically does what iGUIScrollView.removeAll does, but allows for predicate
	{
//		_elements.RemoveAll (predicate);
		foreach(iGUIElement element in _scrollView.allItems)	
		{
			if(predicate(element))
			{
				DestroyImmediate(element.gameObject);	// will this leave a null reference in the _scrollView.allItems array?
			}
		}

		_scrollView.refreshRect ();

	}


    public iGUIElement this[int index]
    {
        get
        {
            if (index >= ItemCount)
            {
                throw new IndexOutOfRangeException();
            }
            return _elements[index];
        }
    }






    public void MoveToElement(int index)
    {
        if (index >= _elements.Count)
        {
            throw new Exception("invalid element index: " + index);
        }

		int totalOffset = 0;
		foreach(var element in _elements)
		{
			int absoluteWidth = Mathf.FloorToInt(element.getAbsoluteRectNonScaled().width);
			totalOffset += absoluteWidth;
		}

        _scrollView.setHorizontalScrollPosition(Convert.ToSingle(totalOffset));
    }

    private void RemoveScrollBarArea()
    {
        float initialWidth = _scrollView.areaWidth;
        if (_scrollView.isAreaWidthRelative)
        {
            initialWidth *= _containerWidth;
        }

        float initialHeight = _containerHeight;

        UpdateAreaSize(initialWidth, initialHeight);
    }



	private void UpdateView()
	{
		if (_elements.Count == 0)
		{
			// size the scrollable area to the size of the container itself
			UpdateAreaSize(_containerWidth, _containerHeight);
			return;
		}
	
		int totalElementWidth = 0;
		foreach(var element in _elements)
		{
			int absoluteWidth = Mathf.FloorToInt(element.getAbsoluteRectNonScaled().width);
			totalElementWidth += absoluteWidth;
		}

		// the totalAreaLength must be able to left-align the last element
		// first, get the length of the last element
		float lastElementWidth = Mathf.Floor(_elements[_elements.Count - 1].getAbsoluteRectNonScaled().width);
		float totalWidth = totalElementWidth - lastElementWidth + _containerWidth;

//		Debug.Log (string.Format ("UpdateView: {0} = {1} - {2} + {3}", totalWidth, totalElementWidth, lastElementWidth, _containerWidth));

		UpdateAreaSize(totalWidth, _containerHeight);
	}



    private void InsertPaddingAlignment()
    {
        if (_elements.Count == 0)
        {
            // no padding is necessary, because there are no elements
            return;
        }

        if (_paddingPercent == 0.0f)
        {
            // pure left-alignment requires no padding either, so just exit
            return;
        }

        // find the first element
        iGUIElement element = _elements[0];

        // determine how much space is left in the container with this element
        float remainingWidth = _containerWidth - element.getAbsoluteRect().width;

        float paddingWidth = remainingWidth * _paddingPercent;
        _paddingElement = CreatePaddingElement(paddingWidth);
        _paddingElement.setOrder(-1);
    }

    private void UpdateAreaSize(float rawWidth, float rawHeight)
    {
        float actualWidth = rawWidth;
        if ((rawHeight >= _containerHeight) && !_scrollView.hideScrollBars)
        {
            actualWidth -= iGUIRoot.skin.verticalScrollbar.fixedWidth;
        }

        float actualHeight = rawHeight;
        if ((rawWidth >= _containerWidth) && !_scrollView.hideScrollBars)
        {
            actualHeight -= iGUIRoot.skin.horizontalScrollbar.fixedHeight;
        }

        if (_scrollView.isAreaWidthRelative)
        {
            _scrollView.isAreaWidthRelative = false;
        }

        if (_scrollView.isAreaHeightRelative)
        {
            _scrollView.isAreaHeightRelative = false;
        }


        _scrollView.setAreaSize(actualWidth, actualHeight);
        _scrollView.refreshRect();
        _scrollView.refreshStyle();
    }
}
