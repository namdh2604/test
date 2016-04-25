
using System;
using System.Collections.Generic;

namespace Voltage.Witches.Story.StoryPlayer
{
	using Voltage.Common.Logging;

	using Voltage.Story.StoryPlayer;
	using Voltage.Story.StoryDivisions;
    using Voltage.Story.User;
	using Voltage.Story.Models.Nodes;
	using Voltage.Story.Models.Nodes.Controllers;
	using Voltage.Story.Models.Nodes.Extensions;

    public sealed class WitchesStoryPlayer : PersistentStoryPlayer
    {
		public string IDFormat { get; private set; }
        private Action _onFinish;
		
		public WitchesStoryPlayer(IPlayer player, ILogger logger, IDictionary<Type,INodeController> nodeControllers, Action onFinish = null)
            : base(player, logger, nodeControllers, null)
		{
            _onFinish = onFinish;

			IDFormat = "D5";
		}
		
		public bool StartScene(Scene scene, int nodeID)
		{
            OnFinish = HandleSceneComplete;

			if(scene != null)
			{
				INode startNode = GetStartNode (scene, nodeID);
				return StartScene (scene, startNode); 					// GoTo (scene, (node) => Convert.ToInt32 (node.ID) == nodeID, history);
			}
			else
			{
				Logger.Log ("no scene given", LogLevel.WARNING);
			}
			
			return false;
		}

        private void HandleSceneComplete()
        {
            if (_onFinish != null)
            {
                _onFinish();
            }
        }
		
		private INode GetStartNode (Scene scene, int id)
		{
			if(scene != null)
			{
				return scene.FindNodeByID(id.ToString(IDFormat));
			}
			else
			{
				return default(INode);
			}
		}
    }
    
}




