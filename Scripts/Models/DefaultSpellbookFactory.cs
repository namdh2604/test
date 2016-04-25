
using System;
using System.Collections.Generic;

namespace Voltage.Witches.Factory
{
	using Voltage.Story.General;

	using Voltage.Witches.Models;
	using Voltage.Witches.Configuration;

	public class DefaultSpellbookFactory : IFactory<string,ISpellbook>
    {
		private readonly BooksConfiguration _bookConfig;
//		private readonly IFactory<string,IRecipe> _recipeFactory;
//		private readonly IDictionary<string,BookPrizeData> _prizeConfig;
//
//		public DefaultSpellbookFactory (BooksConfiguration bookConfig, IFactory<string,IRecipe> recipeFactory, IDictionary<string,BookPrizeData> prizeConfig)	
//		{
//			if(bookConfig == null || recipeFactory == null || prizeConfig == null)
//			{
//				throw new ArgumentNullException();
//			}
//
//			_bookConfig = bookConfig;
//			_recipeFactory = recipeFactory;
//			_prizeConfig = prizeConfig;
//		}
//
//
//		public ISpellbook Create(string bookID)
//		{
//			if(!string.IsNullOrEmpty(bookID))
//			{
//				SpellbookRefConfig bookConfig = _bookConfig.Books_Reference[bookID];
//
//				return CreateBookFromConfig (bookConfig);
//			}
//			else
//			{
//				throw new ArgumentNullException("DefaultSpellbookFactory::Create >>> bookID is null/empty");
//			}
//		}
//
//
//		private Spellbook CreateBookFromConfig (SpellbookRefConfig bookConfig)
//		{
//			Spellbook spellbook = new Spellbook (bookConfig.Name, bookConfig.Id)
//			{
//				IsAccessible = bookConfig.IsAvailable,
//			};
//			
//			foreach (string recipeID in bookConfig.Recipes)
//			{
//				IRecipe recipe = _recipeFactory.Create(recipeID);
//				spellbook.AddRecipe(recipe);
//			}
//
//			spellbook.ClearItems = new List<KeyValuePair<Item,int>> { GetPrize(bookConfig) };
//			
//			return spellbook;
//		}
//
//		private KeyValuePair<string,int> GetPrize(SpellbookRefConfig bookConfig)
//		{
//			KeyValuePair<string,int> prize = new KeyValuePair<string,int> ();
//
//			BookPrizeData prizeData = _prizeConfig [bookConfig.Book_Prize_ID];
//			if(IsValidPrize(prizeData.type))
//			{
//				// TODO: need to evaluate what needs to happen here
//			}
//
//			return prize;
//		}
//
//		private bool IsValidPrize (string type)
//		{
//			return ((type == "ingredient") || (type == "avatar"));
//		}

		private readonly ISpellbookFactoryNew _bookFactory;

		public DefaultSpellbookFactory(BooksConfiguration bookConfig, ISpellbookFactoryNew bookFactory)
		{
			if(bookConfig == null || bookFactory == null)
			{
				throw new ArgumentNullException();
			}

			_bookConfig = bookConfig;
			_bookFactory = bookFactory;
		}

		public ISpellbook Create(string bookID)
		{
			if(!string.IsNullOrEmpty(bookID))
			{
				SpellbookRefConfig bookConfig = _bookConfig.Books_Reference[bookID];

				ISpellbook spellbook = _bookFactory.Create(bookConfig);
				return spellbook;
			}
			else
			{
				throw new ArgumentNullException("DefaultSpellbookFactory::Create >>> bookID is null/empty");
			}
		}


    }
    
}




