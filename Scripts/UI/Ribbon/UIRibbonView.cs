using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Voltage.Witches.Screens;
using Voltage.Witches.Controllers;
using Voltage.Witches.Views;

namespace Voltage.Witches.UI
{
	using UnityEngine.UI;
	using TMPro;

//	[RequireComponent(typeof(RectTransform))]
	public class UIRibbonView : BaseUGUIScreen, IUIRibbonView
    {
		private const float RIBBON_OPEN_POS_Y = 0f;

        #region controls
		[SerializeField]	// production requested that the ribbon's attributes are accessible in the editor
		[Range(0f, 1f)]
		private float _percentageOfRibbonVisibleWhenClosed = 0.18f;			// sync with igui ribbon...InterfaceShellView::PERCENTAGE_OF_RIBBON_VISIBLE_WHEN_CLOSED

		[SerializeField]
		private float _ribbonTranslateDurationInSeconds = 1f;

		[SerializeField]
		private iTween.EaseType _ribbonEaseType = iTween.EaseType.easeOutBounce;

		[SerializeField]
		private Button _ribbonOpenButton;
		public event Action OnOpenRibbon;
		[SerializeField]
		private Button _ribbonCloseButton;
		public event Action OnCloseRibbon;

		public event Action OnRibbonEnabled;

		[SerializeField]
		private Button _shopButton;
		public event Action OnShopButtonSelected;

		[SerializeField]
		private RectTransform _ribbonContainer;
		private Vector2 _ribbonSize;				// don't need to serialize

		[SerializeField]
		private float _fadeDurationInSeconds = 1f;
		[SerializeField]
		private iTween.EaseType _fadeEaseType = iTween.EaseType.easeOutQuad;
		[SerializeField]
		private CanvasGroup _canvasGroup;

		[SerializeField]
		private TextMeshProUGUI _regularCurrencyCounter;
		[SerializeField]
		private TextMeshProUGUI _premiumCurrencyCounter;

		[SerializeField]
		private TextMeshProUGUI _staminaCounter;
		[SerializeField]
		private List<Image> _staminaLevelImages;
		[SerializeField]
		private TextMeshProUGUI _focusCounter;
		[SerializeField]
		private List<Image> _focusLevelImages;

		[SerializeField]
		private string _defaultTime = "-h --m";
		[SerializeField]
		private string _counterFormat = "{0:0}h:{1:00}m";
		[SerializeField]
		private TextMeshProUGUI _staminaTimer;
		[SerializeField]
		private TextMeshProUGUI _focusTimer;
		[SerializeField]
		private GameObject _staminaTimerGO;
		[SerializeField]
		private GameObject _focusTimerGO;

        #endregion

		private const float TIMER_REFRESH_RATE = 10.0f;

        private bool _isOpen = true;

        private bool _activeToggleButton = true;
        private bool _activeShopButton = true;
        private bool _passive = false;

        private DateTime _nextStaminaUpdate;
        private DateTime _nextFocusUpdate;

		public float RibbonTranslateDurationInSeconds
		{
			get { return _ribbonTranslateDurationInSeconds; }
			set { _ribbonTranslateDurationInSeconds = value; } 
		}

		public float RibbonFadeDurationInSeconds
		{
			get { return _fadeDurationInSeconds; }
			set { _fadeDurationInSeconds = value; } 
		}

		public string CounterFormat
		{
			get { return _counterFormat; }
			set { _counterFormat = value; } 
		}

		public string DefaultTime
		{
			get { return _defaultTime; }
			set { _defaultTime = value; } 
		}
	

		private void Awake()
		{
			InitializeRibbon ();
		}

        public void Init(bool isOpen)
        {
            _isOpen = isOpen;
        }

        private void Start()
        {
            SetRibbonPosition(_isOpen);
        }

        private void SetRibbonPosition(bool isOpen)
        {
            if (isOpen)
            {
                _ribbonContainer.anchoredPosition = new Vector2(0, RIBBON_OPEN_POS_Y);
                ShowCloseToggleButton();
            }
            else
            {
                _ribbonContainer.anchoredPosition = new Vector2(0, _ribbonSize.y * (1 - _percentageOfRibbonVisibleWhenClosed));
                ShowOpenToggleButton();
            }
        }

        private void OnEnable()
        {
            if (gameObject.activeInHierarchy)
            {
				if (OnRibbonEnabled != null)
				{
					OnRibbonEnabled();
				}

            }
        }

		private void InitializeRibbon()
		{
			if(_ribbonContainer == null)
			{
				throw new ArgumentNullException("ribbon container not set");	// this.GetComponent<RectTransform>()
			}

			if(_ribbonOpenButton == null || _ribbonCloseButton == null)
			{
				throw new ArgumentNullException();
			}

			if(_shopButton == null)
			{
				throw new ArgumentNullException();
			}

			if(_canvasGroup == null)
			{
				throw new ArgumentNullException();
			}

			if(_premiumCurrencyCounter == null || _regularCurrencyCounter == null)
			{
				throw new ArgumentNullException();
			}

			if(_staminaLevelImages == null || _staminaLevelImages.Count == 0 || _staminaCounter == null || _focusLevelImages == null || _focusLevelImages.Count == 0 || _focusCounter == null)
			{
				throw new ArgumentNullException();
			}

			if(_staminaTimer == null || _focusTimer == null)
			{
				throw new ArgumentNullException();
			}

			_ribbonSize = _ribbonContainer.sizeDelta;
		}


