using iGUI;
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Voltage.Witches.Controllers;
using Voltage.Witches.Components;

namespace Voltage.Witches.Screens
{
	public class MiniGameUIScreen : BaseScreen, IScreen
	{
		[HideInInspector]
		public iGUIContainer Progress_bar,feedback_texts,target_score_grp,score_grp,potion_bottle_meter_bg,stroke_grp,highscore_star_meter_1,highscore_star_meter_2,highscore_star_meter_3,
							 perfect_container,magical_container,nice_container,not_bad_container;

		[HideInInspector]
		public iGUIImage liquid02_superior,nice,not_bad,magical,perfect,progress_gauge;

		[HideInInspector]
		public iGUILabel scorefeedback_counter,currentscore_counter,targetscore_counter,stroke_counter;

		[HideInInspector]
		public iGUIButton Pause_Button;

		TracingScreenController _controller;
		float _baseScale;

		Dictionary<string,iGUIContainer> _containerMap;
		Dictionary<string,iGUIElement> _labelMap;

		IGUIHandler _buttonHandler;
		bool _dialogActive = false;
		public bool HasActiveDialog { get { return _dialogActive; } }
		public bool isDisplayingScore { get { return (_displayScoreRoutine != null); } }

		private IEnumerator _displayScoreRoutine;

		public float DelayNextTime { get { return (((_scaleTime * 2) + (_holdTime + _fadeOutTime)) * 0.6f); } }

		[SerializeField]
		private float _scaleTime = 0.6f;
		[SerializeField]
		private float _startScale = 0.2f;
		[SerializeField]
		private float _fadeOutTime = 0.25f;
		[SerializeField]
		private float _holdTime = 0.25f;
		[SerializeField]
		private iTweeniGUI.EaseType _scaleType = iTweeniGUI.EaseType.easeOutElastic;
		[SerializeField]
		private iTweeniGUI.EaseType _fadeType = iTweeniGUI.EaseType.easeOutElastic;

		public void Init(TracingScreenController controller)
		{
			_controller = controller;
		}

		protected void Awake()
		{
			_buttonHandler = gameObject.AddComponent<IGUIHandler>();
			_buttonHandler.ReleasedButtonEvent += HandleReleasedButtonEvent;
			_buttonHandler.MovedBack += HandleMovedBack;
			_buttonHandler.MovedAway += HandleMovedAway;
			_buttonHandler.InputCancelledEvent += HandleReleasedButtonEvent;
		}

		protected void Start()
		{
			iGUIRoot.instance.refresh();

            // Don't want to see the progress bar anymore, didn't remove at this time.
            // TODO: Once game is finalize and we don't want a progess bar, need to remove it from game.
            Progress_bar.opacity = 0;

			Pause_Button.clickDownCallback += ClickInit;
			currentscore_counter.label.text = "0";
			_labelMap = new Dictionary<string, iGUIElement>()
			{
				{"Not Quite",null},
				{"Nice", nice},
				{"Not Bad", not_bad},
				{"magical",magical},
				{"Magical",magical},
				{"perfect",perfect},
				{"Perfect",perfect}
			};

			_containerMap = new Dictionary<string,iGUIContainer> ()
			{
				{"Not Quite",null},
				{"Nice", nice_container},
				{"Not Bad", not_bad_container},
				{"magical",magical_container},
				{"Magical",magical_container},
				{"perfect",perfect_container},
				{"Perfect",perfect_container}
			};

			ChangeLiquidColor ((Color)new Color32 ((byte)245, (byte)0, (byte)255, (byte)255));
		}

		void Update()
		{
			if(Input.GetKeyUp(KeyCode.Q))
			{
				StartCoroutine(DisplayScoreLabel(new KeyValuePair<string, int>("Perfect",1000)));
			}
			if(Input.GetKeyUp(KeyCode.W))
			{
				StartCoroutine(DisplayScoreLabel(new KeyValuePair<string, int>("Magical",800)));
			}
			if(Input.GetKeyUp(KeyCode.E))
			{
				StartCoroutine(DisplayScoreLabel(new KeyValuePair<string, int>("Nice",400)));
			}
			if(Input.GetKeyUp(KeyCode.R))
			{
				StartCoroutine(DisplayScoreLabel(new KeyValuePair<string, int>("Not Bad",200)));
			}
			if(Input.GetKeyUp(KeyCode.T))
			{
				StartCoroutine(DisplayScoreLabel(new KeyValuePair<string, int>("Not Quite",0)));
			}
		}

