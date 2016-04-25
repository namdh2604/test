using Voltage.Common.Logging;
using System.Collections.Generic;

namespace Voltage.Story.StoryDivisions
{
	public class Story : BaseIdentifiableContainer<Route>
	{
		public IList<Route> Routes { get { return new List<Route> (ElementMap.Values); } }
		public IDictionary<string,Route> RouteMap { get { return ElementMap; } }
		
		public ILogger Logger { get; private set; }

		public Story() : base ("temp"){}	// TEMPORARY

		public Story (ILogger logger, string name, params Route[] routes) : base(name, routes) 
		{
			Logger = logger;
		}
		
		public Route GetRoute (string route)
		{
			if(RouteMap.ContainsKey(route))
			{
				return RouteMap[route];
			}
			{
				Logger.Log("does not contain route", LogLevel.WARNING);
			}
			
			return default(Route);
		}
		
		public Arc GetArc (string route, string arc)
		{
			Route fetchedRoute = GetRoute (route);
			if(fetchedRoute != null)
			{
				if(fetchedRoute.ArcMap.ContainsKey(arc))
				{
					return fetchedRoute.ArcMap[arc];
				}
				else
				{
					Logger.Log("does not contain arc", LogLevel.WARNING);
				}
			}
			
			return default(Arc);
		}

        private string GetFullSceneName(string route, string Arc, string scene, string version="")
        {
            string fmt = "{0}:{1}:{2}";
            List<string> tokens = new List<string>() { route, Arc, scene };

            if (!string.IsNullOrEmpty(version))
            {
                fmt += ":{3}";
                tokens.Add(version);
            }

            return string.Format(fmt, tokens.ToArray());
        }
		
        public Scene GetScene(string route, string arc, string scene, string version="")		// TODO: call GetRoute and GetArc
		{
			if (RouteMap.ContainsKey(route))
			{
				Route fetchedRoute = RouteMap[route];
				if(fetchedRoute.ArcMap.ContainsKey(arc))
				{
					Arc fetchedArc = fetchedRoute.ArcMap[arc];
					if(fetchedArc.SceneMap.ContainsKey(scene))
					{
						return fetchedArc.SceneMap[scene];
					}
					else
					{
                        Logger.Log("does not contain scene: " + GetFullSceneName(route, arc, scene, version), LogLevel.WARNING);
					}
				}
				else
				{
                    Logger.Log("does not contain arc: " + GetFullSceneName(route, arc, scene, version), LogLevel.WARNING);
				}
			}
			else
			{
                Logger.Log("does not contain route: " + GetFullSceneName(route, arc, scene, version), LogLevel.WARNING);
			}
			
			return default(Scene);
		}
	}
    
}




