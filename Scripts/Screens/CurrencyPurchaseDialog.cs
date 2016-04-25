using iGUI;
using UnityEngine;
//using UnityEngine.Advertisements;
using System.Collections.Generic;
using System.Collections;
using Voltage.Witches.Models;
using Voltage.Witches.Events;
using Voltage.Witches.Views;

namespace Voltage.Witches.Screens
{
	using Debug = UnityEngine.Debug;
	using Voltage.Witches.Configuration;

	public class CurrencyPurchaseDialog : AbstractDialog
	{
		[HideInInspector]
		public iGUIContainer starstone_bundles,stamina_bundles,tarot_cards_grp,starterpack_starstone_bundles, starterpack_stamina_bundles;
		
		[HideInInspector]
		public iGUIButton tarot_card_btn,tab_stamina_hitbox,tab_starstone_hitbox,btn_popup_close, btn_buy;

		[HideInInspector]
		public iGUIImage main_header_starstone_shop,main_header_stamina_potion_shop,tab_stamina,tab_starstone,starstones_text,stamina_potions_text;

		[HideInInspector]
		public iGUIScrollView scrollView_starstone, scrollView_stamina;

		private const int TAB_TOP = 9;
		private const int TAB_BOTTOM = 8;
		private const string STARTER_PACK = "Starter Pack";

		public GUIEventHandler OnPurchaseRequest;


		enum ShopState
		{
			STARSTONE = 0,
			STAMINA = 1
		}

		private ShopState _currentShop = ShopState.STARSTONE;

		List<ShopItemData> _shopItemsIndex;
		List<iGUIElement> _shopElements;
		Dictionary<int,iGUISmartPrefab_ShopStaminaBundle> _staminaBundleViews;
		Dictionary<int,iGUISmartPrefab_ShopStarstoneBundle> _starstoneBundleViews;
		List<iGUIButton> _itemButtons;

		Dictionary<iGUIButton,iGUIElement> _buttonArtMap;
		IGUIHandler _buttonHandler;

		private const int STARTER_INDEX_IN_STONE_SHOP = 6;
		private const int STARTER_INDEX_IN_STAMINA_SHOP = 11;

		private bool _starterPackShown = false;

		protected virtual void Awake()
		{
			_buttonHandler = gameObject.AddComponent<IGUIHandler>();
			_buttonHandler.MovedAway += HandleMovedAway;
			_buttonHandler.MovedBack += HandleMovedBack;
			_buttonHandler.ReleasedButtonEvent += HandleReleasedButtonEvent;
			_buttonHandler.InputCancelledEvent += HandleReleasedButtonEvent;
		}

			
		public void SubscribeButtonHandlerForStarterPack()
		{
			_buttonHandler.ReleasedButtonEvent -= HandleReleasedButtonEvent;
			_buttonHandler.ReleasedButtonEvent += HandleReleasedButtonEventForStarterPack;
		}

		public void ActivateStarterPackShopView()
		{
			starstone_bundles.setEnabled(false);
			stamina_bundles.setEnabled(false);
			starterpack_starstone_bundles.setEnabled(true);
			starterpack_stamina_bundles.setEnabled(true);
		}

		public void ActivateRegularShopView()
		{
			starstone_bundles.setEnabled(true);
			stamina_bundles.setEnabled(true);
			starterpack_starstone_bundles.setEnabled(false);
			starterpack_stamina_bundles.setEnabled(false);
		}

		private Player _player;

		protected void Start()
		{
			if (_starterPackShown) 
			{
				LoadStarterPackBundles ();
				_player.OnPurchaseStarterPack += DisableStarterPackButton;
			} 
			else
			{
				LoadPlaceholders();
			}

			//TODO Conditional not to display in earlier version
			{
				tarot_cards_grp.setEnabled(false);
			}
			AssignButtonHandlers();
			SetUpButtonArtMap();
		}

		public void PrepShopDialog(bool starterPackShown, Player player)
		{
			_starterPackShown = starterPackShown;
			_player = player;
		}

		public void Init(ShopItemsConfiguration shopItemsConfig)
		{
			_shopItemsIndex = new List<ShopItemData>();
			if(shopItemsConfig != null)
			{
				var masterData = shopItemsConfig.Shop_Items_Indexed;
				for(int i = 0; i < masterData.Count; ++i)
				{
					var item = masterData[i];
					_shopItemsIndex.Add(item);
				}
			}
			else
			{
				throw new System.Exception("ShotItemsConfig cannot be null");
			}

		}

