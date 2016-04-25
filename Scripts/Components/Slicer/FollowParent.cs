using UnityEngine;
using System.Collections;

public class FollowParent : MonoBehaviour 
{

	private Transform _parent = null;
	private Vector3 _myBounds;

	void Start()
	{
		_parent = gameObject.transform.parent.transform;
		_myBounds = gameObject.transform.position;
	}

	void FixedUpdate()
	{
		if(_parent.transform.position.x != _myBounds.x)
		{
			_myBounds.x = _parent.transform.position.x;
			gameObject.transform.position = _myBounds;
		}
	}
}
