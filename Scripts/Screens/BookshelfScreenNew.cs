using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using iGUI;
using Voltage.Witches.Events;
using Voltage.Witches.Controllers;
using Voltage.Witches.Models;
using Voltage.Witches.Views;
using Voltage.Witches.User;

namespace Voltage.Witches.Screens
{
	using Debug = UnityEngine.Debug;

	public class BookshelfScreenNew : BaseScreen
	{
		Player _player;
		
		[HideInInspector]
		public Placeholder bookshelfViewPlaceholder;
		
		[HideInInspector]
		public Placeholder interfaceBarPlaceholder;
		
		[HideInInspector]
		public Placeholder spellbookViewPlaceholder;
		
		[HideInInspector]
		public Placeholder menuViewPlaceholder;
		
		private iGUISmartPrefab_BookshelfViewNew _bookshelfView;
		private iGUISmartPrefab_SpellbookViewNew _bookView;
		private iGUIElement _bookElement;

		iGUISmartPrefab_InterfaceShell _interface;

		private enum PressableButtons
		{
			HOME = 0,
			STARSTONE = 1,
			OPEN_RIBBON = 2,
			CLOSE_RIBBON = 3,
			RECIPE_1 = 4,
			RECIPE_2 = 5,
			RECIPE_3 = 6,
			RECIPE_4 = 7,
			RECIPE_5 = 8,
			BOOK_1 = 9,
			BOOK_2 = 10,
			BOOK_3 = 11,
			BOOK_4 = 12,
			BOOK_5 = 13
		}

		List<iGUIButton> _pressableButtons;
		BookshelfScreenController _controller;
		IGUIHandler _buttonHandler;
		Dictionary<iGUIButton,iGUIElement> _buttonArtMap;

		[HideInInspector]
		public iGUIContainer gold;

		
		public IButtonHandler GetButtonHandlerInterface()
		{
			return _buttonHandler;
		}

		
		
		public void Init(Player player, BookshelfScreenController controller)
		{			
			_player = player;
			_controller = controller;

			LoadPlaceholders();
			
//			_bookView.SetPlayer(_player);
			SetCurrentBook(GetHighestAccessibleBookIndex());
		}

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
			_interface.SetLayout(InterfaceLayout.Standard);
			if(_player != null)
			{
				_interface.SetCounts(_player);
				_interface.BeginCountDown(CountDownType.FOCUS, _player.FocusNextUpdate);
				_interface.BeginCountDown(CountDownType.STAMINA, _player.StaminaNextUpdate);
			}
			StartCoroutine(WaitToRefresh());
		}

		iGUIElement LoadPlaceholder()
		{
			var element = interfaceBarPlaceholder.SwapForSmartObject() as iGUIContainer;
			element.setLayer(15);
			return element;
		}

		void HandleButtonsReady()
		{
			SetButtonCallbacks();
			SetButtonArtMap();
		}

		void SetButtonCallbacks()
		{
			_pressableButtons = new List<iGUIButton>()
			{
				_interface.btn_home,_interface.rib_add_starstones_hitbox,_interface.arrow_down_hitbox,_interface.arrow_up_hitbox,
				_bookView.Recipe_Buttons[0],_bookView.Recipe_Buttons[1],_bookView.Recipe_Buttons[2],_bookView.Recipe_Buttons[3],
				_bookView.Recipe_Buttons[4],_bookshelfView.Book_Buttons[0],_bookshelfView.Book_Buttons[1],_bookshelfView.Book_Buttons[2],
				_bookshelfView.Book_Buttons[3],_bookshelfView.Book_Buttons[4]
			};

			for(int i = 0; i < _pressableButtons.Count; ++i)
			{
				var currentButton = _pressableButtons[i];
				currentButton.clickDownCallback += ClickInit;
			}
		}

