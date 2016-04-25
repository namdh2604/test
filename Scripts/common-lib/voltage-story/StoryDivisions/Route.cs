using System;
using System.Collections.Generic;

namespace Voltage.Story.StoryDivisions
{
	public class Route : BaseIdentifiableContainer<Arc>
	{
		public IList<Arc> Arcs { get { return new List<Arc> (ElementMap.Values); } }
		public IDictionary<string,Arc> ArcMap { get { return ElementMap; } }
		public Arc GetArc (string arc)
		{
			if(ArcMap != null)
			{
				if(ArcMap.ContainsKey(arc))
				{
					return ArcMap[arc];
				}
			}
			
			return default(Arc);
		}
		
		public Route (string name, params Arc[] arcs) : base(name, arcs) {}
	}

}




