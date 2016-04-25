


namespace Voltage.Story.Models.Nodes.Controllers
{
	using Voltage.Story.StoryPlayer;

	public class BranchNodeController : BaseNodeController 
	{	
		public override void Execute(INode node, IStoryPlayer storyPlayer)
		{
			storyPlayer.Next ();	// TODO: support previous
		}
	}
    
}




