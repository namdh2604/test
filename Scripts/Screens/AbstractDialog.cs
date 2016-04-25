using System;
using UnityEngine;
using iGUI;
using Voltage.Witches.Controllers;

namespace Voltage.Witches.Screens
{
	public class AbstractDialog : BaseScreen, IDialog
	{
		private Action<int> _callback;

		public void Display(Action<int> callback)
		{
			if(screenFrame.container.GetType() == typeof(iGUIContainer))
            {
                int maxLayer = DialogUtil.GetMaxLayer(screenFrame.container as iGUIContainer);
                // HACK - if we don't do this, an iGUI crash is possible when it tries to get the layer of a deleted game object
//                if (screenFrame.layer < maxLayer)
//                {
                    screenFrame.setLayer(maxLayer + 1);
//                }
			}
            this.Show();
			_callback = callback;
		}

		protected virtual void SubmitResponse(int response, bool autoClose=true)
		{
            if (autoClose)
            {
    			screenFrame.setEnabled(false);
            }
    
			if (_callback != null)
			{
				_callback(response);
			}

            if (autoClose)
            {
                // FIXME: This should get replaced with the implementation in the book unlock, arc unlocked, etc dialogs
                // basically, iGUI containers need to handle the removal of their children
                GameObject.Destroy(gameObject);
            }
		}

		protected override IScreenController GetController()
		{
			// TODO: Refactor this out (change hierarchy)
			return null;
		}
	}
}

