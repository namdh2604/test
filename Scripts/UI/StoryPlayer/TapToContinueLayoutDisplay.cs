
using System;
using System.Collections;
using System.Collections.Generic;

namespace Voltage.Witches.Layout
{
	using Voltage.Story.Views;
	using Voltage.Story.Models.Data;

	using Voltage.Common.Utils;

	using StoryPlayerController = Voltage.Witches.Controllers.WitchesStoryPlayerScreenController;

	// FIXME: specifications for TapToContinue have been revised, being a an ILayoutDisplay is inappropriate....original behaviour was tied to node traversal!
	public class TapToContinueLayoutDisplay : ILayoutDisplay
    {
		private readonly ILayoutDisplay _layoutDisplay;
		private readonly ITimer _timer;

		private readonly float _waitDurationInSec;

//		public void SetDelay(float durationInSec)
//		{
//			_waitDurationInSec = durationInSec;
//		}

		public void HideSpeechBox()
		{
			if (_layoutDisplay != null) 
			{
				_layoutDisplay.HideSpeechBox ();
			}
		}

		private bool _promptEnabled;
		public void EnableTimedPrompt(bool value)
		{
			if(!value)
			{
				_timer.StopTimer();
			}

			_promptEnabled = value;
		}

		public TapToContinueLayoutDisplay(ILayoutDisplay layoutDisplay, ITimer timer, float waitDurationInSec=StoryPlayerController.DELAY_TIME)
		{
			if(layoutDisplay == null || timer == null)
			{
				throw new ArgumentNullException();
			}

			_layoutDisplay = layoutDisplay;

			_timer = timer;
			_waitDurationInSec = waitDurationInSec;

			EnableTimedPrompt (true);

//			(_layoutDisplay as StoryPlayerInputLayoutDisplay).OnInputProcessed += (i) => _timer.StopTimer();
		}


		public IEnumerator PreloadDependencies(IEnumerable<string> resources)
		{
			return _layoutDisplay.PreloadDependencies (resources);
		}


		public void DisplayDialogue(DialogueNodeViewData node, Action<List<string>> readyCallback, Func<int,bool> inputCallback)
		{
			Action<Func<int,bool>> action = (responseCallback) => _layoutDisplay.DisplayDialogue(node, readyCallback, responseCallback); 

			DisplayWithTimer (action, inputCallback);
		}


		public void DisplayEI(EventIllustrationNodeViewData node, Action<List<string>> readyCallback, Func<int,bool> inputCallback)
		{
			Action<Func<int,bool>> action = (responseCallback) => _layoutDisplay.DisplayEI(node, readyCallback, responseCallback); 
			
			DisplayWithTimer (action, inputCallback);
		}


		public void DisplaySelection(SelectionNodeViewData node, Action<List<string>> readyCallback, Func<int,bool> inputCallback)
		{
            _timer.StopTimer();
            _layoutDisplay.DisplaySelection(node, readyCallback, inputCallback); 
		}


		// FIXME: specifications for TapToContinue have been revised, being a an ILayoutDisplay is inappropriate....original behaviour was tied to node traversal!
		private void DisplayWithTimer(Action<Func<int,bool>> display, Func<int,bool> inputCallback)
		{
			Func<int,bool> inputHandler = (response => 
			{
                //_timer.StartTimer(_waitDurationInSec); // Reset the tap to continue.
                return inputCallback(response);
			});
			
			display (inputHandler);

			if(_promptEnabled)
			{
				_timer.StartTimer(_waitDurationInSec);
			}
		}

    }
    
}