		public override void Show ()
		{
			base.Show ();
			transform.SetAsLastSibling ();
		}




		////////////////
		// Visibility //
		////////////////

		private IEnumerator OpacityRoutine(float targetOpacity, float duration, Action onComplete)
		{
			object[] args = new object[]
			{
				"from", _canvasGroup.alpha,
				"to", targetOpacity,
				"time", duration,
				"easetype", _fadeEaseType,
				"onupdate", (Action<object>)(value => _canvasGroup.alpha = (float)value),
			};
			
			iTween.ValueTo (gameObject, iTween.Hash(args));
			yield return new WaitForSeconds (duration);

			if(onComplete != null)
			{
				onComplete();
			}
		}




		////////////////
		// Open/Close //
		////////////////

        // NOTE: This is used in ToggleRibbon. The behavior is a bit quirky if you try to toggle
        //  the ribbon while an animation is in progress. While this should likely be implemented in terms of _isOpen,
        //  I'm leaving this as-is due to the existing behavior.
		// wasn't intended to account for transitions, hence its private following internal design
		private bool IsClosed
		{
			get { return _ribbonContainer.anchoredPosition.y > RIBBON_OPEN_POS_Y; }	// == Vector3.zero
		}
		
		public void ToggleRibbon()
		{
			if (IsClosed)
			{
				OpenRibbon();
			}
			else
			{
				CloseRibbon();
			}
		}

		Action _currentOnTranslateCompleteCB = null;

		void onTranslateComplete()
		{
			if (_currentOnTranslateCompleteCB != null) {
				_currentOnTranslateCompleteCB ();
			}
		}

		public void OpenRibbon()
		{
			if(OnOpenRibbon != null)
			{
				OnOpenRibbon();
			}

			_currentOnTranslateCompleteCB = new Action(ShowCloseToggleButton) + (() => _isOpen = true);		// could just bundled in ShowCloseToggleButton with anonymous method

			EnableToggleButtonInput (false);
			StartCoroutine (TranslateRibbonRoutine (1f, _ribbonTranslateDurationInSeconds));		
		}

		public void CloseRibbon()
		{
			if(OnCloseRibbon != null)
			{
				OnCloseRibbon();
			}

			_currentOnTranslateCompleteCB = new Action(ShowOpenToggleButton) + (() => _isOpen = false);		// could just bundled in ShowOpenToggleButton with anonymous method

			EnableToggleButtonInput (false);
			StartCoroutine (TranslateRibbonRoutine (_percentageOfRibbonVisibleWhenClosed, _ribbonTranslateDurationInSeconds));	
		}


		private IEnumerator TranslateRibbonRoutine(float percentVisible, float duration)
		{
			float targetPos = _ribbonSize.y - (percentVisible * _ribbonSize.y);
		
			object[] args = new object[]
			{
				"from", _ribbonContainer.anchoredPosition.y,
				"to", targetPos,
				"time", duration,
				"easetype", _ribbonEaseType,
				"onupdate", (Action<object>)(value => _ribbonContainer.anchoredPosition = new Vector2(0f, (float)value)),
				"oncomplete", "onTranslateComplete"
			};

			iTween.ValueTo (gameObject, iTween.Hash(args));
			yield return null;
		}


		private void ShowOpenToggleButton()
		{
			ToggleButton (true);
			EnableToggleButtonInput (true);
		}

		private void ShowCloseToggleButton()
		{
			ToggleButton (false);
			EnableToggleButtonInput (true);
		}

		private void ToggleButton(bool isOpen)
		{
			_ribbonOpenButton.gameObject.SetActive (isOpen);
			_ribbonCloseButton.gameObject.SetActive (!isOpen);
		}

		public void EnableToggleButtonInput(bool value)
		{
            _activeToggleButton = value;
            _ribbonOpenButton.enabled = IsButtonEnabled(_activeToggleButton);
			_ribbonCloseButton.enabled = IsButtonEnabled(_activeToggleButton);
		}


		////////////////
		// 	  Shop	  //
		////////////////


		public void ShowShopDialogue()
		{
			if(OnShopButtonSelected != null)
			{
				OnShopButtonSelected ();	
			}
		}

		public void EnableShopButtonInput (bool value)
		{
            _activeShopButton = value;
            _shopButton.enabled = IsButtonEnabled(_activeShopButton);
		}


		////////////////
		// 	Counters  //
		////////////////


		public void SetPremiumCurrency(int currency)
		{
			_premiumCurrencyCounter.text = currency.ToString ();
		}

