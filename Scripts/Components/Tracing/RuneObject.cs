using System;
using System.Collections;
using System.Collections.Generic;

namespace Voltage.Witches.Components
{
	using Texture2D = UnityEngine.Texture2D;

	public class RuneObject
	{
		public string Rune_Name { get; protected set; }
		public int Stroke_Count { get; set; }
		public List<Texture2D> Stroke_Textures { get; set; }
//		public List<bool[]> Stroke_Accuracy_Maps { get; set; }

		public RuneObject(string runeName)
		{
			Rune_Name = runeName;
			Stroke_Textures = new List<Texture2D>();
//			Stroke_Accuracy_Maps = new List<bool[]>();
		}

		public void AddTexture(Texture2D texture)
		{
			if(!Stroke_Textures.Contains(texture)) 
			{
				Stroke_Textures.Add(texture);
			}
		}

//		public void AddAccuracyMap(bool[] accuracyMap)
//		{
//			if(!Stroke_Accuracy_Maps.Contains(accuracyMap))
//			{
//				Stroke_Accuracy_Maps.Add(accuracyMap);
//			}
//		}
	}
}