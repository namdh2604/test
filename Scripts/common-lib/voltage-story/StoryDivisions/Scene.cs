using System;
using System.Collections;
using System.Collections.Generic;

using Newtonsoft.Json.Linq;

using Voltage.Story.Models.Nodes;
using Voltage.Story.Models.Nodes.ID;
using Voltage.Story.Models.Nodes.Extensions;
using Voltage.Witches.Exceptions;
using Voltage.Common.Logging;

namespace Voltage.Story.StoryDivisions
{
	public class SceneData		// WIP...may eventually become the "Scene"
	{
		public string JSON { get; set; }
//		public SceneHeader Header { get; set; }
//			public string Route { get; set; }
//			public string Arc { get; set; }
//			public string Name { get; set; }
//			public string Version { get; set; }
		public string Description { get; set; }
		public Scene.TermLevel TerminationLevel { get; set; }
		public IIDGenerator<string,INode> IDGenerator { get; set; }
	}


	public class Scene : TraversalNode, IEnumerable<INode>
	{
        public enum TermLevel
        {
            None = 0,
            Route,
            Arc
        };

        private readonly TermLevel[] TERMINATION_LEVELS = new TermLevel[] { TermLevel.None, TermLevel.Route, TermLevel.Arc };
		// State, visited selection
//		public IList<INode> Visited { get; private set; }	// TODO
	

		public string Route { get; private set; }	// FIXME: return Route type instead?
		public string Arc { get; private set; }		// FIXME: return Arc type instead?

		public string Name { get; private set; }
        public string Version { get; private set; }

        public string Description { get; private set; }
        public TermLevel TerminationLevel { get; private set; }

        public string Path { get { return GetPath(); } }

		public List<string> Requirements { get; private set; }

        private const string PATH_SEPARATOR = "/";
        private string GetPath()
        {
            return CreateScenePath(Route, Arc, Name, Version);
        }

        public static string CreateScenePath(string route, string arc, string scene, string version="")
        {
            List<string> tokens = new List<string>() { route, arc, scene };
            if (!string.IsNullOrEmpty(version))
            {
                tokens.Add(version);
            }

            return string.Join(PATH_SEPARATOR, tokens.ToArray());
        }


		// WIP...or instead of SceneData pass explicitly
		public Scene (SceneData data) : base(data.JSON, "data", null, data.IDGenerator)
		{
			if (data == null)
			{
				throw new ArgumentNullException("Scene:Ctor");
			}

			Version = string.Empty; // TODO: Placeholder for when the version is correctly parsed in composer
//			Visited = new List<INode>();
			
			DefaultAttributes();
			SetAttributes(data.JSON);
			
			SetRequirements (data.JSON);

			Description = data.Description;
			TerminationLevel = data.TerminationLevel;
		}



		// Deprecated
		public Scene (string json, JToken config, IIDGenerator<string,INode> idGenerator=null) : base(json, "data", null, idGenerator)
		{
            Version = string.Empty; // TODO: Placeholder for when the version is correctly parsed in composer
//			Visited = new List<INode>();

			DefaultAttributes();
			SetAttributes(json);

			SetRequirements (json);

            JToken sceneConfig = GetSceneConfig(config);
            SetDescription(sceneConfig);	// TODO: uncomment, resolve exception when not present
            TranslateTerminationLevel(sceneConfig);
		}

		private static IEnumerator<INode> GetEnumeratorInternal(INode start)
		{
			INode node = start;
			while (node != null)
			{
				yield return node;

				if (node is IBranchable<INode>)
				{
					IBranchable<INode> branchingNode = node as IBranchable<INode>;
					foreach (var branch in branchingNode.Branches)
					{
						var branchIt = GetEnumeratorInternal(branch);
						while (branchIt.MoveNext())
						{
							yield return branchIt.Current;
						}
					}
				}

				node = node.Next;
			}
		}

