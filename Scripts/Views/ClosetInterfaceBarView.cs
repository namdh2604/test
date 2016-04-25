using UnityEngine;
using iGUI;
using Voltage.Witches.Events;

namespace Voltage.Witches.Views
{
	public class ClosetInterfaceBarView : MonoBehaviour 
	{
		[HideInInspector]
		public iGUILabel starstone_counter, coin_counter;

		[HideInInspector]
		public iGUIImage zoom_out;

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
		
		public event GUIEventHandler OnPremiumPurchaseRequest;
		public event GUIEventHandler OnHomeNavigation;
		public event GUIEventHandler OnZoomButtonClick;
		public event GUIEventHandler OnUndressButtonClick;
		public event GUIEventHandler OnOutfitSaveRequest;
		
		bool isInit = false;
		
		protected virtual void Start()
		{
			UpdateCounts();
			isInit = true;
		}

		private void UpdateZoomIcon()
		{
			if(zoom_out.enabled)
			{
				zoom_out.setEnabled(false);
			}
			else
			{
				zoom_out.setEnabled(true);
			}
		}
		
		private void UpdateCounts()
		{
			starstone_counter.label.text = FormatCount(_premiumCount);
			coin_counter.label.text = FormatCount(_currencyCount);
		}
		
		public void starstone_buy_button_Click(iGUIButton sender)
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

		public void zoom_button_Click(iGUIButton sender)
		{
			if(OnZoomButtonClick != null)
			{
				OnZoomButtonClick(this, new GUIEventArgs());
			}
			UpdateZoomIcon();
		}

		public void takeoffall_button_Click(iGUIButton sender)
		{
			if(OnUndressButtonClick != null)
			{
				OnUndressButtonClick(this, new GUIEventArgs());
			}
		}

		public void saveoutfit_button_Click(iGUIButton sender)
		{
			if(OnOutfitSaveRequest != null)
			{
				OnOutfitSaveRequest(this, new GUIEventArgs());
			}
		}

		private string FormatCount(int count)
		{
			return count.ToString("D4");
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