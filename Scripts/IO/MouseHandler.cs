using UnityEngine;
using System.Collections;

public class MouseHandler : MonoBehaviour 
{
	private IInput _handler = null;
	private int _clickID = -1;
	private Rect _boundaries;

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
		if(Input.GetMouseButtonDown(0) && (_clickID == -1))
		{
			++_clickID;
//			Debug.Log(_clickID.ToString() + " Mouse handler Down clicks");
			_handler.ActiveDown(AdjustForBoundaries(Input.mousePosition));
		}
		else if(Input.GetMouseButton(0) && (_clickID > -1))
		{
//			Debug.Log(_clickID.ToString() + " Mouse handler Update clicks");
			try
			{
				_handler.ActiveUpdate(AdjustForBoundaries(Input.mousePosition));
			}
			catch(System.Exception)
			{
				Debug.LogWarning("Mouse handler update exception...");
			}
		}
		else if(Input.GetMouseButtonUp(0) && (_clickID > -1))
		{
			_clickID = -1;
//			Debug.Log(_clickID.ToString() + " Mouse handler Up clicks");
			_handler.ActiveUp(AdjustForBoundaries(Input.mousePosition));
		}
		else if(Input.GetMouseButtonUp(1))
		{
			_handler.ClearLinesFromCamera();
		}
		else if(_clickID >= 0)
		{
			_clickID = -1;
			_handler.ActiveUp(AdjustForBoundaries(Input.mousePosition));
			_handler.ResetTracking();
		}
	}
}
