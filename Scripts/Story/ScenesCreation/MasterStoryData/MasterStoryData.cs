
using System;
using System.Collections.Generic;

namespace Voltage.Story.Configurations
{
	using Voltage.Witches.Models;
	using TermLevel = Voltage.Story.StoryDivisions.Scene.TermLevel;

    public class MasterStoryData
    {
		public IDictionary<string, string> SceneDescriptions { get; set; }
		public IDictionary<string, TermLevel> SceneTerminationLevels { get; set; }
		public IList<NamedSelectionData> SceneNamedSelections { get; set; }
		public IDictionary<string, string> PreviewImages { get; set; }

		public IDictionary<string, string> SceneToFileMap { get; set; }
        public IDictionary<string, string> MusicMap { get; set; }

		public List<NPCModel> NPCs = new List<NPCModel>	// TODO: parse list from JSON
		{
			new NPCModel("A", "Anastasia", "Petrova"),
			new NPCModel("R", "Rhys", "Ceridwen"),
			new NPCModel("T", "Tyrone", "Bryant", "Ty"),
			new NPCModel("M", "Melanie", "Harris"),
			new NPCModel("N", "Niklas", "von Reylander"),
		};

		public IEnumerable<ArcData> Arcs = new List<ArcData>
		{
			new ArcData (0, "Prologue", "werbury", "Werbury"),
			new ArcData (1, "Ireland", "ireland"),
			new ArcData (2, "Germany", "germany"),
			new ArcData (3, "Prague", "prague"),
			new ArcData (4, "Salem", "salem"),
		};

    }





    
}




