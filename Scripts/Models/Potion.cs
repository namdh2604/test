using System;
using System.Collections;
using System.Collections.Generic;

namespace Voltage.Witches.Models
{
	public interface IPotion
	{
		PotionCategory PotionCategory { get; }
		string Name { get; }
		string Description { get; }
		string ColorCode { get; }
		string CategoryID { get; }
		Dictionary<string,int> EffectList { get; }
		PURCHASE_TYPE CurrencyType { get; }
		int Coin_Price { get; }
		int Premium_Price { get; }
		string Icon_Path { get; }
	}

	public class Potion : Item, IPotion 
	{
		public PotionCategory PotionCategory { get; set; }
		public string ColorCode { get; protected set; }
		public Dictionary<string,int> EffectList { get; protected set; }
		public string CategoryID { get; set; }
		public PURCHASE_TYPE CurrencyType { get; set; }
		public int Coin_Price { get; set; }
		public int Premium_Price { get; set; }
		public string Icon_Path { get; set; }

		public Potion(string id, string name, string description, string colorCode, Dictionary<string,int> effectList): base(id)
		{

			base.Category = ItemCategory.POTION;
			base.Name = name;
			base.Description = description;
			if(name != "Stamina Potion")
			{
				ColorCode = colorCode;
				PotionCategory = GetCategory();
				BuildEffectList(effectList);
			}
			else
			{
				EffectList = new Dictionary<string, int>();
				PotionCategory = PotionCategory.STAMINA;
				ColorCode = "8A360F";
			}
		}

		void BuildEffectList(Dictionary<string, int> effectList)
		{
			int effectCount = effectList.Count;
//			UnityEngine.Debug.LogWarning(Name + " COUNT = " + effectCount.ToString());
			EffectList = new Dictionary<string, int>(effectCount);

			foreach(KeyValuePair<string,int> pair in effectList)
			{
				var key = pair.Key;
				var value = pair.Value;

//				UnityEngine.Debug.LogWarning("KEY :: " + key + " VALUE :: " + value.ToString());

				EffectList[key] = value;
			}
		}

		PotionCategory GetCategory()
		{
			if((!Name.Contains("Superior")) && (!Name.Contains("Master")))
			{
				return PotionCategory.BASIC;
			}
			else if((Name.Contains("Superior")) && (!Name.Contains("Master")))
			{
				return PotionCategory.SUPERIOR;
			}
			else
			{
				return PotionCategory.MASTER;
			}
		}
	}
}