
using System;

namespace Unit.Witches.Components
{
    using NUnit.Framework;

	using Voltage.Witches.Components;
	using Voltage.Witches.Models;

    [TestFixture]
    public class TestAffinityMap
    {
		private Affinity[] _affinities = { new Affinity ("A", 0, 0), new Affinity ("C", 200, 200), new Affinity ("B", 100, 100) };

        [Test]
        public void Constructor_ValidRange()
        {
			var map = new AffinityMap (_affinities);

			Assert.That (map, Is.InstanceOf<AffinityMap> ());
        }

		[Test]
		public void Constructor_EmptyArray_Invalid()
		{	
			Affinity[] emptyArray = {};

			Assert.Throws<ArgumentNullException> (() => new AffinityMap (emptyArray));
		}

		[Test]
		public void Constructor_NullArray_Invalid()
		{	
			Assert.Throws<ArgumentNullException> (() => new AffinityMap (null));
		}



		[Test]
		public void AffinitiesAreSorted()
		{	
			var map = new AffinityMap (_affinities);

			Assert.That (map.SortedAffinityMap.Count, Is.EqualTo (3));
			Assert.That (map.SortedAffinityMap [0].AffinityTitle, Is.StringMatching ("A"));
			Assert.That (map.SortedAffinityMap [1].AffinityTitle, Is.StringMatching ("B"));
			Assert.That (map.SortedAffinityMap [2].AffinityTitle, Is.StringMatching ("C"));
		}


		[Test]
		public void MapsAreEqual()
		{
			var mapA = new AffinityMap (_affinities);
			var mapB = new AffinityMap (_affinities);

			Assert.That (mapA, Is.EqualTo (mapB));
		}


		[Test]
		public void GetAffinity_ForSingleAffinity()
		{
			var map = new AffinityMap(new Affinity[]{new Affinity("A",0,0)});
			
			Affinity affinity = map.GetAffinityFromScore (200);
			
			Assert.That (affinity.AffinityTitle, Is.StringMatching("A"));
		}


		[Test]
		public void GetAffinity_WhenOutOfRangeOfLowerBound()
		{
			var map = new AffinityMap(_affinities);
			
			Affinity affinity = map.GetAffinityFromScore (-1);
			
			Assert.That (affinity.AffinityTitle, Is.StringMatching("A"));
		}


		[Test]
		public void GetAffinity_ForLowerBounds()
		{
			var map = new AffinityMap(_affinities);

			Affinity affinity = map.GetAffinityFromScore (0);

			Assert.That (affinity.AffinityTitle, Is.StringMatching("A"));
		}

		[Test]
		public void GetAffinity_ForMidRange()
		{
			var map = new AffinityMap(_affinities);
			
			Affinity affinity = map.GetAffinityFromScore (50);
			
			Assert.That (affinity.AffinityTitle, Is.StringMatching("A"));
		}


		[Test]
		public void GetAffinity_ForImmediatelyBeforeNextRange()
		{
			var map = new AffinityMap(_affinities);
			
			Affinity affinity = map.GetAffinityFromScore (99);
			
			Assert.That (affinity.AffinityTitle, Is.StringMatching("A"));
		}

		[Test]
		public void GetAffinity_RangeIsExlusive()
		{
			var map = new AffinityMap(_affinities);
			
			Affinity affinity = map.GetAffinityFromScore (100);
			
			Assert.That (affinity.AffinityTitle, Is.StringMatching("B"));
		}

		[Test]
		public void GetAffinity_ForImmediatelyBeforeLast()
		{
			var map = new AffinityMap(_affinities);
			
			Affinity affinity = map.GetAffinityFromScore (199);
			
			Assert.That (affinity.AffinityTitle, Is.StringMatching("B"));
		}

		[Test]
		public void GetAffinity_ForLast() 
		{
			var map = new AffinityMap(_affinities);
			
			Affinity affinity = map.GetAffinityFromScore (200);
			
			Assert.That (affinity.AffinityTitle, Is.StringMatching("C"));
		}


		[Test]
		public void GetAffinity_WhenOutOfRangeOfUpperBound()
		{
			var map = new AffinityMap(_affinities);
			
			Affinity affinity = map.GetAffinityFromScore (300);
			
			Assert.That (affinity.AffinityTitle, Is.StringMatching("C"));
		}

    }
}