using Voltage.Witches.Screens;

namespace Voltage.Witches.Controllers
{
    using Voltage.Witches.Models;
    using Voltage.Story.StoryDivisions;
	using Voltage.Story.StoryPlayer;

	public class NormalStoryPlayerDialogController : IStoryPlayerDialogController
	{
		private readonly IScreenFactory _factory;
		private bool _isInTutorial;

        public NormalStoryPlayerDialogController (IScreenFactory factory, bool isInTutorial=false)
		{
			_factory = factory;
			_isInTutorial = isInTutorial;
		}

		public void Show (Player player, StoryPlayerSettings settings, SceneHeader header, System.Action<int> callback)
		{
			var dialog = _factory.GetDialog<ChapterClearDialog> ();
            dialog.SetParameters(player);
            dialog.SetHeader(header);
			if (_isInTutorial) 
			{
				dialog.DisableInputs();
				dialog.ShowPointer (new UnityEngine.Vector2 (0.89f, 0.8f), true);
				dialog.ActivateCheckMailButton();
			}

			bool displayMailButton = CheckForMailButtonDisplay(settings);
			dialog.EnableCheckMailButton (displayMailButton);

			dialog.Display (callback);
		}

		private bool CheckForMailButtonDisplay(StoryPlayerSettings settings)
		{
			return _isInTutorial || settings.MailOnComplete;
		}

	}
}