		public void InitStarterPack(ShopItemsConfiguration shopItemsConfig)
		{
			if (_shopElements == null) 
			{
				_shopElements = new List<iGUIElement>();
			}
			if (_itemButtons == null) 
			{
				_itemButtons = new List<iGUIButton>();
			}

			_shopItemsIndex = new List<ShopItemData>();
			if(shopItemsConfig != null)
			{
				var masterData = shopItemsConfig.Shop_Items_Indexed;
				for(int i = 0; i < masterData.Count; ++i)
				{
					var item = masterData[i];
					_shopItemsIndex.Add(item);
				}
			}
			else
			{
				throw new System.Exception("ShotItemsConfig cannot be null");
			}


		}

		public override void Dispose ()
		{
			_player.OnPurchaseStarterPack -= DisableStarterPackButton;	
			
			base.Dispose ();
		}




		void LoadPlaceholders()
		{
			_shopElements = new List<iGUIElement>();
			_itemButtons = new List<iGUIButton>();
			LoadStarstoneBundles();
			LoadStaminaBundles();
		}

		private ShopItemData GetShopItemDataForStarterPack()
		{
			ShopItemData shopItem = null;
			foreach(ShopItemData item in _shopItemsIndex)
			{
				if (item.name == STARTER_PACK) 
				{
					shopItem = item;
				}
			}
			return shopItem;
		}

		private void LoadStarterPackBundles()
		{
			_starstoneBundleViews = new Dictionary<int,iGUISmartPrefab_ShopStarstoneBundle>();

			var items = starterpack_starstone_bundles.items;
			for(int i = 0; i < items.Length; ++i)
			{
				ShopItemData shopItem = null;
				if((_shopItemsIndex != null) && (_shopItemsIndex.Count > i))
				{
					shopItem = _shopItemsIndex[i];
				}
				var current = items[i];
				var order = current.order;
				var layer = current.layer;

				var element = (current as Placeholder).SwapForSmartObject() as iGUIContainer;
				element.setLayer(layer + 1);
				element.setOrder(order + 1);
				_shopElements.Add(element);

				var view = element.GetComponent<iGUISmartPrefab_ShopStarstoneBundle>();
				if(shopItem != null)
				{
					if (i == STARTER_INDEX_IN_STONE_SHOP)
					{
						shopItem = GetShopItemDataForStarterPack();
						view.SetItem (shopItem, view.StarterPackIndex);
						view.AdjustStarterPackIcon ();
						element.setLayer(0);
						element.setOrder(0);
					}
					else 
					{
						view.SetItem (shopItem, i);
					}
				}
				view.OnItemSelect += HandlePurchase;
				_itemButtons.Add(view.btn_buy);
				_starstoneBundleViews[i] = view;
			}

			_staminaBundleViews = new Dictionary<int,iGUISmartPrefab_ShopStaminaBundle>();

			var staminaitems = starterpack_stamina_bundles.items;
			for(int i = 0; i < staminaitems.Length; ++i)
			{
				var index = _shopElements.Count; 
				int adjustedIndex = index -1; //Adjusted due to insertion of starter pack in starstone
				ShopItemData shopItem = null;
				if((_shopItemsIndex != null) && (_shopItemsIndex.Count > adjustedIndex))
				{
					shopItem = _shopItemsIndex[adjustedIndex];
				}
				var current = staminaitems[i];
				var order = current.order;
				var layer = current.layer;

				var element = (current as Placeholder).SwapForSmartObject() as iGUIContainer;
				element.setLayer(layer +1); // shifting one right to insert starter pack
				element.setOrder(order +1);
				_shopElements.Add(element);

				var view = element.GetComponent<iGUISmartPrefab_ShopStaminaBundle>();
				if(shopItem != null)
				{
					if (index == STARTER_INDEX_IN_STAMINA_SHOP ) {
						shopItem = GetShopItemDataForStarterPack();
						view.SetItem (shopItem, view.StarterPackIndex);
						view.AdjustStarterPackIcon ();
						element.setLayer (0);
						element.setOrder (0);
					} else {
						view.SetItem (shopItem, adjustedIndex);
					}
				}
				view.OnItemSelect += HandlePurchase;
				_itemButtons.Add(view.btn_buy);
				_staminaBundleViews[index] = view;
			}

			scrollView_starstone.setAreaWidth(2.36F);// item width * item number
			scrollView_stamina.setAreaWidth(1.7F);
		}
			
