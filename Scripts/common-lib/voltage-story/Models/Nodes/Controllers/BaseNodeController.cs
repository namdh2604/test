using Voltage.Common.Logging;
using Voltage.Story.StoryPlayer;

namespace Voltage.Story.Models.Nodes.Controllers
{
	// TODO: Remove -- doesn't add significant functionality over the INodeController interface
	public abstract class BaseNodeController : INodeController
	{
		public ILogger Logger { get; private set; }
		
		public BaseNodeController() : this(null) {}
		public BaseNodeController (ILogger logger)
		{
			Logger = logger;
		}
		
		
//		public virtual void Execute(INode node, IStoryPlayer storyPlayer, Action<INode> callback) {}	
		public virtual void Execute(INode node, IStoryPlayer storyPlayer) {}
	}
    
}




