using System;
using System.Collections;
using System.Collections.Generic;

namespace Voltage.Witches.Screens
{
	using UnityEngine;
	using UnityEngine.UI;

	using TMPro;

	using Voltage.Witches.Models;
	using Voltage.Witches.Events;
	using Voltage.Witches.Models.Avatar;
	using Voltage.Witches.Unity;


	public class HomeScreenView : BaseUGUIScreen
	{
		
		[SerializeField]
		private Button _storyButton;
		[SerializeField]
		private Image _storyReadImage;
		[SerializeField]
		private Image _storyResumeImage;

		[SerializeField]
		private Button _miniGameButton;
		[SerializeField]
		private Image _miniGameLock;

		[SerializeField]
		private Button _shopButton;
		[SerializeField]
		private Image _shopLock;
		[SerializeField]
		private Button _bundleButton;
        [SerializeField]
        private TextMeshProUGUI _bundleTimerLabel;



		[SerializeField]
		private Button _avatarButton;
		[SerializeField]
		private Image _avatarLock;


		[SerializeField]
		private Button _mailButton;

		[SerializeField]
		private Button _optionButton;

		[SerializeField]
		private TextMeshProUGUI _nameLabel;

		[SerializeField]
		private RawImage _polaroid;

		[SerializeField]
		private Image _tapInstructionImage;



		public override void Dispose ()
		{
			StopTapDisplayRoutine();
			StopMailButtonAnimRoutine();

			base.Dispose ();
		}

		public override void Hide ()
		{
			// relies on Show() to call refresh to populate the Avatar bust
			CleanupAvatarBustAssets();

			base.Hide ();
		}
			

		public void MakePassive(bool value)
		{
			_avatarButton.interactable = !(value || IsAvatarLocked);
			_miniGameButton.interactable = !(value || IsMiniGameLocked);
			_shopButton.interactable = !(value || IsShopLocked);

			_storyButton.interactable = !value;
			_mailButton.interactable = !value;
			_optionButton.interactable = !value;

			_bundleButton.interactable = !value;
		}

		public void MakeLeftSidePassive(bool value)
		{
			_miniGameButton.interactable = !(value || IsMiniGameLocked);
            _bundleButton.interactable = !value;            // FIXME: there is potentially here to become out of sync with MakePassive
		}


		private bool IsAvatarLocked
		{
			get { return _avatarLock.IsActive(); }
		}

		public void UnlockAvatarCloset(bool value)
		{
			_avatarLock.gameObject.SetActive(!value);
			_avatarButton.interactable = value;
		}

		private bool IsShopLocked
		{
			get { return _shopLock.IsActive(); }
		}

		public void UnlockClothingStore(bool value)
		{
			_shopLock.gameObject.SetActive(!value);
			_shopButton.interactable = value;
		}

		private bool IsMiniGameLocked
		{
			get { return _miniGameLock.IsActive(); }
		}

		public void UnlockMiniGame(bool value)
		{
			_miniGameLock.gameObject.SetActive(!value);
			_miniGameButton.interactable = value;
		}



        private IEnumerator _starterPackCountdownRoutine;

        // FIXME requires view to be active to run coroutine for countdown
        // FIXME has a dependency on player (which isn't set till Init()!)
        public void UnlockStarterPack(bool value)
        {
            _bundleButton.enabled = value;
            _bundleButton.gameObject.SetActive(value);

            if (_starterPackCountdownRoutine != null)
            {
                StopCoroutine(_starterPackCountdownRoutine);    // can't stopcoroutine unless object is active? but coroutine won't run if object is inactive anyway
            }

            if (value)
            {
                _starterPackCountdownRoutine = StarterPackCountdown();
                StartCoroutine(_starterPackCountdownRoutine);
            }
        }

        private const float TIMER_REFRESH_RATE = 1f;
        private IEnumerator StarterPackCountdown()
        {
            while (true)
            {
                TimeSpan timeRemaining = _player.TimeToDisableStarterPack - DateTime.UtcNow;
                _bundleTimerLabel.text = GetTimerTime(timeRemaining);

                if (timeRemaining <= TimeSpan.Zero)
                {
                    break;
                }

                yield return new WaitForSeconds(TIMER_REFRESH_RATE);
            }
                
            UnlockStarterPack(false);   // or fire OnStarterPackCountdownCompleted event?
        }

