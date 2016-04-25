using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

using Voltage.Witches.Screens.Closet;

using System;
using System.Collections.Generic;

namespace Voltage.Witches.Screens.AvatarShop
{
    public class ShopCategorySubScreen : MonoBehaviour
    {
        public event Action<ScreenClothingCategory> onCategorySelected;
        public event UnityAction onClose;

        public void MakePassive(bool value)
        {
            _filterButton.interactable = !value;

            foreach (var button in _buttons)
            {
                button.interactable = !value;
            }
        }

        private void HandleCategory(ScreenClothingCategory category)
        {
            if (onCategorySelected != null)
            {
                onCategorySelected(category);
            }
        }

        private void HandleClose()
        {
            if (onClose != null)
            {
                onClose();
            }
        }

        #region Unity
        private void Awake()
        {
            for (int i = 0; i < _buttons.Count; ++i)
            {
                ScreenClothingCategory currentCategory = (ScreenClothingCategory)i;
                _buttons[i].onClick.AddListener(() => HandleCategory(currentCategory));
            }

            _filterButton.onClick.AddListener(HandleClose);
        }

        private void OnDestroy()
        {
            for (int i = 0; i < _buttons.Count; ++i)
            {
                _buttons[i].onClick.RemoveAllListeners();
            }

            _filterButton.onClick.RemoveAllListeners();
        }

        [SerializeField]
        private List<Button> _buttons;

        [SerializeField]
        private Button _filterButton;
        #endregion
    }
}

