using System;
using UnityEngine;

namespace Voltage.Witches.Models
{
	public interface IPlayerPreferences
	{
		bool SoundEnabled { get; set; }
		bool SFXEnabled { get; set; }
		bool HasValues();
		void SetDefaults();
		void SetBGMToggledCallback(Action<bool> bgmCallback);
		void SetSFXToggledCallback(Action<bool> sfxCallback);
	}

	public class PlayerPreferences : IPlayerPreferences
	{
		private const string PREFS_NAMESPACE = "Curses_";
		private const string SOUND_KEY = PREFS_NAMESPACE + "Sound";
		private const string SFX_KEY = PREFS_NAMESPACE + "SFX";
		private const string NOTIFICATION_KEY = PREFS_NAMESPACE + "Notification";

		private const int SOUND_DISABLED = 0;
		private const int SOUND_ENABLED = 1;

		private const int SFX_DISABLED = 0;
		private const int SFX_ENABLED = 1;

		private const int NOTIFICATION_DISABLED = 0;
		private const int NOTIFICATION_ENABLED = 1;

		private static Action<bool> _bgmCallback;
		private static Action<bool> _sfxCallback;

		private static PlayerPreferences _instance;

		public static PlayerPreferences GetInstance()
		{
			if (_instance == null)
			{
				_instance = new PlayerPreferences();
			}

			return _instance;
		}

        public void Reset()
        {
            PlayerPrefs.DeleteAll();
        }

		public bool HasValues()
		{
			return ((PlayerPrefs.HasKey(SOUND_KEY)) && (PlayerPrefs.HasKey(SFX_KEY)) && (PlayerPrefs.HasKey(NOTIFICATION_KEY)));
		}

		public void SetDefaults()
		{
			SetSFX(true);
			SetSound(true);
			SetNotifications(true);
		}

		public void SetBGMToggledCallback(Action<bool> bgmCallback)
		{
			_bgmCallback = bgmCallback;
		}

		public void SetSFXToggledCallback(Action<bool> sfxCallback)
		{
			_sfxCallback = sfxCallback;
		}

		public bool SoundEnabled
		{
			get { return IsSoundEnabled(); }
			set { SetSound(value); }
		}

		private bool IsSoundEnabled()
		{
			int soundNum = PlayerPrefs.GetInt(SOUND_KEY);
			return Convert.ToBoolean(soundNum);
		}

		private void SetSound(bool enabled)
		{
			Debug.Log("Setting Sound to: " + enabled);
			int soundNum = Convert.ToInt32(enabled);
			PlayerPrefs.SetInt(SOUND_KEY, soundNum);
			if(_bgmCallback != null)
			{
				_bgmCallback(IsSoundEnabled());
			}
		}

		public bool SFXEnabled
		{
			get { return AreSFXEnabled(); }
			set { SetSFX(value); }
		}

		private bool AreSFXEnabled()
		{
			int sfxNum = PlayerPrefs.GetInt(SFX_KEY);
			return Convert.ToBoolean(sfxNum);
		}

		private void SetSFX(bool enabled)
		{
			Debug.Log("Setting SFX to: " + enabled);
			int sfxNum = Convert.ToInt32(enabled);
			PlayerPrefs.SetInt(SFX_KEY, sfxNum);
			if(_sfxCallback != null)
			{
				_sfxCallback(AreSFXEnabled());
			}
		}

		public bool NotificationsEnabled
		{
			get { return AreNotificationsEnabled(); }
			set { SetNotifications(value); }
		}
		
		private bool AreNotificationsEnabled()
		{
			int notNum = PlayerPrefs.GetInt(NOTIFICATION_KEY);
			return Convert.ToBoolean(notNum);
		}
		
		private void SetNotifications(bool enabled)
		{
			Debug.Log("Setting Notifications to: " + enabled);
			int notNum = Convert.ToInt32(enabled);
			PlayerPrefs.SetInt(NOTIFICATION_KEY, notNum);
		}
	}
}
