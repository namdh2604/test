using System;
using System.Collections.Generic;
using System.Linq;
using Voltage.Witches;
using Voltage.Witches.Configuration.JSON;
using Voltage.Witches.Models;
using Voltage.Witches.Screens;
using Voltage.Witches.Events;

namespace Voltage.Witches.Controllers
{
	using URLs = Voltage.Witches.Net.URLs;
	using Voltage.Witches.Net;
	using Voltage.Witches.Shop;
	using Voltage.Witches.Configuration;

	using Voltage.Common.Metrics;
	using Voltage.Witches.Metrics;

	using Voltage.Common.DebugTool.Timer;

	public interface IIngredientsSelectScreenController : IScreenController
	{
		List<KeyValuePair<Ingredient, int>> GetIngredientEntries(int ingredientIndex);
		bool HasSufficientFocus();
		void RestoreFocus();
		int RequirementCount { get; }

		IDialog GetConfirmationDialog();
		IDialog GetIngredientPurchaseDialog(Ingredient ingredient);
		IDialog GetInsufficientFocusDialog();
	};

	public class IngredientsSelectScreenController : ScreenController, IIngredientsSelectScreenController
	{
		public int RequirementCount { get { return _recipe.IngredientRequirements.Count; } }

		private IScreenFactory _factory;
		private Player _player;
		public Player Player { get { return _player; } }

        private readonly Inventory _inventory;

		private const int NUM_CATEGORIES = 3;

		private iGUISmartPrefab_IngredientsSelectScreenNew _screen;

		private IRecipe _recipe;
		private List<KeyValuePair<IngredientCategory, List<KeyValuePair<Ingredient, int>>>> _ingredients;

		private List<IngredientCategory> _availableIngredientCategories;
		private Dictionary<int,List<int>> _shelfQualities;

		private Dictionary<string,List<Ingredient>> _ingredientsByCategory;
	

		private MasterConfiguration _masterConfig;
		private IControllerRepo _repo;

		public ShopController ShopController { get; private set; }

//		private iGUISmartPrefab_CurrencyPurchaseDialog _iapDialog;
		private ShopItemData _iapItem;
		private Ingredient _queuedIngredient;
		private IDialog _completingPurchase;
		private readonly ShopDialogueController _shopDialogController;

		private enum IngredientPurchaseType
		{
			NONE = -1,
			FREE = 0,
			PREMIUM = 1
		}

		private IngredientPurchaseType _purchaseType = IngredientPurchaseType.NONE;

		public IngredientsSelectScreenController(ScreenNavigationManager navManager, IScreenFactory factory, Player player, Inventory inventory, 
			IRecipe selectedRecipe, IControllerRepo repo, MasterConfiguration masterConfig,ShopDialogueController shopDialogController): base(navManager)
		{
			_factory = factory;
			_player = player;
            _inventory = inventory;

			_player.StaminaUpdate += HandleStaminaUpdate;
			_player.FocusUpdate += HandleFocusUpdate;

			_masterConfig = masterConfig;
			_repo = repo;

			ShopController = _repo.Get<ShopController>();

			_shopDialogController = shopDialogController;
//			if(selectedRecipe != null)
//			{
//				Console.WriteLine(selectedRecipe.Name);
//				UnityEngine.Debug.Log(selectedRecipe.Name);
//			}

			var allIngredients = GetAllIngredientsFromData();

//			var ingreMessage = string.Format ("Ingredient Count: {0}", allIngredients.Count);
//			UnityEngine.Debug.LogWarning(ingreMessage);

			BuildIngredientMaster(allIngredients);

            _availableIngredientCategories = _inventory.GetAvailableIngredientCategories();

			{
				CreateRecipeUsingData(selectedRecipe);
				BuildIngredientList();

//				UnityEngine.Debug.Log("Built ingredients list from inventory yo!");
			}

			InitializeView();
		}

		public override void MakePassive(bool value)
		{
			_screen.MakePassive(value);
		}

		void HandleStaminaUpdate(object sender, EventArgs e)
		{
			_screen.UpdateInterfaceBar();
		}
		
		void HandleFocusUpdate(object sender, EventArgs e)
		{
			_screen.UpdateInterfaceBar();
		}

