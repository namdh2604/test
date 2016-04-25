using UnityEngine;
using System;
using System.Collections.Generic;

namespace Voltage.Common.UI
{
	using UnityEngine.UI;

	[RequireComponent(typeof(Button))]
    public class ButtonProxy : MonoBehaviour
    {

		public event Action OnClick;

		private Button _button;

		public Button Button { get { return _button; } }

		private void Awake()
		{
			_button = gameObject.GetComponent<Button> ();

			if(_button == null)
			{
				throw new NullReferenceException();
			}

			SubscribeButton ();
		}


		private void SubscribeButton()		
		{
			Action<Action,Button> onClick = ((action,button) => 
			{
				if(action != null)
				{
					action();
				}
			});
			
			_button.onClick.AddListener(() => onClick (OnClick, _button));
		}


		public void MakePassive(bool value)
		{
			_button.interactable = !value;
			_button.enabled = !value;
		}


		public void Dispose()
		{
			Destroy (gameObject);
		}

    }

}


