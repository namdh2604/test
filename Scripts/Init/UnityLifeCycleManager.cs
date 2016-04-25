using UnityEngine;
using System;

namespace Voltage.Witches.Init
{
    public class FocusEventArgs : EventArgs
    {
        public readonly bool IsPaused;

        public FocusEventArgs(bool pauseState)
        {
            IsPaused = pauseState;
        }
    }

    public class UnityLifeCycleManager : MonoBehaviour
    {
        public event EventHandler OnFocus;

        private void OnApplicationFocus(bool recievedFocus)
        {
            if (OnFocus != null)
            {
                OnFocus(this, new FocusEventArgs(!recievedFocus));
            }
        }

        private void OnDestroy()
        {
            if (OnFocus == null)
            {
                return;
            }

            Delegate[] clientList = OnFocus.GetInvocationList();
            foreach (var d in clientList)
            {
                OnFocus -= (d as EventHandler);
            }

            OnFocus = null;
        }
    }
}

