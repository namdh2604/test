
using System;
using System.Collections.Generic;
using Voltage.Story.General;
using Voltage.Story.Models.Nodes.ID;

namespace Voltage.Story.StoryDivisions
{
	public abstract class BaseIdentifiableContainer<T> : IIdentifiable<string> where T : IIdentifiable<string>
	{
		public string ID { get; private set; }
		public string Name { get { return ID; } }
		
		protected IList<T> Elements { get { return new List<T> (ElementMap.Values); } }
		
		protected Dictionary<string,T> ElementMap = new Dictionary<string,T>();
		
		public BaseIdentifiableContainer(string name, params T[] elements)
		{
			ID = name;
			
			if (elements != null)
			{
				foreach (T element in elements)
				{
					ElementMap.Add(element.ID, element);
				}
			}
		}
	}
}




