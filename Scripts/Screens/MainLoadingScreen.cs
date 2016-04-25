using UnityEngine;
using iGUI;
using System;
using System.Collections;
using System.Collections.Generic;
using Voltage.Witches.Events;
using Voltage.Witches.Controllers;
using System.IO;

namespace Voltage.Witches.Screens
{
	using Voltage.Common.Net;
    using Voltage.Witches.Services;
	using TextPool = Voltage.Witches.Components.TextDisplay.TextPool;
	using ITextPool = Voltage.Witches.Components.TextDisplay.TextPool;

	public class MainLoadingScreen : BaseScreen
	{
		[HideInInspector]
		public iGUIImage loading_bar,bg,Transparent_BG, text_bg;

		[HideInInspector]
		public iGUILabel loading_text,loading_percentage;

		[HideInInspector]
		public iGUIButton forward_button,start_button,restore_button;

		[HideInInspector]
		public iGUIContainer loading_screen,top_screen,loading_marquee,buttons_container,transfer_data_container,start_game_container, logo_container;

		[HideInInspector]
		public AnimatedImage loading_book_anim;

		private float _delay = 1.5f;
		ITextPool _textPool;
		MainLoadingScreenController _controller;

		private iGUILabel _versionInfoLabel;
        private string _buildVersion;

		public INetworkTransportLayer Request { get; set; }
		public MainLayoutType Layout { get; protected set; } 

		public Action CallForceUpdate { get; protected set; }
		public Action CallMaintenance { get; protected set; }
		public Action CallNetworkError { get; protected set; }

		private float _testProgression = 0f;
        private float MLtransLevel = 0f;
        private bool loadComplete = false;
        private IEnumerator _loadingCoroutine;
        private bool _hasStarted = false;
		private iGUIContainer _debugContainer;

        IGUIHandler _buttonHandler;
		Dictionary<iGUIButton,iGUIElement> _buttonArtMap;

		public void Init(MainLoadingScreenController controller, INetworkTransportLayer request, int layoutType)	// TODO: aggregate data progress
		{
			Request = request;

			_controller = controller;
            _buildVersion = _controller.GetBuildVersion();
			Layout = (MainLayoutType)layoutType;
		}

		protected void Awake()
		{
			_buttonHandler = gameObject.AddComponent<IGUIHandler>();
			_buttonHandler.MovedBack += HandleMovedBack;
			_buttonHandler.MovedAway += HandleMovedAway;

			CallForceUpdate = DisplayForceUpdateDialog;
			CallMaintenance = DisplayMaintenanceDialog;
			CallNetworkError = DisplayNetworkErrorDialog;
            //enableFade = true;
            //if (Layout == MainLayoutType.LOADING_MARQUEE)
            //{
            //    enableFade = false;
            //}
		}

        public override void Show()
        {
            base.Show();
        }

        public override void Dispose()
        {
            var isTopScreen = (Layout == MainLayoutType.TOP_SCREEN);
            if (isTopScreen)
            {
                _buttonHandler.ReleasedButtonEvent -= HandleReleasedButtonEvent;
                _buttonHandler.InputCancelledEvent -= HandleReleasedButtonEvent;
            }
            base.Dispose();
        }

        public override void Hide()
        {
            if (Layout == MainLayoutType.TOP_SCREEN)
            {
                MLtransLevel = 1.0f;
                screenFrame.opacity = 1.0f;
                top_screen.gameObject.SetActive(false);
                StartCoroutine(MLFadeOut());
            }
            else if (Layout == MainLayoutType.LOADING)
            {
                loadComplete = true; // Flag that the loading is done and have the loading bar to indicate this in the update progress.
                MLtransLevel = 1.0f;
                screenFrame.opacity = 1.0f;
                StartCoroutine(LSFadeOut());
            }
            else
            {
                base.Hide();
            }
        }

		public override void MakePassive(bool value)
		{
			base.MakePassive(value);

			if (value) 
			{
				_buttonHandler.Deactivate ();
			} 
			else 
			{
				_buttonHandler.Activate ();
			}
		}

        // Doing a special fade for the top screen
        protected IEnumerator MLFadeOut()
        {
            while (fading)
            {
                yield return null;
            }
            
            float startTime = Time.time;
            fading = true;
            while (MLtransLevel > 0f)
            {
                float fadeTime = Time.time - startTime;
                MLtransLevel = 1f - (fadeTime/fadeSpeed);
                if (MLtransLevel < 0f)
                {
                    MLtransLevel = 0f;
                }
                top_screen.opacity = MLtransLevel;
                yield return null;
            }
            
//            Debug.Log("TOP SCREEN FADE OUT DONE");
            fading = false;
            Dispose();
        }

