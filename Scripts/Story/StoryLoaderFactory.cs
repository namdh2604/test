using System;

namespace Voltage.Witches.Story
{
    using Voltage.Witches.Screens;
    using Voltage.Witches.Models;
    using Voltage.Witches.Controllers;
    using Voltage.Story.StoryPlayer;
    using Voltage.Witches.Controllers.Factories;
    using Voltage.Story.StoryDivisions;

	using Voltage.Story.General;
	using Scene = Voltage.Story.StoryDivisions.Scene;

    public interface IStoryLoaderFactory
    {
		IStoryLoader Create(Player player);
        IStoryLoaderFacade CreateFacade(Player player);
    }

    public class StoryLoaderFactory : IStoryLoaderFactory
    {
        private ScreenNavigationManager _screenNavManager;
        private readonly IWitchesStoryPlayerScreenControllerFactory _screenFactory;

		private IFactory<string,Scene> _sceneFactory;
        private readonly ISceneHeaderFactory _headerFactory;
        private readonly RequirementEvaluator _reqEvaluator;

        public StoryLoaderFactory (ScreenNavigationManager screenNavManager, IFactory<string,Scene> sceneFactory, 
            IWitchesStoryPlayerScreenControllerFactory screenFactory, ISceneHeaderFactory headerFactory, RequirementEvaluator reqEvaluator)
		{
			if (screenNavManager == null || sceneFactory == null) 
			{
				throw new ArgumentNullException();
			}

			_screenNavManager = screenNavManager;
			_sceneFactory = sceneFactory;
            _screenFactory = screenFactory;
            _headerFactory = headerFactory;
            _reqEvaluator = reqEvaluator;
		}

		public IStoryLoader Create(Player player)
		{
            return new StoryLoader(_screenNavManager, player, _sceneFactory, _screenFactory, _headerFactory, _reqEvaluator);
		}

        public IStoryLoaderFacade CreateFacade(Player player)
        {
            return new StoryLoaderFacade(Create(player));
        }
    }
}

