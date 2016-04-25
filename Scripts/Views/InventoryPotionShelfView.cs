using System.Collections.Generic;
using UnityEngine;
using iGUI;
using Voltage.Witches.Models;
using Voltage.Witches.Events;

namespace Voltage.Witches.Views
{
	public class InventoryPotionShelfView : MonoBehaviour
	{
		[HideInInspector]
		public iGUIScrollView scrollAble;
		
		[HideInInspector]
		public iGUIContainer Potions_Container;

		[HideInInspector]
		public Placeholder potionPlaceHolder_0;

		private List<iGUISmartPrefab_InventoryPotion> _potionViews;
		private List<iGUIContainer> _potionContainers;

		public GUIEventHandler OnPotionSelection;
		public GUIEventHandler PotionViewsSetUp;

		public List<iGUIButton> Potion_Buttons;

		public void SetUpView(int potionCount)
		{
			if(potionCount > 0)
			{
				Rect newPlaceholderRect = potionPlaceHolder_0.positionAndSize;
				newPlaceholderRect.x = 0.001f;
				for(int i = 1; i < potionCount; ++i)
				{
					CloneAndAddPotionPlaceholder(i,newPlaceholderRect);
				}
			}
		}

		public void UpdateView(int potionCount)
		{
			if(potionCount > _potionContainers.Count)
			{
				Rect newPotionRect = _potionContainers[_potionContainers.Count - 1].positionAndSize;
				newPotionRect.x = 0.001f;
				for(int i = _potionContainers.Count; i < potionCount; ++i)
				{
					CloneAndAddPotionView(i,newPotionRect);
				}

				ResetPositions();
			}
		}

		public void SetPotion(int index, Potion potion, int itemCount)
		{
			_potionContainers [index].setEnabled(true);
			_potionViews[index].SetPotion(potion,itemCount);
		}

		public void HidePotion(int index)
		{
			_potionContainers[index].setEnabled(false);
		}

		void CloneAndAddPotionPlaceholder(int index, Rect positionAndSize)
		{
			string newObjectName = "potion_" + index.ToString();
			GameObject clonedPlaceholder = Instantiate(potionPlaceHolder_0.gameObject,Vector3.zero,Quaternion.identity) as GameObject;
			clonedPlaceholder.name = newObjectName;
			clonedPlaceholder.gameObject.transform.parent = Potions_Container.gameObject.transform;
			iGUIElement element = clonedPlaceholder.GetComponent<iGUIElement>();
			element.setPositionAndSize(positionAndSize);
			element.setVariableName(newObjectName);
			element.order = index;
		}

		void CloneAndAddPotionView(int index, Rect positionAndSize)
		{
			string newObjectName = "potion_" + index.ToString();
			GameObject clonedContainer = Instantiate(_potionContainers[_potionContainers.Count - 1].gameObject, Vector3.zero, Quaternion.identity) as GameObject;
			clonedContainer.name = newObjectName;
			clonedContainer.gameObject.transform.SetParent(Potions_Container.gameObject.transform);
			iGUIElement element = clonedContainer.GetComponent<iGUIElement>();
			iGUIRoot.instance.refresh();
			element.setPositionAndSize(positionAndSize);
			element.setVariableName(newObjectName);
			iGUIRoot.instance.refresh();
			element.setOrder(index);
			iGUIRoot.instance.refresh();

			_potionContainers.Add(element as iGUIContainer);
			var view = element.GetComponent<iGUISmartPrefab_InventoryPotion> ();
			_potionViews.Add(view);
			view.OnPotionClick += HandlePotionClick;
			Potion_Buttons.Add(view.potion_button);
		}

		protected virtual void Start()
		{
			ResetPositions();
			LoadPlaceholders();
			scrollAble.hideScrollBars = true;
		}

		void ResetPositions()
		{
			var totalItems = Potions_Container.itemCount;
			var groupsOfTen = (float)totalItems / 10.0f;
			
			if(groupsOfTen > 0.4f)
			{
				scrollAble.setAreaWidth(groupsOfTen * 2.25f);
				var scrollAbleWidth = scrollAble.areaWidth;
				scrollAble.setAreaWidth(scrollAbleWidth - ((groupsOfTen - 1) * 0.25f));
				potionPlaceHolder_0.setX(0.075f / groupsOfTen);
			}
			else if((groupsOfTen > 0.1f) && (groupsOfTen <= 0.4f))
			{
				scrollAble.setAreaWidth(0.8f);
				var scrollAbleWidth = scrollAble.areaWidth;
				scrollAble.setAreaWidth(scrollAbleWidth - ((groupsOfTen - 1) * 0.25f));
				potionPlaceHolder_0.setX(0.075f / groupsOfTen);
			}
			else
			{
				scrollAble.setAreaWidth(0.7f);
				var scrollAbleWidth = scrollAble.areaWidth;
				scrollAble.setAreaWidth(scrollAbleWidth - ((groupsOfTen - 1) * 0.25f));
				potionPlaceHolder_0.setX(0.075f / groupsOfTen);
			}
			
//			LoadPlaceholders();
		}

		void LoadPlaceholders ()
		{
			_potionViews = new List<iGUISmartPrefab_InventoryPotion>();
			_potionContainers = new List<iGUIContainer>();

			var totalPlaceholders = Potions_Container.items;
			Potion_Buttons = new List<iGUIButton>();
			for(int i = 0; i < totalPlaceholders.Length; ++i)
			{
				var indexOrder = totalPlaceholders[i].order;
				iGUIElement element = ((Placeholder)totalPlaceholders[i]).SwapForSmartObject();
				element.setOrder(indexOrder);
				var view = element.GetComponent<iGUISmartPrefab_InventoryPotion>();
				view.OnPotionClick += HandlePotionClick;
				_potionContainers.Add((iGUIContainer)element);
				_potionViews.Add(view);
				Potion_Buttons.Add(view.potion_button);
			}

			Potions_Container.refreshRect();
			if(PotionViewsSetUp != null)
			{
				PotionViewsSetUp(this, new GUIEventArgs());
			}
		}

		public void ExecuteButtonClick(iGUIButton button)
		{
			var index = Potion_Buttons.IndexOf(button);
			var view = _potionViews[index];
			view.Potion_Click(button);
		}

		void HandlePotionClick(object sender, GUIEventArgs e)
		{
			if(OnPotionSelection != null)
			{
				OnPotionSelection(sender,e);
			}
		}
	}
}