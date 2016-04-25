
using System;
using System.Collections.Generic;

namespace Voltage.Witches.Tutorial
{
	using UnityEngine;
	using iGUI;

	using Voltage.Witches.Screens;
	


	public class TutorialStoryMapScreen : AbstractDialog
    {
		public iGUIImage screenOverlay;
		public iGUIImage staminaMask;

		public iGUIContainer characterNarrator;	// HACK

		public iGUIContainer pointerRoot;

		private ICharacterAlignment _characterAlignment;
		private ICharacterNarrator _characterNarrator;

		public iGUIButton readStoryTrigger;
		public iGUIButton ribbonTrigger;
		public iGUIButton clickAnywhereTrigger;

		public event EventHandler OnRibbon;
		public event EventHandler OnReadStory;
		public event EventHandler OnClickAnywhere;


		public void readStoryTrigger_Click(iGUIButton sender)
		{
			if(OnReadStory != null)
			{
				OnReadStory (sender, null);
			}
		}

		public void ribbonTrigger_Click(iGUIButton sender)
		{
			if(OnRibbon != null)
			{
				OnRibbon (sender, null);
			}
		}

		public void clickAnywhereTrigger_Click(iGUIButton sender)
		{
			if(OnClickAnywhere != null)
			{
				OnClickAnywhere (sender, null);
			}
		}


		public void EnableTrigger(TriggerType type)
		{
			switch(type)
			{
				case TriggerType.RIBBON:
					ribbonTrigger.setEnabled(true); break;
				case TriggerType.READSTORY:
					readStoryTrigger.setEnabled(true); break;
				case TriggerType.ANYWHERE:
				default:
					clickAnywhereTrigger.setEnabled(true); break;
			}

		}

		public void DisableTrigger(TriggerType type)
		{
			switch(type)
			{
				case TriggerType.RIBBON:
					ribbonTrigger.setEnabled(false); break;
				case TriggerType.READSTORY:
					readStoryTrigger.setEnabled(false); break;
				case TriggerType.ANYWHERE:
				default:
					clickAnywhereTrigger.setEnabled(false); break;
			}
		}



		public void Awake()
		{
//			_characterNarrator = (this.GetComponentsInChildren (typeof(ICharacterNarrator), true) as ICharacterNarrator[])[0];
			_characterNarrator = this.GetComponentsInChildren<CharacterNarrator>(true)[0];

		}



		public void EnableOverlay(bool value)
		{
			screenOverlay.setEnabled(value);
		}

		public void EnableStaminaMask(bool value)
		{
			staminaMask.setEnabled (value);
		}


		public void ShowCharacter(bool value)
		{
			_characterNarrator.ShowCharacter (value);
		}

		public void ShowDialogue(string text)
		{
			_characterNarrator.SetText (text);
			_characterNarrator.ShowDialogueBox (true, CharacterNarratorDialogueAlignment.RIGHT);
		}

		public void HideDialogue()
		{
			_characterNarrator.ShowDialogueBox (false, CharacterNarratorDialogueAlignment.RIGHT);
		}



		public void ShowPointer(Vector2 position)
		{
			pointerRoot.setPosition (position);
			pointerRoot.setEnabled (true);
		}

		public void HidePointer()
		{
			pointerRoot.setEnabled (false);
		}


		public enum TriggerType
		{
			ANYWHERE,
			RIBBON,
			READSTORY
		}


    }
    
}




