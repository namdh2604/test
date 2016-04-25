
using System;
using System.Collections.Generic;

namespace Voltage.Story.StoryPlayer
{
	using Voltage.Common.Logging;
	using Voltage.Story.Models.Nodes;
	using Voltage.Story.StoryDivisions;
	using Voltage.Story.Models.Nodes.Controllers;

	using Voltage.Story.StoryDivisions.Search;


	public class StoryPlayerNoHistory : IStoryPlayer
	{
		public ILogger Logger { get; private set; }
		public IDictionary<Type,INodeController> NodeControllers { get; private set; } // NOTE: not really private
		
		public Action OnFinish { get; set; }
		
		
		public Scene CurrentScene { get; protected set; }
		public INode CurrentNode { get; protected set; }
		public INodeController CurrentNodeController { get { return GetController (CurrentNode); } }
		
		protected IDictionary<INode,INode> _branchDic = new Dictionary<INode,INode>();	
		
		
		public StoryPlayerNoHistory (ILogger logger, IDictionary<Type,INodeController> nodeControllers, Action onFinish=null)	
		{
			Logger = logger;
			OnFinish = onFinish;
			
			NodeControllers = nodeControllers;
		}
		
		
		public bool StartScene (Scene scene)
		{
			return StartScene (scene, scene);
		}
		
		public virtual bool StartScene (Scene scene, INode startNode)
		{
			if(scene != null)
			{
				CurrentScene = scene;
				LinkBranches(CurrentScene);
				
				if(startNode != null)
				{
//					Logger.Log(string.Format ("loading scene: {0} at node: {1}", scene.ID, startNode.ID), LogLevel.INFO);
					
					return Process (startNode);	// if nodeID == 0	
				}
				else
				{
					Logger.Log("start node not found", LogLevel.WARNING);
				}
			}
			else
			{
				Logger.Log ("scene is null", LogLevel.WARNING);
			}
			
			return false;
		}
		
		protected void LinkBranches (Scene scene)
		{
			_branchDic = StorySearch.GetAllBranchContinuationPoints(scene);
		}
		
		
		protected virtual bool Process (INode node)	// FIXME: return bool???
		{
			INodeController nodeCtrl = GetController (node);	// implicit check for NULL node as GetController will return null if not found
			if(nodeCtrl != null)
			{
				CurrentNode = node;
				
				nodeCtrl.Execute(node, this);	// TODO: pass in callback for Execute() to call when finished, nodeCtrl.Execute(node, this, Callback);	
				return true;
			}
			else
			{
                Logger.Log ("node controller is null", LogLevel.WARNING);
                throw new Exception("Could not locate appropriate controller for node");
			}
		}
		
		protected virtual INodeController GetController (INode node)	// FIXME: or call INodeControllerFactory or IoC container!!!
		{
//			Logger.Log ("getting controller for: " + (node != null ? node.GetType().ToString() : "null"), LogLevel.INFO);

//			foreach(var kvp in NodeControllers)
//			{
//				Logger.Log (kvp.Key + ", " + kvp.Value, LogLevel.INFO);
//			}

			INodeController controller = default(INodeController);
			if(node != null && NodeControllers != null)
			{
				NodeControllers.TryGetValue(node.GetType(), out controller);	// if (NodeControllers.TryGetValue(node.GetType(), out controller))
			}
			
			return controller;
		}

		
		public virtual void Next()
		{
			if(CurrentNode != null && CurrentNode.Next != null)
			{
				Process(CurrentNode.Next);
			}
			else
			{
				HandleEndOfBranch(CurrentNode);
			}
		}
		
		protected virtual void HandleEndOfBranch(INode currentNode)
		{
			Logger.Log ("Handling End of Branch", LogLevel.INFO);
			
			if(currentNode != null && _branchDic.ContainsKey(currentNode))
			{
				Process(_branchDic[currentNode]);
			}
			else
			{
				Logger.Log ("no node to go to", LogLevel.INFO);
				Fin ();
			}
		}
		
		protected virtual void Fin()
		{
			if(OnFinish != null)
			{
				Logger.Log ("Calling OnFinish()", LogLevel.INFO);
				OnFinish();
			}
			else
			{
				Logger.Log ("No OnFinish() callback", LogLevel.WARNING);
			}
		}
		
		public virtual void Next(int index)
		{
			if(CurrentNode != null)
			{
				IBranchable<INode> forkNode = CurrentNode as IBranchable<INode>;	// NOTE: does not get added to history!!!
				if(forkNode != null)
				{
					if(forkNode.Branches != null && ValidIndex(index, forkNode))	//index < forkNode.Branches.Count)
					{
//						Process(forkNode.Branches[index].Next);		// NOTE: Will skip option/branch node
						Process(forkNode.Branches[index]);			// NOTE: may need to adjust StoryUtil generate history to account for this
					}
					else
					{
						Logger.Log ("branchable index out of bounds", LogLevel.WARNING);
					}
				}
				else
				{
					Logger.Log ("not a branchable node", LogLevel.WARNING);
				}
			}
		}
		
		private bool ValidIndex(int index, IBranchable<INode> branch)
		{
			return index >= 0 && index < branch.Branches.Count;
		}
		
		
		public virtual void Previous()	
		{
			if(CurrentNode != null && CurrentNode.Previous != null)
			{
				Process (CurrentNode.Previous);		
			}
			else
			{
				Logger.Log ("no previous node to go back to", LogLevel.INFO);
			}
		}
		
		
		
		protected bool HasNextNode(INode node)
		{
			return node != null && node.Next != null;
		}
		
	}
}




