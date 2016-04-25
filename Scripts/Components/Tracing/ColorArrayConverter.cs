using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Voltage.Witches.Components
{
	public class ColorArrayConverter
	{
		public Color32 TrueValue { get; protected set; }		// Colour True: RGBA(0, 255, 0, 255), False: RGBA(218, 5, 15, 255)
		public Color32 FalseValue { get; protected set; }

		public ColorArrayConverter(Color32 trueElement,Color32 falseElement)
		{
			TrueValue = trueElement;
			FalseValue = falseElement;
		}

		public bool[] ConvertColorArrayToBoolArray(Color32[] inputArray)
		{
			var length = inputArray.Length;
			bool[] outputArray = new bool[length];

			for(int i = 0; i < length; ++i)
			{
				var currentPixel = inputArray[i];
				if(currentPixel.Equals(TrueValue))
				{
					outputArray.SetValue(true,i);
				}
				else
				{
					outputArray.SetValue(false,i);
				}
			}

			return outputArray;
		}
	}
}