using UnityEngine;
using System;
using System.Collections.Generic;
using System.Collections;


// FIXME: iGUI characternarrator to be replaced by Voltage.Witches.StoryMap.CharacterNarrator (StoryMap namespace to be changed)
namespace Voltage.Witches.Tutorial
{
	using iGUI;

	public interface ICharacterNarrator
	{
		void SetCharacter (Texture characterImage);
		void ShowCharacter (bool value);
		void SetText(string text);
		void ShowDialogueBox (bool value, CharacterNarratorDialogueAlignment alignment);
		void HideBlinkingDownArrow();
//		void SetDialogueBox

        bool enabled { get; set; }
	}

    public class CharacterNarrator : MonoBehaviour, ICharacterNarrator
    {
        public iGUIContainer element;
		public iGUIImage CharacterImage;

		public iGUIContainer DialogueBox;
		public iGUIImage DialogueBoxImage;
		public iGUILabel DialogueBoxLabel;
		public iGUIImage DownArrowImage;

		public float _flickerDelay = 0.5f;


		private void Awake()
		{
			UpdateFontSizeForAspectRatio();
		}

        private void Start()
        {
            // can't be called in Awake() as certain iGUI components are not available yet
            UpdateDialogBoxPositionForAspectRatio();
        }

        // HACK: adjust dynamic font as determined by aspect ratio 
        private void UpdateFontSizeForAspectRatio()
        {
            Double aspectRatio = System.Math.Round((Screen.width / (Double)Screen.height), 1); // 1.777 rounds up to 1.8
            Double fourThirdRatio = 1.3D;

            float fourThirdSize = 0.39f;
            float defaultSize = 0.45f;
            float fontSize = aspectRatio == fourThirdRatio ? fourThirdSize : defaultSize;    

            DialogueBoxLabel.dynamicFontSize = fontSize;
        }

        // HACK: adjust position of dialogue to work with different aspect ratios
        private void UpdateDialogBoxPositionForAspectRatio()
        {
            Double aspectRatio = System.Math.Round((Screen.width / (Double)Screen.height), 1);   // 1.777 rounds up to 1.8
            Double thresholdRatio = 1.6D; 

            float widerPosX = 0.48f;
            float defaultPosX = 0.55f;
            float posX = aspectRatio >= thresholdRatio ? widerPosX : defaultPosX;    

            DialogueBox.setX(posX);
        }



		public void Init(Texture characterImage, string text="")
		{
			SetCharacter (characterImage);
			SetText (text);
		}

		public Rect GetNarratorImagePos()
		{
			return CharacterImage.positionAndSize;
		}

		public void SetNarratorPos(Rect newNarratorpos)
		{
			CharacterImage.setPositionAndSize (newNarratorpos);
		}

		public void SetCharacter(Texture characterImage)
		{
			CharacterImage.image = characterImage;
		}

		public void MoveCharacterToInTime(Vector2 newNarratorpos, float time)
		{
			CharacterImage.positionTo (newNarratorpos, time);
		}

		public void ShowCharacter(bool value)
		{
			CharacterImage.setEnabled (value);
		}

        private void OnEnable()
        {
            element.setEnabled(true);
        }

        private void OnDisable()
        {
            element.setEnabled(false);
        }


		private IEnumerator _blinkingRoutine;

		public void ShowDialogueBox (bool enabled, CharacterNarratorDialogueAlignment alignment)
		{
//			SetDialogueBoxAlignment (alignment);

			if (enabled) 
			{
				ShowDialogue();
			} else 
			{
				HideDialogue();
			}
		}

		public void HideBlinkingDownArrow()
		{
			this.StopAllCoroutines();
			DownArrowImage.enabled = false;
		}
		
		private IEnumerator Blink()
		{
			while (true)
			{
				DownArrowImage.enabled = !DownArrowImage.enabled;
				yield return new WaitForSeconds(_flickerDelay);
			}
		}
		
		private void StartBlink()
		{
			if(_blinkingRoutine == null)
			{
				_blinkingRoutine = Blink ();
			}
			else
			{
				StopCoroutine(_blinkingRoutine);
			}

			StartCoroutine (_blinkingRoutine);
		}


		private void ShowDialogue()
		{
			DialogueBox.setEnabled (true);
			if (DownArrowImage != null)
			{
				AdjustArrowPosition ();
				StartBlink ();
			}
		}

		private void HideDialogue()
		{
			DialogueBox.setEnabled (false);
		}


//		private void SetDialogueBoxAlignment(CharacterNarratorDialogueAlignment alignment)	// HACK
//		{
//			switch(alignment)
//			{
//				case CharacterNarratorDialogueAlignment.RIGHT:
//					DialogueBox.setPosition(new Vector2(-270, 95));
//					DialogueBoxImage.setScale(new Vector2(1,1));
//					break;
//
//				case CharacterNarratorDialogueAlignment.LEFT:
//				default:
//					DialogueBox.setPosition(new Vector2(240, 95));	
//					DialogueBoxImage.setScale(new Vector2(-1,1));
//					break;
//			}
//		}


		public void SetText(string text)
		{


			DialogueBoxLabel.label.text = text;
		}


		private float ArrowSizeXAdjustment = 0.333F;		// HACK adjustment needed since the art is taken from other scene resource. arrow needed to be shrunk to 3%
		private float ArrowSizeYAdjustment = 0.6F;		// HACK adjustment needed since the art is taken from other scene resource. arrow needed to be shrunk to 3%
		private float ArrowSizeAdjustment = 3F;		// HACK adjustment needed since the art is taken from other scene resource. arrow needed to be shrunk to 3%
		private void AdjustArrowPosition() 
		{
			Rect ArrowSize = DialogueBox.positionAndSize;

			Rect arrowAdjustedSizeToFit = new Rect (ArrowSize.x * ArrowSizeXAdjustment, ArrowSize.y * ArrowSizeYAdjustment, ArrowSize.width * ArrowSizeAdjustment, ArrowSize.height * ArrowSizeAdjustment );
			DownArrowImage.setPositionAndSize (arrowAdjustedSizeToFit);

			float width = DialogueBox.containerRect.width;
			float height = DialogueBox.containerRect.height;
			float arrowAdjustedX = width * 0.77f;
			float arrowAdjustedY = height * 0.325f;
			DownArrowImage.rect = new Rect (arrowAdjustedX, arrowAdjustedY, DownArrowImage.rect.width, DownArrowImage.rect.height);
		}

    }

	public enum CharacterNarratorDialogueAlignment
	{
		LEFT,
		RIGHT,
	}

}


