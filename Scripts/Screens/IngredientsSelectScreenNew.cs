using UnityEngine;
using iGUI;
using Voltage.Witches.Controllers;
using System.Collections.Generic;
using Voltage.Witches.Models;
using Voltage.Witches.Events;
using Voltage.Witches.Views;
using Voltage.Witches.Components;
using Voltage.Witches.Lib;
using System;
using System.Collections;

namespace Voltage.Witches.Screens
{
	using Voltage.Witches.Shop;
	using Debug = UnityEngine.Debug;

	using Voltage.Witches.Configuration;

	public class IngredientsSelectScreenNew : BaseScreen
	{
		[HideInInspector]
		public iGUIButton btn_tarot_card, btn_bookmark;

		[HideInInspector]
		public iGUIContainer shelves,gold;
		
		[HideInInspector]
		public iGUILabel Potion_name_header,hint_text_label;
		
		[HideInInspector]
		public Placeholder difficultyPlaceholder;
		
		[HideInInspector]
		public Placeholder interfaceBarPlaceholder;
		private iGUISmartPrefab_InterfaceShell _interface;

		private iGUISmartPrefab_DifficultyViewNew _difficultyView;
		
		private List<iGUIContainer> _shelfContainers;
		private List<iGUISmartPrefab_IngredientShelfNew> _shelves;
		
		private List<Ingredient> _selectedIngredients;
		
		private DifficultyMap _difficultyMap;

		private IngredientsSelectScreenController _controller;
		private Player _player;

		private List<iGUIButton> _pressableButtons;
		private Dictionary<int,List<iGUIButton>> _shelfButtons;

		Ingredient _requestedIngredient = null;
		IGUIHandler _buttonHandler;
		Dictionary<iGUIButton,iGUIElement> _buttonArtMap;
		int _loadedShelves = 0;

				
		public IButtonHandler GetButtonHandlerInterface()
		{
			return _buttonHandler;
		}
	

		private static List<string> difficultyMapKeys = new List<string>()
		{
			"minigame_difficulty_easy",
			"minigame_difficulty_normal",
			"minigame_difficulty_tricky",
			"minigame_difficulty_hard",
			"minigame_difficulty_trouble"
		};

		protected void Awake()
		{
			_buttonHandler = gameObject.AddComponent<IGUIHandler>();
			_buttonHandler.MovedAway += HandleMovedAway;
			_buttonHandler.MovedBack += HandleMovedBack;
			_buttonHandler.ReleasedButtonEvent += HandleReleasedButtonEvent;
			_buttonHandler.InputCancelledEvent += HandleReleasedButtonEvent;
		}

		protected void Start()
		{
			_interface = LoadPlaceholder().GetComponent<iGUISmartPrefab_InterfaceShell>();
			_interface.SetLayout(Voltage.Witches.Views.InterfaceLayout.Standard);
			_interface.SetCounts(_player);
			_interface.BeginCountDown(CountDownType.FOCUS, _player.FocusNextUpdate);
			_interface.BeginCountDown(CountDownType.STAMINA, _player.StaminaNextUpdate);

			StartCoroutine(WaitToRefresh());
		}

		void OnDestroy()
		{
			Debug.LogWarning ("WE have destroy the iGUI IngredientsSelectScreenNew screen");
			UnloadButtons ();
			_interface = null;
			_buttonHandler.ReleasedButtonEvent -= HandleReleasedButtonEvent;
			_buttonHandler.InputCancelledEvent -= HandleReleasedButtonEvent;
			_buttonHandler.MovedAway -= HandleMovedAway;
			_buttonHandler.MovedBack -= HandleMovedBack;
			_buttonHandler = null;
			Resources.UnloadUnusedAssets ();
		}

		void HandleButtonsLoaded()
		{
			++_loadedShelves;
		}

