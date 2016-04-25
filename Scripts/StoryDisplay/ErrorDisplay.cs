using UnityEngine;
using TMPro;

namespace Voltage.Witches.Layout
{
	[AddComponentMenu("StoryTool/ErrorDisplay")]
	public class ErrorDisplay : MonoBehaviour
	{
		public TextMeshProUGUI _text;

		public void SetText(string text)
		{
			_text.text = text;
		}
	}
}