		void SetButtonArtMap()
		{
			_buttonArtMap = new Dictionary<iGUIButton, iGUIElement>()
			{
				{_interface.btn_home,_interface.home},
				{_interface.rib_add_starstones_hitbox,_interface.rib_add_starstones_hitbox.getTargetContainer()},
				{_interface.arrow_down_hitbox,_interface.arrow_down},
				{_interface.arrow_up_hitbox,_interface.arrow_up},
				{_bookView.Recipe_Buttons[0],_bookView.Recipe_Buttons[0].getTargetContainer()},
				{_bookView.Recipe_Buttons[1],_bookView.Recipe_Buttons[1].getTargetContainer()},
				{_bookView.Recipe_Buttons[2],_bookView.Recipe_Buttons[2].getTargetContainer()},
				{_bookView.Recipe_Buttons[3],_bookView.Recipe_Buttons[3].getTargetContainer()},
				{_bookView.Recipe_Buttons[4],_bookView.Recipe_Buttons[4].getTargetContainer()},
				{_bookshelfView.Book_Buttons[0],_bookshelfView.Book_Buttons[0].getTargetContainer()},
				{_bookshelfView.Book_Buttons[1],_bookshelfView.Book_Buttons[1].getTargetContainer()},
				{_bookshelfView.Book_Buttons[2],_bookshelfView.Book_Buttons[2].getTargetContainer()},
				{_bookshelfView.Book_Buttons[3],_bookshelfView.Book_Buttons[3].getTargetContainer()},
				{_bookshelfView.Book_Buttons[4],_bookshelfView.Book_Buttons[4].getTargetContainer()}
			};
		}

		//HACK There was a problem returning from MiniGame that made the text disappear
		IEnumerator WaitToRefresh()
		{
			iGUIRoot.instance.setEnabled(false);
			yield return new WaitForEndOfFrame();
			iGUIRoot.instance.setEnabled(true);
		}

		private int GetHighestAccessibleBookIndex()
		{
			int highestBookIndex = -1;
			
			var books = _player.GetBooks();
			for (int i = 0; i < books.Count; ++i)
			{
				if (books[i].IsAccessible && i > highestBookIndex)
				{
					highestBookIndex = i;
				}
			}
			
			return highestBookIndex;
		}
		
		protected override IScreenController GetController()
		{
			return _controller;
		}
		
