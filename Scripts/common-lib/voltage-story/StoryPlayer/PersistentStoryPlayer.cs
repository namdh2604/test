using System;
using System.Collections.Generic;

namespace Voltage.Story.StoryPlayer
{
    using Voltage.Common.Logging;
    using Voltage.Story.User;
    using Voltage.Story.Models.Nodes.Controllers;

    public class PersistentStoryPlayer : StoryPlayerNoHistory
    {
        protected IPlayer Player { get; private set; }

        public PersistentStoryPlayer(IPlayer player, ILogger logger, IDictionary<Type, INodeController> nodeControllers, Action onFinish=null) : base(logger, nodeControllers, onFinish)
        {
            Player = player;
        }

        public override void Next()
        {
            SaveProgress();
            base.Next();
        }

        public override void Next(int index)
        {
            SaveProgress();
            base.Next(index);
        }

        protected virtual void SaveProgress()
        {
            if (Player != null)
            {
                Player.UpdateSceneProgress(this.CurrentScene.Path, CurrentNode.ID);
                Player.Serialize();
            }
        }
    }
}

