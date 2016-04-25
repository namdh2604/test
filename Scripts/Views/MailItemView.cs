using UnityEngine;
using iGUI;
using System.Collections.Generic;
using System.Text;
using Voltage.Witches.Models;
using Voltage.Witches.Events;

namespace Voltage.Witches.Views
{
	using Debug = UnityEngine.Debug;
    using Voltage.Witches.Controllers;

	public class MailItemView : MonoBehaviour
	{
		[HideInInspector]
        public iGUILabel item_name, item_count;

		[HideInInspector]
		public iGUIImage received_badge,received_surprint,item_MA,avatar_parts_MA,received_item_starstone,received_item_coin;

		[HideInInspector]
		public iGUIContainer btn_receive_grp;

		[HideInInspector]
		public iGUIButton btn_receive;

		public GUIEventHandler OnItemReceived;
		private const int MAX_DESCRIPTION_CHAR = 81;

        private const string COUNT_FORMAT = "x {0}";

		Item _myItem;

		protected void Start()
		{
		}

        public void SetItem(MailAttachment attachment)
        {
            _myItem = attachment.item;

            item_name.label.text = _myItem.Name;
            item_count.label.text = string.Format(COUNT_FORMAT, attachment.count);

            received_badge.setEnabled(attachment.isClaimed);
            received_surprint.setEnabled(attachment.isClaimed);
            btn_receive_grp.setEnabled((!attachment.isClaimed));
            
            LoadIcon();
        }

		bool CheckDescriptionLength(string description)
		{
			return (description.Length > MAX_DESCRIPTION_CHAR);
		}

		void LoadIcon()
		{
			if((_myItem as Ingredient) != null)
			{
				item_MA.setEnabled(true);
				Ingredient ingredient = _myItem as Ingredient;
				item_MA.image = Resources.Load<Texture2D>(ingredient.IconFilePath);
				item_MA.scaleMode = ScaleMode.ScaleToFit;
			}
			if((_myItem as Clothing) != null)
			{
				avatar_parts_MA.setEnabled(true);
				Clothing clothing = _myItem as Clothing;
				avatar_parts_MA.image = Resources.Load<Texture2D>(clothing.IconFilePath);
				avatar_parts_MA.scaleMode = ScaleMode.ScaleToFit;
			}
			if((_myItem as Potion) != null)
			{
				Potion potion = _myItem as Potion;
				Debug.Log(potion.Name + " has no image now");
				item_MA.setEnabled(true);
				iGUIContainer newContainer = CloneObjectASContainer<iGUIImage>(item_MA);
				try
				{
					iGUIElement view = newContainer.addSmartObject("InventoryPotion");
					view.setX(0.5f);
					iGUISmartPrefab_InventoryPotion component = view.GetComponent<iGUISmartPrefab_InventoryPotion>();
					component.SetPotion(potion,1);
					component.potions_number_badge.setEnabled(false);
				}
				catch(System.Exception e)
				{
					Debug.Log(e.Message);
				}
			}
			if((_myItem as StarStoneItem) != null)
			{
				received_item_starstone.setEnabled(true);
			}
			if((_myItem as CoinItem) != null)
			{
				received_item_coin.setEnabled(true);
			}
		}

		public iGUIContainer CloneObjectASContainer<T>(T element) where T : iGUIElement
		{
			iGUIContainer parent = (iGUIContainer)element.getTargetContainer();
			iGUIContainer newContainer = parent.addElement<iGUIContainer>(("item_container"), element.positionAndSize);
			newContainer.name = "item_container";
			newContainer.elementAspectRatio = element.elementAspectRatio;
			newContainer.setVariableName(("item_container"));
			newContainer.setLayer(element.layer);
			element.setEnabled(false);

			return newContainer;
		}

		public void ExecuteButtonClick(iGUIButton sender)
		{
			if(OnItemReceived != null)
			{
				OnItemReceived(this, new ItemReceivedArgs(_myItem));
			}
		}
	}
}