using System;
using System.Collections.Generic;
using System.Collections;

namespace Voltage.Witches.Login
{
	using UnityEngine;
    using UnityEngine.UI;

	using TMPro;

	public class BonusItemView : MonoBehaviour
	{
		[SerializeField]
		private Image _bonusBadgeImage;

		[SerializeField]
		private Image _specialBadgeImage;

        [SerializeField]
        private Image _receivedBannerImage;

		[SerializeField]
		private Image _itemImage;
//		private RawImage _bonusImage;

		[SerializeField]
		private TextMeshProUGUI _quantityLabel;


		private void Awake()
		{
			if (_itemImage == null || _receivedBannerImage == null || _specialBadgeImage == null || _bonusBadgeImage == null || _quantityLabel == null) 
			{
                throw new NullReferenceException();
			}
		}


		public void Init(Sprite sprite, int quantity, bool received=false) 
		{
            _itemImage.sprite = sprite;

			_quantityLabel.text = string.Format("x{0}", quantity);

			_receivedBannerImage.gameObject.SetActive(received);
		}



	}
		
}
