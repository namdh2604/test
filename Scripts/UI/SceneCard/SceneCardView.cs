using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Voltage.Witches.StoryMap
{
	using UnityEngine.UI;
	using TMPro;

	using Voltage.Witches.Models;

    public class SceneCardView : MonoBehaviour
    {
		private const int MAX_BIT_PROGRESS = 5;
		private static IDictionary<SceneStatus,string> buttonTextMap = new Dictionary<SceneStatus, string>()
		{
			{SceneStatus.COMPLETED, "COMPLETED"},
			{SceneStatus.LOCKED, "LOCKED"},
			{SceneStatus.READABLE, "READ"}
		};

		[SerializeField]
		private TextMeshProUGUI _titleText;
		[SerializeField]
		private TextMeshProUGUI _descriptionText;

		[SerializeField]
		private Button _button;
		[SerializeField]
        private OutlinedFont _buttonText;
//		private TextMeshProUGUI _buttonText;
		public event EventHandler OnButtonSelected;

		[SerializeField]
		private Image _clearedBanner;
		[SerializeField]
		private Image _lockedBanner;

//		[SerializeField]
//		private Image _polaroidPlaceholder;

        [SerializeField]
        private RawImage _polaroid;

        private bool _buttonEnabled;

		private void Awake()
		{
			if(_titleText == null || _descriptionText == null)
			{
				throw new NullReferenceException();
			}

			if(_button == null || _buttonText == null)
			{
				throw new NullReferenceException();
			}

			if(_clearedBanner == null || _lockedBanner == null)
			{
				throw new NullReferenceException();
			}

//			if(_polaroidPlaceholder == null)
//			{
//				throw new NullReferenceException();
//			}

            if (_polaroid == null)
            {
                throw new NullReferenceException();
            }
		}

		public void Dispose()
		{
			Destroy (this);
		}

        private void OnEnable()
        {
            _button.enabled = _buttonEnabled;
        }

        private void OnDisable()
        {
            _button.enabled = false;
        }

		public void OnClick()
		{
			if(OnButtonSelected != null)
			{
				OnButtonSelected(this, new EventArgs());
			}
		}

		public void ResetEventHandler()
		{
			OnButtonSelected = null;
//			foreach(Delegate eventHandlers in OnButtonSelected.GetInvocationList())
//			{
//				OnButtonSelected -= (EventHandler)eventHandlers;		
//			}
		}

		public void SetTitle(string text)
		{
			_titleText.text = text;
		}

		public void SetDescription(string text)
		{
			_descriptionText.text = text;
		}

//		public void SetPolaroidPicture(Sprite image)
      public void SetPolaroidPicture(Texture2D image)
		{
            _polaroid.texture = image;
//			_polaroidPlaceholder.sprite = image;

			if(image != null)
			{
//				_polaroidPlaceholder.enabled = true;
                _polaroid.enabled = true;
			}
			else
			{
                _polaroid.enabled = false;
//				_polaroidPlaceholder.sprite = null;
//				_polaroidPlaceholder.enabled = false;
			}
		}
			
		public void SetCardState(SceneStatus state)
		{
			switch(state)
			{
				case SceneStatus.READABLE:
					ConfigureCard (new ReadableConfig()); break;
				case SceneStatus.LOCKED:
					ConfigureCard (new LockedConfig()); break;
				case SceneStatus.COMPLETED:
				default:
					ConfigureCard (new CompletedConfig());
					break;
			}
		}

		private void ConfigureCard(ICardStateConfig config)
		{
			SetButtonText (config.ButtonText);
			ShowButton (config.ButtonVisible);
			EnableButtonInput (config.ButtonVisible);
			ShowLockBanner (config.LockBannerVisible);
			ShowCompletedBanner (config.CompleteBannerVisible);
		}


		private void SetButtonText(string text)
		{
			_buttonText.text = text;
		}

		public void EnableButtonInput(bool value)
		{
            _buttonEnabled = value;
            _button.enabled = this.enabled && value;
		}

		private void ShowButton(bool visible)
		{
			_button.gameObject.SetActive (visible);
		}

		private void ShowLockBanner (bool visible)
		{
			_lockedBanner.gameObject.SetActive (visible);
		}

		private void ShowCompletedBanner (bool visible)
		{
			_clearedBanner.gameObject.SetActive (visible);
		}

		private interface ICardStateConfig
		{
			int BitProgress { get; }
			string ButtonText { get; }
			bool ButtonVisible { get; }
			bool LockBannerVisible { get; }
			bool CompleteBannerVisible { get; }
		}

		private struct ReadableConfig : ICardStateConfig
		{
			public int BitProgress { get { return 0; } }	// not valid
			public string ButtonText { get { return buttonTextMap[SceneStatus.READABLE]; } }
			public bool ButtonVisible { get { return true; } }
			public bool LockBannerVisible { get { return false; } }
			public bool CompleteBannerVisible { get { return false; }}
		}

		private struct CompletedConfig : ICardStateConfig
		{
			public int BitProgress { get { return 5; } }
			public string ButtonText { get { return buttonTextMap[SceneStatus.COMPLETED]; } }
			public bool ButtonVisible { get { return false; } }
			public bool LockBannerVisible { get { return false; } }
			public bool CompleteBannerVisible { get { return true; }}
		}

		private struct LockedConfig : ICardStateConfig
		{
			public int BitProgress { get { return 0; } }
			public string ButtonText { get { return buttonTextMap[SceneStatus.LOCKED]; } }
			public bool ButtonVisible { get { return true; } }
			public bool LockBannerVisible { get { return true; } }
			public bool CompleteBannerVisible { get { return false; }}
		}



    }

}


