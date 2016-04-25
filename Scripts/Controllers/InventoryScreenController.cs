using Ninject;
using Ninject.Modules;
using Voltage.Witches.DI;

using System;
using System.Collections.Generic;
using UnityEngine;
using Voltage.Witches;
using Voltage.Witches.Models;
using Voltage.Witches.Net;
using Voltage.Witches.Screens;

namespace Voltage.Witches.Controllers
{
	using Voltage.Witches.Configuration;
	using Voltage.Witches.Shop;
    using Voltage.Witches.Controllers.Factories;
	using Voltage.Common.Net;
	using URLs = Voltage.Witches.Net.URLs;

	public class InventoryScreenController : ScreenController
	{
		private IScreenFactory _factory;
		private Player _player;
        private readonly Inventory _inventory;
        private IControllerRepo _repo;

		public int PotionCount
		{
			get { return _availablePotions.Count; }
		}

		public int IngredientCount
		{
			get { return _availableIngredients.Count; }
		}

		private List<Potion> _availablePotions;

		private int _activeView = 1;
		private iGUISmartPrefab_InventoryScreen _screen;

		private List<Ingredient> _availableIngredients;

		private INetworkTimeoutController<WitchesRequestResponse> _networkController;

		public ShopController ShopController { get; protected set; }

		private ShopItemData _iapItem;
		private IDialog _completingPurchase;
		private Potion _queuedPotion;
		private bool _playerHadNoStaminaPotions;

		private readonly ShopDialogueController _shopDialogController;

        public InventoryScreenController(ScreenNavigationManager controller, IScreenFactory factory, Player player, Inventory inventory, 
			int activeView, IControllerRepo repo, MasterConfiguration masterConfig, ShopDialogueController shopDialogController):base(controller)
		{
			_factory = factory;
			_player = player;
            _inventory = inventory;
            _repo = repo;
			_activeView = activeView;

			_player.FocusUpdate += HandleFocusUpdate;
			_player.StaminaUpdate += HandleStaminaUpdate;

			_networkController = _repo.Get<INetworkTimeoutController<WitchesRequestResponse>>();
			ShopController = _repo.Get<ShopController>();
			_shopDialogController = shopDialogController;

			List<Item> allIngredients = inventory.GetAllItemsByCategory(ItemCategory.INGREDIENT);
			List<Item> allPotions = inventory.GetAllItemsByCategory(ItemCategory.POTION);

			_availablePotions = new List<Potion>();
			_availableIngredients = new List<Ingredient>();

			BuildItemListTemplate<Potion>(allPotions,_availablePotions);
			BuildItemListTemplate<Ingredient>(allIngredients,_availableIngredients);
			if(_player.StaminaPotions > 0)
			{
				AddStaminaPotion();
			}
			else
			{
				_playerHadNoStaminaPotions = true;
			}

			InitializeView();
		}

		void HandleStaminaUpdate(object sender, EventArgs e)
		{
			_screen.UpdateInterfaceBar();
		}
		
		void HandleFocusUpdate(object sender, EventArgs e)
		{
			_screen.UpdateInterfaceBar();
		}

        public override void Dispose()
        {
            // TODO: Not calling the base because the base is calling GetScreen, which will force us to generate another screen then delete and then set to null.
            // We need to find out if we can safely turn all GetScreen to return the screen and not make one if one doesn't exist.
            if (_screen != null)
            {
                _player.FocusUpdate -= HandleFocusUpdate;
                _player.StaminaUpdate -= HandleStaminaUpdate;
                _screen.Dispose();
                _screen = null;
            }
        }

		protected override IScreen GetScreen()
		{
			if(_screen != null)
			{
				return _screen;
			}
			else
			{
				_screen = _factory.GetScreen<iGUISmartPrefab_InventoryScreen>();
				_screen.Init(this,_player,_activeView);
				return _screen;
			}
		}

		public override void MakePassive(bool value)
		{
			_screen.MakePassive(value);
		}
		
		private void InitializeView()
		{
			_screen = _factory.GetScreen<iGUISmartPrefab_InventoryScreen>();
			_screen.Init(this,_player,_activeView);
		}

