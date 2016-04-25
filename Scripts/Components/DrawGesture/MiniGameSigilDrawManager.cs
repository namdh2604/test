using iGUI;
using PDollarGestureRecognizer;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class MiniGameSigilDrawManager : MonoBehaviour 
{
	private InputHandler _input = null;
	private MouseHandler _mouse = null;
	private TouchHandler _touch = null;

	private Rect _bounds;
	private Vector3 _startEscapePoint;
	private List<GameObject> _prinnies = new List<GameObject>();
	private GameObject _glowing = null;
	private List<GameObject> _glowers = new List<GameObject>();
	private Dictionary<GameObject,GameObject> _touchedPoints = new Dictionary<GameObject,GameObject>();
	private Dictionary<string, GameObject> _strokeKeys = new Dictionary<string, GameObject>();
	private List<GameObject> _gridPoints = new List<GameObject>();
	private List<GameObject> _activeGridPoints = new List<GameObject>();
	private List<Point> _points = new List<Point>();
	private List<Gesture> _sigilSets = new List<Gesture>();
	private Dictionary<int,GameObject> _gridColumns = new Dictionary<int,GameObject>();
	private Dictionary<int,List<GameObject>> _gridRows = new Dictionary<int,List<GameObject>>();
	private Sprite _defaultGridSprite = null;
	private GameObject _activeStrokeKey = null;
	private string _activeGesture = string.Empty;
	private Dictionary<string,Dictionary<int,List<int>>> _gesturePathSets = new Dictionary<string,Dictionary<int, List<int>>>();

	private bool _successiveStrokes = false;
	private bool _strokeCountDownActive = false;
	private int _strokeNumber = 0;
	private float _countDownTime = 0.0f;

	private iGUILabel _swipeDebugInfo = null;

	private static List<string> _standardGestureList = new List<string>()
	{
		"HorizontalSwipe",
		"VerticalSwipe",
		"DiagonalSwipeFalling",
		"DiagonalSwipeRising",
		"Box",
		"Circle",
		"Diamond",
		"HourGlass",
		"LeftBottomTriangle",
		"RightBottomTriangle",
		"LeftTopTriangle",
		"RightTopTriangle",
		"LeftPointTriangle",
		"RightPointTriangle"
//		"CheckMark",
	};

	void Start () 
	{
		AddHandlers();
		SetBounds();
		EnableCallBacks();
		SetUpStrokeKey();
		GetGlowingSprite();
		CreateBoard();
		GenerateStrokes();
		GenerateStandardGestures();
		AssignGesture(0);
		AddDebugSwiper();
	}

	void FixedUpdate()
	{
		if(_strokeCountDownActive)
		{
			_countDownTime -= (Time.deltaTime);

			if(_countDownTime <= 0.0f)
			{
				Debug.Log("Time's up, start over");
				_successiveStrokes = false;
				_strokeNumber = 0;
				_strokeCountDownActive = false;
				_points.Clear();
				HandleOnLinesClear();
			}
			else
			{
				_successiveStrokes = true;
			}
		}

		if(Input.GetKeyUp(KeyCode.RightArrow))
		{
			int current = GetActiveStrokeObjectIndex(_activeStrokeKey);
			if(current < 3)
			{
				++current;
			}
			else
			{
				current = 0;
			}
			ChangeActiveStrokeObject(current);
			RandomizeCurrentGestureForEval();
		}
		if(Input.GetKeyUp(KeyCode.LeftArrow))
		{
			int current = GetActiveStrokeObjectIndex(_activeStrokeKey);
			if(current > 0)
			{
				--current;
			}
			else
			{
				current = 3;
			}
			ChangeActiveStrokeObject(current);
			RandomizeCurrentGestureForEval();
		}
	}

	void ChangeGridObjects()
	{
		foreach(GameObject gridPoint in _gridPoints)
		{
			bool isActive = false;
			if(_activeGridPoints.Contains(gridPoint))
			{
				isActive = true;
			}

			gridPoint.GetComponent<GridPointSpriteManager>().ChangeState(_activeStrokeKey.name, isActive);
		}
	}

	void ChangeActiveStrokeObject(int index)
	{
		_activeStrokeKey.GetComponent<AnimationManager>().ChangeAnimationState(false);
		string key = string.Empty;
		switch(index)
		{
		case 0: 
			key = "Prinny_Blue"; 
			break;
			case 1:
			key = "Prinny_Red"; 
			break;
			case 2:
			key = "Prinny_Yellow"; 
			break;
			case 3:
			key = "Prinny_Green"; 
			break;
		}
		_activeStrokeKey = _strokeKeys[key];
		_activeStrokeKey.GetComponent<AnimationManager>().ChangeAnimationState(true);
		ChangeGridObjects();
	}

	int GetActiveStrokeObjectIndex (GameObject activeStrokeObject)
	{
		string objectName = activeStrokeObject.name;
		string[] nameParts = objectName.Split(('_'));
		int index = Convert.ToInt32(nameParts [1]);
		return index;
	}

	void AddDebugSwiper ()
	{
		_swipeDebugInfo = gameObject.GetComponent<iGUIContainer>().addElement<iGUILabel>();
		_swipeDebugInfo.setPositionAndSize(new Rect(0.0f,1.0f,0.3f,0.1f));
		_swipeDebugInfo.style.alignment = TextAnchor.MiddleLeft;
		_swipeDebugInfo.style.fontSize = Convert.ToInt32(Screen.height * 0.05f);
		_swipeDebugInfo.label.text = string.Empty;
	}

	void UpdateDebugLabel(string message)
	{
		_swipeDebugInfo.label.text = message;
	}

	List<GameObject> GetGridColumns (List<GameObject> gridPoints)
	{
		List<GameObject> columns = new List<GameObject>();
		int currentColumn = 0;

		foreach(GameObject go in gridPoints)
		{
			GameObject column = go.transform.parent.gameObject;
			if((!columns.Contains(column)) && (column.name.Contains(currentColumn.ToString())))
			{
				columns.Add(column);
				++currentColumn;
			}
		}

		return columns;
	}

	List<GameObject> GetGridRowsFromId(int idNum)
	{
		List<GameObject> gridRow = new List<GameObject>();
		foreach(GameObject gridPoint in _gridPoints)
		{
			if(gridPoint.name.Contains(idNum.ToString()))
			{
				gridRow.Add(gridPoint);
			}
		}

		return gridRow;
	}

	void CreateBoard()
	{
		_gridPoints = GameObject.FindGameObjectsWithTag("GridPoint").ToList();
		List<GameObject> columns = GetGridColumns(_gridPoints);
		List<GameObject> rows;
		for(int i = 0; i < columns.Count; ++i)
		{
			if(i < (columns.Count - 1))
			{
				rows = GetGridRowsFromId(i);
				_gridRows[i] = rows;
			}
			if((columns[i].name.Contains(i.ToString())) && (!_gridColumns.ContainsKey(i)))
			{
				_gridColumns[i] = columns[i];
			}
		}

		if(_defaultGridSprite == null)
		{
			_defaultGridSprite = _gridRows[0][0].GetComponent<SpriteRenderer>().sprite;
		}

		Debug.Log("Board set up");
	}

	Point GetPointFromDrawPoint(GameObject drawPoint, int stroke)
	{
		Vector3 onScreen = Camera.main.WorldToScreenPoint(drawPoint.transform.position);

//		string[] objectNameParts = drawPoint.name.Split(('_'));
//		drawObjectPoint.StrokeID = Convert.ToInt32(objectNameParts[1]);

		Point drawObjectPoint = new Point (onScreen.x, onScreen.y, stroke);

		return drawObjectPoint;
	}

	Point[] GetPointsFromDrawPoints(List<GameObject> drawPoints, int stroke)
	{
		int totalPoints = drawPoints.Count;
		Point[] newPoints = new Point[totalPoints];

		for(int i = 0; i < totalPoints; ++i)
		{
			Point newPoint = GetPointFromDrawPoint(drawPoints[i],stroke);
			newPoints[i] = newPoint;
		}

		return newPoints;
	}

	void GetGlowingSprite ()
	{
		_glowing = GameObject.FindGameObjectWithTag("Glowing");
	}

	void SetUpStrokeKey ()
	{
		_prinnies = GameObject.FindGameObjectsWithTag("DrawPoint").ToList();
		foreach(GameObject go in _prinnies)
		{
			if(!go.name.Contains("Clone"))
			{
				Animator myAnimator = go.GetComponent<Animator>();
				string key = myAnimator.runtimeAnimatorController.name;
				if(!_strokeKeys.ContainsKey(key))
				{
					_strokeKeys[key] = go;
				}
			}
			else
			{
				Destroy(go,0.15f);
			}
		}
	}

	GameObject GetStrokeKeyFromGOName(string name)
	{
		GameObject returnee = null;

		foreach(GameObject go in _prinnies)
		{
			if(go.name.Contains(name))
			{
				returnee = go;
			}
		}

		return returnee;
	}

	void SetActiveStroke (GameObject strokeKeyObject)
	{
		strokeKeyObject.GetComponent<AnimationManager>().ChangeAnimationState(true);
		_activeStrokeKey = strokeKeyObject;
		foreach(GameObject go in _prinnies)
		{
			if((go != strokeKeyObject) && (!go.name.Contains("Clone")))
			{
				go.GetComponent<AnimationManager>().ChangeAnimationState(false);
			}
			else if(go.name.Contains("Clone"))
			{
				Destroy(go, 0.1f);
			}
		}
	}

	GameObject GetGridPointBasedOnRowAndID(int rowID, int columnID)
	{
		GameObject returnee = null;
		if(_gridRows.ContainsKey(rowID))
		{
			List<GameObject> goList = _gridRows[rowID];
			foreach(GameObject go in goList)
			{
				if(go.transform.parent.name.Contains("Column_" + columnID.ToString()))
				{
					returnee = go;
					break;
				}
			}
		}

		return returnee;
	}

	void AssignGesture(int index)
	{
		_activeGesture = _sigilSets[index].Name;
		_activeGridPoints = GetActivePathObjects(_gesturePathSets[_activeGesture]);
		ChangeGridObjects();
	}

	List<GameObject> GetActivePathObjects(Dictionary<int,List<int>> pathDictionary)
	{
		List<GameObject> gridPointsForGesture = new List<GameObject>();
		
		for(int i = 0; i < pathDictionary.Keys.Count; ++i)
		{
			List<int> currentList = pathDictionary[i];
			int row = currentList[0];
			int column = currentList[1];

			GameObject go = GetGridPointBasedOnRowAndID(row,column);
			
			gridPointsForGesture.Add(go);
		}

		return gridPointsForGesture;
	}

	void GenerateStandardGestures ()
	{
		for(int i = 0; i < _standardGestureList.Count; ++i)
		{
			Dictionary<int,List<int>> strokeKey = new Dictionary<int, List<int>>();
			string currentType = _standardGestureList[i];
			switch(currentType)
			{
				case "HorizontalSwipe":
					strokeKey[0] = new List<int>(2){0,0};
					strokeKey[1] = new List<int>(2){0,1};
					strokeKey[2] = new List<int>(2){0,2};
					strokeKey[3] = new List<int>(2){0,3};
					strokeKey[4] = new List<int>(2){0,4};
					strokeKey[5] = new List<int>(2){0,5};
					break;
				case "VerticalSwipe":
					strokeKey[0] = new List<int>(2){0,0};
					strokeKey[1] = new List<int>(2){1,0};
					strokeKey[2] = new List<int>(2){2,0};
					strokeKey[3] = new List<int>(2){3,0};
					strokeKey[4] = new List<int>(2){4,0};
					break;
				case "DiagonalSwipeFalling":
					strokeKey[0] = new List<int>(2){0,0};
					strokeKey[1] = new List<int>(2){1,1};
					strokeKey[2] = new List<int>(2){2,2};
					strokeKey[3] = new List<int>(2){3,3};
					strokeKey[4] = new List<int>(2){4,4};
					break;
				case "DiagonalSwipeRising":
					strokeKey[0] = new List<int>(2){0,5};
					strokeKey[1] = new List<int>(2){1,4};
					strokeKey[2] = new List<int>(2){2,3};
					strokeKey[3] = new List<int>(2){3,2};
					strokeKey[4] = new List<int>(2){4,1};
					break;
				case "Box":
					strokeKey[0] = new List<int>(2){1,1};
					strokeKey[1] = new List<int>(2){2,1};
					strokeKey[2] = new List<int>(2){3,1};
					strokeKey[3] = new List<int>(2){4,1};
					strokeKey[4] = new List<int>(2){4,2};
					strokeKey[5] = new List<int>(2){4,3};
					strokeKey[6] = new List<int>(2){4,4};
					strokeKey[7] = new List<int>(2){3,4};
					strokeKey[8] = new List<int>(2){2,4};
					strokeKey[9] = new List<int>(2){1,4};
					strokeKey[10] = new List<int>(2){1,3};
					strokeKey[11] = new List<int>(2){1,2};
					strokeKey[12] = new List<int>(2){1,1};
					break;
				case "Circle":
					strokeKey[0] = new List<int>(2){0,3};
					strokeKey[1] = new List<int>(2){1,2};
					strokeKey[2] = new List<int>(2){2,2};
					strokeKey[3] = new List<int>(2){3,3};
					strokeKey[4] = new List<int>(2){3,4};
					strokeKey[5] = new List<int>(2){2,5};
					strokeKey[6] = new List<int>(2){1,5};
					strokeKey[7] = new List<int>(2){0,4};
					strokeKey[8] = new List<int>(2){0,3};
					break;
				case "Diamond":
					strokeKey[0] = new List<int>(2){0,3};
					strokeKey[1] = new List<int>(2){1,4};
					strokeKey[2] = new List<int>(2){2,5};
					strokeKey[3] = new List<int>(2){3,4};
					strokeKey[4] = new List<int>(2){4,3};
					strokeKey[5] = new List<int>(2){3,2};
					strokeKey[6] = new List<int>(2){2,1};
					strokeKey[7] = new List<int>(2){1,2};
					strokeKey[8] = new List<int>(2){0,3};
					break;
				case "HourGlass":
					strokeKey[0] = new List<int>(2){0,1};
					strokeKey[1] = new List<int>(2){1,2};
					strokeKey[2] = new List<int>(2){2,3};
					strokeKey[3] = new List<int>(2){3,4};
					strokeKey[4] = new List<int>(2){4,5};
					strokeKey[5] = new List<int>(2){4,4};
					strokeKey[6] = new List<int>(2){4,3};
					strokeKey[7] = new List<int>(2){4,2};
					strokeKey[8] = new List<int>(2){4,1};
					strokeKey[9] = new List<int>(2){3,2};
					strokeKey[10] = new List<int>(2){2,3};
					strokeKey[11] = new List<int>(2){1,4};
					strokeKey[12] = new List<int>(2){0,5};
					strokeKey[13] = new List<int>(2){0,4};
					strokeKey[14] = new List<int>(2){0,3};
					strokeKey[15] = new List<int>(2){0,2};
					strokeKey[16] = new List<int>(2){0,1};
					break;
				case "LeftBottomTriangle":
					strokeKey[0] = new List<int>(2){0,0};
					strokeKey[1] = new List<int>(2){1,1};
					strokeKey[2] = new List<int>(2){2,2};
					strokeKey[3] = new List<int>(2){3,3};
					strokeKey[4] = new List<int>(2){4,4};
					strokeKey[5] = new List<int>(2){4,3};
					strokeKey[6] = new List<int>(2){4,2};
					strokeKey[7] = new List<int>(2){4,1};
					strokeKey[8] = new List<int>(2){4,0};
					strokeKey[9] = new List<int>(2){3,0};
					strokeKey[10] = new List<int>(2){2,0};
					strokeKey[11] = new List<int>(2){1,0};
					strokeKey[12] = new List<int>(2){0,0};
					break;
				case "RightBottomTriangle":
					strokeKey[0] = new List<int>(2){0,5};
					strokeKey[1] = new List<int>(2){1,4};
					strokeKey[2] = new List<int>(2){2,3};
					strokeKey[3] = new List<int>(2){3,2};
					strokeKey[4] = new List<int>(2){4,1};
					strokeKey[5] = new List<int>(2){4,2};
					strokeKey[6] = new List<int>(2){4,3};
					strokeKey[7] = new List<int>(2){4,4};
					strokeKey[8] = new List<int>(2){4,5};
					strokeKey[9] = new List<int>(2){3,5};
					strokeKey[10] = new List<int>(2){2,5};
					strokeKey[11] = new List<int>(2){1,5};
					strokeKey[12] = new List<int>(2){0,5};
					break;
				case "LeftTopTriangle":
					strokeKey[0] = new List<int>(2){0,0};
					strokeKey[1] = new List<int>(2){1,0};
					strokeKey[2] = new List<int>(2){2,0};
					strokeKey[3] = new List<int>(2){3,0};
					strokeKey[4] = new List<int>(2){4,0};
					strokeKey[5] = new List<int>(2){3,1};
					strokeKey[6] = new List<int>(2){2,2};
					strokeKey[7] = new List<int>(2){1,3};
					strokeKey[8] = new List<int>(2){0,4};
					strokeKey[9] = new List<int>(2){0,3};
					strokeKey[10] = new List<int>(2){0,2};
					strokeKey[11] = new List<int>(2){0,1};
					strokeKey[12] = new List<int>(2){0,0};
					break;
				case "RightTopTriangle":
					strokeKey[0] = new List<int>(2){0,5};
					strokeKey[1] = new List<int>(2){1,5};
					strokeKey[2] = new List<int>(2){2,5};
					strokeKey[3] = new List<int>(2){3,5};
					strokeKey[4] = new List<int>(2){4,5};
					strokeKey[5] = new List<int>(2){3,4};
					strokeKey[6] = new List<int>(2){2,3};
					strokeKey[7] = new List<int>(2){1,2};
					strokeKey[8] = new List<int>(2){0,1};
					strokeKey[9] = new List<int>(2){0,2};
					strokeKey[10] = new List<int>(2){0,3};
					strokeKey[11] = new List<int>(2){0,4};
					strokeKey[12] = new List<int>(2){0,5};
					break;
				case "LeftPointTriangle":
					strokeKey[0] = new List<int>(2){2,0};
					strokeKey[1] = new List<int>(2){1,1};
					strokeKey[2] = new List<int>(2){0,2};
					strokeKey[3] = new List<int>(2){1,2};
					strokeKey[4] = new List<int>(2){2,2};
					strokeKey[5] = new List<int>(2){3,2};
					strokeKey[6] = new List<int>(2){4,2};
					strokeKey[7] = new List<int>(2){3,1};
					strokeKey[8] = new List<int>(2){2,0};
					break;
				case "RightPointTriangle":
					strokeKey[0] = new List<int>(2){2,5};
					strokeKey[1] = new List<int>(2){1,4};
					strokeKey[2] = new List<int>(2){0,3};
					strokeKey[3] = new List<int>(2){1,3};
					strokeKey[4] = new List<int>(2){2,3};
					strokeKey[5] = new List<int>(2){3,3};
					strokeKey[6] = new List<int>(2){4,3};
					strokeKey[7] = new List<int>(2){3,4};
					strokeKey[8] = new List<int>(2){2,5};
					break;
			}

			Point[] points = GetPointsFromDrawPoints(GetActivePathObjects(strokeKey),0);
			Gesture currentGesture = new Gesture(points, currentType);
			_sigilSets.Add(currentGesture);
			_gesturePathSets[currentType] =  strokeKey;
		}

		Debug.Log(_sigilSets.Count);
	}

	void GenerateStrokes()
	{
		Dictionary<int,List<int>> strokeKey = new Dictionary<int, List<int>>(9);
		GameObject strokeOrder = GetStrokeKeyFromGOName("Prinny_1");

		SetActiveStroke(strokeOrder);
		Sprite prinnyArt = strokeOrder.GetComponent<SpriteRenderer>().sprite;
		List<GameObject> pointList = new List<GameObject>(9);

		strokeKey[0] = new List<int>(2){0,0};
		strokeKey [1] = new List<int>(2){1,1};
		strokeKey[2] = new List<int>(2){2,2};
		strokeKey [3] = new List<int>(2){3,3};
		strokeKey[4] = new List<int>(2){4,4};
		strokeKey [5] = new List<int>(2){4,3};
		strokeKey[6] = new List<int>(2){4,2};
		strokeKey [7] = new List<int>(2){4,1};
		strokeKey [8] = new List<int>(2){4,0};

		for(int i = 0; i < strokeKey.Keys.Count; ++i)
		{
			List<int> currentList = strokeKey[i];
			int row = currentList[0];
			int point = currentList[1];

			Debug.Log(_gridRows[row][point].name + " is the grid point in column# " + _gridRows[row][point].transform.parent.name.ToString());
			GameObject go = GetGridPointBasedOnRowAndID(row,point);

			go.GetComponent<SpriteRenderer>().sprite = prinnyArt;
			pointList.Add(go);
		}
		Point[] points = GetPointsFromDrawPoints(pointList,0);
		Gesture strokeOne = new Gesture (points, "UpperLeftToLowerRightToLowerLeft");
		_sigilSets.Add(strokeOne);
		_gesturePathSets["UpperLeftToLowerRightToLowerLeft"] =  strokeKey;
		ChangeGridObjects();
	}

	void RandomizeCurrentGestureForEval()
	{
		int rangeTop = _sigilSets.Count - 1;
		int randomNum = UnityEngine.Random.Range(0, rangeTop);

		while(_activeGesture == _sigilSets[randomNum].Name)
		{
			Debug.Log("Is same, try again");
			randomNum = UnityEngine.Random.Range (0, rangeTop);
		}
		Debug.Log(randomNum.ToString());
		AssignGesture(randomNum);		
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
		_input.OnSwipeDrag += HandleOnSwipeDrag;
		_input.OnSwipeEnd += HandleOnSwipeEnd;
		_input.OnLinesClear += HandleOnLinesClear;
	}

	void HandleOnLinesClear ()
	{
		_glowers = _touchedPoints.Values.ToList();
		foreach(GameObject glower in _glowers)
		{
			Destroy(glower);
		}
		_glowers.Clear();
		_touchedPoints.Clear();
		_activeStrokeKey.GetComponent<AnimationManager>().ResetState();
	}

	void HandleOnSwipeEnd(Vector3 screenPoint)
	{
		RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(screenPoint), Vector2.zero);
		if(DidIHitADrawPoint(hit))
		{
			DrawGlow(hit.transform.gameObject);
		}
		StartCoroutine(EvaluateGesture());
		
	}

	void HandleOnSwipeDrag(Vector3 screenPoint)
	{
		RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(screenPoint), Vector2.zero);
		if(DidIHitADrawPoint(hit))
		{
			DrawGlow(hit.transform.gameObject);
		}

	}

	void HandleOnSwipeStart(Vector3 screenPoint)
	{
		_points.Clear();
		if(!_successiveStrokes)
		{
			_strokeNumber = 0;
			Debug.Log("Is stroke number: " + _strokeNumber.ToString());
		}
		else
		{
			++_strokeNumber;
			Debug.Log("Is stroke number: " + _strokeNumber.ToString());
		}

		RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(screenPoint), Vector2.zero);
		if(DidIHitADrawPoint(hit))
		{
			DrawGlow(hit.transform.gameObject);
		}

		if((screenPoint.x <= (_bounds.width * 0.1f)) && (screenPoint.y <= (_bounds.height - 5.0f)))
		{
			Debug.Log("Call the quit to menu button");
			StoreStartPoint(screenPoint);
			_input.OnEscapeGesture += HandleOnEscapeGesture;
		}
