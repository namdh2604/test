using System;
using System.Collections;
using System.Collections.Generic;

namespace Voltage.Story.Views
{
	using Voltage.Story.Models.Nodes;
	using Voltage.Story.Models.Data;

	public interface ILayoutDisplay
	{
		void HideSpeechBox ();
		IEnumerator PreloadDependencies(IEnumerable<string> resources);
		void DisplayDialogue(DialogueNodeViewData node, Action<List<string>> readyCallback, Func<int,bool> inputCallback);
        void DisplayEI(EventIllustrationNodeViewData node, Action<List<string>> readyCallback, Func<int,bool> inputCallback);
		void DisplaySelection(SelectionNodeViewData node, Action<List<string>> readyCallback, Func<int,bool> inputCallback);
	}
}

