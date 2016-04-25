using System;
using System.Collections.Generic;
using System.Collections;

namespace Voltage.Witches.Login
{
	using Voltage.Witches.Screens;

	using UnityEngine;
	using UnityEngine.UI;

	using TMPro;


    using Voltage.Witches.Configuration;


	public class LoginBonusDialog : BaseUGUIScreen	// : AbstractDialogUGUI
	{

        private const int MAX_ITEMS = 4;


		[SerializeField]
		private Button _okButton;

		[SerializeField]
		private TextMeshProUGUI _itemLabel;

		// ...or could dynamically generate BonusItemView
		[SerializeField]
		private BonusItemView _pastItem;

		[SerializeField]
		private BonusItemView _currentItem;

		[SerializeField]
		private BonusItemView _nextItem;

		[SerializeField]
		private BonusItemView _futureItem;


		private void Awake()
		{
			// TODO: guard clauses
		}


        public void Init(IList<BonusItemViewModel> itemList, bool receivedLastItem=true)
		{
            if (itemList == null || itemList.Count != MAX_ITEMS)
			{
                throw new ArgumentException("Error with Bonus Item List");
			}

            SetTodaysLabel(itemList[1]);

            // could iterate thru List of items instead, but view is fixed to a few elements
            Sprite pastIcon = GetSprite(itemList[0].IconPath);
            _pastItem.Init(pastIcon, itemList[0].Quantity, receivedLastItem);                     // login bonus is received in sequence, so past item is always received

            Sprite currentIcon = GetSprite(itemList[1].IconPath);
			_currentItem.Init(currentIcon, itemList[1].Quantity);

            Sprite nextIcon = GetSprite(itemList[2].IconPath);
			_nextItem.Init(nextIcon, itemList[2].Quantity);

            Sprite futureIcon = GetSprite(itemList[3].IconPath);
			_futureItem.Init(futureIcon, itemList[3].Quantity);
		}


		private void SetTodaysLabel(BonusItemViewModel item)
		{
			_itemLabel.text = string.Format("You received x{0} <b>{1}{2}</b>!", item.Quantity, item.Name, (item.Quantity > 1 ? "s" : string.Empty));
		}


		private Sprite GetSprite(string texturePath)
		{
			Texture2D texture = Resources.Load<Texture2D>(texturePath);
            Vector2 pivot = new Vector2(0.5f, 0.5f);
            Rect dimensions = new Rect(0f, 0f, texture.width, texture.height);
            float pixelPerUnit = 100f;          // needs to conform with asset import conventions

            return Sprite.Create(texture, dimensions, pivot, pixelPerUnit);   
		}



        public void Display(Action<int> responseHandler)
        {
            SubscribeButtons (responseHandler);
        }

        private void SubscribeButtons(Action<int> responseHandler)      
        {
            Action onClick = () => 
            {
                if(responseHandler != null) 
                {
                    responseHandler(0);
                }
                Dispose ();
            };

            _okButton.onClick.AddListener(() => onClick());
        }




		protected override Voltage.Witches.Controllers.IScreenController GetController()
		{
			return null;
		}
	}

}
