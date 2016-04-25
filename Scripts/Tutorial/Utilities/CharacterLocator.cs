using UnityEngine;
using System;
using System.Collections.Generic;

namespace Voltage.Witches.Tutorial
{
	using iGUI;

	public interface ICharacterAlignment
	{
		void Show (ICharacterNarrator narrator, CharacterScreenAlignment alignment);
		void Hide (CharacterScreenAlignment alignment);
	}

    public class CharacterLocator : MonoBehaviour, ICharacterAlignment
    {
		public Transform LeftLoc;
		public Transform RightLoc;

		public iGUIContainer LeftContainer;
		public iGUIContainer RightContainer;

		private void Awake()
		{
//			Hide (CharacterScreenAlignment.LEFT);
//			Hide (CharacterScreenAlignment.RIGHT);
		}

		public void Show(ICharacterNarrator narrator, CharacterScreenAlignment alignment)
		{
			// HACK
			switch(alignment)
			{
				case CharacterScreenAlignment.RIGHT:
					RightContainer.setEnabled(true); break;

				case CharacterScreenAlignment.LEFT:
					LeftContainer.setEnabled(true); break;

				default: break;
			}
		}

		public void Hide(CharacterScreenAlignment alignment)
		{
			// HACK
			switch(alignment)
			{
				case CharacterScreenAlignment.RIGHT:
					RightContainer.setEnabled(false); break;
					
				case CharacterScreenAlignment.LEFT:
					LeftContainer.setEnabled(false); break;

				default: break;
			}
		}

    }

	public enum CharacterScreenAlignment
	{
		LEFT,
		RIGHT
	}

}


