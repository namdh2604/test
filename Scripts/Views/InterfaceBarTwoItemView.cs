using UnityEngine;
using iGUI;
using Voltage.Witches.Events;

namespace Voltage.Witches.Views
{
	public class InterfaceBarTwoItemView : MonoBehaviour
	{
		[HideInInspector]
		public iGUILabel starstone_counter, coin_counter;
		
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
		
		bool isInit = false;
		
		protected virtual void Start()
		{
			UpdateCounts();
			isInit = true;
		}
		
		private void UpdateCounts()
		{
			starstone_counter.label.text = FormatCount(_premiumCount);
			coin_counter.label.text = FormatCount(_currencyCount);
		}
		
		public void add_starstones_button_Click(iGUIButton sender)
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