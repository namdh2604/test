using System;
using Voltage.Witches.Models;

namespace Voltage.Witches.Events
{
	public class SceneSelectedEventArgs : GUIEventArgs
	{
        public SceneSelectedEventArgs(SceneViewModel scene)
		{
			Scene = scene;
		}

        public SceneViewModel Scene { get; protected set ; }
	}
}