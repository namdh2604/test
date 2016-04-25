using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Voltage.Witches.Screens.Dialogues
{
	using Voltage.Witches.Screens;
	using Voltage.Common.Utils;
	using UnityEngine.UI;

	using Voltage.Common.Logging;
    using StoryPlayerController = Voltage.Witches.Controllers.WitchesStoryPlayerScreenController;


	// FIXME: breaks (unknown) invariant where dialogues don't persist
	public class TapToContinueDialogue : BaseUGUIScreen, ITimer  // : AbstractDialogUGUI, ITimer
    {
//		[SerializeField]
//		private string text = "Tap to continue"

		[SerializeField]
		private Image _prompt;

		public event Action OnTimeOut;

		private IEnumerator _promptRoutine;
        private bool _passiveEnabled = false;

        public void MakePassive(bool value)
        {
            _passiveEnabled = value;
            if (_passiveEnabled)
            {
                StopTimer();
            }
            else
            {
                StartTimer(StoryPlayerController.DELAY_TIME);
            }
        }

		private void Awake()
		{
			if(_prompt == null)
			{
				throw new NullReferenceException();
			}
		}

		public void StartTimer(float durationInSeconds)			
		{
            if (!_passiveEnabled)
            {
                StopTimer();

                _promptRoutine = PromptRoutine(durationInSeconds);
                StartCoroutine(_promptRoutine);
            }
		}


		private void OnTimeOutEvent()
		{
			if(OnTimeOut != null)
			{
				OnTimeOut();
			}
		}


		private IEnumerator PromptRoutine(float duration)
		{
			yield return new WaitForSeconds (duration);

			OnTimeOutEvent ();

			foreach(var op in ShowPrompt())
			{
				yield return op;
			}
		}


		private IEnumerable ShowPrompt()
		{
			ShowAboveAllDialogues ();

			_prompt.canvasRenderer.SetAlpha(0f);
			_prompt.rectTransform.localScale = Vector2.zero;
			_prompt.gameObject.SetActive (true);

			while(true)
			{
				float scaleFactor = 1f;
				float scaleInDuration = 1.5f;
				_prompt.CrossFadeAlpha(1f, scaleInDuration / 3f, true);
				iTween.ScaleTo(_prompt.gameObject, new Hashtable{{"scale", new Vector3(scaleFactor,scaleFactor,scaleFactor)}, {"time", scaleInDuration}, {"easetype", iTween.EaseType.easeOutElastic}}); 

				yield return new WaitForSeconds(scaleInDuration);

				float scaleOutDuration = 0.5f;
				_prompt.CrossFadeAlpha(0f, scaleOutDuration / 2f, true);
				iTween.ScaleTo(_prompt.gameObject, new Hashtable{ {"scale", Vector3.zero}, {"time", scaleOutDuration},  {"easetype", iTween.EaseType.easeOutElastic} });

				yield return new WaitForSeconds(scaleOutDuration);

				float pauseDuration = 0.5f;
				yield return new WaitForSeconds(pauseDuration);
			}
		}

		private void ShowAboveAllDialogues()
		{
			this.transform.SetAsLastSibling ();		// HACK: since this dialogue is created/exists at the start of the storyplayer, this ensures that its over all other dialogues!
		}


		private void HidePrompt()
		{
			_prompt.gameObject.SetActive (false);
			iTween.Stop (_prompt.gameObject);
		}


		public void StopTimer()
		{
			if(_promptRoutine != null)
			{
				StopCoroutine(_promptRoutine);
			}

			HidePrompt ();
		}







		private Voltage.Witches.Controllers.IScreenController _controller;

		public void Init(Voltage.Witches.Controllers.IScreenController controller)
		{
			_controller = controller;
		}
		
		protected override Voltage.Witches.Controllers.IScreenController GetController()
		{
			return _controller;
		}


    }

}







//		private void VerifyDuration(float givenDuration)
//		{
//			float animDuration = _scaleInDuration + _scaleOutDuration + _pauseDuration;
//
//			if(givenDuration < animDuration)
//			{
//				AmbientLogger.Current.Log (string.Format("TapToContinueDialogue: WARNING, given duration ({0}s) is shorter than animation duration ({1}s). Possibility of collision with the next cycle", givenDuration, animDuration), LogLevel.WARNING);
//			}
//		}


