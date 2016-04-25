
using System;
using System.Collections.Generic;

namespace Voltage.Witches.DI
{
	using Voltage.Common.Logging;

	using Voltage.Story.General;
	using Voltage.Witches.Configuration;
	using Voltage.Story.Configurations;

	using Voltage.Witches.Story;

	using UnityEngine;

	using Voltage.Witches.Services;

	using Newtonsoft.Json;
	using Newtonsoft.Json.Linq;

    using Voltage.Witches.Configuration.JSON;

    public interface ILocalDataFetcher
    {
        void Fetch(Action<Exception, LocalData> callback);
    }

	public class LocalDataFetcher : ILocalDataFetcher
	{
		private readonly IParser<MasterStoryData> _storyDataParser;
        private readonly MusicParser _musicParser;

		public LocalDataFetcher(IParser<MasterStoryData> storyDataParser, MusicParser musicParser)	
		{
			if(storyDataParser == null)
			{
				throw new ArgumentNullException();
			}

			_storyDataParser = storyDataParser;
            _musicParser = musicParser;
		}


		public void Fetch(Action<Exception, LocalData> callback)
		{
            try
            {
    			string gameResources = Resources.Load<TextAsset>("JSON/STORY/gameResources").text;	// maybe move into code that gets MasterConfiguration

    			MasterStoryData storyData = GetMasterStoryData();
                storyData.MusicMap = _musicParser.Parse(gameResources);

    			LocalData data = new LocalData
    			{
    				GameResources = gameResources,
    				StoryMain = storyData,
    			};
    			callback(null, data);
            }
            catch (Exception e)
            {
                callback(e, null);
            }
		}

		private MasterStoryData GetMasterStoryData()
		{
			string storyMainRaw = Resources.Load<TextAsset>("JSON/STORY/masterData").text;	
			MasterStoryData data = _storyDataParser.Parse (storyMainRaw);

			string manifest = Resources.Load<TextAsset>("JSON/STORY/sceneManifest").text;
			data.SceneToFileMap = JsonConvert.DeserializeObject<Dictionary<string,string>> (manifest);
			AmbientLogger.Current.Log("Scene to File Map Count: " + data.SceneToFileMap.Count, LogLevel.INFO);

			return data;
		}
	
	}


    
}




