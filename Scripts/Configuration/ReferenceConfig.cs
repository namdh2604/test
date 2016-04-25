using System;
using SimpleJSON;
using Voltage.Witches.Models;
using Voltage.Witches.Util;
using System.Reflection;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace Voltage.Witches.Configuration
{
	public interface IReferenceConfig
	{
		IEnumerable<SpellbookRef> GetBooks();
		SpellbookRef GetBook(int id);
		SpellRef GetSpell(int id);
	}

	public class ReferenceConfig : IReferenceConfig
	{
		JSONNode _json;

		public ReferenceConfig(string rawData)
		{
			_json = SimpleJSON.JSON.Parse(rawData);
		}

		public IEnumerable<SpellbookRef> GetBooks()
		{
			List<SpellbookRef> books = new List<SpellbookRef>();
			IEnumerable<JSONNode> inputList = _json["books"].AsArray.Cast<JSONNode>();
			inputList.Each((book, index) => books.Add(GetBook(index)));

			return books;
		}

		public SpellbookRef GetBook(int id)
		{
//			JSONNode bookNode = _json["books"][id];
//			string name = bookNode["name"].Value;
//			List<int> spells = new List<int>();
//			foreach (JSONNode spellNode in bookNode["spells"].AsArray)
//			{
//				spells.Add(spellNode.AsInt);
//			}

			//SpellbookRef book = new SpellbookRef(name, spells);
			return null;
			//return book;
		}

		public SpellRef GetSpell(int id)
		{
			string name = _json["spells"][id].Value;
			return new SpellRef(name);
		}
	}
}
