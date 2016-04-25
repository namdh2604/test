using System;
using System.Collections.Generic;

namespace Voltage.Witches.Story.Variables
{
	using Voltage.Common.Logging;
	using Voltage.Witches.Models;
	
	using StoryPlayer;
	using Voltage.Story.Models.Nodes;
	using Voltage.Story.Models.Nodes.Extensions;
	using Voltage.Story.Variables;

	using Voltage.Story.User;
	using Voltage.Story.StoryDivisions;
	
	using Voltage.Story.Mapper;

	using Voltage.Story.Configurations;
    using Voltage.Witches.Models.Avatar;

	public sealed class WitchesVariableMapper : VariableMapper
	{
		public Player Player { get; private set; }

		public WitchesVariableMapper (Player player, MasterStoryData storyData, IEnumerable<NPCModel> npcs, ILogger logger) : base (logger)
		{
			if(player == null || storyData == null)
			{
				throw new ArgumentNullException();
			}

			Player = player;
		
			AddSelectionVariables (storyData);
			AddMainCharacterVariables ();
			AddCharacterAffinityVariables (npcs);
            AddClothingVariables();
		}


		private void AddSelectionVariables(MasterStoryData storyData)	
		{
			foreach (NamedSelectionData selectionData in storyData.SceneNamedSelections)
			{
				string key = string.Format("Selections/{0}/{1}", selectionData.Path, selectionData.Name);

				NamedSelectionData dataRef = selectionData;
				Map[key] = (() => Player.GetSelectionChoice(dataRef.Path, dataRef.Name));
			}
		}


		private void AddMainCharacterVariables()
		{
			if(Player != null)
			{
				Map.Add ("MC/First", () => Player.FirstName);		
				Map.Add ("MC/Last", () => Player.LastName);			
			}
			else
			{
				Logger.Log ("WitchesVariableMapper::AddMainCharacterVariables >>> Could not add Player variables", LogLevel.WARNING);
			}
		}

        private void AddClothingVariables()
        {
            foreach (OutfitCategory category in Enum.GetValues(typeof(OutfitCategory)))
            {
                OutfitCategory currentCategory = category; // local variable to deal with closures
                Map.Add(category.ToString(), () => GetClothing(currentCategory));
            }
        }

        private string GetClothing(OutfitCategory category)
        {
            Outfit outfit = Player.GetOutfit();
            return outfit.GetWornItem(category);
        }


		
		private void AddCharacterAffinityVariables (IEnumerable<NPCModel> characters)
		{
			if(Player != null && characters != null) 
			{
				foreach (NPCModel character in characters)
				{
                    // TODO: Remove full name entry -- this is due to an error with composer's design
                    // Currently, composer enters character names as full names, when it should use the shorthand.
                    // When this is fixed, the full name entry should be removed.
                    AddFullNameCharacterEntry(character);
                    AddNormalCharacterEntry(character);
				}
			}
			else
			{
				Logger.Log ("WitchesVariableMapper::AddCharacterAffinityVariables >>> Could not add character affinity variables", LogLevel.WARNING);
			}
		}

        private void AddFullNameCharacterEntry(NPCModel character)
        {
            CreateCharacterMapEntry(character.FullName, character.ID);
        }

        private void AddNormalCharacterEntry(NPCModel character)
        {
            CreateCharacterMapEntry(character.Shorthand, character.ID);
        }

        private void CreateCharacterMapEntry(string charName, string charID)
        {
            Map.Add(string.Format("Characters/{0}/Affinity", charName), CreateAffinityHandler(charID));
        }
		
		private Func<object> CreateAffinityHandler(string charID)
		{
			return () => Player.GetAffinity(charID);
		}
		
		


		// TODO: Outfits



		
		
	}
}







