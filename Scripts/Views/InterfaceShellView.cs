using UnityEngine;
using iGUI;
using System;
using System.Collections;
using System.Collections.Generic;
using Voltage.Witches.Events;
using Voltage.Witches.Models;

namespace Voltage.Witches.Views
{
	public class InterfaceShellView : MonoBehaviour 
	{
		[HideInInspector]
		public iGUIContainer ribbon,home_button,rewind_container,arrow_down_container,arrow_up_container,closet,avatar_parts_shop;

		[HideInInspector]
		public iGUILabel focus_counter, starstone_counter, coin_counter, stamina_counter, focus_timer_counter, stamina_timer_counter;
		
		[HideInInspector]
		public iGUIImage rib_fire_stage01,rib_fire_stage02,rib_fire_stage03,rib_fire_stage04,rib_fire_stage05,rib_lightning_stage01,
						 rib_lightning_stage02,rib_lightning_stage03,rib_lightning_stage04,rib_lightning_stage05,rib_add_starstones,
						 home,btn_rewind,zoom_out_closet,zoom_out_avatar,zoom_in_closet,zoom_in_avatar,gift_card,arrow_down,arrow_up,
						 ribbon_base_long;

		[HideInInspector]
		public iGUIButton rib_add_starstones_hitbox, btn_home,btn_rewind_hitbox,btn_gift_card,arrow_down_hitbox,arrow_up_hitbox,zoom_hitbox_avatar,zoom_hitbox_closet,
						  takeoffall_button,saveoutfit_button,ribbon_base_long_hitbox;

		[SerializeField]
		private int _focusCount = 0;

        private const string _timerFormat = "{0:0}h {1:00}m";
        private const string _defaultTime = "-h --m";

        private Player _player;
		
		public int FocusCount
		{
			get { return _focusCount; }
			set
			{ 
				_focusCount = value;
				focus_counter.label.text = FormatCount(_focusCount);
			}
		}
		
		[SerializeField]
		private int _premiumCount = 0;
		
		public int PremiumCount
		{
			get { return _premiumCount; }
			set
			{
				_premiumCount = value;
				starstone_counter.label.text = FormatCount(_premiumCount);
			}
		}
		
		[SerializeField]
		private int _currencyCount = 0;
		
		public int CurrencyCount
		{
			get { return _currencyCount; }
			set
			{
				_currencyCount = value;
				coin_counter.label.text = FormatCount(_currencyCount);
			}
		}
		
		[SerializeField]
		private int _staminaCount = 0;
		
		public int StaminaCount
		{
			get { return _staminaCount; }
			set
			{
				_staminaCount = value;
				stamina_counter.label.text = FormatCount(_staminaCount);
			}
		}
		
		public event GUIEventHandler OnPremiumPurchaseRequest;
		public event GUIEventHandler OnHomeNavigation;
		public event GUIEventHandler OnZoom;
		public event GUIEventHandler OnRewind;
		public event GUIEventHandler OnGiftCardButtonPress;

		bool isInit = false;

		public enum RibbonState
		{
			CLOSED = 0,
			OPEN = 1
		}

		public RibbonState CurrentRibbonState { get; protected set; }

		Rect _ribbonOpenState;
		Rect _ribbonClosedState;
		Coroutine _changingRibbon;
		float _moveSpeed = 0.25f;

		public InterfaceLayout Layout { get; protected set; }

		Coroutine _focusCountdown;
		Coroutine _staminaCountdown;

		protected virtual void Start()
		{
			ribbon.setEnabled(false);
			CurrentRibbonState = RibbonState.OPEN;
			GetRibbonRect();
			if((Layout != InterfaceLayout.Home_Button_Only) && (Layout != InterfaceLayout.Tutorial))
			{
				SetStartPositionForRibbon();
				UpdateCounts();
				isInit = true;
				List<iGUILabel> labels = new List<iGUILabel> ()
				{
					focus_counter,starstone_counter,coin_counter,stamina_counter
				};
				
				foreach(var label in labels)
				{
					label.style.alignment = TextAnchor.MiddleCenter;
				}
			}
			gameObject.GetComponent<iGUIContainer>().refreshStyle();
		}

