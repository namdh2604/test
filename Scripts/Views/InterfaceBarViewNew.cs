using UnityEngine;
using iGUI;
using System.Collections.Generic;
using Voltage.Witches.Events;
using Voltage.Witches.Models;

namespace Voltage.Witches.Views
{
	public class InterfaceBarViewNew : MonoBehaviour 
	{
		[HideInInspector]
		public iGUILabel focus_counter, starstone_counter, coin_counter, stamina_counter;

		[HideInInspector]
		public iGUIImage fire_stage01,fire_stage02,fire_stage03,fire_stage04,fire_stage05,lightning_stage01,
		lightning_stage02,lightning_stage03,lightning_stage04,lightning_stage05;

		[SerializeField]
		private int _focusCount = 0;
		
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
		
		bool isInit = false;
		
		protected virtual void Start()
		{
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

		public void SetCounts(Player player)
		{
			_currencyCount = player.Currency;
			_premiumCount = player.CurrencyPremium;
			_staminaCount = player.Stamina;
			_focusCount = player.Focus;
			UpdateCounts();
		}

		private void UpdateCounts()
		{
			focus_counter.label.text = FormatCount(_focusCount);
			starstone_counter.label.text = FormatCount(_premiumCount);
			coin_counter.label.text = FormatCount(_currencyCount);
			stamina_counter.label.text = FormatCount(_staminaCount);
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
			lightning_stage01.setEnabled(IsLightningImageEnabled(1));
			lightning_stage02.setEnabled(IsLightningImageEnabled(2));
			lightning_stage03.setEnabled(IsLightningImageEnabled(3));
			lightning_stage04.setEnabled(IsLightningImageEnabled(4));
			lightning_stage05.setEnabled(IsLightningImageEnabled(5));
		}

		void UpdateFocusIcon()
		{
			fire_stage01.setEnabled(IsFireImageEnabled(1));
			fire_stage02.setEnabled(IsFireImageEnabled(2));
			fire_stage03.setEnabled(IsFireImageEnabled(3));
			fire_stage04.setEnabled(IsFireImageEnabled(4));
			fire_stage05.setEnabled(IsFireImageEnabled(5));
		}
		
		public void starstone_shop_badge_Click(iGUIButton sender)
		{
			if (OnPremiumPurchaseRequest != null)
			{
				OnPremiumPurchaseRequest(this, new GUIEventArgs());
			}
		}
		
		public void home_button_Click(iGUIButton sender)
		{
			if (OnHomeNavigation != null)
			{
				OnHomeNavigation(this, new GUIEventArgs());
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
}