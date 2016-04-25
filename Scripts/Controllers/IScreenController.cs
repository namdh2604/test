namespace Voltage.Witches.Controllers
{
	public interface IScreenController
	{
		void Show();
		void Hide();
		void Close();
		void Dispose();

		void MakePassive(bool value);

//		void SetEnabled(bool value);
//		void Unload();

		string Name { get; }
	}
}

