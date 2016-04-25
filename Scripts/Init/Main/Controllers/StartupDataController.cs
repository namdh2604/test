
using System;
using System.Collections.Generic;

namespace Voltage.Witches.DI
{
//	using Voltage.Common.Startup;
	using Voltage.Witches.User;
	using Voltage.Witches.Configuration;
	using Voltage.Story.Configurations;

	using Voltage.Witches.Exceptions;


	public class StartupDataController
    {
		public StartupData Data { get; private set; }

		private readonly PlayerDataController _playerDataController;
		private readonly ServerDataController _serverDataController;
		private readonly LocalDataController _localDataController;

		public StartupDataController(PlayerDataController playerDataController, ServerDataController serverDataController, LocalDataController localDataController)
		{
			if(playerDataController == null || serverDataController == null || localDataController == null)
			{
				throw new ArgumentNullException();
			}

			_playerDataController = playerDataController;
			_serverDataController = serverDataController;
			_localDataController = localDataController;
		}

		public void Execute(Action<Exception> callback)
		{
			_playerDataController.Execute((e) => OnDataReady(e, callback));
			_serverDataController.Execute((e) => OnDataReady(e, callback));
			_localDataController.Execute((e) => OnDataReady(e, callback));
		}

        public bool HasExistingPlayerData()
        {
            return _playerDataController.HasExistingData;
        }

		private void OnDataReady(Exception e, Action<Exception> callback)
		{
            if (e != null)
            {
                callback(e);
                return;
            }

            try
            {
    			if(_playerDataController.Data != null && _serverDataController.Data != null && _localDataController.Data != null)
    			{
    				_serverDataController.Data.MasterConfig.GameResources = _localDataController.Data.GameResources;

    				Data = new StartupData
    				{
    					PlayerData = _playerDataController.Data,
    					MasterConfig = _serverDataController.Data.MasterConfig,
    					GameResources = _localDataController.Data.GameResources,
    					StoryMain = _localDataController.Data.StoryMain,
    				};

                    callback(null);
    			}
            }
            catch (Exception ex)
            {
                callback(ex);
            }
		}


    }

	public class StartupData
	{
		public PlayerDataStore PlayerData { get; set; }

		public MasterConfiguration MasterConfig { get; set; }
		public MasterStoryData StoryMain { get; set; }
		public string GameResources { get; set; }
	}
    
}