        // Doing a special fade for the top screen
        protected IEnumerator LSFadeOut()
        {
            while (fading)
            {
                yield return null;
            }
            
            fading = true;
            yield return new WaitForSeconds(0.5f);  // HOld the loading bar screen for a bit then finish the fade out.

            // Design doesn't want to cross fade, until we can segment the fades porperly, going for a quick instant fade.
            fading = false;
            Dispose();
//            float startTime = Time.time;
//            while (MLtransLevel > 0f)
//            {
//                float fadeTime = Time.time - startTime;
//                MLtransLevel = 1f - (fadeTime/fadeSpeed);
//                if (MLtransLevel < 0f)
//                {
//                    MLtransLevel = 0f;
//                }
//                screenFrame.opacity = MLtransLevel;
//                yield return null;
//            }
//            
//            //            Debug.Log("TOP SCREEN FADE OUT DONE");
//            fading = false;
//            Dispose();
        }

        protected void Start()
		{
			var isTopScreen = (Layout == MainLayoutType.TOP_SCREEN);
			var isLoading = (Layout == MainLayoutType.LOADING);
			var isLoadingMarquee = (Layout == MainLayoutType.LOADING_MARQUEE);

            // Keeping debug stuff comment for now. - Hung Nguyen
//            if (isLoadingMarquee)
//            {
//                Debug.Log("This is the Loading MARQUEE START");
//            }
//            else if (isLoading)
//            {
//                Debug.Log("This is the LOADING BAR");
//            }
//            else if (isTopScreen)
//            {
//                Debug.Log("This is the TOPSCREEN");
//            }

            // TODO - Hung Nguyen
            // Need to look at the flow so we don't draw this screen three times, waste of memeory and resources.

//            top_screen.setEnabled(isTopScreen);
//            loading_screen.setEnabled(isLoading);
//            loading_marquee.setEnabled(false); // They don't want the animating book anymore.
//            Transparent_BG.setEnabled(false); // They don't want a transparent box anymore as it was use with the book.
//            bg.setEnabled(!isLoadingMarquee); // Always have the background.

//          This is the old option setting, keeping this hear incase they want to bring back the book, if after two sprint, 
//          we should delete next time we are in this code. - Hung Nguyen 
            top_screen.setEnabled(isTopScreen);
			loading_screen.setEnabled(isLoading);
			loading_marquee.setEnabled(isLoadingMarquee);
			Transparent_BG.setEnabled(isLoadingMarquee);
			bg.setEnabled((!isLoadingMarquee));

			start_button.clickDownCallback += ClickInit;
			restore_button.clickDownCallback += ClickInit;

			_buttonArtMap = new Dictionary<iGUIButton,iGUIElement>()
			{
				{start_button,start_button.getTargetContainer()},
				{restore_button,restore_button.getTargetContainer()}
			};

			if((isTopScreen) && (!Debug.isDebugBuild))
			{
				HideDataTransfer();
				CenterStartGame();
			}

			//TODO Remove this after confirming functionality
			if((isTopScreen) && (Debug.isDebugBuild))
			{
				var parent = gameObject.GetComponent<iGUIContainer> ();
				_debugContainer = parent.addElement<iGUIContainer>("Debug_Container", new Rect(0.5f,0.5f,1f,1f));
				_debugContainer.setVariableName("Debug_Container");
				_debugContainer.name = _debugContainer.variableName;
				_debugContainer.setLayer(parent.itemCount + 1);
			}

            float centerX = loading_screen.baseRect.width/2; // Get the screen width
            Vector2 temp;
            temp.x = centerX - (text_bg.rect.width/2); // The loading_screen is an invisibile rect and the size is much larger.  Grab the width of the bounding box instead.
            temp.y = loading_screen.rect.y;
            loading_screen.setPosition(temp);

            if(isLoading)
			{
                
//                Rect loadingbar = loading_screen.rect;

                _textPool = new TextPool("hint_text");
//				Debug.Log("Execute loading routine...");
				if(_debugContainer != null)
				{
					_debugContainer.setEnabled(false);
				}
				var hintDisplay = UpdateHints(_textPool.GetRandomizedStringSet(null));
				StartCoroutine(hintDisplay);
				UpdateLoadingBar();
				UpdateLoadingText();

				_loadingCoroutine = UpdateProgress();
				StartCoroutine(_loadingCoroutine);
			}

			if(isLoadingMarquee)
			{
				Debug.Log("Execute loading marquee routine....");
				if(_debugContainer != null)
				{
					_debugContainer.setEnabled(false);
				}
				var element = gameObject.GetComponent<iGUIElement>();
				var currentLayer = element.layer;
				element.setLayer(++currentLayer);
			}

			//Production wants to display this in the produciton build now
//			if(Debug.isDebugBuild)
			if (isTopScreen)
			{
				var container = gameObject.GetComponent<iGUIContainer>();
				_versionInfoLabel = container.addElement<iGUILabel>();
				_versionInfoLabel.name = "VERSION_INFO";
				_versionInfoLabel.setVariableName(_versionInfoLabel.name);
				_versionInfoLabel.setDynamicFontSize(0.2f);
				_versionInfoLabel.setLayer(container.itemCount + 1);
				_versionInfoLabel.setPositionAndSize(new Rect(0f,1f,0.25f,0.1f));

                _versionInfoLabel.label.text = " BUILD: " + _buildVersion;
                _hasStarted = false;
                _buttonHandler.InputCancelledEvent += HandleReleasedButtonEvent;
                _buttonHandler.ReleasedButtonEvent += HandleReleasedButtonEvent;

			}
		}

