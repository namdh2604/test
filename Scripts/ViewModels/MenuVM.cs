using System;

namespace Voltage.Witches.ViewModels
{
	public class MenuVM
	{
		bool _menuIsOpen;
		public bool MenuIsOpen { get; protected set; }

		public event EventHandler OnChange;

		public MenuVM()
		{
			MenuIsOpen = false;
		}

		public void SetMenuState(bool isOpen)
		{
			UnityEngine.Debug.Log("Menu state changing!");
			MenuIsOpen = isOpen;
			InvalidateData();
		}

		public void InvalidateData()
		{
			if (OnChange != null)
			{
				OnChange(this, new EventArgs());
			}
		}
	}
}