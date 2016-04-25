using System;

namespace Voltage.Witches.Story
{
    using Voltage.Story.StoryDivisions;
    using Voltage.Story.StoryPlayer;

    public class LoadStatus
    {
        public LockType lockType;
        public bool needsStamina;
        public StoryPlayerSettings settings;
        public SceneHeader header;

        public LoadStatus(LockType lockType, SceneHeader header, StoryPlayerSettings settings)
        {
            this.lockType = lockType;
            this.header = header;
            this.settings = settings;
        }

        public bool IsReady()
        {
            return (lockType == LockType.None);
        }
    }
}

