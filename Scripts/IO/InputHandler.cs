using iGUI;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using PDollarGestureRecognizer;

public class InputHandler: MonoBehaviour, IInput 
{
	private bool _buttonDown = false;
	public bool IsActive { get; protected set; }

	private List<Vector3> _pointsWhileButtonDown = new List<Vector3>();
	private List<List<Vector3>> _allDrawnPaths = new List<List<Vector3>>();
	private Vector3 _lastUpdatedPoint;
	private Vector3 _releasePoint;

	public delegate void ClearLinesCallback();
	public event ClearLinesCallback OnLinesClear;

	public delegate void OnSwipeCallback(Vector3 screenPoint);
	public event OnSwipeCallback OnSwipeStart;
	public event OnSwipeCallback OnSwipeDrag;
	public event OnSwipeCallback OnSwipeEnd;

	public delegate void OnEscapeCallback(Vector3 screenPoint);
	public event OnEscapeCallback OnEscapeGesture;

	enum SWIPE_TYPE
	{
		STATIONARY,
		VERTICAL,
		HORIZONTAL,
		DIAGONAL
	}

	void Awake()
	{
		ClearTracking();
		IsActive = true;
	}

	void Start()
	{
		//
	}

	public void Activate()
	{
		if(!IsActive)
		{
			IsActive = true;
		}
	}

	public void Deactivate()
	{
		if(IsActive)
		{
			IsActive = false;
		}
	}

	private void AddVectorToList(Vector3 point)
	{
		_pointsWhileButtonDown.Add(point);
	}

	private void ClearTracking()
	{
		_pointsWhileButtonDown.Clear();
		_lastUpdatedPoint = new Vector3(0f,0f,0f);
		_releasePoint = new Vector3(0f,0f,0f);
	}

	public void ClearLinesFromCamera()
	{
		_allDrawnPaths.Clear();

		if(OnLinesClear != null)
		{
			OnLinesClear();
		}
	}

    public void ActiveDown(Vector3 startPoint)
    {
		if(!IsActive)
		{
			return;
		}

		if(!_buttonDown)
		{
			ClearTracking();
			_lastUpdatedPoint = new Vector3(startPoint.x,startPoint.y,startPoint.z);
			AddVectorToList(_lastUpdatedPoint);

			if(OnSwipeStart != null)
			{
				OnSwipeStart(startPoint);
			}
		}
    }

	public void ActiveUp(Vector3 endPoint)
	{
		if(!IsActive)
		{
			return;
		}

		if(_buttonDown)
		{
			if(_releasePoint != Vector3.zero)
			{
				_releasePoint = new Vector3(0f,0f,0f);
			}
			_releasePoint = new Vector3(endPoint.x,endPoint.y,endPoint.z);
			AddVectorToList(_releasePoint);

			_allDrawnPaths.Add(_pointsWhileButtonDown);

			_buttonDown = false;
			if(OnSwipeEnd != null)
			{
				OnSwipeEnd(endPoint);
			}
			if(OnEscapeGesture != null)
			{
				OnEscapeGesture(endPoint);
			}
		}
	}

	private SWIPE_TYPE CompareDifferences(Vector3 firstPoint, Vector3 secondPoint)
	{
		SWIPE_TYPE swipe = SWIPE_TYPE.STATIONARY;

		float xDiff = GetDifferenceBetween (firstPoint.x, secondPoint.x);
		float yDiff = GetDifferenceBetween (firstPoint.y, secondPoint.y);

		if(!((xDiff >= 5.0f) && (yDiff >= 5.0f)))
		{
			return swipe;
		}
		else if((xDiff >= 5.0f) && (yDiff >= 5.0f))
		{
			swipe = SWIPE_TYPE.DIAGONAL;
		}
		else if(!(xDiff >= 5.0f) && (yDiff >= 5.0f))
		{
			swipe = SWIPE_TYPE.VERTICAL;
		}
		else if((xDiff >= 5.0f) && !(yDiff >= 5.0f))
		{
			swipe = SWIPE_TYPE.HORIZONTAL;
		}
		return swipe;
	}

	public void ActiveUpdate(Vector3 currentPoint)
	{
		if(!IsActive)
		{
			return;
		}

		Vector3 lastPoint = _pointsWhileButtonDown[_pointsWhileButtonDown.Count - 1];
		_buttonDown = true;
		SWIPE_TYPE swipe = CompareDifferences(lastPoint, currentPoint);
		switch(swipe)
		{
			case SWIPE_TYPE.STATIONARY:
				AddVectorToList(currentPoint);
				break;
			case SWIPE_TYPE.HORIZONTAL:
				AddVectorToList(currentPoint);
				break;
			case SWIPE_TYPE.VERTICAL:
				AddVectorToList(currentPoint);
				break;
			case SWIPE_TYPE.DIAGONAL:
				AddVectorToList(currentPoint);
				break;
		}

		if(OnSwipeDrag != null)
		{
			OnSwipeDrag(currentPoint);
		}
	}

	public void ResetTracking()
	{
		_buttonDown = false;
		ClearTracking();
	}

	private float GetDifferenceBetween(float pointOne, float pointTwo)
	{
		if(pointOne > pointTwo)
		{
			return (pointOne - pointTwo);
		}
		else 
		{
			return (pointTwo - pointOne);
		}
	}
}
