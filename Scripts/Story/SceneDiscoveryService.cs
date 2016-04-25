using System;
using System.Collections.Generic;
using Voltage.Common.Logging;

namespace Voltage.Witches.Story
{
    using Voltage.Witches.Services;


    public interface ISceneDiscoveryService
    {
        Dictionary<string, string> DiscoverScenes(string root);
    }

    public class SceneDiscoveryService : ISceneDiscoveryService
    {
        private readonly IScenePathParser _parser;
        private readonly IFilesystemService _filesystem;
        private readonly ILogger _logger;

        public SceneDiscoveryService(IScenePathParser parser, IFilesystemService filesystem, ILogger logger)
        {
            _parser = parser;
            _filesystem = filesystem;
            _logger = logger;
        }

        public Dictionary<string, string> DiscoverScenes(string root)
        {
            Dictionary<string, string> sceneDict = new Dictionary<string, string>();

            // find all files underneath the root directory (recursively)
            var paths = _filesystem.ListAllFiles(root, "*.json");

            foreach (var path in paths)
            {
                string text = _filesystem.ReadAllText(path);

                string scenePath;
                try
                {
                    scenePath = _parser.Parse(text);
                }
                catch (Exception e)
                {
                    string format = "Parsing exception in {0}: {1}";
                    _logger.Log(string.Format(format, path, e.Message), LogLevel.WARNING);
                    continue; // ignore this scene -- should likely be changed when scripts are in a better state
                }

                if (sceneDict.ContainsKey(scenePath))
                {
                    string existingPath = sceneDict[scenePath];
                    string errorMsgFormat = "Duplicate route {0} found in file {1}. Already exists in {2}";
                    _logger.Log(string.Format(errorMsgFormat, scenePath, path, existingPath), LogLevel.WARNING);
                    continue;
                }

                sceneDict[scenePath] = path;
            }

            return sceneDict;
        }
    }
}

