using System.Collections;
using System.Collections.Generic;
using Voltage.Witches.Models;
using Voltage.Witches.Configuration;

namespace Voltage.Witches.Configuration.JSON
{
	using Newtonsoft.Json;
	using Newtonsoft.Json.Linq;

	public interface IBooksConfigParser
	{
		BooksConfiguration Construct(string json);
		BooksConfiguration Construct(List<SpellBookData> data);
	}

	public class BooksConfigParser : IBooksConfigParser
	{
		ISpellbookRefConfigParser _spellbookrefConfigParser;

		public BooksConfigParser(ISpellbookRefConfigParser spellbookRefConfigParser)
		{
			_spellbookrefConfigParser = spellbookRefConfigParser;
		}

		public BooksConfiguration Construct(List<SpellBookData> data)
		{
			BooksConfiguration booksConfig = new BooksConfiguration();

			if(data != null)
			{
				for(int i = 0; i < data.Count; ++i)
				{
					var currentBook = data[i];
					booksConfig.Books_Reference[currentBook.id] = _spellbookrefConfigParser.CreateRef(currentBook);
	//				Spellbook book = GetSpellbook(booksConfig.Books_Reference[currentBook.id]);
					booksConfig.Books_Index[currentBook.display_order] = booksConfig.Books_Reference[currentBook.id];
				}
			}

			return booksConfig;
		}

//		private Spellbook GetSpellbook(SpellbookRefConfig config)
//		{
//			Spellbook book = new Spellbook(config.Name);
//			book.IsAccessible = false;
//			
//			foreach (string recipeId in config.Recipes)
//			{
//				RecipeReference recipeConfig = _recipesConfig.Recipes[recipeId];
//				book.AddRecipe(
//			}
//		}

		public BooksConfiguration Construct(string json)
		{
			BooksConfiguration booksConfig = new BooksConfiguration();
			JObject jsonObject = JObject.Parse(json);
			SpellbookMasterData booksMaster = JsonConvert.DeserializeObject<SpellbookMasterData>(jsonObject.ToString());

			for(int i = 0; i < booksMaster.book_mst.Count; ++i)
			{
				var currentBook = booksMaster.book_mst[i];
				booksConfig.Books_Reference[currentBook.id] = _spellbookrefConfigParser.CreateRef(currentBook);
			}

			return booksConfig;
		}
	}
}