		void LoadStarstoneBundles()
		{
			_starstoneBundleViews = new Dictionary<int,iGUISmartPrefab_ShopStarstoneBundle>();

			var items = starstone_bundles.items;
			for(int i = 0; i < items.Length; ++i)
			{
				ShopItemData shopItem = null;
				if((_shopItemsIndex != null) && (_shopItemsIndex.Count > i))
				{
					shopItem = _shopItemsIndex[i];
				}
				var current = items[i];
				var order = current.order;
				var layer = current.layer;

				var element = (current as Placeholder).SwapForSmartObject() as iGUIContainer;
				element.setLayer(layer);
				element.setOrder(order);
				_shopElements.Add(element);

				var view = element.GetComponent<iGUISmartPrefab_ShopStarstoneBundle>();
				if(shopItem != null)
				{
					view.SetItem(shopItem,i);
				}
				view.OnItemSelect += HandlePurchase;
				_itemButtons.Add(view.btn_buy);
				_starstoneBundleViews[i] = view;
			}
		}

		void LoadStaminaBundles()
		{
			_staminaBundleViews = new Dictionary<int,iGUISmartPrefab_ShopStaminaBundle>();

			var items = stamina_bundles.items;
			for(int i = 0; i < items.Length; ++i)
			{
				var index = (_shopElements.Count);
				ShopItemData shopItem = null;
				if((_shopItemsIndex != null) && (_shopItemsIndex.Count > index))
				{
					shopItem = _shopItemsIndex[index];
				}
				var current = items[i];
				var order = current.order;
				var layer = current.layer;
				
				var element = (current as Placeholder).SwapForSmartObject() as iGUIContainer;
				element.setLayer(layer);
				element.setOrder(order);
				_shopElements.Add(element);
				
				var view = element.GetComponent<iGUISmartPrefab_ShopStaminaBundle>();
				if(shopItem != null)
				{
					view.SetItem(shopItem,index);
				}
				view.OnItemSelect += HandlePurchase;
				_itemButtons.Add(view.btn_buy);
				_staminaBundleViews[index] = view;
			}
		}

		void AssignButtonHandlers()
		{
			var pressable = new List<iGUIButton> ()
			{
				btn_popup_close,tab_stamina_hitbox,tab_starstone_hitbox,tarot_card_btn
			};

			pressable.AddRange(_itemButtons);

			for(int i = 0; i < pressable.Count; ++i)
			{
				var button = pressable[i];
				button.clickDownCallback += ClickInit;
			}
		}

		void SetUpButtonArtMap()
		{
			_buttonArtMap = new Dictionary<iGUIButton,iGUIElement>();
			_buttonArtMap[tarot_card_btn] = tarot_cards_grp;
			_buttonArtMap[btn_popup_close] = btn_popup_close;
			_buttonArtMap[tab_stamina_hitbox] = tab_stamina;
			_buttonArtMap[tab_starstone_hitbox] = tab_starstone;

			for(int i = 0; i < _itemButtons.Count; ++i)
			{
				var button = _itemButtons[i];
				var parentElement = _shopElements[i];
				iGUIElement element = null;
				if((parentElement.GetComponent<iGUISmartPrefab_ShopStaminaBundle>()) != null)
				{
					element = (parentElement.GetComponent<iGUISmartPrefab_ShopStaminaBundle>()).button_buy_grp;
				}
				else if((parentElement.GetComponent<iGUISmartPrefab_ShopStarstoneBundle>()) != null)
				{
					element = (parentElement.GetComponent<iGUISmartPrefab_ShopStarstoneBundle>()).button_buy_grp;
				}
				else
				{
					element = button.getTargetContainer();
				}

				_buttonArtMap[button] = element;
			}
		}

		public void ToggleActiveElements(bool value)
		{
			if(value)
			{
				_buttonHandler.Activate();
				scrollView_stamina.passive = false;
				scrollView_starstone.passive = false;
			}
			else
			{
				_buttonHandler.Deactivate();
				scrollView_stamina.passive = true;
				scrollView_starstone.passive = true;
			}
		}

		public void ChangeActiveState(int state)
		{
			_currentShop = (ShopState)state;

			UpdateActiveState();
		}

		void UpdateActiveState()
		{
			ToggleShops(_currentShop);
			UpdateTabs (_currentShop);
		}

		void ToggleShops(ShopState state)
		{
			var isStarstoneActive = (state == ShopState.STARSTONE);
			var isStaminaActive = (state == ShopState.STAMINA);

			scrollView_starstone.setEnabled(isStarstoneActive);
			main_header_starstone_shop.setEnabled(isStarstoneActive);
			starstones_text.setEnabled(isStarstoneActive);

			scrollView_stamina.setEnabled(isStaminaActive);
			main_header_stamina_potion_shop.setEnabled(isStaminaActive);
			stamina_potions_text.setEnabled(isStaminaActive);
		}

