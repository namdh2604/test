
using System;
using System.Collections;
using System.Collections.Generic;

namespace Voltage.Witches.Layout
{
	using Voltage.Story.Views;
	using Voltage.Story.Models.Data;

	using Voltage.Common.Logging;

	public class StoryPlayerInputLayoutDisplay : ILayoutDisplay
    {
		private readonly ILayoutDisplay _layoutDisplay;

		private readonly Func<bool> _enableInput;				
		private bool _displayInitialized;					
		public event Action<int> OnInputProcessed;			// now called after EVERY input	


		public event Action OnSelectionDisplay;				// HACK: WitchesStoryPlayerScreenController doesn't know when a selection node is called, but needs to be partially passive (UI disabled, but Play is possible)
		public event Action OnSelectionExit;				// HACK: WitchesStoryPlayerScreenController doesn't know when a selection node is called, but needs to be partially passive (UI disabled, but Play is possible)



		public void HideSpeechBox()
		{
			if (_layoutDisplay != null) 
			{
				_layoutDisplay.HideSpeechBox ();
			}
		}

		public StoryPlayerInputLayoutDisplay(ILayoutDisplay layoutDisplay, Func<bool> inputClause)
		{
			if(layoutDisplay == null || inputClause == null)
			{
				throw new ArgumentNullException();
			}

			_layoutDisplay = layoutDisplay;
			_enableInput = inputClause;
			_displayInitialized = false;
		}


		public IEnumerator PreloadDependencies(IEnumerable<string> resources)
		{
			return _layoutDisplay.PreloadDependencies(resources);
		}


		public void DisplayDialogue(DialogueNodeViewData node, Action<List<string>> readyCallback, Func<int,bool> inputCallback)
		{
			Action<List<string>> onReadyCallback = CreateOnReadyCallback (readyCallback);
			Func<int,bool> inputHandler = (response => InputHandler (response, inputCallback));

			_layoutDisplay.DisplayDialogue (node, onReadyCallback, inputHandler);
		}


		public void DisplayEI(EventIllustrationNodeViewData node, Action<List<string>> readyCallback, Func<int,bool> inputCallback)
		{
			Action<List<string>> onReadyCallback = CreateOnReadyCallback (readyCallback);
			Func<int,bool> inputHandler = (response => InputHandler (response, inputCallback));

			_layoutDisplay.DisplayEI (node, onReadyCallback, inputHandler);
		}


		public void DisplaySelection(SelectionNodeViewData node, Action<List<string>> readyCallback, Func<int,bool> inputCallback)
		{
			Action<List<string>> onReadyCallback = CreateOnReadyCallback (readyCallback);

			Func<int,bool> inputHandler = (response => 
			{
				bool value = InputHandler (response, inputCallback);

				if(OnSelectionExit != null)		// HACK: WitchesStoryPlayerScreenController doesn't know when a selection node is called, but needs to be partially passive (UI disabled, but Play is possible)
				{
					OnSelectionExit();
				}

				return value;
			});

			if(OnSelectionDisplay != null)		// HACK: WitchesStoryPlayerScreenController doesn't know when a selection node is called, but needs to be partially passive (UI disabled, but Play is possible)
			{
				OnSelectionDisplay();
			}

			_layoutDisplay.DisplaySelection (node, onReadyCallback, inputHandler);
		}


		private bool AllowInput()
		{
			return _enableInput() && _displayInitialized;
		}


		private bool InputHandler(int response, Func<int,bool> inputCallback)
		{
			bool processed = false;		// FIXME...i believe this was intended to mean "processed", but in this context it could be confused with meaning whether input is handled or not

			if(AllowInput())
			{
				processed = inputCallback(response);
			}
			else
			{
				AmbientLogger.Current.Log("StoryPlayerInputLayoutDisplay::InputHandler >>> storyplayer input disabled", LogLevel.INFO);
			}

			if(OnInputProcessed != null)			// bit of a HACK, was cleaner when associated with _enableInput()
			{
				OnInputProcessed(response);
			}

			return processed;
		}


		private Action<List<string>> CreateOnReadyCallback(Action<List<string>> callback)
		{
			_displayInitialized = false;

			return (list) =>
			{
				if(callback != null)
				{
					callback(list);
				}

				_displayInitialized = true;
			};
		}


    }
    
}




