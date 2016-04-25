using UnityEngine;
using System.Collections;
using System;

namespace Voltage.Witches.Screens
{
	public class AbstractDialogUGUI : MonoBehaviour, IDialog
	{
		private Action<int> _callback;

		public void Display(Action<int> callback)
		{
			_callback = callback;
		}

		protected virtual void SubmitResponse (int response)
		{
			if (_callback != null) {
				_callback (response);
			}
			GameObject.Destroy (gameObject);
		}

	}
}
