using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using iGUI;
using Voltage.Witches.Lib;

namespace Voltage.Witches.Views
{
	public class DifficultyView : MonoBehaviour
	{
		[HideInInspector]
		public iGUIImage current_difficulty_highlight;

		[HideInInspector]
		public iGUIContainer difficulties;

		[HideInInspector]
		public iGUIImage easy_label, normal_label, tricky_label, hard_label, trouble_label;

		private Dictionary<MiniGameDifficulty, iGUIElement> _difficultyImages;
		private Dictionary<MiniGameDifficulty, float> _selectedLocation;

		public MiniGameDifficulty difficultyValue;
		private MiniGameDifficulty _currentDifficulty;

		protected virtual void Awake()
		{

		}

		protected virtual void OnValidate()
		{
			if (difficultyValue != _currentDifficulty)
			{
				SetDifficulty(difficultyValue);
			}
		}

		protected virtual void Start()
		{
			_difficultyImages = new Dictionary<MiniGameDifficulty, iGUIElement>();
			_difficultyImages[MiniGameDifficulty.Trouble] = trouble_label;
			_difficultyImages[MiniGameDifficulty.Hard] = hard_label;
			_difficultyImages[MiniGameDifficulty.Tricky] = tricky_label;
			_difficultyImages[MiniGameDifficulty.Normal] = normal_label;
			_difficultyImages[MiniGameDifficulty.Easy] = easy_label;

			_selectedLocation = new Dictionary<MiniGameDifficulty, float>();
			foreach (var entry in _difficultyImages)
			{
				_selectedLocation[entry.Key] = GetVerticalCenter(entry.Value);
			}
		}

		private float GetVerticalCenter(iGUIElement element)
		{
			Rect bounds = element.getAbsoluteRect();
			return bounds.y + bounds.height / 2.0f;
		}

		private void CenterVertically(iGUIElement element, float y)
		{
			float newY = y - element.getAbsoluteRect().height / 2.0f;
			float adjustedY = newY - element.container.getAbsoluteRect().y;

			element.setPosition(new Vector2(element.positionAndSize.x, adjustedY));
		}

		public void SetDifficulty(MiniGameDifficulty newDifficulty)
		{
			if(_selectedLocation != null)
			{
				_currentDifficulty = newDifficulty;
				var targetLocation = _selectedLocation[_currentDifficulty];
				CenterVertically(current_difficulty_highlight, targetLocation);
			}
			else
			{
				StartCoroutine(WaitToDisplayDifficulty(newDifficulty));
			}
		}

		IEnumerator WaitToDisplayDifficulty(MiniGameDifficulty newDifficulty)
		{
			while(_selectedLocation == null)
			{
				yield return new WaitForSeconds(0.1f);
			}

			_currentDifficulty = newDifficulty;
			var targetLocation = _selectedLocation[_currentDifficulty];
			CenterVertically(current_difficulty_highlight, targetLocation);
		}
	}
}

