using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Voltage.Witches.UI
{
	using UnityEngine.UI;

//	[ExecuteInEditMode]
    public class HandPointer : MonoBehaviour
    {
		[SerializeField]
		private RectTransform _translation;
		[SerializeField]
		private RectTransform _rotation;
		[SerializeField]
		private Image _image;				// used to get Canvas, alternatively could GetComponentInParent<Canvas>()

		[SerializeField]
		[Range(0f, 360f)]
		private float degrees = 22f;		// euler

		[SerializeField]
		[Range(0.00f, 1.00f)]
		private float amplitude = 0.05f;	// percent of screen
		[SerializeField]
		private float frequency = 0.66f;	// cycle per second


		private IEnumerator _translatePointerRoutine;
		private IEnumerator _rotatePointerRoutine;	



		public void ShowPointer (Vector2 normalizedPosition, bool animate=false)
		{
			StopAnimationRoutine ();

			_translation.anchoredPosition = TranslatePosition(normalizedPosition);
			this.transform.SetAsLastSibling ();
			this.gameObject.SetActive (true);

			if(animate)
			{
				_translatePointerRoutine = TranslatePointerRoutine();
				StartCoroutine(_translatePointerRoutine);		// may need to stop prior routine (as position is determined formulaically you don't notice it)

				_rotatePointerRoutine = RotatePointerRoutine();
				StartCoroutine(_rotatePointerRoutine);
			}
		}

		private float GetScreenWidth()			
		{
			return _image.canvas.GetComponent<RectTransform> ().rect.width;			// Screen.width
		}

		private float GetScreenHeight()
		{
			return _image.canvas.GetComponent<RectTransform>().rect.height;			// Screen.height
		}


		private Vector2 TranslatePosition(Vector2 normalizedPosition)
		{
			float x = GetScreenWidth() * normalizedPosition.x;		// could combine into one call to get Vector2
			float y = GetScreenHeight() * normalizedPosition.y;

			return new Vector2 (x, y);
		}

		
		private IEnumerator TranslatePointerRoutine()
		{
			float originalY = _translation.anchoredPosition.y;			// continuing routine will not account for changes to position
			
			while(true)
			{
				Vector2 newPos = _translation.anchoredPosition;
				newPos.y = originalY + (Mathf.Sin(2f * Mathf.PI * Time.time * frequency) * amplitude * GetScreenHeight());

				_translation.anchoredPosition = newPos;
				
				yield return null;
			}
		}
		
		
		private IEnumerator RotatePointerRoutine()
		{
			while(true)
			{
				float angle = Mathf.Sin(2f * Mathf.PI * Time.time * frequency) * (degrees/2f);

				_rotation.rotation = Quaternion.Euler(new Vector3(0f, 0f, angle));

				yield return null;
			}
		}
		
		
		public void HidePointer ()
		{
			StopAnimationRoutine ();
			this.gameObject.SetActive (false);
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


    }

}




//		private void AnimatePointer(float speed=1f)
//		{
//			object[] args = new object[]
//			{
//				"name", "pointer",
//				"islocal", true,
//				"y", 15f,
//				"speed", 5f,
//				"easetype", iTween.EaseType.easeInOutSine,
//				"looptype", iTween.LoopType.pingPong,
//			};
//
//			iTween.MoveTo (pointerImage.gameObject, iTween.Hash (args));
//
//		}

//				iTween.Stop(pointerImage.gameObject);

