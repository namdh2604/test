
using System;
using System.Collections.Generic;

namespace Voltage.Witches.DI
{
//	using Voltage.Common.Startup;
	using System.IO;
	using Voltage.Witches.User;


    public class PlayerDataController
    {
		public PlayerDataStore Data { get; private set; }	// not a fan

		private readonly NewPlayerFetcher _playerDataFetcher;
        private readonly IPlayerWriter _writer;

		private Action<Exception> _onComplete;

        public PlayerDataController(IPlayerWriter writer, NewPlayerFetcher playerDataFetcher)
		{
			if (writer == null || playerDataFetcher == null)
			{
				throw new ArgumentNullException();
			}

			_playerDataFetcher = playerDataFetcher;
            _writer = writer;
		}

		public void Execute(Action<Exception> callback)
		{
            try
            {
    			_onComplete = callback;

    			if(HasExistingData)
    			{
    				LoadPlayerData();
    			}
    			else
    			{
    				_playerDataFetcher.Fetch(OnFetchNewPlayerData);
    			}
            }
            catch (Exception e)
            {
                callback(e);
            }
		}

		private void OnFetchNewPlayerData(Exception e, PlayerDataStore data)
		{
            try
            {
                if (e != null)
                {
                    _onComplete(e);
                    return;
                }

                _writer.Save(data);
                LoadPlayerData();
                _onComplete(null);
            }
            catch (Exception ex)
            {
                _onComplete(ex);
            }
		}


		private void LoadPlayerData()
		{
            Data = _writer.Load();
		}


		public bool HasExistingData
		{
			get
			{
                return _writer.HasExistingData;
			}
		}
		

    }
    



}