		public void BeginCountDown(CountDownType type,DateTime nextUpdate)
		{
			if(type == CountDownType.FOCUS)
			{
				if(_focusCountdown != null)
				{
					StopCoroutine(_focusCountdown);
				}
				_focusCountdown = StartCoroutine(BeginFocusCountdown(nextUpdate.ToLocalTime()));
			}
			else
			{
				if(_staminaCountdown != null)
				{
					StopCoroutine(_staminaCountdown);
				}
				_staminaCountdown = StartCoroutine(BeginStaminaCountdown());
			}
		}

		IEnumerator BeginFocusCountdown(DateTime nextUpdate)
		{
            while (true)
            {
                if (_focusCount < 5)
                {
                    UpdateFocusTimer();
                }
                yield return new WaitForSeconds(10.0f);
            }
		}

        IEnumerator BeginStaminaCountdown()
        {
            while (true)
            {
                if (_staminaCount < 5)
                {
                    UpdateStaminaTimer();
                }
                yield return new WaitForSeconds(10.0f);
            }
        }

        private void UpdateStaminaTimer()
        {
            stamina_timer_counter.label.text = GetRemainingTime(_player.StaminaNextUpdate);
            stamina_timer_counter.getTargetContainer().setEnabled(true);
        }

		private void UpdateFocusTimer()
		{
            focus_timer_counter.label.text = GetRemainingTime(_player.FocusNextUpdate);
            focus_timer_counter.getTargetContainer().setEnabled(true);
		}


        private string GetRemainingTime(DateTime nextUpdate)
        {
            DateTime now = DateTime.Now;
            TimeSpan remaining = nextUpdate.ToLocalTime().Subtract(now);

            return GetRemainingTime(remaining);
        }

        private string GetRemainingTime(TimeSpan remaining)
        {
            string result;
            if (remaining.TotalSeconds > 0)
            {
                int minutesLeft = (int)remaining.TotalMinutes;
                result = string.Format(_timerFormat, Mathf.Floor(minutesLeft / 60), minutesLeft % 60);
            }
            else
            {
                result = _defaultTime;
            }

            return result;
        }

        private void UpdateStaminaLabel()
        {
            if (_player.IsStaminaMaxed())
            {
                stamina_timer_counter.getTargetContainer().setEnabled(false);
            }
            else
            {
                stamina_timer_counter.label.text = GetRemainingTime(_player.StaminaNextUpdate);
                stamina_timer_counter.getTargetContainer().setEnabled(true);
            }
        }

        private void UpdateFocusLabel()
        {
            if (_player.IsFocusMaxed())
            {
                focus_timer_counter.getTargetContainer().setEnabled(false);
            }
            else
            {
                focus_timer_counter.label.text = GetRemainingTime(_player.FocusNextUpdate);
                focus_timer_counter.getTargetContainer().setEnabled(true);
            }
        }


		private const float PERCENTAGE_OF_RIBBON_VISIBLE_WHEN_CLOSED = 0.18f;	// sync with ugui ribbon...UIRibbonView::_percentageOfRibbonVisibleWhenClosed

		void GetRibbonRect()
		{
			_ribbonOpenState = ribbon.getAbsoluteRect();
			var closeStateRect = new Rect(_ribbonOpenState);

			float percentOfRibbonHiddenWhenClosed = 1f - PERCENTAGE_OF_RIBBON_VISIBLE_WHEN_CLOSED;
			closeStateRect.y = -(_ribbonOpenState.height * percentOfRibbonHiddenWhenClosed);

			_ribbonClosedState = closeStateRect;
		}

