using System;
using System.Collections.Generic;


namespace Voltage.Story.StoryPlayer
{
	using Voltage.Story.StoryDivisions;
	using Voltage.Story.Models.Nodes;

	public interface IStoryPlayer	// FIXME: this interface has become too large
	{
		void Next();
		void Next(int index);		// FIXME: should this be included?
		void Previous();
		
		Scene CurrentScene { get; }	// TODO: should this be in the interface? REMOVE!!!
		INode CurrentNode { get; }	// TODO: should this be in the interface? REMOVE!!!

		bool StartScene (Scene scene, INode startNode);	// FIXME: is this good enough? pass in starting INode?
	}
}