		void ClickInit(iGUIElement element)
		{
			if((_buttonHandler.IsActive) && (_buttonHandler.PressedButton == null))
			{
				var button = (iGUIButton)element;
				_buttonHandler.SelectButton(button);
				button.colorTo(Color.grey,0f);
			}
		}
		
		void HandleMovedAway(iGUIButton button)
		{
			button.colorTo(Color.white,0.3f);
		}
		
		void HandleMovedBack(iGUIButton button)
		{
			button.colorTo(Color.grey,0f);
		}
		
		void HandleReleasedButtonEvent(iGUIButton button, bool isOverButton)
		{
			if(isOverButton)
			{
				if(button == Pause_Button)
				{
					ExecutePause();
				}
			}
			
			button.colorTo(Color.white,0.3f);
		}

		public override void MakePassive (bool isPassive)
		{
			Pause_Button.passive = isPassive;
		}

		public void DisplayUI()
		{
			target_score_grp.setEnabled(true);
			score_grp.setEnabled(true);
			potion_bottle_meter_bg.setEnabled(true);
			Pause_Button.setEnabled(true);
		}

		public void HideUI()
		{
			target_score_grp.setEnabled(false);
			score_grp.setEnabled(false);
			potion_bottle_meter_bg.setEnabled (false);
			Pause_Button.setEnabled(false);
		}

		public void SetScoreParams(int target,int mid,int high,int max)
		{
			SetTarget(target);
			SetBars(target,mid,high,max);
			SetGauge(0f);
		}

		void SetTarget(int target)
		{
			targetscore_counter.label.text = target.ToString();
		}

		void SetBars(int target,int mid,int high,int max)
		{
			var firstHeight = 1f - ((float)target / (float)max);
			var secondHeight = 1f - ((float)mid / (float)max);
			var thirdHeight = 1f - ((float)high / (float)max);

			highscore_star_meter_1.setY(firstHeight);
			highscore_star_meter_2.setY(secondHeight);
			highscore_star_meter_3.setY(thirdHeight);
		}

		void SetGauge(float newHeight)
		{
			if(newHeight > 1f)
			{
				newHeight = 1f;
			}
			var rect = liquid02_superior.positionAndSize;
			rect.height = newHeight;

			liquid02_superior.moveTo(rect,0.3f);
		}

		void ChangeLiquidColor(Color color)
		{
			liquid02_superior.colorTo(liquid02_superior.color, color, 0.3f);
		}

		public void UpdateStrokeCount(string strokesCount)
		{
			if(!stroke_grp.enabled)
			{
				stroke_grp.setEnabled(true);
			}
			stroke_counter.label.text = strokesCount;
		}

		public void UpdateScore(int score,float height)
		{
			currentscore_counter.label.text = score.ToString();
			SetGauge(height);
			if(height >= 1f)
			{
				height = 1f;
			}
		}

		public void ShowTimer(float scale)
		{
			_baseScale = scale;

			if(!Progress_bar.enabled)
			{
				Progress_bar.setEnabled(true);
			}
			progress_gauge.setWidth(1f);
			progress_gauge.setColor(Color.green);
		}

		public void UpdateGaugePosition(float currentTime)
		{
			var newWidth = currentTime / _baseScale;
			if((progress_gauge.color == Color.green) && (newWidth < 0.5f))
			{
				progress_gauge.colorTo(Color.red,0.3f);
			}
			if(newWidth <= 0f)
			{
				newWidth = 0f;
			}
			progress_gauge.setWidth(newWidth);
		}

		public void DisplayScoreOnScreen(KeyValuePair<string,int> scoreLabel)
		{
			if(_displayScoreRoutine != null)
			{
				StopCoroutine(_displayScoreRoutine);
			}
			_displayScoreRoutine = DisplayScoreLabel(scoreLabel);
			StartCoroutine(_displayScoreRoutine);
		}

		public void StopScoreDisplay()
		{
			if(_displayScoreRoutine != null)
			{
				StopCoroutine(_displayScoreRoutine);
			}
			feedback_texts.setEnabled(false);
			scorefeedback_counter.setOpacity(1f);
			feedback_texts.setOpacity(1f);
		}

