namespace Voltage.Witches.Models
{
	public class IngredientCategory
	{
		readonly string _id;
		public string Id { get { return _id; } }

		readonly string _name;
		public string Name { get { return _name; } }

		public IngredientCategory(string id, string name)
		{
			_id = id;
			_name = name;
		}

		public override bool Equals(System.Object obj)
		{
			if(obj == null)
			{
				return false;
			}

			IngredientCategory candidateCat = obj as IngredientCategory;
			if((System.Object)candidateCat == null)
			{
				return false;
			}

			if((candidateCat.Name != this.Name) && (candidateCat.Id != this.Id))
			{
				return false;
			}

			return true;
		}

		public override int GetHashCode()
		{
			return this.GetHashCode();	
		}
	}
}

