using System;
using System.Collections.Generic;

namespace Voltage.Witches.DebugTool
{
	using Ninject;
	using Voltage.Witches.Models;
	using Voltage.Story.Configurations;

    public class DebugAccessor	// WIP
    {
		private static readonly DebugAccessor _instance = new DebugAccessor();
        public static DebugAccessor Instance { get { return _instance; } }

        private DebugAccessor () {}  
        static DebugAccessor () {}

		[Inject]
		public Player Player { get; set; }

		[Inject]
		public MasterStoryData StoryData { get; set; }
    }

}

