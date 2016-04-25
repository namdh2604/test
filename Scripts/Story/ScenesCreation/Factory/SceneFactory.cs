
using System;
using System.Collections.Generic;

namespace Voltage.Witches.Story.StoryDivisions.Factory
{
	using Voltage.Common.Logging;

	using Voltage.Story.General;
	using Voltage.Witches.Story;
	using Voltage.Story.StoryDivisions;
	using Voltage.Story.Configurations;
	using Voltage.Witches.Services;

	using Voltage.Story.Models.Nodes.ID;

	using TermLevel = Voltage.Story.StoryDivisions.Scene.TermLevel;


    public class SceneFactory : IFactory<string,Scene>	// IFactory<ScenePath,Scene>
    {
		private readonly IDictionary<string,string> _fileMap = new Dictionary<string,string> ();
		private readonly IFilesystemService _filesystemService;
		private readonly MasterStoryData _storyData;

		public SceneFactory(MasterStoryData storyData, IFilesystemService filesystemService) 
		{
			if(storyData == null || filesystemService == null) 
			{
				throw new ArgumentNullException("SceneFactory::Ctor");
			}

			_fileMap = storyData.SceneToFileMap;		// SceneToFileMap can be null
			_filesystemService = filesystemService;
			_storyData = storyData;
		}


		public Scene Create(string scenePath)
		{
			if(_fileMap.ContainsKey(scenePath))
			{
				string json = GetJson(_fileMap[scenePath]);

				SceneData data = new SceneData
				{
					JSON = json,
					Description = GetDescription(scenePath),
					TerminationLevel = GetTerminationLevel(scenePath),
					//TODO Scene Image path is NEEDED, from the data config for previewPath
//					SceneImagePath = GetSceneImagePath(scenePath),
					IDGenerator = new SimpleNumIncrementIDGenerator(),
				};

				AmbientLogger.Current.Log (string.Format ("SceneFactory: Creating Scene: {0}", scenePath), LogLevel.INFO);
				return new Scene(data);
			}
			else
			{
				throw new ArgumentException();
			}
		}

		private string GetJson(string path)
		{
			return _filesystemService.ReadAllText(path);
		}

		private string GetSceneImagePath(string scenePath)
		{
			return _storyData.PreviewImages[scenePath].Trim();
		}

		private string GetDescription(string scenePath)	// NOTE: descriptions are handled differently from the rest (ie, is referenced in MasterStoryData)
		{
			string description = _storyData.SceneDescriptions[scenePath];
			if (description == null)
			{
				description = string.Empty;
			}
			else
			{
				description = description.Trim();
			}
			return description;
		}

		private TermLevel GetTerminationLevel(string scenePath)
		{
			return _storyData.SceneTerminationLevels [scenePath];
		}

    }
    
}






