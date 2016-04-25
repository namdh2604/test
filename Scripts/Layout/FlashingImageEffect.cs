using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace Voltage.Witches.Layout
{
	public class FlashingImageEffect : MonoBehaviour
	{
		public float _flickerDelay = 0.5f;
		private Image _image;

		private void Awake()
		{
			_image = GetComponent<Image>();
			if (!_image)
			{
				Debug.LogError("Flashing Image requires an image");
				enabled = false;
			}
		}

		private IEnumerator Blink()
		{
			while (true)
			{
				yield return new WaitForSeconds(_flickerDelay);
				_image.enabled = !_image.enabled;
			}
		}

		private void OnEnable()
		{
			this.StopAllCoroutines();
			StartCoroutine(Blink());
		}
	}
}

