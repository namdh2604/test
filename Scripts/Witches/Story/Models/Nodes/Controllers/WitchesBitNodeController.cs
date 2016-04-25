using System;
using System.Collections.Generic;

namespace Voltage.Witches.Story.Models.Nodes.Controllers
{
	using Voltage.Common.Logging;

	using Voltage.Story.Models.Nodes;
	using Voltage.Story.Models.Nodes.Controllers;
	using Voltage.Story.StoryPlayer;
	using Voltage.Witches.Models;

	using Voltage.Common.Metrics;

    using Voltage.Witches.Data.Tutorial;

	public class WitchesBitNodeController : BaseNodeController
    {
		private readonly Player _player;

        // Maps scene path/name to the index of its event, if it is tracked.
        // This is not meant to contain all scenes
        private readonly Dictionary<string, int> _sceneMetricMap;

        public WitchesBitNodeController(Player player, ILogger logger) : base (logger)
		{
			if (player == null) 
			{
				throw new ArgumentNullException("WitchesBitNodeController::Ctor >>>");
			}

			_player = player;

            _sceneMetricMap = new Dictionary<string, int>() {
                { MainTutorial.TUTORIAL_SCENE_ONE, 1 },
                { MainTutorial.TUTORIAL_SCENE_TWO, 2 },
                { MainTutorial.TUTORIAL_SCENE_THREE, 3 }
            };

		}
			
		public override void Execute(INode node, IStoryPlayer storyPlayer)
		{
            // TODO: Remove. This is currently necessary because the server receives story updates on the deduct stamina calls.
            // The implementation looks very buggy and unreliable. Without this information, the scene complete event doesn't know what scene is being finished.
            // The cleanest fix is likely to send the completed scene ID with the complete scene call.
            _player.DeductStamina();
            _player.IncreaseBitProgress();
            LogSceneProgress();
            storyPlayer.Next();
		}

        private void LogSceneProgress()
        {
            if (_sceneMetricMap.ContainsKey(_player.CurrentScene))
            {
                int sceneIndex = _sceneMetricMap[_player.CurrentScene];
                string eventName = string.Format("scene{0:D2}_bit{1:D2}", sceneIndex, _player.CurrentBitProgress);
                AmbientMetricManager.Current.LogEvent(eventName);
            }
        }
    }
    
}
