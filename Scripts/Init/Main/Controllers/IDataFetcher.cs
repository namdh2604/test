
using System;

namespace Voltage.Witches.DI
{

//	public interface Task
//	{
//		void Start();
//	}

    public interface IDataFetcher<T>
    {
		T Data { get; }
		void Fetch();
    }

	public interface IDataCallbackFetcher
	{
		void Fetch(Action callback);
	}

	public interface IDataCallbackFetcher<T> //: IDataFetcher<T>
	{
		void Fetch(Action<T> callback);
	}
    
}



