using System.Collections.Generic;

namespace Voltage.Witches.Configuration
{
	public class SpellbookRefConfig
	{
		public string Id { get; protected set; }
		public string Name { get; set; }
		public int Display_Order { get; set; }
		public string Unlock_Point_ID { get; set; }
		public bool IsAvailable { get; set; }
		public string Book_Prize_ID { get; set; }
		public List<string> Recipes { get; set; }

		public SpellbookRefConfig(string id)
		{
			Id = id;
			Recipes = new List<string>();
		}
	}
}