		void DisplayDialog()
		{
			_buttonHandler.Deactivate();
			IDialog dialog = _controller.GetDetailsDialog();
			dialog.Display(OnDialogResponse);
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
		
		void OnDialogResponse(int answer)
		{
//			Debug.Log("Response was: " + answer);
			if ((DialogResponse)answer == DialogResponse.OK)
			{
				_controller.MoveToIngredientsScreen();
			}
			_buttonHandler.Activate();
		}
		
		void SetCurrentBook(int index)
		{
			_bookView.SetBook(_player.GetBooks()[index]);
			_bookshelfView.ChangeBookSelection(index);
		}
		
		void LoadPlaceholders()
		{
			LoadBookshelfView();
			LoadSpellbookView();
		}
		
		void LoadBookshelfView()
		{
			iGUIElement element = bookshelfViewPlaceholder.SwapForSmartObject();
			_bookshelfView = element.GetComponent<iGUISmartPrefab_BookshelfViewNew>();
			_bookshelfView.SetBooks(_player.GetBooks());
			_bookshelfView.OnComplete += HandleButtonsReady;
			_bookshelfView.OnBookClick += HandleOnBookClick;
		}
		
		void LoadSpellbookView()
		{
			_bookElement = spellbookViewPlaceholder.SwapForSmartObject();
			_bookView = _bookElement.GetComponent<iGUISmartPrefab_SpellbookViewNew>();
			_bookView.SetVariableMapper(_controller.VariableMapper);
			_bookView.Init();
			_bookView.ListenForRecipeSelections(HandleOnRecipeSelect);
		}

		public void UpdateInterfaceElements()
		{
			if(_interface != null)
			{
				_interface.SetCounts(_player);
			}
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
			var buttonType = (PressableButtons)_pressableButtons.IndexOf(button);

			if(isOverButton)
			{
				if(buttonType == PressableButtons.BOOK_1)
				{
					_bookshelfView.ExecuteButtonClick(0);
				}
				else if(buttonType == PressableButtons.BOOK_2)
				{
					_bookshelfView.ExecuteButtonClick(1);
				}
				else if(buttonType == PressableButtons.BOOK_3)
				{
					_bookshelfView.ExecuteButtonClick(2);
				}
				else if(buttonType == PressableButtons.BOOK_4)
				{
					_bookshelfView.ExecuteButtonClick(3);
				}
				else if(buttonType == PressableButtons.BOOK_5)
				{
					_bookshelfView.ExecuteButtonClick(4);
				}
				else if(buttonType == PressableButtons.RECIPE_1)
				{
					_bookView.ExecuteRecipeClick(0);
				}
				else if(buttonType == PressableButtons.RECIPE_2)
				{
					_bookView.ExecuteRecipeClick(1);
				}
				else if(buttonType == PressableButtons.RECIPE_3)
				{
					_bookView.ExecuteRecipeClick(2);
				}
				else if(buttonType == PressableButtons.RECIPE_4)
				{
					_bookView.ExecuteRecipeClick(3);
				}
				else if(buttonType == PressableButtons.RECIPE_5)
				{
					_bookView.ExecuteRecipeClick(4);
				}
				else if(buttonType == PressableButtons.CLOSE_RIBBON)
				{
					StartCoroutine(_interface.UpdateRibbonPosition(Voltage.Witches.Views.InterfaceShellView.RibbonState.CLOSED));
				}
				else if(buttonType == PressableButtons.HOME)
				{
					_controller.GoHome();
				}
				else if(buttonType == PressableButtons.OPEN_RIBBON)
				{
					StartCoroutine(_interface.UpdateRibbonPosition (Voltage.Witches.Views.InterfaceShellView.RibbonState.OPEN));
				}
				else if(buttonType == PressableButtons.STARSTONE)
				{
					_buttonHandler.Deactivate();
					_controller.ShowCurrencyPurchaseDialog();
				}
			}

			_buttonArtMap[button].colorTo(Color.white,0.3f);
		}
			
			
		void HandleCloseCurrencyDialog (int answer)
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
		
		void HandleOnBookClick(object sender, EventArgs e)
		{
			BookChangedEventArgs bookEvent = e as BookChangedEventArgs;
			if (bookEvent.Index >= _player.GetBooks().Count)
			{
//				var booksData = DataManager.GetData().MasterConfig
				IDialog dialog = _controller.GetBookLockedDialog(bookEvent.Index);
				dialog.Display(HandleBookLocked);
				_buttonHandler.Deactivate();
			}
			else
			{
				_bookView.SetBook(_player.GetBooks()[bookEvent.Index]);
				// Move book off screen
				// Switch book contents
				// Move book back on screen
				//		AnimationClass testAnim = new AnimationClass(this, _bookElement);
				//		StartCoroutine(testAnim.Execute());
			}
		}
		
		void HandleBookLocked(int response)
		{
			switch ((BookLockedChoice)response)
			{
			case BookLockedChoice.StoryMap:
			{
				_controller.GoToStoryMap();
				break;
			}
			}
			_buttonHandler.Activate();
		}
		
		void HandleOnRecipeSelect(object sender, EventArgs e)
		{
			RecipeSelectedEventArgs recipeEvent = e as RecipeSelectedEventArgs;
			_controller.HandleRecipeClick(recipeEvent.Recipe);
			DisplayDialog();
			//		Debug.Log("Recipe selected: " + recipeEvent.Recipe.Name);
		}
		
		public void SetEnabled(bool value)
		{
			screenFrame.setEnabled(value);
			gameObject.SetActive(value);
		}
		
		class AnimationClass
		{
			MonoBehaviour _parent;
			iGUIElement _element;
			Rect _originalBounds;
			
			public AnimationClass(MonoBehaviour parent, iGUIElement element)
			{
				_parent = parent;
				_element = element;
				_originalBounds = element.getAbsoluteRect();
			}
			
			public IEnumerator Execute()
			{
				yield return _parent.StartCoroutine(MoveVertically(true));
				yield return _parent.StartCoroutine(MoveVertically(false));
			}
			
			IEnumerator MoveVertically(bool upwards)
			{
				float end = upwards ? - _originalBounds.height : _originalBounds.y;
				
				_element.positionTo(new Vector2(_originalBounds.x, end), 1.0f);
				
				while (_element.getAbsoluteRect().y != end)
				{
					yield return null;
				}
			}
		}

	}
}