		List<Ingredient> GetAllIngredientsFromData()
		{
			List<Ingredient> allIngredients = new List<Ingredient>();

			var masterIngredients = _masterConfig.Ingredients;
			var IngredientParser = new ItemRawParser(_masterConfig);

			foreach(var pair in masterIngredients)
			{
				var currentData = pair.Value;
				Ingredient ingredient = IngredientParser.CreateIngredient(currentData) as Ingredient;
				
				allIngredients.Add(ingredient);
			}

			return allIngredients;
		}

		void BuildIngredientMaster(List<Ingredient> allIngredients)
		{
			_ingredientsByCategory = new Dictionary<string, List<Ingredient>>();

			for(int i = 0; i < allIngredients.Count; ++i)
			{
				var currentIngredient = allIngredients[i];
				var currentCategory = currentIngredient.IngredientCategory.Name;

				if(!_ingredientsByCategory.ContainsKey(currentCategory))
				{
					List<Ingredient> newList = new List<Ingredient>(){currentIngredient};
					_ingredientsByCategory[currentCategory] = newList;
				}
				else
				{
					var currentList = _ingredientsByCategory[currentCategory];
					currentList.Add(currentIngredient);
				}
			}

			foreach(var ingredientList in _ingredientsByCategory)
			{
				var currentSet = ingredientList.Value;
				currentSet.Sort((ingredient1,ingredient2) => ingredient1.DisplayOrder.CompareTo(ingredient2.DisplayOrder));
				if(ingredientList.Key == "Water")
				{
					for(int i = 0; i < currentSet.Count; ++i)
					{
//						string message = string.Format("INGREDIENT {0} DISPLAY ORDER {1}",currentSet[i].Name,currentSet[i].DisplayOrder);
//						UnityEngine.Debug.LogWarning(message);
					}
				}
			}
		}

		List<IngredientCategory> MakeListOfPossibleCategories ()
		{
			List<IngredientCategory> listOfCats = new List<IngredientCategory>();
			for(int i = 0; i < NUM_CATEGORIES; ++i)
			{
				int index = UnityEngine.Random.Range(0,_availableIngredientCategories.Count);
				IngredientCategory cat = _availableIngredientCategories[index];
				while(listOfCats.Contains(cat))
				{
					index = UnityEngine.Random.Range(0,_availableIngredientCategories.Count);
					cat = _availableIngredientCategories[index];
				}
				listOfCats.Add(cat);
			}

			return listOfCats;
		}

		private void CreateRecipeUsingData(IRecipe recipe)
		{
			if(!recipe.HasIngredientRequirements)
			{
				UnityEngine.Debug.Log("Recipe does not have ingredient requirements");
				Recipe newRecipe = new Recipe(recipe.Name);
				if(!String.IsNullOrEmpty(recipe.Hint))
				{
					newRecipe.Hint = recipe.Hint;
				}
				else
				{
					newRecipe.Hint = recipe.Name + " is the recipe you have selected. It doesn't have a hint yet, but just you wait!";
				}

				List<IngredientCategory> possibleCategories = MakeListOfPossibleCategories ();
				for (int i = 0; i < NUM_CATEGORIES; ++i)
				{
					switch(i)
					{
					case 0:
						newRecipe.IngredientRequirements.Add(new IngredientRequirement(possibleCategories[i], 100));
						break;
					case 1:
						newRecipe.IngredientRequirements.Add(new IngredientRequirement(possibleCategories[i], 50));
						break;
					case 2:
						newRecipe.IngredientRequirements.Add(new IngredientRequirement(possibleCategories[i], 10));
						break;
					}
				}

				_recipe = newRecipe;
			}
			else
			{
//				UnityEngine.Debug.Log("Recipe has ingredient requirements");
				{
					_recipe = recipe;
				}
			}
		}

		//This was a test case to prove that if production actually updates the parameters correctly, the difficulty is also updated
		Recipe GetRecipeFromTestJSON(IRecipe recipe)
		{
			Recipe newRecipe = new Recipe(recipe.Name);
			newRecipe.Id = recipe.Id;
			newRecipe.SetStage((int)recipe.CurrentStage);
			newRecipe.Hint = "TESTETSTESTETETSTTEE";
			newRecipe.IsAccessible = true;
			newRecipe.SetProducts(recipe.Products);

			var contributions = recipe.IngredientRequirements;
			var newList = new List<IngredientRequirement>();
			for(int i = 0; i < contributions.Count; ++i)
			{
				var current = contributions[i];
				var quality = (i == 0) ? 100 : 30;
				var req = new IngredientRequirement(current.Category,quality);
				newList.Add(req);
			}

			newRecipe.SetIngredientRequirements(newList);
			return newRecipe;
		}

