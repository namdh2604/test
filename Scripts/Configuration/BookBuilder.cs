using Voltage.Witches.Models;

namespace Voltage.Witches.Configuration
{
	public interface IBookBuilder
	{
//		Spellbook CreateBook(int bookId, IPlayer player);
	}

	public class BookBuilder : IBookBuilder
	{
		IReferenceConfig _config;

		public BookBuilder(IReferenceConfig config)
		{
//			_config = config;
		}

//		public Spellbook CreateBook(int bookId, IPlayer player)
//		{
//			SpellbookRef bookData = _config.GetBook(bookId);
//			Spellbook book = new Spellbook(bookData.Name);
//			foreach (int spellId in bookData.Spells)
//			{
//				SpellRef spellData = _config.GetSpell(spellId);
//				Spell spell = new Spell(spellData.Name);
//				if (player.CompletedSpells.Contains(spellId))
//				{
//					spell.IsComplete = true;
//				}
//				book.AddSpell(spell);
//			}
//			return book;
//		}
	}
}

