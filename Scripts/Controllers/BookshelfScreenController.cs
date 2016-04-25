using Ninject;
using Ninject.Modules;
using System;
using System.Collections.Generic;

using Voltage.Witches.DI;
using Voltage.Witches.Configuration;
using Voltage.Witches.Screens;
using Voltage.Witches;
using Voltage.Witches.Models;
using Voltage.Witches.Events;

namespace Voltage.Witches.Controllers
{
	using Voltage.Witches.Shop;
    using Voltage.Witches.Controllers.Factories;
	using Voltage.Story.Variables;

	public class BookshelfScreenController : ScreenController
	{
		private IScreenFactory _factory;
		private Player _player;
        private IControllerRepo _repo;
		private IRecipe _selectedRecipe;
		private BooksConfiguration _booksMaster;
		private SpellbookFactoryNew _bookFactory;

		private iGUISmartPrefab_BookshelfScreenNew _screen;

		private ShopItemData _iapItem;
		private IDialog _completingPurchase;

		public ShopController ShopController { get; protected set; } 
		public VariableMapper VariableMapper { get; protected set; }

		private readonly ShopDialogueController _shopDialogController;

        public BookshelfScreenController(ScreenNavigationManager controller, IScreenFactory factory, Player player, IControllerRepo repo, 
			MasterConfiguration masterConfig,ShopDialogueController shopDialogController): base(controller)
		{
			_factory = factory;
			_player = player;
            _repo = repo;

			_player.StaminaUpdate += HandleStaminaUpdate;
			_player.FocusUpdate += HandleFocusUpdate;

			ShopController = _repo.Get<ShopController>();

			_booksMaster = masterConfig.Books_Configuration;
			_bookFactory = new SpellbookFactoryNew(masterConfig, new RecipeFactoryNew(masterConfig));

			VariableMapper = _repo.Get<VariableMapper>();
			_shopDialogController = shopDialogController;
			InitializeView();
		}

		void HandleStaminaUpdate(object sender, EventArgs e)
		{
			_screen.UpdateInterfaceElements();
		}
		
		void HandleFocusUpdate(object sender, EventArgs e)
		{
			_screen.UpdateInterfaceElements();
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
			_booksMaster = null;
			_bookFactory = null;
        }

        protected override IScreen GetScreen()
		{
			if(_screen != null)
			{
				return _screen;
			}
			else
			{
				_screen = _factory.GetScreen<iGUISmartPrefab_BookshelfScreenNew>();
				_screen.Init(_player, this);
				return _screen;
			}
		}

		void InitializeView()
		{
			_screen = _factory.GetScreen<iGUISmartPrefab_BookshelfScreenNew>();
			_screen.Init(_player, this);
		}



		public override void MakePassive (bool value)
		{
			_screen.MakePassive(value);
		}

		public void HandleRecipeClick(IRecipe recipe)
		{
//			UnityEngine.Debug.Log("Recipe selected was: " + recipe.Name);
			_selectedRecipe = recipe;
		}

		public IDialog GetDetailsDialog()
		{
			var dialog = _factory.GetDialog<iGUISmartPrefab_DetailedRecipeDialog>();
			dialog.SetRecipe(_selectedRecipe);
			return dialog;
		}

		public void MoveToIngredientsScreen()
		{
			SendSelectedRecipeMetric ();

            IIngredientsSelectScreenControllerFactory factory = _repo.Get<IIngredientsSelectScreenControllerFactory>();
            IScreenController nextScreen = factory.Create(_selectedRecipe);

			Manager.Add(nextScreen);
		}

		public void ShowCurrencyPurchaseDialog()
		{
			_shopDialogController.Show (ShopDisplayType.STARSTONES, OnFinishTransaction);
		}

		public void OnFinishTransaction()
		{
			_screen.UpdateInterfaceElements();
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

		public ScreenController DisplayLoadingMarquee()
		{
			MainLoadingScreenControllerFactory factory = _repo.Get<MainLoadingScreenControllerFactory>();
			MainLoadingScreenController loading = factory.Create((int)MainLayoutType.LOADING_MARQUEE, null, null);
			loading.Show();
			return loading;
		}

		public void GoToStoryMap()
		{
			var loading = DisplayLoadingMarquee();
			Action<int> callback = (loading as MainLoadingScreenController).ProcessAction;
			IStoryMapScreenControllerFactory storyMapScreenFactory = _repo.Get<IStoryMapScreenControllerFactory>();
            StoryMapUGUIScreenController nextScreen = storyMapScreenFactory.CreateUGUI(_player,callback);
			Manager.Add(nextScreen);
		}

//		private Spellbook _currentBook;
		private Spellbook GetBookFromIndex(int index)
		{
			++index;
			SpellbookRefConfig config = _booksMaster.Books_Index[index];
			Spellbook book = _bookFactory.Create(config);
			return book;

//			_currentBook = _bookFactory.Create(config);
//			return _currentBook;
		}

		public IDialog GetBookLockedDialog(int index)
		{
			var dialog = _factory.GetDialog<iGUISmartPrefab_BookLockedDialog>();
			var book = GetBookFromIndex(index);
			UnityEngine.Debug.LogWarning("Has clear item count :: " + book.ClearItems.Count.ToString());
			dialog.SetBook(book);
			return dialog; 
		}

		public void GoToPrototype()
		{
            IScreenController nextScreen = _repo.Get<TracingScreenController>();
			(nextScreen as TracingScreenController).SetQueuedObject(null);
			Manager.Add(nextScreen);
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

		public void SetEnabled(bool value)
		{
			_screen.SetEnabled(value);
		}

		public void Unload()
		{
			_screen.SetEnabled(false);
		}
	

		private void SendSelectedRecipeMetric()	// TODO: eventually move out to its own class
		{
//			if(_currentBook == null)
//			{
//				SpellbookRefConfig config = _booksMaster.Books_Index[1];
//				_currentBook = _bookFactory.Create(config);
//			}

			IDictionary<string,object> data = new Dictionary<string,object> 
			{
//				{"book_id", _currentBook.Name},
				{"recipe_id", _selectedRecipe.Name}
			};
			
			Voltage.Common.Metrics.AmbientMetricManager.Current.LogEvent (Voltage.Witches.Metrics.MetricEvent.MINIGAMEPAGE_SELECTED_RECIPE, data);
		}


	}
}