		void UpdateTabs(ShopState state)
		{
			var isStarstone = (state == ShopState.STARSTONE);
			var isStamina = (state == ShopState.STAMINA);
			var staminaLayer = (isStarstone) ? TAB_BOTTOM : TAB_TOP; 
			var starstoneLayer = (isStarstone) ? TAB_TOP : TAB_BOTTOM;

			tab_stamina.setLayer(staminaLayer);
			tab_starstone.setLayer(starstoneLayer);

			tab_stamina_hitbox.setEnabled(isStarstone);
			tab_starstone_hitbox.setEnabled(isStamina);
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


		public void DisableStarterPackButton()
		{
				_starstoneBundleViews [STARTER_INDEX_IN_STONE_SHOP].DisableStarterPackButton ();
				_staminaBundleViews[STARTER_INDEX_IN_STAMINA_SHOP].DisableStarterPackButton ();
		}

		void HandleReleasedButtonEventForStarterPack(iGUIButton button, bool isOverButton)
		{if(isOverButton)
			{
				if(!_itemButtons.Contains(button))
				{
					if(button == btn_popup_close)
					{
						SubmitResponse((int)CurrencyPurchaseResponse.CLOSE);
					}
					else if(button == tarot_card_btn)
					{
						Debug.Log("Get free things button!");
					}
					else if(button == tab_stamina_hitbox)
					{
						if(_currentShop != ShopState.STAMINA)
						{
							ChangeActiveState((int)ShopState.STAMINA);
						}
					}
					else if(button == tab_starstone_hitbox)
					{
						if(_currentShop != ShopState.STARSTONE)
						{
							ChangeActiveState((int)ShopState.STARSTONE);
						}
					}
				}
				else
				{
					var index = _itemButtons.IndexOf(button);
					var isStarstone = (index <= 5);
					bool isStarterPack = (index == STARTER_INDEX_IN_STONE_SHOP || index == STARTER_INDEX_IN_STAMINA_SHOP);
					if (isStarstone)
					{
						_starstoneBundleViews [index].ExecuteItemClick (button);
					} else if (isStarterPack) 
					{
						if (_currentShop == ShopState.STARSTONE) 
						{
							_starstoneBundleViews [STARTER_INDEX_IN_STONE_SHOP].ExecuteItemClick (button);
						} 
						else 
						{
							_staminaBundleViews[STARTER_INDEX_IN_STAMINA_SHOP].ExecuteItemClick(button);
						}
					}
					else
					{
						_staminaBundleViews[index].ExecuteItemClick(button);
					}
				}
			}

			_buttonArtMap[button].colorTo(Color.white,0.3f);
			
		}

		void HandleReleasedButtonEvent(iGUIButton button, bool isOverButton)
		{
			if(isOverButton)
			{
				if(!_itemButtons.Contains(button))
				{
					if(button == btn_popup_close)
					{
						SubmitResponse((int)CurrencyPurchaseResponse.CLOSE);
					}
					else if(button == tarot_card_btn)
					{
						Debug.Log("Get free things button!");
						//TODO Implement when we have ads
			//			if(Advertisement.isReady())
			//			{
			//				Debug.Log("Ad is ready");
			//				ShowOptions options = new ShowOptions();
			//				options.pause = true;
			//				options.resultCallback = HandleShowResult;
			//				Advertisement.Show(null, options);
			//			}
			//			else
			//			{
			//				Debug.Log("Ad is not ready");
			//			}
					}
					else if(button == tab_stamina_hitbox)
					{
						if(_currentShop != ShopState.STAMINA)
						{
							ChangeActiveState((int)ShopState.STAMINA);
						}
					}
					else if(button == tab_starstone_hitbox)
					{
						if(_currentShop != ShopState.STARSTONE)
						{
							ChangeActiveState((int)ShopState.STARSTONE);
						}
					}
				}
				else
				{
					var index = _itemButtons.IndexOf(button);
					var isStarstone = (index <= 5);
					if (isStarstone)
					{
						_starstoneBundleViews [index].ExecuteItemClick (button);
					}
					else
					{
						_staminaBundleViews[index].ExecuteItemClick(button);
					}
				}
			}

			_buttonArtMap[button].colorTo(Color.white,0.3f);
		}

		void HandlePurchase(object sender, GUIEventArgs e)
		{
			if(OnPurchaseRequest != null)
			{
				OnPurchaseRequest(sender,e);
			}
		}
	}
}

public enum CurrencyPurchaseResponse
{
	CLOSE = 0
}
