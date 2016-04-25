using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

namespace Voltage.Witches.Components
{
	using Voltage.Witches.Controllers;
	using Voltage.Common.DebugTool.Timer;

    public struct TraceData
    {
        public List<Vector3> points;
        //public Vector3[] points;
        public string name;
    }

	public class TracingManager : MonoBehaviour 
	{
		public TracingScreenController Controller { get; protected set; }
		public iGUISmartPrefab_MiniGameUI UIScreen { get; protected set; }

		public InputHandler input { get; protected set; }
//		public TouchHandler touch { get; protected set; }
		public MouseHandler mouse { get; protected set; }
		public Rect bounds { get; protected set; }

		public ICamera MiniGameCamera { get; protected set; }

		public Text DisplayMessage = null;
		public Text CurrentStroke = null;

		public Image Guide = null;
		
        public GameObject touchReference;
		private GameObject _onScreenRef;
		private GameObject _canvas;
		private GameObject _currentRendererObject;

		private List<GameObject> _guideObjects;

		private bool _shouldReset = false;

		
		public Color32 TrueValue;
		public Color32 FalseValue;
		public float StartingLeniency;
        private int _numberOfStrokeInRune;
		private List<float> _accuracyPerStroke;
		private int _targetStroke;
		private int _currentRune;
        private List<int> _strokeOrder;

        private float _timeLeft = 0f;

		public bool InPlay { get; protected set; }
		public bool IsPaused { get; protected set; }

		private DisplayHighlighter _highlighter;
		private IEnumerator _evaluation;
		[SerializeField]
		private bool _useEditorValues = false;
		public bool IsUsingEditorValues { get { return _useEditorValues; } }
		[SerializeField]
		private float _timePerStroke;
		private IEnumerator _timer;
		[SerializeField]
		private List<float> _baseValues = new List<float>() { 0.25f,0.5f,0.75f };
		[SerializeField]
		private int _maxScore = 12000;
		[SerializeField]
        private List<int> _runesPerDifficulty = new List<int>(){ 3,3,3,3,3 };
		[SerializeField]
		private List<int> _scoreValues = new List<int>(){ 0,200,400,600,800,900,1000 };
		[SerializeField]
		private List<float> _scoreEvaluationMappingValues = new List<float>{ 0.49f,0.59f,0.69f,0.79f,0.89f,0.95f,1f };

		private Dictionary<string,int> _feedbackScores = new Dictionary<string, int>();
		private static List<float> _speedsForProducers = new List<float>{ 2f,1.75f,1.5f,1.25f,1f };

		private RuneRepo _runeRepo;
		private List<RuneObject> _runeSet;
		private GameObject _currentRuneObject;
		private List<Color> _strokeColors;
		private List<int> _strokeOrdering;
        private GameObject _currentTraceObject;

		private ScoreManager _scoreManager;
		private IEnumerator _scoreDisplay;
		bool _resultsDisplay = false;
		Action _screenElementsLoaded;

		private Vector2 _screenPos;
        private float _runeWidth = 0f;
        private float _runeHeight = 0f;
        private float _recWidth = 0f;
        private float _recHeight = 0f;
        private List<Vector3> _screenPoints = new List<Vector3>();
        private string _runeStrokeName = "";
        private List<string> _runeStrokeList = new List<string>();

        private List<TraceData> _tracePoints = new List<TraceData>();

        public bool autoPlay = false;
        private bool _playStarted = false;
        private bool _playDone = false;
        private bool _playing = false;
        private int _playIndex = -1;
        private int _traceIndex = 0;

        public void SetController(TracingScreenController controller,Action screenLoaded)
		{
			_screenElementsLoaded = screenLoaded;
			Controller = controller;
			SetUpScoreManager();
		}

		public void SetUI(iGUISmartPrefab_MiniGameUI uiScreen)
		{
			UIScreen = uiScreen;
		}

		public void Pause()
		{
			if(!IsPaused)
			{
				IsPaused = true;
			}
			if(input.IsActive)
			{
				input.Deactivate();
			}
		}

		public void Resume()
		{
			if(IsPaused)
			{
				IsPaused = false;
			}
			if(!input.IsActive)
			{
				input.Activate();
			}
		}

