
using System;
using System.Collections.Generic;

namespace Voltage.Witches.Story.StoryDivisions.Parsers
{
	using Newtonsoft.Json;
	using Newtonsoft.Json.Linq;

	using Voltage.Story.General;
	using Voltage.Story.StoryDivisions;
	using Voltage.Witches.Models.MissionRequirements;
	using Voltage.Story.Expressions;

    public class SceneHeaderParser : IParser<SceneHeader>
    {

		private readonly IMissionRequirementParser _requirementsParser;
		private readonly IParser<ExpressionState> _expressionStateParser;

		public SceneHeaderParser(IMissionRequirementParser requirementsParser, IParser<ExpressionState> expressionStateParser)
		{
			if(requirementsParser == null || expressionStateParser == null)
			{
				throw new ArgumentNullException("SceneHeaderParser::Ctor");
			}

			_requirementsParser = requirementsParser;
			_expressionStateParser = expressionStateParser;
		}



		public SceneHeader Parse(string rawScene)
		{
			string headerJson = JObject.Parse (rawScene) ["header"].ToString();

			ProxyHeader proxy = JsonConvert.DeserializeObject<ProxyHeader> (headerJson);
			List<IMissionRequirement> requirements = CreateRequirementsList (proxy.requirements);

			return new SceneHeader (Sanitize(proxy.route), Sanitize(proxy.arc), Sanitize(proxy.scene), proxy.description, proxy.sceneImagePath, requirements);
		}

        private string Sanitize(string raw)
        {
            return raw.Trim();
        }



		private List<IMissionRequirement> CreateRequirementsList(IList<string> requirements)
		{
			List<IMissionRequirement> requirementsList = new List<IMissionRequirement>();

			foreach(string rawRequirement in requirements)
			{
				ExpressionState expState = _expressionStateParser.Parse(rawRequirement);
				string[] tokens = { expState.Left, expState.Operator, expState.Right };
				string basicExpression = string.Join(" ", tokens);
				IMissionRequirement req = _requirementsParser.Parse (basicExpression);
				requirementsList.Add(req);
			}

			return requirementsList;
		}


		private class ProxyHeader
		{
			public string route { get; set; }
			public string arc { get; set; }
			public string scene { get; set; }
			public string description { get; set; }
			public string sceneImagePath { get; set; }
			public IList<string> requirements { get; private set; }

			public ProxyHeader(List<object> reqs)
			{
				route = string.Empty;
				arc = string.Empty;
				scene = string.Empty;
				description = string.Empty;
				sceneImagePath = string.Empty;

				requirements = TranslateRequirements(reqs);
			}

			private IList<string> TranslateRequirements(IList<object> reqs)
			{
				IList<string> requirementList = new List<string> ();
				foreach(object req in reqs)
				{
					requirementList.Add (req.ToString());
				}

				return requirementList;
			}
		}

    }


    
}




