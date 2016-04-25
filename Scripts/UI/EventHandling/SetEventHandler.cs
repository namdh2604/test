using UnityEngine;
using System;
using System.Collections.Generic;

namespace Voltage.Witches.InputEvents
{
	using UnityEngine.UI;
	using UnityEngine.EventSystems;

	[RequireComponent(typeof(EventSystem))]
    public class SetEventHandler : MonoBehaviour
    {
		[SerializeField]
		private EventSystem _eventSystem;

//		[SerializeField]
//		private float _scaleFactor = 2f;

		[SerializeField]
		private float _refPixelWidthForDrag = 50f;

		[SerializeField]
		private Canvas _canvas;


		private void Awake()
		{
			if(_eventSystem == null || _canvas == null)
			{
				throw new NullReferenceException();
			}
		}

		private void Start()
		{
			SetDragThreshold ();
		}

		private void SetDragThreshold()
		{
//			float scaleFactor = (_canvas.GetComponent<RectTransform>().rect.width / Screen.width) * _scaleFactor;
//			_eventSystem.pixelDragThreshold *= (int)scaleFactor;

			_eventSystem.pixelDragThreshold = (int)((_refPixelWidthForDrag * Screen.width) / _canvas.GetComponent<RectTransform> ().rect.width);
		}

    }

}


