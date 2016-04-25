using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TouchHandler : MonoBehaviour 
{
	private IInput _handler = null;
	private float _minimumMovement = 10f;
	private Vector2 _start;
	private Vector2 _lastPoint;
	private int _touchID = -1;
	private Rect _boundaries;
//    public bool skip = false;
	public void AcceptInterface(InputHandler handler)
	{
		_handler = handler;
	}

	public void SetBoundaries(Rect boundaries)
	{
		_boundaries = boundaries;
	}

	private Vector3 AdjustForBoundaries(Vector3 inputPosition)
	{
		if(inputPosition.x < _boundaries.x)
		{
			inputPosition.x = _boundaries.x;
		}
		if(inputPosition.x > (_boundaries.x + _boundaries.width))
		{
			inputPosition.x = (_boundaries.x + _boundaries.width);
		}
		if(inputPosition.y < _boundaries.y)
		{
			inputPosition.y = _boundaries.y;
		}
		if(inputPosition.y > (_boundaries.y + _boundaries.height))
		{
			inputPosition.y = (_boundaries.y + _boundaries.height);
		}
		
		return inputPosition;
	}

	void Update()
	{
        // Leaving this comment out for when we need to do new record to ensure no random input.
//        if (skip)
//        {
//            return;
//        }

		if(Input.touches.Length > 0)
		{
			foreach(var touch in Input.touches)
			{
				var first = touch.position;
				if((touch.phase == TouchPhase.Began) && (_touchID == -1))
				{
					_touchID = touch.fingerId;
					_start = first;
					Vector3 convertedStart = new Vector3(_start.x,_start.y);
					_handler.ActiveDown(AdjustForBoundaries(convertedStart));
				}
				else if(touch.fingerId == _touchID)
				{
					var delta = first - _start;
					if((touch.phase == TouchPhase.Moved) && (delta.magnitude > _minimumMovement))
					{
						_touchID = -1;
						_lastPoint = delta;
						Vector3 convertedCurrent = new Vector3(delta.x,delta.y);
						_handler.ActiveUpdate(AdjustForBoundaries(convertedCurrent));
					}
					else if((touch.phase == TouchPhase.Ended) || (touch.phase == TouchPhase.Canceled))
					{
						_touchID = -1;
						_lastPoint = delta;
						Vector3 convertedEnd = new Vector3(delta.x,delta.y);
						_handler.ActiveUp(AdjustForBoundaries(convertedEnd));
					}
				}
//				else if((touch.fingerId > _touchID) && (touch.phase == TouchPhase.Began || touch.phase == TouchPhase.Canceled || touch.phase == TouchPhase.Ended))
//				{
//					_touchID = -1;
//					_handler.ResetTracking();
//					_handler.ClearLinesFromCamera();
//				}
			}
			if(Input.touchCount > 1)
			{
//				if((touch.fingerId > _touchID) && (touch.phase == TouchPhase.Began || touch.phase == TouchPhase.Canceled || touch.phase == TouchPhase.Ended))
//				{
					_touchID = -1;
					_handler.ResetTracking();
					_handler.ClearLinesFromCamera();
//				}
			}
		}
		else if((Input.touches.Length == 0) && (_touchID > -1))
		{
			_touchID = -1;
			_handler.ActiveUp(AdjustForBoundaries(new Vector3(_lastPoint.x,_lastPoint.y)));
			_handler.ResetTracking();
		}
	}

//	private Vector3 ConvertToVector3(Vector2 touchPosition)
//	{
//		return Camera.main.ScreenToWorldPoint(touchPosition);
//	}
}
