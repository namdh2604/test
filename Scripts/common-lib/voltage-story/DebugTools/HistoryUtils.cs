using System;
using System.Collections.Generic;
using System.Linq;

using Voltage.Story.StoryDivisions;
using Voltage.Story.StoryDivisions.Search;
using Voltage.Story.Models.Nodes;

namespace Voltage.Story.DebugTools
{
	public class HistoryUtils	// TODO: Note change in StoryPlayer now DOES NOT ignore option/branch nodes may affect this classes function !!!!
	{
		private static readonly HistoryUtils instance = new HistoryUtils();
		public static HistoryUtils Default { get { return instance; } }

		private HistoryUtils () {}	// msc: insert default code here
		static HistoryUtils () {}

		public static IList<INode> GenerateRandomHistoryFor (Scene scene, string goalNodeID)// IComparer<INode> sortBy=null)	// TODO: change IBranchable<INode> to SelectionNode ????
		{
			return GenerateHistoryFor (scene, goalNodeID);
		}

		        
		public static IList<INode> GenerateHistoryFor (Scene scene, string goalNodeID, IEnumerable<int> choiceList=null)	// FIXME: may be some issue with sub-branches and not obvious conflict with choice list when history includes choices
		{
			Stack<int> choiceStack = choiceList != null ? new Stack<int>(choiceList) : new Stack<int> ();
			IList<INode> path = new List<INode> ();

			Queue<INode> directPath = new Queue<INode>(StorySearch.GetPathFrom (scene, (node) => node.ID == goalNodeID));

			while(directPath != null && directPath.Count > 0)
			{
				INode node = directPath.Dequeue();
				path.Add (node);

				if(node is IBranchable<INode> && directPath.Count > 0)	// FIXME: ugly, but need to check to make sure there are nodes to Peek
				{	
					IBranchable<INode> branch = node as IBranchable<INode>;
					if(branch.Branches.Contains(directPath.Peek()))
					{
						directPath.Dequeue ();	// FIXME: this works, but rework history generation functions to account for path finder doing its job
					}
					else
					{
						path.Concat(GetBranch(node as IBranchable<INode>, choiceStack));
					}
				}
				
			}

			if(path.Count > 0)
			{
				path.RemoveAt (path.Count - 1);	// NOTE: includes the target node by default
			}

			return path;
		}

		private static IList<INode> GetBranch(IBranchable<INode> branch, Stack<int> choiceStack=null)
		{
			List<INode> nodeList = new List<INode> ();
			
			if(branch != null)
			{
				INode node = branch.Branches[GetChoiceIndex(branch, choiceStack)].Next;	// NOTE: Skips OptionNode
				while(node != null)
				{
					nodeList.Add(node);
					
					if(node is IBranchable<INode>)
					{
						nodeList.AddRange(GetBranch(node as IBranchable<INode>, choiceStack));
					}
					
					node = node.Next;
				}
			}

			return nodeList;
		}
		
		private static int GetChoiceIndex (IBranchable<INode> branch, Stack<int> choiceStack=null)
		{
			System.Random rand = new System.Random ();
			
			if (branch != null && branch.Branches != null)
			{
				if(choiceStack != null && choiceStack.Count > 0)
				{
					int index = choiceStack.Pop();
					if(index >= 0 && index < branch.Branches.Count)
					{
						return index;
					}
				}
				
				Console.WriteLine ("getting random choice");
				return rand.Next (0, branch.Branches.Count);
			}
			else
			{
				throw new NullReferenceException("no branches to get choice");
			}
		}
	}
}
