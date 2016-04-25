using System;
using System.Collections.Generic;
using System.Linq;

namespace Voltage.Witches.Story
{
    using Voltage.Story.StoryDivisions;
    using Voltage.Witches.Models;
    using Voltage.Witches.Screens;
    using Voltage.Witches.Controllers;
    using Voltage.Story.StoryPlayer;
    using Voltage.Story.Models.Nodes;
    using Voltage.Witches.Controllers.Factories;

	using Voltage.Story.General;
	using TermLevel = Voltage.Story.StoryDivisions.Scene.TermLevel;
	using Voltage.Story.Configurations;

	using Voltage.Common.Logging;
    using Voltage.Story.StoryDivisions.Search;

	using Scene = Voltage.Story.StoryDivisions.Scene;

    public interface IStoryLoader
    {
		WitchesStoryPlayerScreenController Load(string scenePath, bool showInterface, Action<Exception> errorHandler, string dialogType);
		WitchesStoryPlayerScreenController Resume(bool showInterface, Action<Exception> errorHandler, string dialogType);
        WitchesStoryPlayerScreenController Load(StoryPlayerSettings settings, bool showInterface, Action<Exception> errorHandler, string dialogType);
		void SetSceneLoadedCallback(Action<int> responseHandler);
        LoadStatus GetLoadStatus(string scenePath);
    }


    public class StoryLoader : IStoryLoader
    {

		private Player _player;
		private IFactory<string,Scene> _sceneFactory;
        private ISceneHeaderFactory _headerFactory;
		private ScreenNavigationManager _screenManager;
        private readonly IWitchesStoryPlayerScreenControllerFactory _screenFactory;
		Action<int> _sceneLoadedResponse;
        private readonly RequirementEvaluator _reqEvaluator;

        public StoryLoader (ScreenNavigationManager screenNavManager, Player player, IFactory<string,Scene> sceneFactory, // IControllerRepo repo,
            IWitchesStoryPlayerScreenControllerFactory screenFactory, ISceneHeaderFactory headerFactory, RequirementEvaluator reqEvaluator)
		{
            if (screenNavManager == null || sceneFactory == null || screenFactory == null)
			{
				throw new ArgumentNullException();
			}

			_player = player;

			_sceneFactory = sceneFactory;
            _headerFactory = headerFactory;

			_screenManager = screenNavManager;
            _screenFactory = screenFactory;
            _reqEvaluator = reqEvaluator;
		}

        public WitchesStoryPlayerScreenController Load(string scenePath, bool showInterface, Action<Exception> errorHandler, string dialogType)
		{
			Scene scene = _sceneFactory.Create (scenePath);		// scenePath can be null/empty

			StoryPlayerSettings settings = new StoryPlayerSettings (scene);
			return Load(settings, showInterface, errorHandler, dialogType);
		}

		public WitchesStoryPlayerScreenController Resume(bool showInterface, Action<Exception> errorHandler, string dialogType)
		{
			string scenePath = _player.CurrentScene;
			Scene scene = _sceneFactory.Create (scenePath);
            int currentNodeId = GetCurrentNode(scene);

			StoryPlayerSettings settings = new StoryPlayerSettings(scene, currentNodeId);

            return Load(settings, showInterface, errorHandler, dialogType);
		}

		public void SetSceneLoadedCallback(Action<int> responseHandler)
		{
			_sceneLoadedResponse = responseHandler;
		}

        public WitchesStoryPlayerScreenController Load(StoryPlayerSettings sceneSettings, bool showInterface, Action<Exception> errorHandler, string dialogType)
		{
			Action wrappedErrorHandler = () => AmbientLogger.Current.Log("Unknown Story Player Error", LogLevel.ERROR);
			if(errorHandler != null)
			{
				wrappedErrorHandler = () => errorHandler(new Exception("Unknown Story Player Error occured"));
			}
			// Play given scene -- on completion, return to scene selection page, unless finished with the entire story
			
            WitchesStoryPlayerScreenController storyController = _screenFactory.Create(sceneSettings, showInterface, wrappedErrorHandler, dialogType);

			// add storyplayer screen to navigator after it is ready for display
			Action<int> onStoryDisplay = (i) => { _sceneLoadedResponse(i); _screenManager.Add(storyController);};
			storyController.SetStoryDisplayedHandler(onStoryDisplay);

            return storyController;
		}

        public LoadStatus GetLoadStatus(string scenePath)
        {
            SceneHeader header = _headerFactory.Create(scenePath);

            LockType lockReasons = _reqEvaluator.GetLockType(header);

            Scene scene = _sceneFactory.Create(scenePath);
            StoryPlayerSettings settings = null;
            if (lockReasons == LockType.None)
            {
                int currentNodeId = GetCurrentNode(scene);
                settings = new StoryPlayerSettings(scene, currentNodeId);
            }

            return new LoadStatus(lockReasons, header, settings);
        }

        private int GetCurrentNode(Scene scene)
        {
            int currentNodeId = 0;
            if (!string.IsNullOrEmpty(_player.CurrentNodeID))
            {
                currentNodeId = Int32.Parse(_player.CurrentNodeID) + 1; // add one, because the scene itself is considered 0

                // HACK -- When the player cannot finish a scene due to a network outage,
                // the current node will be pointing to the last node in the scene; obviously, node + 1 doesn't exist.
                // This adjusts the node to point to the last node, but also means we will re-execute that last node.
                // This allows the scene to resume on the end of the scene and show the scene complete dialog
                int lastNodeId = Int32.Parse(scene.Last.ID);
                if (currentNodeId >= lastNodeId)
                {
                    currentNodeId = lastNodeId;
                }
            }

            return currentNodeId;
        }
    }
}

