using UnityEngine;
using iGUI;
using Voltage.Witches.Events;

namespace Voltage.Witches.Views
{
	public class InterfaceBarOneItemView : MonoBehaviour
	{
		[HideInInspector]
		public iGUILabel stamina_counter;

		[HideInInspector]
		public iGUIImage lightning_stage01,lightning_stage02,lightning_stage03,lightning_stage04,lightning_stage05;

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

		public event GUIEventHandler OnHomeNavigation;

		bool isInit = false;
		
		protected virtual void Start()
		{
			UpdateCounts();
			isInit = true;
		}

		public void home_button_Click(iGUIButton sender)
		{
			if (OnHomeNavigation != null)
			{
				OnHomeNavigation(this, new GUIEventArgs());
			}
		}

		private void UpdateCounts()
		{
			stamina_counter.label.text = FormatCount(_staminaCount);
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