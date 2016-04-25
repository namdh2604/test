using System;
using System.Collections;

namespace Voltage.Witches.Controllers
{
    using Voltage.Witches.Story;
    using Voltage.Witches.Models;
    using Voltage.Witches.Exceptions;
    using Voltage.Common.Logging;

    public class StoryPlayerTutorialAPI
    {
        private readonly IStoryLoaderFactory _storyLoaderFactory;

        private bool _isSceneLoaded;
        private WitchesStoryPlayerTutorialScreenController _activeSceneController;
        private readonly Player _player;

        public bool ShowCompletedScene { get; set; }

        public StoryPlayerTutorialAPI(IStoryLoaderFactory storyLoaderFactory, Player player)
        {
            _storyLoaderFactory = storyLoaderFactory;

            _isSceneLoaded = false;

            _player = player;
            ShowCompletedScene = true;
        }

        public WitchesStoryPlayerTutorialScreenController GetActiveScreen()
        {
            return _activeSceneController;
        }

		public IEnumerator PlayScene(string scene, bool showInterface=false, string dialogType=WitchesStoryPlayerScreenController.TUTORIAL_DIALOG)
        {
            IStoryLoader loader = _storyLoaderFactory.Create(_player);
            loader.SetSceneLoadedCallback(HandleStoryLoaded);

            if (_player.CurrentScene == scene)
            {
				_activeSceneController = loader.Resume(showInterface, errorHandler: HandleLoadingError, dialogType: dialogType) as WitchesStoryPlayerTutorialScreenController;
            }
            else 
            {
                if (!string.IsNullOrEmpty(_player.CurrentScene) && (_player.CurrentScene != scene))
                {
                    AmbientLogger.Current.Log("Overwriting existing player progress for: " + _player.CurrentScene, LogLevel.WARNING);
                }
				_activeSceneController = loader.Load(scene, showInterface, errorHandler: HandleLoadingError, dialogType: dialogType) as WitchesStoryPlayerTutorialScreenController;
            }
            _activeSceneController.ShowCompletedScreen = ShowCompletedScene;

            while (!_isSceneLoaded)
            {
                yield return null;
            }
        }

        private void HandleStoryLoaded(int response)
        {
            _isSceneLoaded = true;
        }

        public IEnumerator EnableInput(bool value)
        {
			return _activeSceneController.EnableInput(value);
        }

		public IEnumerator MoveToNextNode()
		{
			return _activeSceneController.MoveToNextNode ();
		}

        public IEnumerator WaitForSceneEnd(bool resetMusic=true)
        {
            if (_activeSceneController == null)
            {
                throw new WitchesException("Cannot wait for scene end when no scene is active");
            }

            return _activeSceneController.WaitForSceneEnd(resetMusic);
        }

        public IEnumerator Close()
        {
            _activeSceneController.Close();
            _isSceneLoaded = false;
            _activeSceneController = null;
            yield break;
        }

        private void HandleLoadingError(Exception e)
        {
            AmbientLogger.Current.Log("Could not load the story due to: " + e.Message, LogLevel.CRITICAL);
        }
    }
}

