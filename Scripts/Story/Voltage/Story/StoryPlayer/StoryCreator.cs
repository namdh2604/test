using System;
using System.Collections.Generic;
using Voltage.Common.Logging;
using System.Linq;

namespace Voltage.Story.StoryPlayer
{
	using Newtonsoft.Json;
	using Newtonsoft.Json.Linq;

	using Voltage.Story.Models.Nodes;
	using Voltage.Story.Models.Nodes.ID;
	using Voltage.Story.StoryDivisions;

	using Voltage.Common.Json;

	public class StoryCreator
	{
		private static readonly StoryCreator instance = new StoryCreator();
		public static StoryCreator Default { get { return instance; } }

		private StoryCreator () {}	
		static StoryCreator () {}

	
		public ILogger Logger { get; private set; }


		public StoryCreator (ILogger logger)	// TODO: could combine with Story class???
		{
			Logger = logger;
		}

        public Story CreateStory(string name, string masterStoryData, IDictionary<string, string> sceneJsons)
		{
            if (string.IsNullOrEmpty(masterStoryData) || (sceneJsons == null))
            {
                throw new Exception("Missing story parameter");
            }

			JToken masterStoryDataTokenized = TokenizeMasterDoc(masterStoryData);
            IList<Scene> scenePool = CreateScenes(sceneJsons, masterStoryDataTokenized);

			return new Story(Logger, name, SetupRoutes (masterStoryDataTokenized, scenePool).ToArray<Route>());
		}



		public JToken TokenizeMasterDoc(string masterDocPath)
		{
			return JsonUtilities.TokenizeJson (masterDocPath);
		}


        public IList<Scene> CreateScenes(IDictionary<string, string> sceneJsons, JToken masterConfig)
		{
			// TODO: add argument for IIDGenerator<string,INode>
            string errorFormat = "Error occured constructing scene at {0}: {1}; Skipping";
            return JsonUtilities.ConvertJsonToListOf<Scene>(ConstructScene, sceneJsons, masterConfig, errorFormat);
		}

        private Scene ConstructScene(string rawJson, JToken config)
        {
            return new Scene(rawJson, config, new SimpleNumIncrementIDGenerator());
        }

		private IIDGenerator<string,INode> GetIDGenerator(string json)	// TODO: replace 'new SimpleNumIncrementIDGenerator()' in CreateScenes
		{
			// parse out Route, Arc, Scene....return RouteArcSceneNumIncrementIDGenerator(route, arc, scene)
			throw new NotImplementedException ();
		}


		public IList<Route> SetupRoutes(JToken masterDoc, IEnumerable<Scene> scenePool)
		{
			List<Route> routes = new List<Route>();

			if(masterDoc !=  null && scenePool != null)
			{
				JToken routeTokens = masterDoc["routes"];
				if(routeTokens != null)
				{
					foreach(JProperty route in routeTokens)
					{
						List<Arc> routeArcs = new List<Arc>();
						foreach(JProperty arc in route.Value["arcs"])
						{
                            List<string> sceneList = new List<string>();
                            foreach (JProperty rawScene in arc.Value["scenes"])
                            {
                                sceneList.Add(rawScene.Name);
                            }
							List<Scene> arcScenes = new List<Scene>(scenePool).FindAll((scene) => SceneHasProperPath(scene, route.Name, arc.Name, sceneList)); 

							routeArcs.Add(new Arc (arc.Name, arcScenes.ToArray()));
						}

						routes.Add(new Route(route.Name, routeArcs.ToArray()));
					}
				}
			}

			return routes;
		}

		private bool SceneHasProperPath (Scene candidateScene, string route, string arc, IList<string> sceneList)
		{
			return candidateScene.Route == route && candidateScene.Arc == arc && sceneList.Contains (candidateScene.Name);
		}
	}
}




