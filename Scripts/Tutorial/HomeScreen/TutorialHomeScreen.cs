
using System;
using System.Collections;
using System.Collections.Generic;

namespace Voltage.Witches.Tutorial
{
	using UnityEngine;
	using iGUI;

	using Voltage.Witches.Screens;

	public class TutorialHomeScreen : AbstractDialog
	{
		[HideInInspector]
		public iGUIImage screenOverlay;
		public iGUIImage pointerImage;
		[HideInInspector]
		public iGUIContainer pointerRoot;
		[HideInInspector]
		public iGUIContainer dialogueBox;

//		[HideInInspector]
		public iGUIButton clickAnywhereTrigger;
		public event EventHandler OnClickAnywhere;

		private CharacterNarrator _characterNarrator;


		public const string READ_STORY_BUTTON = "read_story_button";
		public const float SLIDE_IN_SPEED = 1.5f;

		public void Awake ()
		{
			_characterNarrator = this.GetComponentsInChildren<CharacterNarrator> (true) [0];	// this.GetComponentsInChildren (typeof(ICharacterNarrator), true) as ICharacterNarrator[]
		}

		public void EnableTrigger(TriggerType type)
		{
			clickAnywhereTrigger.setEnabled (true);
		}

		public void DisableTrigger(TriggerType type)
		{
			clickAnywhereTrigger.setEnabled (false);
		}

		public void clickAnywhereTrigger_Click(iGUIButton sender)
		{
			if(OnClickAnywhere != null)
			{
				OnClickAnywhere (sender, null);
			}
		}

		public void EnableOverlay (bool value)
		{
			screenOverlay.setEnabled (value);
		}

		public IEnumerator SlideInNarrator ()
		{
			Rect narratorPos = _characterNarrator.GetNarratorImagePos ();
			Rect hideNarrator = new Rect (-narratorPos.width, narratorPos.y, narratorPos.width, narratorPos.height);
			_characterNarrator.SetNarratorPos (hideNarrator);
			_characterNarrator.enabled = true;
			Vector2 showNarrator = new Vector2 (narratorPos.x, narratorPos.y);
			_characterNarrator.MoveCharacterToInTime (showNarrator, SLIDE_IN_SPEED);

			yield return new WaitForSeconds(SLIDE_IN_SPEED);
		}

		public void ShowCharacter (bool value)
		{
			_characterNarrator.ShowCharacter (value);
            _characterNarrator.enabled = true;
		}

		public void ShowDialogue (string text)
		{
			_characterNarrator.SetText (text);
			_characterNarrator.ShowDialogueBox (true, CharacterNarratorDialogueAlignment.RIGHT);
            _characterNarrator.enabled = true;
		}

		public void HideDialogue ()
		{
			_characterNarrator.ShowDialogueBox (false, CharacterNarratorDialogueAlignment.RIGHT);
		}



		[Range(0f, 360f)]
		public float degrees = 22f;
		
		[Range(0.00f, 1.00f)]
		public float amplitude = 0.05f;
		public float frequency = 0.66f;	// cycle per second

		private IEnumerator _rotatePointerRoutine;	
		private IEnumerator _translatePointerRoutine;


		public void ShowPointer (Vector2 position, bool animate=false)
		{
			StopAnimationRoutine ();

			pointerRoot.setPosition (position);
			pointerRoot.setEnabled (true);

			if(animate)
			{
				_translatePointerRoutine = TranslatePointerRoutine();
				StartCoroutine(_translatePointerRoutine);					

				_rotatePointerRoutine = RotatePointerRoutine();
				StartCoroutine(_rotatePointerRoutine);
			}
		}

		private IEnumerator TranslatePointerRoutine()
		{
			float originalY = pointerRoot.positionAndSize.y;

			while(true)
			{
				Vector2 newPos = pointerRoot.positionAndSize.position;
				newPos.y = originalY + (Mathf.Sin(2f * Mathf.PI * Time.time * frequency) * amplitude);
				
				pointerRoot.setPosition(newPos);

				yield return null;
			}
		}
		
		private IEnumerator RotatePointerRoutine()
		{
			while(true)
			{
				float angle = Mathf.Sin(2f * Mathf.PI * Time.time * frequency) * (degrees/2f);
				
				pointerImage.setRotation(angle);
				
				yield return null;
			}
		}

		public void HidePointer ()
		{
			StopAnimationRoutine ();
			pointerRoot.setEnabled (false);
		}

		private void StopAnimationRoutine()
		{
			if(_rotatePointerRoutine != null)
			{
				StopCoroutine(_rotatePointerRoutine);
				_rotatePointerRoutine = null;
			}
			
			if(_translatePointerRoutine != null)
			{
				StopCoroutine(_translatePointerRoutine);
				_translatePointerRoutine = null;
			}
		}



		public iGUIButton GetClonedButton (string buttonName)
		{
			iGUIButton clonedButton = null;
			iGUIElement[] children = this.GetComponentsInChildren<iGUIElement> ();
			for (int i = 0; i < children.Length; i++) {
				var current = children [i];
				if ((current.name == buttonName)) {
					clonedButton = (iGUIButton)current;
				}
			}
							
			return clonedButton;
		}

		public void HideBlinkingDownArrow()
		{
			_characterNarrator.HideBlinkingDownArrow ();
		}

		public enum TriggerType
		{
			ANYWHERE,
			READSTORY,
		}


	}
    
}




