

namespace Voltage.Witches.UI
{

    public interface IDimmable
    {
		void Dim(float value, float overDuration=0f);
    }
    

	public interface IShowable
	{
		void Show();
		void Hide();
	}

}