		protected void Awake()
		{
			AmbientDebugTimer.Current.Start("Caching some Prefabs");
			AmbientDebugTimer.Current.Stop();
			AmbientDebugTimer.Current.Start("Finding Canvas Object");
			GetCanvas();

			AmbientDebugTimer.Current.Stop();

            TextAsset[] traceTexts = Resources.LoadAll<TextAsset>("Tracing");
            for (int i = 0; i < traceTexts.Length; i++)
            {
                TraceData tempData = new TraceData();
                tempData.name = traceTexts[i].name;
                string[] textArray = traceTexts[i].text.Split('\n');
                tempData.points = new List<Vector3>();
          
                // Get the recorded postion.  Commented out cause doesn't look like we need it.
//                string[] vecCord = textArray[0].Split(',');
//                int recPosX = int.Parse(vecCord[0]);
//                int recPosY = int.Parse(vecCord[1]);
                // Get the recorded width/height.
                string[] vecCord = textArray[1].Split(',');
                _recWidth = int.Parse(vecCord[0]);
                _recHeight = int.Parse(vecCord[1]);
                int count = textArray.Length;
                float lastx = -1;
                float lasty = -1;

                for (int t = 2; t < count-1; t++)
                {
                    vecCord = textArray[t].Split(',');
                    float x = float.Parse(vecCord[0]);
                    float y = float.Parse(vecCord[1]);
                    if ((lastx != x) || (lasty != y))
                    {
                        Vector3 point = new Vector3(x, y, 0f);
                        tempData.points.Add(point);
                        lastx = x;
                        lasty = y;
                    }
                }
                _tracePoints.Add(tempData);
            }

			MiniGameCamera = gameObject.GetComponentInChildren<CameraEnabler>();
		}

        private Vector3 ScaleRecordPoints(Vector3 point)
        {
            float xFactor = _runeWidth/_recWidth;
            float yFactor = _runeHeight/_recHeight;
            point.x = (point.x * xFactor) + _screenPos.x;
            point.y = (point.y * yFactor) + _screenPos.y;
            return point;
        }

        private void AutoPlay()
        {
            if (_playStarted)
            {
                if (_playDone)
                {
                    _playing = false;
                    _playDone = false;
                }
                else if (_playing)
                {
                    if (_traceIndex < _tracePoints[_playIndex].points.Count)
                    {
                        Vector3 simInput = _tracePoints[_playIndex].points[_traceIndex];
                        simInput = ScaleRecordPoints(simInput);
                        _traceIndex++;
                        HandleOnSwipeDrag(simInput);
                    }
                    else if (_traceIndex == _tracePoints[_playIndex].points.Count)
                    {
                        Vector3 simInput = _tracePoints[_playIndex].points[_traceIndex-1];
                        simInput = ScaleRecordPoints(simInput);
                        HandleOnSwipeEnd(simInput);
                        _traceIndex++;
                    }
                }
                else
                {
                    // Do start
                    for(int i = 0; i < _tracePoints.Count; i++)
                    {
                        if (_tracePoints[i].name == _runeStrokeName)
                        {
                            _playIndex = i;
                            _traceIndex = 1;
                            Vector3 simInput = _tracePoints[i].points[0];
                            simInput = ScaleRecordPoints(simInput);
                            HandleOnSwipeStart(simInput);
                            _playing = true;
                            break;
                        }
                    }
                    if (!_playing)
                    {
                        Debug.LogWarning("Couldn't find rune stroke : " + _runeStrokeName);
                    }
                }
            }
        }

        public void Update()
        {
            if (autoPlay)
            {
                AutoPlay();
            }
        }

		void DisplayScreenInfo()
		{
			var dpi = Screen.dpi;
			var resolution = Screen.currentResolution;

			Debug.LogWarning("HEIGHT :: " + resolution.height.ToString ());
			Debug.LogWarning("WIDTH :: " + resolution.width.ToString ());
			Debug.LogWarning("REFRESH RATE :: " + resolution.refreshRate.ToString ());
			Debug.LogWarning("DPI :: " + dpi.ToString ());
		}

		public void OnDestroy()
		{
			Debug.Log ("We are destroyed!!!!");

			_runeRepo.UnloadResources ();
			_runeRepo = null;
		}

		protected void Start()
		{
			StartCoroutine(InitializingElements());
		}

		IEnumerator InitializingElements()
		{
			AmbientDebugTimer.Current.Start("TracingManager::InitializingElements >>> assignment");

			_guideObjects = new List<GameObject>();

			_runeRepo = new RuneRepo();

			yield return new WaitForEndOfFrame();

			AmbientDebugTimer.Current.Start("TracingManager::InitializingElements >>> Setup");

			InPlay = false;
			IsPaused = false;
			SetUpBounds();
			AddHandlers();
			SetBounds();
			EnableCallBacks();
			#if !UNITY_EDITOR
			DisplayMessage.gameObject.GetComponent<CanvasRenderer>().SetAlpha(0f);
			CurrentStroke.gameObject.GetComponent<CanvasRenderer>().SetAlpha(0f);
			#endif
			yield return new WaitForEndOfFrame();

			AmbientDebugTimer.Current.Start("TracingManager::InitializingElements >>> Load Screen Elements");

			if(_screenElementsLoaded != null)
			{
				_screenElementsLoaded();
			}

			AmbientDebugTimer.Current.Stop();
			yield return null;
		}

		void GetCanvas()
		{
			var canvas = transform.FindChild("ReferenceCanvas").gameObject;
			if (canvas != null)
			{
				_canvas = canvas;
			}
		}
		
		void SetUpBounds()
		{
			var height = Screen.height;
			var width = Screen.width;
			
			bounds = new Rect(0f,0f,width,height);
		}
		
