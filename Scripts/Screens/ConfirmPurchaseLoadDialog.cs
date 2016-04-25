using iGUI;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Voltage.Witches.Screens
{
	public class ConfirmPurchaseLoadDialog : AbstractDialog
	{
		[HideInInspector]
		public iGUILabel progress_label,message_label;

		bool _isLoading = false;
		int MAX_CHAR = 0;
		int _count = 0;
		float _time = 0.0f;

		protected virtual void Start()
		{
			MAX_CHAR = progress_label.label.text.Length;
			progress_label.label.text = string.Empty;
		}

		protected virtual void Update()
		{
			if(_isLoading)
			{
				_time += Time.deltaTime;
				var timeInt = Mathf.FloorToInt(_time);
				if(((timeInt % 1) == 0) && (_count < MAX_CHAR))
				{
					progress_label.label.text += ".";
					++_count;
				}
				else
				{
					progress_label.label.text = string.Empty;
					_count = 0;
				}
			}
		}

		public void BeginLoading()
		{
			_isLoading = true;
		}

		public void EndLoading()
		{
			_isLoading = false;
			SubmitResponse((int)DialogResponse.OK);
		}
	}
}