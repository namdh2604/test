namespace Voltage.Story.Models.Nodes.Serialization
{
	public interface INodeState
	{
		string Route { get; }
		string Arc { get; }
		string Scene { get; }
		string NodeID { get; }
	}
}