		private void BuildIngredientList()
		{
			_ingredients = new List<KeyValuePair<IngredientCategory, List<KeyValuePair<Ingredient, int>>>>();

			for(int i = 0; i < NUM_CATEGORIES; ++i)
			{
				if(i < _recipe.IngredientRequirements.Count)
				{
					IngredientCategory currentCat = _recipe.IngredientRequirements[i].Category;
					var currentList = new List<KeyValuePair<Ingredient, int>>();

					var availableIngredients = _ingredientsByCategory[currentCat.Name];

					for(int j = 0; j < availableIngredients.Count; ++j)
					{
						Ingredient currentItem = availableIngredients[j];

						if(currentItem != null)
						{
							Item item = currentItem as Item;
                            var ingredientCount = _inventory.GetCount(item);

							KeyValuePair<Ingredient,int> currentPair = new KeyValuePair<Ingredient,int>(currentItem,ingredientCount);
							currentList.Add(currentPair);
						}
					}
					
					KeyValuePair<IngredientCategory,List<KeyValuePair<Ingredient,int>>> currentCatList = new KeyValuePair<IngredientCategory, List<KeyValuePair<Ingredient, int>>>(currentCat,currentList);
					_ingredients.Add(currentCatList);
				}
			}
		}

		void UpdateIngredientEntries()
		{
			foreach(var categories in _ingredients)
			{
				var currentList = categories.Value;
				for(int i = 0; i < currentList.Count; ++i)
				{
					var currentPair = currentList[i];
					Ingredient item = currentPair.Key;
                    var currentCount = _inventory.GetCount(item);
					if(currentCount != currentPair.Value)
					{
						var newPair = new KeyValuePair<Ingredient, int>(item,currentCount);
						var index = i;

						currentList.RemoveAt(index);
						currentList.Insert(index,newPair);
					}
				}
			}
		}

		private void CreateRecipe()
		{
			Recipe recipe = new Recipe("Test Recipe");
			recipe.Hint = "Do Crazy Stuff!";
			_shelfQualities = new Dictionary<int, List<int>>();

			for (int i = 0; i < NUM_CATEGORIES; ++i)
			{
				List<int> possibleQualities = new List<int>();
				
				IngredientCategory category = new IngredientCategory(i.ToString(), "Category " + i);
				switch(i)
				{
					case 0:
						recipe.IngredientRequirements.Add(new IngredientRequirement(category, 100));
						possibleQualities = new List<int>{100,70,30,10,10,50,5};
						break;
					case 1:
						recipe.IngredientRequirements.Add(new IngredientRequirement(category, 50));
						possibleQualities = new List<int>{50,100,100,10,10,30,5};
						break;
					case 2:
						recipe.IngredientRequirements.Add(new IngredientRequirement(category, 10));
						possibleQualities = new List<int>{10,10,100,10,0,50,5};
						break;
				}
				_shelfQualities[i] = possibleQualities;
			}

			_recipe = recipe;
		}

		private void CreateIngredients()
		{
			_ingredients = new List<KeyValuePair<IngredientCategory, List<KeyValuePair<Ingredient, int>>>>();
			for (int i = 0; i < NUM_CATEGORIES; ++i)
			{
				IngredientCategory category = _recipe.IngredientRequirements[i].Category;
				var categoryList = new List<KeyValuePair<Ingredient, int>>();
				var intList = _shelfQualities[i];

				for (int j = 0; j < 7; ++j)
				{
					int id = i * 7 + j;

					int value = intList[j];
					int count = UnityEngine.Random.Range(1,5);

					var ingredient = new Ingredient(id.ToString(), id.ToString(), category, value, false);
					ingredient.Display_Order = j;
					categoryList.Add(new KeyValuePair<Ingredient, int>(ingredient, count));
				}

				_ingredients.Add(new KeyValuePair<IngredientCategory, List<KeyValuePair<Ingredient, int>>>(category, categoryList));
			}
		}

