using System;
using System.Collections;
using Voltage.Witches.Models;

namespace Voltage.Witches.Configuration
{
	public class ItemConfiguration
	{
		public string Id { get; protected set; }
		public string CategoryID { get; set; }
		public ItemCategory ItemCategory { get; set; }
		public object Item { get; set;}                 // this is actually of type BaseData, NOT Item!

		public ItemConfiguration(string id)
		{
			Id = id;
		}
	}
}