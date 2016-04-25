using UnityEngine;
using iGUI;
using Voltage.Witches.Controllers;
using System;
using System.Collections.Generic;
using Voltage.Witches.Models;
using Voltage.Witches.Events;
using Voltage.Witches.Views;
using Voltage.Witches.Components;
using Voltage.Witches.Lib;

namespace Voltage.Witches.Screens
{
	using Debug = UnityEngine.Debug;

	public class InventoryScreen : BaseScreen 
	{
		[HideInInspector]
		public Placeholder interfaceBarPlaceholder,potionShelf,ingredientShelf;

		private iGUISmartPrefab_InterfaceShell _interface;
		private iGUISmartPrefab_InventoryPotionShelf _potionShelf;
		private iGUISmartPrefab_InventoryIngredientShelf _ingredientShelf;

		[HideInInspector]
		public iGUIButton ingredients_button,potions_button;

		[HideInInspector]
		public iGUIContainer potion_tab, ingredient_tab;

		[SerializeField]
		private ActiveItem _currentItem = ActiveItem.POTIONS;

		private enum ButtonState
		{
			ACTIVE = 0,
			GRAYED = 1
		}

		private enum ActiveItem
		{
			INGREDIENTS = 0,
			POTIONS = 1
		}

		Player _player;
		private InventoryScreenController _controller;
		IGUIHandler _buttonHandler;
		Dictionary<iGUIButton,iGUIElement> _buttonArtMap;
		int _shelvesLoaded;

		protected virtual void Awake()
		{
			_buttonHandler = gameObject.AddComponent<IGUIHandler>();
			_buttonHandler.MovedAway += HandleMovedAway;
			_buttonHandler.MovedBack += HandleMovedBack;
			_buttonHandler.ReleasedButtonEvent += HandleReleasedButtonEvent;
			_buttonHandler.InputCancelledEvent += HandleReleasedButtonEvent;
		}

		protected virtual void Start()
		{
			LoadPlaceholders();

			UpdateButtons();
			UpdateViews();
		}

		void LoadPlaceholders ()
		{
			LoadPotionShelf();
			LoadInventoryShelf();

			_interface = LoadPlaceholder().GetComponent<iGUISmartPrefab_InterfaceShell>();
			_interface.SetLayout(Voltage.Witches.Views.InterfaceLayout.Standard);
			_interface.SetCounts(_player);
			_interface.BeginCountDown(CountDownType.FOCUS, _player.FocusNextUpdate);
			_interface.BeginCountDown(CountDownType.STAMINA, _player.StaminaNextUpdate);
		}

		void LoadPotionShelf ()
		{
			var layer = potionShelf.layer;
			var container = potionShelf.SwapForSmartObject() as iGUIContainer;
			if(container != null)
			{
				_potionShelf = container.GetComponent<iGUISmartPrefab_InventoryPotionShelf>();
				_potionShelf.SetUpView(_controller.PotionCount);
				_potionShelf.OnPotionSelection += HandlePotionSelection;
				_potionShelf.PotionViewsSetUp += HandlePotionsSetUp;
				_potionShelf.GetComponent<iGUIElement>().setLayer(layer);
			}
		}

		iGUIElement LoadPlaceholder()
		{
			var element = interfaceBarPlaceholder.SwapForSmartObject() as iGUIContainer;
			element.setLayer(15);
			return element;
		}

		void LoadInventoryShelf ()
		{
			var layer = ingredientShelf.layer;
			var container = ingredientShelf.SwapForSmartObject() as iGUIContainer;
			if(container != null)
			{
				_ingredientShelf = container.GetComponent<iGUISmartPrefab_InventoryIngredientShelf>();
				_ingredientShelf.SetUpView(_controller.IngredientCount);
				_ingredientShelf.OnIngredientSelection += HandleIngredientSelection;
				_ingredientShelf.IngredientViewsSetUp += HandleIngredientsSetUp;
				_ingredientShelf.GetComponent<iGUIElement>().setLayer(layer);
			}
		}

		void HandlePotionsSetUp(object sender, GUIEventArgs e)
		{
			SetUpPotions();
			++_shelvesLoaded;
			if(_shelvesLoaded == 2)
			{
				AssignButtonCallbacks();
			}
		}

		void HandleIngredientsSetUp(object sender, GUIEventArgs e)
		{
			SetUpIngredients();
			++_shelvesLoaded;
			if(_shelvesLoaded == 2)
			{
				AssignButtonCallbacks();
			}
		}

