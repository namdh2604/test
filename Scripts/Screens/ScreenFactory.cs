using iGUI;

namespace Voltage.Witches.Screens
{
	public interface IScreenFactory
	{
		T GetScreen<T>() where T : UnityEngine.Component;
		T GetDialog<T>() where T : UnityEngine.Component;
        T GetOverlay<T>() where T : UnityEngine.Component;
	}



    // TODO: Ideally, this entire class will be replaced with just the UGUI Factory. Until that point, delegate to the UGUI factory
    //  for UGUI Screens
	public class ScreenFactory : IScreenFactory
	{
		private iGUIContainer _contentRoot;
		private iGUIContainer _dialogRoot;
        private iGUIContainer _overlayRoot;
        private UGUIScreenFactory _screenFactory;

        private enum ScreenLayer
        {
            Screen,
            Dialog,
            Overlay
        }

        public ScreenFactory(iGUIContainer contentRoot, iGUIContainer dialogRoot, iGUIContainer overlayRoot, UGUIScreenFactory uGUIScreenFactory)
		{
			_contentRoot = contentRoot;
			_dialogRoot = dialogRoot;
            _overlayRoot = overlayRoot;
            _screenFactory = uGUIScreenFactory;
		}

        // TODO: Legacy constructor -- supports tests classes, but will be removed when this entire class is unnecessary
        public ScreenFactory(iGUIContainer contentRoot, iGUIContainer dialogRoot, iGUIContainer overlayRoot) : this(contentRoot, dialogRoot, overlayRoot, null)
        {
            UnityEngine.Debug.LogWarning("uGUI factory is null -- uGUI screens will be unavailable");
        }

		public T GetScreen<T>() where T : UnityEngine.Component
		{
            return GetScreenInternal<T>(ScreenLayer.Screen);
		}

		public T GetDialog<T>() where T : UnityEngine.Component
		{
            return GetScreenInternal<T>(ScreenLayer.Dialog);
		}

        public T GetOverlay<T>() where T : UnityEngine.Component
        {
            return GetScreenInternal<T>(ScreenLayer.Overlay);
            
        }

        private T GetScreenInternal<T>(ScreenLayer layer) where T : UnityEngine.Component
        {
            if (typeof(BaseUGUIScreen).IsAssignableFrom(typeof(T)))
            {
                return GetUGUIScreen<T>(layer);
            }
            else
            {
                return GetiGUIScreen<T>(layer);
            }
        }

        private T GetUGUIScreen<T>(ScreenLayer layer) where T : UnityEngine.Component
        {
            if (layer == ScreenLayer.Dialog)
            {
                return _screenFactory.GetDialog<T>();
            }
            else
            {
                return _screenFactory.GetScreen<T>();
            }
        }

        private T GetiGUIScreen<T>(ScreenLayer layer) where T : UnityEngine.Component
		{
            iGUIContainer parent = null;
            if (layer == ScreenLayer.Screen)
            {
                parent = _contentRoot;
            }
            else if (layer == ScreenLayer.Dialog)
            {
                parent = _dialogRoot;
            }
            else
            {
                parent = _overlayRoot;
            }

			string typename = typeof(T).Name;
			typename = ParseSmartObjectName(typename);
			iGUIElement element = null;
			if(typename != "MiniGameRuneCauldronManager")
			{
				element = parent.addSmartObject(typename);
			}
			else 
			{
				element = parent;
			}
			return element.GetComponent<T>();
		}

		const string PREFIX = "iGUISmartPrefab_";
		string ParseSmartObjectName(string rawName)
		{
			return rawName.Replace(PREFIX, "");
		}
	}
}

