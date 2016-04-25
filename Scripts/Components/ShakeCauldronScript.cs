using UnityEngine;
using System.Collections;

public class ShakeCauldronScript : MonoBehaviour 
{
	[SerializeField]
	private GameObject _wholeCauldron = null;
	private Vector3 _originPoint;
	private Vector3 _rightTarget;
	private Vector3 _leftTarget;
	private bool _isOscillating = false;
	private bool _hasBegun = false;
	private bool _needsToStop = false;
	private bool _isAtRightTarget = false;
	private bool _isAtLeftTarget = false;
	private float _timeOscillating = 0.0f;
	private float _speed = 25.0f;

	public bool isOscillating 
	{
		get { return _isOscillating; }
	}

	void Awake()
	{
//		_originPoint = _wholeCauldron.transform.position;
		Transform[] myChildren = gameObject.GetComponentsInChildren<Transform>();
		for(int i = myChildren.Length - 1; i > -1; --i)
		{
			if(myChildren[i].name == "Cauldron")
			{
				_wholeCauldron = myChildren[i].gameObject;
				_originPoint = _wholeCauldron.transform.position;
			}
		}
	}

	void FixedUpdate()
	{
		if(Debug.isDebugBuild)
		{
		if(Input.GetKeyUp(KeyCode.D))
		{
			ShakeCauldron();
		}
		if(Input.GetKeyUp(KeyCode.A))
		{
			StopCauldron();
		}
		}

		if(_hasBegun)
		{
			_timeOscillating += Time.deltaTime;

			if((_wholeCauldron.transform.position == _originPoint) && (!_isOscillating))
			{
				float step = Time.deltaTime * _speed;
				_wholeCauldron.transform.position = Vector3.Slerp(_wholeCauldron.transform.position,_rightTarget,step);
//				_wholeCauldron.transform.position = Vector3.Lerp(_wholeCauldron.transform.position,_rightTarget,step);
				_isOscillating = true;
				_isAtLeftTarget = true;
			}
			else if((_isOscillating) && (!_isAtRightTarget) && (_isAtLeftTarget))
			{
				float step = Time.deltaTime * _speed;
				_wholeCauldron.transform.position = Vector3.Slerp(_wholeCauldron.transform.position,_rightTarget,step);
//				_wholeCauldron.transform.position = Vector3.Lerp(_wholeCauldron.transform.position,_rightTarget,step);
				if(_wholeCauldron.transform.position == _rightTarget)
				{
					_isAtLeftTarget = false;
					_isAtRightTarget = true;
				}
			}
			else if((_isOscillating) && (_isAtRightTarget) && (!_isAtLeftTarget))
			{
				float step = Time.deltaTime * _speed;
				_wholeCauldron.transform.position = Vector3.Slerp(_wholeCauldron.transform.position,_leftTarget,step);
//				_wholeCauldron.transform.position = Vector3.Lerp(_wholeCauldron.transform.position,_leftTarget,step);
				if(_wholeCauldron.transform.position == _leftTarget)
				{
					_isAtLeftTarget = true;
					_isAtRightTarget = false;
					UpdateTargets(_timeOscillating * 2.0f);
				}
			}
//			else if((_wholeCauldron.transform.position == _originPoint) && (_isOscillating) && (_isAtLeftTarget))
//			{
//				UpdateTargets(_timeOscillating);
//			}
		}
		if((!_hasBegun) && (_needsToStop))
		{
			float step = Time.deltaTime * 7.5f;

			_wholeCauldron.transform.position = Vector3.Lerp(_wholeCauldron.transform.position,_originPoint,step);

			if(_wholeCauldron.transform.position == _originPoint)
			{
				_needsToStop = false;
				_timeOscillating = 0.0f;
				_speed = 25.0f;
				_isAtRightTarget = false;
				_isAtLeftTarget = false;
				_isOscillating = false;
				_hasBegun = false;
			}
		}
	}

	void UpdateTargets(float timeOscillating)
	{
		_rightTarget.x = UnityEngine.Random.Range(0.1f,0.3f);
		_leftTarget.x = UnityEngine.Random.Range(-0.3f,-0.1f);
		_speed += timeOscillating;
		if(_speed >= 50.0f)
		{
			_speed = 50.0f;
		}
//		_rightTarget.x += (timeOscillating * 0.1f);
//		_leftTarget.x -= (timeOscillating * 0.1f);
	}

	public void StopCauldron ()
	{
//		Debug.Log("Stop Everything!!");
		_hasBegun = false;
		_needsToStop = true;
	}

	public void ShakeCauldron ()
	{
//		Debug.Log("Shake that booty");
		_rightTarget = new Vector3((_originPoint.x + 0.1f),_originPoint.y,_originPoint.z);
		_leftTarget = new Vector3((_originPoint.x - 0.1f),_originPoint.y,_originPoint.z);
		_hasBegun = true;
	}
}
