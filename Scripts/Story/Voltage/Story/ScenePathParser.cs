using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Voltage.Witches.Story
{
    using Voltage.Story.StoryDivisions;

    public interface IScenePathParser
    {
        string Parse(string rawScene);
    }

    public class ScenePathParser : IScenePathParser
    {
        public ScenePathParser()
        {
        }

        public string Parse(string rawScene)
        {
            JObject restoredScene = JObject.Parse(rawScene);
            JToken header = restoredScene["header"];
            string route = Sanitize(GetRequiredValue(header, "route", "Missing Route"));
            string arc = Sanitize(GetRequiredValue(header, "arc", "Missing Arc"));
            string scene = Sanitize(GetRequiredValue(header, "scene", "Missing Scene Name"));
            string version = Sanitize(header.Value<string>("version"));

            return Scene.CreateScenePath(route, arc, scene, version);
        }

        private string GetRequiredValue(JToken parent, string child, string message)
        {
            string result = parent.Value<string>(child);
            if (string.IsNullOrEmpty(result))
            {
                throw new Exception(message);
            }

            return result;
        }

        private string Sanitize(string raw)
        {
            if (string.IsNullOrEmpty(raw))
            {
                return string.Empty;
            }

            return raw.Trim();
        }
    }
}