		void UnloadButtons()
		{
			for(int i = 0; i < _shelves.Count; ++i)
			{
				var currentShelf = _shelves[i];
				currentShelf.arrow_left.clickDownCallback -= ClickInit;
				currentShelf.arrow_right.clickDownCallback -= ClickInit;
				var buttons = currentShelf.Ingredient_Buttons;
				for(int j = 0; j < buttons.Count; ++j)
				{
					var currentButton = buttons[j];
					currentButton.clickDownCallback -= ClickInit;
				}
			}
			for(int l = 0; l < _pressableButtons.Count; ++l)
			{
				var currentButton = _pressableButtons[l];
				currentButton.clickDownCallback -= ClickInit;
				DestroyObject (currentButton);
			}
			_pressableButtons.Clear ();
			_buttonArtMap.Clear ();
			_shelfButtons.Clear ();
		}

		void AssignButtonCallbacks()
		{
			_pressableButtons = new List<iGUIButton> ()
			{
				btn_bookmark,btn_tarot_card,_interface.arrow_up_hitbox,_interface.arrow_down_hitbox,_interface.rib_add_starstones_hitbox,
				_interface.btn_home
			};

			_buttonArtMap = new Dictionary<iGUIButton,iGUIElement>()
			{
				{btn_bookmark,btn_bookmark.getTargetContainer()},
				{btn_tarot_card,btn_tarot_card.getTargetContainer()},
				{_interface.arrow_up_hitbox,_interface.arrow_up},
				{_interface.arrow_down_hitbox,_interface.arrow_down},
				{_interface.rib_add_starstones_hitbox,_interface.rib_add_starstones_hitbox.getTargetContainer()},
				{_interface.btn_home,_interface.home}
			};

			_shelfButtons = new Dictionary<int,List<iGUIButton>>();
//			Debug.Log("Shelf buttons has been initialzied? :: " + (_shelfButtons != null).ToString());

			for(int i = 0; i < _shelves.Count; ++i)
			{
				var shelfButtons = new List<iGUIButton>();
				var currentShelf = _shelves[i];
//				Debug.LogWarning(currentShelf.name);
				currentShelf.arrow_left.clickDownCallback += ClickInit;
				currentShelf.arrow_right.clickDownCallback += ClickInit;
				shelfButtons.Add(currentShelf.arrow_left);
				shelfButtons.Add(currentShelf.arrow_right);
				_buttonArtMap[currentShelf.arrow_left] = currentShelf.arrow_left.getTargetContainer();
				_buttonArtMap[currentShelf.arrow_right] = currentShelf.arrow_right.getTargetContainer();
				var buttons = currentShelf.Ingredient_Buttons;
				for(int j = 0; j < buttons.Count; ++j)
				{
					var currentButton = buttons[j];
//					Debug.LogWarning(currentButton.name);
					currentButton.clickDownCallback += ClickInit;
					_buttonArtMap[currentButton] = currentButton.getTargetContainer();
					shelfButtons.Add(currentButton);
				}

				_shelfButtons[i] = shelfButtons;
			}

			for(int l = 0; l < _pressableButtons.Count; ++l)
			{
				var currentButton = _pressableButtons[l];
				currentButton.clickDownCallback += ClickInit;
			}
		}

		//HACK There was a problem returning from MiniGame that made the text disappear
		IEnumerator WaitToRefresh()
		{
			iGUIRoot.instance.setEnabled(false);
			yield return new WaitForEndOfFrame();
			iGUIRoot.instance.setEnabled(true);

			if(_loadedShelves == _shelves.Count)
			{
				AssignButtonCallbacks();
			}
		}

		iGUIElement LoadPlaceholder()
		{
			var element = interfaceBarPlaceholder.SwapForSmartObject() as iGUIContainer;
			element.setLayer(15);
			return element;
		}

		protected virtual void StartScreen()
		{
			_selectedIngredients = new List<Ingredient>();

			LoadPlaceholders();
			InitRecipeValues();
			InitShelves();
			var requirementCount = _controller.RequirementCount;

			for(int i = 0; i < _shelves.Count; ++i)
			{
				var shelf = _shelves[i];
				if(i < requirementCount)
				{
					HandleSelectionChange(shelf,new GUIEventArgs());
				}
				else
				{
					shelf.GetComponent<iGUIElement>().setEnabled(false);
				}
			}
		}

