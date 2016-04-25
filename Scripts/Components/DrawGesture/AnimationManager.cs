using UnityEngine;
using System.Collections;

public class AnimationManager : MonoBehaviour 
{

	private Animator myAnimator = null;
	private bool _isSelected = false;
	private bool _correctGesture = false;
	private bool _isEvaluating = false;

	// Use this for initialization
	void Start () 
	{
		myAnimator = gameObject.GetComponent<Animator>();
	}

	public void ChangeAnimationState(bool isSelected)
	{
		_isSelected = isSelected;
		myAnimator.SetBool("_isSelected",_isSelected);
	}

	public void EvaluateAnimationState(bool isEvaluating)
	{
		_isEvaluating = isEvaluating;
		myAnimator.SetBool("_isEvaluating",_isEvaluating);
	}

	public void CorrectGestureState(bool correctGesture)
	{
		_correctGesture = correctGesture;
		myAnimator.SetBool ("_correctGesture",_correctGesture);
	}

	public void ResetState()
	{
		_correctGesture = false;
		_isEvaluating = false;
		myAnimator.SetBool ("_correctGesture",_correctGesture);
		myAnimator.SetBool("_isEvaluating",_isEvaluating);
	}

	// Update is called once per frame
	void Update () 
	{
		if(gameObject.name == "Prinny_0")
		{
			if(Input.GetKeyUp(KeyCode.A))
			{
				if(_isSelected)
				{
					_isSelected = false;
					myAnimator.SetBool("_isSelected",_isSelected);
				}
				else
				{
					_isSelected = true;
					myAnimator.SetBool("_isSelected",_isSelected);
				}
			}

			if((_isSelected) && (Input.GetKeyUp(KeyCode.B)))
			{
				if(!_isEvaluating)
				{
					_isEvaluating = true;
					myAnimator.SetBool("_isEvaluating", _isEvaluating);
					myAnimator.SetBool("_correctGesture", _correctGesture);
				}
				else if(_isEvaluating)
				{
					_isEvaluating = false;
					_correctGesture = false;
					myAnimator.SetBool("_isEvaluating", _isEvaluating);
					myAnimator.SetBool("_correctGesture", _correctGesture);
				}
			}
			if(Input.GetKeyUp(KeyCode.N))
			{
				if(_correctGesture)
				{
					_correctGesture = false;
				}
				else
				{
					_correctGesture = true;
				}
			}
		}
	}
}
