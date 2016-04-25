using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BezierGuideObject : MonoBehaviour 
{
	[SerializeField]
	private Bezier _myBezier;
	private float t = 0.0f;
	private bool _isMoving = false;
//	private int _revolutions = 0;

	public void StartGuideCurve(Vector3[] guidePoints)
	{
		_myBezier = new Bezier(guidePoints[0],guidePoints[1],guidePoints[2],guidePoints[3]);
		_isMoving = true;
	}

	public void KillGuide()
	{
		_isMoving = false;
	}

	void DestroyGuide ()
	{
		Destroy(gameObject);
	}

	void FixedUpdate()
	{
		if(_isMoving)
		{
			Vector3 vec = _myBezier.GetPointAtTime(t);
			transform.position = vec;
			t += 0.03f;
			if(t > 1.2f)
			{
				_isMoving = false;
//				t = 0.0f;
//				transform.position = _myBezier.GetPointAtTime(0.0f);
//				++_revolutions;
//				if(_revolutions >= 1)
//				{
//					_isMoving = false;
//				}
			}
		}
		else if((!_isMoving))
		{
			DestroyGuide();
		}
	}
}