		void SetStartPositionForRibbon()
		{
			CurrentRibbonState = RibbonState.CLOSED;
			
			if(btn_home.passive)
			{
				btn_home.passive = false;
			}
			if(zoom_hitbox_avatar.passive)
			{
				zoom_hitbox_avatar.passive = false;
			}
			if(zoom_hitbox_closet.passive)
			{
				zoom_hitbox_closet.passive = false;
			}
			if(btn_gift_card.passive)
			{
				btn_gift_card.passive = false;
			}
			if(takeoffall_button.passive)
			{
				takeoffall_button.passive = false;
			}
			if(saveoutfit_button.passive)
			{
				saveoutfit_button.passive = false;
			}

			ribbon.setPositionAndSize(_ribbonClosedState);
			UpdateArrowButtons();
			if(!ribbon.enabled)
			{
				ribbon.enabled = true;
			}
		}

		public IEnumerator UpdateRibbonPosition(RibbonState state)
		{
			Rect movePosition = _ribbonOpenState;

			if(state == RibbonState.CLOSED)
			{
				movePosition = _ribbonClosedState;
				if(btn_home.passive)
				{
					btn_home.passive = false;
				}
				if(zoom_hitbox_avatar.passive)
				{
					zoom_hitbox_avatar.passive = false;
				}
				if(zoom_hitbox_closet.passive)
				{
					zoom_hitbox_closet.passive = false;
				}
				if(btn_gift_card.passive)
				{
					btn_gift_card.passive = false;
				}
				if(takeoffall_button.passive)
				{
					takeoffall_button.passive = false;
				}
				if(saveoutfit_button.passive)
				{
					saveoutfit_button.passive = false;
				}
			}
			else
			{
				if(!btn_home.passive)
				{
					btn_home.passive = true;
				}
				if(!zoom_hitbox_avatar.passive)
				{
					zoom_hitbox_avatar.passive = true;
				}
				if(!zoom_hitbox_closet.passive)
				{
					zoom_hitbox_closet.passive = true;
				}
				if(!btn_gift_card.passive)
				{
					btn_gift_card.passive = true;
				}
				if(!takeoffall_button.passive)
				{
					takeoffall_button.passive = true;
				}
				if(!saveoutfit_button.passive)
				{
					saveoutfit_button.passive = true;
				}
			}

			ribbon.moveTo(movePosition,_moveSpeed,iTweeniGUI.EaseType.linear);
			yield return new WaitForSeconds(_moveSpeed);
			CurrentRibbonState = state;
			UpdateArrowButtons();
			if(!ribbon.enabled)
			{
				ribbon.enabled = true;
			}
			_changingRibbon = null;
		}

		void UpdateArrowButtons()
		{
			var isOpen = (CurrentRibbonState == RibbonState.CLOSED);
			var isClosed = (CurrentRibbonState == RibbonState.OPEN);

			arrow_down_container.setEnabled(isOpen);
			arrow_up_container.setEnabled(isClosed);
		}

		public void SetCounts(Player player)
		{
            _player = player;

			if(player != null)
			{
				_currencyCount = player.Currency;
				_premiumCount = player.CurrencyPremium;
				_staminaCount = player.Stamina;
				_focusCount = player.Focus;
				UpdateCounts();
			}

            UpdateStaminaLabel();
            UpdateFocusLabel();
            UpdateCounts();
		}
		
		private void UpdateCounts()
		{
			focus_counter.label.text = FormatCount(_focusCount);
			starstone_counter.label.text = FormatCount(_premiumCount);
			coin_counter.label.text = FormatCount(_currencyCount);
			stamina_counter.label.text = FormatCount(_staminaCount);

			var showFocusTimer = (_focusCount < 5);
			var showStaminaTimer = (_staminaCount < 5);

			focus_timer_counter.getTargetContainer().setEnabled(showFocusTimer);
			stamina_timer_counter.getTargetContainer().setEnabled(showStaminaTimer);

			UpdateFocusIcon();
			UpdateStaminaIcon();
		}
		
		bool IsFireImageEnabled(int index)
		{
			return(index <= FocusCount);
		}
		
		bool IsLightningImageEnabled(int index)
		{
			return(index <= StaminaCount);
		}
		
		void UpdateStaminaIcon()
		{
			rib_lightning_stage01.setEnabled(IsLightningImageEnabled(1));
			rib_lightning_stage02.setEnabled(IsLightningImageEnabled(2));
			rib_lightning_stage03.setEnabled(IsLightningImageEnabled(3));
			rib_lightning_stage04.setEnabled(IsLightningImageEnabled(4));
			rib_lightning_stage05.setEnabled(IsLightningImageEnabled(5));
		}
		
