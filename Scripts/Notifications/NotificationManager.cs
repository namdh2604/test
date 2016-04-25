#pragma warning disable 0414
using System;
using System.Collections.Generic;
using UTNotifications;
using UnityEngine;

namespace Voltage.Witches.Notifications
{
    using Voltage.Witches.Models;
	using Voltage.Common.PushNotification;

    public class NotificationManager : MonoBehaviour
    {
        [Serializable]
        public class NotificationEntry
        {
            public string Title;
            public string Message;
        }

        [Serializable]
        public class TimedNotificationEntry : NotificationEntry
        {
            public int Hours;
        }

        public List<TimedNotificationEntry> _notifications;
        public NotificationEntry _staminaNotification;
        public NotificationEntry _focusNotification;

        private readonly Dictionary<string, string> _userData = new Dictionary<string, string>() {
            { "applicationIconBadgeNumber", "1" }
        };

        private const int STARTING_MESSAGE_ID = 0;

        private Player _player;
        private bool _isInit = false;

		// NOTE: This is not embedded into this class/game object because Parse actually destroys and recreates its own game object, which owns the configuration script.
		// It may be safe to transfer the initialize portion over to here, but I'm not sure how Parse's internals work yet.
		public PushBehaviour _parseManager;

        public void Init(Player player)
        {
            _player = player;
			_player.OnNotificationEnable += HandleNotificationEnable;

			Setup();

            _isInit = true;
        }

		private void Setup()
		{
			if (_player.NotificationsEnabled)
			{
                // if this is the first time that initialize has been called on iOS,
                // it will prompt the user whether or not they will accept notifications
                // NOTE: once that device has explicit settings, even if you uninstall the application,
                // it will take a day to prompt you again.  If you cannot wait the day, you can move your clock forward a day
                // http://stackoverflow.com/questions/2438400/reset-push-notification-settings-for-app
				UTNotifications.Manager.Instance.Initialize(false);
				UTNotifications.Manager.Instance.CancelAllNotifications();
				UTNotifications.Manager.Instance.SetBadge(0);

				_parseManager.EnableParse();
			}
		}

		private void HandleNotificationEnable(object sender, EventArgs args)
		{
			Setup();
		}

        public void ScheduleReminders()
        {
			if (_player.NotificationsEnabled)
			{
	            CancelReminders();

	            int staminaSeconds = _player.GetSecondsUntilStaminaMaxed();
	            if (staminaSeconds > 0)
	            {
	                ScheduleNotification(staminaSeconds, _staminaNotification, STARTING_MESSAGE_ID);
	            }

	            int focusSeconds = _player.GetSecondsUntilFocusMaxed();
	            if (focusSeconds > 0)
	            {
	                ScheduleNotification(focusSeconds, _focusNotification, STARTING_MESSAGE_ID + 1);
	            }

	            for (int i = 0; i < _notifications.Count; ++i)
	            {
	                ScheduleNotification(HoursToSeconds(_notifications[i].Hours), _notifications[i], STARTING_MESSAGE_ID + 2 + i);
	            }
			}
        }

        private const string DEFAULT_TITLE = "Kisses & Curses";
        private void ScheduleNotification(int seconds, NotificationEntry notification, int id)
        {
            string title = (string.IsNullOrEmpty(notification.Title)) ? DEFAULT_TITLE : notification.Title;
            UTNotifications.Manager.Instance.ScheduleNotification(seconds, title, notification.Message, id, _userData);
        }

        private int HoursToSeconds(int hours)
        {
            return hours * 3600;
        }

        public void CancelReminders()
        {
			if (_player.NotificationsEnabled)
			{
	            UTNotifications.Manager.Instance.CancelAllNotifications();
			}
        }

        #region UnityCallbacks
        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }

        private void OnApplicationQuit()
        {
            if (_isInit)
            {
                ScheduleReminders();
            }
        }

        private void OnApplicationPause(bool isPaused)
        {
            if (isPaused && _isInit)
            {
                ScheduleReminders();
            }
        }

        private void OnApplicationFocus(bool isFocused)
        {
            if (isFocused && _isInit)
            {
                CancelReminders();
            }
        }
        #endregion
    }
}

