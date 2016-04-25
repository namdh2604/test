using System;
using System.Collections.Generic;
using Voltage.Common.Utilities.Extensions;
using Voltage.Story.Models.Nodes;

namespace Voltage.Story.StoryDivisions.Search
{

	public static class StorySearch
	{

		private enum Direction		// TODO: Drop Direction Enum, make Forward/Back explicit calls
		{
			FORWARD,		// (node) => (node.Next)
			BACKWARD		// (node) => (node.Previous)
		}

		public static void PreorderTraverseForward(INode start, Predicate<INode> condition, Action<INode> action, bool firstOccurence=false, Func<INode,bool> breakCondition=null)
		{
			PreorderTraverse(start, condition, action, Direction.FORWARD, firstOccurence, breakCondition);
		}

		public static void PreorderTraverseBackward(INode start, Predicate<INode> condition, Action<INode> action, bool firstOccurence=false, Func<INode,bool> breakCondition=null)
		{
			PreorderTraverse(start, condition, action, Direction.BACKWARD, firstOccurence, breakCondition);
		}

		private static void PreorderTraverse(INode start, Predicate<INode> condition, Action<INode> action, Direction direction=Direction.FORWARD, bool firstOccurence=true, Func<INode,bool> breakCondition=null)
		{
			INode node = start;	// for exclusive, use 'start.Next'

			while (node != null)
			{
				if(breakCondition != null && breakCondition(node))
				{
					break;
				}

				if(condition != null && condition(node))
				{
					if(action != null)
					{
//						try
						{
							action(node);
						}
//						catch (InvalidCastException e)
//						{
//							Console.WriteLine(e);
//						}
					}

					if(firstOccurence)
					{
						break;
					}
				}

				if(direction == Direction.BACKWARD)		// TODO: Separate logic for Forward/Back into its own calls
				{
					node = node.Previous;
				}
				else
				{
					if(node is IBranchable<INode>)
					{
						foreach(INode subNode in ((IBranchable<INode>)node).Branches)
						{
							PreorderTraverse(subNode, condition, action, direction, firstOccurence);
						}
					}

					node = node.Next;
				}
			}
		}

        public static Dictionary<INode, INode> GetAllBranchContinuationPoints(INode startNode, INode parentNext=null)
        {
            Dictionary<INode, INode> branchDic = new Dictionary<INode, INode>();
            INode currentNode = startNode;

            while (currentNode != null)
            {
                if (currentNode is IBranchable<INode>)
                {
                    IBranchable<INode> branchNode = currentNode as IBranchable<INode>;
                    INode nextNode = (currentNode.Next == null) ? parentNext : currentNode.Next;

                    foreach (INode childNode in branchNode.Branches)
                    {
                        var childContinuationPoints = GetAllBranchContinuationPoints(childNode, nextNode);
                        branchDic.AddRange<KeyValuePair<INode, INode>>(childContinuationPoints);
                    }
                }

                if ((currentNode.Next == null) && (parentNext != null))
                {
                    branchDic.Add(currentNode, parentNext);
                }

                currentNode = currentNode.Next;
            }

            return branchDic;
        }
		
		public static IList<int> GetChoicesForPath(IEnumerable<INode> path)		
		{
			throw new NotImplementedException ();
		}

		// TODO: playing around with another search algorithm, maybe standardize
		public static IList<INode> GetPathFrom(INode startNode, Predicate<INode> goalNodePredicate)
		{
			INode goalNode = null;
			Dictionary<INode,INode> visitedNodeDic = GetSearchedNodes (startNode, goalNodePredicate, ref goalNode);

			return GetDirectPathFromVisited (startNode, goalNode, visitedNodeDic);
		}

		private static Dictionary<INode,INode> GetSearchedNodes (INode startNode, Predicate<INode> goalNodePredicate, ref INode goalNode)
		{
			Dictionary<INode,INode> visitedNodeDic = new Dictionary<INode,INode> { {startNode,null} };

			if(startNode != null && goalNodePredicate != null)
			{
				Queue<INode> frontier = new Queue<INode> ();
				frontier.Enqueue (startNode);

				while(frontier.Count > 0)
				{
					INode current = frontier.Dequeue();
					
					if(goalNodePredicate(current))
					{
						goalNode = current;
						break;
					}
					
					foreach (INode nextNode in GetBranchesFor(current)) 
					{
						if(!visitedNodeDic.ContainsKey(nextNode))
						{
							frontier.Enqueue(nextNode);
							visitedNodeDic.Add(nextNode, current);	// TODO: need to also include visitedNodeDic[startNode] = ???
						}
					}
				}
			}

			return visitedNodeDic;
		}


		private static IEnumerable<INode> GetBranchesFor(INode node)
		{
			List<INode> branchList = new List<INode> ();
			
			if(node != null)
			{
				if( node.Next != null)
				{
					branchList.Add (node.Next);
				}
				
				IBranchable<INode> branchNode = node as IBranchable<INode>;
				if(branchNode != null)
				{
					branchList.AddRange(branchNode.Branches);
				}
			}
			
			return branchList;
		}

		private static IList<INode> GetDirectPathFromVisited(INode startNode, INode goalNode, Dictionary<INode,INode> visitedNodeDic)
		{
			List<INode> path = new List<INode> ();

			if(startNode != null && goalNode != null && visitedNodeDic != null)
			{
				INode current = goalNode;
				path.Add (current);
				
				while (current != startNode)
				{
					current = visitedNodeDic[current];
					path.Add(current);
				}
				
				path.Reverse();

//				DebugPath(path);
			}
			else
			{
				Console.WriteLine("goal node is null");
			}
			
			
			return path;
		}

		private static void DebugPath(IEnumerable<INode> path)
		{
			string nodeList = string.Empty;
			foreach(INode node in path)
			{
				nodeList += string.Format("{0}[{1}],\n", node.GetType().ToString(), node.ID);
			}
			Console.WriteLine (nodeList);
		}
	}
}
