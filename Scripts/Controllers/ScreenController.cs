using Voltage.Witches;
using Voltage.Witches.Screens;

namespace Voltage.Witches.Controllers
{
	public abstract class ScreenController : IScreenController
	{
		private const string SCREEN_CONTROLLER_SUFFIX = "ScreenController";

		protected ScreenNavigationManager Manager { get; private set; }

		public ScreenController(ScreenNavigationManager manager)
		{
			Manager = manager;
		}

		public void SetManager(ScreenNavigationManager manager)
		{
			Manager = manager;
		}

		public virtual void Show()
		{
            if (GetScreen() != null)
            {
                GetScreen().Show();
            }
		}

		public virtual void Hide()
		{
            // Removing the dispose as we should be calling dispose if we want to remove the screens.
            if(GetScreen() != null)
            {
                GetScreen().Hide();
                //GetScreen().Dispose();
            }
		}

		public virtual void Close()
		{
			Hide();
			Manager.CloseCurrentScreen();
		}

		public virtual void Dispose()
		{
			if(GetScreen() != null)
			{
				GetScreen().Dispose();
			}
		}

		public abstract void MakePassive(bool value);

		protected abstract IScreen GetScreen();

		private string ParseClassName(string className)
		{
			int suffixLength = SCREEN_CONTROLLER_SUFFIX.Length;
			return className.Substring(0, className.Length - suffixLength);
		}

		public virtual string Name
		{
			get
			{
				return ParseClassName(this.GetType().Name);
			}
		}
	}
}