		public override void MakePassive(bool value)
		{
			base.MakePassive(value);

			if (value) 
			{
				_buttonHandler.Deactivate ();
			} 
			else 
			{
				_buttonHandler.Activate ();
			}
		}

		private DifficultyMap MapDifficulty(MasterConfiguration masterConfig)
		{
			var difficulty = masterConfig.Game_Properties_Config.Mini_Game_Difficulty;

			var easy = difficulty[difficultyMapKeys[0]]["high"];
			var normal = difficulty[difficultyMapKeys[1]]["high"];
			var tricky = difficulty[difficultyMapKeys[2]]["high"];
			var hard = difficulty[difficultyMapKeys[3]]["high"];
			var trouble = difficulty[difficultyMapKeys[4]]["high"];

//			object[] obs = new object[] { trouble, hard, tricky, normal, easy };
//			var message = string.Format("TROUBLE {0}, HARD {1}, TRICKY {2}, NORMAL {3}, EASY {4}",obs);
//			Debug.LogWarning(message);

			return new DifficultyMap(new float[] { trouble, hard, tricky, normal, easy });
		}

		void HandleSystemDialog(string message)
		{

			IDialog dialog = _controller.GetSystemDialog(message);
			dialog.Display(HandleCloseSystemDialog);
		}

		void HandleCloseSystemDialog(int answer)
		{
			if(!_buttonHandler.IsActive)
			{
				_buttonHandler.Activate();
			}
		}

		private void LoadPlaceholders()
		{
			if (_shelfContainers != null && _shelves != null)
			{
				return;
			}
			
			_shelfContainers = new List<iGUIContainer>();
			_shelves = new List<iGUISmartPrefab_IngredientShelfNew>();
//			Debug.Log("Shelves has been initialzied? :: " + (_shelves != null).ToString());
			
			foreach (var item in shelves.items)
			{
				var ph = item as Placeholder;
				var container = ph.SwapForSmartObject() as iGUIContainer;
				var shelf = container.GetComponent<iGUISmartPrefab_IngredientShelfNew>();
				
				_shelfContainers.Add(container);
				_shelves.Add(shelf);
				shelf.OnComplete += HandleButtonsLoaded;
			}
			
			LoadDifficultyView();
		}
		
		private void LoadDifficultyView()
		{
			var container = difficultyPlaceholder.SwapForSmartObject() as iGUIContainer;
			_difficultyView = container.GetComponent<iGUISmartPrefab_DifficultyViewNew>();
		}
		
		private void InitRecipeValues()
		{
			IRecipe recipe = _controller.GetRecipe();
			Potion_name_header.label.text = recipe.Name;

			{
				hint_text_label.label.text = recipe.Hint;
			}

		}
		
		private void InitShelves()
		{
			var requirementCount = _controller.RequirementCount;

			for (int i = 0; i < requirementCount; ++i)
			{
//				IngredientCategory category = _controller.GetIngredientCategory(i);
				List<KeyValuePair<Ingredient, int>> ingredients = _controller.GetIngredientEntries(i);
//				_shelves[i].SetCategory(category);
				_shelves[i].SetIngredients(ingredients);
				_shelves[i].Init();
				_shelves[i].AddPurchaseRequestHandler(HandlePurchaseRequest);
				_shelves[i].SelectionChanged += HandleSelectionChange;
					
				_selectedIngredients.Add(_shelves[i].GetSelectedIngredient().Key);
			}
		}

		public void UpdateIngredientsInShelves()
		{
			var count = _controller.RequirementCount;

			for(int i = 0; i < count; ++i)
			{
				List<KeyValuePair<Ingredient, int>> ingredients = _controller.GetIngredientEntries(i);
				_shelves[i].SetIngredients(ingredients);
				_shelves[i].UpdateView();
			}
		}

		public void Init(IngredientsSelectScreenController controller,Player player, MasterConfiguration masterConfig)
		{
			_controller = controller;
			_player = player;
			_difficultyMap = MapDifficulty(masterConfig);

			StartScreen();
		}
		
		protected override IScreenController GetController()
		{
			return _controller;
		}

