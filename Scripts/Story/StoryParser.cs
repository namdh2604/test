using System.Collections.Generic;
using Voltage.Story.StoryDivisions;
using Voltage.Story.Models.Nodes;
using Voltage.Story.Models.Nodes.Helpers;

namespace Voltage.Witches.Story
{
	public class StoryParser
	{
		public StoryParser()
		{
		}

		public IEnumerable<string> GetAllCharacters(Scene scene)
		{
			HashSet<string> uniqueCharacters = new HashSet<string>();

			// HACK - Hung Nguyen
			// This is to let the loading bar know we are starting another stage of loading.
			// This is where we are going to get what character is needed.  In the editor you won't see a load time 
			// but should be visible on device.
			Voltage.Witches.DI.WitchesStartupSequence.loadingStage++;

			foreach (INode node in scene)
			{
				if (node is IHasCharacters)
				{
					IHasCharacters charNode = node as IHasCharacters;
					AddDisplayableCharacter(charNode.LeftCharacter, uniqueCharacters);
					AddDisplayableCharacter(charNode.RightCharacter, uniqueCharacters);
				}

				if (node is IHasSpeaker)
				{
					IHasSpeaker speakingNode = node as IHasSpeaker;
					AddSpeakingCharacter(speakingNode.Speaker, uniqueCharacters);
				}
			}
			return uniqueCharacters;
		}

		private void AddDisplayableCharacter(CharacterAttribute character, HashSet<string> existingChars)
		{
			if ((character != null) && (!string.IsNullOrEmpty(character.Name)))
			{
				existingChars.Add(character.Name);
			}
		}

		private void AddSpeakingCharacter(string speaker, HashSet<string> existingChars)
		{
			if (!string.IsNullOrEmpty(speaker))
			{
				existingChars.Add(speaker);
			}
		}
	}
}

