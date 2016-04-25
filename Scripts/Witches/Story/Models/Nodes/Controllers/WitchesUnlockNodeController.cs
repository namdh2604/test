
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

	
	public class WitchesUnlockNodeController : BaseNodeController
	{
		public Player Player { get; private set; }
//		public ISceneHeaderFactory SceneHeaderFactory { get; private set; }

//		public WitchesUnlockNodeController (Player player, ISceneHeaderFactory sceneHeaderFactory, ILogger logger) : base (logger)
		public WitchesUnlockNodeController (Player player, ILogger logger) : base (logger)
		{
			Player = player;
//			SceneHeaderFactory = sceneHeaderFactory;
		}
		
		public override void Execute(INode node, IStoryPlayer storyPlayer)
		{
			UnlockNode unlockNode = node as UnlockNode;

            string path = Scene.CreateScenePath(unlockNode.Route, unlockNode.Arc, unlockNode.Scene, unlockNode.Version);

            Player.AddAvailableScene(path);

            storyPlayer.Next();
		}



		
	}
	
}




