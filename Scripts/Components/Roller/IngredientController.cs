using UnityEngine;
using System.Collections;

public class IngredientController : MonoBehaviour
{
	private Rigidbody _myBall = null;
	public float speed = 0.0f;
	private bool _isEnabled = false; 

	void Start () 
	{
		_myBall = gameObject.GetComponent<Rigidbody>();
	}

	public void EnableBall()
	{
		_isEnabled = true;
	}

	void OnTriggerEnter(Collider other)
	{
		Debug.Log (other.name + " is touching me");
		if((other.tag == "CrashPad") && (!_isEnabled))
		{
			EnableBall();
		}
	}

	void FixedUpdate()
	{
		if(_isEnabled)
		{
			#if!(UNITY_IOS || UNITY_IPHONE || UNITY_ANDROID)
			float moveHorizontal = Input.GetAxis ("Horizontal");
			float moveVertical = Input.GetAxis ("Vertical");
			#elif(UNITY_IOS || UNITY_IPHONE || UNITY_ANDROID) && !(UNITY_EDITOR)
			float moveHorizontal = Input.acceleration.x;
			float moveVertical = Input.acceleration.y;
			#elif UNITY_EDITOR
			float moveHorizontal = Input.GetAxis ("Horizontal");
			float moveVertical = Input.GetAxis ("Vertical");
			#endif

			Vector3 movement = new Vector3(moveHorizontal, 0.0f, moveVertical);
			_myBall.AddForce (movement * speed);
		}
	}
}
