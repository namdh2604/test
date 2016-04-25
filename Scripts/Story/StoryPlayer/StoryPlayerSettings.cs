
using System;
using System.Collections.Generic;

namespace Voltage.Story.StoryPlayer
{
	using Voltage.Story.DebugTools;
	using Voltage.Story.StoryDivisions;
	using Voltage.Story.Models.Nodes;

//	using TermLevel = Voltage.Story.StoryDivisions.Scene.TermLevel;

	public class StoryPlayerSettings	// FIXME: This will likely change
	{
		public Scene Scene { get; private set; }
		public int Node { get; private set; }							// TODO: should be string?
//		public TermLevel TerminationLevel { get; private set; }
//		public IEnumerable<INode> History;	

		public string SceneName { get { return Scene.Name; } }
		public string ArcName { get { return Scene.Arc; } }
		public string RouteName { get { return Scene.Route; } }

		public string ScenePath { get { return Scene.Path; } }

		public bool MailOnComplete { get; set; }


		public StoryPlayerSettings (Scene scene, int nodeID=0) //, TermLevel terminationLvl=TermLevel.None, bool mailOnComplete=false
		{
			if(scene == null)
			{
				throw new NullReferenceException("Scene is null, the scene cannot be null");
			}

			Scene = scene;
			Node = nodeID;

			MailOnComplete = false;
		}
	}

    
}




