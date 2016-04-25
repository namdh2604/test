using System;
using System.Collections.Generic;
using UnityEngine;
using iGUI;
using Voltage.Witches.Models;
using Voltage.Witches.Events;

namespace Voltage.Witches.Views
{
	using Debug = UnityEngine.Debug;

	public class OutfitItemView : MonoBehaviour
	{
		[HideInInspector]
		public iGUIButton outfit_button;

		[HideInInspector]
		public iGUIImage outfit_icon;

        private string _name;
		private Texture2D _texture;
		public EventHandler OnOutfitSelect;

        public void SetOutfit(string name, Texture2D texture)
		{
            _name = name;
			_texture = texture;
		}

		protected void Start()
		{
			if (_texture != null)
			{
				SetImage();
			}
		}

		public void UpdateImage()
		{
			if(_texture != null)
			{
				SetImage();
			}
		}

		void SetImage()
		{
			outfit_icon.scaleMode = ScaleMode.ScaleToFit;
			outfit_icon.image = _texture;
		}

		public void ExecuteButtonPress(iGUIButton button)
		{
			if (OnOutfitSelect != null)
			{
				OnOutfitSelect(this, new OutfitSelectedEventArgs(_name));
			}
		}
	}
}