using System.Collections;
using Voltage.Witches.Models;

namespace Voltage.Witches.Configuration
{
	public class AffinityConfiguration
	{
		public string Id { get; protected set; }
		public string Name { get; set; }
		public int Grade { get; set; }
		public int TotalAffinity { get; set; }

		public AffinityConfiguration(string id)
		{
			Id = id;
		}
	}
}