		public void SetRegularCurrency(int currency)
		{
			_regularCurrencyCounter.text = currency.ToString ();
		}


		private void ZeroOutStamina()
		{
			foreach(Image stamina in _staminaLevelImages)
			{
				stamina.enabled = false;
			}
		}

		public void SetStamina(int stamina)
		{
			for(int i=0; i < _staminaLevelImages.Count; i++)
			{
				if(i < stamina)
				{
					_staminaLevelImages[i].enabled = true;
				}
				else
				{
					_staminaLevelImages[i].enabled = false;
				}
			}

			_staminaCounter.text = stamina.ToString ();
		}

		public void SetStaminaTimer(TimeSpan timeRemaining)
		{
			_staminaTimerGO.SetActive (true);
			_staminaTimer.text = GetTimerTime (timeRemaining);
		}

		public void HideStaminaTimer()
		{
            if(_staminaCountdown != null)
            {
                StopCoroutine(_staminaCountdown);
                _staminaCountdown = null;
            }

			_staminaTimerGO.SetActive (false);
		}

		private void ZeroOutFocus()
		{
			foreach(Image focus in _focusLevelImages)
			{
				focus.enabled = false;
			}
		}

		public void SetFocus(int focus)
		{
			for(int i=0; i < _focusLevelImages.Count; i++)
			{
				if(i < focus)
				{
					_focusLevelImages[i].enabled = true;
				}
				else
				{
					_focusLevelImages[i].enabled = false;
				}
			}

			_focusCounter.text = focus.ToString ();
		}

		public void SetFocusTimer(TimeSpan timeRemaining)
		{
			_focusTimerGO.SetActive (true);
			_focusTimer.text = GetTimerTime (timeRemaining);
		}

		public void HideFocusTimer()
		{

			if (_focusCountdown != null)
            {
                StopCoroutine(_focusCountdown);
                _focusCountdown = null;
            }

			_focusTimerGO.SetActive (false);
		}

		private string GetTimerTime(TimeSpan timeRemaining)
		{
			string time = _defaultTime;
			if(timeRemaining.TotalSeconds > 0)
			{
				int minutesLeft = (int)timeRemaining.TotalMinutes;
				time = string.Format(_counterFormat,Mathf.Floor(minutesLeft/60), minutesLeft % 60);
			}
			
			return time;
		}


		protected override IScreenController GetController ()
		{
			return null;
		}

		Coroutine _staminaCountdown;
		Coroutine _focusCountdown;
		public void SetNextUpdate(CountDownType type,DateTime nextUpdate)
		{
			
			if (type == CountDownType.STAMINA)
			{
                _nextStaminaUpdate = nextUpdate.ToLocalTime();

				if(_staminaCountdown != null)
				{
					StopCoroutine(_staminaCountdown);
				}

                if (gameObject.activeInHierarchy)
                {
                    _staminaCountdown = StartCoroutine(StaminaCountdown(_nextStaminaUpdate));
                }
			}
			else if(type == CountDownType.FOCUS)
			{
                _nextFocusUpdate = nextUpdate.ToLocalTime();

				if(_focusCountdown != null)
				{
					StopCoroutine(_focusCountdown);
				}

                if (gameObject.activeInHierarchy)
                {
                    _focusCountdown = StartCoroutine(FocusCountdown(_nextFocusUpdate));
                }
			}
			else
			{
				Debug.LogError("Count Down Type must be Focus or Stamina");
				throw new Exception();
			}
		}
		
		
		IEnumerator StaminaCountdown(DateTime nextUpdate)
		{
			while (true)
			{
				UpdatePlayerStaminaTimer(nextUpdate);
				yield return new WaitForSeconds(TIMER_REFRESH_RATE);
			}
		}
		
		IEnumerator FocusCountdown(DateTime nextUpdate)
		{
			while (true)
			{
				UpdatePlayerFocusTimer(nextUpdate);
				yield return new WaitForSeconds(TIMER_REFRESH_RATE);
			}
		}
		
		public void UpdatePlayerStaminaTimer (DateTime nextUpdate)
		{
			SetStaminaTimer(GetRemainingTime(nextUpdate));
		}
		
		public void UpdatePlayerFocusTimer (DateTime nextUpdate)
		{
			SetFocusTimer (GetRemainingTime (nextUpdate));
		}
		
		private TimeSpan GetRemainingTime(DateTime nextUpdate)
		{
			DateTime now = DateTime.Now;
			return nextUpdate.ToLocalTime().Subtract(now);
		}

        private bool IsButtonEnabled(bool buttonStatus)
        {
            return !_passive && buttonStatus;
        }

        public void MakePassive(bool value)
        {
            _passive = value;
            _shopButton.enabled = IsButtonEnabled(_activeShopButton);
            _ribbonOpenButton.enabled = IsButtonEnabled(_activeToggleButton);
            _ribbonCloseButton.enabled = IsButtonEnabled(_activeToggleButton);
        }
	}
	
}
