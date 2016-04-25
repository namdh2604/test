using UnityEngine;
using System;
using System.Collections.Generic;

namespace Voltage.Common.IAP.Tools
{

    public class IAPTools : MonoBehaviour
    {

		// http://stackoverflow.com/a/11716141/4339956
		public static string StringTransformXOR(String s, int i) 
		{
			char[] chars = s.ToCharArray();
			for(int j = 0; j<chars.Length; j++)
			{
				chars[j] = (char)(chars[j] ^ i);
			}
			return new string(chars);
		}


    }

}


