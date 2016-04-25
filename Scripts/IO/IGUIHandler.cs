using iGUI;

using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public interface IButtonHandler
{
	void Activate();
	void Deactivate();
}

public class IGUIHandler : MonoBehaviour, IButtonHandler 
{
	public iGUIButton PressedButton { get; protected set; }
	public bool IsTrackingButton { get; protected set; }
	public event Action<iGUIButton> MovedAway;
	public event Action<iGUIButton> MovedBack;
	public event Action<iGUIButton,bool> ReleasedButtonEvent;
	public event Action<iGUIButton,bool> InputCancelledEvent;

	private bool _isActive;
	public bool IsActive 
	{ 
		get 
		{
			// must take into account AllowInput as IsActive is used as a condition before performing some actions outside the control of iGUIHandler (e.g., tinting buttons on selection)
			return _isActive && AllowInput;
		} 
		protected set 
		{
			_isActive = value;
		}
	}

	bool IsOverButton = true;
	ButtonBounds Boundaries;
	IActivity HandlerInterface;


	// HACK: DisableAll is static and will apply to every iGUIHandler, an iGUIHandler instance can override it by setting their own OverrideDisableAll
	// DON"T forget to reenable all input when done!
	public static bool DisableAll = false;
	public bool OverrideDisableAll { get; set; }

	private bool AllowInput
	{
		get 
		{
			return !DisableAll || OverrideDisableAll;
		}
	}


	protected void Awake()
	{
		if(SystemInfo.deviceType != DeviceType.Handheld)
		{
			HandlerInterface = gameObject.AddComponent<MouseActivity>();
//			Debug.Log("Mouse Handling");
		}
		else
		{
			HandlerInterface = gameObject.AddComponent<TouchActivity>();
//			Debug.Log("Touch Handling");
		}

		HandlerInterface.OnRelease += HandleOnRelease;
		HandlerInterface.OnUpdate += HandleOnUpdate;
		HandlerInterface.OnCancel += HandleOnCancelled;
		IsActive = true;

		OverrideDisableAll = false;
	}
	
	void HandleOnUpdate(Vector3 position)
	{
		if(AllowInput) 
		{
			if(isInBounds(position))
			{
				if(!IsOverButton)
				{
					IsOverButton = true;
					if(MovedBack != null)
					{
						MovedBack(PressedButton);
					}
				}
			}
			else
			{
				if(IsOverButton)
				{
					IsOverButton = false;
					if(MovedAway != null)
					{
						MovedAway(PressedButton);
					}
				}
			}
		}
	}

	void HandleOnRelease()
	{
		if (AllowInput) 
		{
			IsTrackingButton = false;
			ReleasedButtonEvent (PressedButton, IsOverButton);
			Boundaries = null;
			PressedButton = null;
			IsOverButton = false;
		} 
	}

	void HandleOnCancelled()
	{
		if (AllowInput) 
		{
			IsTrackingButton = false;
			if(InputCancelledEvent!=null)
				InputCancelledEvent (PressedButton, false);
			Boundaries = null;
			PressedButton = null;
			IsOverButton = false;
		}
	}

	public void SelectButton(iGUIButton pressedButton)
	{	
		// IsActive already checks for AllowInput	
		if ((IsActive) && (PressedButton == null)) 
		{
			PressedButton = pressedButton;
			Boundaries = new ButtonBounds (PressedButton);
			IsTrackingButton = true;

			HandlerInterface.Tracking ();
		} 

	}

	public void Activate()
	{
		IsActive = true;
	}

	public void Deactivate()
	{
		IsActive = false;
	}

	bool isInBounds(Vector3 position)
	{
		if((isInLateralBounds(position.x)) && (isIsInVerticalBounds(position.y)))
		{
			return true;
		}

		return false;
	}

	bool isInLateralBounds(float xPosition)
	{
		if((Boundaries.MinX < xPosition) && (Boundaries.MaxX > xPosition))
		{
			return true;
		}

		return false;
	}

	bool isIsInVerticalBounds(float yPosition)
	{
		if((Boundaries.MinY > yPosition) && (Boundaries.MaxY < yPosition))
		{
			return true;
		}

		return false;
	}

	private class ButtonBounds
	{
		public float MinX { get; set; }
		public float MaxX { get; set; }

		public float MinY { get; set; }
		public float MaxY { get; set; }

		public ButtonBounds(iGUIButton button)
		{
			var rect = button.getAbsoluteRect();

			MinX = rect.xMin;
			MaxX = rect.xMax;

			MinY = Screen.height - rect.y;
			MaxY = Screen.height - (rect.y + rect.height);
		}
	}

	private interface IActivity
	{
		event Action<Vector3> OnUpdate;
		event Action OnRelease;
		event Action OnCancel;
		void Tracking();
		Vector3 CurrentLocation();
		void Released();
	}

	private class MouseActivity: MonoBehaviour,IActivity
	{
		bool _isTracking = false;

		public event Action OnRelease;
		public event Action<Vector3> OnUpdate;
		public event Action OnCancel;

		Rect ScreenBounds;
		