		IEnumerator DisplayScoreLabel(KeyValuePair<string,int> scoreLabel)
		{
			scorefeedback_counter.setEnabled(false);
			scorefeedback_counter.setOpacity(0f);
			feedback_texts.setEnabled(true);
			feedback_texts.setOpacity(1f);
			var container = _containerMap[scoreLabel.Key];
			var element = _labelMap[scoreLabel.Key];
			if(element != null)
			{
				element.setEnabled(true);
				element.setOpacity(1f);
				element.setScale(_startScale);
			}

			var clipName = GetClipNameFromLabel(scoreLabel.Key);
			_controller.HandlePlayAudioClip(clipName);

			scorefeedback_counter.label.text = scoreLabel.Value.ToString();
			scorefeedback_counter.setEnabled(true);
			scorefeedback_counter.fadeTo (1f, (_scaleTime * 0.5f), (_scaleTime * 0.2f), _fadeType);
			if((container != null) && (element != null))
			{
				container.setEnabled(true);
				element.scaleTo(1f,_scaleTime,_scaleType);
			}

			yield return new WaitForSeconds(_scaleTime + _holdTime);
			if(element != null)
			{
				element.fadeTo(0f, _fadeOutTime, _fadeType);
			}
			yield return new WaitForSeconds(_fadeOutTime);
			if(element != null)
			{
				element.setEnabled(false);
				element.setOpacity(1f);
			}
			scorefeedback_counter.fadeTo(0f, _scaleTime, _fadeType);
			yield return new WaitForSeconds(_scaleTime);
			feedback_texts.setEnabled(false);
			scorefeedback_counter.setOpacity(1f);
			feedback_texts.setOpacity(1f);
			Debug.LogWarning("Done with the display close me");
			yield return null;
		}

		string GetClipNameFromLabel(string key)
		{
			var lowerKey = key.ToLower();
			if(lowerKey == "not quite")
			{
				return "Fail Game";
			}
			else if(lowerKey == "nice")
			{
				return "Nice!";
			}
			else if(lowerKey == "not bad")
			{
				return "Not Bad";
			}
			else if(lowerKey == "magical")
			{
				return "Magical";
			}
			else if(lowerKey == "perfect")
			{
				return "Perfect";
			}
			else
			{
				return string.Empty;
			}
		}

		public void ShowResults(int score,int stages,Action<int> responseHandler)
		{
			var dialog = _controller.GetResultsDialog(score,stages);
			_buttonHandler.Deactivate();
			dialog.Display(responseHandler);
		}

		public void HandleResultsResponse(int answer)
		{
			//TODO Maybe remove this
			_buttonHandler.Activate();
		}


		// HACK: to help Quit button know when it can call MakePassive against the screen controller and when it should defer to the dialogue
		public bool ShowingPauseDialogue { get; private set; }

		void ExecutePause()
		{
			ShowingPauseDialogue = true;

			_controller.PauseGame();
			var dialog = _controller.GetPauseDialog();
			_dialogActive = true;
			_buttonHandler.Deactivate();

			dialog.Display((i) => 
			{
				ShowingPauseDialogue = false;
				HandlePauseResponse(i);
			});
		}

		void HandlePauseResponse(int answer)
		{
			switch((PauseDialogResults)answer)
			{
				case PauseDialogResults.Resume:
					_controller.ResumeGame();
					break;
				case PauseDialogResults.Quit:
					ExecuteQuitConfirm();
					break;
			}

			_dialogActive = false;
			_buttonHandler.Activate();
		}

		void ExecuteQuitConfirm()
		{
			var dialog = _controller.GetQuitConfirmDialog();
			_dialogActive = true;
			_buttonHandler.Deactivate();
			dialog.Display(HandleQuitResponse);
		}

		void HandleQuitResponse(int answer)
		{
			switch((GiveUpConfirmationResponse)answer)
			{
				case GiveUpConfirmationResponse.Cancel:
					_controller.ResumeGame();
					break;
				case GiveUpConfirmationResponse.Quit:
					ExecuteQuit();
					break;
			}

			_dialogActive = false;
			_buttonHandler.Activate();
		}

		void ExecuteQuit()
		{
			_controller.DeductIngredients();
			_controller.PlayMenuMusic();
			_controller.MoveToIngredientsSelectionScreen();
		}

		protected override IScreenController GetController()
		{
			return _controller;
		}
	}
}