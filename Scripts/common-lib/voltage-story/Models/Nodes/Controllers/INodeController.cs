using Voltage.Story.StoryPlayer;

namespace Voltage.Story.Models.Nodes.Controllers
{
	public interface INodeController
	{
		void Execute(INode node, IStoryPlayer storyPlayer);
	}
}
