
using System;
using Voltage.Witches.Controllers;
using Voltage.Witches.Views;

namespace Voltage.Witches.UI
{


	public interface IUIRibbonView		// interface too large
	{
        void Init(bool isOpen);
		void Show();
		void Hide();
        void Dispose();
		void OpenRibbon();
		void CloseRibbon();
//		void ToggleRibbon();
		void EnableToggleButtonInput (bool value);
		event Action OnOpenRibbon;
		event Action OnCloseRibbon;

		event Action OnShopButtonSelected;
		event Action OnRibbonEnabled;

		void EnableShopButtonInput (bool value);
		
		void SetPremiumCurrency (int currency);
		void SetRegularCurrency (int currency);
		
		void SetStamina (int stamina);
		void SetStaminaTimer (TimeSpan timeRemaining);
		
		void SetFocus (int focus);
		void SetFocusTimer (TimeSpan timeRemaining);
		void HideStaminaTimer ();
		void HideFocusTimer ();
		void SetNextUpdate(CountDownType type, DateTime nextUpdate);

        void MakePassive(bool value);
	}

}



