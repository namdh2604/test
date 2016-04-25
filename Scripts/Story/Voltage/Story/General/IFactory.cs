
using System;

namespace Voltage.Story.General
{
    public interface IFactory<T,U>
    {
		U Create (T input);
    }

	public interface IAsyncFactory<T,U>
	{
		void Create (T input, Action<U> callback);	// Action onFailureCallback ???
	}
    
}



