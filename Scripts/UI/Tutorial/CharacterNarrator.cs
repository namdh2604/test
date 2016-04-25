using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

// FIXME: namespace should be Voltage.Witches.Tutorial
namespace Voltage.Witches.StoryMap
{
	using UnityEngine.UI;
	using TMPro;

    public class CharacterNarrator : MonoBehaviour
    {
		// ASSUMES asset faces left by default
		public enum FacingDirection
		{
			LEFT,
			RIGHT
		}


		[SerializeField]
		private Image _character;					// TODO: add functionality to set image w/ proper dimensions

		[SerializeField]
		private Image _dialogueBox;					// TODO: add functionality to set direction 

		[SerializeField]
		private RectTransform _dialogueBoxRoot;					

		[SerializeField]
		private TextMeshProUGUI _text;

		[SerializeField]
		private RectTransform _arrowRect;

//		[SerializeField]
		private RectTransform _root;


		private void Awake()
		{
			_root = this.GetComponent<RectTransform> ();

			if(_root ==  null)
			{
				throw new NullReferenceException();
			}

			if(_character == null || _dialogueBox == null || _text == null || _arrowRect == null)
			{
				throw new NullReferenceException();
			}

			SetText (string.Empty);
//			SetAlpha (_character, false);
//			SetAlpha (_dialogueBox, false);
//			SetAlpha (_text, false);
		}


		// ASSUMES asset faces left by default
		// regardless the relative positions of narrator to dialogue default to this
		public void SetDirection(FacingDirection dir)
		{
			Vector3 dialogueBoxPos = _dialogueBoxRoot.anchoredPosition3D;

			switch (dir) 
			{
				case FacingDirection.RIGHT:
					Vector3 horizFlip = new Vector3(-1f, 1f, 1f);
					_character.transform.localScale = horizFlip;

					_dialogueBoxRoot.localScale = horizFlip;
					_dialogueBoxRoot.anchoredPosition3D = new Vector3 (-dialogueBoxPos.x, dialogueBoxPos.y, dialogueBoxPos.z);

					float relativePosInDialogueBox = 0.75f;		// FIXME: the original anchor of arrow is offset from dialoguebox, need to account for this
					_arrowRect.anchoredPosition3D = new Vector3(-(_dialogueBoxRoot.sizeDelta.x * relativePosInDialogueBox), 0f, 0f);

					_text.transform.localScale = horizFlip;
					break;

				case FacingDirection.LEFT:
				default: 
					_character.transform.localScale = Vector3.one;

					_dialogueBoxRoot.localScale = Vector3.one;
					_dialogueBoxRoot.anchoredPosition3D = new Vector3(Mathf.Abs(dialogueBoxPos.x), dialogueBoxPos.y, dialogueBoxPos.z);
					_arrowRect.anchoredPosition3D = Vector3.zero;

					_text.transform.localScale = Vector3.one;
					break;
			}
		}


		public void SetImage(Sprite sprite)
		{
			// TODO: need to handle resizing of image to sprite dimensions
			_character.sprite = sprite;
		}


		private void SetAlpha(Graphic graphic, bool enable)
		{
			float alpha = (enable ? 1f : 0f);
			graphic.canvasRenderer.SetAlpha (alpha);		// Graphic.CrossFadeAlpha won't work if canvasRenderer's alpha isn't set
		}



		public float NarratorWidth
		{
			get { return _character.rectTransform.rect.width; }
		}


		public void SetNarratorPosition(Vector2 anchoredPos)
		{
			_root.anchoredPosition = anchoredPos;		// _character.rectTransform.anchoredPosition = pos;
		}

		public Vector2 CurrentAnchoredPosition
		{
			get { return _root.anchoredPosition; }
		}



		private IEnumerator  _narratorFadeRoutine;

		public void ShowNarrator(bool enable, float fadeDuration=0f)
		{
			StopNarratorFade ();

			EnableNarrator(true);

			if(fadeDuration > 0f)
			{
				_narratorFadeRoutine = NarratorFadeRoutine (fadeDuration, enable);
				StartCoroutine (_narratorFadeRoutine);
			}
			else 							// not relying on coroutine for immediate change
			{
				EnableNarrator(enable);
				SetAlpha(_character, enable);
			}
		}

		private IEnumerator NarratorFadeRoutine(float duration, bool enable)
		{
			float endOpacity = (enable ? 1f : 0f);
			_character.CrossFadeAlpha (endOpacity, duration, false);

			yield return new WaitForSeconds (duration);

			EnableNarrator(enable);
		}


		private void StopNarratorFade()
		{
			if(_narratorFadeRoutine != null)
			{
				StopCoroutine(_narratorFadeRoutine);
			}
		}

		private void EnableNarrator(bool enable)
		{
			_character.enabled = enable;
		}






		private IEnumerator _textFadeRoutine;

		public void SetText(string text, float fadeDuration=0f)
		{
			StopTextFade ();

//			_text.enabled = true;

			if(fadeDuration > 0f)
			{
				_textFadeRoutine = FadeTextInOut (text, fadeDuration);
				StartCoroutine (_textFadeRoutine);
			}
			else 					// not relying on coroutine for immediate change
			{
				_text.text = text;
				SetAlpha(_text, true);
			}
		}

		private IEnumerator FadeTextInOut (string newText, float fadeDuration)
		{
			if(!string.IsNullOrEmpty(_text.text))
			{
				_text.CrossFadeAlpha(0f, fadeDuration, false);
				yield return new WaitForSeconds(fadeDuration);
			}

			_text.text = newText;
			_text.CrossFadeAlpha (1f, fadeDuration, false);
		}


		private void StopTextFade()
		{
			if(_textFadeRoutine != null)
			{
				StopCoroutine(_textFadeRoutine);
			}
		}





		private IEnumerator _fadeDialogueRoutine;

		public void ShowDialogueBox(bool enable, float fadeDuration=0f)
		{
			StopDialogueFade ();

			EnableDialogBox(true);

			if(fadeDuration > 0)
			{
				_fadeDialogueRoutine = FadeDialogueRoutine(fadeDuration, enable);
				StartCoroutine (_fadeDialogueRoutine);
			}
			else  					// not relying on coroutine for immediate change
			{
				EnableDialogBox(enable);
				SetAlpha(_dialogueBox, enable);
				SetAlpha(_text, enable);
			}
		}


		private IEnumerator FadeDialogueRoutine(float duration, bool enable)
		{
			float endOpacity = (enable ? 1f : 0f);
			_dialogueBox.CrossFadeAlpha (endOpacity, duration, false);
			_text.CrossFadeAlpha (endOpacity, duration, false);

			yield return new WaitForSeconds (duration);

			EnableDialogBox (enable);
		}

		public void EnableDialogBox(bool enable)
		{
			_dialogueBoxRoot.gameObject.SetActive (enable);				// _dialogueBox.gameObject.SetActive (enable);
		}

		private void StopDialogueFade()
		{
			if(_fadeDialogueRoutine != null)
			{
				StopCoroutine(_fadeDialogueRoutine);
			}
		}

		public bool DialogueVisible
		{
			get { return _dialogueBoxRoot.gameObject.activeSelf; } 			// return _dialogueBox.gameObject.activeSelf;
		}




    }

}


