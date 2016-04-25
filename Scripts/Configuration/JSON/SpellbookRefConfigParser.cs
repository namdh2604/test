using Voltage.Witches.Models;
using System.Collections.Generic;

namespace Voltage.Witches.Configuration.JSON
{
	public interface ISpellbookRefConfigParser
	{
		SpellbookRefConfig CreateRef(SpellBookData spellbookData);
	}

	public class SpellbookRefConfigParser : ISpellbookRefConfigParser
	{
		public SpellbookRefConfigParser()
		{
		}

		public SpellbookRefConfig CreateRef(SpellBookData spellbookData)
		{
			SpellbookRefConfig refConfig = new SpellbookRefConfig(spellbookData.id);
			refConfig.Name = spellbookData.name;
			refConfig.Display_Order = spellbookData.display_order;
			refConfig.Book_Prize_ID = spellbookData.book_prize_id;
			refConfig.IsAvailable = spellbookData.available;
			refConfig.Recipes = spellbookData.recipes;
			refConfig.Unlock_Point_ID = spellbookData.unlock_point_id;

			return refConfig;
		}
	}
}