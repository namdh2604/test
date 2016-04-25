using System;

namespace Voltage.Witches.Controllers.Factories
{
    using Voltage.Witches.Screens;
    using Voltage.Witches.Services;

	using Voltage.Witches.DI;

    public class MainLoadingScreenControllerFactory
    {
        private readonly IScreenFactory _factory;
        private readonly IBuildNumberService _versionService;

		private readonly RestorePlayerController _restorePlayerController;

        public MainLoadingScreenControllerFactory(IScreenFactory factory, IBuildNumberService versionService, RestorePlayerController restorePlayerController)
        {
            _factory = factory;
            _versionService = versionService;

			_restorePlayerController = restorePlayerController;
        }

        public MainLoadingScreenController Create(int mainLayout, Action callback1, Action callback2)
        {
            return new MainLoadingScreenController(_factory, _versionService, mainLayout, callback1, callback2, _restorePlayerController);
        }
    }
}

