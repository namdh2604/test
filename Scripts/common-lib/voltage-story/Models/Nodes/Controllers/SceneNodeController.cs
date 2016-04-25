namespace Voltage.Story.Models.Nodes.Controllers
{
	using Voltage.Story.StoryPlayer;

	public class SceneNodeController : INodeController
	{
		public SceneNodeController()
		{
		}

		public void Execute(INode node, IStoryPlayer storyPlayer)
		{
			storyPlayer.Next();
		}
	}
}

