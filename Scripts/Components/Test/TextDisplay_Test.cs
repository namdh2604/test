using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using Voltage.Witches.Components.TextDisplay;

namespace Voltage.Witches.Components
{
	public class TextDisplay_Test : MonoBehaviour 
	{
		public Image Text_Frame;
		public Text Text_Display;
		private ITextPool _textPool;
		private IEnumerator _showText;
		private float _delay = 1.5f;

		void Awake()
		{
			Text_Frame.gameObject.SetActive(true);
			Text_Display = Text_Frame.GetComponentInChildren<Text>();
			Text_Frame.enabled = false;
			Text_Display.enabled = false;
			_textPool = new TextPool("hint_text");
			Debug.Log("TEXTPOOL IS NOT NULL?? :: " + (_textPool != null).ToString());
		}

		public void LoadText()
		{
			string display = "Button pressed, load text please";
			Debug.Log(display);
			if(Text_Display != null)
			{
				EnableText();
				var hints = _textPool.GetRandomizedStringSet(null);
				_showText = ShowText(hints);
				StartCoroutine(_showText);
			}
		}

		void EnableText()
		{
			if(!Text_Display.enabled)
			{
				Text_Display.enabled = true;
			}
			if(!Text_Frame.enabled)
			{
				Text_Frame.enabled = true;
			}
		}

		IEnumerator ShowText(List<string> hints)
		{
			Debug.Log("Begin displaying hints...");

			for(int i = 0; i < hints.Count; ++i)
			{
				Text_Display.text = hints[i];
				yield return new WaitForSeconds(_delay);
			}

			Debug.Log("Hint display complete");
		}
	}
}