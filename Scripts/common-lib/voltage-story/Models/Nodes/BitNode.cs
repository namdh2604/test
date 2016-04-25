
using System;
using System.Collections.Generic;

namespace Voltage.Story.Models.Nodes
{
	using Newtonsoft.Json.Linq;
	using Voltage.Story.Models.Nodes.ID;

    public class BitNode : BaseNode
    {
		public BitNode(JToken json, INode caller, IIDGenerator<string,INode> idGenerator) : base (json, caller, idGenerator) 
		{
			// nothing to do here
		}

    }
    
}