		bool ShouldTreatAsNormal(iGUIButton button)
		{
			return ((IsIngredientButton (button.name)) || (IsArrowButton (button.name)) || (button == btn_tarot_card) || (button == btn_bookmark)); 
		}

		bool IsIngredientButton(string buttonName)
		{
			return (buttonName.Contains("buy_ingredient_btn"));
		}

		bool IsArrowButton(string buttonName)
		{
			return ((buttonName.Contains("arrow_right")) || (buttonName.Contains("arrow_left")));
		}

		void ClickInit(iGUIElement element)
		{
			if((_buttonHandler.IsActive) && (_buttonHandler.PressedButton == null))
			{
				var button = (iGUIButton)element;
				_buttonHandler.SelectButton(button);
				_buttonArtMap[button].colorTo(Color.grey,0f);
			}
		}

		void HandleMovedAway(iGUIButton button)
		{
			_buttonArtMap[button].colorTo(Color.white,0.3f);
		}

		void HandleMovedBack(iGUIButton button)
		{
			_buttonArtMap[button].colorTo(Color.grey,0f);
		}

		void HandleReleasedButtonEvent(iGUIButton button, bool isOverButton)
		{
			if(isOverButton)
			{
				if(!ShouldTreatAsNormal(button))
				{
					if(button == _interface.arrow_up_hitbox)
					{
						StartCoroutine(_interface.UpdateRibbonPosition (Voltage.Witches.Views.InterfaceShellView.RibbonState.CLOSED));
					}
					else if(button == _interface.arrow_down_hitbox)
					{
						StartCoroutine(_interface.UpdateRibbonPosition (Voltage.Witches.Views.InterfaceShellView.RibbonState.OPEN));
					}
					else if(button == _interface.btn_home)
					{
						_controller.GoHome();
					}
					else if(button == _interface.rib_add_starstones_hitbox)
					{
						_buttonHandler.Deactivate();
						_controller.ShowCurrencyPurchaseDialog();
					}
				}
				else if((!IsIngredientButton(button.name)) && (IsArrowButton(button.name)))
				{
					ExecuteArrowClick(button);
				}
				else if((IsIngredientButton(button.name)) && (!IsArrowButton(button.name)))
				{
					ExecuteIngredientClick(button);
				}
				else if(button == btn_tarot_card)
				{
					Confirm_Ingredients();
				}
				else if(button == btn_bookmark)
				{
					_controller.GoBackToBookshelf();
				}
			}

			_buttonArtMap[button].colorTo(Color.white,0.3f);
		}

		void ExecuteArrowClick(iGUIButton button)
		{
			foreach(var pair in _shelfButtons)
			{
				var buttons = pair.Value;
				if(buttons.Contains(button))
				{
					var index = pair.Key;
					var shelf = _shelves[index];
					if(button.name.Contains("right"))
					{
						shelf.RotateRight();
					}
					else
					{
						shelf.RotateLeft();
					}
				}
			}
		}

		void ExecuteIngredientClick(iGUIButton button)
		{
			foreach(var pair in _shelfButtons)
			{
				var buttons = pair.Value;
				if(buttons.Contains(button))
				{
					var index = pair.Key;
					var shelf = _shelves[index];
					shelf.ExecuteButtonPress(button);
				}
			}
		}

		void Confirm_Ingredients()
		{
			bool requirementsMet = true;
			var requirementCount = _controller.RequirementCount;
			
			for(int i = 0; i < requirementCount; ++i)
			{
				var shelf = _shelves[i];
				KeyValuePair<Ingredient, int> selectedIngredient = shelf.GetSelectedIngredient();
				if(selectedIngredient.Value == 0)
				{
					shelf.DisplayIngredientAlert();
					requirementsMet = false;
				}
			}

			if (!requirementsMet)
			{
				return;
			}
			
			if(_controller.HasSufficientFocus())
			{
				_buttonHandler.Deactivate();
				IDialog dialog = _controller.GetConfirmationDialog();
				dialog.Display(OnDialogResponse);
			}
			else
			{
				_buttonHandler.Deactivate();
				IDialog dialog = _controller.GetInsufficientFocusDialog();
				dialog.Display(OnInsufficientFocusResponse);
			}
		}
		
