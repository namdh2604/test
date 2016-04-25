using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

using TMPro;

using Voltage.Common.Utilities;
using Voltage.Witches.Utilities;

//using Voltage.Story.Models.StructNodes;
using Voltage.Story.Models.Nodes;

namespace Voltage.Witches.Layout
{
	using Voltage.Story.Models.Data;

	public class SelectionDisplay : MonoBehaviour
	{
		public List<string> _options;
		public GameObject _buttonPrefab;
		private Func<int,bool> _responseHandler;

		private Button.ButtonClickedEvent _pressed;			// is this being used?

		private bool _choiceWasMade;		

		void Start()
		{
			UpdateDisplay();
		}

		public void SetInputHandler(Func<int,bool> responseCallback)
		{
			_responseHandler = responseCallback;
		}

		public IEnumerator Display(SelectionNodeViewData node, Action<List<string>> callback = null)
		{
			List<string> warnings = new List<string>();

			_options = new List<string> (node.Options);


			UpdateDisplay();

			if (callback != null)
			{
				callback(warnings);
			}

			yield break;
		}

		public void ResetDisplay()
		{
			GameObjectUtils.RemoveChildren(gameObject);
		}

		public void UpdateDisplay()
		{
			ResetDisplay();

			if (_buttonPrefab == null)
			{
				return;
			}

			_choiceWasMade = false;		// set to true when user presses a button (see delegate returned by CreateListener())

			for (int i = 0; i < _options.Count; ++i)
			{
				GameObject optionGO = PrefabHelper.Instantiate(_buttonPrefab) as GameObject;
				Button button = optionGO.GetComponent<Button>();
				TextMeshProUGUI text = optionGO.GetComponentInChildren<TextMeshProUGUI>();
				text.text = _options[i];
				button.onClick.AddListener(CreateListener(i));

				optionGO.transform.SetParent(transform, false);
			}
		}

		private UnityEngine.Events.UnityAction CreateListener(int i)
		{
//			return () => Debug.Log ("Button " + i + " was pressed!");
			return delegate {
				if (_responseHandler != null)
				{
					if(!_choiceWasMade)
					{
						Voltage.Common.Logging.AmbientLogger.Current.Log("Choice made: " + i, Voltage.Common.Logging.LogLevel.INFO);

						_responseHandler(i);
						_choiceWasMade = true;
					}
				}
			};
		}

	}

}