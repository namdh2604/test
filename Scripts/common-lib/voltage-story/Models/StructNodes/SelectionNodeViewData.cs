using System.Collections.Generic;

namespace Voltage.Story.Models.Data
{
    public class SelectionNodeViewData
    {
        public IList<string> Options { get; set; }
		public DialogueNodeViewData DialogueNode { get; set; }

		public string Prompt { get; set; }

		public SelectionNodeViewData()
		{
			Options = new List<string>();
			DialogueNode = new DialogueNodeViewData();
		}
    }

}