		void AddHandlers()
		{
			input = gameObject.AddComponent<InputHandler>();
//			touch = gameObject.AddComponent<TouchHandler>();
			mouse = gameObject.AddComponent<MouseHandler>();

//			touch.AcceptInterface(input);
			mouse.AcceptInterface(input);
		}
		
		void SetBounds ()
		{
//			touch.SetBoundaries(bounds);
			mouse.SetBoundaries(bounds);
		}
		
		void EnableCallBacks()
		{
			input.OnSwipeStart += HandleOnSwipeStart;
			input.OnSwipeDrag += HandleOnSwipeDrag;
			input.OnSwipeEnd += HandleOnSwipeEnd;
		}

		void DisplayButtons(bool value)
		{
			#if UNITY_EDITOR
			if(Guide != null)
			{
				Guide.gameObject.SetActive(value);
			}
			#else
			//DO NOTHING
			#endif
		}
		
		void SetUpScoreManager()
		{
			var data = Controller.GameData;
			var scoreScalars = _baseValues;

			int? maxScore = null;
			if(Controller != null)
			{
				scoreScalars = Controller.GetRecipeTargetScoreScalars();
			}
			_feedbackScores = data.FeedbackScores;

			_scoreManager = new ScoreManager(maxScore,scoreScalars,_feedbackScores);
		}
		
		void CreateReferencePoint(Vector3 worldPoint)
		{
			if(_onScreenRef == null)
			{
                _onScreenRef = Instantiate(touchReference,worldPoint,Quaternion.identity) as GameObject;
				_onScreenRef.name = "TOUCH_REFERENCE";
				_onScreenRef.transform.SetParent(gameObject.transform);
				_onScreenRef.GetComponent<Renderer>().sortingOrder = _canvas.GetComponent<Renderer>().sortingOrder + 15;
				_onScreenRef.GetComponent<Renderer>().sortingLayerName = _canvas.GetComponent<Renderer>().sortingLayerName;
				_onScreenRef.GetComponent<Renderer>().sortingLayerID = _canvas.GetComponent<Renderer>().sortingLayerID;

				var particleObject = _onScreenRef.GetComponentInChildren<ParticleSystem>().gameObject;
				particleObject.GetComponent<Renderer>().sortingLayerName = _onScreenRef.GetComponent<Renderer>().sortingLayerName;
				particleObject.GetComponent<Renderer>().sortingOrder = _onScreenRef.GetComponent<Renderer>().sortingOrder;

				var trail = _onScreenRef.GetComponent<TrailRenderer>();
				trail.enabled = true;
			}
		}
		
        private void ProcessTrace()
        {
            if (_onScreenRef != null)
            {
                KillRef();
            }
            if ((_evaluation == null) && (_scoreDisplay == null))
            {
                _playStarted = false;
                EvaluateTracing();
            }
        }

		void DrawCurrentAndScore()
		{	
            ProcessTrace();

			if(InPlay)
			{
				if(!IsPaused)
				{
					if(_evaluation != null)
					{
						KillCoroutine(_evaluation);
					}
                    _evaluation = WaitToEvaluate(null);
                    StartCoroutine(_evaluation);
				}
			}
		}

		void EvaluateTracing()
		{

            // Code to record our refernce data.
//            using (StreamWriter writer = new StreamWriter("Assets/Resources/Tracing/" + _runeStrokeName + ".txt"))
//            {
//                // Save the rune position on the screen at recording.
//                string appendString = (int) _screenPos.x + "," + (int) _screenPos.y;
//                writer.WriteLine(appendString);
//
//                // Save the rune size at time of recording
//                appendString = (int)_runeWidth + "," + (int)_runeHeight;
//                writer.WriteLine(appendString);
//                foreach (Vector3 p in _screenPoints)
//                {
//                    appendString = (int)(p.x-_screenPos.x) + "," + (int)(p.y-_screenPos.y);
//                    writer.WriteLine(appendString);
//                }
//            }

			if(InPlay)
			{
				if(!IsPaused)
				{
					if(_evaluation != null)
					{
						KillCoroutine(_evaluation);
					}
                    _evaluation = WaitToEvaluate(null);
                    StartCoroutine(_evaluation);
				}
			}
			else
			{
				Debug.LogWarning("Is not in play....");
                _evaluation = WaitToEvaluate(null);
                StartCoroutine(_evaluation);
			}
		}

		IEnumerator WaitToEvaluate(LineRenderer renderer)
		{
			yield return new WaitForEndOfFrame();
			
            CompareRuneToTrace();
            yield return new WaitForEndOfFrame();

			while(IsPaused)
			{
				yield return new WaitForEndOfFrame();
			}

			yield return new WaitForEndOfFrame();

			if((InPlay))
			{
				if(_timer != null)
				{
					StopCoroutine(_timer);
					_timer = null;
				}

				if((_shouldReset) && (IsLastRune()))
				{
					Debug.LogWarning("Should Reset");
				}
			}

			if(_scoreDisplay != null)
			{
				_evaluation = null;
			}
			yield return null;
			_evaluation = null;
            _playDone = true;
            _playStarted = false;
		}

