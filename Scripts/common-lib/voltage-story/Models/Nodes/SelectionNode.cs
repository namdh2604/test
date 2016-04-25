using System.Collections.Generic;
using Newtonsoft.Json.Linq;

using Voltage.Story.Models.Nodes.ID;

namespace Voltage.Story.Models.Nodes
{
    public class SelectionNode : BranchingNode, IHavePrompt, IHasMusic //<OptionNode> //BaseNode, IBranchable<INode>
    {
		public string Prompt { get; private set; }
		public string Name { get; private set; }

		public SelectionNode(string prompt, INode caller, IIDGenerator<string,INode> idGenerator) : this(JToken.FromObject(new Dictionary<string,object>{{"prompt",prompt}}), caller, idGenerator) {}
        public SelectionNode (JToken json, INode caller, IIDGenerator<string,INode> idGenerator) : base(json, caller, (token,node) => CreateChild(token, node, idGenerator), idGenerator)
		{
			DefaultAttributes();
			SetAttributes(json);

            SetMusic(json["data"]);

			Prompt = TryGet<string>(json, "prompt", string.Empty);
			Name = TryGet<string> (json, "name", string.Empty);
		}

        private static OptionNode CreateChild(JToken token, INode node, IIDGenerator<string, INode> generator)
        {
            return new OptionNode(token, node, generator);
        }

		private void DefaultAttributes()
		{
			_data = new Dictionary<string,string>
			{
				{"text", string.Empty}
			};
		}

        private void SetMusic(JToken data)
        {
            this.Music = DialogueNode.GetMusic(Previous, data);
        }

		public string Text
		{ 
			get 
			{
				string value = string.Empty;
				_data.TryGetValue("text", out value);
				return value;
			}
			set
			{
				string key = "text";
				if(_data.ContainsKey(key))
				{
					_data[key] = value;
				}
				else
				{
					_data.Add(key, value);
				}
			}
		}

        public string Music
        { 
            get 
            {
                string value = string.Empty;
                _data.TryGetValue("music", out value);
                return value;
            }
            set
            {
                string key = "music";
                if(_data.ContainsKey(key))
                {
                    _data[key] = value;
                }
                else
                {
                    _data.Add(key, value);
                }
            }
        }
    }
}
