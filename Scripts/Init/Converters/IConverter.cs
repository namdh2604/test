

namespace Voltage.Common.Converters
{
    public interface IConverter<T,U>
    {
		U Convert(T original);
    }
    

	public interface IGenericConverter
	{
		U Convert<T,U>(T original);
	}

}



