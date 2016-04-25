using System.Collections.Generic;

namespace Voltage.Witches.Models
{
    using Voltage.Story.StoryDivisions;
	using Voltage.Story.Configurations;

	public class CountryArc 
	{
		public int Id { get; protected set; }
		public string Name { get; protected set; }
		public bool isAvailable { get; protected set; }
		public int SceneCount { get; protected set; }
        public List<SceneHeader> AvailableScenes { get; protected set; }
		public ArcData Arc { get; protected set; }

		public CountryArc(ArcData arc)
		{
			Arc = arc;
			Id = Arc.Order;
			Name = Arc.Country;

            AvailableScenes = new List<SceneHeader>();
			SceneCount = AvailableScenes.Count;
		}

        public void SetUpScenes(List<SceneHeader> scenes)
		{
            AvailableScenes = scenes;

			SceneCount = AvailableScenes.Count;
		}

		public void UnlockCountry()
		{
			isAvailable = true;
		}
	}
}