		void BuildItemListTemplate<T>(List<Item> allItems,List<T> outputList) where T : Item
		{
			for(int i = 0; i < allItems.Count; ++i)
			{
				T current = allItems[i] as T;
				if((current != null) && (!outputList.Contains(current)))
				{
					outputList.Add(current);
				}
			}
		}

		void AddStaminaPotion()
		{
			Potion staminaPotion = new Potion("Stamina Potion", "Stamina Potion", "It's a stamina potion", string.Empty, new Dictionary<string,int>());
			_availablePotions.Add(staminaPotion);
			_playerHadNoStaminaPotions = false;
		}

        public int GetItemCount(Item item)
        {
			if(item.Name != "Stamina Potion")
			{
            	return _inventory.GetCount(item);
			}

			return _player.StaminaPotions;
        }

		public Ingredient GetIngredientFromIndex(int index)
		{
			return _availableIngredients[index];
		}

		public Potion GetPotionFromIndex(int index)
		{
			return _availablePotions[index];
		}

		private IRecipe GetRecipeFromBook(Potion potion)
		{
			var books = _player.GetBooks();
			for(int i = 0; i < books.Count; ++i)
			{
				var book = books[i];
				var recipes = book.Recipes;
				for(int j = 0; j < recipes.Count; ++j)
				{
					var recipe = recipes[j];
					if(recipe.ContainsRecipeForPotion(potion))
					{
						return recipe;
					}
				}
			}

			return null;
		}

		public void UsePotion(Potion potion)
		{
			int total = 0;

			foreach(var pair in potion.EffectList)
			{
				_player.AddAffinity(pair.Key,pair.Value);
				total += pair.Value;
			}
//			_player.CurrentAffinity.UpdateAffinity(total);


            _inventory.Remove(potion, 1);
			if((_inventory.GetCount(potion)) < 1)
			{
				_availablePotions.Remove(potion);
				_screen.UpdatePotions();
			}

			SendUsedPotionMetric (potion);
		}

		public IDialog GetStaminaPotionInfoDialog()
		{
			return _factory.GetDialog<iGUISmartPrefab_StaminaPotionInfoDialog>();
		}

		public IDialog GetPotionUsed(Potion potion)
		{
			var dialog = _factory.GetDialog<iGUISmartPrefab_InventoryPotionUsedDialog>();
			dialog.SetPotion(potion);
			dialog.SetAffinityAndAlignment(GetCurrentPlayerValues());
			return dialog;
		}

		Dictionary<string,int> GetCurrentPlayerValues()
		{
			var effectKeys = new string[]{"T","M","A","N","R"};
			var currentValueMap = new Dictionary<string,int>();
			for(int i = 0; i < effectKeys.Length; ++i)
			{
				var key = effectKeys[i];
				{
					currentValueMap[key] = _player.GetAffinity(key);
				}

			}

			return currentValueMap;
		}

		public IDialog GetPotionDetails(Potion potion)
		{
			_queuedPotion = potion;
			var dialog = _factory.GetDialog<iGUISmartPrefab_InventoryPotionDetailsDialog>();
            var count = _inventory.GetCount(potion);
			dialog.SetPotion(potion, count);
			return dialog;
		}

		public IDialog GetIngredientDetails(Ingredient ingredient)
		{
			var dialog = _factory.GetDialog<iGUISmartPrefab_InventoryIngredientDetailsDialog>();
            var count = _inventory.GetCount(ingredient);
			dialog.SetIngredient(ingredient, count);
			return dialog;
		}


		public void ShowCurrencyPurchaseDialog()
		{
			_shopDialogController.Show (ShopDisplayType.STARSTONES, OnFinishTransaction);
		}

		public void OnFinishTransaction()
		{
			if(_playerHadNoStaminaPotions)
			{
				Debug.LogWarning("Adding Stamina potion now");
				AddStaminaPotion();
			}
			_screen.UpdatePotions();
			_screen.UpdateInterfaceBar();
			MakePassive (false);
		}


		public IDialog GetPrePurchaseLoadingDialog()
		{
			return _factory.GetDialog<iGUISmartPrefab_ConfirmPurchaseLoadDialog>();
		}
		
