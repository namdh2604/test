using System;

namespace Voltage.Story.Models.Nodes.Serialization
{
	[Serializable]
	public class NodeSaveState : INodeState
	{
		public string Route { get; private set; }
		public string Arc { get; private set; }
		public string Scene { get; private set; }
		public string NodeID { get; set; }
		
		public NodeSaveState (string route, string arc, string scene, string id)
		{
			Route = route;
			Arc = arc;
			Scene = scene;
			NodeID = id;
		}
	}
}

