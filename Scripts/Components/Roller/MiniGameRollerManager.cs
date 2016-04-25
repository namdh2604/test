using iGUI;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class MiniGameRollerManager : MonoBehaviour 
{
	private InputHandler _input = null;
	private MouseHandler _mouse = null;
	private TouchHandler _touch = null;
	private Vector3 _startEscapePoint;
	private Rect _bounds;

	public GameObject ingredient = null;
	private GameObject _spawnPoint = null;
	private GameObject _roller = null;
	private IngredientController _ingredientController = null;

	private iGUILabel _debugLabel = null;
	private iGUIButton _startButton = null;
	private iGUIFloatHorizontalSlider _speedSlider = null;

	void Awake()
	{
		_spawnPoint = GameObject.FindGameObjectWithTag("Spawn");
	}

	void AddHandlers ()
	{
		_input = gameObject.AddComponent<InputHandler>();
		_mouse = gameObject.AddComponent<MouseHandler>();
		_touch = gameObject.AddComponent<TouchHandler>();
		
		_mouse.AcceptInterface(_input);
		_touch.AcceptInterface(_input);
	}

	void SetBounds ()
	{
		_bounds = gameObject.GetComponent<iGUIContainer>().getAbsoluteRect();
		_mouse.SetBoundaries(_bounds);
		_touch.SetBoundaries(_bounds);
	}

	void EnableCallBacks ()
	{
		_input.OnSwipeStart += HandleOnSwipeStart;
//		_input.OnSwipeDrag += HandleOnSwipeDrag;
//		_input.OnSwipeEnd += HandleOnSwipeEnd;
		_input.OnLinesClear += HandleOnLinesClear;
	}

	void StoreStartPoint (Vector3 screenPoint)
	{
		_startEscapePoint = screenPoint;
	}
	
	void HandleOnEscapeGesture (Vector3 screenPoint)
	{
		float distance = Vector3.Distance(_startEscapePoint, screenPoint);
		float xDiff;

		if(_startEscapePoint.x >= screenPoint.x)
		{
			xDiff = _startEscapePoint.x - screenPoint.x;
		}
		else
		{
			xDiff = screenPoint.x - _startEscapePoint.x;
		}
		
		if((xDiff <= 5.0f) && (distance > 2.5f))
		{
			Debug.Log("Quit to Selection screen");
			SceneManager.LoadScene("selectScenes");
		}
		else
		{
			Debug.Log("Escape Cancelled");
			_input.OnEscapeGesture -= HandleOnEscapeGesture;
			_startEscapePoint = Vector3.zero;
		}
	}

	void HandleOnSwipeStart (Vector3 screenPoint)
	{
		if((screenPoint.x <= (_bounds.width * 0.1f)) && (screenPoint.y <= (_bounds.height - 5.0f)))
		{
			Debug.Log("Call the quit to menu button");
			StoreStartPoint(screenPoint);
			_input.OnEscapeGesture += HandleOnEscapeGesture;
		}
	}

//	void HandleOnSwipeDrag (Vector3 screenPoint)
//	{
//		throw new NotImplementedException ();
//	}
//
//	void HandleOnSwipeEnd (Vector3 screenPoint)
//	{
//		throw new NotImplementedException ();
//	}

	void HandleOnLinesClear ()
	{
//		throw new NotImplementedException ();
	}

	void Start()
	{
		AddHandlers();
		SetBounds();
		EnableCallBacks();		

		iGUIContainer container = gameObject.GetComponent<iGUIContainer>();
		_startButton = container.addElement<iGUIButton>();
		_startButton.style.fixedHeight = 0;
		_startButton.style.stretchHeight = true;
		_startButton.setPositionAndSize(new Rect (0.5f, 0.5f, 0.25f, 0.3f));
		_startButton.style.fontSize = Convert.ToInt32 (Screen.height * 0.05f);
		_startButton.label.text = "START";
		_startButton.clickCallback += ClickHandler;
	}

	void FixedUpdate()
	{
		if((_debugLabel != null) && (_roller != null))
		{
			_debugLabel.label.text = _roller.transform.position.ToString () + " is where the ingredient is";
		}
	}

	void CreateIngredient ()
	{
		if(_roller != null)
		{
			Destroy(_roller);
			_roller = null;
		}

		_roller = Instantiate (ingredient, _spawnPoint.transform.position, _spawnPoint.transform.rotation) as GameObject;
		_roller.name = "CurrentIngredient";
		_ingredientController = _roller.GetComponent<IngredientController>();
		if(_debugLabel == null)
		{
			_debugLabel = gameObject.GetComponent<iGUIContainer> ().addElement<iGUILabel> ();
			_debugLabel.setPositionAndSize (new Rect (0.5f, 0.0f, 0.5f, 0.1f));
			_debugLabel.label.text = _roller.transform.position.ToString () + " is where the ingredient is";
		}
		if(_speedSlider == null)
		{
			_speedSlider = gameObject.GetComponent<iGUIContainer>().addElement<iGUIFloatHorizontalSlider> ();
			_speedSlider.sliderStyle.fixedHeight = Convert.ToInt32(Screen.height * 0.08f);
			_speedSlider.sliderStyle.fixedWidth = Convert.ToInt32(Screen.width * 0.3f);
			_speedSlider.sliderStyle.stretchWidth = false;
			_speedSlider.sliderStyle.stretchHeight = false;
			_speedSlider.thumbStyle.stretchWidth = true;
			_speedSlider.thumbStyle.stretchHeight = true;
			_speedSlider.thumbStyle.fixedHeight = Convert.ToInt32(Screen.height * 0.08f);
			_speedSlider.thumbStyle.fixedWidth = Convert.ToInt32(Screen.width * 0.05f);
			_speedSlider.setPositionAndSize (new Rect (1f,0f,0.5f,0.25f));
			_speedSlider.allowedMin = 100.0f;
			_speedSlider.allowedMax = 1000.0f;
			_speedSlider.min = 100.0f;
			_speedSlider.max = 1000.0f;
			_speedSlider.setValue(115.0f);
			_speedSlider.valueChangeCallback += ClickHandler;
		}
	}

	public void ResetTheMiniGame()
	{
		Destroy(_roller);
		_roller = null;
		_startButton.enabled = true;
	}

	void HandleSpeedChange ()
	{
		_ingredientController.speed = _speedSlider.value;
	}

	public void ClickHandler(iGUIElement target)
	{
		if(target == _startButton)
		{
			_startButton.enabled = false;
			CreateIngredient();
		}
		else if(target == _speedSlider)
		{
			HandleSpeedChange();
		}
	}

}
