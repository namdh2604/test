namespace Voltage.Witches.ViewModels
{
	public interface IViewModel
	{
		string ViewName { get; }
		void SetEnabled(bool value);
		void Unload();
	}
}