using Newtonsoft.Json.Linq;
using Voltage.Story.Models.Nodes.ID;

namespace Voltage.Story.Models.Nodes
{
	public class BranchNode : TraversalNode
	{
		public string Expression { get; private set; }

		public BranchNode (JToken json, INode caller, IIDGenerator<string,INode> idGenerator) : base(json, "children", caller, idGenerator) 
		{
			try
			{
				Expression = json ["expression"].ToString();
			}
			catch (System.Exception e)
			{
				System.Console.WriteLine(e.ToString());
			}
		}
		
		public override string ToString ()
		{
			return string.Format ("[{0}]branch()\n{1}", ID, Next != null ? Next.ToString () : "null");
		}
	}
}

