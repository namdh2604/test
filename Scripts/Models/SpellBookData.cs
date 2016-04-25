using System.Collections;
using System.Collections.Generic;

namespace Voltage.Witches.Models
{
	public class SpellBookData : BaseData
	{
		public int display_order;
		public string unlock_point_id;
		public bool available;
		public string book_prize_id;
		public List<string> recipes;
	}
}