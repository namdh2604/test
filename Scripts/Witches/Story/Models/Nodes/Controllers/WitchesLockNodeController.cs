
using System;
using System.Collections.Generic;

namespace Voltage.Witches.Story.Models.Nodes.Controllers
{
	using Voltage.Common.Logging;
	using Voltage.Story.StoryPlayer;
	using Voltage.Witches.Models;
	using Voltage.Story.Models.Nodes;
	using Voltage.Story.Models.Nodes.Controllers;
	
	using Voltage.Story.StoryDivisions;
	using Scene = Voltage.Story.StoryDivisions.Scene;
	
	
	public class WitchesLockNodeController : BaseNodeController
	{
		public Player Player { get; private set; }
//		public ISceneHeaderFactory SceneHeaderFactory { get; private set; }

//		public WitchesLockNodeController (Player player, ISceneHeaderFactory sceneHeaderFactory, ILogger logger) : base (logger)
		public WitchesLockNodeController (Player player, ILogger logger) : base (logger)
		{
			Player = player;
//			SceneHeaderFactory = sceneHeaderFactory;
		}
		
		public override void Execute(INode node, IStoryPlayer storyPlayer)
		{
            LockNode lockNode = node as LockNode;
            string path = Scene.CreateScenePath(lockNode.Route, lockNode.Arc, lockNode.Scene, lockNode.Version);
            Player.RemoveScene(path);
            storyPlayer.Next();
		}
		
		

		
	}
    
}







