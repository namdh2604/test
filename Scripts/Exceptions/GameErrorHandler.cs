using System;
using System.Collections.Generic;

namespace Voltage.Witches.Exceptions
{
    using Voltage.Witches.Screens;
    using Voltage.Common.Logging;
    using Voltage.Witches.User;
    using Voltage.Witches.Resetter;
    using Voltage.Witches.Services;
    using UnityEngine;

	using Voltage.Witches.Net;
	using Voltage.Common.Net;

    public class GameErrorHandler
    {
        private readonly IScreenFactory _screenFactory;
        private readonly ScreenEnabler _screenEnabler;
        private readonly IPlayerWriter _playerAccess;
        private readonly IResetter _resetter;
        private readonly ErrorResolver _errorResolver;

		private readonly INetworkTimeoutController<WitchesRequestResponse> _networkController;
        private readonly IPlayerDataSerializer _serializer;

        private readonly IBuildNumberService _buildNumberService;

        public bool UseDebugMode { get; set; }

        public GameErrorHandler(IScreenFactory screenFactory, ScreenEnabler screenEnabler, IPlayerWriter playerAccess, 
            IResetter resetter, ErrorResolver errorResolver, 
            INetworkTimeoutController<WitchesRequestResponse> networkController, IPlayerDataSerializer serializer, IBuildNumberService buildNumberService)
        {
            _screenFactory = screenFactory;
            _screenEnabler = screenEnabler;
            _playerAccess = playerAccess;
            _resetter = resetter;
            _errorResolver = errorResolver;

			_networkController = networkController;
            _serializer = serializer;
            _buildNumberService = buildNumberService;

            UseDebugMode = false;
        }

        // determines whether or not to show unhandled errors to the user
        public static bool DisplayUnhandledErrors { get; set; }
        static GameErrorHandler()
        {
            DisplayUnhandledErrors = false;
        }

        public void HandleError(Exception e)
        {
            string exceptionText;
            if (UseDebugMode)
            {
                exceptionText = e.Message;
            }
            else
            {
                exceptionText = _errorResolver.GetUserFacingErrorMessage(e);
            }

            DisplayError(exceptionText, e.StackTrace);
            PostErrorReport(exceptionText, e.StackTrace);
        }

        private void DisplayError(string exceptionText, string stackTrace)
        {
            _screenEnabler.EnableInput(false);

            iGUISmartPrefab_ErrorDialog dialog = _screenFactory.GetOverlay<iGUISmartPrefab_ErrorDialog>();
            dialog.Init(GetUserID(), exceptionText);
            dialog._buttonHandler.OverrideDisableAll = true;
            dialog.Display(HandleUserResponse);

            if (!string.IsNullOrEmpty(stackTrace))
            {
                AmbientLogger.Current.Log(stackTrace, LogLevel.ERROR);
            }
        }

        private string GetUserID()
        {
            string userID = string.Empty;
            try
            {
                if (_playerAccess.HasExistingData)
                {
                    var playerData = _playerAccess.Load();
                    userID = playerData.userID;
                }
            }
            catch (Exception)
            {
                // swallow all exceptions so that there's a single return point
                // any exception trying to get the user ID just means there should be no user ID displayed
            }

            return userID;
        }

        private PlayerDataStore GetPlayerData()
        {
            PlayerDataStore data = null;

            try
            {
                if (_playerAccess.HasExistingData)
                {
                    data = _playerAccess.Load();        // Load() can return null
                }
            }
            catch (Exception)
            {
                // swallow all exceptions so that there's a single return point
                // any exception trying to get the user ID just means there should be no user ID displayed
            }

            return data;
        }

        private void HandleUserResponse(int choice)
        {
            ErrorDialogResponse response = (ErrorDialogResponse)choice;
            switch (response)
            {
                case ErrorDialogResponse.Reset:
                    _resetter.Reset();
                    _screenEnabler.EnableInput(true);
                    break;
                case ErrorDialogResponse.Support:
                    Application.OpenURL("https://voltageent.zendesk.com/home");
                    break;
                default:
                    AmbientLogger.Current.Log("Unknown Error Response", LogLevel.ERROR);
                    break;
            }
        }

        // Used as a unity callback when a log statement occurs for exceptions
        private void HandleException(string condition, string stackTrace, LogType type)
        {
            if (type != LogType.Exception)
            {
                return;
            }

            string text = _errorResolver.GetErrorText(condition, UseDebugMode);

            if (DisplayUnhandledErrors || UseDebugMode)
            {   
                DisplayError(text, stackTrace);
            }

            PostErrorReport(text, stackTrace);
        }

		private void PostErrorReport(string errorMsg, string stackTrace)
		{
            IDictionary<string,string> report = new Dictionary<string,string>() 
            {
                {"phone_id", string.Empty},
                {"build_version", _buildNumberService.GetBuildVersion()},
                {"playerjson", string.Empty},
                {"error_msg", errorMsg},
                {"stacktrace", stackTrace},

                // system details
                {"device_id", GetDeviceID()},
                {"device_model", GetDeviceModel()},
                {"device_os", GetOS()},
                {"device_system_mem", GetSystemMem().ToString()},
                {"device_graphics_mem", GetGraphicsMem().ToString()},
            };

            PlayerDataStore data = GetPlayerData();
            if (data != null)
            {
                report["phone_id"] = data.userID;
                report["playerjson"] = _serializer.Serialize(data);
            }

            _networkController.Send(URLs.ERROR_REPORT, report, (response) => AmbientLogger.Current.Log("Error reported successfully", LogLevel.INFO), (response) => AmbientLogger.Current.Log("Error report failed", LogLevel.WARNING));
		}

        // using UnityEngine calls out of convenience, could breakout and pass in as a separate component
        private string GetDeviceID()
        {
            return SystemInfo.deviceUniqueIdentifier;
        }
        private string GetDeviceModel()
        {
            return SystemInfo.deviceModel;
        }
        private string GetOS()
        {
            return SystemInfo.operatingSystem;
        }
        private int GetSystemMem()
        {
            return SystemInfo.systemMemorySize;
        }
        private int GetGraphicsMem()
        {
            return SystemInfo.graphicsMemorySize;
        }



        private static Application.LogCallback _callback;
        public static void RegisterHandler(GameErrorHandler handler)
        {
            _callback = handler.HandleException;
            Application.logMessageReceived += _callback;
        }

        public static void DeregisterHandler()
        {
            if (_callback != null)
            {
                Application.logMessageReceived -= _callback;
                _callback = null;
            }
        }
    }
}