		bool IsLastRune()
		{
			return ((_currentRune + 1) == (_runeSet.Count));
		}
		
		void MoveRef(Vector3 worldPoint)
		{
			if(_onScreenRef != null)
			{
				_onScreenRef.transform.position = worldPoint;
			}
		}
		
		void KillRef()
		{
			if(_onScreenRef != null)
			{
				Destroy(_onScreenRef);
				_onScreenRef = null;
			}
		}
		
        private void AddTracePoints(Vector3 point)
        {
            Vector3 nPoint = new Vector3();
            nPoint.x = (float) ((int)point.x);
            nPoint.y = (float) ((int)point.y);
            nPoint.z = 0f;

//            Debug.LogWarning("Input Received Position  = " + point);

            if (_screenPoints.Count > 0)
            {
                if ((_screenPoints[_screenPoints.Count-1].x != nPoint.x) || (_screenPoints[_screenPoints.Count-1].y != nPoint.y))
                {
                    _screenPoints.Add(nPoint);
                }
            }
            else
            {
                _screenPoints.Add(nPoint);
            }

        }
        
        void HandleOnSwipeStart(Vector3 screenPoint)
        {
            if (_playStarted)
            {
                _screenPoints.Clear();
                AddTracePoints(screenPoint);
                var adjusted = Camera.main.ScreenToWorldPoint(screenPoint);
                adjusted.z = 0f;
                CreateReferencePoint(adjusted);
            }
		}
		
		void HandleOnSwipeDrag(Vector3 screenPoint)
		{
            if (_playStarted)
			{
                AddTracePoints(screenPoint);
                var adjusted = Camera.main.ScreenToWorldPoint(screenPoint);
				adjusted.z = 0f;
				MoveRef(adjusted);
			}
		}
		
		void HandleOnSwipeEnd(Vector3 screenPoint)
		{
            if (_playStarted)
            {
                AddTracePoints(screenPoint);
                ProcessTrace();
            }
		}
		
		bool ShouldGoToNextStroke()
		{
			return ((_targetStroke + 1) <= (_numberOfStrokeInRune - 1));
		}

		bool ShouldGoToNextRune()
		{
			return (_currentRune < (_runeSet.Count - 1));
		}

		IEnumerator DisplayScores(KeyValuePair<string,int> scoreLabel)
		{
			if(UIScreen != null)
			{
				var score = _scoreManager.Current_Score;
				var height = (float)_scoreManager.Current_Score / (float)_scoreManager.Max_Score;

				UIScreen.UpdateScore(score,height);
				UIScreen.DisplayScoreOnScreen(scoreLabel);
			}
			yield return new WaitForSeconds(1f);

			while(IsPaused)
			{
				yield return new WaitForEndOfFrame();
			}

			if(ShouldGoToNextStroke())
			{
				while((IsPaused))
				{
					Debug.LogWarning("Waiting.....for evaluation to go to next stroke");
					yield return new WaitForEndOfFrame();
				}

				StartCoroutine(GoToNextStroke());
			}
			else if(ShouldGoToNextRune())
			{
				while((IsPaused))
				{
					Debug.LogWarning("Waiting.....for evaluation to go to next rune");
					yield return new WaitForEndOfFrame();
				}

				DisplayFinalScores();
				yield return new WaitForEndOfFrame();
				ResetCurrentRune();
				yield return new WaitForEndOfFrame();

				StartCoroutine(GoToNextRune());
			}
			else
			{
				Debug.LogWarning("RESET!!!!!!!!!!!!!RESET!!!!!!!!!!!!!");
				yield return new WaitForEndOfFrame();
				_shouldReset = true;
				if(_evaluation == null)
				{
					if(!_resultsDisplay)
					{
						CompleteMiniGame();
					}
				}
			}

			_scoreDisplay = null;
		}

		IEnumerator GoToNextStroke()
		{
			if(_guideObjects.Count > 0)
			{
				for(int i = 0; i < _guideObjects.Count; ++i)
				{
					Destroy(_guideObjects[i]);
				}
				_guideObjects.Clear();
			}
			if((UIScreen != null) && (UIScreen.isDisplayingScore))
			{
				UIScreen.StopScoreDisplay();
				yield return new WaitForEndOfFrame();
			}
			float delay = (UIScreen != null) ? UIScreen.DelayNextTime : 2f;
			yield return new WaitForSeconds(delay);
            _targetStroke++;
			yield return new WaitForEndOfFrame();
			UpdateStroke(_strokeOrder[_targetStroke]);
		}

		IEnumerator GoToNextRune()
		{
			++_currentRune;
			if((UIScreen != null) && (UIScreen.isDisplayingScore))
			{
				UIScreen.StopScoreDisplay();
				yield return new WaitForEndOfFrame();
			}

			float delay = (UIScreen != null) ? UIScreen.DelayNextTime : 2f;
			yield return new WaitForSeconds(delay);
			ClearCurrentRuneObject();
			yield return new WaitForEndOfFrame();
			DisplayRune(_runeSet[_currentRune]);
            BuildStrokeList();
            yield return new WaitForEndOfFrame();
            UpdateStroke(_strokeOrder[_targetStroke]);
        }

