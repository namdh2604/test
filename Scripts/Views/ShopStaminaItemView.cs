using UnityEngine;
using iGUI;
using System.Collections;
using Voltage.Witches.Models;
using Voltage.Witches.Events;

namespace Voltage.Witches.Views
{
	public class ShopStaminaItemView : MonoBehaviour
	{
		[HideInInspector]
		public iGUILabel bundle_price_counter_label,bundle_quantity_counter_label,bundle_name_label;
		
		[HideInInspector]
		public iGUIButton btn_buy;

		[HideInInspector]
		public iGUIContainer button_buy_grp, stamina_potion_bundle_icons, icon_tag;
		
		[HideInInspector]
		public iGUIImage stamina_potion_trio, stamina_potion_hoard,stamina_potion_case,stamina_potion_set,stamina_potion_batch,divider_left,
						starter_pack,gold_frame;

		[HideInInspector] 
		public iGUIImage badge_set, badge_case, badge_hoard;
		
		public GUIEventHandler OnItemSelect;
		
		public ShopItemData ShopItem { get; protected set; }
		public int ItemIndex { get; protected set; }
		
		public void SetItem(ShopItemData itemData, int index)
		{
			ShopItem = itemData;
			ItemIndex = index;
		}
		
		private enum IconType
		{
			TRIO = 6,
			SET = 7,
			CASE = 8,
			HOARD = 9,
			STARTER = 10
		}
				
		public int StarterPackIndex {
			get {
				return (int)IconType.STARTER;
			}
		}	
		
		protected void Start()
		{
			if(ShopItem != null)
			{
				DisplayName();
				DisplayQuantity();
				DisplayPrice();
				DisplayBundle();
			}

			divider_left.setEnabled(false);
		}
		
		void DisplayName()
		{
			bundle_name_label.label.text = ShopItem.name;
		}
		
		void DisplayQuantity()
		{
			bundle_quantity_counter_label.label.text = ShopItem.premium_qty.ToString();
		}
		
		void DisplayPrice()
		{
			bundle_price_counter_label.label.text = ShopItem.price.ToString("C");
		}
		
		void DisplayBundle()
		{
			var isTrio = (((IconType)ItemIndex) == IconType.TRIO);
			var isSet = (((IconType)ItemIndex) == IconType.SET);
			var isCase = (((IconType)ItemIndex) == IconType.CASE);
			var isHoard = (((IconType)ItemIndex) == IconType.HOARD);
			var isStarter = (((IconType)ItemIndex) == IconType.STARTER);

			stamina_potion_trio.setEnabled(isTrio);
			stamina_potion_set.setEnabled(isSet);
			stamina_potion_hoard.setEnabled(isHoard);
			stamina_potion_case.setEnabled(isCase);
			starter_pack.setEnabled(isStarter);

            // retaining same design as icons
            badge_set.setEnabled(isSet);
			badge_case.setEnabled(isCase);
			badge_hoard.setEnabled(isHoard);
		}

		public void AdjustStarterPackIcon()
		{
			stamina_potion_bundle_icons.setPositionAndSize (gold_frame.positionAndSize);
			stamina_potion_bundle_icons.elementAspectRatio = 0;
			icon_tag.setEnabled (false);
		}

		public void ExecuteItemClick(iGUIButton button)
		{
			Debug.LogWarning (button.name + " pressed on " + gameObject.name + " " + gameObject.GetComponent<iGUIElement> ().order.ToString ());
			if(OnItemSelect != null)
			{
				OnItemSelect(this, new PremiumPurchaseRequestEventArgs(ShopItem));
			}
		}

		public void DisableStarterPackButton()
		{
			btn_buy.setEnabled (false);
			button_buy_grp.setEnabled(false);
			starter_pack.opacity = 0.3F;
		}
	}
}