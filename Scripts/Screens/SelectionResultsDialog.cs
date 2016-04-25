using iGUI;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using Voltage.Witches.Models;

namespace Voltage.Witches.Screens
{
	public class SelectionResultsDialog : AbstractDialog 
	{
		[HideInInspector]
		public iGUIContainer favorability_1,favorability_2,favorability_3,favorability_4,favorability_5,
							 favorability_display;

		[HideInInspector]
		public iGUIImage favorability_icon_1,favorability_icon_2,favorability_icon_3,favorability_icon_4,
						 favorability_icon_5,icon_placeholder;

		[HideInInspector]
		public iGUILabel favorability_counter;

		public KeyValuePair<string,int> Change { get; protected set; }
		private static Dictionary<string,iGUIImage> effectLabelMap;

		public void SetDisplay(KeyValuePair<string,int> change)
		{
			Change = change;
			//TODO Determine what params are to be passed in
		}

		protected void Start()
		{
			effectLabelMap = new Dictionary<string,iGUIImage>()
			{
				{"N",favorability_icon_1},
				{"M",favorability_icon_2},
				{"R",favorability_icon_3},
				{"A",favorability_icon_4},
				{"T",favorability_icon_5}
			};

			StartCoroutine(DisplayChange());
		}

		IEnumerator DisplayChange()
		{
			Texture2D icon = (Texture2D)effectLabelMap[Change.Key].image;
			icon_placeholder.image = icon;
			string prefix = "+";
			if(Change.Value < 0)
			{
				prefix = "-";
			}
			favorability_counter.label.text = prefix + Change.Value.ToString();

			favorability_display.fadeTo(1f,0f);
			favorability_display.scaleTo(2.5f,0.3f,iTweeniGUI.EaseType.easeInOutBounce);
			yield return new WaitForSeconds(0.3f);

			favorability_counter.basicOutline = true;
			favorability_display.scaleTo(1f,0.35f,iTweeniGUI.EaseType.easeInBounce);
			yield return new WaitForSeconds(0.35f);

			SubmitResponse((int)DialogResponse.OK);
		}
	}
}