		void ClearCurrentRuneObject()
		{
			Destroy(_currentRuneObject);
			_currentRuneObject = null;
		}

		void KillCoroutine(IEnumerator routine)
		{
			if(routine != null)
			{
				StopCoroutine(routine);
			}
		}

		void ResetSettings()
		{
			_scoreManager = null;
			int? max = _maxScore;
			Dictionary<string,int> mappings = new Dictionary<string,int> ()
			{
				{"Not Quite",_scoreValues[0]},
				{"Nice",_scoreValues[1]},
				{"Not Bad",_scoreValues[2]},
				{"magical",_scoreValues[3]},
				{"Magical",_scoreValues[4]},
				{"perfect",_scoreValues[5]},
				{"Perfect",_scoreValues[6]}
			};
			_scoreManager = new ScoreManager(max, _baseValues, mappings);
			_scoreManager.UpdateScoreMapping(_scoreEvaluationMappingValues);
		}

        private void BuildStrokeList()
        {
            _targetStroke = 0;
            
            if (_strokeOrder == null)
            {
                _strokeOrder = new List<int>();
            }
            else
            {
                _strokeOrder.Clear();
            }
            int count = _runeStrokeList.Count;
            List<int> tempList = new List<int>();
            for (int i =0; i < count; i++)
            {
                tempList.Add(i);               
            }

            System.Random randGen = new System.Random();
            for (int i = 0; i < count; i++)
            {
                int itemRemain = tempList.Count;
                int randNumber = randGen.Next(itemRemain);
                _strokeOrder.Add(tempList[randNumber]);
                tempList.RemoveAt(randNumber);
            }
        }

		public void BeginMiniGame(int difficulty)
		{
            if (_useEditorValues)
            {
                ResetSettings();
            }
            if (UIScreen != null)
            {
                UIScreen.DisplayUI();
                UIScreen.SetScoreParams(_scoreManager.Target_Score, _scoreManager.Median_Score, _scoreManager.High_Score, _scoreManager.Max_Score);
            }
            var rating = (Difficulty)difficulty;
            SetSpeed(rating);
            _runeSet = GetRandomizedSet(rating);
            #if UNITY_EDITOR
            DisplayButtons(false);
            #endif
            InPlay = true;
            _accuracyPerStroke = new List<float>();
            _currentRune = 0;
            DisplayRune(_runeSet[_currentRune]);
            BuildStrokeList();
            UpdateStroke(_strokeOrder[_targetStroke]);
        }

		void SetSpeed(Difficulty rating)
		{
			//TODO Reverse the values on the difficulty conversion
			if(Controller == null)
			{
				_timePerStroke = 14f;
				return;
			}
			if(_useEditorValues)
			{
				return;
			}

			var data = Controller.GameData;
			var lookup = "minigame_speed_";

			switch(rating)
			{
			case Difficulty.EASY:
				lookup += "trouble";
				_timePerStroke = data.DifficultySpeeds[lookup];
				//HACK just for the producers
				_timePerStroke = _speedsForProducers[0];
				break;
			case Difficulty.NORMAL:
				lookup += "hard";
				_timePerStroke = data.DifficultySpeeds[lookup];
				//HACK just for the producers
				_timePerStroke = _speedsForProducers[1];
				break;
			case Difficulty.TRICKY:
				lookup += "tricky";
				_timePerStroke = data.DifficultySpeeds[lookup];
				//HACK just for the producers
				_timePerStroke = _speedsForProducers[2];
				break;
			case Difficulty.HARD:
				lookup += "normal";
				_timePerStroke = data.DifficultySpeeds[lookup];
				//HACK just for the producers
				_timePerStroke = _speedsForProducers[3];
				break;
			case Difficulty.TROUBLE:
				lookup += "easy";
				_timePerStroke = data.DifficultySpeeds[lookup];
				//HACK just for the producers
				_timePerStroke = _speedsForProducers[4];
				break;
			}
//            _timePerStroke = 500f;
		}

		void CompleteMiniGame()
		{
			if((UIScreen != null) && (Controller != null))
			{
				_resultsDisplay = true;
				Action<int> response = delegate(int answer) { HandleEnding(answer); };

				Controller.CompleteMiniGame((Voltage.Witches.Models.CompletionStage)_scoreManager.Tiers_Reached);	// cast can throw exception, should be some checks or better yet use enum consistently

				UIScreen.ShowResults(_scoreManager.Current_Score,_scoreManager.Tiers_Reached,response);
				DisplayFinalScores();
				ClearCurrentRuneObject();
			}
			else
			{
				DisplayFinalScores();
				ResetToTop();
			}
		}

