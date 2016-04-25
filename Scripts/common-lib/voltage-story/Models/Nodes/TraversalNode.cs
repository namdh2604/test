using System;
using System.Reflection;
using Newtonsoft.Json.Linq;
using Voltage.Story.Utilities;
using Voltage.Story.Models.Nodes.ID;

namespace Voltage.Story.Models.Nodes
{
    using Voltage.Story.Models.Nodes.Factories;

	public abstract class TraversalNode : BaseNode
	{

		public override INode Next 
		{ 
			get { return First; }
			set { First = value; }
		}
		
		
		public INode First { get; protected set; }
		public INode Last { get; protected set; }


		public TraversalNode (JToken json, string key, INode caller, IIDGenerator<string,INode> idGenerator) : this(json.ToString(), key, caller, idGenerator) {}
		public TraversalNode (string json, string key, INode caller, IIDGenerator<string,INode> idGenerator) : base(json, caller, idGenerator)
		{
			Last = this;

			StoryTraversalUtils.TraverseJson(json, key, (type,token) => 
				{
                    INode node;

                    if (type == typeof(EINode))
                    {
                        EventIllustrationNodeFactory factory = new EventIllustrationNodeFactory();
                        node = factory.Create(token, idGenerator.GenerateID(), Last);
                    }
                    else if (type == typeof(EstablishingNode))
                    {
                        EstablishingNodeFactory factory = new EstablishingNodeFactory();
                        node = factory.Create(token, idGenerator.GenerateID(), Last);
                    }
                    else
                    {
                        try
                        {
                            node = (INode)Activator.CreateInstance(type, token, Last, idGenerator);
                        }
                        catch (TargetInvocationException e)
                        {
                            throw e.InnerException; // Activator exceptions are wrapped -- rethrow the inner one, that's the one that is real
                        }
                    }

                    Last.Next = node;
					Last = Last.Next;
				}	
			);
		}
	}
}

