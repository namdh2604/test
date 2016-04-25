using System;
using System.Collections.Generic;
using UnityEngine;
using iGUI;
using Voltage.Witches.Events;
using Voltage.Witches.Models;
using Voltage.Witches.Util;

namespace Voltage.Witches.Views
{
	using Voltage.Common.DebugTool.Timer;

	public class CountryshelfView : MonoBehaviour 
	{
		public List<iGUIButton> Pin_Buttons { get; protected set; }

		private List<iGUIContainer> _pinContainers;
		private List<iGUISmartPrefab_CountryPinView> _pinViews;
		private List<CountryArc> _arcs;
		private CircularArray<KeyValuePair<CountryArc,int>> _arcArray;

		public GUIEventHandler OnArcSelected;
		private bool _hasSetCurrent = false;
		private int _loadedViews = 0;

		protected void Awake()
		{
			_arcArray = new CircularArray<KeyValuePair<CountryArc, int>>();
		}

		protected void Start()
		{
			LoadPlaceholders();
			SetUpPinViews();
		}

		void LoadPlaceholders()
		{
			Pin_Buttons = new List<iGUIButton>();
			_pinContainers = new List<iGUIContainer>();
			_pinViews = new List<iGUISmartPrefab_CountryPinView>();

			iGUIContainer arcPinContainer = gameObject.GetComponent<iGUIContainer>();
			var items = arcPinContainer.items;

			IList<iGUIElement> placeholders = new List<iGUIElement> ();

			for(int i = 0; i < items.Length; ++i)
			{
				var placeholder = items[i] as Placeholder;
				placeholders.Add (placeholder);

				var element = placeholder.SwapForSmartObject(false) as iGUIContainer;
				var view = element.GetComponent<iGUISmartPrefab_CountryPinView>();
				var button = view.arc_hitbox;

				Pin_Buttons.Add(button);
				_pinContainers.Add(element);
				_pinViews.Add(view);
			}

			foreach(iGUIElement arcPin in placeholders)
			{
				DestroyImmediate(arcPin.gameObject);	// replicating iGUI call
			}
			arcPinContainer.refreshRect ();
		}



		void HandleSetUpComplete()
		{
			++_loadedViews;
			if((_loadedViews >= _arcs.Count) && (!_hasSetCurrent))
			{
				SetCurrentArc(GetHighestAvailableArc());
			}
		}

		int GetHighestAvailableArc()
		{
			int index = 0;

			for(int i = 0; i < _arcs.Count; ++i)
			{
				var currentArc = _arcs[i];
				if((currentArc.AvailableScenes.Count > 0) && (currentArc.isAvailable))
				{
					index = i;
				}
			}

			return index;
		}

		public void PrepareArcs(List<CountryArc> arcs)
		{
			_arcs = arcs;
			for(int i = 0; i < _arcs.Count; ++i)
			{
				KeyValuePair<CountryArc,int> arcPair = new KeyValuePair<CountryArc, int>(_arcs[i],i);
				_arcArray.Add(arcPair);
			}

		}

		void SetUpPinViews()
		{
			for(int i = 0; i < _pinViews.Count; ++i)
			{
				var view = _pinViews[i];
				var arc = _arcArray[i].Key;
				view.SetCountryAndOrder(arc,i);
				view.OnArcSelected += HandleArcSelected;
				view.SetUpComplete += HandleSetUpComplete;
			}
		}

		void UpdatePins()
		{
			for(int i = 0; i < 5; ++i)
			{
				var currentArc = _arcArray[i].Key;
				var message = string.Format("CURRENT ARC AT {0}: {1}",i,currentArc.Name);
				Debug.LogWarning(message);
			}
		}

		void HandleArcSelected(object sender, GUIEventArgs e)
		{
			if(OnArcSelected != null)
			{
				OnArcSelected(sender,e);
			}
		}

		//TODO Use this to set current arc
		public void SetCurrentArc(int index)
		{
			var button = Pin_Buttons[index];
			_pinViews[index].ExecuteButtonClick(button);
			_hasSetCurrent = true;
		}

		public void ExecuteButtonPress(iGUIButton button)
		{
			var index = Pin_Buttons.IndexOf(button);
			_pinViews[index].ExecuteButtonClick(button);

			for(int i = 0; i < _pinViews.Count; ++i)
			{
				if(i != index)
				{
					_pinViews[i].Select(false);
				}
			}
		}
	}
}