namespace Voltage.Witches.Models
{
	public class Item
	{
		public string Id { get; protected set; }
		public string Name { get; set; }
		public int Display_Order { get; set; }
		public ItemCategory Category { get; set; }
		public string Description { get; set; }

		public Item(string id)
		{
			Id = id;
		}
	}
}

