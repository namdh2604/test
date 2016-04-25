using Voltage.Story.StoryDivisions;

namespace Voltage.Witches.Models
{
	public enum SceneStatus
	{
		READABLE,
		COMPLETED,
		LOCKED,
	}

    public class SceneViewModel
    {
        public string Route {get; set; }
        public string Arc { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
		public string PolaroidPath { get; set; }
        public LockType LockStatus { get; set; }
		public int BitProgress { get; set; }
        public bool Completed { get; set; }

//        public SceneViewModel(string route, string arc, string name, string description, string polaroidPath, LockType lockStatus)
        public SceneViewModel(string route, string arc, string name, string description, string polaroidPath, LockType lockStatus, int bitProgress, bool completed)
        {
            Route = route;
            Arc = arc;
            Name = name;
            Description = description;
			PolaroidPath = polaroidPath;
            LockStatus = lockStatus;
			BitProgress = bitProgress;
            Completed = completed;
        }
    }
}