		public void HandleEnding(int answer)
		{
			if(!_useEditorValues)
			{
				switch((PotionDialogResponse)answer)
				{
					case PotionDialogResponse.Back:
						Controller.MoveToPotionSelectScreen();
						break;
					case PotionDialogResponse.CheckMail:
						Controller.GoToMailBox();
						break;
					case PotionDialogResponse.PlayAgain:
						Controller.MoveToIngredientsSelectionScreen();
						break;
				}
				UIScreen.HandleResultsResponse(answer);
			}
			//TODO Change to editor only functionality
			ResetToTop();
		}

		List<RuneObject> GetRandomizedSet(Difficulty rating)
		{
            List<RuneObject> runeSet = new List<RuneObject>();
//            runeSet = _runeRepo.GetRandomizedRuneSetByNumber(12);
            switch(rating)
			{
			case Difficulty.EASY:
				runeSet = _runeRepo.GetRandomizedRuneSetByNumber(_runesPerDifficulty[0]);
				break;
			case Difficulty.NORMAL:
				runeSet = _runeRepo.GetRandomizedRuneSetByNumber(_runesPerDifficulty[1]);
				break;
			case Difficulty.TRICKY:
				runeSet = _runeRepo.GetRandomizedRuneSetByNumber(_runesPerDifficulty[2]);
				break;
			case Difficulty.HARD:
				runeSet = _runeRepo.GetRandomizedRuneSetByNumber(_runesPerDifficulty[3]);
				break;
			case Difficulty.TROUBLE:
				runeSet = _runeRepo.GetRandomizedRuneSetByNumber(_runesPerDifficulty[4]);
				break;
			}

            return runeSet;
		}

		void DisplayRune(RuneObject runeObject)
		{
			var allStrokes = runeObject.Stroke_Textures;
			var numberOfStrokes = runeObject.Stroke_Count;
            _runeStrokeList.Clear();
            _numberOfStrokeInRune = numberOfStrokes;

			for(int i = 0; i < numberOfStrokes; ++i)
			{
				Texture2D displayTexture = allStrokes[i];
				
                _runeStrokeList.Add(displayTexture.name);
			}

			DisplayRuneSprite(runeObject.Rune_Name);
        }

        public Rect BoundsToScreenRect(Bounds bounds)
        {
            // Get mesh origin and farthest extent (this works best with simple convex meshes)
            Vector3 origin = Camera.main.WorldToScreenPoint(new Vector3(bounds.min.x, bounds.max.y, 0f));
            Vector3 extent = Camera.main.WorldToScreenPoint(new Vector3(bounds.max.x, bounds.min.y, 0f));
            
            // Create rect in screen space and return - does not account for camera perspective
            return new Rect(origin.x, Screen.height - origin.y, extent.x - origin.x, origin.y - extent.y);
        }

		void DisplayRuneSprite(string runeName)
		{
			_currentRuneObject = new GameObject("Rune_Sprite",typeof(SpriteRenderer));
			_currentRuneObject.layer = 10;
			_currentRuneObject.GetComponent<Renderer>().sortingOrder = 5;
			_currentRuneObject.transform.position = new Vector3(0f,0.15f,-1f);
			_currentRuneObject.transform.localScale = new Vector3(0.5f,0.5f,1f);
			_currentRuneObject.transform.SetParent(gameObject.transform);
			var spriteRenderer = _currentRuneObject.GetComponent<SpriteRenderer>();
			spriteRenderer.sprite = _runeRepo.Available_Rune_Art[runeName];
            Rect screenRect = BoundsToScreenRect(_currentRuneObject.GetComponent<Renderer>().bounds);
			_runeWidth = (float)((int)screenRect.width);
			_runeHeight = (float)((int)screenRect.height);
            _screenPos = new Vector2(screenRect.x, screenRect.y);
		}

        const float STARTING_TRANSULENCY = 0.9f;

        void DisplayTraceSprite(string name)
        {
            string traceName = name;
            _currentTraceObject = new GameObject("Trace_Sprite",typeof(SpriteRenderer));
            _currentTraceObject.layer = 10;
            _currentTraceObject.GetComponent<Renderer>().sortingOrder = 6;
            _currentTraceObject.transform.position = new Vector3(0f,0.15f,-1f);
            _currentTraceObject.transform.localScale = new Vector3(0.5f,0.5f,1f);
            _currentTraceObject.transform.SetParent(gameObject.transform);
            var spriteRenderer = _currentTraceObject.GetComponent<SpriteRenderer>();
            spriteRenderer.sprite = _runeRepo.Available_Trace_Art[traceName];
            Color color = spriteRenderer.color;
            color.a = STARTING_TRANSULENCY;  // Magic number as a start based on production value.
            spriteRenderer.color = color;
        }
        

		void SetUpTimer()
		{
			if(UIScreen != null)
			{
				UIScreen.ShowTimer(_timePerStroke);
			}
			_timer = BeginTimer(_timePerStroke);
			StartCoroutine(_timer);
		}

        private void UpdateTraceState(float time)
        {
            // Dealing with animating the transparency
            if (_playStarted)
            {
                if (_currentTraceObject != null)
                {
                    var spriteRenderer = _currentTraceObject.GetComponent<SpriteRenderer>();
                    Color color = spriteRenderer.color;
                    float percent = time/_timePerStroke;
                    color.a = STARTING_TRANSULENCY * percent;
                    spriteRenderer.color = color;
                }
            }
        }

