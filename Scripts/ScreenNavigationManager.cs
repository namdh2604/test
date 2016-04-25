using System;
using System.Collections.Generic;
using System.Linq;
using Voltage.Witches.Controllers;
using Voltage.Witches.Util;

namespace Voltage.Witches
{
	public class ScreenNavigationManager
	{
		private Stack<IScreenController> _screens;

		bool _isNavigating;

		public ScreenNavigationManager()
		{
			_screens = new Stack<IScreenController>();
			_isNavigating = false;
		}

		public string GetCurrentPath()
		{
			return ScreenPathUtil.GetPath(_screens);
		}

		public IScreenController CurrentController 
		{ 
			get 
			{ 
				if(_screens.Count > 0)
				{
					return _screens.Peek (); 
				}
				else
				{
					return default(IScreenController);
				}
			} 
		}

		public void Add(IScreenController screen, bool showOnLoad=true)
		{
//			UnityEngine.Debug.LogWarning (string.Format ("NAVMGR >>> ADD >>> ScreenNavigationManager [{0}], screen [{1}], screen.GetType(): {2}", Voltage.Common.ID.UniqueObjectID.Default.GetID (this), Voltage.Common.ID.UniqueObjectID.Default.GetID (screen), screen.GetType ().ToString ()));
//			UnityEngine.Debug.LogWarning (string.Format ("NAVMGR >>> adding screen:{0} to {1} [count: {2}]", screen.Name, GetCurrentPath (), _screens.Count));
			
			if (_screens.Count > 0)
			{
				IScreenController prevScreen = _screens.Peek();
			    prevScreen.Hide();
			}
			_screens.Push(screen);

		   // Calling this here as the code does not call show and is not called anywhere.  I am calling this to let the screen know
		   // so it can prep itself to be show.
			if (showOnLoad) 
			{
				screen.Show ();
			} 
			else 
			{
				// TODO: shouldn't need to explicitly Hide! but some screens still create views on construction (visible by default)
				screen.Hide ();		
			}
		}

        public void ReplaceCurrent(IScreenController screen)
        {
            IScreenController prevScreen = null;

            if (_screens.Count > 0)
            {
                prevScreen = _screens.Pop();
            }

            _screens.Push(screen);

            if (prevScreen != null)
            {
                prevScreen.Dispose();
            }
        }

        public void HideCurrent()
        {
            IScreenController currentScreen = _screens.Pop();
            if(currentScreen != null)
            {
                IScreenController prevScreen = currentScreen;
                prevScreen.Hide();
            }
        }
        
        public void CloseCurrentScreen()
		{
			IScreenController currentScreen = _screens.Pop();
			if(currentScreen != null)
			{
				IScreenController prevScreen = currentScreen;

				if (!_isNavigating && _screens.Count > 0)
				{
					currentScreen = _screens.Peek();
                    currentScreen.Show();
				}

				prevScreen.Dispose();
			}
		}

		public T GetController<T>() where T : ScreenController, new()
		{
			T newT = new T();
			newT.SetManager(this);

			return newT;
		}

		private void CheckForExistingPath(string path)
		{
			if (path.StartsWith("/"))
			{
				path = path.Substring(1);
			}

			if (path == string.Empty)
			{
				return;
			}

			string[] screenTokens = path.Split('/');

			IScreenController[] screenArray = _screens.ToArray().Reverse().ToArray();
			for (int i = 0; i < screenTokens.Length; ++i)
			{
				if (screenArray[i].Name != screenTokens[i])
				{
					throw new Exception("Requested path (" + path + ") does not exist. Current path is: " + GetCurrentPath());
				}
			}
		}

		private void ReduceToPath(string path)
		{
			try
			{
				CheckForExistingPath(path);
			}
			catch(Exception)
			{
				throw new Exception(string.Format("Error Checking for existing path: {0} [Current: {1}]", path, GetCurrentPath()));
			}

			if (path.StartsWith("/"))
			{
				path = path.Substring(1);
			}

            char[] separators = { '/' };
            string[] screenTokens = path.Split(separators, StringSplitOptions.RemoveEmptyEntries);

			int numScreensToRemove = _screens.Count - screenTokens.Length;

			_isNavigating = true;
			for (int i = 0; i < numScreensToRemove; ++i)
			{
				IScreenController currentScreen = _screens.Pop();
				if(currentScreen != null)
				{
					currentScreen.Dispose();
				}
			}
			_isNavigating = false;
		}

		public void GoToExistingScreen(string path)
		{
			try
			{
				ReduceToPath(path);
			}
			catch(Exception e)
			{
                throw new Exception("Error reducing path: " + e.Message);
			}

			if (_screens.Count > 0)
			{
				IScreenController currentScreen = _screens.Peek();
				if(currentScreen != null)
				{
                    currentScreen.Show();
				}
			}
		}

		public void OpenScreenAtPath(IScreenController screen, string path)
		{
			ReduceToPath(path);

			_screens.Push(screen);	// should this use Add(screen)?
            screen.Show();
		}

		public void CloseAll()
		{
			while(_screens.Count > 0)
			{
				IScreenController screen = _screens.Pop();
				screen.Dispose();
			}
		}

        public void HideAll()
        {
            while(_screens.Count > 0)
            {
                IScreenController screen = _screens.Pop();
                screen.Hide();
            }
        }

    }
}