		void AssignButtonCallbacks()
		{
			if(_buttonArtMap != null)
			{
				_buttonArtMap.Clear();
			}
			_buttonArtMap = new Dictionary<iGUIButton,iGUIElement> ()
			{
				{ingredients_button,ingredient_tab},
				{potions_button,potion_tab},
				{_interface.arrow_down_hitbox,_interface.arrow_down},
				{_interface.arrow_up_hitbox,_interface.arrow_up},
				{_interface.btn_home,_interface.home},
				{_interface.rib_add_starstones_hitbox,_interface.rib_add_starstones_hitbox.getTargetContainer()}
			};

			List<iGUIButton> allButtons = new List<iGUIButton> ()
			{
				ingredients_button,potions_button,_interface.arrow_down_hitbox,_interface.arrow_up_hitbox,_interface.btn_home,
				_interface.rib_add_starstones_hitbox
			};

			var potionButtons = _potionShelf.Potion_Buttons;
			var ingredientButtons = _ingredientShelf.Ingredient_Buttons;

			foreach(iGUIButton button in potionButtons)
			{
				_buttonArtMap[button] = button.getTargetContainer();
			}

			foreach(iGUIButton button in ingredientButtons)
			{
				_buttonArtMap[button] = button.getTargetContainer();
			}

			allButtons.AddRange(potionButtons);
			allButtons.AddRange(ingredientButtons);

			for(int i = 0; i < allButtons.Count; ++i)
			{
				var button = allButtons[i];
				button.clickDownCallback += ClickInit;
			}
		}

		void SetUpPotions()
		{
			if(_controller.PotionCount < 1)
			{
				_potionShelf.Potions_Container.setEnabled(false);
			}
			else
			{
				_potionShelf.Potions_Container.setEnabled(true);
			}
			if(_controller.PotionCount != _potionShelf.Potions_Container.itemCount)
			{
				_potionShelf.UpdateView(_controller.PotionCount);
			}


			for(int i = 0; i < _potionShelf.Potions_Container.itemCount; ++i)
			{
				if(i < _controller.PotionCount)
				{
					var potion = _controller.GetPotionFromIndex(i);
	                var itemCount = _controller.GetItemCount(potion);
					if(itemCount > -1)
					{
						_potionShelf.SetPotion(i,potion,itemCount);
					}
				}
				else
				{
					_potionShelf.HidePotion(i);
				}
			}
		}

		void SetUpIngredients()
		{
			if(_controller.IngredientCount < 1)
			{
				_ingredientShelf.Ingredients_Container.setEnabled(false);
			}

			for(int i = 0; i < _controller.IngredientCount; ++i)
			{
				var ingredient = _controller.GetIngredientFromIndex(i);
                var itemCount = _controller.GetItemCount(ingredient);
				if(itemCount > 0)
				{
					_ingredientShelf.SetIngredient(i,ingredient,itemCount);
				}
			}
		}

		public void UpdatePotions()
		{
			SetUpPotions();
			AssignButtonCallbacks();
		}

		public void UpdateInterfaceBar()
		{
			if(_interface != null)
			{
				_interface.SetCounts(_player);
			}
		}

		public void Init(InventoryScreenController controller,Player player, int activeItem)
		{
			_controller = controller;
			_player = player;
			_currentItem = (ActiveItem)activeItem;
		}

		protected override IScreenController GetController()
		{
			return _controller;
		}

		public override void MakePassive(bool value)
		{
			base.MakePassive(value);
			ToggleScrollView(!value);

			if (value) 
			{
				_buttonHandler.Deactivate ();
			} 
			else 
			{
				_buttonHandler.Activate ();
			}
		}

		void HandleCurrencyPurchase(int answer)
		{
			switch((CurrencyPurchaseResponse)answer)
			{
			case CurrencyPurchaseResponse.CLOSE:
				ToggleScrollView(true);
				_buttonHandler.Activate();
				break;
			}
		}
			

		private void HandleHomeNavigation(object sender, GUIEventArgs args)
		{
			Debug.Log("Navigate to Home");
			_controller.GoHome();
		}

		void HandleIngredientSelection(object sender, GUIEventArgs e)
		{
			ToggleScrollView(false);
			_buttonHandler.Deactivate();
			Debug.Log(sender.ToString() + " was selected, do shit");
			var IngredientArgs = e as IngredientPurchaseRequestEventArgs;
			var Ingredient = IngredientArgs.RequestedIngredient;
			var dialog = _controller.GetIngredientDetails(Ingredient);
			dialog.Display(HandleIngredientResponse);
		}

		void HandlePotionSelection(object sender, GUIEventArgs e)
		{
			var PotionArgs = e as PotionSelectedEventArgs;
			var potion = PotionArgs.Potion;
			Debug.Log(sender.ToString() + " was selected, get the appropriate dialog"); 
			Debug.Log(potion.Name + " was selected");
			Debug.Log(potion.Description);
			ToggleScrollView(false);
			_buttonHandler.Deactivate();
			if(potion.PotionCategory != PotionCategory.STAMINA)
			{
				var dialog = _controller.GetPotionDetails(potion);
				dialog.Display(HandlePotionDetailResponse);
			}
			else
			{
				var dialog = _controller.GetStaminaPotionInfoDialog();
				dialog.Display(HandleStaminaDialogResponse);
			}
		}