		IEnumerator BeginTimer(float time)
		{
			_timeLeft = time;
            while ((_evaluation != null) || (_scoreDisplay != null))
            {
                yield return null;
            }
            _playStarted = true;

			while(_timeLeft > 0f)
			{
				while(IsPaused)
				{
					yield return new WaitForFixedUpdate();
				}
				_timeLeft -= Time.deltaTime;
                UpdateTraceState(_timeLeft);
				if(UIScreen != null)
				{
                    UIScreen.UpdateGaugePosition(_timeLeft);
				}
				yield return new WaitForFixedUpdate();
			}

			DrawCurrentAndScore();

			Debug.Log("Time's up!");
			_timer = null;
		}

		public void UpdateStroke(int stroke)
		{
            if (_runeStrokeList.Count > stroke)
            {
                _runeStrokeName = _runeStrokeList[stroke];
            }

            if (_currentTraceObject != null)
            {
                Destroy(_currentTraceObject);
                _currentTraceObject = null;
            }

            DisplayTraceSprite(_runeStrokeName);

            var strokeDisplay = (_targetStroke + 1).ToString () + " / " + _runeStrokeList.Count.ToString ();
			CurrentStroke.text = (strokeDisplay);
			if(UIScreen != null)
			{
				UIScreen.UpdateStrokeCount(strokeDisplay);
			}
			if(_timer != null)
			{
				StopCoroutine(_timer);
				_timer = null;
			}
			if(_evaluation != null)
			{
				StopCoroutine(_evaluation);
			}
			SetUpTimer();
		}

		List<int> GetVisualIndexList(int stroke)
		{
			List<int> visualOrdering = new List<int>();

			Util.CircularArray<int> circArray = new Util.CircularArray<int>();

			circArray.RotateLeft((stroke + 1));

			return visualOrdering;
		}

		public void Dispose()
		{
			MiniGameCamera.Dispose();
			DestroyImmediate(this.gameObject);
		}

		void DisplayAccuracy(float accuracy)
		{
			if(DisplayMessage != null)
			{
				DisplayMessage.text = "ACCURACY: " + accuracy.ToString("P");
			}
		}

		void DisplayFinalScores()
		{
			float totalAccuracy = 0f;
			int totalStrokes = _numberOfStrokeInRune;
			for(int i = 0; i < _accuracyPerStroke.Count; ++i)
			{
				totalAccuracy += _accuracyPerStroke[i];
			}

			totalAccuracy = totalAccuracy / totalStrokes;

			if(DisplayMessage != null)
			{
				DisplayMessage.text = "TOTAL ACCURACY :: " + totalAccuracy.ToString("P");
			}
		}
		
		void StopDrawing(List<Vector3> pathPoints)
		{
			pathPoints.Clear();
			if(_currentRendererObject != null)
			{
				Destroy(_currentRendererObject);
			}
			_currentRendererObject = null;
		}

		void ResetCurrentRune()
		{
			CurrentStroke.text = string.Empty;
			_targetStroke = 0;
			_numberOfStrokeInRune = 0;
			_accuracyPerStroke.Clear();
		}

		void ResetToTop()
		{
			#if UNITY_EDITOR
			DisplayButtons(true);
			#endif
			InPlay = false;
			_resultsDisplay = false;
			if (_scoreManager != null) 
			{
				_scoreManager.ResetScore ();
			}
			if(UIScreen != null)
			{
				UIScreen.Progress_bar.setEnabled(false);
			}

			_currentRune = 0;
			_runeSet.Clear();

			if(CurrentStroke != null)
			{
				CurrentStroke.text = string.Empty;
			}
			_targetStroke = 0;
			_numberOfStrokeInRune = 0;
			_shouldReset = false;
			if(UIScreen != null)
			{
				UIScreen.HideUI();
			}
		}


		// exposing fields to allow accuracy adjustment in inspector
		public float PERFECT_PERCENT = 0.005f;
		public float MAGICAL_PERCENT = 0.010f;
		public float GOOD_PERCENT = 0.015f;
		public float OK_PERCENT = 0.05f;
		public float PERFECT_VALUE = 1f;
		public float MAGICAL_VALUE = 0.8f;
		public float GOOD_VALUE = 0.6f;
		public float OK_VALUE = 0.5f;

