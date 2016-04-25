
using System;
using System.Collections.Generic;

using Voltage.Common.Serialization;
using Voltage.Story.Models.Nodes.ID;
using Voltage.Story.Models.Nodes.Serialization;
using Voltage.Story.Models.Nodes.Extensions;
using Voltage.Story.StoryDivisions;

namespace Voltage.Story.Models.Nodes
{

	using Newtonsoft.Json;
	using Newtonsoft.Json.Linq;


	public abstract class BaseNode : INode	//, ISavable<NodeSaveState>  // FIXME: ugly mess
	{
		private readonly string _id = string.Empty;
		public string ID { get { return _id; } } 
		
		public virtual INode Next { get; set; }
		public virtual INode Previous { get; set; }
		
		private readonly string _originalJson;
		public string DebugJson { get { return _originalJson; } }
		
		
		protected BaseNode (JToken json, INode caller, IIDGenerator<string,INode> idGenerator)
		{
			_originalJson = json.ToString();
			
			Previous = caller;
			
			if (idGenerator != null)
			{
				_id = idGenerator.GenerateID();
			}
		}
		
		
		public string Background 
		{ 
			get 
			{
				string value = string.Empty;
				_data.TryGetValue("background", out value);
				return value;
			}
			set
			{
				string key = "background";
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
		
		
		protected T TryGet<T>(JToken json, string key, T defaultValue)	// FIXME: likely can't convert all types, should constrain
		{
			if (json != null && !string.IsNullOrEmpty(key))
			{
				try
				{
					JToken token = json[key];
					if (token != null)
					{
						return (T)Convert.ChangeType(token.ToString(), typeof(T));
					}
				}
				catch(Exception e)
				{
					Console.WriteLine(e);
				}
			}
			
			return defaultValue;
		}

		protected Dictionary<string,string> _data = new Dictionary<string,string>();		// TODO: move to DialogueNode if no other Nodes require this
		
		protected virtual void SetAttributes (JToken json)									// TODO: move to DialogueNode if no other Nodes require this
		{
			if(json != null && json.HasValues)
			{
				try
				{
					SetExistingAttributes(json, _data);
					JToken data = json["data"];
					if (data != null)
					{
						SetExistingAttributes(json["data"], _data);
					}
					FindAndSetRemainingAttributes(GetListEmptyKeys(_data), _data);
				}
				catch (Exception e)
				{
					Console.WriteLine(e);
				}
				
			}
		}
		
		
		protected void FindAndSetRemainingAttributes (IList<string> attributesToFind, IDictionary<string,string> data)
		{
			if(attributesToFind.Count > 0 && Previous != null)
			{
				List<string> attributeList = new List<string>(attributesToFind);
				
				foreach(string attribute in attributeList)
				{
					if(_data.ContainsKey(attribute) && !string.IsNullOrEmpty(_data[attribute]))
					{
						data[attribute] = _data[attribute];
						attributesToFind.Remove(attribute);
					}
				}
				
				Previous.FindNodeBefore<BaseNode>().FindAndSetRemainingAttributes(attributesToFind, data);
			}
		}
		
		
		protected List<string> GetListEmptyKeys (IDictionary<string,string> data)	// TODO: move to DialogueNode if no other Nodes require this
		{
			List<string> emptyKeys = new List<string> ();
			
			if (data != null)
			{
				foreach(KeyValuePair<string,string> kvp in data)
				{
					if(string.IsNullOrEmpty(kvp.Value))
					{
						emptyKeys.Add(kvp.Key);
					}
				}
			}
			
			return emptyKeys;
		}
		
		
		private void SetExistingAttributes(JToken json, Dictionary<string,string> dict)	// TODO: move to DialogueNode if no other Nodes require this
		{
			if(json != null && dict != null)
			{
				JObject jsonObj = JObject.Parse (json.ToString ());
				foreach (KeyValuePair<string,JToken> attribute in jsonObj)
				{
					if(dict.ContainsKey(attribute.Key))
					{
						dict[attribute.Key] = attribute.Value.ToString();
					} 
				}
			}
		}


	}
}