        private const string DEFAULT_TIME = "--:--:--";
        private const string COUNTER_FORMAT = "{0:00}:{1:00}:{2:00}";
        private string GetTimerTime(TimeSpan timeRemaining)
        {
            string time = DEFAULT_TIME;
            if(timeRemaining > TimeSpan.Zero)
            {
                int minutesLeft = (int)(timeRemaining.TotalMinutes);
                double hours = System.Math.Floor(minutesLeft / 60D);
                double minutes = minutesLeft % 60;
                int seconds = timeRemaining.Seconds;

                time = string.Format("{0:00}:{1:00}:{2:00}", hours, minutes, seconds);
            }

            return time;
        }





		public void ShowAvatarTapText(float duration=0f)
		{
			StopTapDisplayRoutine();

			_tapInstructionDisplayRoutine = DisplayTapInstructionsRoutine(duration);
			StartCoroutine(_tapInstructionDisplayRoutine);
		}

		private void StopTapDisplayRoutine()
		{
			if(_tapInstructionDisplayRoutine != null) 
			{
				StopCoroutine(_tapInstructionDisplayRoutine);
				_tapInstructionDisplayRoutine = null;
			}
		}


		private IEnumerator _tapInstructionDisplayRoutine;
		private IEnumerator DisplayTapInstructionsRoutine(float duration)
		{
			_tapInstructionImage.gameObject.SetActive(true);
			_tapInstructionImage.canvasRenderer.SetAlpha(0f);

			float fadeDuration = 1f;

			_tapInstructionImage.CrossFadeAlpha (1f, fadeDuration, false);
			yield return new WaitForSeconds(fadeDuration);

			yield return new WaitForSeconds(duration);

			_tapInstructionImage.CrossFadeAlpha (0f, fadeDuration, false);
			yield return new WaitForSeconds(fadeDuration);

			_tapInstructionImage.gameObject.SetActive(false);
		}



		public void DisplayMailButton()
		{
			StopMailButtonAnimRoutine();

			_mailButtonAnimRoutine = DisplayMailButtonAnimRoutine();
			StartCoroutine(_mailButtonAnimRoutine);
		}

		private void StopMailButtonAnimRoutine()
		{
			if(_mailButtonAnimRoutine != null) 
			{
				StopCoroutine(_mailButtonAnimRoutine);
				_mailButtonAnimRoutine = null;
			}
		}

		private IEnumerator _mailButtonAnimRoutine;
		public IEnumerator DisplayMailButtonAnimRoutine()
		{
			float fadeDuration = 0.5f;

			_mailButton.transform.localScale = Vector3.one;
			_mailButton.image.canvasRenderer.SetAlpha(0f);

			_mailButton.gameObject.SetActive(true);

			_mailButton.image.CrossFadeAlpha(1f, fadeDuration, false);
			yield return new WaitForSeconds(fadeDuration);

			float scaleFactorX = 1.6f;
			float scaleFactorY = 1.2f;
			float scaleDuration = 1f;

			iTween.ScaleTo(_mailButton.gameObject, new Hashtable{{"scale", new Vector3(scaleFactorX,scaleFactorY,1f)}, {"time", scaleDuration}, {"looptype", iTween.LoopType.pingPong}, {"easetype", iTween.EaseType.easeOutBounce}}); 
		}

		public void HideMailButton()
		{
			StopMailButtonAnimRoutine();
			_mailButton.gameObject.SetActive (false);
		}


			
		private void Awake()
		{
			// TODO: guard clauses

            // TODO: may need to handle when view is shown/hidden
			EnsureButtonsHiddenAtStart();
			DefaultLock();						// maybe unnecessary

			SubscribeEvents ();
		}

		private void EnsureButtonsHiddenAtStart()
		{
			_mailButton.gameObject.SetActive(false);
			_bundleButton.gameObject.SetActive(false);
		}

		private void DefaultLock()
		{
			UnlockAvatarCloset(false);
			UnlockClothingStore(false);
			UnlockMiniGame(false);
		}




		public event Action OnAvatarButtonSelected;
		public event Action OnMailButtonSelected;
		public event Action OnOptionButtonSelected;
		public event Action OnMiniGameButtonSelected;
		public event Action OnStoryButtonSelected;
		public event Action OnShopButtonSelected;
		public event Action OnStarterPackSelected;
//		public event Action OnNewsButtonSelected;
//		public event Action OnProfileButtonSelected;
//		public event Action OnSocialButtonSelected;



