using UnityEngine;
using System;
using System.Collections.Generic;

namespace Voltage.Witches.StoryMap
{
	using UnityEngine.UI;
	using TMPro;

	using Voltage.Witches.UI;

    public class ArcButton : MonoBehaviour	
    {
		[SerializeField]
		private Button _button;
		public event EventHandler OnButtonClick;

		[SerializeField]
		private TextMeshProUGUI _label;
		[SerializeField]
		private Image _labelBG;


		[SerializeField]
		private Image _normalImage;
		[SerializeField]
		private Image _highlightImage;		// simple sprite replace won't work, icon is larger

		public string Arc { get { return _label.text; } }	// or make into getter/private setter

		public void Awake()
		{
			if(_button == null || _label == null || _labelBG == null || _normalImage == null || _highlightImage == null)
			{
				throw new NullReferenceException();
			}
		}

		public void SetName(string name)
		{
			_label.text = name;
		}

		public void SetImages(Sprite normal, Sprite highlighted)
		{
			_normalImage.sprite = normal;
			_highlightImage.sprite = highlighted;
		}

		public void HighlightButton(bool value)
		{
			_highlightImage.enabled = value;
			_normalImage.enabled = !value;
		}

		public bool IsHighlighted
		{
			get { return _highlightImage.isActiveAndEnabled; }
		}

		public void EnableButton(bool value)
		{
			_button.interactable = value;
		}

        public void MakePassive(bool value)
        {
			if (_button.interactable) 
			{
				_button.enabled = !value;
			}
        }

		public void OnClick()
		{
            bool isReady = !IsHighlighted;
            HighlightButton(true);

            if (OnButtonClick != null && isReady)
			{
				OnButtonClick (this, new EventArgs ());
			}
		}
    }

}


