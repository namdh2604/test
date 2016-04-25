using iGUI;

namespace Voltage.Witches.Screens
{
	public class MinigameScreen: IScreenFactory
	{
		private iGUIContainer _contentRoot;
		private iGUIContainer _dialogRoot;

		public MinigameScreen(iGUIContainer contentRoot, iGUIContainer dialogRoot)
		{
			_contentRoot = contentRoot;
			_dialogRoot = dialogRoot;
		}

		public T GetScreen<T>() where T : UnityEngine.Component
		{
			return GetContentScreen<T>(_contentRoot);
		}

		public T GetDialog<T>() where T : UnityEngine.Component
		{
			return GetInternalScreen<T>(_dialogRoot);
		}

        public T GetOverlay<T>() where T : UnityEngine.Component
        {
            throw new System.NotImplementedException();
        }

		private T GetContentScreen<T>(iGUIContainer parent) where T : UnityEngine.Component
		{
			iGUIElement element = parent.GetComponent<iGUIElement>();
			return element.GetComponent<T>();

		}

		private T GetInternalScreen<T>(iGUIContainer parent) where T : UnityEngine.Component
		{
			string typename = typeof(T).Name;
			typename = ParseSmartObjectName(typename);
			iGUIElement element = parent.addSmartObject(typename);
			return element.GetComponent<T>();
		}

		const string PREFIX = "iGUISmartPrefab_";
		string ParseSmartObjectName(string rawName)
		{
			return rawName.Replace(PREFIX, "");
		}
	}
}