using System;
using System.Collections.Generic;
using System.Collections;

namespace Voltage.Witches.Login
{
	using Newtonsoft.Json;


	public struct BonusItem 
	{
		[JsonProperty(PropertyName="id")]
		public string ID { get; set; }

		[JsonProperty(PropertyName="qty")]
		public int Quantity { get; set; }
	}

	// maybe replace with ItemViewModel
    public struct BonusItemViewModel
    {
        public string Name { get; set; }
		public int Quantity { get; set; }
        public string IconPath { get; set; }

//        public bool Received { get; set; }
//      public bool Special { get; set; }
    }
}