        public float CalculatePointCoverage(List<Vector3> list1, List<Vector3> list2)
        {
            if ((list1.Count == 0) || (list2.Count == 0))
            {
                return 0f;
            }
            // Magic number here are tunning the how many pixel off before we reduce the value of the accuracy.
            // It is factor base on the screen width.

            float perfect_zone = (Screen.width * PERFECT_PERCENT);
            perfect_zone *= perfect_zone; // square the distance
            float magical_zone = (Screen.width * MAGICAL_PERCENT);
            magical_zone *= magical_zone; // square the distance
            float good_zone = (Screen.width * GOOD_PERCENT);
            good_zone *= good_zone; // square the distance
            float ok_zone = (Screen.width * OK_PERCENT);
            ok_zone *= ok_zone; // square the distance

            float accuracy = 0f;
            
            for (int i = 0; i < list1.Count; i++)
            {
                float distance = ok_zone + 1;
                for (int j= 0; j < (list2.Count); j++)
                {
                    float dist = (list1[i]-list2[j]).sqrMagnitude;
                    if (dist <= distance)
                    {
                        distance = dist;
                        if (distance <= perfect_zone)
                        {
                            break;
                        }
                    }
                }
                
                if (distance <= perfect_zone)
                {
                    accuracy += PERFECT_VALUE;
                }
                else if (distance <= magical_zone)
                {
                    accuracy += MAGICAL_VALUE;
                }
                else if (distance < good_zone)
                {
                    accuracy += GOOD_VALUE;
                }
                else if (distance < ok_zone)
                {
                    accuracy += OK_VALUE;
                }
            }
            
            return (accuracy/(float)list1.Count);
        }

        float CalculateLinesLength(List<Vector3> lineOfPoints)
        {
            float dist = 0;
            for (int i = 0; i < lineOfPoints.Count-1; i++)
            {
                dist += (lineOfPoints[i]-lineOfPoints[i+1]).magnitude;
            }

            return dist;
        }

        public void CompareRuneToTrace()
        {
            int stroke = -1;
            for(int i = 0; i < _tracePoints.Count; i++)
            {
                if (_tracePoints[i].name == _runeStrokeName)
                {
                    stroke = i;
                    break;
                }
            }
            
            if (stroke >= 0)
            {
                List<Vector3> recPoints = new List<Vector3>();
                int pointInRuneStroke = _tracePoints[stroke].points.Count;
                for (int i = 0; i < pointInRuneStroke; i++)
                {
                    Vector3 tPoint = _tracePoints[stroke].points[i];
                    tPoint = ScaleRecordPoints(tPoint);
                    recPoints.Add(tPoint);
                }
                
                // Check for coverage for both list and return the one that is less.
                // If player only draw half of the trace, he will get half because the 
                // the full trace only see half of its pint covered.  If the
                // player draw too much, then his list is not fully correct.
                float acc1 = CalculatePointCoverage(recPoints, _screenPoints);
                float acc2 = CalculatePointCoverage(_screenPoints, recPoints);

                // User can draw very quick.  If the distance of the user line is about equal to the recorded trace, 
                // override the record trace score use the user score only.
                if (_screenPoints.Count < recPoints.Count)
                {
                    float playerLength = CalculateLinesLength(_screenPoints);
                    float recLength = CalculateLinesLength(recPoints)*0.9f; // 90% of the recLenght.
                    if (playerLength >= recLength)
                    {
                        acc1 = acc2;
                    }
                }
                float accuracy = acc2;
                if (acc1 < acc2)
                {
                    accuracy = acc1;
                }

                // Using lots of magic numbers here, but since these are only used here, no point making const for them.
                // We are limiting the best accuracy to 80 percent if you used all the time.  If you are faster, there 
                // will be up to 20 percent accuracy added back, but can't be better than your actual accurancy.
                const float BASE_ACC_THRESHOLD = 0.6f;
                const float TIMES_HALF = 0.5f;
                const float MAX_BONUS = 0.2f;

                if (accuracy > BASE_ACC_THRESHOLD)
                {
                    float orginalAcc = accuracy;
                    accuracy -= BASE_ACC_THRESHOLD;
                    accuracy *= TIMES_HALF;
                    accuracy += BASE_ACC_THRESHOLD;
                    
                    float timeLeft = _timeLeft;
                    if (timeLeft < 0f)
                    {
                        timeLeft = 0;
                    }
                    float timeBonus = (timeLeft/_timePerStroke);
                    if (timeBonus > MAX_BONUS)
                    {
                        timeBonus = MAX_BONUS;
                    }
                    accuracy += (timeBonus);
                    if (accuracy > orginalAcc)
                    {
                        accuracy = orginalAcc;
                    }
//                    Debug.LogWarning("Time Left was " + timeLeft);
//                    Debug.LogWarning("Time Percent was " + timeBonus);
                }
                
                _accuracyPerStroke.Add(accuracy);
                
                DisplayAccuracy(accuracy);
                
                if((_scoreDisplay == null) && (_scoreManager != null))
                {
                    var scoreLabel = _scoreManager.EvaluatePoints(accuracy);
                    _scoreDisplay = DisplayScores(scoreLabel);
                    StartCoroutine(_scoreDisplay);
                }
            }
            
            if (_screenPoints != null)
            {
                _screenPoints.Clear();
            }
            
            _runeStrokeName = ""; // done with the current stroke.
            
        }
        

	}

	public enum Difficulty
	{
		EASY = 0,
		NORMAL = 1,
		TRICKY = 2,
		HARD = 3,
		TROUBLE = 4
	}

}