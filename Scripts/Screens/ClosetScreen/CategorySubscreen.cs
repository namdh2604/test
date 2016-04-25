using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System;
using System.Collections.Generic;

namespace Voltage.Witches.Screens.Closet
{
	public class CategorySubscreen : MonoBehaviour
	{
		public event Action<ScreenClothingCategory> onCategorySelected;
		public event UnityAction onClose;

        public void MakePassive(bool value)
        {
            _filterButton.interactable = !value;
            _sortButton.interactable = !value;
            _archiveButton.interactable = !value;
            _closetSpaceButton.interactable = !value;

            foreach (var button in _buttons)
            {
                button.interactable = !value;
            }
        }

		#region Unity
		private void Awake()
		{
			if (onCategorySelected != null)
			{
				for (int i = 0; i < _buttons.Count; ++i)
				{
					ScreenClothingCategory currentCategory = (ScreenClothingCategory)i;
					_buttons[i].onClick.AddListener(() => onCategorySelected(currentCategory));
				}
			}

			if (onClose != null) { _filterButton.onClick.AddListener(onClose); };
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
        [SerializeField]
        private Button _sortButton;
        [SerializeField]
        private Button _archiveButton;
        [SerializeField]
        private Button _closetSpaceButton;

		#endregion
	}
}