        public override void Dispose()
        {
            // TODO: Not calling the base because the base is calling GetScreen, which will force us to generate another screen then delete and then set to null.
            // We need to find out if we can safely turn all GetScreen to return the screen and not make one if one doesn't exist.
            if (_screen != null)
            {
                _player.StaminaUpdate -= HandleStaminaUpdate;
                _player.FocusUpdate -= HandleFocusUpdate;
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
				_screen = _factory.GetScreen<iGUISmartPrefab_IngredientsSelectScreenNew>();
				_screen.Init(this, _player, _masterConfig);
				return _screen;
			}
		}

		private void InitializeView()
		{
			_screen = _factory.GetScreen<iGUISmartPrefab_IngredientsSelectScreenNew>();
			_screen.Init(this, _player, _masterConfig);
		}

		public void SetEnabled(bool value)
		{
			_screen.SetEnabled(value);
		}

		public void Unload()
		{
			Manager.CloseCurrentScreen();
		}

		public IRecipe GetRecipe()
		{
			return _recipe;
		}

		public IngredientCategory GetIngredientCategory(int ingredientIndex)
		{
			return _ingredients[ingredientIndex].Key;
		}

		public List<KeyValuePair<Ingredient, int>> GetIngredientEntries(int ingredientIndex)
		{
			return _ingredients[ingredientIndex].Value;
		}

		public bool HasSufficientFocus()
		{
			return (_player.Focus > 0);
		}

		public void RestoreFocus()
		{
			Action<bool> responseHandler = delegate(bool obj) { HandleFocusRestore(obj); };
			ShopController.BuyFocus(responseHandler);
		}

		void HandleFocusRestore(bool isSuccessful)
		{
			if(isSuccessful)
			{
				var dialog = GetSystemDialog("Thank you for your business!");
				dialog.Display(HandleSystemDialogResponse);
				_player.RestoreFocus(2);
				_player.UpdatePremiumCurrency(-1);
				_screen.UpdateInterfaceBar();
			}
			else
			{
				var dialog = GetSystemDialog("Sorry, there was an error in your purchase");
				dialog.Display(HandleSystemDialogResponse);
			}
		}


		public IDialog GetConfirmationDialog()
		{
			List<Ingredient> selectedIngredients = _screen.GetSelectedIngredients;
			List<int> ingredientCounts = new List<int>();
			var requirementCount = RequirementCount;
			float rawDifficulty = 0.0f;
			switch(requirementCount)
			{
				case 1:
					rawDifficulty = _recipe.GetDifficulty(selectedIngredients[0]);
					ingredientCounts.Add(_inventory.GetCount(selectedIngredients[0]));
					break;
				case 2:
					rawDifficulty = _recipe.GetDifficulty(selectedIngredients[0], selectedIngredients[1]);
					ingredientCounts.Add(_inventory.GetCount(selectedIngredients[0]));
					ingredientCounts.Add(_inventory.GetCount(selectedIngredients[1]));
					break;
				case 3:
					rawDifficulty = _recipe.GetDifficulty(selectedIngredients[0], selectedIngredients[1], selectedIngredients[2]);
					ingredientCounts.Add(_inventory.GetCount(selectedIngredients[0]));
					ingredientCounts.Add(_inventory.GetCount(selectedIngredients[1]));
					ingredientCounts.Add(_inventory.GetCount(selectedIngredients[2]));
					break;
			}

			var confirmDialog = _factory.GetDialog<iGUISmartPrefab_IngredientsConfirmDialog>();
			confirmDialog.Prepare(selectedIngredients,ingredientCounts,rawDifficulty);
			return confirmDialog;
		}

		public void ShowCurrencyPurchaseDialog()
		{
			_shopDialogController.Show (ShopDisplayType.STARSTONES, OnFinishTransaction);
		}

		public void OnFinishTransaction()
		{
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
			dialog.SetText (message);
			return dialog;
		}
		
		void HandleSystemDialogResponse(int answer)
		{
			UnityEngine.Debug.Log("Close system dialog");
		}

		public IDialog GetIngredientPurchaseFinishDialog(Ingredient ingredient)
		{
			var dialog = _factory.GetDialog<iGUISmartPrefab_IngredientsPurchaseFinishDialog>();
			dialog.SetIngredient(ingredient);
			return dialog;
		}

