using System.Collections;
using System.Collections.Generic;
using Voltage.Witches.Models;

namespace Voltage.Witches.Configuration
{

	public class BooksConfiguration 
	{
		public Dictionary<string,SpellbookRefConfig> Books_Reference { get; set; }
		public Dictionary<int,SpellbookRefConfig> Books_Index { get; set; }

		public Dictionary<string,string> BookUnlockMap { get; set; }

		public BooksConfiguration()
		{
			Books_Reference = new Dictionary<string,SpellbookRefConfig>();
			Books_Index = new Dictionary<int,SpellbookRefConfig>();

			BookUnlockMap = new Dictionary<string, string> ();
		}
	}
}