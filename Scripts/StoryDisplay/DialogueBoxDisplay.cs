using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Voltage.Witches.AssetManagement;
using Newtonsoft.Json.Linq;
using Voltage.Witches.Bundles;

namespace Voltage.Witches.Layout
{
	public class DialogueBoxDisplay : MonoBehaviour
	{
		public TextMeshProUGUI _text;
        public TextMeshProUGUI _charName;
        private const string UNKNOWN_NAME = "????";
        private const int UNKNOWN_NAME_SPACING = 10;
        private const int NAME_SPACING = 0;
        private bool _animateText = true;
        private int textPos = 0;
        public GameObject _arrow;

        private bool _isAnimating = false;

        public bool IsAnimating()
        {
            return _isAnimating;
        }

        public void SetShowArrow(bool state)
        {
            _arrow.SetActive(state);
        }

        public void SetAnimateText(bool animText)
        {
            _animateText = animText;
        }


		public virtual void Setup(ICharacterBundleManager manager, IAvatarResourceManager avatarResourceManager, JObject config)
		{
			;
		}

        public virtual void UpdateDisplay(string text, string speaker, bool isAvatar)
		{
            _text.text = text;
            _text.maxVisibleCharacters = 0;
            textPos = 0;

            if (_charName != null)
            {
                if (string.IsNullOrEmpty(speaker))
                {
                    _charName.text = UNKNOWN_NAME;
                    _charName.characterSpacing = UNKNOWN_NAME_SPACING;
                }
                else
                {
                    _charName.text = speaker;
                    _charName.characterSpacing = NAME_SPACING;
                }
            }
		}

		public void EnableText(bool enabled)
		{
			_text.enabled = enabled;
            if (_charName != null)
            {
                _charName.enabled = enabled;
            }
		}

        public void StopAnimating()
        {
            int length = _text.textInfo.characterCount;
            _text.maxVisibleCharacters = length;
            textPos = length;
            _isAnimating = false;
            _arrow.SetActive(true);
        }

        public void Update()
        {
            int length = _text.textInfo.characterCount;
            if (_animateText)
            {
                textPos++;

                if (textPos <= length)
                {
                    _isAnimating = true;
                    _text.maxVisibleCharacters = textPos;
                    _arrow.SetActive(false);
                }
                else
                {
                    _isAnimating = false;
                    _arrow.SetActive(true);
                }
            }
            else
            {
                _text.maxVisibleCharacters = length;
                _isAnimating = false;
                _arrow.SetActive(false);
            }
        }
	}
}

