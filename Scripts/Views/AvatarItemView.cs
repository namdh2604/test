using System;
using System.Collections.Generic;
using UnityEngine;
using iGUI;
using Voltage.Witches.Models;
using Voltage.Witches.Events;

namespace Voltage.Witches.Views
{
	using Debug = UnityEngine.Debug;

	public class AvatarItemView : MonoBehaviour 
	{
		[HideInInspector]
		public iGUIImage avatar_parts_MA,icon_starstone;
		
		[HideInInspector]
		public iGUIButton item_button;

		[HideInInspector]
		public iGUIContainer coin_container;

		[HideInInspector]
		public iGUILabel starstone_counter,or_label;
		
		public Clothing AvatarItem { get; protected set; }
		public bool IsAvailable { get; protected set; }
		
		public void SetItem(Clothing closetItem, bool playerDoesNotHave)
		{
			AvatarItem = closetItem;
			IsAvailable = playerDoesNotHave;
			UpdateVisual();
			UpdateIcon();
			UpdatePrices();
			gameObject.GetComponent<iGUIContainer>().refreshRect();
		}

		void UpdateVisual()
		{
			if(!IsAvailable)
			{
				avatar_parts_MA.setColor(Color.grey);
			}
		}
		
		void UpdateIcon()
		{
			avatar_parts_MA.setPositionAndSize(new Rect (0.5f, 0.5f, 1f, 1f));
			var texture = Resources.Load<Texture2D>(AvatarItem.IconFilePath);
			if(texture != null)
			{
				var sizes = new Vector2 (texture.width, texture.height);
				var rect = new Rect(avatar_parts_MA.positionAndSize);
				var evenlySized = (sizes.x == sizes.y);
				if(!evenlySized)
				{
					rect = GetScaledRect(sizes,rect);
				}
				
				avatar_parts_MA.image = texture;
				avatar_parts_MA.setPositionAndSize(rect);
				avatar_parts_MA.setEnabled(true);
			}
			else
			{
				avatar_parts_MA.setEnabled(false);
			}
		}

		Rect GetScaledRect(Vector2 sizes, Rect rect)
		{
			var width = rect.width;
			var height = rect.height;
			var widthIsBigger = (sizes.x > sizes.y) ? true : false;
			if(widthIsBigger)
			{
				height = (sizes.y / sizes.x);
			}
			else
			{
				width = (sizes.x / sizes.y);
			}
			
			rect.width = width;
			rect.height = height;
			return rect;
		}

		void UpdatePrices()
		{
			switch(AvatarItem.CurrencyType)
			{
			case PURCHASE_TYPE.NONE:
				Debug.Log("DEFAULT");
				starstone_counter.setEnabled(false);
				coin_container.setEnabled(false);
				icon_starstone.setEnabled(false);
				break;
			case PURCHASE_TYPE.COIN:
				Debug.Log("Coin purchase");
				starstone_counter.setEnabled(false);
				coin_container.setEnabled(true);
				or_label.setEnabled(false);
				icon_starstone.setEnabled(false);
				break;
			case PURCHASE_TYPE.PREMIUM:
				Debug.Log("Premium Purchase");
				starstone_counter.setEnabled(true);
				coin_container.setEnabled(false);
				icon_starstone.setEnabled(true);
				break;
			case PURCHASE_TYPE.BOTH:
				Debug.Log("Any way you want it, that's the way you need it");
				starstone_counter.setEnabled(true);
				coin_container.setEnabled(true);
				icon_starstone.setEnabled(true);
				break;
			}
			starstone_counter.label.text = AvatarItem.PremiumPrice.ToString();
		}

		bool isPremiumOrCoin()
		{
			return (AvatarItem.CurrencyType == PURCHASE_TYPE.BOTH);
		}

		public iGUIButton GetButton()
		{
			return item_button;
		}
		
		public void item_button_Click(iGUIButton sender)
		{
			//
		}
	}
}