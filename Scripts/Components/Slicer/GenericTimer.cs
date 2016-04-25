using UnityEngine;
using System.Collections;

public class GenericTimer : MonoBehaviour 
{
	private float _timeRemaining = 0.0f;
	private bool _inPlay = false;
	private int _myInstance = 0;
	private int _speedMultiplier = 1;
	
	public delegate void TimerInteractionCallback(bool isInPlay);
	public event TimerInteractionCallback TimerStarted;
	public event TimerInteractionCallback TimerStopped;
	
	public delegate void CurrentPositionCallback(float currentHeight);
	public event CurrentPositionCallback CurrentPosition;
	
	public int TimerInstanceID {
		get { return _myInstance; }
		set { _myInstance = value; }
	}

	public bool IsPaused 
	{
		get { return _inPlay; }
	}

	void Update()
	{
		if(_inPlay)
		{
			
			_timeRemaining -= (Time.deltaTime * _speedMultiplier);
			if(_timeRemaining <= 0.0f)
			{
				//Finish mini game Call
				StopTimer();
//				Debug.Log("Game's done");
			}
			else
			{
				//DO other call that iterates visually
				UpdateManager();
//				Debug.Log("Time remaining: " + _timeRemaining.ToString ());
			}
			
		}
	}

	void UpdateManager ()
	{
		if(CurrentPosition != null)
		{
			CurrentPosition(_timeRemaining);
		}
	}

	public void AddTime(float increment)
	{
		_timeRemaining += increment;
	}

	public void StopTimer()
	{
		_inPlay = false;
		if(TimerStopped != null)
		{
			TimerStopped(_inPlay);
		}

//		Debug.Log("Time left: " + _timeRemaining.ToString());
	}
	
	public void KillTimer(int instanceID)
	{
		if(instanceID == _myInstance)
		{
			DestroyImmediate(this);
		}
		//DestroyImmediate(gameObject);
	}

	public void CreateTimer(float timeRemaining, int speed)
	{
		_timeRemaining = timeRemaining;
		_speedMultiplier = speed;
	}

	public void TogglePause()
	{
		if(_inPlay)
		{
			_inPlay = false;
		}
		else
		{
			_inPlay = true;
		}
	}

	public void BeginTimer()
	{
		_inPlay = true;
		if(TimerStarted != null)
		{
			TimerStarted(_inPlay);
		}
		Debug.Log("Timer " + _myInstance.ToString () + " has started");
	}
}