		void HideDataTransfer ()
		{
			transfer_data_container.setEnabled(false);
		}

		void CenterStartGame ()
		{
			start_game_container.setX(0.5f);
		}

		IEnumerator UpdateHints(List<string> hints)
		{	
			for(int i = 0; i < hints.Count; ++i)
			{
				loading_text.label.text = hints[i];
				yield return new WaitForSeconds(_delay);
			}
		}

		// HACK - Hung Nguyen
		// This hack in being put in place to add some level of accurracy of where we are in the loading process.
		// We made static global ppublic member call loadingStage that is set at each check point.
		IEnumerator UpdateProgress()
		{
			float stage = 0f;
			float max_ahead = 0f;	
			while (_testProgression < 1f)
			{
				// Determine what stage we have reach in the loading process and move the bar to that state.
				// This hack will ensure all loading bar to be in the same place at each distict state.
				switch (Voltage.Witches.DI.WitchesStartupSequence.loadingStage)
				{
				case 0:
					stage = 0.0f;
					max_ahead = 0.15f;
					break;
				case 1:
					stage = 0.15f;
					max_ahead = 0.30f;
					break;
				case 2:
					stage = 0.30f;
					max_ahead = 0.55f;
					break;
				case 3:
					stage = 0.55f;
					max_ahead = 0.85f;
					break;
				case 4:
					stage = 0.85f;
					max_ahead = 0.99f;
					break;
				default:
					stage = 0f;
					max_ahead = 0.95f;
					break;
				}

				if (_testProgression < stage)
				{
					_testProgression = stage;
				}

				// This is to slow down the loading bar as we get closer to the end of the bar.
                if (_testProgression < 0.15f)
                {
                    _testProgression += 0.005f;
                }
                else if (_testProgression < 0.50f)
				{
					_testProgression += 0.01f;
				}
				else if (_testProgression < 0.75f)
				{
					_testProgression += 0.005f;
				}
				else if (_testProgression < 0.95f)
				{
					_testProgression += 0.0025f;
				}
				else
				{
					_testProgression += 0.00025f;
				}

				// This will let the loading bar continue pass the stage we are at, but not ahead of where the next stage is.
				// It will also cap the progressing to 99 percent.
				if (_testProgression >= max_ahead)
				{
					_testProgression = max_ahead;
				}
                if (loadComplete)
                {
                    // We know that we are done so set the loading bar to 100%
                    _testProgression = 1f;
                }

				UpdateLoadingBar();
				UpdateLoadingText();
				yield return new WaitForEndOfFrame();
			}
			
			loading_text.label.text = "Loading complete";
			yield return new WaitForEndOfFrame();
			yield return new WaitForEndOfFrame();
			
			_controller.LoadingComplete();
			Debug.Log("Done loading");
		}

		void ChangeLoadingText()
		{
			loading_text.label.text = "Here is where loading stuff will display\nIt is useful right?\nRIGHT??";
		}

		void UpdateLoadingBar()
		{
			loading_bar.setWidth(_testProgression);
		}

