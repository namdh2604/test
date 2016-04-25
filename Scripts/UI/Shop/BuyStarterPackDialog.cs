using System;
using System.Collections.Generic;
using System.Collections;

namespace Voltage.Witches.Shop
{
	using UnityEngine;
	using UnityEngine.UI;

	using Voltage.Witches.Screens;

	using TMPro;

	public class BuyStarterPackDialog : BaseUGUIScreen	// : AbstractDialogUGUI
	{

		[SerializeField]
		public Button _buyButton;

		[SerializeField]
		public Button _closeButton;

		[SerializeField]
		private TextMeshProUGUI _timeLabel;


		private void Awake()
		{
			// TODO: guard clauses
		}


		public void Init(DateTime endTime)
		{
            StartTimer(endTime);
		}
    

		public void Display(Action<int> responseHandler)
		{
			SubscribeButtons (responseHandler);
		}


		private void SubscribeButtons(Action<int> responseHandler)		
		{
			Action<ButtonType> onClick = (choice) => 
			{
				if(responseHandler != null) 
				{
					responseHandler((int)choice);
				}
				Dispose ();
			};

			_closeButton.onClick.AddListener(() => onClick(ButtonType.CLOSE));
			_buyButton.onClick.AddListener(() => onClick(ButtonType.BUY));
		}

		private void UnsubscribeButtons()
		{
			_closeButton.onClick.RemoveAllListeners();
			_buyButton.onClick.RemoveAllListeners();
		}


		public enum ButtonType
		{
			CLOSE = 0,
			BUY
		}


        public override void Dispose()
        {
            StopTimer();

            base.Dispose();
        }



        private IEnumerator _starterPackCountdownRoutine;

        private void StartTimer(DateTime endTime)
        {
            StopTimer();

            _starterPackCountdownRoutine = StarterPackCountdown(endTime);
            StartCoroutine(_starterPackCountdownRoutine);
        }

        private void StopTimer()
        {
            if (_starterPackCountdownRoutine != null)
            {
                StopCoroutine(_starterPackCountdownRoutine);
                _starterPackCountdownRoutine = null;
            }
        }

        private const float TIMER_REFRESH_RATE = 1f;
        private IEnumerator StarterPackCountdown(DateTime endTime)
        {
            while (true)
            {
                TimeSpan timeRemaining = endTime - DateTime.UtcNow;
                _timeLabel.text = GetTimerTime(timeRemaining);

                if (timeRemaining <= TimeSpan.Zero)
                {
                    break;
                }

                yield return new WaitForSeconds(TIMER_REFRESH_RATE);
            }
        }

        private const string DEFAULT_TIME = "00:00:00";
        private const string COUNTER_FORMAT = "{0:00}:{1:00}:{2:00}";
        private string GetTimerTime(TimeSpan timeRemaining)
        {
            string time = DEFAULT_TIME;
            if(timeRemaining > TimeSpan.Zero)
            {
                int minutesLeft = (int)(timeRemaining.TotalMinutes);
                double hours = System.Math.Floor(minutesLeft / 60D);
                double minutes = minutesLeft % 60;
                int seconds = timeRemaining.Seconds;

                time = string.Format("{0:00}:{1:00}:{2:00}", hours, minutes, seconds);
            }

            return time;
        }






		// DEPRECATED
		protected override Voltage.Witches.Controllers.IScreenController GetController()
		{
			return null;
		}
	}
}
