
using System;
using System.Collections.Generic;

namespace Voltage.Story.Models.Nodes
{
	using Newtonsoft.Json.Linq;
	using Voltage.Story.Models.Nodes.ID;

    public class LockNode : BaseNode
    {
		public string Route { get; private set; }
		public string Arc { get; private set; }
		public string Scene { get; private set; }
        public string Version { get; private set; }

		public LockNode (JToken json, INode caller, IIDGenerator<string,INode> idGenerator) : base (json, caller, idGenerator) 
		{
            Route = SanitizeStoryToken(TryGet(json, "route", string.Empty));
            Arc = SanitizeStoryToken(TryGet(json, "arc", string.Empty));
            Scene = SanitizeStoryToken(TryGet(json, "scene", string.Empty));
            Version = SanitizeStoryToken(TryGet(json, "version", string.Empty));
		}

        private string SanitizeStoryToken(string raw)
        {
            return raw.Trim();
        }
    }
    
}




