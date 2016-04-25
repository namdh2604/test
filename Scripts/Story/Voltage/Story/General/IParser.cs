

namespace Voltage.Story.General
{

    public interface IParser<T>
    {
		T Parse (string rawText);
    }
    
}



