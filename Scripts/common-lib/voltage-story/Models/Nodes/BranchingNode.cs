using System;
using System.Collections.Generic;

using Newtonsoft.Json.Linq;

using Voltage.Story.Models.Nodes.ID;
using Voltage.Story.Utilities;

namespace Voltage.Story.Models.Nodes
{
	public abstract class BranchingNode : BaseNode, IBranchable<INode> //where T : INode
	{
		public IList<INode> Branches { get; private set;}
		
		public BranchingNode (JToken json, INode caller, Func<JToken,INode,INode> createNode, IIDGenerator<string,INode> idGenerator) : base(json, caller, idGenerator)
		{
			SetupBranch (json, createNode);
		}

		private void SetupBranch (JToken json, Func<JToken,INode,INode> createNode) //IIDGenerator<string,INode> idGenerator)
		{
			Branches = new List<INode>();
			
			if (json != null)
			{
				StoryTraversalUtils.TraverseJson(json.ToString(), "children", (type,token) => Branches.Add(createNode(token, this)));	// Branches.Add((INode)Activator.CreateInstance(typeof(T), token, this, idGenerator)));		
			}
			else
			{
				throw new ArgumentNullException();
			}
		}

		public override string ToString ()
		{
			string output = string.Empty;
			if(Branches != null)
			{
				foreach(INode option in Branches)
				{
					output += option.ToString() + "\n";
				}
			}
			return string.Format ("[{0}]{1}({2})\n{3}\n{4}", ID, GetType().ToString(), (Branches != null ? Branches.Count : 0), output, Next != null ? Next.ToString() : string.Empty);
		}
		
		public int BranchCount { get { return Branches.Count; } }	// TODO : check for null
		public INode GetBranch(int index)
		{
			if (Branches != null && index < Branches.Count)		// TODO : check for null
			{
				return Branches[index];
			}
			else
			{
				return default(INode);
			}
		}

		public bool HasBranches { get { return Branches != null && Branches.Count > 0; } }
	}
}