		protected void Awake()
		{
			ScreenBounds = new Rect(0.5f,0.5f,Screen.width,Screen.height);
//			Debug.Log(ScreenBounds.ToString());
		}

		public void Tracking()
		{
			_isTracking = true;
		}

		void Update()
		{
			if(_isTracking)
			{
				if(Input.GetMouseButtonUp(0))
				{
					Released();
				}
				else if((Input.GetMouseButton(0)) && (Input.GetMouseButtonDown(1)))
				{
					Cancelled();
				}
				else
				{
					if(OnUpdate != null)
					{
						OnUpdate(CurrentLocation());
					}
				}
			}
		}

		private Vector3 AdjustForBoundaries(Vector3 inputPosition)
		{
			if(inputPosition.x < ScreenBounds.x)
			{
				inputPosition.x = ScreenBounds.x;
			}
			if(inputPosition.x > (ScreenBounds.x + ScreenBounds.width))
			{
				inputPosition.x = (ScreenBounds.x + ScreenBounds.width);
			}
			if(inputPosition.y < ScreenBounds.y)
			{
				inputPosition.y = ScreenBounds.y;
			}
			if(inputPosition.y > (ScreenBounds.y + ScreenBounds.height))
			{
				inputPosition.y = (ScreenBounds.y + ScreenBounds.height);
			}
			
			return inputPosition;
		}

		public Vector3 CurrentLocation()
		{
			var current = AdjustForBoundaries (Input.mousePosition);
			return current;
		}

		public void Cancelled()
		{
			if(OnCancel != null)
			{
				OnCancel();
			}
			_isTracking = false;
		}

		public void Released()
		{
			if(OnRelease != null)
			{
				OnRelease();
			}
			_isTracking = false;
		}
	}

	private class TouchActivity: MonoBehaviour,IActivity
	{
		bool _isTracking = false;

		public event Action OnRelease;
		public event Action<Vector3> OnUpdate;
		public event Action OnCancel;

		Rect ScreenBounds;

		protected void Awake()
		{
			ScreenBounds = new Rect(0.5f,0.5f,Screen.width,Screen.height);
			Debug.Log(ScreenBounds.ToString());
		}

		public void Tracking()
		{
			_isTracking = true;
		}

		void Update()
		{
			if((_isTracking) && (Input.touchCount > 0) && (Input.touchCount < 2))
			{
				if((Input.touches[0].phase != TouchPhase.Ended) && (Input.touches[0].phase != TouchPhase.Canceled))
				{
					if(OnUpdate != null)
					{
						OnUpdate(CurrentLocation());
					}
				}
				else if(Input.touches[0].phase == TouchPhase.Ended)
				{
					Released();
				}
				else if((Input.touchCount > 1)  && (Input.touches[0].phase == TouchPhase.Ended))
				{
					Debug.LogWarning("Extra finger added, cancelling input");
					Cancelled();
				}
				else
				{
					Debug.LogWarning("Something else happening after initializing a single finger touch");
//					Cancelled();
				}
			}
			else if((_isTracking) && (Input.touchCount > 1))
			{
				Debug.LogWarning("Too many fingers on screen, cancelling input...after release");

				if((Input.touches[0].phase == TouchPhase.Ended) && (Input.touches[1].phase == TouchPhase.Ended))
				{
					Debug.LogWarning("Two fingers released, cancelling input...");
					Cancelled();
				}
				else if((Input.touches[0].phase == TouchPhase.Ended) && (Input.touches[1].phase != TouchPhase.Ended))
				{
					Debug.LogWarning("Second finger released, cancelling input...");
					Cancelled();
				}
				else if((Input.touches[0].phase != TouchPhase.Ended) && (Input.touches[1].phase == TouchPhase.Ended))
				{
					Debug.LogWarning("First finger released, cancelling input...");
					Cancelled();
				}
				else
				{
					Cancelled();
				}
			}
		}

		private Vector3 AdjustForBoundaries(Vector2 inputPosition)
		{
			if(inputPosition.x < ScreenBounds.x)
			{
				inputPosition.x = ScreenBounds.x;
			}
			if(inputPosition.x > (ScreenBounds.x + ScreenBounds.width))
			{
				inputPosition.x = (ScreenBounds.x + ScreenBounds.width);
			}
			if(inputPosition.y < ScreenBounds.y)
			{
				inputPosition.y = ScreenBounds.y;
			}
			if(inputPosition.y > (ScreenBounds.y + ScreenBounds.height))
			{
				inputPosition.y = (ScreenBounds.y + ScreenBounds.height);
			}
			
			return new Vector3(inputPosition.x,inputPosition.y);
		}

		public Vector3 CurrentLocation()
		{
			var touchPosition = Input.touches[0].position;
			var current = AdjustForBoundaries (touchPosition);
			return current;
		}

		public void Cancelled()
		{
			if(OnCancel != null)
			{
				OnCancel();
			}
			_isTracking = false;
		}

		public void Released()
		{
			if(OnRelease != null)
			{
				OnRelease();
			}
			_isTracking = false;
		}
	}
}