		private void OnDialogResponse(int answer)
		{
			if ((DialogResponse)answer == DialogResponse.OK)
			{
				_controller.LoadNextScreen();
			}
			_buttonHandler.Activate();
		}
		
		private void OnInsufficientFocusResponse(int answer)
		{
			if((InsufficientFocusResponse)answer == InsufficientFocusResponse.RegainFocus)
			{
				_controller.RestoreFocus();
			}
			_buttonHandler.Activate();
		}

		private void HandlePurchaseRequest(object sender, GUIEventArgs args)
		{
			IngredientPurchaseRequestEventArgs ingredientArgs = args as IngredientPurchaseRequestEventArgs;
			Debug.Log(ingredientArgs.RequestedIngredient.Name);
			_requestedIngredient = ingredientArgs.RequestedIngredient;
			IDialog dialog = _controller.GetIngredientPurchaseDialog(_requestedIngredient);
			_buttonHandler.Deactivate();
			dialog.Display(OnPurchaseResponse);
		}

		void HandleCloseCurrencyDialog(int answer)
		{
			if(!_buttonHandler.IsActive)
			{
				_buttonHandler.Activate();
			}
		}
			
		private void HandleHomeNavigation(object sender, GUIEventArgs args)
		{
			Debug.Log("Navigate to Home");
			_controller.GoHome();
		}
		
		private void HandleSelectionChange(object sender, GUIEventArgs args)
		{
			var shelf = sender as IngredientShelfViewNew;
			var ingredient = shelf.GetSelectedIngredient();
				
			// locate which shelf send this
			int shelfIndex = _shelves.IndexOf(shelf as iGUISmartPrefab_IngredientShelfNew);
			_selectedIngredients[shelfIndex] = ingredient.Key;
				
			string selectedIngredients = "";
			foreach(var ele in _selectedIngredients)
			{
				selectedIngredients += ele.Name + ", ";
			}
//			Debug.LogWarning("Selected ingredients are: " + selectedIngredients);
				
			// recompute difficulty based on ingredients
			IRecipe recipe = _controller.GetRecipe();
			var numberOfIngredients = recipe.IngredientRequirements.Count;
			float rawDifficulty = 0.0f;
			switch(numberOfIngredients)
			{
				case 1:
					rawDifficulty = recipe.GetDifficulty(_selectedIngredients[0]);
					break;
				case 2:
					rawDifficulty = recipe.GetDifficulty(_selectedIngredients[0], _selectedIngredients[1]);
					break;
				case 3:
					rawDifficulty = recipe.GetDifficulty(_selectedIngredients[0], _selectedIngredients[1], _selectedIngredients[2]);
					break;
			}

//			Debug.LogWarning("Raw difficulty is: " + rawDifficulty);
				
			MiniGameDifficulty difficultyStatus = _difficultyMap.GetDifficulty(rawDifficulty);
//			Debug.LogWarning("Difficulty value is: " + difficultyStatus);
			_difficultyView.SetDifficulty(difficultyStatus);
		}
		
		private void OnPurchaseResponse(int answer)
		{
			var isPurchase = (((PurchaseResponse)answer) != PurchaseResponse.Close);
			if(isPurchase)
			{
				var isPremium = (((PurchaseResponse)answer) == PurchaseResponse.PremiumPurchase);
				_controller.InitiateIngredientPurchase(_requestedIngredient, isPremium);
			}
			_requestedIngredient = null;
			_buttonHandler.Activate();
		}

		public void UpdateInterfaceBar()
		{
			if(_interface != null)
			{
				_interface.SetCounts(_player);
			}
		}

		//TODO This should probably be changed to send this to the controller
		public List<Ingredient> GetSelectedIngredients
		{
			get { return _selectedIngredients; }
		}

		
		public void SetEnabled(bool value)
		{
			screenFrame.setEnabled(value);
			gameObject.SetActive(value);
		}
		
		public void Unload()
		{
			_controller.Unload();
			GameObject.Destroy(gameObject);
		}
	}
}