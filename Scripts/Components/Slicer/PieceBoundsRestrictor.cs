using UnityEngine;
using System.Collections;

public class PieceBoundsRestrictor : MonoBehaviour 
{
	private Vector3 _rightBounds;
	private Vector3 _leftBounds;

	void Start()
	{
		GameObject board = GameObject.FindGameObjectWithTag ("Board");

//		_rightBounds = Camera.main.ScreenToWorldPoint(new Vector3((float)Screen.width, 0.0f));
//		_leftBounds = Camera.main.ScreenToWorldPoint(new Vector3(0.0f, 0.0f));

		_rightBounds = board.GetComponent<Collider2D>().bounds.max;
		_rightBounds.x = _rightBounds.x - (gameObject.transform.localScale.x * 0.25f);
		_leftBounds = board.GetComponent<Collider2D>().bounds.min;
		_leftBounds.x = _leftBounds.x + (gameObject.transform.localScale.x * 0.25f);
	}

	void FixedUpdate()
	{
		if(gameObject.transform.position.x >= _rightBounds.x)
		{
//			Vector3 current = gameObject.transform.position;
			gameObject.GetComponent<Rigidbody2D>().AddForce(Vector2.zero);
			gameObject.GetComponent<Rigidbody2D>().Sleep();
//			gameObject.transform.position.Set(_rightBounds.x, current.y,current.z);
		}
		else if(gameObject.transform.position.x <= _leftBounds.x)
		{
//			Vector3 current = gameObject.transform.position;
			gameObject.GetComponent<Rigidbody2D>().AddForce(Vector2.zero);
			gameObject.GetComponent<Rigidbody2D>().Sleep();
		}
	}

}
