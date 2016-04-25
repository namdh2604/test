
using System.Collections.Generic;

namespace Voltage.Story.Effects
{

	public interface IEffectResolver
	{
		void Resolve(IDictionary<string,int> effects);
	}
    
}



