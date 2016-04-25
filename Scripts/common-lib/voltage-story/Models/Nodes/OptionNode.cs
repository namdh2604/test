using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Voltage.Story.Models.Nodes.ID;

namespace Voltage.Story.Models.Nodes
{
	using Newtonsoft.Json;		// FIXME: decouple json.net parsing from class

	public class OptionNode : TraversalNode, IHaveText
	{
		public string Text { get; set; }

		public Dictionary<string,int> Effects { get; private set; }


		public OptionNode (JToken json, INode caller, IIDGenerator<string,INode> idGenerator) : base(json, "children", caller, idGenerator)
		{
			string effectJson = TryGet (json, "effects", string.Empty);
			Effects = !string.IsNullOrEmpty (effectJson) ? JsonConvert.DeserializeObject<Dictionary<string,int>> (effectJson) : new Dictionary<string,int> ();

			Text = TryGet (json, "text", string.Empty);
		}

		public override string ToString ()
		{
			return string.Format ("[{0}]option({1})\n{2}", ID, Text, Next != null ? Next.ToString () : "null");
		}
	}
}