		public IDialog GetPostPurchaseLoadingDialog()
		{
			return _factory.GetDialog<iGUISmartPrefab_ConfirmCompleteLoadDialog>();
		}
		
		public IDialog GetSystemDialog(string message)
		{
			var dialog = _factory.GetDialog<iGUISmartPrefab_SystemPopupDialog>();
			dialog.SetText(message);
			return dialog;
		}

		void HandleSystemDialogResponse(int answer)
		{
			UnityEngine.Debug.Log("Close system dialog");
		}

		public void InitiatePotionUsage()
		{
			string userID = _player.UserID;
			if(string.IsNullOrEmpty(userID))
			{
				userID = PlayerPrefs.GetString("phone_id");
			}

			Dictionary<string,string> parameters = new Dictionary<string,string> ()
			{
				{"phone_id",userID},
				{"potion_id",_queuedPotion.Id}
			};

			_networkController.Send(URLs.USE_POTION,parameters,HandlePotionUsedSuccess,HandlePotionUsedFail);
		}

		void HandlePotionUsedSuccess(Voltage.Common.Net.WWWNetworkPayload payload)
		{
			//TODO Use the json to update the player state

			UsePotion(_queuedPotion);
			var dialog = GetPotionUsed(_queuedPotion);
			Action<int> responseHandler = delegate(int obj) { _screen.HandlePotionUsedResponse(obj); };
			dialog.Display(responseHandler);
			_queuedPotion = null;
		}

		void HandlePotionUsedFail(Voltage.Common.Net.WWWNetworkPayload payload)
		{
			UnityEngine.Debug.LogWarning("Potion was NOT used");
			var dialog = GetSystemDialog("There was an error using your potion");
			dialog.Display(HandleSystemDialogResponse);
			_queuedPotion = null;
		}

		public void SetEnabled(bool value)
		{
			_screen.SetEnabled(value);
		}
		
		public void Unload()
		{
			Manager.CloseCurrentScreen();
		}

		public ScreenController DisplayLoadingMarquee()
		{
			MainLoadingScreenControllerFactory factory = _repo.Get<MainLoadingScreenControllerFactory>();
			MainLoadingScreenController loading = factory.Create((int)MainLayoutType.LOADING_MARQUEE, null, null);
			loading.Show();
			return loading;
		}

		public void GotToStoryMap()
		{
			var loading = DisplayLoadingMarquee();
			Action<int> storyMapLoaded = (loading as MainLoadingScreenController).ProcessAction;
			IStoryMapScreenControllerFactory storyMapScreenFactory = _repo.Get<IStoryMapScreenControllerFactory>();
            StoryMapUGUIScreenController nextScreen = storyMapScreenFactory.CreateUGUI(_player,storyMapLoaded);	
			Manager.Add(nextScreen);
		}

		public void GoToMiniGame()
		{
			var recipe = GetRecipeFromBook(_queuedPotion);

            var factory = _repo.Get<IIngredientsSelectScreenControllerFactory>();
            IScreenController nextScreen = factory.Create(recipe);
			Manager.Add(nextScreen);
		}

		public void GoBackToMenu()
		{
			if(Manager.GetCurrentPath().Contains("/Home/Menu"))
			{
				Manager.GoToExistingScreen("/Home/Menu");
			}
			else
			{
				IScreenController nextScreen = _repo.Get<MenuScreenController>();
				Manager.Add(nextScreen);
			}
		}

		public void GoHome()
		{
			if(Manager.GetCurrentPath().Contains("/Home"))
			{
				Manager.GoToExistingScreen("/Home");
			}
			else
			{
				HomeScreenController nextScreen = _repo.Get<HomeScreenController>();
				Manager.Add(nextScreen);
			}
		}

		private void SendUsedPotionMetric(Potion potion)
		{
			IDictionary<string,object> data = new Dictionary<string,object>
			{
				{"potion_id", potion.Name},
				{"potion_num", _inventory.GetCount(potion)}
			};
			
			Voltage.Common.Metrics.AmbientMetricManager.Current.LogEvent (Voltage.Witches.Metrics.MetricEvent.INVENTORY_USED_POTION, data);
		}
	}
}