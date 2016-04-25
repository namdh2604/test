using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

namespace Voltage.Witches.Screens
{
	using Voltage.Witches.Controllers;
	using Voltage.Witches.Screens;
	using Voltage.Witches.Models;
	using Voltage.Witches.Configuration;
	using Voltage.Witches.Shop;
	using Voltage.Witches.Events;



	public class HomeScreenFeatureLockHandler {

		private const string AVATAR_UNLOCK_SCENE = "Prologue/Prologue/Sister Sister";
		private const string CLOTHING_UNLOCK_SCENE = "Prologue/Prologue/Sister Sister";
		private const string MINI_GAME_UNLOCK_SCENE = "Prologue/Prologue/Sister Sister"; //HACK this should be "Prologue/Prologue/Research Redux" but needs to be unlocked until Minigame tutorial is ready

		public event Action<bool> HandleAvatarClosetLock;
		public event Action<bool> HandleClothingStoreLock;
		public event Action<bool> HandleMinigameLock;
        public event Action<bool> HandleStarterPackLock;
         

		private readonly Player _player;

		private readonly StarterPackEvaluator _starterPackEval;


        public HomeScreenFeatureLockHandler(Player player, StarterPackEvaluator starterPackEval)
		{
            if (player == null || starterPackEval == null) 
            {
				throw new ArgumentNullException ("HomeScreenFeatureLockHandler::Ctor");
			}
			_player = player;
            _starterPackEval = starterPackEval;
		}

		public void HandleLocks()
		{
			bool isClosetUnlocked = IsSceneToUnlock(AVATAR_UNLOCK_SCENE);
			bool isClothingStoreUnlocked = IsSceneToUnlock(CLOTHING_UNLOCK_SCENE);
			bool isMiniGameUnlocked = IsSceneToUnlock(MINI_GAME_UNLOCK_SCENE);

			HandleAvatarCloset (isClosetUnlocked);
			HandleClothingStore (isClothingStoreUnlocked);
			HandleMiniGame (isMiniGameUnlocked);

            HandleStarterPackLock(IsStarterBundleUnlocked());
		}

		private bool IsSceneToUnlock(string sceneName)
		{
			IList<string> sceneHistory = new List<string>(_player.CompletedScenes);

			if (_player.CompletedRouteCount > 0) 
			{
				return true;
			}

			if (!string.IsNullOrEmpty(_player.CurrentScene))
			{
				sceneHistory.Add(_player.CurrentScene);
			}
			else
			{
				Voltage.Common.Logging.AmbientLogger.Current.Log("Player::DeductStaminaEnabled >>> CurrentScene is null!", Voltage.Common.Logging.LogLevel.DEBUG);
			}
			return sceneHistory.Contains(sceneName); // if scene name found, it's time to unlock
		}

        private bool IsStarterBundleUnlocked()
        {
			return _starterPackEval.IsAvailable();
        }

		// handle unlock buttons
		private void HandleAvatarCloset(bool value)
		{
			if (HandleAvatarClosetLock != null) 
			{
				HandleAvatarClosetLock (value);
			}
		}

		private void HandleClothingStore(bool value)
		{
			if (HandleClothingStoreLock != null)
			{
				HandleClothingStoreLock(value);
			}
		}

		private void HandleMiniGame(bool value)
		{
			if (HandleMinigameLock != null)
			{
				HandleMinigameLock(value);
			}
		}
	}
}