//		Debug.Log("Touch Started at " + screenPoint.ToString());
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

	void DrawGlow(GameObject gameObject)
	{
		if(!_touchedPoints.ContainsKey(gameObject))
		{
			Vector3 drawSpot = gameObject.transform.position;
			Vector3 drawPoint = new Vector3(drawSpot.x, drawSpot.y, (drawSpot.z + 1.0f));
			GameObject newGlow = Instantiate(_glowing, drawPoint, gameObject.transform.rotation) as GameObject;
			newGlow.transform.localScale = new Vector3(3.0f,3.0f);
			_touchedPoints[gameObject] = newGlow;
			if(DoesDrawPointArtMatchKey(gameObject))
			{
				Point newOne = GetPointFromDrawPoint(gameObject, _strokeNumber);
				_points.Add(newOne);
			}
		}
	}

	IEnumerator EvaluateGesture()
	{
		if(isCorrectSwipe(EvaluateSwipe(_points)))
		{
			Debug.Log("Success, you did the right swipe!");
			_activeStrokeKey.GetComponent<AnimationManager>().CorrectGestureState(true);
		}
		_activeStrokeKey.GetComponent<AnimationManager>().EvaluateAnimationState(true);
		yield return new WaitForSeconds(1.5f);
		_countDownTime = 0.75f;
		_strokeCountDownActive = true;
		_activeStrokeKey.GetComponent<AnimationManager>().EvaluateAnimationState(false);
		_activeStrokeKey.GetComponent<AnimationManager>().CorrectGestureState(false);
		yield return new WaitForSeconds(1.0f);
		GetNewGesture();
	}

	void GetNewGesture ()
	{
		int current = GetActiveStrokeObjectIndex(_activeStrokeKey);
		int newKey = UnityEngine.Random.Range (0,(_strokeKeys.Count - 1));
		while(current == newKey)
		{
			newKey = UnityEngine.Random.Range (0,(_strokeKeys.Count - 1));
		}
		ChangeActiveStrokeObject(newKey);
		RandomizeCurrentGestureForEval();
	}
	
	private bool DidIHitADrawPoint(RaycastHit2D rayCast)
	{
		if(rayCast)
		{
//			if(rayCast.transform.tag == "DrawPoint")
//			{
//				return true;
//			}
			if(rayCast.transform.tag == "GridPoint")
			{
				return true;
			}
		}
		return false;
	}

	private bool DoesDrawPointArtMatchKey(GameObject gridPoint)
	{
		GridPointSpriteManager spritemanager = gridPoint.GetComponent<GridPointSpriteManager>();
		int gridPointState = spritemanager.GetCurrentArtState();
		int currentKeyState = GetActiveStrokeObjectIndex(_activeStrokeKey);

		if(gridPointState == currentKeyState)
		{
			return true;
		}

		return false;
	}

	private Gesture GetGestureFromStringName(string gestureName)
	{
		Gesture returnGesture = null;
		foreach(Gesture gesture in _sigilSets)
		{
			if(gesture.Name == gestureName)
			{
				returnGesture = gesture;
				break;
			}
		}

		return returnGesture;
	}

	//
	private string EvaluateDirection(Point[] pointsFromSwipe)
	{
		string direction = string.Empty;
		Point firstPoint = pointsFromSwipe[0];
		bool fromLeft = false;
		bool fromTop = false;
		Point lastPoint = pointsFromSwipe[pointsFromSwipe.Length - 1];
		float xDiff = 0.0f;
		float yDiff = 0.0f;

		if(firstPoint.X > lastPoint.X)
		{
			xDiff = firstPoint.X - lastPoint.X;
		}
		else 
		{
			xDiff =  lastPoint.X - firstPoint.X;
			fromLeft = true;
		}

		if(firstPoint.Y > lastPoint.Y)
		{
			yDiff = firstPoint.Y - lastPoint.Y;
			fromTop = true;
		}
		else
		{
			yDiff = lastPoint.Y - firstPoint.Y;
		}

		if((xDiff > 0.5f) && (yDiff <= 0.5f))
		{
			string vector = "FromRight";
			if(fromLeft)
			{
				vector = "FromLeft";
			}

			direction = vector;
		}
		else if((yDiff > 0.5f) && (xDiff <= 0.5f))
		{
			string vector = "FromBottom";
			if(fromTop)
			{
				vector = "FromTop";
			}

			direction = vector;
		}
		else if((xDiff > 0.5f) && (yDiff > 0.5f))
		{
			string vertical;
			string horizontal;

			if(fromLeft)
			{
				horizontal = "FromLeft";
			}
			else 
			{
				horizontal = "FromRight";
			}

			if(fromTop)
			{
				vertical = "FromTop";
			}
			else
			{
				vertical = "FromBottom";
			}

			direction = horizontal + vertical;
		}

		return direction;
	}

	private string EvaluateSwipe (List<Point> pointsFromSwipe)
	{
		string gestureEvaluation = string.Empty;

		if(pointsFromSwipe.Count > 1)
		{
			Point[] points = pointsFromSwipe.ToArray();

			Gesture completedGesture = new Gesture(points);
			Gesture[] sigils = _sigilSets.ToArray();

			gestureEvaluation = PointCloudRecognizer.Classify(completedGesture, sigils);
			UpdateDebugLabel(gestureEvaluation + " eval gesture");
		}

		return gestureEvaluation;
	}

	private bool isCorrectSwipe(string evaluation)
	{
		if(evaluation == _activeGesture)
		{
			return true;
		}
		else
		{
			return false;
		}
	}
}

