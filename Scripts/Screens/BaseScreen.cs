using System;
using UnityEngine;
using iGUI;
using Voltage.Witches.Controllers;
using System.Collections;

namespace Voltage.Witches.Screens
{
	public abstract class BaseScreen : MonoBehaviour, IScreen
	{
		[HideInInspector]
		public iGUIContainer screenFrame;

        // Need to find a way to expose this in the editor so that 
        // designer can adjust fade speed in the editor.
        public static float fadeSpeed = 0.75f;
        public bool screenLoaded = false;

        protected bool enableFade = false;
        protected bool fading = false;

        private Action<int> _callback;

        public virtual bool IsScreenLoaded()
        {
            return screenLoaded;
        }

        // Leaving Debug comments as we will come back to finish off the fading work.
        public virtual void Show()
		{
//            Debug.Log("SHOW was called!");
			screenFrame.setEnabled(true);
			if(gameObject != null)
			{
				gameObject.SetActive(true);
                if (enableFade)
                {
                    StartCoroutine(FadeIn());
                }
                else
                {
                    screenLoaded = true;
                }
			}
		}

        public virtual void Hide()
        {
            Hide(true);
        }

        // HACK - attached an optional dispose parameter to allow just hiding instead of disposing. Eventually, this should just be the standard behavior
        public virtual void Hide(bool dispose)
		{
//            Debug.Log("HIDE was called!");
            if (screenFrame != null)
            {
                if (enableFade)
                {
                    StartCoroutine(FadeOut());
                    // We will deactivate when done fading
                }
                else
                {
                    if (!dispose)
                    {
                        screenFrame.setEnabled(false);
                    }
                    else
                    {
                        // Normally hide should just hide a screen but the engine is relying on 
                        // the screen to be detory on hide.   Keep this behavior for now until we can
                        // redesign a controller that decide when stuff should be delete and when to keep stuff
                        // around for reuse.
    //                    Debug.Log("HIDE SCREEN NO FADE");
                        Dispose();
                    }
                }
            }
		}

		public void Close()
		{
			GetController().Close();
		}

		public virtual void Dispose()
		{
            gameObject.SetActive(false);
            screenFrame.setEnabled(false);
			GameObject.Destroy(gameObject);
		}

		public void ShowDialog(IDialog dialog, Action<int> callback)
		{
			_callback = callback;
			MakePassive(true);
			dialog.Display(OnCallback);
		}

		public void ShowMaintenanceDialogue(iGUISmartPrefab_SystemPopupDialog dialogue)
		{
			MakePassive (true);
			dialogue.SetText ("Server is in Maintenance");	
			dialogue.Display (null);
		}

		private void OnCallback(int result)
		{
			MakePassive(false);
			_callback(result);
		}

		public virtual void MakePassive(bool isPassive)
		{
			screenFrame.passive = isPassive;
		}

		protected abstract IScreenController GetController();

        // This FadeIn will take a screen from 100% to 0%, it is not designed do partial fades
        protected IEnumerator FadeOut()
        {
            // We will wait if the screen is alreading fading in.
            while (fading)
            {
                yield return null;
            }

//            Debug.Log("FADE OUT STARTS was called!");

            float startTime = Time.time;
            screenFrame.opacity = 1f;
            float transLevel = 1f;
            while (transLevel > 0f)
            {
                float fadeTime = Time.time - startTime;
                transLevel = 1f - (fadeTime/fadeSpeed);
                if (transLevel < 0f)
                {
                    transLevel = 0f;
                }
                screenFrame.opacity = transLevel;
                //Debug.Log("FADE OUT level is : " + transLevel);
                yield return null;
            }

  //          Debug.Log("FADE OUT DONE");
            fading = false;
            screenLoaded = false;
            // Like in the other hide, we need to destory the screen since the engine requires it right now.
            Dispose();
        }

        // This FadeIn will take a screen from 0% to 100%, it is not designed do partial fades
        protected IEnumerator FadeIn()
        {
            // We will wait if the screen is alreading fading out.
            while (fading)
            {
                yield return null;
            }
            
            float startTime = Time.time;
            fading = true;
            screenFrame.opacity = 0f;
            float transLevel = 0f;
            while (transLevel < 1f)
            {
                float fadeTime = Time.time - startTime;
                transLevel = fadeTime/fadeSpeed;
                if (transLevel < 0f)
                {
                    transLevel = 0f;
                }
                screenFrame.opacity = transLevel;
                //Debug.Log("FADE IN level is : " + transLevel);
                yield return null;
            }
            fading = false;
            screenLoaded = true;
        }
	}
}

