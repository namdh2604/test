using iGUI;
using UnityEngine;
using System.Collections;

namespace Voltage.Witches.Screens
{
	public class RestoreDataSuccessDialog : AbstractDialog 
	{
		[HideInInspector]
		public iGUIImage ok_text;

		protected void Start()
		{
			ok_text.passive = true;
		}

		public void btn_ok_Click(iGUIButton sender)
		{
			SubmitResponse((int)DialogResponse.OK);
		}
	}
}