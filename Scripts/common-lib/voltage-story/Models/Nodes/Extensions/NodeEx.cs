using System;
using System.Collections.Generic;

using Voltage.Story.StoryDivisions;
using Voltage.Story.StoryDivisions.Search;

namespace Voltage.Story.Models.Nodes.Extensions
{
	public static class NodeEx
	{
		public static T FindNode<T>(this INode startNode, Predicate<INode> condition=null, Func<INode,bool> breakCondition=null) where T : INode 	// TODO: maybe don't need <T>
		{
			INode target = null;
			StorySearch.PreorderTraverseForward (startNode, (node) => node is T && (condition != null ? condition(node) : true), (node) => target = node, true, breakCondition);
			return (T)target;
		}
		
		public static List<T> FindAllNodes<T>(this INode startNode, Predicate<INode> condition=null) where T : INode	// TODO: maybe don't need <T>, return IList<T> ???
		{
			List<T> allNodes = new List<T> ();
			StorySearch.PreorderTraverseForward (startNode, (node) => node is T && (condition != null ? condition(node) : true), (node) => allNodes.Add((T)node), false, null);
			return allNodes;
		}
		
		
		public static void PerformAction (this INode startNode, Action<INode> action, Func<INode,bool> breakCondition=null)
		{
			StorySearch.PreorderTraverseForward (startNode, (node) => true, action, false, breakCondition);
		}
		
		
		public static T FindNodeBefore<T>(this INode startNode, Predicate<INode> condition=null, Func<INode,bool> breakCondition=null) where T : INode	// TODO: maybe don't need <T>
		{
			INode target = null;
			StorySearch.PreorderTraverseBackward(startNode, (node) => node is T && (condition != null ? condition(node) : true), (node) => target = node, true, breakCondition);
			return (T)target;
		}

		// is this guaranteed to follow story node sequence?
        public static INode FindNodeBefore(this INode startNode, Predicate<INode> condition=null)
        {
            INode target = null;
            StorySearch.PreorderTraverseBackward(startNode, condition, (node) => target = node, true, null);

            return target;
        }
		
		public static List<T> FindAllNodesBefore<T>(this INode startNode, Predicate<INode> condition=null) where T : INode	// TODO: maybe don't need <T>, return IList<T> ???
		{
			List<T> allNodes = new List<T> ();
			StorySearch.PreorderTraverseBackward (startNode, (node) => node is T && (condition != null ? condition(node) : true), (node) => allNodes.Add((T)node), false, null);
			return allNodes;	
		}

        // is this guaranteed to follow story node sequence?
        public static INode FindNodeAfter(this INode startNode, Predicate<INode> condition=null)
        {
            INode target = null;
            StorySearch.PreorderTraverseForward(startNode, condition, (node) => target = node, true, null);

            return target;
        }





		public static string DebugNodeListToString (this IEnumerable<INode> nodelist)
		{
			string output = string.Empty;
			foreach(INode node in nodelist)
			{
				output += string.Format("\n{0}", node!=null ? node.ID : "null");
			}
			return output;
		}

		// TODO: This should be handled differently -- override the .toString() method for INodes
		public static string DebugSceneAllChildrenNodesToString (this Scene scene)	// NOTE: maybe allow any INode to do this?
		{
			string listAll = string.Empty;
			
			if(scene != null)
			{
				scene.PerformAction ((node) => 
				{
					if(node is IHaveText) 
						listAll += string.Format("\n{0}: {1}:{2}", node.ID, node.GetType(), ((IHaveText)node).Text);
					else if(node is IHavePrompt)
						listAll += string.Format("\n{0}: {1}:{2}", node.ID, node.GetType(), ((IHavePrompt)node).Prompt);
					else
						listAll += string.Format("\n{0}: {1}", node.ID, node.GetType());
				});
			}
			
			return listAll;
		}



	}
}
