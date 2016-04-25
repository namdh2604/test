using System.Collections.Generic;

namespace Voltage.Story.StoryDivisions
{
    using Voltage.Witches.Models.MissionRequirements;
    using Voltage.Story.Mapper;
    using Voltage.Story.Expressions;
    using Voltage.Story.General;

	public interface ISceneHeaderFactoryOriginal
	{
        SceneHeader Create(string scenePath);
	}

	public class SceneHeaderFactoryOriginal : ISceneHeaderFactoryOriginal	// TODO: Deprecate
	{
        IParser<ExpressionState> _expressionParser;
        IMissionRequirementParser _reqParser;
        Story _story;

		public SceneHeaderFactoryOriginal(Story story, IParser<ExpressionState> parser, IMissionRequirementParser reqParser)
		{
            _story = story;
            _expressionParser = parser;
            _reqParser = reqParser;
		}

        private const int INDEX_ROUTE = 0;
        private const int INDEX_ARC = 1;
        private const int INDEX_SCENE = 2;
        private const int INDEX_VERSION = 3;

        public SceneHeader Create(string scenePath)
        {
            string[] tokens = scenePath.Split('/');
            string version = (tokens.Length > INDEX_VERSION) ? tokens[INDEX_VERSION] : string.Empty;
            Scene scene = _story.GetScene(tokens[INDEX_ROUTE], tokens[INDEX_ARC], tokens[INDEX_SCENE], version);

            List<IMissionRequirement> reqs = CreateRequirements(scene);

			//TODO Get the image path from the preview path
			string sceneImagePath = string.Empty;

            SceneHeader header = new SceneHeader(scene.Route, scene.Arc, scene.Name, scene.Description, sceneImagePath, reqs);

            return header;
        }

        private List<IMissionRequirement> CreateRequirements(Scene scene)
        {
            List<IMissionRequirement> requirements = new List<IMissionRequirement>();

            foreach(string rawReq in scene.Requirements)
            {
                ExpressionState expState = _expressionParser.Parse(rawReq);
                string[] tokens = { expState.Left, expState.Operator, expState.Right };
                string basicExp = string.Join(" ", tokens);
//                UnityEngine.Debug.LogError("req is: " + basicExp);
                IMissionRequirement req = _reqParser.Parse(basicExp);
                requirements.Add(req);
            }

            return requirements;
        }
	}
}

