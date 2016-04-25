using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Voltage.Witches.Models
{
	public class BooksData 
	{
		//TODO There will need to be an update to this to account for recipe completion stage
		public List<PlayerRecipeData> recipes;
		public string id;
		public bool is_complete;
	}
}