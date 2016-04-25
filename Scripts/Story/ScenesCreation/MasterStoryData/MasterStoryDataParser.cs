using System;
using System.Collections.Generic;

namespace Voltage.Witches.DI
{
	using Newtonsoft.Json;
	using Newtonsoft.Json.Linq;

	using Voltage.Story.General;
	using Voltage.Story.Configurations;

	using Voltage.Witches.Exceptions;
	using TermLevel = Voltage.Story.StoryDivisions.Scene.TermLevel;

	public class MasterStoryDataParser : IParser<MasterStoryData>
    {
		private const string DEFAULT_FORMAT = "{0}/{1}/{2}";
		private readonly string _pathFormat;

		public MasterStoryDataParser(string pathFormat=DEFAULT_FORMAT)
		{
			_pathFormat = (!string.IsNullOrEmpty(pathFormat) ? pathFormat : DEFAULT_FORMAT);
		}

		public MasterStoryData Parse(string text)
		{
			JObject root = JObject.Parse(text);
			MasterStoryDataTuple data = GetSceneDetails(root);

			return new MasterStoryData ()
			{
				SceneDescriptions = data.DescriptionMap,			
				PreviewImages = data.PreviewPathMap,
				SceneTerminationLevels = data.TerminationMap,

				SceneNamedSelections = data.NamedSelectionList,
			};
		}

		private MasterStoryDataTuple GetSceneDetails (JObject root)
		{
			IDictionary<string,string> descriptionMap = new Dictionary<string,string> ();
			IDictionary<string,string> previewPathMap = new Dictionary<string,string> ();
			IDictionary<string,Voltage.Story.StoryDivisions.Scene.TermLevel> terminationMap = new Dictionary<string,Voltage.Story.StoryDivisions.Scene.TermLevel> ();
			List<NamedSelectionData> namedSelectionList = new List<NamedSelectionData> (); 


			foreach (JProperty route in root["routes"])
			{
				foreach (JProperty arc in route.Value["arcs"])
				{
					foreach (JProperty scene in arc.Value["scenes"])
					{
						string path = GetPath(route.Name, arc.Name, scene.Name);
						var sceneObj = scene.Value;

						string description = GetDescription(sceneObj);
						descriptionMap[path] = description;

						string imagePath = sceneObj.Value<string>("previewPath");
						previewPathMap[path] = imagePath;

						TermLevel termLevel = GetTermLevel(sceneObj);
						terminationMap[path] = termLevel;

						namedSelectionList.AddRange(GetSceneNamedSelections(path, sceneObj));
					}
				}
			}

			return new MasterStoryDataTuple ()
			{
				DescriptionMap = descriptionMap,
				PreviewPathMap = previewPathMap,
				TerminationMap = terminationMap,
				NamedSelectionList = namedSelectionList
			};
		}

		private string GetPath (string route, string arc, string scene)
		{
			return string.Format(_pathFormat, route, arc, scene);
		}

		private string GetDescription(JToken sceneDetails)
		{
			string description = sceneDetails.Value<string>("description");
			return (description == null) ? string.Empty : description;
		}

		private readonly TermLevel[] TERMINATION_LEVELS = new TermLevel[] { TermLevel.None, TermLevel.Route, TermLevel.Arc };

		private TermLevel GetTermLevel(JToken sceneDetails)
		{
			JToken termToken = sceneDetails["terminates"];
			if (termToken == null)
			{
				return TermLevel.None;
			}

			int level = termToken.Value<int>();
			if ((level < 0) || (level >= TERMINATION_LEVELS.Length))
			{
				throw new WitchesException("Invalid termination level: " + level);
			}

			return TERMINATION_LEVELS[level];
		}

		private List<NamedSelectionData> GetSceneNamedSelections(string path, JToken sceneDetails)
		{
			List<NamedSelectionData> namedSelections = new List<NamedSelectionData>();
			if (sceneDetails["selections"] != null)
			{
				foreach (var selection in sceneDetails["selections"])
				{
					namedSelections.Add(new NamedSelectionData(path, selection.Value<string>()));
				}
			}

			return namedSelections;
		}

		private class MasterStoryDataTuple
		{
			public IDictionary<string,string> DescriptionMap { get; set; }
			public IDictionary<string,TermLevel> TerminationMap { get; set; }
			public IDictionary<string,string> SceneImageMap { get; set; }
			public IList<NamedSelectionData> NamedSelectionList { get; set; }
			public IDictionary<string,string> PreviewPathMap { get; set; }
		}
    }
    
}



