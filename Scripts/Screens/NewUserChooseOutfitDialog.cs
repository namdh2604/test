using iGUI;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using Voltage.Witches.Models;
using Voltage.Witches.Events;

namespace Voltage.Witches.Screens
{
	public class NewUserChooseOutfitDialog : AbstractDialog 
	{
		[HideInInspector]
		public iGUIImage glow_preppy,glow_funky,glow_rebel;

		[HideInInspector]
		public iGUIButton button_rebel,button_funky,button_preppy;
		
		public GUIEventHandler OutfitSelected;
		
		//TODO Add in name validation stuff??

		IEnumerator AnimateAndClose(ChosenOutfitResponse outfit)
		{
			iGUIElement glowingBit = null;
//			iGUIElement selected = null;
			switch(outfit)
			{
				case ChosenOutfitResponse.FUNKY:
					glowingBit = glow_funky;
//					selected = button_funky;
					break;
				case ChosenOutfitResponse.PREPPY:
					glowingBit = glow_preppy;
//					selected = button_preppy;
					break;
				case ChosenOutfitResponse.REBEL:
					glowingBit = glow_rebel;
//					selected = button_rebel;
					break;
			}
			glowingBit.setEnabled(true);
			glowingBit.fadeTo(0f, 0.25f, 0.5f);
			glowingBit.scaleTo(0f,3f,0f,0.5f,iTweeniGUI.EaseType.easeOutElastic);
			yield return new WaitForSeconds(0.5f);
			SubmitResponse((int)outfit);
		}

		public void button_rebel_Click(iGUIButton sender)
		{
			if(OutfitSelected != null)
			{
				OutfitSelected(this, new GUIEventArgs());
			}
//			SubmitResponse((int)ChosenOutfitResponse.REBEL);
			StartCoroutine(AnimateAndClose(ChosenOutfitResponse.REBEL));
		}

		public void button_funky_Click(iGUIButton sender)
		{
			if(OutfitSelected != null)
			{
				OutfitSelected(this, new GUIEventArgs());
			}
//			SubmitResponse((int)ChosenOutfitResponse.FUNKY);
			StartCoroutine(AnimateAndClose(ChosenOutfitResponse.FUNKY));			
		}

		public void button_preppy_Click(iGUIButton sender)
		{
			if(OutfitSelected != null)
			{
				OutfitSelected(this, new GUIEventArgs());
			}
//			SubmitResponse((int)ChosenOutfitResponse.PREPPY);
			StartCoroutine(AnimateAndClose(ChosenOutfitResponse.PREPPY));			
		}
	}

	public enum ChosenOutfitResponse
	{
		PREPPY = 0,
		FUNKY = 1,
		REBEL = 2
	}
}