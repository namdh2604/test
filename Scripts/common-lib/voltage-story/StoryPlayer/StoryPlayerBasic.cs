using System;
using System.Collections.Generic;

using Voltage.Common.Logging;
using Voltage.Story.StoryDivisions;
using Voltage.Story.StoryDivisions.Search;
using Voltage.Story.Models.Nodes;
using Voltage.Story.Models.Nodes.Controllers;
using Voltage.Story.Models.Nodes.Extensions;

namespace Voltage.Story.StoryPlayer
{
//	using Voltage.Story.Utilities;	

	public class StoryPlayerBasic : StoryPlayerNoHistory
	{
		public Stack<INode> History { get; protected set; }									// NOTE: OptionNodes are not added to history

		// TODO: CHANGE NODECONTROLLER DEPENDENCY, DEPENDS ON INTEGRATION...AND DROP CALLBACK
		public StoryPlayerBasic(ILogger logger, IDictionary<Type,INodeController> nodeControllers, Action onFinish=null) : base (logger, nodeControllers, onFinish) 
		{
			History = new Stack<INode> ();
		}


		public bool StartScene (Scene scene, INode startNode, IEnumerable<INode> history)
		{
			SetupHistory (history);
			return StartScene (scene, startNode);
		}

		private void SetupHistory (IEnumerable<INode> history)
		{
			History = history != null ? new Stack<INode>(history) : new Stack<INode>();	// TODO: verify history???
		}


		public override bool StartScene (Scene scene, INode startNode)	// TEMP
		{
			if(History != null && ValidateHistory(scene, startNode, History))
			{
				return base.StartScene(scene, startNode);
			}
			else
			{
				Logger.Log ("invalid history", LogLevel.WARNING);
				return false;
			}
		}


		public override void Next()
		{
			AddNodeToHistory (CurrentNode);				// FIXME: Bit too fragile? moving this call could have side effects!!! has to be in Next (NOT anywhere Previous will access)
		
			base.Next ();
		}


		public override void Next(int index)
		{
			AddNodeToHistory (CurrentNode);	// FIXME: Bit too fragile? moving this call could have side effects!!! has to be in Next (NOT anywhere Previous will access)

			base.Next (index);
		}


		public override void Previous()	
		{
			if(History != null && History.Count > 0)
			{
				INode node = History.Pop();
				node = node != CurrentNode ? node : History.Pop();	// FIXME: last node issue, temporary fix

				Process (node);		
			}
			else
			{
				Logger.Log ("no history to go back to", LogLevel.WARNING);
			}
		}

		protected void AddNodeToHistory(INode node)
		{
			if(node != null && History != null && !History.Contains(node))
			{	
				Logger.Log ("adding to history: " + node.GetType().ToString(), LogLevel.INFO);
				History.Push(node);		
			}
		}
	



		// FIXME: REPLACE VALIDATE HISTORY!!!! MOVE OUT TO STORY UTIL CLASS
		protected virtual bool ValidateHistory(Scene scene, INode startNode, Stack<INode> history)	// FIXME: BARE MINIMUM...CLEANUP and add more thorough checks
		{	// TODO: this method should be MOVED to StoryUtils.cs
			if (startNode != scene && history.Count == 0)	
			{
				Logger.Log ("History Validated: False(A)", LogLevel.WARNING);
				return false;
			}
			
			if(history.Count > 0)
			{
				INode lastHistoryNode = history.Peek ();
//				Console.WriteLine(string.Format("\n\nlastHistoryNode: {0}, lastHistoryNode.Next: {1}, startNode: {2}", lastHistoryNode.ID, lastHistoryNode.Next!=null?lastHistoryNode.Next.ID:"null", startNode.ID));
				
				if(lastHistoryNode.Next == null)
				{
//					Console.WriteLine("_branchDic > " + _branchDic[lastHistoryNode].ID);
					if(_branchDic[lastHistoryNode] != startNode)
					{
						Logger.Log ("History Validated: False(B)", LogLevel.WARNING);
						return false;
					}
				}
				else if (!(lastHistoryNode is IBranchable<INode>) && lastHistoryNode.Next != startNode)		// NOTE: can still be possible if it follows a nonrecorded node (such as an OptionNode)
				{
					Logger.Log ("History Validated: False(C)", LogLevel.WARNING);
					return false;
				}
				
			}
			
			Logger.Log ("History Validated: True", LogLevel.INFO);
			return true;
		}



		protected virtual void DebugHistory()
		{
			Logger.Log (string.Format ("Current Node: {0} [{1}]", CurrentNode.GetType().ToString(), CurrentNode.ID), LogLevel.INFO);
			
			if(History != null)
			{
				string history = string.Format("History Count: {0}\n", History.Count);
				foreach(INode node in History)
				{
					history += string.Format("\n{0}: {1}", node.ID, node.GetType().ToString());
				}
				
				Logger.Log (history, LogLevel.INFO);
			}

			if (_branchDic != null)
			{
				string branches = string.Format("Branch Count: {0}\n", _branchDic.Count);
				foreach (KeyValuePair<INode,INode> kvp in _branchDic)
				{
					branches += string.Format ("\n{0} : {1}", kvp.Key.ID, kvp.Value.ID);
				}
				Logger.Log (branches, LogLevel.INFO);
			}
		}
	}
}
