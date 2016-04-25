namespace Voltage.Witches.Screens
{
	public interface IScreen
	{
		void Show();
		void Hide();
		void Close();
		void Dispose();
        bool IsScreenLoaded();
	}
}

