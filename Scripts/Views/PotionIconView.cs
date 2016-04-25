using System.Collections.Generic;
using UnityEngine;
using iGUI;
using Voltage.Witches.Models;
using Voltage.Witches.Events;

namespace Voltage.Witches.Views
{
	using Debug = UnityEngine.Debug;

	public class PotionIconView : MonoBehaviour
	{
		[HideInInspector]
		public iGUIImage icon_master_label,icon_superior_label,icon_normal_label,icon_bottle_front,icon_liquid03_master,icon_liquid02_superior,icon_liquid01_basic,icon_bottle_rear;

		private Potion _myPotion;

		public void SetPotion(Potion potion)
		{
			_myPotion = potion;
		}

		public void DisplayPotion()
		{
			UpdateLabel();
			UpdateLiquid();
			ChangeLiquidColor();
		}

		void UpdateLabel()
		{
			var isMaster = (_myPotion.PotionCategory == PotionCategory.MASTER);
			var isSuperior = (_myPotion.PotionCategory == PotionCategory.SUPERIOR);
			var isBasic = (_myPotion.PotionCategory == PotionCategory.BASIC);

			icon_master_label.setEnabled(isMaster);
			icon_superior_label.setEnabled(isSuperior);
			icon_normal_label.setEnabled(isBasic);
		}

		void UpdateLiquid()
		{
			var isMaster = (_myPotion.PotionCategory == PotionCategory.MASTER);
			var isSuperior = (_myPotion.PotionCategory == PotionCategory.SUPERIOR);
			var isBasic = (_myPotion.PotionCategory == PotionCategory.BASIC);
			
			icon_liquid03_master.setEnabled(isMaster);
			icon_liquid02_superior.setEnabled(isSuperior);
			icon_liquid01_basic.setEnabled(isBasic);
		}

		void ChangeLiquidColor()
		{
			var liquid = GetActiveLiquidImage();
			if(liquid != null)
			{
				liquid.setColor(HexToColor(_myPotion.ColorCode));
			}
		}

		iGUIImage GetActiveLiquidImage()
		{
			var images = new iGUIImage[3]{icon_liquid03_master,icon_liquid02_superior,icon_liquid01_basic};
			for(int i = 0; i < images.Length; ++i)
			{
				var current = images[i];
				if(current.enabled)
				{
					return current;
				}
			}

			return null;
		}

		Color HexToColor(string hex)
		{
			byte r = byte.Parse(hex.Substring(0,2), System.Globalization.NumberStyles.HexNumber);
			byte g = byte.Parse(hex.Substring(2,2), System.Globalization.NumberStyles.HexNumber);
			byte b = byte.Parse(hex.Substring(4,2), System.Globalization.NumberStyles.HexNumber);
			return new Color32(r,g,b, 255);
		}
	}
}