		void HandleIngredientResponse(int answer)
		{
			ToggleScrollView(true);
			_buttonHandler.Activate();
		}

		void HandlePotionDetailResponse(int answer)
		{
			switch((PotionDetailResponse)answer)
			{
			case PotionDetailResponse.BREW:
				_controller.GoToMiniGame();
				_buttonHandler.Activate();
				ToggleScrollView(true);
				break;
			case PotionDetailResponse.USE:
				_controller.InitiatePotionUsage();
				break;
			case PotionDetailResponse.CLOSE:
				_buttonHandler.Activate();
				ToggleScrollView(true);
				break;
			}
		}

		void HandleStaminaDialogResponse(int answer)
		{
			_buttonHandler.Activate();
			ToggleScrollView(true);
		}

		public void HandlePotionUsedResponse(int answer)
		{
			switch((DialogResponse)answer)
			{
				case DialogResponse.Cancel:
					_controller.GotToStoryMap();
					break;
				case DialogResponse.OK:
					SetUpPotions();
					break;
			}
			_buttonHandler.Activate();
			ToggleScrollView(true);
		}

		void UpdateViews()
		{
			var ingredientShelfElement = _ingredientShelf.GetComponent<iGUIElement>();
			var potionShelfElement = _potionShelf.GetComponent<iGUIElement>();

			switch(_currentItem)
			{
				case ActiveItem.INGREDIENTS:
					if(!ingredientShelfElement.enabled)
					{
						ingredientShelfElement.setEnabled(true);
					}
					if(potionShelfElement.enabled)
					{
						potionShelfElement.setEnabled(false);
					}
					break;
				case ActiveItem.POTIONS:
					if(ingredientShelfElement.enabled)
					{
						ingredientShelfElement.setEnabled(false);
					}
					if(!potionShelfElement.enabled)
					{
						potionShelfElement.setEnabled(true);
					}
					break;
			}
		}

		void ToggleScrollView(bool value)
		{
			Debug.Log("Toggle the scroll views");
			switch(_currentItem)
			{
				case ActiveItem.INGREDIENTS:
						_ingredientShelf.scrollAble.setEnabled(value);
					break;
				case ActiveItem.POTIONS:
						_potionShelf.scrollAble.setEnabled(value);
					break;
			}
		}

		void UpdateButtons()
		{
			switch(_currentItem)
			{
				case ActiveItem.INGREDIENTS:
					SetContainerOrder(ingredient_tab,ButtonState.GRAYED);
					SetContainerOrder(potion_tab,ButtonState.ACTIVE);
					break;
				case ActiveItem.POTIONS:
					SetContainerOrder(ingredient_tab,ButtonState.ACTIVE);
					SetContainerOrder(potion_tab,ButtonState.GRAYED);
					break;
			}
		}

		void SetContainerOrder(iGUIElement container, ButtonState buttonState)
		{
			int order = (buttonState == ButtonState.ACTIVE) ? 7 : 8;
			container.setLayer(order);
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

		bool ShouldTreatAsStandardButton(iGUIButton button)
		{
			return ((IsIngredientButton(button.name)) || (IsPotionButton(button.name)));
		}

		bool IsIngredientButton(string buttonName)
		{
			return (buttonName.Contains("ingredient_button"));
		}

		bool IsPotionButton(string buttonName)
		{
			return (buttonName.Contains("potion_button"));
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
				if(!ShouldTreatAsStandardButton(button))
				{
					if(button == _interface.arrow_down_hitbox)
					{
						StartCoroutine(_interface.UpdateRibbonPosition (Voltage.Witches.Views.InterfaceShellView.RibbonState.OPEN));
					}
					else if(button == _interface.arrow_up_hitbox)
					{
						StartCoroutine(_interface.UpdateRibbonPosition (Voltage.Witches.Views.InterfaceShellView.RibbonState.CLOSED));
					}
					else if(button == _interface.rib_add_starstones_hitbox)
					{
						_buttonHandler.Deactivate();
						_controller.ShowCurrencyPurchaseDialog();
					}
					else if(button == _interface.btn_home)
					{
						_controller.GoHome();
					}
					else if(button == ingredients_button)
					{
						Debug.Log("Ingredients Button clicked");
						if(_currentItem != ActiveItem.INGREDIENTS)
						{
							_currentItem = ActiveItem.INGREDIENTS;
							UpdateViews();
							UpdateButtons();
						}
					}
					else if(button == potions_button)
					{
						Debug.Log("Potions button clicked");
						if(_currentItem != ActiveItem.POTIONS)
						{
							_currentItem = ActiveItem.POTIONS;
							UpdateViews();
							UpdateButtons();
						}
					}
				}
				else
				{
					if(IsIngredientButton(button.name))
					{
						_ingredientShelf.ExecuteIngredientClick(button);
					}
					else
					{
						_potionShelf.ExecuteButtonClick(button);
					}
				}
			}

			_buttonArtMap[button].colorTo(Color.white,0.3f);
		}
	}
}