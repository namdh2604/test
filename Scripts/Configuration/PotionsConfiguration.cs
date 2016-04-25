using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Voltage.Witches.Models;

namespace Voltage.Witches.Configuration
{
	public class PotionsConfiguration
	{
		public Dictionary <string, PotionData> Potions_Dictionary { get; set; }

		public PotionsConfiguration()
		{
			Potions_Dictionary = new Dictionary<string,PotionData>();
		}
	}
}
