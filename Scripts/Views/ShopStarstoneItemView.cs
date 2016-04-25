using UnityEngine;
using iGUI;
using System.Collections;
using Voltage.Witches.Models;
using Voltage.Witches.Events;

namespace Voltage.Witches.Views
{
	public class ShopStarstoneItemView : MonoBehaviour
	{
		[HideInInspector]
		public iGUILabel bundle_price_counter_label,bundle_quantity_counter_label,bundle_name_label;

		[HideInInspector]
		public iGUIButton btn_buy;

		[HideInInspector]
		public iGUIContainer button_buy_grp, starstone_bundle_icon, icon_tag;

		[HideInInspector]
		public iGUIImage starstone_trio, starstone_cache,starstone_constellation,starstone_hoard,starstone_chest,starstone_assembly,
						starter_pack, divider_left, gold_frame;

		[HideInInspector] 
		public iGUIImage badge_cache, badge_assembly, badge_chest, badge_hoard, badge_constellation;

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
			TRIO = 0,
			CACHE = 1,
			ASSEMBLY = 2,
			CHEST = 3,
			HOARD = 4,
			CONSTELLATION = 5,
			STARTER = 10,

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
			var isCache = (((IconType)ItemIndex) == IconType.CACHE);
			var isAssembly = (((IconType)ItemIndex) == IconType.ASSEMBLY);
			var isChest = (((IconType)ItemIndex) == IconType.CHEST);
			var isHoard = (((IconType)ItemIndex) == IconType.HOARD);
			var isConstellation = (((IconType)ItemIndex) == IconType.CONSTELLATION);
			var isStarter = (((IconType)ItemIndex) == IconType.STARTER);

			starstone_trio.setEnabled(isTrio);
			starstone_cache.setEnabled(isCache);
			starstone_constellation.setEnabled(isConstellation);
			starstone_hoard.setEnabled(isHoard);
			starstone_chest.setEnabled(isChest);
			starstone_assembly.setEnabled(isAssembly);
			starter_pack.setEnabled(isStarter);

            // retaining same design as icons
            badge_cache.setEnabled(isCache);
            badge_assembly.setEnabled(isAssembly);
            badge_chest.setEnabled(isChest);
            badge_hoard.setEnabled(isHoard);
            badge_constellation.setEnabled(isConstellation);
		}



		public void AdjustStarterPackIcon()
		{
			starstone_bundle_icon.setPositionAndSize (gold_frame.positionAndSize);
			starstone_bundle_icon.elementAspectRatio = 0;
			icon_tag.setEnabled (false);
		}

		public void ExecuteItemClick(iGUIButton button)
		{
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