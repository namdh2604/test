using UnityEngine;
using System;
using System.Collections.Generic;

namespace Voltage.Common.UI
{

	using UnityEngine.UI;

	// could be based off a dynamic radio button implementation
    public class ToggleWidget : MonoBehaviour	
    {
		[SerializeField]
		private Button _onCollider;			// maybe can use a collider instead of button?
		[SerializeField]
		private Button _offCollider;


		public event Action<bool> OnToggle;

//		private bool _isEnabled = false;	// adding this to be more explicit about whether state is on or off
//		public bool IsEnabled { get { return _isEnabled; } }


		private void Awake()
		{
			if(_onCollider == null || _offCollider == null)
			{
				throw new NullReferenceException();
			}

			SubscribeButtons ();

			// TODO: guard clause: buttons subscribed

//			ToggleOn (false);
		}

		private void SubscribeButtons()
		{
			_onCollider.onClick.AddListener (() => ToggleOn (false));
			_offCollider.onClick.AddListener (() => ToggleOn (true));
		}

        public void SetValue(bool value)
        {
            _onCollider.gameObject.SetActive(value);
            _offCollider.gameObject.SetActive(!value);
        }


		public void ToggleOn(bool value)					// explicit state being passed in, so not so much a "toggle"
		{
            SetValue(value);

			if (OnToggle != null)
			{
				OnToggle(value);
			}
		}

		public void MakePassive(bool value)
		{
			_onCollider.enabled = !value;
			_offCollider.enabled = !value;
		}

    }

}


