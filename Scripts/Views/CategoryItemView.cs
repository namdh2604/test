using System.Collections.Generic;
using UnityEngine;
using iGUI;
using Voltage.Witches.Models;
using Voltage.Witches.Events;

namespace Voltage.Witches.Views
{
	using Debug = UnityEngine.Debug;

	public class CategoryItemView : MonoBehaviour
	{
		[HideInInspector]
		public iGUILabel item_name_text;
		
		[HideInInspector]
		public iGUIButton item_btn;
		
		public iGUIButton GetButton()
		{
			return item_btn;
		}
		
		public void item_btn_Click(iGUIButton sender)
		{
			Debug.Log("Button done been clicked");
		}
	}
}