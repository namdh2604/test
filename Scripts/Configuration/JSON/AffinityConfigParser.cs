using System;
using System.Collections;
using System.Collections.Generic;
using Voltage.Witches.Configuration;

namespace Voltage.Witches.Configuration.JSON
{
	using Voltage.Witches.Models;

	public interface IAffinityConfigParser
	{
		AffinityConfiguration Create(AffinityData data);
	}

	public class AffinityConfigParser : IAffinityConfigParser
	{
		private static List<string> _gradeIndex = new List<string>()
		{
			"n/a","Sphere 1","Sphere 2","Sphere 3","Sphere 4","Sphere 5","Sphere 6","Sphere 7","Sphere 8","Sphere 9"
		};

		public AffinityConfigParser()
		{
		}

		public AffinityConfiguration Create(AffinityData data)
		{
			AffinityConfiguration affinityConfig = new AffinityConfiguration(data.id);
			affinityConfig.Name = data.name;
			affinityConfig.Grade = _gradeIndex.IndexOf(data.grade);
			affinityConfig.TotalAffinity = data.total_affinity;

			return affinityConfig;
		}
	}

}