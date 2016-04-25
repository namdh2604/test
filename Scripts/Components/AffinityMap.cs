using System;
using System.Collections;
using System.Collections.Generic;
using Voltage.Witches.Lib;
using Voltage.Witches.Models;

namespace Voltage.Witches.Components
{
	using System.Linq;

	public class AffinityMap
	{
		private readonly List<Affinity> _sortedAffinities;

		public IList<Affinity> SortedAffinityMap
		{
			get { return _sortedAffinities.AsReadOnly(); }
		}

		public AffinityMap(Affinity[] affinityData)
		{
			if(affinityData == null || affinityData.Length == 0) 
			{
				throw new System.ArgumentNullException();
			}

//			_sortedAffinities = new List<Affinity> (affinityData.OrderByAscending(a => a.CurrentAffinityScore));	// IOrderedEnumerable...also check against iOS deploy
			_sortedAffinities = new List<Affinity> (affinityData);
			_sortedAffinities.Sort ((a,b) => a.CurrentAffinityScore.CompareTo (b.CurrentAffinityScore));
		}

		public override bool Equals(System.Object obj)
		{
			if(obj == null)
			{
				return false;
			}
			
			AffinityMap candidateMap = obj as AffinityMap;
			if(candidateMap == null)
			{
				return false;
			}
			
			return _sortedAffinities.SequenceEqual (candidateMap.SortedAffinityMap);	
		}
		
		public override int GetHashCode()
		{
			return this.GetHashCode();
		}


		public Affinity GetAffinityFromScore(int currentAffinity)
		{
			Affinity affinity = _sortedAffinities[0];

			int i=1;
			while(NotInRange(currentAffinity, i))		// NOTE: not performant when score reaches the upper range of affinities
			{
				affinity = _sortedAffinities[i++];
			}

			return affinity;
		}


		private bool NotInRange(int score, int rangeIndex) // Affinity affinity)
		{
			return (rangeIndex < _sortedAffinities.Count) && (score >= _sortedAffinities [rangeIndex].CurrentAffinityScore);
		}
	}
}



