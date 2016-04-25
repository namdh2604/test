using Voltage.Witches.Screens;
using UnityEngine;

namespace Voltage.Witches.Controllers
{
    using Voltage.Witches.Models;
    using Voltage.Story.StoryDivisions;
	using Voltage.Story.StoryPlayer;

	public class TutorialDialogController : IStoryPlayerDialogController
	{
		private readonly IScreenFactory _factory;

		public TutorialDialogController (IScreenFactory factory)
		{
			_factory = factory;
		}

		public void Show (Player player, StoryPlayerSettings settings, SceneHeader header, System.Action<int> callback)
		{
			var dialog = _factory.GetDialog<TutorialCompleteSceneDialogIGUI> (); //TODO needs to be replaced with TutorialCOmpleteSceneDialog (UGUI)
			dialog.Display (callback);
		}
	}
}