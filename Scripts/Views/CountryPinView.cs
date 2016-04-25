using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using iGUI;
using Voltage.Witches.Events;
using Voltage.Witches.Models;

namespace Voltage.Witches.Views
{
	public class CountryPinView : MonoBehaviour
	{
		[HideInInspector]
		public iGUIButton arc_hitbox;

		[HideInInspector]
		public iGUIContainer pin_container,country_icon_selected,country_icon_regular;

		[HideInInspector]
		public iGUIImage Werbury,Ireland,Germany,Czech,Salem,salem_selected_icon,werbury_selected_icon,ireland_selected_icon,germany_selected_icon,
						 czech_selected_icon, salem_icon,werbury_icon,ireland_icon,germany_icon,czech_icon;

		private Dictionary<string,List<iGUIImage>> _nameArtMap;
		private Dictionary<iGUIImage,iGUIImage> _iconMap;

		public GUIEventHandler OnArcSelected;
		public int Order { get; protected set; }
		public string Country { get; protected set; }
		//TODO Change this to use Michael's update
		public CountryArc Arc { get; protected set; }

		public delegate void SetUpCallback();
		public event SetUpCallback SetUpComplete;

		public void SetCountryAndOrder(CountryArc arc,int order)
		{
			string country = arc.Name;

			if(country == "Werbury")
			{
				country = "Prologue";
			}

			Arc = arc;
			Country = country;
			Order = order;
		}

		void Reset()
		{
			AssignIcons();
			SetOrder();
		}

		protected void Start()
		{
			SetUpMapping();
			SetUpIconMapping();
			if(!string.IsNullOrEmpty(Country))
			{
				AssignIcons();
				SetOrder();
			}

			if(SetUpComplete != null)
			{
				SetUpComplete();
			}
		}

		void SetUpMapping()
		{
			_nameArtMap = new Dictionary<string, List<iGUIImage>> ()
			{
				{"Prologue", new List<iGUIImage>(){ Werbury,werbury_icon }},
				{"Salem", new List<iGUIImage>(){ Salem,salem_icon }},
				{"Ireland", new List<iGUIImage>(){ Ireland,ireland_icon }},
				{"Germany", new List<iGUIImage>(){ Germany,germany_icon }},
				{"Prague", new List<iGUIImage>(){ Czech,czech_icon }}
			};
		}

		void SetUpIconMapping()
		{
			_iconMap = new Dictionary<iGUIImage,iGUIImage> ()
			{
				{werbury_icon,werbury_selected_icon},
				{salem_icon,salem_selected_icon},
				{ireland_icon,ireland_selected_icon},
				{germany_icon,germany_selected_icon},
				{czech_icon,czech_selected_icon}
			};
		}

		void AssignIcons()
		{
			List<iGUIImage> icons = new List<iGUIImage>();
			if(_nameArtMap.TryGetValue(Country, out icons))
			{
				for(int i = 0; i < icons.Count; ++i)
				{
					icons[i].setEnabled(true);
				}
			}
		}

		void SetOrder()
		{
			pin_container.setOrder(Order);
		}

		public void ExecuteButtonClick(iGUIButton button)
		{
			if((OnArcSelected != null) && (button == arc_hitbox))
			{
				OnArcSelected(this, new ArcSelectedEventArgs(Arc));
				Select(true);
			}
		}

		public void Select(bool isSelected)
		{
			var regularIcon = GetActiveIcon();
			var selectedIcon = _iconMap[regularIcon];

			regularIcon.setEnabled((!isSelected));
			selectedIcon.setEnabled(isSelected);
		}

		iGUIImage GetActiveIcon()
		{
			return _nameArtMap[Country][1];
		}
	}
}
