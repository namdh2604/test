using System;
using System.Collections.Generic;

namespace Voltage.Story.StoryDivisions
{
    using Voltage.Story.Expressions;
    using Voltage.Witches.Models.MissionRequirements;

	public class SceneHeader : IEquatable<SceneHeader>
	{
		public string Route { get; private set; }
		public string Arc { get; private set; }
		public string Scene { get; private set; }
        public string Description { get; set; }
		public string PolaroidPath { get; set; }
        public string Version { get; private set; }

        public List<IMissionRequirement> Requirements { get; private set; }

        public string Path { get { return GetPath(); } }
        private const string PATH_SEPARATOR = "/";
        private string GetPath()
        {
            List<string> tokens = new List<string>() { Route, Arc, Scene };
            if (!string.IsNullOrEmpty(Version))
            {
                tokens.Add(Version);
            }

            return string.Join(PATH_SEPARATOR, tokens.ToArray());
        }

        public SceneHeader(string route, string arc, string scene, string description, string sceneImagePath, List<IMissionRequirement> requirements = null)
		{
			Route = route;
			Arc = arc;
			Scene = scene;
			PolaroidPath = sceneImagePath;
            Description = description;
            Requirements = (requirements != null) ? requirements : new List<IMissionRequirement>();
		}

		public bool Equals(SceneHeader other) 
		{
			if (other == null) return false;
			
			if (this.Route == other.Route && this.Arc == other.Arc && this.Scene == other.Scene)
			{
				return true;
			}
			else 
			{
				return false;
			}
		}

		public override bool Equals(Object obj)
		{
			if (obj == null) return false;
			
			SceneHeader other = obj as SceneHeader;
			if (other != null)
			{
				return Equals(other); 
			}
			else
			{
				return false;
			}
		}   
		
		public override int GetHashCode()
		{
			return this.GetHashCode();		// FIXME: not great! replace!
		}

	}
}