		private void SubscribeEvents()
		{
			Action<Action> onClick = (action) => 
			{
				if(action != null) 
				{
					action ();
				}
			};

			_avatarButton.onClick.AddListener(() => onClick(OnAvatarButtonSelected));
			_mailButton.onClick.AddListener(() => onClick(OnMailButtonSelected));
			_optionButton.onClick.AddListener(() => onClick(OnOptionButtonSelected));
			_miniGameButton.onClick.AddListener(() => onClick(OnMiniGameButtonSelected));
			_storyButton.onClick.AddListener(() => onClick(OnStoryButtonSelected));
			_shopButton.onClick.AddListener(() => onClick(OnShopButtonSelected));
			_bundleButton.onClick.AddListener(() => onClick(OnStarterPackSelected));
		}



        private Player _player;

        // FIXME: this should be replaced by Show() when inScene/polaroidPath dependencies are not required to be passed in
        // Init(player) can then be called once when view is created
        public void Refresh (Player player, bool inScene, string polaroidPath)
		{	
            _player = player;

			SetName(player.FirstName, player.LastName);
			SetStoryButton(inScene, polaroidPath);

            GetAvatarSprite();
            ScaleAvatarToFit();
		}


		private void SetName(string given, string surname)
		{
			string fullname = string.Format("{0}\n{1}", given, surname);
			_nameLabel.text = fullname;
		}

		private void SetStoryButton(bool inScene, string polaroidPath)
		{
			_storyResumeImage.enabled = inScene;
			_storyReadImage.enabled = !inScene;

			_polaroid.texture = Resources.Load<Texture>(polaroidPath);		// can throw exception
		}


        private static readonly Vector2 CENTER_PIVOT = new Vector2(0.5f, 0.5f);
        private void GetAvatarSprite()
        {
			CleanupAvatarBustAssets();		// may not be necessary to call this here, as its being called on Hide()

            _bustTexture = new AvatarTextureReader ().GetTexture(AvatarType.Headshot);
			_bustSprite = Sprite.Create(_bustTexture.Texture, new Rect (0.0f, 0.0f, _bustTexture.Texture.width, _bustTexture.Texture.height), CENTER_PIVOT);

			_avatarButton.image.sprite = _bustSprite;
//			_avatarButton.image.sprite = Sprite.Create(_bustTexture.Texture, new Rect (0.0f, 0.0f, _bustTexture.Texture.width, _bustTexture.Texture.height), CENTER_PIVOT);
        }

		WrappedTexture _bustTexture;
		Sprite _bustSprite;
		private void CleanupAvatarBustAssets()
		{
			if (_avatarButton.image.sprite != null)
			{
//				Sprite sprite = _avatarButton.image.sprite;
				_avatarButton.image.sprite = null;

//				DestroyImmediate (sprite, true);
			}

			if (_bustSprite != null) 
			{
				Destroy (_bustSprite);
			}

			if (_bustTexture != null) 
			{
                _bustTexture.Dispose();
			}	
		}



        private const float IMPORT_SCALAR = 2f;                 // image is imported at double the scale of actual
        private static readonly float HEIGHT_ANCHOR = 0.95f;    // percent of screen height image should fill

        // can't be called in Awake() as computing scale requires canvas, which isn't guaranteed to be properly parented until Start()
        private void ScaleAvatarToFit()
        {
            Image avatarImage = _avatarButton.image;    
            Vector2 graphicSize = new Vector2(avatarImage.sprite.texture.width, avatarImage.sprite.texture.height) * IMPORT_SCALAR; 

//          Rect pixelRect = avatarImage.canvas.pixelRect;                              // graphic canvas
//          Vector2 deviceResolution = new Vector2 (pixelRect.width, pixelRect.height);

            Rect canvasRect = avatarImage.canvas.GetComponent<RectTransform>().rect; 
            Vector2 referenceResolution = new Vector2 (canvasRect.width, canvasRect.height);

            float spriteRatio = graphicSize.x / graphicSize.y;
//          float aspectRatio = deviceResolution.x / deviceResolution.y;

            float scaledHeight = referenceResolution.y * HEIGHT_ANCHOR;
            float scaledWidth = scaledHeight *  spriteRatio;                
            Vector2 scaledSize = new Vector2(scaledWidth, scaledHeight);

            avatarImage.rectTransform.sizeDelta = scaledSize;
        }


       






		// DEPRECATED
		protected override Voltage.Witches.Controllers.IScreenController GetController()
		{
			return null;
		}



	}
}



