using System.Collections.Generic;
using UnityEngine;
using iGUI;
using Voltage.Witches.Models;
using Voltage.Witches.Events;

namespace Voltage.Witches.Views
{
	using Debug = UnityEngine.Debug;

	public class InventoryPotionView : MonoBehaviour
	{
		[HideInInspector]
		public iGUIImage normal_label, superior_label, master_label, liquid01_basic, liquid02_superior, liquid03_master;

		[HideInInspector]
		public iGUILabel potion_name_text, potions_counter;

		[HideInInspector]
		public iGUIContainer potions_number_badge;

		[HideInInspector]
		public iGUIButton potion_button;

		public GUIEventHandler OnPotionClick;
		private Potion _myPotion;

		public void SetPotion (Potion potion, int count)
		{
			_myPotion = potion;

			normal_label.setEnabled(isBasic());
			superior_label.setEnabled(isSuperior());
			master_label.setEnabled(isMaster());

			Color potionColor = Color.white;
			try
			{
				potionColor = HexToColor(_myPotion.ColorCode);
			}
			catch(System.Exception)
			{
				var error = string.Format("POTION: {0} with ID: {1} is incorrect", _myPotion.Name,_myPotion.Id);
				
				throw new System.Exception(error);
			}
			liquid01_basic.setColor(potionColor);
			liquid01_basic.setEnabled(isMaster());
			liquid02_superior.setColor(potionColor);
			liquid02_superior.setEnabled(isSuperior());
			liquid03_master.setColor(potionColor);
			liquid03_master.setEnabled(isBasic());

			var potionNameString = StripUnnecessaryHeader (_myPotion.Name);
			potion_name_text.label.text = potionNameString;

			potions_counter.label.text = count.ToString();
			potions_counter.style.alignment = TextAnchor.MiddleCenter;

		}

		bool isMaster ()
		{
			return((_myPotion.PotionCategory == PotionCategory.MASTER));
		}

		bool isSuperior ()
		{
			return((_myPotion.PotionCategory == PotionCategory.SUPERIOR));
		}

		bool isBasic ()
		{
			return((_myPotion.PotionCategory == PotionCategory.BASIC) || (_myPotion.PotionCategory == PotionCategory.STAMINA));
		}

		string StripUnnecessaryHeader (string potionName)
		{
			if ((!potionName.Contains ("Master")) && (!potionName.Contains ("Superior"))) 
			{
				return potionName;
			}

			if (potionName.Contains ("Master")) 
			{
				var returnName = potionName.Replace ("Master ", "");
				return returnName;
			} 
			else 
			{
				var returnName = potionName.Replace ("Superior ", "");
				return returnName;
			}
		}

		Color HexToColor (string hex)
		{
			byte r = byte.Parse (hex.Substring (0, 2), System.Globalization.NumberStyles.HexNumber);
			byte g = byte.Parse (hex.Substring (2, 2), System.Globalization.NumberStyles.HexNumber);
			byte b = byte.Parse (hex.Substring (4, 2), System.Globalization.NumberStyles.HexNumber);
			return new Color32 (r, g, b, 255);
		}

		public void Potion_Click (iGUIButton button)
		{
			Debug.Log (button.container.name + " was pressed");
			if (OnPotionClick != null) {
				OnPotionClick (this, new PotionSelectedEventArgs (_myPotion));
			}
		}
	}
}