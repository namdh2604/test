using UnityEngine;
using System;
using System.Collections.Generic;

namespace Voltage.Witches.StoryMap
{
	using UnityEngine.UI;
//	using Voltage.Witches.UI;
	using Voltage.Story.Configurations;

	[RequireComponent(typeof(HorizontalLayoutGroup))]
	[RequireComponent(typeof(ArcButtonFactory))]
    public class ArcDisplayPanelView : MonoBehaviour
    {
		[SerializeField]
		private HorizontalLayoutGroup _horizontalLayoutController;		// or this.GetComponent<HorizontalLayoutGroup>()
		[SerializeField]
		private ArcButtonFactory _buttonFactory;

//		[SerializeField]
//		private CanvasGroup _canvasGrp;


		private void Awake()
		{
			if(_horizontalLayoutController == null || _buttonFactory == null)
			{
				throw new NullReferenceException();
			}

			_horizontalLayoutController.childForceExpandHeight = false;
			_horizontalLayoutController.childForceExpandWidth = false;
			_horizontalLayoutController.childAlignment = TextAnchor.MiddleCenter;
			_horizontalLayoutController.SetLayoutHorizontal ();
		}

		public IEnumerable<ArcButton> GetButtons()	// could cache buttons when added
		{
			IList<ArcButton> arcButtons = new List<ArcButton> ();		
			foreach(Transform button in this.transform)
			{
				arcButtons.Add (button.GetComponent<ArcButton>());		// gameObject.GetComponentsInChildren (typeof(ArcButton), true)
			}
			return arcButtons;
		}

		public bool HasButtons
		{
			get { return gameObject.transform.childCount > 0; }
		}


		public ArcButton Add (ArcData arc)
		{
			ArcButton button = _buttonFactory.Create (arc);

			button.transform.SetParent (this.transform);
			button.transform.localScale = Vector3.one;

			return button;
		}

        public void MakePassive(bool value)
        {
            foreach (var button in GetButtons())
            {
                button.MakePassive(value);
            }
        }
    }

}




