using System;
using System.Collections.Generic;
using UnityEngine;
using iGUI;
using Voltage.Witches.Models;
using Voltage.Witches.Events;

namespace Voltage.Witches.Views
{
	using Debug = UnityEngine.Debug;

	public class ClosetItemView : MonoBehaviour 
	{
		[HideInInspector]
		public iGUIImage item_icon;

		[HideInInspector]
		public iGUIButton cloth_item_button,btn_archive;

		public Clothing ClosetItem { get; protected set; }

		public void SetItem(Clothing closetItem)
		{
			ClosetItem = closetItem;
			UpdateIcon();
			gameObject.GetComponent<iGUIContainer>().refreshRect();
		}

		public void ToggleArchiveButton()
		{
			if(btn_archive.enabled)
			{
				btn_archive.setEnabled(false);
				cloth_item_button.setEnabled(true);
			}
			else
			{
				btn_archive.setEnabled(true);
				cloth_item_button.setEnabled(false);
			}
		}

		void UpdateIcon()
		{
			item_icon.setPositionAndSize(new Rect (0.5f, 0.5f, 1f, 1f));
			var texture = Resources.Load<Texture2D>(ClosetItem.IconFilePath);
			if(texture != null)
			{
				var sizes = new Vector2 (texture.width, texture.height);
				var rect = new Rect(item_icon.positionAndSize);
				var evenlySized = (sizes.x == sizes.y);
				if(!evenlySized)
				{
					rect = GetScaledRect(sizes,rect);
				}

				item_icon.image = texture;
				item_icon.setPositionAndSize(rect);
				item_icon.setEnabled(true);
			}
			else
			{
				item_icon.setEnabled(false);
			}
		}

		Rect GetScaledRect(Vector2 sizes, Rect rect)
		{
			var width = rect.width;
			var height = rect.height;
			var widthIsBigger = (sizes.x > sizes.y) ? true : false;
			if(widthIsBigger)
			{
				height = sizes.y / sizes.x;
			}
			else
			{
				width = sizes.x / sizes.y;
			}

			rect.width = width;
			rect.height = height;
			return rect;
		}

		public iGUIButton GetButton()
		{
			return cloth_item_button;
		}
	}
}