using System;

namespace Voltage.Witches.Controllers
{
    using Voltage.Witches.Models;
    using Voltage.Story.StoryDivisions;
	using Voltage.Story.StoryPlayer;

	public interface IStoryPlayerDialogController
	{
		void Show (Player player, StoryPlayerSettings settings, SceneHeader header, Action<int> callback);
	}
}
