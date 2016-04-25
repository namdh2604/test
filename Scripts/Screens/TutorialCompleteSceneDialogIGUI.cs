using UnityEngine;
using System.Collections;

//HACK needs to be replaced with TutorialCOmpleteSceneDialog (UGUI)
using iGUI;


namespace Voltage.Witches.Screens
{
	public class TutorialCompleteSceneDialogIGUI : AbstractDialog
	{
		[HideInInspector]
		public iGUIButton screen_btn;
        public iGUIButton btn_taptocontinue;
		[HideInInspector]
		public iGUIContainer tap_continue_container;

		private TapToContinue _tapToContinue;

		public void Start()
		{
			_tapToContinue = gameObject.AddComponent <TapToContinue>() as TapToContinue;
			_tapToContinue.tapToContinue = tap_continue_container;
		}

        public void btn_taptocontinue_Click(iGUIButton sender)
		{
            Debug.Log("TaptoContinue button click");
			_tapToContinue.KillPrompt ();
			SubmitResponse ((int)DialogResponse.OK);
		}
	}
}