		public IDialog GetIngredientPurchaseDialog(Ingredient ingredient)
		{
			SendIngredientConfirmMetric (ingredient);

			var dialog = _factory.GetDialog<iGUISmartPrefab_IngredientsBuyDialog>();
			dialog.SetIngredient(ingredient);
			dialog.SetPlayer(_player);
			return dialog;
		}

		public void InitiateIngredientPurchase(Ingredient ingredient, bool isPremium)
		{
			_queuedIngredient = ingredient;
			_completingPurchase = GetPostPurchaseLoadingDialog();
			_completingPurchase.Display(null);
			(_completingPurchase as iGUISmartPrefab_ConfirmCompleteLoadDialog).BeginLoading();
//			Action<bool> responseHandler = delegate(bool obj) { CompleteIngredientTransaction(obj); };
			Action<bool> responseHandler = (result => CompleteIngredientTransaction(result, ingredient));

			if(isPremium)
			{
				_purchaseType = IngredientPurchaseType.PREMIUM;
//				Action<bool> responseHandler = (result => CompleteIngredientTransaction(result, ingredient, _purchaseType));
				ShopController.SpendPremium(_queuedIngredient.Id, responseHandler);

			}
			else
			{
				_purchaseType = IngredientPurchaseType.FREE;
//				Action<bool> responseHandler = (result => CompleteIngredientTransaction(result, ingredient, _purchaseType));
				ShopController.SpendCoins(_queuedIngredient.Id,responseHandler);
			}
		}

		void CompleteIngredientTransaction(bool isSuccess, Ingredient ingredient)//, IngredientPurchaseType purchaseType)
		{
			(_completingPurchase as iGUISmartPrefab_ConfirmCompleteLoadDialog).EndLoading();
			_completingPurchase = null;
			if(isSuccess)
			{
				_screen.GetButtonHandlerInterface ().Deactivate ();
				var dialog = GetIngredientPurchaseFinishDialog(_queuedIngredient);
				_completingPurchase = dialog;
				Action<int> handleTransaction = (i) => 
				{
					HandleEndIngredientTransaction(i);
					_screen.GetButtonHandlerInterface().Activate();
				};
				dialog.Display(handleTransaction);

				SendIngredientBoughtMetric(ingredient, _purchaseType);
			}
			else
			{
				UnityEngine.Debug.LogWarning("There was a major fail in your purchase");
				var dialog = GetSystemDialog("Sorry, there was an error in your purchase");
				dialog.Display(HandleSystemDialogResponse);
			}
		}

		void HandleEndIngredientTransaction(int answer)
		{
			AddIngredientToInventory(_queuedIngredient);
			_purchaseType = IngredientPurchaseType.NONE;
			_completingPurchase = null;
			_queuedIngredient = null;
		}

		public IDialog GetInsufficientFocusDialog()
		{
			var insufficientFocusDialog = _factory.GetDialog<iGUISmartPrefab_InsufficientFocusPopupDialog>();
			insufficientFocusDialog.SetPremiumAvailable(_player.CurrencyPremium);
			return insufficientFocusDialog;
		}

		public void AddIngredientToInventory(Ingredient ingredient)
		{
            _inventory.Add(ingredient, 1);
			if(_purchaseType == IngredientPurchaseType.PREMIUM)
			{
                _player.UpdatePremiumCurrency(-ingredient.PremiumPrice);
			}
			else
			{
                _player.UpdateCurrency(-ingredient.RegularPrice);
			}
			_screen.UpdateInterfaceBar();
			UpdateIngredientEntries();
			_screen.UpdateIngredientsInShelves();
		}