		public IEnumerator<INode> GetEnumerator()
		{
			return GetEnumeratorInternal(this);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		private void DefaultAttributes()
		{
			Name = string.Empty;
//			ID = string.Empty;
		}

        private JToken GetSceneConfig(JToken config)
        {
            JToken route = config["routes"][Route];
            if (route == null)
            {
                throw new WitchesException("Route not found in configuration: " + Route);
            }

            JToken arc = route["arcs"][Arc];
            if (arc == null)
            {
                throw new WitchesException("Arc not found in configuration: " + Arc);
            }

            JToken scene = arc["scenes"][Name];
            if (scene == null)
            {
                throw new WitchesException("Scene not found in configuration: " + Name);
            }

            return scene;
        }

        private void SetDescription(JToken config)
        {
            if (config == null)
            {
                return;
            }

            Description = config.Value<string>("description");
        }

        private void TranslateTerminationLevel(JToken config)
        {
            if (config == null)
            {
                TerminationLevel = TermLevel.None;
                return;
            }

            int termLevel = config.Value<int>("terminates");
            if ((termLevel < 0) || (termLevel >= TERMINATION_LEVELS.Length))
            {
                throw new WitchesException("Invalid termination level: " + termLevel);
            }

            TerminationLevel = TERMINATION_LEVELS[termLevel];
        }

		private void SetAttributes(string json)
		{
			try
			{
				if(!string.IsNullOrEmpty(json))
				{
					JToken token = JToken.Parse(json);
					if(token != null)
					{
						JToken header = token["header"];
						if(header != null)
						{
                            Route = SanitizeHeaderField(TryGet<string>(header, "route", string.Empty));
                            Arc = SanitizeHeaderField(TryGet<string>(header, "arc", string.Empty));
                            Name = SanitizeHeaderField(TryGet<string>(header, "scene", string.Empty));
                            Description = SanitizeHeaderField(TryGet<string>(header, "description", string.Empty));
						}
						else
						{
							Console.WriteLine("Scene::SetAttribute >>> No header");
						}
					}
				}
			}
			catch(Exception e)
			{
				Console.WriteLine(e);
			}
		}

        private string SanitizeHeaderField(string raw)
        {
            return raw.Trim();
        }

		private void SetRequirements(string json)
		{
			Requirements = new List<string>();

			if(!string.IsNullOrEmpty(json))
			{	
				JToken token = JToken.Parse(json);
				if(token != null)
				{
					JToken header = token["header"];
					if(header != null)
					{
						string reqs = TryGet<string>(header, "reqs", string.Empty);

						if(!string.IsNullOrEmpty(reqs))
						{
							JArray jsonArray = JArray.Parse(reqs);
							foreach(JToken requirement in jsonArray)
							{
								Requirements.Add (requirement.ToString());
							}
						}
						else
						{
//							Console.WriteLine("Scene::SetRequirements >>> No requirements");
						}

					}
					else
					{
						Console.WriteLine("Scene::SetRequirements >>> No header");
					}
				}
			}
		}


		public INode FindNodeByID(string id)
		{
			return this.FindNode<INode>((node) => node.ID == id);
		}


		public override string ToString ()
		{
			return string.Format ("[{0}]scene\n{1}", ID, Next != null ? Next.ToString () : "null"); 
		}

        /// <summary>
        /// Test Constructor -- DO NOT USE IN PRODUCTION CODE; scene needs to be refactored to support this constructor eventually
        /// </summary>
        public Scene(string route, string arc, string name) : this(route, arc, name, string.Empty) { }

        /// <summary>
        /// Test Constructor -- DO NOT USE IN PRODUCTION CODE; scene needs to be refactored to support this constructor eventually
        /// </summary>
        public Scene(string route, string arc, string name, string version) : base ("{}", "data", null, null)
        {
            Route = route;
            Arc = arc;
            Name = name;
            Version = version;
        }
	}
}
