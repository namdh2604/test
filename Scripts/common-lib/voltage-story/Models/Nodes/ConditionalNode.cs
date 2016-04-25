using Newtonsoft.Json.Linq;
using Voltage.Story.Models.Nodes.ID;

namespace Voltage.Story.Models.Nodes	// Voltage.Story.Models.StructNodes
{
	

	public class ConditionalNode : BranchingNode
	{
		public ConditionalNode (JToken json, INode caller, IIDGenerator<string,INode> idGenerator) : base(json, caller, (token,node) => new BranchNode(token,node,idGenerator), idGenerator) {}
	}
}