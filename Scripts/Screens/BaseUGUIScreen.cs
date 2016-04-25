using UnityEngine;

namespace Voltage.Witches.Screens
{
    using Voltage.Witches.Controllers;

    public abstract class BaseUGUIScreen : MonoBehaviour, IScreen
    {
        private bool _isScreenLoaded = false;

        public virtual void Show()
        {
            gameObject.SetActive(true);
            _isScreenLoaded = true;
        }

        public virtual void Hide()
        {
            _isScreenLoaded = false;
            gameObject.SetActive(false);
        }

        public virtual void Close()
        {
            GetController().Close();
        }

        public virtual void Dispose()
        {
            Destroy(gameObject);
        }

        public virtual bool IsScreenLoaded()
        {
            return _isScreenLoaded;
        }

        protected abstract IScreenController GetController();

    }
}