		void UpdateLoadingText()
		{
			loading_percentage.label.text = (ReturnProgressAsString());
		}

		string ReturnProgressAsString()
		{
			return ((_testProgression.ToString("P0")));
		}

		public void StopLoading()
		{
			if(_loadingCoroutine != null)
			{
				StopCoroutine(_loadingCoroutine);
			}
		}

		public void DisplayForceUpdateDialog()
		{
			_buttonHandler.Deactivate();
			var dialog = _controller.GetForceUpdateDialog();
			dialog.Display(HandleForceUpdateResponse);
		}

		public void DisplayNetworkErrorDialog()
		{
			var dialog = _controller.GetNetworkErrorDialog();
			dialog.Display (HandleNetworkErrorResponse);
		}

		public void DisplayMaintenanceDialog()
		{
			var dialog = _controller.GetMaintenanceDialog();
			dialog.Display (HandleMaintenanceResponse);
		}

		void ClickInit(iGUIElement element)
		{
			if((_buttonHandler.IsActive) && (_buttonHandler.PressedButton == null))
			{
				var button = (iGUIButton)element;
				_buttonHandler.SelectButton(button);
				_buttonArtMap[button].colorTo(Color.grey,0f);
			}
		}
		
		void HandleMovedAway(iGUIButton button)
		{
			_buttonArtMap[button].colorTo(Color.white,0.3f);
		}
		
		void HandleMovedBack(iGUIButton button)
		{
			_buttonArtMap[button].colorTo(Color.grey,0f);
		}
		
		void HandleReleasedButtonEvent(iGUIButton button, bool isOverButton)
		{
			Debug.Log (string.Format("HandleReleasedButtonEvent >>> button:{0}, controller: {1}, appstartevent: {2}", (button!=null?button.name:"null"), (_controller!=null?_controller.GetType().ToString():"null"), (_controller!=null&&_controller.AppStartEvent!=null?_controller.AppStartEvent.Method.ToString():"null")));
            if (_hasStarted)
            {
                return;
            }

            _hasStarted = true;

			if(isOverButton)
			{
				if(button == start_button)
				{
					_controller.AppStartEvent();
				}
				else if(button == restore_button)
				{
					_controller.RestoreEvent();
				}
			}

			_buttonArtMap[button].colorTo(Color.white,0.3f);
		}

		void QuitApplication()
		{
			#if UNITY_IOS || UNITY_IPHONE && !UNITY_EDITOR
			//May need to change this
			Debug.LogWarning("The app should quit, but iOS guidelines require quitting to be user initiated");
			#elif UNITY_ANDROID && !UNITY_EDITOR
			Application.Quit();
			#else
			Debug.Log("Quit app on android, suspend on ios?");
			#endif
		}

		void HandleForceUpdateResponse(int answer)
		{
			//TODO Update the url to match our actual app id/page when available
			#if UNITY_IPHONE || UNITY_IOS && !UNITY_EDITOR
			Application.OpenURL("itms-apps://itunes.apple.com/us/app/kisses-curses/id962414762?ls=1&mt=8");
			#elif UNITY_ANDROID && !UNITY_EDITOR
			//May need to remove the &hl=en at the end of this url
			Application.OpenURL("market://details?id=com.voltage.curse.en&hl=en");
			#else
			Application.OpenURL("https://play.google.com/store/apps/details?id=com.voltage.curse.en");
			#endif
			//TODO Is this really necessary?  If they update, the app will shut down regardless
			_buttonHandler.Activate();
			QuitApplication();
		}

		void HandleNetworkErrorResponse(int answer)
		{
			switch((DialogResponse)answer)
			{
				case DialogResponse.Cancel:
					//TODO Make Controller call to try again or refresh
					QuitApplication();
					break;
				case DialogResponse.OK:
					QuitApplication();
					break;
			}
		}

		void HandleCantConnectResponse(int answer)
		{
			switch((DialogResponse)answer)
			{
				case DialogResponse.Cancel:
					//TODO Make Controller call to try again or refresh
					QuitApplication();
					break;
				case DialogResponse.OK:
					//Do nothing
					break;
			}
		}

		void HandleMaintenanceResponse(int answer)
		{
			QuitApplication();
		}

		protected override IScreenController GetController()
		{
			return _controller;
		}

		public void SetEnabled(bool value)
		{
			screenFrame.setEnabled(value);
			gameObject.SetActive(value);
		}

        public void Update()
        {
        }
	}
}
