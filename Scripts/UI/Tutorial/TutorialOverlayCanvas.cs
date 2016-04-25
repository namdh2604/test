using UnityEngine;
using System;
using System.Collections.Generic;

namespace Voltage.Witches.Tutorial.uGUI
{
	using Voltage.Witches.Screens;

	public class TutorialOverlayCanvas : BaseUGUIScreen
    {
		[SerializeField]
		private TutorialScreenOverlay _screenOverlay;

		public TutorialScreenOverlay OverlayController { get { return _screenOverlay; } }

		private void Awake()
		{
			if(_screenOverlay == null)
			{
				throw new NullReferenceException();
			}

//			transform.SetParent (null);		// detach from parent canvas
		}

		private void Start()
		{
			// HACK: remove when ScreenFactory::GetOverlay is used
			ReParentScreenCanvas ();	
		}

		private void ReParentScreenCanvas()
		{
			GameObject uGUIRoot = GameObject.Find("UGUIScreenRoot");	// HACK: detaching from parent canvas here since uGUI factory / UGUIScreenRoot supports only a single canvas presently
			transform.SetParent(uGUIRoot.transform);				
			transform.localPosition = Vector3.zero;
			transform.localScale = Vector3.one;
		}



		private TutorialStoryMapScreenController _controller;
		
		public void Init(TutorialStoryMapScreenController controller)
		{
			_controller = controller;
		}
		
		protected override Voltage.Witches.Controllers.IScreenController GetController()
		{
			return _controller;
		}

    }

}


