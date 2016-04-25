using System;

namespace Voltage.Witches.Story
{
    using Voltage.Witches.Controllers;
    using Voltage.Story.StoryPlayer;

    public interface IStoryLoaderFacade
    {
        WitchesStoryPlayerScreenController Load(string scenePath, Action<int> onLoadComplete, Action<Exception> errorHandler);
        WitchesStoryPlayerScreenController Load(StoryPlayerSettings settings, Action<int> onLoadComplete, Action<Exception> errorHandler);
        LoadStatus GetLoadStatus(string scenePath);
    }

    // a facade that hides extraneous options from the story loader for normal game usage.
    // Extra options can be desired for the tutorial, so you should use the base class in those cases
    public class StoryLoaderFacade : IStoryLoaderFacade
    {
        private readonly IStoryLoader _loader;
		private readonly string defaultDialog = string.Empty;

        public StoryLoaderFacade(IStoryLoader loader)
        {
            _loader = loader;
        }

        public WitchesStoryPlayerScreenController Load(string scenePath, Action<int> onLoadComplete, Action<Exception> errorHandler)
        {

            _loader.SetSceneLoadedCallback(onLoadComplete);
			return _loader.Load(scenePath, true, errorHandler, defaultDialog);
        }

        public WitchesStoryPlayerScreenController Load(StoryPlayerSettings settings, Action<int> onLoadComplete, Action<Exception> errorHandler)
        {
            _loader.SetSceneLoadedCallback(onLoadComplete);
            return _loader.Load(settings, true, errorHandler, defaultDialog);
        }

        public LoadStatus GetLoadStatus(string scenePath)
        {
            return _loader.GetLoadStatus(scenePath);
        }
    }
}