		public void LoadNextScreen()
		{
			AmbientDebugTimer.Current.Start("IngredientsSelectScreenController >>> LoadNextScreen");

			List<Ingredient> selectedIngredients = _screen.GetSelectedIngredients;
			var requirementCount = RequirementCount;
			float rawDifficulty = 0.0f;
			switch(requirementCount)
			{
			case 1:
				rawDifficulty = _recipe.GetDifficulty(selectedIngredients[0]);
				break;
			case 2:
				rawDifficulty = _recipe.GetDifficulty(selectedIngredients[0], selectedIngredients[1]);
				break;
			case 3:
				rawDifficulty = _recipe.GetDifficulty(selectedIngredients[0], selectedIngredients[1], selectedIngredients[2]);
				break;
			}

			AmbientDebugTimer.Current.Start("IngredientsSelectScreenController >>> Deduct Focus");

			_player.DeductFocus();
			MakeDeductFocusCall();

			AmbientDebugTimer.Current.Start("IngredientsSelectScreenController >>> Show LoadingScreenController");
			IScreenController nextScreen = new LoadingScreenController(Manager, _factory, _player, rawDifficulty, _recipe, selectedIngredients, _repo, _masterConfig);

			Manager.Add(nextScreen);

			AmbientDebugTimer.Current.Stop();

			SendSelectedIngredientMetric (rawDifficulty);
		}

		void MakeDeductFocusCall()
		{
			var network = ShopController.NetworkController;

			Dictionary<string,string> parameters = new Dictionary<string, string> ()
			{
				{"phone_id",_player.UserID}
			};

			network.Send (URLs.USE_FOCUS, parameters, HandleFocusSuccess, HandleFocusFail);
		}

		void HandleFocusSuccess (Voltage.Common.Net.WWWNetworkPayload payload)
		{
			string message = String.Format ("SUCCESS  URL::{0},RESPONSE::{1}", payload.WWW.url, payload.WWW.text);
			UnityEngine.Debug.Log(message);
		}

		void HandleFocusFail (Voltage.Common.Net.WWWNetworkPayload payload)
		{
			string error = String.Format ("FAILED  URL::{0},RESPONSE::{1}", payload.WWW.url, payload.WWW.text);
			UnityEngine.Debug.LogWarning(error);
		}

		public void GoBackToBookshelf()
		{
			IScreenController nextScreen = _repo.Get<BookshelfScreenController>();
			if(!Manager.GetCurrentPath().Contains("/Home/BookshelfScreenNew"))
			{
				Manager.Add(nextScreen);
			}
			else
			{
				Manager.OpenScreenAtPath(nextScreen,"/Home");
			}
		}

		public void GoHome()
		{
			if(!Manager.GetCurrentPath().Contains("/Home"))
			{
				IScreenController nextScreen = _repo.Get<HomeScreenController>();
				Manager.Add(nextScreen);
			}
			else
			{
				Manager.GoToExistingScreen("/Home");
			}
		}

		private void SendSelectedIngredientMetric(float difficulty)	// TODO: eventually move out to its own class
		{
			IDictionary<string,object> data = new Dictionary<string,object> 
			{
				{"recipe_id", _recipe.Name},
//				{"ingredients_id", },
				{"difficulty_id", difficulty }
			};
			
			AmbientMetricManager.Current.LogEvent (MetricEvent.MINIGAMEPAGE_SELECTED_INGREDIENT, data);
		}

		private void SendIngredientConfirmMetric(Ingredient ingredient)	// TODO: eventually move out to its own class
		{
			IDictionary<string,object> data = new Dictionary<string,object> 
			{
				{"recipe_id", _recipe.Name},
				{"ingredient_id", ingredient.Name},
				{"ingredients_starstones_cost", ingredient.PremiumPrice },
				{"ingredients_coins_cost", ingredient.RegularPrice }
			};
			
			AmbientMetricManager.Current.LogEvent (MetricEvent.MINIGAMEPAGE_DISPLAY_DIALOGUE_BUY_INGREDIENT, data);
		}


		private void SendIngredientBoughtMetric(Ingredient ingredient, IngredientPurchaseType purchaseType)	// TODO: eventually move out to its own class
		{
			int starstonesPaid = 0;
			int coinsPaid = 0;

			if(purchaseType == IngredientPurchaseType.PREMIUM)
			{
				starstonesPaid = ingredient.PremiumPrice;
			}
			else
			{
				coinsPaid = ingredient.RegularPrice;
			}

			IDictionary<string,object> data = new Dictionary<string,object> 
			{
				{"recipe_id", _recipe.Name},
				{"ingredients_id", ingredient.Name},
				{"starstones_paid", starstonesPaid },
				{"coins_paid", coinsPaid },
			};
			
			AmbientMetricManager.Current.LogEvent (MetricEvent.MINIGAMEPAGE_BOUGHT_INGREDIENT, data);
		}

	}
}
