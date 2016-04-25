
using System;
using System.Collections.Generic;

namespace Voltage.Story.Configurations
{

    public class ArcData
    {
		private readonly int _order;
		public int Order { get { return _order; } }

		public string Country { get; private set; }
		public string CountryAlias { get; private set; }
		public string ImageName { get; private set; }
        public bool Locked { get; set; }

		public ArcData (int order, string country, string image, string alias="")
		{
			_order = order;
			Country = country;
			CountryAlias = alias;
			ImageName = image;
            Locked = false;
		}

        public ArcData(ArcData existing)
        {
            _order = existing.Order;
            Country = existing.Country;
            CountryAlias = existing.CountryAlias;
            ImageName = existing.ImageName;
            Locked = existing.Locked;
        }

    }
    
}




