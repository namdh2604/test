
using System;
using System.Collections.Generic;

namespace Voltage.Story.Configurations
{

	public class NamedSelectionData
	{
		public string Path { get; private set; }
		public string Name { get; private set; }
		
		public NamedSelectionData (string path, string name)
		{
			Path = path;
			Name = name;
		}
		
		public override string ToString ()
		{
			return string.Format ("<Path={0}, Name={1}>", Path, Name);
		}
	}

}




