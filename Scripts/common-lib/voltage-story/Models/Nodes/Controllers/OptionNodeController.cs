namespace Voltage.Story.Models.Nodes.Controllers
{
	using Voltage.Story.StoryPlayer;
	using Voltage.Story.Effects;

	public class OptionNodeController : BaseNodeController 
	{
        public OptionNodeController(IEffectResolver resolver)
		{
		}

		public override void Execute(INode node, IStoryPlayer storyPlayer)
		{
			storyPlayer.Next ();	// TODO: support previous
		}
	}
}




