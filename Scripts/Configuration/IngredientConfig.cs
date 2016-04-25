namespace Voltage.Witches.Configuration
{
	public class IngredientConfig
	{
		public string ItemId { get; protected set; }
		public string Category { get; set; }
		public int Value { get; set; }
		public bool IsInfinite { get; set; }

		public IngredientConfig(string id)
		{
			ItemId = id;
			IsInfinite = false;
		}
	}
}

