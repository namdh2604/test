
using System;
using System.Collections.Generic;

namespace Voltage.Witches.Story.StoryDivisions.Factory
{
	using Voltage.Story.General;
	using Voltage.Story.StoryDivisions;
	using Voltage.Witches.Services;

	using Voltage.Story.Configurations;
    using Voltage.Witches.Exceptions;

//	using Voltage.Common.Logging;

	using UnityEngine;

//	using Voltage.Common.DebugTool.Timer;

	public class SceneHeaderFactory : IFactory<string,SceneHeader>, ISceneHeaderFactory	// IFactory<ScenePath,Scene>
    {
		private readonly IDictionary<string,string> _fileMap = new Dictionary<string,string> ();
		private readonly IDictionary<string,string> _previewImageMap = new Dictionary<string,string>();
		private readonly IDictionary<string,string> _descriptionMap = new Dictionary<string,string>(); 
		private readonly IFilesystemService _filesystemService;
		private readonly IParser<SceneHeader> _headerParser;
		private static string _polaroidBasePath = "Polaroids/";

		public SceneHeaderFactory(MasterStoryData storyData, IParser<SceneHeader> headerParser, IFilesystemService filesystemService)
		{
			if(storyData == null || headerParser == null) // || filesystemService == null) 
			{
				throw new ArgumentNullException("SceneHeaderFactory::Ctor");
			}

			_fileMap = storyData.SceneToFileMap;		// SceneToFileMap can be null
			_previewImageMap = storyData.PreviewImages;
			_filesystemService = filesystemService;
			_descriptionMap = storyData.SceneDescriptions;
			_headerParser = headerParser;
		}


		public SceneHeader Create(string scenePath)
		{
			if(_fileMap.ContainsKey(scenePath))
			{
				string json = GetJson(_fileMap[scenePath]);

				SceneHeader header = _headerParser.Parse(json);			// can throw exception

				string imageFile = GetPreviewImagePath(scenePath);
				
				if(!string.IsNullOrEmpty(imageFile))
				{
					imageFile = _polaroidBasePath + imageFile;
				}
				else
				{
					imageFile = string.Empty;
				}

				header.PolaroidPath = imageFile;	// can throw exception if header is null

				header.Description = GetDescriptionFromPath(scenePath);

				return header;
			}
			else
			{
				throw new WitchesException("Unknown Scene: " + scenePath);
			}
		}

		private string GetJson(string path)
		{
			return _filesystemService.ReadAllText(path);
		}

		private string GetPreviewImagePath(string path)
		{
			string imagePath = path;
			if(_previewImageMap.ContainsKey(path))
			{
				imagePath = _previewImageMap[path];
			}

			var index = imagePath.LastIndexOf("/");
			var fileName = imagePath.Substring(index + 1);
			if(fileName.Contains(".png"))
			{
				fileName = fileName.Replace(".png","");
			}
	
			return fileName;
		}

		private string GetDescriptionFromPath(string scenePath)
		{
			if(_descriptionMap.ContainsKey(scenePath))
			{
				return _descriptionMap[scenePath];
			}

			return string.Empty;
		}
    }
    
}





