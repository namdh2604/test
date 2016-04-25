using System;
using System.Collections.Generic;

using Voltage.Common.Logging;
using Voltage.Story.StoryDivisions;
using Voltage.Story.Models.Nodes;
using Voltage.Story.Models.Nodes.Controllers;
using Voltage.Story.Models.Nodes.Extensions;

namespace Voltage.Story.StoryPlayer
{
    using Voltage.Story.User;

	public sealed class StoryPlayerHistoryIntID : StoryPlayerBasicForwardHistory
	{
		public string IDFormat { get; private set; }

		public StoryPlayerHistoryIntID(string idFormat, ILogger logger, IDictionary<Type,INodeController> nodeControllers, Action onFinish=null) : base (logger, nodeControllers, onFinish)  
		{
			IDFormat = idFormat;
		}

		public bool StartScene(Scene scene, int nodeID, IEnumerable<int> history)	// NOTE: maybe shouldn't make nodeID optional ???
		{
			if(scene != null)
			{
				return StartScene (scene, nodeID, GetEnumerableNodesByIDInOrder (scene, history));
			}
			else
			{
				Logger.Log ("no scene given", LogLevel.WARNING);
			}

			return false;
		}

		public bool StartScene(Scene scene, int nodeID, IEnumerable<INode> history)
		{
			if(scene != null)
			{
				INode startNode = GetStartNode (scene, nodeID);
				return StartScene (scene, startNode, history); 	// GoTo (scene, (node) => Convert.ToInt32 (node.ID) == nodeID, history);
			}
			else
			{
				Logger.Log ("no scene given", LogLevel.WARNING);
			}

			return false;
		}

		private INode GetStartNode (Scene scene, int id)
		{
			if(scene != null)
			{
				return scene.FindNodeByID(id.ToString(IDFormat));
			}
			else
			{
				return default(INode);
			}
		}


		private IList<INode> GetEnumerableNodesByIDInOrder (Scene scene, IEnumerable<int> history)	// NOTE: scene.FindAllNodes<INode> uses Preorder search which will destroy given order
		{
			List<INode> nodes = new List<INode> ();
			
			if (scene != null && history != null)
			{
				foreach(int id in history)
				{
					nodes.Add(scene.FindNode<INode>((node) => Convert.ToInt32(node.ID) == id));
				}	
			}

			return nodes;
		}
		
	}
    
}








//public sealed class StoryPlayerHistoryIntID : StoryPlayerBasicForwardHistory
//{
//	
//	public StoryPlayerHistoryIntID(ILogger logger, IDictionary<Type,INodeController> nodeControllers, Action onFinish=null) : base (logger, nodeControllers, onFinish)  {}
//	
//	public override bool StartScene (Scene scene)				// FIXME: StartScene methods need to make more sense!!! get rid or fix them
//	{
//		return StartScene (scene, 1, new List<int>{0});
//	}
//	
//	public bool StartScene(Scene scene, int nodeID, IEnumerable<int> history)	// NOTE: maybe shouldn't make nodeID optional ???
//	{
//		return StartScene (scene, nodeID, GetEnumerableNodesByIDInOrder (scene, history));
//	}
//	
//	
//	public bool StartScene(Scene scene, int nodeID, IEnumerable<INode> history)
//	{
//		return GoTo (scene, (node) => Convert.ToInt32 (node.ID) == nodeID, history);
//	}
//	
//	private IList<INode> GetEnumerableNodesByIDInOrder (Scene scene, IEnumerable<int> history)	// NOTE: scene.FindAllNodes<INode> uses Preorder search which will destroy given order
//	{
//		List<INode> nodes = new List<INode> ();
//		
//		if (scene != null && history != null)
//		{
//			foreach(int id in history)
//			{
//				nodes.Add(scene.FindNode<INode>((node) => Convert.ToInt32(node.ID) == id));
//			}	
//		}
//		
//		return nodes;
//	}
//	
//}