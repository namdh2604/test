using UnityEngine;
using System;
using System.Collections.Generic;

namespace Voltage.Witches.Screens.Dialogues
{
	using UnityEngine.UI;
	using TMPro;

	using Voltage.Witches.Models;
	using Voltage.Story.StoryPlayer;
	using Voltage.Common.UI;

	public class StoryPlayerInfoDialogue : BaseUGUIScreen	// : AbstractDialogUGUI 
    {
		
		[SerializeField]
		private CanvasGroup _canvasGrp;			

		[SerializeField]
		private TextMeshProUGUI _routeLabel;
		[SerializeField]
		private TextMeshProUGUI _arcLabel;
		[SerializeField]
		private TextMeshProUGUI _sceneLabel;
	

		[SerializeField]
		private Button _potionButton;
		[SerializeField]
		private TextMeshProUGUI _potionCount;


		[SerializeField]
		private Button _dialogueButton;
		[SerializeField]
		private TextMeshProUGUI _dialogueLabel;
		[SerializeField]
		private Button _closeButton;


		[SerializeField]
		private TextMeshProUGUI _anastasiaCount;
		[SerializeField]
		private TextMeshProUGUI _rhysCount;
		[SerializeField]
		private TextMeshProUGUI _tyroneCount;
		[SerializeField]
		private TextMeshProUGUI _melanieCount;
		[SerializeField]
		private TextMeshProUGUI _niklasCount;


		[SerializeField]
		private ToggleWidget _bgmToggle;
		[SerializeField]
		private ToggleWidget _sfxToggle;



		public event Action OnRefillStamina;
		public event Action OnDialogueButton;
		public event Action OnCloseButton;

//		public event Action<bool> OnToggleBGM;
//		public event Action<bool> OnToggleSFX;

		public void RefreshInfo(Player player, StoryPlayerSettings storyDetails, bool bgmEnabled=false, bool sfxEnabled=false)
		{
			if(player == null || storyDetails == null)
			{
				throw new ArgumentNullException();
			}

			SetStoryStatus (storyDetails);
			SetPotionCount (player.StaminaPotions);

			foreach(var kvp in player.GetAllAffinities())
			{
				SetCounter(kvp.Key, kvp.Value);
			}

            _bgmToggle.SetValue(bgmEnabled);
            _sfxToggle.SetValue(sfxEnabled);
		}




		private void Awake()
		{
			// TODO: guard clauses


			SubscribeButtons ();

			// TODO: guard clause that buttons are initialized

		}


		private void SubscribeButtons()		// or explicitly set persistent listener on button!
		{
			Action<Action,Button> onClick = ((action,button) => 
          	{
//				Debug.Log ("button clicked: " + button.name);

				if(action != null)
				{
					action();
				}
			});

			_potionButton.onClick.AddListener(() => onClick (OnRefillStamina, _potionButton));
			_dialogueButton.onClick.AddListener (() => onClick (OnDialogueButton, _dialogueButton));
			_closeButton.onClick.AddListener (() => onClick (OnCloseButton, _closeButton));

			_bgmToggle.OnToggle += EnableBGM;
			_sfxToggle.OnToggle += EnableSFX;
		}

		private void UnsubscribeButtons()
		{
			_potionButton.onClick.RemoveAllListeners ();
			_dialogueButton.onClick.RemoveAllListeners ();
			_closeButton.onClick.RemoveAllListeners ();

			_bgmToggle.OnToggle -= EnableBGM;
			_sfxToggle.OnToggle -= EnableSFX;
		}



		private void EnableBGM(bool value)
		{
			PlayerPreferences.GetInstance().SoundEnabled = value;
		}

		private void EnableSFX(bool value)
		{
			PlayerPreferences.GetInstance().SFXEnabled = value;
		}




		public void MakePassive(bool value)
		{
			_potionButton.enabled = !value;
			_dialogueButton.enabled = !value;
			_closeButton.enabled = !value;

			_bgmToggle.MakePassive (value);
			_sfxToggle.MakePassive (value);
		}

		public void MakeCloseButtonPassive(bool value)
		{
			_closeButton.enabled = !value;
		}


		public void EnableDialogueButton(bool enable)
		{
			_dialogueButton.interactable = enable;

			float labelAlpha = (enable ? _dialogueButton.colors.normalColor.a : _dialogueButton.colors.disabledColor.a );
			_dialogueLabel.CrossFadeAlpha (labelAlpha, 0f, true);
		}




		private void SetStoryStatus(StoryPlayerSettings details)	
		{
			_routeLabel.text = details.RouteName;
			_arcLabel.text = details.ArcName;
			_sceneLabel.text = details.SceneName;
		}

		private void SetPotionCount(int count)
		{
			_potionCount.text = count.ToString ();
		}

		private void SetCounter(string characterID, int amount)		// would prefer an enumeration, but affinity is mapped to a string ID
		{
			// sync against MasterStoryData.NPCs or PlayerDataStore
			IDictionary<string,TextMeshProUGUI> countLabelMap = new Dictionary<string,TextMeshProUGUI>()	// HACK would prefer an enumeration OR sequence convention
			{
				{"A", _anastasiaCount},
				{"R", _rhysCount},
				{"T", _tyroneCount},
				{"M", _melanieCount},
				{"N", _niklasCount},
			};

			countLabelMap [characterID].text = amount.ToString ();	// can throw exception if characterID doesnt exist
		}




		private Voltage.Witches.Controllers.IScreenController _controller;

		public void Init(Voltage.Witches.Controllers.IScreenController controller)
		{
			_controller = controller;
		}

		protected override Voltage.Witches.Controllers.IScreenController GetController()
		{
			return _controller;
		}


    }

}


