using UnityEngine;
using iGUI;
using System.Collections;
using System.Collections.Generic;
using Voltage.Witches.Events;
using Voltage.Witches.Models;

namespace Voltage.Witches.Views
{
	public class SettingsShellView : MonoBehaviour
	{
		[HideInInspector]
		public iGUIContainer menu_container,setting_button,skip_container,mailbox_filters;

		[HideInInspector]
		public iGUIButton menu_button,btn_setting,btn_skip_hitbox,btn_all,btn_read,btn_unread,btn_chara,btn_system;

		[HideInInspector]
		public iGUIImage btn_setting_art,btn_skip_art;

		public SettingsShellLayout Layout { get; protected set; }

		public event GUIEventHandler OnMenuPress;
		public event GUIEventHandler OnSettingsPress;
		public event GUIEventHandler OnSkipPress;

		public void SetLayout(SettingsShellLayout layout)
		{
			Layout = layout;
			var isStory = ((Layout == SettingsShellLayout.STORY) && (Layout != SettingsShellLayout.TUTORIAL));
			var isNotStory = (Layout == SettingsShellLayout.DEFAULT);
			var isMailbox = (Layout == SettingsShellLayout.MAIL);

			setting_button.setEnabled(isStory);
			skip_container.setEnabled(isStory);
			menu_container.setEnabled(isNotStory);
			mailbox_filters.setEnabled(isMailbox);
		}

		public void menu_button_Click(iGUIButton sender)
		{
			if(OnMenuPress != null)
			{
				OnMenuPress(this, new GUIEventArgs());
			}
		}

		public void btn_setting_Click(iGUIButton sender)
		{
			if(OnSettingsPress != null)
			{
				OnSettingsPress(this, new GUIEventArgs());
			}
		}

		public void btn_skip_hitbox_Click(iGUIButton sender)
		{
			if(OnSkipPress != null)
			{
				OnSkipPress(this, new GUIEventArgs());
			}
		}
	}

	public enum SettingsShellLayout
	{
		DEFAULT = 0,
		STORY = 1,
		MAIL = 2,
		TUTORIAL = 3
	}
}