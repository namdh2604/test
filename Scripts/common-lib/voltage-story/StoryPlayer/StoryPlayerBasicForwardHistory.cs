using System;
using System.Collections.Generic;
using Voltage.Common.Logging;
using Voltage.Story.Models.Nodes;
using Voltage.Story.Models.Nodes.Controllers;

namespace Voltage.Story.StoryPlayer
{
    using Voltage.Story.User;

	public class StoryPlayerBasicForwardHistory : StoryPlayerBasic, IHaveHistory
	{
		protected Stack<INode> HistoryAhead = new Stack<INode>();
		public bool InHistoryMode { get { return HistoryAhead != null && HistoryAhead.Count > 0; }	}

		public StoryPlayerBasicForwardHistory (ILogger logger, IDictionary<Type,INodeController> nodeControllers, Action onFinish=null) : base (logger, nodeControllers, onFinish)  {}

		public override void Next ()
		{
			if(InHistoryMode)
			{
				AddNodeToHistory(CurrentNode);
				Process(HistoryAhead.Pop());
			}
			else
			{
				Logger.Log ("no history ahead", LogLevel.INFO);
				base.Next ();
			}
		}

		public override void Previous ()
		{	
			HistoryAhead.Push(CurrentNode);
			base.Previous();
		}


		protected override void DebugHistory ()
		{
			if (HistoryAhead != null)
			{
				string debug = string.Format("HistoryAhead Count: {0}\n", HistoryAhead.Count);
				foreach(INode history in HistoryAhead)
				{
					debug += string.Format("\n{0}: {1}", history.ID, history.GetType().ToString());
				}
				Logger.Log (debug, LogLevel.INFO);
			}
		}
	}
}

