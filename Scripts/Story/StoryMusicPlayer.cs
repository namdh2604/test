using System.Collections.Generic;
using Voltage.Witches.Exceptions;

namespace Voltage.Witches.Story
{
    public class StoryMusicPlayer
    {
        private readonly IDictionary<string, string> _lookupTable;
        private readonly IAudioController _audioController;

        public StoryMusicPlayer(IDictionary<string, string> lookupTable, IAudioController audioController)
        {
            _lookupTable = lookupTable;
            _audioController = audioController;
        }

        public void Play(string rawName)
        {
            if (string.IsNullOrEmpty(rawName))
            {
                // Story wants to turn off the music
                _audioController.StopAndClearTrack();
            }
            else
            {
                if (!_lookupTable.ContainsKey(rawName))
                {
                    throw new WitchesException("No Music found corresponding to: " + rawName);
                }

                if (string.IsNullOrEmpty(_lookupTable[rawName]))
                {
                    _audioController.StopAndClearTrack();
                }

                string clipName = _lookupTable[rawName];
                if (_audioController.CurrentClip != clipName)
                {
                    _audioController.PlayBGMTrack(clipName);
                }
            }
        }
    }
}