		void UpdateFocusIcon()
		{
			rib_fire_stage01.setEnabled(IsFireImageEnabled(1));
			rib_fire_stage02.setEnabled(IsFireImageEnabled(2));
			rib_fire_stage03.setEnabled(IsFireImageEnabled(3));
			rib_fire_stage04.setEnabled(IsFireImageEnabled(4));
			rib_fire_stage05.setEnabled(IsFireImageEnabled(5));
		}

		public void SetLayout(InterfaceLayout layout)
		{
			Layout = layout;
			UpdateDisplay();
		}

		void UpdateDisplay()
		{
			var isStandard = ((Layout != InterfaceLayout.StoryUI) && (Layout != InterfaceLayout.Home_Button_Only));
			var isNotHomeNorStoryPlayerScreen = ((Layout != InterfaceLayout.Home) && (Layout != InterfaceLayout.StoryUI) && (Layout != InterfaceLayout.Tutorial));
			var isAvatarShop = (Layout == InterfaceLayout.AvatarShop);
			var isCloset = (Layout == InterfaceLayout.Closet);
			var isStory = (Layout == InterfaceLayout.StoryUI);

			ribbon.setEnabled(isStandard);
			home_button.setEnabled(isNotHomeNorStoryPlayerScreen);
			rewind_container.setEnabled(isStory);
			avatar_parts_shop.setEnabled(isAvatarShop);
			closet.setEnabled(isCloset);
			//Requested to disable saving outfits for now
			saveoutfit_button.setEnabled(false);
		}

		public bool isZoomed()
		{
			return ((zoom_out_avatar.enabled) && (zoom_out_closet.enabled));
		}

		public void UpdateZoom()
		{
			if((zoom_out_avatar.enabled) && (zoom_out_closet.enabled))
			{
				zoom_out_avatar.setEnabled(false);
				zoom_out_closet.setEnabled(false);
			}
			else
			{
				zoom_out_avatar.setEnabled(true);
				zoom_out_closet.setEnabled(true);
			}
		}

		public void rib_add_starstones_hitbox_Click(iGUIButton sender)
		{
			if((OnPremiumPurchaseRequest != null) && (_changingRibbon == null))
			{
				OnPremiumPurchaseRequest(this, new GUIEventArgs());
			}
		}
		
		public void btn_home_Click(iGUIButton sender)
		{
			if((OnHomeNavigation != null) && (_changingRibbon == null))
			{
				OnHomeNavigation(this, new GUIEventArgs());
			}
		}

		public void btn_rewind_hitbox_Click(iGUIButton sender)
		{
			if((OnRewind != null) && (_changingRibbon == null))
			{
				OnRewind(this, new GUIEventArgs());
			}
		}

		public void zoom_hitbox_avatar_Click(iGUIButton sender)
		{
			if((OnZoom != null) && (_changingRibbon == null))
			{
				OnZoom(this, new GUIEventArgs());
			}
		}

		public void zoom_hitbox_closet_Click(iGUIButton sender)
		{
			if((OnZoom != null) && (_changingRibbon == null))
			{
				OnZoom(this, new GUIEventArgs());
			}
		}

		public void btn_gift_card_Click(iGUIButton sender)
		{
			if((OnGiftCardButtonPress != null) && (_changingRibbon == null))
			{
				OnGiftCardButtonPress(this, new GUIEventArgs());
			}
		}

		private string FormatCount(int count)
		{
			return count.ToString();
		}
		
		private void OnValidate()
		{
			if (!isInit)
			{
				return;
			}
			
			UpdateCounts();
		}
	
	}

	public enum CountDownType
	{
		FOCUS = 0,
		STAMINA = 1
	}

	public enum InterfaceLayout
	{
		Standard = 0,
		Home = 1,
		AvatarShop = 2,
		StoryUI = 3,
		Home_Button_Only = 4,
		Closet = 5,
		Tutorial = 6
	}
}
